using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private GameManager _gameManager => GameManager.Instance;
    [SerializeField] private InputActionReference moveActionToUse;
    // [SerializeField] public InputActionAsset playerInputActions;
    [SerializeField] private InputActionReference attackActionInput;
    // [SerializeField] private InputAction doubleTapAction;
    [SerializeField] private BaseMachine _machine;
    [SerializeField] Camera _camera;
    [SerializeField] Camera _cameraFPS;
    [SerializeField] Vector3 moveDirection;
    [SerializeField] Vector3 rotateDirection;
    Camera Camera => _camera.gameObject.activeSelf == true ? _camera : _cameraFPS;

    private float speedRotateCameraDiff;
    
    [Header("Настройки зажатой кнопки стрельбы")]
    [SerializeField] private bool holdShot;
    
    [Header("Настройки двойного клика")]
    private float lastClickTime = 0f;
    private bool firstClickDetected = false;

    void Awake()
    {
        _machine = GetComponent<BaseMachine>();

        // doubleTapAction = playerInputActions.FindActionMap("Player").FindAction("DoubleTap");
    }

    void OnEnable()
    {
        moveActionToUse.action.Enable();
        attackActionInput.action.Enable();

        // doubleTapAction.Enable();
    }

    void OnDisable()
    {
        moveActionToUse.action.Disable();
        attackActionInput.action.Disable();

        // doubleTapAction.Disable();
    }

    void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("CameraGame").GetComponent<Camera>();
        _cameraFPS = _machine.Camera;
        
        // захват клика мыши для стрельбы.
        if (!_machine.MachineLevelData.isBot)
        {
            if (_gameManager.Settings.DebugSettings.mode == AppMode.Desktop)
            {
                attackActionInput.action.started += EnableShot;
                attackActionInput.action.canceled += DisableShot;
            }
            // else
            // {
            //     doubleTapAction.performed += OnDoubleTapPerformed;
            // }
        }

        // вешаем событие на нажатие правого джойстика.
        JoystickController.RunPointerDown += OnPointerDownRightJoystick;
    }

    void OnDestroy()
    {
        attackActionInput.action.started -= EnableShot;
        attackActionInput.action.canceled -= DisableShot;

        // doubleTapAction.performed -= OnDoubleTapPerformed;

        
        JoystickController.RunPointerDown -= OnPointerDownRightJoystick;
    }

    void Update()
    {
        // захватываем значения джойстика перемещения.
        moveDirection = Vector2.zero;

        if (_gameManager.Settings.DebugSettings.mode == AppMode.Mobile)
        {
            moveDirection = _machine.LevelManager.JoystickMove.Direction;
        }
        else
        {
            moveDirection = moveActionToUse.action.ReadValue<Vector2>();
        }

        // if (moveDirection != Vector3.zero)
        // {
        //     // Debug.Log($"moveDirection = {moveDirection}");
        //     moveDirection.Normalize();
        // }

        if (_gameManager.Settings.autoTakeEnemy)
        {
            _machine.LevelManager.cinemachineOrbitalFollow.Radius = 0.6f;
            _machine.LevelManager.cinemachineOrbitalFollow.TargetOffset = new Vector3(0, 3, 0);
            _machine.LevelManager.cinemachineCamera.Lens = new Unity.Cinemachine.LensSettings
            {
                FieldOfView = 30f,
                NearClipPlane = 0.1f,
                FarClipPlane = 5000,
                Dutch = 0,
                ModeOverride = Unity.Cinemachine.LensSettings.OverrideModes.None,
            };
        }

        // захватываем позицию мыши или джойстика управления башней.
        if (!_machine.MachineLevelData.isBot) // && !_gameManager.Settings.autoTakeEnemy
        {
            rotateDirection = Vector3.zero;

            if (_gameManager.Settings.DebugSettings.mode == AppMode.Mobile)
            {

                // добавляем время удержания джойстика.
                if (_machine.LevelManager.JoystickTower.TimeTouch > 0) {
                    _machine.LevelManager.JoystickTower.AddTimeTouch(Time.deltaTime);
                }

                // android.
                rotateDirection = _machine.LevelManager.JoystickTower.Direction;
                rotateDirection.z = rotateDirection.y * _gameManager.Settings.playerOptions.speedRotateCamera.y * _machine.LevelManager.JoystickTower.TimeTouch;
                rotateDirection.y = 0;
                rotateDirection.x = rotateDirection.x * _gameManager.Settings.playerOptions.speedRotateCamera.x * _machine.LevelManager.JoystickTower.TimeTouch;

                
                        // android.
                        if (_machine.LevelManager.cinemachineOrbitalFollow != null)
                        {
                            if (_machine.LevelManager.cinemachineOrbitalFollow.HorizontalAxis.Value == -180)
                            {
                                _machine.LevelManager.cinemachineOrbitalFollow.HorizontalAxis.Value = 180;
                            } else if (_machine.LevelManager.cinemachineOrbitalFollow.HorizontalAxis.Value == 180)
                            {
                                _machine.LevelManager.cinemachineOrbitalFollow.HorizontalAxis.Value = -180;
                            }
                            _machine.LevelManager.cinemachineOrbitalFollow.HorizontalAxis.Value = Mathf.Clamp(
                                _machine.LevelManager.cinemachineOrbitalFollow.HorizontalAxis.Value + rotateDirection.x,
                                -180,
                                180
                            );
                            _machine.LevelManager.cinemachineOrbitalFollow.VerticalAxis.Value = Mathf.Clamp(
                                _machine.LevelManager.cinemachineOrbitalFollow.VerticalAxis.Value - rotateDirection.z,
                                -10,
                                25
                            );
                        }
            }
        }


        // Если зажата кнопка - стреляем
        if (holdShot)
        {
            _machine.OnShot();
        }
    }

    void FixedUpdate()
    {
        // if (_machine.Badge != null)
        // {
        //     _machine.Badge.OnSetNameText(moveDirection.ToString());
        // }

        if (moveDirection != Vector3.zero)
        {
            // if (moveDirection.x > 0.5f || moveDirection.x < -0.5f)
            // {
            //     _machine.Rotate(moveDirection);
            // }
            // else
            // {
            //     _machine.Rotate(Vector3.zero);
            // }

            _machine.Move(moveDirection);
        }
        else
        {
            // if (_machine.IsMove)
            // {
            // }
            _machine.Stop();
        }

        // обработка вращения башни.
        if (!_machine.MachineLevelData.isBot && !_gameManager.Settings.autoTakeEnemy && rotateDirection != Vector3.zero)
        {
            if (_gameManager.Settings.DebugSettings.mode == AppMode.Mobile)
            {
                // // android.
                // if (_machine.levelManager.cinemachineOrbitalFollow != null)
                // {
                //     if (_machine.levelManager.cinemachineOrbitalFollow.HorizontalAxis.Value == -180)
                //     {
                //         _machine.levelManager.cinemachineOrbitalFollow.HorizontalAxis.Value = 180;
                //     } else if (_machine.levelManager.cinemachineOrbitalFollow.HorizontalAxis.Value == 180)
                //     {
                //         _machine.levelManager.cinemachineOrbitalFollow.HorizontalAxis.Value = -180;
                //     }
                //     _machine.levelManager.cinemachineOrbitalFollow.HorizontalAxis.Value = Mathf.Clamp(
                //         _machine.levelManager.cinemachineOrbitalFollow.HorizontalAxis.Value + rotateDirection.x,
                //         -180,
                //         180
                //     );
                //     _machine.levelManager.cinemachineOrbitalFollow.VerticalAxis.Value = Mathf.Clamp(
                //         _machine.levelManager.cinemachineOrbitalFollow.VerticalAxis.Value - rotateDirection.z,
                //         -10,
                //         25
                //     );
                // }
                // _machine.levelManager.UiTopSide.OnChangeCrossPosition(direction);

                // // Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera, _machine.levelManager.UiTopSide.CrossObjectTransform.position);
                // // Vector3 worldPosition = Camera.ScreenToWorldPoint(_machine.levelManager.UiTopSide.CrossObjectTransform.position);
                // // Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera, _machine.levelManager.UiTopSide.CrossObjectTransform.position);

                // // Vector3 worldPoint;
                // // RectTransformUtility.ScreenPointToWorldPointInRectangle(_machine.levelManager.UiTopSide.CrossObjectTransform, screenPoint, Camera, out worldPoint);

                // Ray ray = Camera.ScreenPointToRay(_machine.levelManager.UiTopSide.CrossObjectTransform.position);
                // if (Physics.Raycast(ray, out RaycastHit hit))
                // {
                //     Vector3 targetPosition = hit.point;
                //     directionRotation = targetPosition - transform.position;
                //     //direction.y = 0;
                //     Debug.DrawRay(Camera.transform.position, directionRotation);
                //     // Debug.Log($"Camera name: {Camera.name}, POINT={hit.point}");

                // }

                // // Vector3 directionToTarget = worldPoint - transform.position;
                // // Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                // // Debug.Log($"targetRotation={targetRotation.eulerAngles}, worldPoint={worldPoint}");
            }
            // else
            // {
            //     // WEBGL.
            //     Vector3 positionMouse = Mouse.current.position.ReadValue();

            //     // positionMouse.z = Camera.transform.position.z;

            //     Vector3 positionMouseWorld = Camera.ScreenToWorldPoint(positionMouse);
            //     // Debug.Log($"positionMouseWorld = {positionMouseWorld}, positionMouse={positionMouse}");

            //     _machine.levelManager.UiTopSide.OnSetCrossPosition(positionMouse);

            //     // Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera, _machine.levelManager.UiTopSide.CrossObjectTransform.position);

            //     // Vector3 worldPoint;
            //     // RectTransformUtility.ScreenPointToWorldPointInRectangle(_machine.levelManager.UiTopSide.CrossObjectTransform, screenPoint, Camera, out worldPoint);

            //     Ray ray = Camera.ScreenPointToRay(positionMouse);
            //     // positionMouse.z = transform.position.z - _camera.transform.position.z; //_camera.farClipPlane * .5f;;
            //     // Vector3 worldPoint = _camera.ScreenToWorldPoint(positionMouse);
            //     // // Calculate the direction vector from the object to the mouse
            //     // direction = worldPoint - transform.position;
            //     // Debug.Log($"worldPoint= {worldPoint}, positionMouse= {positionMouse}, direction={direction}");
            //     if (Physics.Raycast(ray, out RaycastHit hit))
            //     {
            //         Vector3 targetPosition = hit.point;
            //         direction = targetPosition - transform.position;
            //         //direction.y = 0;
            //         Debug.DrawRay(Camera.transform.position, direction, Color.magenta);
            //         // Debug.Log($"Camera name: {Camera.name}, hit name={hit.collider.name}, POINT={hit.point}");
            //     }
            // }

            // if (direction != Vector3.zero)
            // {
            //     Debug.Log($"direction={direction}");
            //     // // Calculate the angle in degrees
            //     // float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            //     Quaternion lookRotation = Quaternion.LookRotation(direction);


            //     // Debug.Log($"angle = {angle}, direction= {direction}");
            //     for (int i = 0; i < _machine.Towers.Count; i++)
            //     {
            //         // _machine.Towers[i].OnSetAngleTower(angle);
            //         _machine.Towers[i].OnSetAngleTower(lookRotation.eulerAngles.y);
            //     }
            // }
            // if (rotateDirection != Vector3.zero)
            // {
            //     // Debug.Log($"direction={direction}");
            //     // // Calculate the angle in degrees
            //     // float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            //     // Quaternion lookRotation = Quaternion.LookRotation(direction);


            //     // Debug.Log($"angle = {angle}, direction= {direction}");
            //     for (int i = 0; i < _machine.Towers.Count; i++)
            //     {
            //         _machine.Towers[i].OnSetAngleTower(rotateDirection);
            //     }
            // }
        }
    }
    

    private void OnPointerDownRightJoystick(PointerEventData eventData)
    {
        if (firstClickDetected && Time.time - lastClickTime < _gameManager.Settings.playerOptions.doubleClickThreshold)
        {
            // Обнаружен двойной щелчок.

            // запускаем функции.
            _machine.OnShot();

            firstClickDetected = false; // Сброс для следующего потенциального двойного щелчка
        }
        else
        {
            // Обнаружен первый щелчок, запущен таймер
            firstClickDetected = true;
            lastClickTime = Time.time;
        }
        
        // Если был обнаружен первый щелчок, но порог двойного щелчка превышен
        if (firstClickDetected && Time.time - lastClickTime >= _gameManager.Settings.playerOptions.doubleClickThreshold)
        {
            // Обнаружен одиночный щелчок (если необходимо дифференцировать)
            
            firstClickDetected = false; // Сброс
        }
    }



    public void EnableShot(InputAction.CallbackContext context)
    {
        holdShot = true;
    }

    public void DisableShot(InputAction.CallbackContext context)
    {
        holdShot = false;
    }

    // private void OnDoubleTapPerformed(InputAction.CallbackContext context)
    // {
    //     Debug.Log("Double Tap Detected!");
    //     // Add your double-tap specific logic here
    //     _machine.OnShot();
    // }
}
