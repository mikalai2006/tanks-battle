using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionReference moveActionToUse;
    private Entity _playerInputEntity; // Reference to the singleton entity

    void Awake()
    {
        moveActionToUse.action.performed += ctx => UpdateMoveInput(ctx.ReadValue<Vector2>());
        // moveActionToUse.action.performed += ctx => UpdateFireInput(true);
    }

    void OnEnable() { moveActionToUse.action.Enable(); }
    void OnDisable() { moveActionToUse.action.Disable(); }

    // This would be called by an ECS system or during conversion
    public void SetPlayerInputEntity(Entity entity)
    {
        _playerInputEntity = entity;
    }

    private void UpdateMoveInput(Vector2 moveDirection)
    {
        if (_playerInputEntity != Entity.Null)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            entityManager.SetComponentData(_playerInputEntity, new InputComponent { movemement = moveDirection });
        }
    }

    // private void UpdateFireInput(bool isFiring)
    // {
    //     if (_playerInputEntity != Entity.Null)
    //     {
    //         var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    //         entityManager.SetComponentData(_playerInputEntity, new InputComponent { movemement = isFiring });
    //     }
    // }
}
