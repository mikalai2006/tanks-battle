using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AreaAttack : MonoBehaviour
{
    GameManager _gameManager = GameManager.Instance;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] BaseMachine machine;
    [SerializeField] Dictionary<BaseMachine, float> targets;
    public Dictionary<BaseMachine, float> Targets => targets;
    public List<BaseMachine> testTargets;

    void Awake()
    {
        targets = new();

        sprite = GetComponent<SpriteRenderer>();
        machine = GetComponentInParent<BaseMachine>();
    }

    public void OnSetColor(Color color)
    {
        sprite.color = color;
    }

    public void OnSetSize(float size)
    {
        transform.localScale = new Vector3(size, size, size);
    }

    private void OnAddMachine(BaseMachine _machine)
    {
         if (_machine != null)
        {
            // float distance = Vector2.Distance(machine.transform.position, _machine.transform.position);
            if (!targets.ContainsKey(_machine)) //  && distance <= machine.Config.distanceAttack - 1
            {
                targets.Add(_machine, 0);
            }
        }
    }


    private void OnRemoveMachine(BaseMachine _machine)
    {
        if (_machine != null)
        {
            if (targets.ContainsKey(_machine))
            {
                targets[_machine] = 0;
                targets.Remove(_machine);
            }
        }
    }

    void Update()
    {
        for (int i = targets.Count - 1; i >= 0; i--)
        {
            if (targets.ElementAt(i).Key == null)
            {
                targets.Remove(targets.ElementAt(i).Key);
            }
        }

        for (int i = 0; i < targets.Count; i++)
        {
            targets[targets.ElementAt(i).Key] += Time.deltaTime;
        }

        // Test.
        testTargets = Targets.Keys.ToList();
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        var _baseMachine = collider.GetComponentInParent<BaseMachine>();

        if (_baseMachine != null)
        {
            var isColliderTarget = collider.GetComponent<AreaAttack>();

            // if (_baseMachine.AreaMove != collider && _baseMachine.AreaAttack != collider)
            if (_baseMachine != machine && !isColliderTarget)
            {
                // float offsetRay = _baseMachine.AreaMove.transform.localScale.x;
                var direction = _baseMachine.transform.position - transform.position;
                Vector3 startRay = transform.position;// + offsetRay * direction.normalized;
                RaycastHit2D[] hits = Physics2D.RaycastAll(startRay, direction, machine.Config.distanceAttack);

                bool isObstacle = false;
                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit2D hit = hits[i];

                    AreaAttack isColliderAreaAttack = hit.collider.GetComponent<AreaAttack>();
                    AreaMove isColliderAreaMove = hit.collider.GetComponent<AreaMove>();
                    BaseBullet isBullet = hit.collider.GetComponent<BaseBullet>();
                    // игнорируем снаряды
                    if (isBullet)
                    {
                        continue;
                    }
                    // игнорируем проверку зоны атаки
                    if (isColliderAreaAttack)
                    {
                        continue;
                    }
                    // игнорируем свою зону поиска
                    if (isColliderAreaMove == machine.AreaMove)
                    {
                        continue;
                    }
                    // если есть препятствие тайл, выходим и устанавливаем препятствие
                    if (hit.collider.CompareTag("TilemapWithCollider"))
                    {
                        isObstacle = true;
                        break;
                    }
                    // если нашли коллайдер который искали выходим
                    if (_baseMachine.AreaMove == isColliderAreaMove)
                    {
                        break;
                    }
                    // иначе если коллайдер другой машины, устанавливаем его как препятствие
                    else
                    {
                        isObstacle = true;
                        break;
                    }
                }
                if (!isObstacle)
                {
                    Debug.DrawRay(startRay, direction, Color.green);
                    OnAddMachine(_baseMachine);
                }
                else
                {
                    Debug.DrawRay(startRay, direction, Color.red);
                    OnRemoveMachine(_baseMachine);
                }
                // Tilemap tm = hit.collider.GetComponent<Tilemap>();
                // if (tm != null) {
                //     Debug.Log($"Point2: {hit.point}");
                // }

                // if (hit.collider)
                // {
                //     BaseMachine bm = hit.collider.GetComponentInParent<BaseMachine>();
                //     AreaAttack isColliderAreaAttack = collider.GetComponent<AreaAttack>();
                //     AreaMove isColliderMyAreaMove = collider.GetComponent<AreaMove>();

                //     if (bm && !isColliderAreaAttack)
                //     {
                //         Debug.DrawRay(startRay, direction, Color.green);
                //         Debug.Log($"Trigger2:  baseMachine {_baseMachine.name}/ {hit}");
                //     }
                // }
                // // machine.stateController.patrolState.OnSetEnemy(_baseMachine);
                // machine.stateController.patrolState.OnSetObstacle(collision.collider.gameObject.transform.position);
                // // machine.stateController.ChangeState(machine.stateController.chaseState);
            }

        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        var _baseMachine = collider.GetComponentInParent<BaseMachine>();
        OnRemoveMachine(_baseMachine);
    }
}
