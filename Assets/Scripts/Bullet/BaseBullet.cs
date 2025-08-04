using System.Linq;
using Mikalai2006.Voxel;
using UnityEngine;

public class BaseBullet : MonoBehaviour
{
    GameManager _gameManager => GameManager.Instance;
    BaseMachine Machine;
    GameMuzzle ConfigMuzzle;
    public float moveSpeed = 5f;
    // [SerializeField] private SpriteRenderer sprite;
    [SerializeField] Rigidbody rb;
    [SerializeField] private Vector3 toPoint;
    // [SerializeField] private Vector3 forward;

    /// <summary>
    /// Инициализация снаряда
    /// </summary>
    /// <param name="machine">Машина которая произвела снаряд</param>
    public void OnInit(BaseMachine machine, BaseTower Tower, GameMuzzle confgiMuzzle)
    {
        // sprite = GetComponent<SpriteRenderer>();

        Machine = machine;

        ConfigMuzzle = confgiMuzzle;

        // sprite.material = confgiMuzzle.material;

        moveSpeed = confgiMuzzle.Bullet.speed;

        // Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, Machine.Data.directionTower);
        // transform.rotation = Quaternion.Euler(0, 0, lookRotation.eulerAngles.z);
        // toPoint = transform.position + towerDir * Machine.Config.distanceAttack;


        // определяем фактический угол поворота башни
        // и устанавливаем его для снаряда
        var direction = Tower.Data.directionTower;
        // float angle = Tower.Data.angleTower;  //Tower.transform.eulerAngles.y * Mathf.Deg2Rad;
        // Debug.Log($"direction={direction}, angle={angle}");
        // float sin = Mathf.Sin(angle);
        // float cos = Mathf.Cos(angle);
        // forward = new Vector3(
        //     direction.x * cos - direction.z * sin,
        //     0f,
        //     direction.x * sin + direction.z * cos
        // );

        transform.localEulerAngles = new Vector3(0,Tower.transform.localEulerAngles.y,0);

        // Rotate the direction vector
        // Quaternion rotation = Quaternion.Euler(0, Tower.Data.angleTower, 0); // Rotate around the Y-axis
        // Vector3 rotatedDirection = rotation * direction;

        // Calculate the new position
        float distance = Tower.DistanceAttack;
        var angle = Tower.Data.currentAngleTower;
        var xOffset = distance * Mathf.Cos(Mathf.Deg2Rad * angle);
        var zOffset = distance * Mathf.Sin(Mathf.Deg2Rad * angle);
        
        Vector3 pos = transform.position;
        toPoint = pos + Tower.transform.forward * distance; // new Vector3(xOffset, 0, zOffset);

#if UNITY_EDITOR
        if (_gameManager.Settings.drawLineAttack)
        {
            // Debug.DrawLine(pos, direction, Color.magenta,2);
            Debug.DrawLine(pos, toPoint, Color.green, 2);
            Debug.Log($"toPoint={toPoint}");
            // Debug.Log($"position = {pos}, toPoint = {toPoint}");
        }
#endif
        rb.AddForce(Tower.transform.forward * 1000 * 6, ForceMode.Impulse);
    }

    /// <summary>
    /// Функция взрыва снаряда
    /// </summary>
    /// <param name="_targetMachine">Игровой объект, на котором произошел взрыв</param>
    public void OnBoom(BaseMachine _targetMachine)
    {
        if (_targetMachine)
        {
            _targetMachine.OnAddDamage(ConfigMuzzle.Bullet.damage);

            if (!_targetMachine.MachineLevelData.isBot || !Machine.MachineLevelData.isBot)
            {
                // Создаем текст с уроном
                TextDamage obText = Lean.Pool.LeanPool.Spawn(_gameManager.Settings.prefabTextDamage, _targetMachine.LevelManager.objectSpawnText.transform);
                
                if (obText)
                {
                    obText.Init(_targetMachine, true);
                    obText.OnSetColor(_gameManager.Settings.colorTextDamage);
                    obText.OnSetText(string.Concat("-", ConfigMuzzle.Bullet.damage.ToString()));
                }
                // _targetMachine.OnDrawAnimateText();
            }
        }
        else if (Machine && ConfigMuzzle.Bullet.effectBoom)
        {
            var obj = Lean.Pool.LeanPool.Spawn(ConfigMuzzle.Bullet.effectBoom, Machine.LevelManager.objectSpawnEffect.transform, false);
            obj.transform.localPosition = transform.position;
            obj.isStatic = true;
        }

        if (Machine && ConfigMuzzle.Bullet.particleBoom)
        {
            var objParticle = Lean.Pool.LeanPool.Spawn(ConfigMuzzle.Bullet.particleBoom, Machine.LevelManager.objectSpawnEffect.transform);
            ParticleSystem[] particles = objParticle.transform.GetChild(0).GetComponentsInChildren<ParticleSystem>();
            if (particles.Length > 0)
            {
                for (int i = 0; i < particles.Length; i++)
                {
                    var main = particles[i].main;
                    var rend = particles[i].GetComponent<ParticleSystemRenderer>();
                    if (rend)
                    {
                        rend.materials[0] = ConfigMuzzle.material;
                        rend.material = ConfigMuzzle.material; //gameObject.GetComponent<MeshRenderer>().material;
                    }
                    else
                    {
                        Debug.Log($"not found material");
                    }
                }
            }
            objParticle.transform.position = transform.position;
            Lean.Pool.LeanPool.Despawn(objParticle, 2);
        }

        // Lean.Pool.LeanPool.Despawn(this);
        Destroy(gameObject);
    }

    void Update()
    {
        // rb.linearVelocity = towerDir * moveSpeed;
        // var newPosition = Vector2.MoveTowards(transform.position, toPoint, Time.deltaTime);
        // rb.MovePosition(newPosition);

        // Update the object's position
        // transform.position = newPosition;
        if (transform.position == toPoint)
        {
            OnBoom(null);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("IgnoreCollision"))
        {
            return;
        }

        Debug.Log($"Collision: {collision.gameObject.name}");
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
            Debug.Log($"collision.gameObject={collision.gameObject.name}, position={collision.gameObject.transform.position}");
            Vector3 localPoint = collision.gameObject.transform.InverseTransformPoint(contact.point);
            Debug.Log($"point = {contact.point}, localPositionPoint={localPoint}");
            Container voxelContainer = collision.gameObject.GetComponent<Container>();
            if (voxelContainer == null)
            {
                BaseMachine bm = collision.gameObject.GetComponent<BaseMachine>();
                if (bm)
                {
                voxelContainer = bm.Body.GetComponentInChildren<Container>();
                }
            }

            if (voxelContainer != null)
            {
                var listVoxels = voxelContainer.ExposionVoxels(localPoint, true, collision);
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
        
        Destroy(gameObject);

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
}
