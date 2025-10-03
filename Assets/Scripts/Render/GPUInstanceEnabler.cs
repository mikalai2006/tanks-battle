using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class GPUInstanceEnabler : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private MaterialPropertyBlock _materialPropertyBlock;
    public Color color1;

    void Awake()
    {
        _materialPropertyBlock = new MaterialPropertyBlock();
        _meshRenderer = GetComponent<MeshRenderer>();

        // color1 = _meshRenderer.material.color;
        _meshRenderer.SetPropertyBlock(_materialPropertyBlock,0);
        // StartCoroutine(ChangeColor());
        //SetColor(UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
    }

    void Start()
    {
        SetColor(color1);
    }

    private IEnumerator ChangeColor()
    {
        while (true)
        {
            SetColor(UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
            yield return new WaitForSeconds(1f);
        }
    }

    public void SetColor(Color color, int indexSubmesh = 0)
    {
        if (_meshRenderer)
        {
            _materialPropertyBlock.SetColor("_NoiseColor", color);
            _meshRenderer.SetPropertyBlock(_materialPropertyBlock, indexSubmesh);
            color1 = color;
        }
    }
}
