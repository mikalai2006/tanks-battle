using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    public InputAction pressEscape; // Assign this in the Inspector

    void Start()
    {
        // Блокируем курсор в центре экрана и скрываем его
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnEnable()
    {
        pressEscape.Enable();
        pressEscape.performed += OnJumpPerformed; // Subscribe to the performed event
    }

    void OnDisable()
    {
        pressEscape.Disable();
        pressEscape.performed -= OnJumpPerformed; // Unsubscribe
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    // void Update()
    // {
    //     // Для выхода из блокировки можно использовать, например, клавишу Escape
    //     if (Keyboard.current.escapeKey.isPressed)
    //     {
    //         Cursor.lockState = CursorLockMode.None;
    //     }
    // }
}