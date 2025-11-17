using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class JoystickController : Joystick
{
    public static event System.Action<PointerEventData> RunPointerDown;
    public static event System.Action<PointerEventData> RunPointerUp;
    [SerializeField] private float moveThreshold = 1;
    [SerializeField] private JoystickControllerType joystickType = JoystickControllerType.Fixed;
    public float MoveThreshold { get { return moveThreshold; } set { moveThreshold = Mathf.Abs(value); } }
    private Vector2 fixedPosition = Vector2.zero;

    public void SetMode(JoystickControllerType joystickType)
    {
        this.joystickType = joystickType;
        if(joystickType == JoystickControllerType.Fixed)
        {
            background.anchoredPosition = fixedPosition;
            background.gameObject.SetActive(true);
        }
        else
            background.gameObject.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();
        fixedPosition = background.anchoredPosition;
        SetMode(joystickType);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(joystickType != JoystickControllerType.Fixed)
        {
            background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
            background.gameObject.SetActive(true);
        }
        base.OnPointerDown(eventData);

        RunPointerDown?.Invoke(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if(joystickType != JoystickControllerType.Fixed)
            background.gameObject.SetActive(false);

        base.OnPointerUp(eventData);

        RunPointerUp?.Invoke(eventData);
    }

    protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (joystickType == JoystickControllerType.Dynamic && magnitude > moveThreshold)
        {
            Vector2 difference = normalised * (magnitude - moveThreshold) * radius;
            background.anchoredPosition += difference;
        }
        base.HandleInput(magnitude, normalised, radius, cam);
    }
}

[System.Serializable]
public enum JoystickControllerType { Fixed, Floating, Dynamic }