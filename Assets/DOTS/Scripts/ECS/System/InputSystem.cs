// using System;
// using Unity.Entities;
// using UnityEngine;
// using UnityEngine.InputSystem;

// public partial class InputSystem : SystemBase
// {
//     private InputSystem_Actions controls;
//     protected override void OnCreate()
//     {
//         if (!SystemAPI.TryGetSingleton<InputComponent>(out InputComponent input))
//         {
//             EntityManager.CreateEntity(typeof(InputComponent));
//         }

//         controls = new InputSystem_Actions();

//         // .performed += OnClickPerformed;

//         controls.Enable();
//     }

//     // private void OnClickPerformed(InputAction.CallbackContext context)
//     // {
//     //     Debug.Log("Mouse button clicked!");
//     // }

//     protected override void OnUpdate()
//     {
//         Vector2 moveVector = controls.Player.Move.ReadValue<Vector2>();
//         Vector2 mousePosition = controls.Player.Move.ReadValue<Vector2>();
//         float pressingLMB = controls.Player.Attack.ReadValue<float>();

//         SystemAPI.SetSingleton(new InputComponent
//         {
//             movemement = moveVector,
//             mousePos = mousePosition,
//             pressingLMB = pressingLMB,
//         });
//     }
// }
