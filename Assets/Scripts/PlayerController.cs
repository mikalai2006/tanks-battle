using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class PlayerController : MonoBehaviour
{
    private GameManager _gameManager = GameManager.Instance;
    [SerializeField] private InputActionReference moveActionToUse;
    [SerializeField] private BaseMachine _machine;
    [SerializeField] Camera _camera;

    void Awake()
    {
        _machine = GetComponent<BaseMachine>();
        moveActionToUse.action.Enable();
    }

    void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("CameraGame").GetComponent<Camera>();

    }

    void Update()
    {
        // захватываем значения джойстика перемещения.
        Vector2 moveDirection = Vector2.zero;
        if (_gameManager.Settings.inputJoystick)
        {
            moveDirection = _machine.LevelManager.JoystickMove.Direction;
        }
        else
        {
            moveDirection = moveActionToUse.action.ReadValue<Vector2>();
        }
        moveDirection.Normalize();
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
            if (_machine.IsMove)
            {
                _machine.Stop();
            }
        }

        // захватываем позицию мыши или джойстика управления башней.
        if (!_machine.MachineLevelData.isBot && !_gameManager.Settings.autoTakeEnemy)
        {
            Vector3 direction = Vector3.zero;

            if (_gameManager.Settings.inputJoystick)
            {
                // android.
                direction = _machine.LevelManager.JoystickTower.Direction;
            }
            else
            {
                // WEBGL.
                Vector3 positionMouse = Mouse.current.position.ReadValue();
                Ray ray = _camera.ScreenPointToRay(positionMouse);
                // positionMouse.z = transform.position.z - _camera.transform.position.z; //_camera.farClipPlane * .5f;;
                // Vector3 worldPoint = _camera.ScreenToWorldPoint(positionMouse);
                // // Calculate the direction vector from the object to the mouse
                // direction = worldPoint - transform.position;
                // Debug.Log($"worldPoint= {worldPoint}, positionMouse= {positionMouse}, direction={direction}");
                if (Physics.Raycast(ray, out RaycastHit hit, 200f))
                {
                    Vector3 targetPosition = hit.point;
                    direction = targetPosition - transform.position;
                    direction.y = 0;
                    Debug.DrawRay(targetPosition, direction);

                }
            }

            // Debug.Log($"direction={direction}");
            if (direction != Vector3.zero)
            {
                // // Calculate the angle in degrees
                // float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
                Quaternion lookRotation = Quaternion.LookRotation(direction);


                // Debug.Log($"angle = {angle}, direction= {direction}");
                for (int i = 0; i < _machine.Towers.Count; i++)
                {
                    // _machine.Towers[i].OnSetAngleTower(angle);
                    _machine.Towers[i].OnSetAngleTower(lookRotation.eulerAngles.y);
                }
            }
        }
    }

}
