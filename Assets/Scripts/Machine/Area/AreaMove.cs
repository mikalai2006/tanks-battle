using UnityEngine;
using UnityEngine.Tilemaps;

public class AreaMove : MonoBehaviour
{
    [SerializeField] BaseMachine machine;
    public Collider2D Collider;

    // void OnTriggerEnter2D(Collider2D collider)
    // {
    //     Debug.Log($"Collised:");
    //     if (machine.stateController != null && machine.stateController.enabled)
    //     {
    //         Debug.Log($"Collised:  {collider.gameObject.name}, controller={machine.stateController != null}");
    //         var _baseMachine = collider.gameObject.GetComponentInParent<BaseMachine>();

    //         if (_baseMachine != null)
    //         {

    //             // machine.stateController.patrolState.OnSetEnemy(_baseMachine);
    //             machine.stateController.patrolState.OnSetObstacle(collider.gameObject);
    //             // machine.stateController.ChangeState(machine.stateController.chaseState);
    //         }
    //         else
    //         {
    //             machine.stateController.patrolState.OnSetObstacle(collider.gameObject);
    //         }
    //     }
    // }

    void Awake()
    {
        machine = GetComponentInParent<BaseMachine>();
    }


    void OnCollisionStay2D(Collision2D collision)
    {
        if (machine.stateController != null && machine.stateController.enabled)
        {
            // Debug.Log($"Collised1:  {collision.collider.gameObject.name}, controller={machine.stateController != null}, {collision.collider.GetType()}");

            Vector3 pos = Vector3.zero;

            Tilemap tm = collision.collider.GetComponent<Tilemap>();
            if (tm != null)
            {
                for (int i =0; i< collision.contactCount; i++)
                {
                    var contact = collision.contacts[i];
                    pos = tm.layoutGrid.WorldToCell(contact.point);
                    // var go = tm.GetTile(tilePosition);
                    // Debug.Log($"Tilemap Point: {contact.point}/{pos}");
                }
            }
            else
            {
                pos = collision.collider.gameObject.transform.position;
            }

            var _baseMachine = collision.collider.gameObject.GetComponentInParent<BaseMachine>();

            if (_baseMachine != null)
            {
                // Debug.Log("Collised2:  baseMachine");

                // machine.stateController.patrolState.OnSetEnemy(_baseMachine);
                machine.stateController.patrolState.OnSetObstacle(collision.collider.gameObject.transform.position);
                // machine.stateController.ChangeState(machine.stateController.chaseState);
                
            }
            else
            {
                // Debug.Log("Collised2:  other");

                if (pos != Vector3.zero)
                {
                    machine.stateController.patrolState.OnSetObstacle(pos);
                }
            }
        }
    }

    // void OnTriggerExit2D(Collider2D collider)
    // {
    //     if (machine.stateController)
    //     {
    //         // machine.stateController.ChangeState(machine.stateController.patrolState);
    //         // machine.stateController.patrolState.OnSetEnemy(null);
    //     }
    // }
}
