using UnityEngine;

public class BaseBullet : MonoBehaviour
{
    GameManager _gameManager => GameManager.Instance;
    BaseMachine Machine;
    GameMuzzle ConfigMuzzle;
    public float moveSpeed = 5f;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] private Vector3 toPoint;
    [SerializeField] private Vector3 forward;

    /// <summary>
    /// Инициализация снаряда
    /// </summary>
    /// <param name="machine">Машина которая произвела снаряд</param>
    public void OnInit(BaseMachine machine, GameMuzzle confgiMuzzle)
    {
        sprite = GetComponent<SpriteRenderer>();

        Machine = machine;

        ConfigMuzzle = confgiMuzzle;

        sprite.material = confgiMuzzle.material;

        moveSpeed = confgiMuzzle.Bullet.speed;

        // Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, Machine.Data.directionTower);
        // transform.rotation = Quaternion.Euler(0, 0, lookRotation.eulerAngles.z);
        // toPoint = transform.position + towerDir * Machine.Config.distanceAttack;


        // определяем фактический угол поворота башни
        // и устанавливаем его для снаряда
        var direction = Vector3.right;
        float angle = Machine.Tower.transform.eulerAngles.z * Mathf.Deg2Rad;
        float sin = Mathf.Sin(angle);
        float cos = Mathf.Cos(angle);
        forward = new Vector3(
            direction.x * cos - direction.y * sin,
            direction.x * sin + direction.y * cos,
            0f
        );

        transform.eulerAngles = new Vector3(0,0,Machine.Tower.transform.eulerAngles.z);

        Vector3 pos = transform.position;
        toPoint = pos + forward * Machine.Tower.DistanceAttack;
#if UNITY_EDITOR
        if (_gameManager.Settings.drawLineAttack) {
            Debug.DrawLine(pos, forward, Color.magenta);
            Debug.DrawLine(pos, toPoint, Color.green, 1);
            // Debug.Log($"position = {pos}, toPoint = {toPoint}");
        }
#endif
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
        var newPosition = Vector2.MoveTowards(transform.position, toPoint, ConfigMuzzle.Bullet.speed * Time.deltaTime);
        rb.MovePosition(newPosition);

        // Update the object's position
        // transform.position = newPosition;
        if (transform.position == toPoint)
        {
            OnBoom(null);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("TilemapWithCollider"))
        {
            OnBoom(null);
            // Debug.Log($"Collision border to point {transform.position}");
        }
        else
        {
            AreaMove areaMoveObject = collision.gameObject.GetComponent<AreaMove>();
            if (areaMoveObject != null && Machine != null && Machine.AreaMove != areaMoveObject)
            {
                BaseMachine enemy = collision.gameObject.GetComponentInParent<BaseMachine>();
                if (enemy != null)
                {
                    // Debug.Log($"Collision enemy - {enemy.gameObject}");
                    OnBoom(enemy);
                }
            }

            BaseBullet otherBullet = collision.gameObject.GetComponent<BaseBullet>();
            if (otherBullet != null)
            {
                OnBoom(null);
            }
        }
    }
}
