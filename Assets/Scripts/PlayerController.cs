using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference moveActionToUse;
    [SerializeField] private BaseMachine _machine;

    void Awake()
    {
        _machine = GetComponent<BaseMachine>();
        moveActionToUse.action.Enable();
    }

    void Update()
    {
        Vector2 moveDirection = moveActionToUse.action.ReadValue<Vector2>();

        // if (_machine.Badge != null)
        // {
        //     _machine.Badge.OnSetNameText(moveDirection.ToString());
        // }

        if (moveDirection != Vector2.zero)
        {
            _machine.Move(moveDirection);
        }
        else
        {
            _machine.Stop();
        }

    }

}
