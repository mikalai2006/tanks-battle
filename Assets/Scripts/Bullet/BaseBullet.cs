using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mikalai2006.Voxel;
using UnityEngine;
using Random = UnityEngine.Random;
public class BaseBullet : MonoBehaviour
{
    GameManager _gameManager => GameManager.Instance;
    BaseMachine Machine;
    GameMuzzle ConfigMuzzle;
    BaseMuzzle Muzzle;
    [SerializeField] Rigidbody rb;
    [SerializeField] BoxCollider boxCollider;
    Vector3 forward;
    int countCollisions;
    [SerializeField] float lifeTime;
    [SerializeField] bool isActive;
    TrailRenderer trailRenderer;
    // MeshRenderer meshRenderer;
    List<GameObject> collisionsObjects = new();
    [SerializeField] private LayerMask ignoreMask;
    Vector3 startPosition;
    Vector3 endPosition;
    float hitDistance = 0;
    float currentDistance = 0;
    RaycastHit hit;
    float chanceReflex = 0.8f;
    public GPUInstanceEnabler gPUInstanceEnabler;
    [SerializeField] private GameObject trail;
    // private MeshFilter meshFilter;
    public VoxelMeshRender voxelMeshRender;

#region Unity methods
    void Awake()
    {
        boxCollider = transform.GetComponent<BoxCollider>();
        // meshRenderer = transform.GetComponentInChildren<MeshRenderer>();
        trailRenderer = transform.GetComponentInChildren<TrailRenderer>();
        // meshFilter = transform.GetComponentInChildren<MeshFilter>();
        voxelMeshRender = transform.GetComponentInChildren<VoxelMeshRender>();
    }


    void Update()
    {
        if (!isActive) return;
        
        lifeTime += Time.deltaTime;
        if (lifeTime > ConfigMuzzle.Bullet.lifeTime)
        {
            OnBoom(null);
        }
        
        if (endPosition != Vector3.zero)
        {
            currentDistance = Vector3.Distance(transform.position, endPosition);
            // Debug.Log($"currentDistance={currentDistance} [{hitDistance}]<{Vector3.Distance(transform.position, startPosition)}>");
            if (currentDistance <= 1)
            {
                OnCreateContact(hit);
            }
        }
    }

    void FixedUpdate()
    {
        if (!isActive) return;

        // if (rb.isKinematic)
        // {
        //     Vector3 targetPosition = rb.position + (forward * Muzzle.Data.speedBullet * 100f * Time.fixedDeltaTime); // Example: moving forward
        //     rb.MovePosition(targetPosition);
        // }
    }

    
    void OnCollisionEnter(Collision collision)
    {
        // Debug.Log($"<color=green>OnCollisionEnter {collision.gameObject.name}[detect object => {gameObject.name}]</color>");

        if (collision.gameObject.CompareTag("IgnoreCollision"))
        {
            return;
        }

        var ra = Random.value;
        if (ra > chanceReflex)
        {
            Debug.Log($"<color=#7ccf00>РИКОШЕТ</color>");
            return;
        }

        if (!collisionsObjects.Contains(collision.gameObject))
        {
            if (!Machine.MachineLevelData.isBot)
                    {
            Debug.Log($"<color=green>OnCollisionEnter {collision.gameObject.name}[detect object => {gameObject.name}]</color>");
                    }
            // Debug.Log($"<color=yellow>================Colission=====================================</color>");
            // foreach (ContactPoint contact in collision.contacts)
            // {
            //     // Access the collision point's position
            //     Vector3 collisionPoint = contact.point;

            //     // You can also access the normal at the collision point
            //     Vector3 collisionNormal = contact.normal;

            //     // Do something with the collision point and normal
            //     Debug.Log("Collision point: " + collisionPoint + ", Normal: " + collisionNormal);

            //     Machine.levelManager.WorldManager.Container.ExposionVoxels(collisionPoint);
            // }

            for (int q = 0; q < 1; q++)
            {
                var contact = collision.contacts[q];
                // Debug.Log($"collision.gameObject={collision.gameObject.name}, position={collision.transform.position}");
                Vector3 localPoint = collision.gameObject.transform.InverseTransformPoint(contact.point);

                // Debug.Log($"point = {contact.point}, localPositionPoint={localPoint}");

                // VoxelMeshRenderWithSubmeshes voxelMesh = collision.gameObject.GetComponent<VoxelMeshRenderWithSubmeshes>();
                // if (voxelMesh != null)
                // {
                //     collisionsObjects.Add(collision.transform.gameObject);
                //     voxelMesh.ExposionVoxels(localPoint, true, collision.gameObject, ConfigMuzzle.Bullet.damageRadius).Forget();
                // }

                List<Container> collisionContainers = new List<Container>();
                Container voxelContainer = collision.gameObject.GetComponent<Container>();
                if (voxelContainer == null)
                {
                    BaseMachine bm = collision.gameObject.GetComponent<BaseMachine>();
                    if (bm)
                    {
                        collisionsObjects.Add(collision.transform.gameObject);
                        bm.OnCollision(contact.point, true, collision.gameObject, ConfigMuzzle.Bullet.damageRadius, forward, hit.normal);
                        // collisionContainers.AddRange(bm.Body.GetComponentsInChildren<Container>());
                        // collisionContainers.AddRange(bm.Towers[0].GetComponentsInChildren<Container>());
                    }
                }
                else
                {
                    if (voxelContainer.IsDestructible())
                    {
                        collisionContainers.Add(voxelContainer);
                    }
                }

                if (collisionContainers.Count > 0)
                {
                    collisionsObjects.Add(collision.transform.gameObject);
                    // Debug.Log($"collisionContainers.Count={collisionContainers.Count}");
                    for (int x = 0; x < collisionContainers.Count; x++)
                    {

                        collisionContainers[x].ExposionVoxels(localPoint, true, collision.gameObject, ConfigMuzzle.Bullet.damageRadius, forward, hit.normal).Forget();
                        // for (int t=0; t < 100; t++) {

                        //     float forceMagnitude = 3 * 100;
                        //     // GameObject gObj = Instantiate(GameManager.Instance.Settings.prefabVoxel, Machine.levelManager.objectSpawnEffect.transform);
                        //     GameObject gObj = Lean.Pool.LeanPool.Spawn(GameManager.Instance.Settings.prefabVoxel, Machine.LevelManager.objectSpawnEffect.transform);
                        //     Vector3 pointSpawnVoxel = new Vector3(0,0,0);
                        //     gObj.transform.SetPositionAndRotation(pointSpawnVoxel, Quaternion.identity);
                        //     // gObj.transform.SetLocalPositionAndRotation(listVoxels.ElementAt(k).Key, Quaternion.identity);
                        //     // gObj.gameObject.AddComponent<BoxCollider>();

                        //     var mat = gObj.gameObject.GetComponent<MeshRenderer>().material;
                        //     var mesh = gObj.gameObject.GetComponent<MeshFilter>().mesh;
                        //     RenderParams _rp = new RenderParams(mat);
                        //     Graphics.RenderMesh(_rp, mesh, 0, Matrix4x4.Translate(pointSpawnVoxel));   

                        //     var r = gObj.gameObject.GetComponent<Rigidbody>();
                        //     if (r == null)
                        //     {
                        //         r = gObj.gameObject.AddComponent<Rigidbody>();
                        //     }
                        //     r.mass = 50f;
                        //     r.useGravity = true;
                        //     var forceDirection = Vector3.Scale(UnityEngine.Random.onUnitSphere, transform.forward);
                        //     r.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
                        //     // gObj.isStatic = false;
                        //     // Destroy(gObj, 15);
                        //     Lean.Pool.LeanPool.Despawn(gObj, 15);
                        // }
                    }
                }

                // Debug.Log($"================/Colission=====================================");
            }
        }

        if (collisionsObjects.Count >= ConfigMuzzle.Bullet.countCollisions)
            {
                OnBoom(null);
            }
            
        // Machine.levelManager.WorldManager.Container.ExposionVoxels(contact.point, true);
        // Destroy(gameObject);
        // if (collision.gameObject.CompareTag("TilemapWithCollider"))
        // {
        //     OnBoom(null);
        //     // Debug.Log($"Collision border to point {transform.position}");
        // }
        // else
        // {
        //     AreaMove areaMoveObject = collision.gameObject.GetComponent<AreaMove>();
        //     if (areaMoveObject != null && Machine != null && Machine.AreaMove != areaMoveObject)
        //     {
        //         BaseMachine enemy = collision.gameObject.GetComponentInParent<BaseMachine>();
        //         if (enemy != null)
        //         {
        //             // Debug.Log($"Collision enemy - {enemy.gameObject}");
        //             OnBoom(enemy);
        //         }
        //     }

        //     BaseBullet otherBullet = collision.gameObject.GetComponent<BaseBullet>();
        //     if (otherBullet != null)
        //     {
        //         OnBoom(null);
        //     }
        // }

        // Debug.Log($"collision {collision.collider.name}, point={collision.collide}");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"<color=green>OnTriggerEnter {other.name}[detect object => {gameObject.name}]</color>");
        if (!collisionsObjects.Contains(other.gameObject))
        {
            // Debug.Log($"<color=green>OnTriggerEnter {other.name}</color>");

            // Получаем позицию другого объекта
            // Vector3 otherObjectPosition = other.transform.position;
            // Debug.Log("Объект вошел в триггер с позицией: " + otherObjectPosition);

            // Определяем точку касания на поверхности нашего триггера
            Vector3 contactPoint = other.ClosestPoint(transform.position);
            // Debug.Log("Точка касания на триггере: " + contactPoint);

            Vector3 localPoint = other.gameObject.transform.InverseTransformPoint(contactPoint);
            // Debug.Log($"point = {contactPoint}, localPositionPoint={localPoint}");

            // VoxelMeshRenderWithSubmeshes voxelMesh = other.GetComponent<VoxelMeshRenderWithSubmeshes>();
            // if (voxelMesh != null)
            // {
            //     voxelMesh.ExposionVoxels(localPoint, true, other.gameObject, ConfigMuzzle.Bullet.damageRadius).Forget();
            // }

            List<Container> collisionContainers = new List<Container>();
            Container voxelContainer = other.gameObject.GetComponent<Container>();
            if (voxelContainer != null && voxelContainer.IsDestructible())
            {
                collisionContainers.Add(voxelContainer);
            }

            if (collisionContainers.Count > 0)
            {
                collisionsObjects.Add(other.gameObject);

                for (int x = 0; x < collisionContainers.Count; x++)
                {

                    collisionContainers[x].ExposionVoxels(localPoint, true, other.gameObject, ConfigMuzzle.Bullet.damageRadius,forward, hit.normal).Forget();
                }
            }


            Debug.Log($"<color=green>Count collision ={countCollisions}</color>");
            if (collisionsObjects.Count >= ConfigMuzzle.Bullet.countCollisions)
            {
                OnBoom(null);
            }
        }
    }
    
#endregion

    /// <summary>
    /// Инициализация снаряда
    /// </summary>
    /// <param name="machine">Машина которая произвела снаряд</param>
    public void OnInit(BaseMachine machine, BaseTower Tower, BaseMuzzle muzzle, GameMuzzle configMuzzle)
    {
        hit = default;
        startPosition = Vector3.zero;
        endPosition = Vector3.zero;
        currentDistance = 0;
        hitDistance = 0;

        forward = new Vector3(muzzle.transform.forward.x, muzzle.transform.forward.y, muzzle.transform.forward.z);

        // Quaternion targetRotation = Quaternion.LookRotation(forward);
        // transform.rotation = targetRotation;
        // trail.transform.rotation = targetRotation;

        transform.localScale = new Vector3(_gameManager.Settings.scaleObjects, _gameManager.Settings.scaleObjects, _gameManager.Settings.scaleObjects);

        // boxCollider.size = new Vector3(5,5,5);

        rb.position = transform.position;

        lifeTime = 0;

        collisionsObjects.Clear();
        // sprite = GetComponent<SpriteRenderer>();

        Machine = machine;

        ConfigMuzzle = configMuzzle;
        
        voxelMeshRender.OnSetConfigMeshGenerator(ConfigMuzzle.Bullet.MeshConfig);

        Muzzle = muzzle;

        if (trailRenderer)
        {
            trailRenderer.material = ConfigMuzzle.Bullet.material;
        }
        
        // if (meshFilter)
        // {
        //     Color32[] colors = new Color32[meshFilter.sharedMesh.vertices.Length];
        //     for (int i = 0; i < colors.Length; i++)
        //     {
        //         colors[i] = Color.red; // Set all vertices to red
        //         // Or, for example, assign colors based on vertex position or other criteria
        //         // colors[i] = new Color(mesh.vertices[i].x, mesh.vertices[i].y, mesh.vertices[i].z, 1f);
        //     }
        //     meshFilter.mesh.colors32 = colors;
        // }
        // meshRenderer.material = ConfigMuzzle.Bullet.material;

        // if (gPUInstanceEnabler)
        // {
        //     gPUInstanceEnabler.SetColor(Color.red);
        // }
        // #if UNITY_EDITOR
        //         if (_gameManager.Settings.drawLineAttack)
        //         {
        //             // Debug.DrawLine(pos, direction, Color.magenta,2);
        //             Debug.DrawRay(transform.position, muzzle.transform.forward * 5, Color.blue, 2);
        //             // Debug.Log($"position = {pos}, toPoint = {toPoint}");
        //         }
        // #endif

        // _muzzle = muzzle;


        //         // sprite.material = confgiMuzzle.material;

        //         // moveSpeed = confgiMuzzle.Bullet.speed;

        //         // Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, Machine.Data.directionTower);
        //         // transform.rotation = Quaternion.Euler(0, 0, lookRotation.eulerAngles.z);
        //         // toPoint = transform.position + towerDir * Machine.Config.distanceAttack;


        //         // определяем фактический угол поворота башни
        //         // и устанавливаем его для снаряда
        //         var direction = Tower.Data.directionTower;
        //         // float angle = Tower.Data.angleTower;  //Tower.transform.eulerAngles.y * Mathf.Deg2Rad;
        //         // Debug.Log($"direction={direction}, angle={angle}");
        //         // float sin = Mathf.Sin(angle);
        //         // float cos = Mathf.Cos(angle);
        //         // forward = new Vector3(
        //         //     direction.x * cos - direction.z * sin,
        //         //     0f,
        //         //     direction.x * sin + direction.z * cos
        //         // );

        //         transform.localEulerAngles = new Vector3(muzzle.transform.localEulerAngles.x, Tower.transform.localEulerAngles.y, 0);

        //         // Rotate the direction vector
        //         // Quaternion rotation = Quaternion.Euler(0, Tower.Data.angleTower, 0); // Rotate around the Y-axis
        //         // Vector3 rotatedDirection = rotation * direction;

        //         // Calculate the new position
        //         float distance = Tower.DistanceAttack;
        //         // var angle = Tower.Data.currentAngleTower;
        //         // var xOffset = distance * Mathf.Cos(Mathf.Deg2Rad * angle);
        //         // var zOffset = distance * Mathf.Sin(Mathf.Deg2Rad * angle);

        //         Vector3 pos = transform.position;
        //         toPoint = pos + transform.forward * distance; // new Vector3(xOffset, 0, zOffset);

        // #if UNITY_EDITOR
        //         if (_gameManager.Settings.drawLineAttack)
        //         {
        //             // Debug.DrawLine(pos, direction, Color.magenta,2);
        //             Debug.DrawLine(pos, toPoint, Color.green, 2);
        //             Debug.Log($"toPoint={toPoint}");
        //             // Debug.Log($"position = {pos}, toPoint = {toPoint}");
        //         }
        // #endif
        
        var ra = Random.value;
        if (ra < chanceReflex)
        {
            if (Physics.Raycast(transform.position, forward, out hit, Muzzle.Data.distanceAttack, ~(ignoreMask)))
            {
                Container voxelContainer = hit.collider.gameObject.GetComponent<Container>();
                if (voxelContainer != null && voxelContainer.IsDestructible())
                {
                    startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                    endPosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                    hitDistance = Vector3.Distance(startPosition, endPosition);
                    if (!Machine.MachineLevelData.isBot)
                    {
                        Debug.Log($"<color=#FFA500FF>Raycast ударил в объект {hit.collider.name}<{hit.point}>[distance={hitDistance}]</color>");
                    }
                } else
                {
                    if (!Machine.MachineLevelData.isBot)
                    {
                        Debug.Log($"<color=#7ccf00>Raycast ударил в объект {hit.collider.name}<{hit.point}> без Container или он не разрушаемый</color>");
                    }
                }

                // Vector3 localPoint = hit.collider.gameObject.transform.parent.InverseTransformPoint(hit.point);

                // Vector3 worldPointCollisionWithOffset = hit.point + forward;
                // Vector3 localPoint2 = hit.collider.gameObject.transform.parent.InverseTransformPoint(worldPointCollisionWithOffset);
                // Debug.Log($"point = {hit.point}, localPositionPoint={localPoint}, worldPointCollisionWithOffset={worldPointCollisionWithOffset}, localPoint2={localPoint2}");

            }
            // Debug.DrawRay(startPosition, forward * ConfigMuzzle.distanceAttack, Color.white, 3);
        } else
        {
            Debug.Log($"<color=#7ccf00>РИКОШЕТ</color>");
        }


        if (!rb.isKinematic)
        {
            Vector3 speedForce = forward * Muzzle.Data.speedBullet * 100;
            rb.AddForce(speedForce, ForceMode.Acceleration);
        }
        isActive = true;
    }

    private void OnCreateContact(RaycastHit hit)
    {
        if (hit.collider != null)
        {
            Debug.Log($"<color=magenta>Count collision ={hit.collider.name}</color>");

            if (!collisionsObjects.Contains(hit.collider.gameObject))
            {
                var contact = hit;
                // Debug.Log($"collision.gameObject={collision.gameObject.name}, position={collision.transform.position}");
                Vector3 localPoint = hit.collider.gameObject.transform.InverseTransformPoint(contact.point);

                // Debug.Log($"point = {contact.point}, localPositionPoint={localPoint}");

                // VoxelMeshRenderWithSubmeshes voxelMesh = hit.collider.gameObject.GetComponent<VoxelMeshRenderWithSubmeshes>();
                // if (voxelMesh != null)
                // {
                //     collisionsObjects.Add(hit.collider.transform.gameObject);
                //     voxelMesh.ExposionVoxels(localPoint, true, hit.collider.gameObject, ConfigMuzzle.Bullet.damageRadius).Forget();
                // }

                List<Container> collisionContainers = new List<Container>();
                Container voxelContainer = hit.collider.gameObject.GetComponent<Container>();
                if (voxelContainer != null)
                {
                    collisionContainers.Add(voxelContainer);
                }

                if (collisionContainers.Count > 0)
                {
                    collisionsObjects.Add(hit.collider.transform.gameObject);
                    // Debug.Log($"collisionContainers.Count={collisionContainers.Count}");
                    for (int x = 0; x < collisionContainers.Count; x++)
                    {

                        collisionContainers[x].ExposionVoxels(localPoint, true, hit.collider.gameObject, ConfigMuzzle.Bullet.damageRadius, forward, hit.normal).Forget();
                        // for (int t=0; t < 100; t++) {

                        //     float forceMagnitude = 3 * 100;
                        //     // GameObject gObj = Instantiate(GameManager.Instance.Settings.prefabVoxel, Machine.levelManager.objectSpawnEffect.transform);
                        //     GameObject gObj = Lean.Pool.LeanPool.Spawn(GameManager.Instance.Settings.prefabVoxel, Machine.LevelManager.objectSpawnEffect.transform);
                        //     Vector3 pointSpawnVoxel = new Vector3(0,0,0);
                        //     gObj.transform.SetPositionAndRotation(pointSpawnVoxel, Quaternion.identity);
                        //     // gObj.transform.SetLocalPositionAndRotation(listVoxels.ElementAt(k).Key, Quaternion.identity);
                        //     // gObj.gameObject.AddComponent<BoxCollider>();

                        //     var mat = gObj.gameObject.GetComponent<MeshRenderer>().material;
                        //     var mesh = gObj.gameObject.GetComponent<MeshFilter>().mesh;
                        //     RenderParams _rp = new RenderParams(mat);
                        //     Graphics.RenderMesh(_rp, mesh, 0, Matrix4x4.Translate(pointSpawnVoxel));   

                        //     var r = gObj.gameObject.GetComponent<Rigidbody>();
                        //     if (r == null)
                        //     {
                        //         r = gObj.gameObject.AddComponent<Rigidbody>();
                        //     }
                        //     r.mass = 50f;
                        //     r.useGravity = true;
                        //     var forceDirection = Vector3.Scale(UnityEngine.Random.onUnitSphere, transform.forward);
                        //     r.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
                        //     // gObj.isStatic = false;
                        //     // Destroy(gObj, 15);
                        //     Lean.Pool.LeanPool.Despawn(gObj, 15);
                        // }
                    }
                }

                // Debug.Log($"================/Colission=====================================");
            }

            if (collisionsObjects.Count >= ConfigMuzzle.Bullet.countCollisions)
            {
                OnBoom(null);
            }
        }
    }


    /// <summary>
    /// Функция взрыва снаряда
    /// </summary>
    /// <param name="_targetMachine">Игровой объект, на котором произошел взрыв</param>
    public void OnBoom(BaseMachine _targetMachine)
    {
        if (trailRenderer)
        {
            trailRenderer.Clear();
        }


        collisionsObjects.Clear();
        lifeTime = 0;
        isActive = false;
        // if (_targetMachine)
        // {
        //     _targetMachine.OnAddDamage(ConfigMuzzle.Bullet.damage);

        //     if (!_targetMachine.MachineLevelData.isBot || !Machine.MachineLevelData.isBot)
        //     {
        //         // Создаем текст с уроном
        //         TextDamage obText = Lean.Pool.LeanPool.Spawn(_gameManager.Settings.prefabTextDamage, _targetMachine.LevelManager.objectSpawnText.transform);

        //         if (obText)
        //         {
        //             obText.Init(_targetMachine, true);
        //             obText.OnSetColor(_gameManager.Settings.colorTextDamage);
        //             obText.OnSetText(string.Concat("-", ConfigMuzzle.Bullet.damage.ToString()));
        //         }
        //         // _targetMachine.OnDrawAnimateText();
        //     }
        // }
        // else if (Machine && ConfigMuzzle.Bullet.effectBoom)
        // {
        //     var obj = Lean.Pool.LeanPool.Spawn(ConfigMuzzle.Bullet.effectBoom, Machine.LevelManager.objectSpawnEffect.transform, false);
        //     obj.transform.localPosition = transform.position;
        //     obj.isStatic = true;
        // }

        // if (Machine && ConfigMuzzle.Bullet.particleBoom)
        // {
        //     var objParticle = Lean.Pool.LeanPool.Spawn(ConfigMuzzle.Bullet.particleBoom, Machine.LevelManager.objectSpawnEffect.transform);
        //     ParticleSystem[] particles = objParticle.transform.GetChild(0).GetComponentsInChildren<ParticleSystem>();
        //     if (particles.Length > 0)
        //     {
        //         for (int i = 0; i < particles.Length; i++)
        //         {
        //             var main = particles[i].main;
        //             var rend = particles[i].GetComponent<ParticleSystemRenderer>();
        //             if (rend)
        //             {
        //                 rend.materials[0] = ConfigMuzzle.material;
        //                 rend.material = ConfigMuzzle.material; //gameObject.GetComponent<MeshRenderer>().material;
        //             }
        //             else
        //             {
        //                 Debug.Log($"not found material");
        //             }
        //         }
        //     }
        //     objParticle.transform.position = transform.position;
        //     Lean.Pool.LeanPool.Despawn(objParticle, 2);
        // }


        // Lean.Pool.LeanPool.Despawn(transform.gameObject);
        // rb.linearVelocity = Vector3.zero;
        // rb.angularVelocity = Vector3.zero;
        // Destroy(gameObject);
        Machine.LevelManager.poolBullet.ReturnObject(gameObject);
    }


    // void OnTriggerStay(Collider other)
    // {
    //     if (!collisionsObjects.Contains(other.gameObject))
    //     {
    //         Debug.Log($"<color=magenta>OnTriggerStay {other.name}</color>");

    //         // Получаем позицию другого объекта
    //         // Vector3 otherObjectPosition = other.transform.position;
    //         // Debug.Log("Объект вошел в триггер с позицией: " + otherObjectPosition);

    //         // Определяем точку касания на поверхности нашего триггера
    //         Vector3 contactPoint = other.ClosestPoint(transform.position);
    //         // Debug.Log("Точка касания на триггере: " + contactPoint);

    //         Vector3 localPoint = other.gameObject.transform.InverseTransformPoint(contactPoint);
    //         // Debug.Log($"point = {contactPoint}, localPositionPoint={localPoint}");

    //         VoxelMeshRenderWithSubmeshes voxelMesh = other.GetComponent<VoxelMeshRenderWithSubmeshes>();
    //         if (voxelMesh != null)
    //         {
    //             voxelMesh.ExposionVoxels(localPoint, true, other.gameObject, ConfigMuzzle.Bullet.damageRadius).Forget();
    //         }

    //         List<Container> collisionContainers = new List<Container>();
    //         Container voxelContainer = other.gameObject.GetComponent<Container>();
    //         if (voxelContainer != null)
    //         {
    //             collisionContainers.Add(voxelContainer);
    //         }

    //         if (collisionContainers.Count > 0)
    //         {
    //             collisionsObjects.Add(other.gameObject);

    //             for (int x = 0; x < collisionContainers.Count; x++)
    //             {

    //                 collisionContainers[x].ExposionVoxels(localPoint, true, other.gameObject, ConfigMuzzle.Bullet.damageRadius, forward).Forget();
    //             }
    //         }


    //         Debug.Log($"<color=green>Count collision ={countCollisions}</color>");
    //         if (collisionsObjects.Count >= ConfigMuzzle.Bullet.countCollisions)
    //         {
    //             OnBoom(null);
    //         }
    //     }
    // }
}
