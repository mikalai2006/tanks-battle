// using UnityEngine;
// using UnityEngine.InputSystem;

// public class PlayerController3D : MonoBehaviour
// {
//     private GameManager _gameManager = GameManager.Instance;
//     [SerializeField] private InputActionReference moveActionToUse;
//     [SerializeField] private GameObject _basa;
//     [SerializeField] private GameObject _machine;
//     [SerializeField] Camera _camera;

//     void Awake()
//     {
//         moveActionToUse.action.Enable();
//     }

//     void Start()
//     {
//         _camera = GameObject.FindGameObjectWithTag("CameraGame").GetComponent<Camera>();
//     }

//     void Update()
//     {
//         // захватываем значения джойстика перемещения.
//         // Vector2 moveDirection = moveActionToUse.action.ReadValue<Vector2>();
//         Vector2 moveDirection = _machine.LevelManager.JoystickMove.Direction;

//         // if (_machine.Badge != null)
//         // {
//         //     _machine.Badge.OnSetNameText(moveDirection.ToString());
//         // }

//         if (moveDirection != Vector2.zero)
//         {
//             _machine.Move(moveDirection);
//         }
//         else
//         {
//             _machine.Stop();
//         }

//         // захватываем позицию мыши или джойстика управления башней.
//         if (!_machine.MachineLevelData.isBot && !_gameManager.Settings.autoTakeEnemy)
//         {
// #if android
//         // android.
//         Vector3 direction = _machine.LevelManager.JoystickTower.Direction;
// #endif

// #if webgl
//         // WEBGL.
//         Vector3 positionMouse = Mouse.current.position.ReadValue();
//         positionMouse.z = _camera.transform.position.z; //_camera.farClipPlane * .5f;;
//         Vector3 worldPoint = _camera.ScreenToWorldPoint(positionMouse);
//         // Calculate the direction vector from the object to the mouse
//         Vector3 direction = worldPoint - transform.position;
//         // Debug.Log($"angle = {angle}, worldPoint= {worldPoint}, positionMouse= {positionMouse}");
// #endif

//             if (direction != Vector3.zero)
//             {
//                 // Calculate the angle in degrees
//                 float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

//                 // Debug.Log($"angle = {angle}, direction= {direction}");
//                 for (int i = 0; i < _machine.Towers.Count; i++)
//                 {   
//                     _machine.Towers[i].OnSetAngleTower(angle);
//                 }
//             }
//         }
//     }

// }
