using UnityEngine;

[CreateAssetMenu(fileName = "CameraConfig", menuName = "Scriptable Objects/CameraConfig")]
public class CameraConfig : ScriptableObject
{
    public float turnSmooth;
    public float pivotSpeed;
    public float Y_rot_speed;
    public float X_rot_speed;
    public float minAngle;
    public float maxAngle;
    public float normalX;
    public float normalY;
    public float normalZ;
    public float aimZ;
    public float aimX;
}
