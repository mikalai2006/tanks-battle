using UnityEngine;
using UnityEngine.InputSystem;

public class CameraHandler : MonoBehaviour
{
    private GameManager _gameManager = GameManager.Instance;
    public Transform camTrans;
    public Transform pivot;
    public BaseMachine Character;
    public Transform mTransform;

    public CameraConfig cameraConfig;
    public GameObject TargetLook;

    public bool leftPivot;
    public float delta;

    public float mouseX;
    public float mouseY;
    public float smoothX;
    public float smoothY;
    public float smoothXVelocity;
    public float smoothYVelocity;
    public float lookAngle;
    public float titlAngle;


    void Update()
    {
        FixedTick();
    }

    void FixedTick()
    {
        delta = Time.deltaTime;

        HandlePosition();
        HandleRotation();

        Vector3 targetPosition = Vector3.Lerp(mTransform.position, Character.transform.position, 1);
        mTransform.position = targetPosition;
    }

    public void OnSetCharacter(BaseMachine machine)
    {
        Character = machine;
    }

    void HandlePosition()
    {
        float targetX = cameraConfig.normalX;
        float targetY = cameraConfig.normalY;
        float targetZ = cameraConfig.normalZ;

        if (leftPivot)
        {
            targetX = -targetX;
        }

        Vector3 newPivotPosition = pivot.localPosition;
        newPivotPosition.x = targetX;
        newPivotPosition.y = targetY;

        Vector3 newCameraPosition = camTrans.localPosition;
        newCameraPosition.z = targetZ;

        float t = delta * cameraConfig.pivotSpeed;
        pivot.localPosition = Vector3.Lerp(pivot.localPosition, newPivotPosition, t);
        camTrans.localPosition = Vector3.Lerp(camTrans.localPosition, newCameraPosition, t);
    }

    void HandleRotation()
    {
        var mousePosition = Vector2.zero;
        if (_gameManager.Settings.inputJoystick == true)
        {
            mousePosition = Character.LevelManager.JoystickTower.Direction;
        }
        else
        {
            mousePosition = Mouse.current.delta.ReadValue();
        }

        
        if (mousePosition == Vector2.zero)
            {
                return;
            }

        mouseX = mousePosition.x;
        mouseY = mousePosition.y;

        if (cameraConfig.turnSmooth > 0)
        {
            smoothX = Mathf.SmoothDamp(smoothX, mouseX, ref smoothXVelocity, cameraConfig.turnSmooth);
            smoothY = Mathf.SmoothDamp(smoothY, mouseY, ref smoothYVelocity, cameraConfig.turnSmooth);
        }
        else
        {
            smoothX = mouseX;
            smoothY = mouseY;
        }

        lookAngle += smoothX * cameraConfig.Y_rot_speed;
        Quaternion targetRot = Quaternion.Euler(0, lookAngle, 0);
        mTransform.rotation = targetRot;

        titlAngle -= smoothY * cameraConfig.X_rot_speed;
        titlAngle = Mathf.Clamp(titlAngle, cameraConfig.minAngle, cameraConfig.maxAngle);
        pivot.localRotation = Quaternion.Euler(titlAngle,0,0);
    }
}
