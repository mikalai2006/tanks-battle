using Unity.Entities;
using Unity.Mathematics;

public struct InputComponent : IComponentData
{
    public float2 movemement;
    public float2 mousePos;
    public bool pressingLMB;
}
