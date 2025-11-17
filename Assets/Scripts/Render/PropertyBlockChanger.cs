using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class PropertyBlockChanger : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private MaterialPropertyBlock _materialPropertyBlock;
    public Color color1;
    public float emissionValue;

    void Awake()
    {
        _materialPropertyBlock = new MaterialPropertyBlock();
        _meshRenderer = GetComponent<MeshRenderer>();

        _meshRenderer.SetPropertyBlock(_materialPropertyBlock,0);
    }

    // void Start()
    // {
    //     SetData(color1);
    // }

    public void SetData(float _emissionValue, int indexSubmesh = 0)
    {
        if (_meshRenderer)
        {
            emissionValue = _emissionValue;
            _materialPropertyBlock.SetFloat("_EmissionValue", emissionValue);
            // _materialPropertyBlock.SetColor("_Color", color);
            _meshRenderer.SetPropertyBlock(_materialPropertyBlock, indexSubmesh);
            // color1 = color;
        }
    }
}
