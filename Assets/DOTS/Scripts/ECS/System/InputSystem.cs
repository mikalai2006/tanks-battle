using Unity.Entities;
using UnityEngine;

public partial class InputSystem : SystemBase
{
    private InputSystem_Actions controls;
    protected override void OnCreate()
    {
        if (!SystemAPI.TryGetSingleton<InputComponent>(out InputComponent input))
        {
            EntityManager.CreateEntity(typeof(InputComponent));
        }

        controls = new InputSystem_Actions();
        controls.Enable();
    }
    protected override void OnUpdate()
    {
        Vector2 moveVector = controls.Player.Move.ReadValue<Vector2>();
        Vector2 mousePosition = controls.Player.Move.ReadValue<Vector2>();

        SystemAPI.SetSingleton(new InputComponent
        {
            movemement = moveVector,
            mousePos = mousePosition
        });
    }
}
