using System.Collections;
using UnityEngine;

public class TestGPUInstanceEnabler : MonoBehaviour
{
private MeshRenderer _meshRenderer;
    private MaterialPropertyBlock _materialPropertyBlock;
    public Color color1;

    void Awake()
    {
        _materialPropertyBlock = new MaterialPropertyBlock();
        _meshRenderer = GetComponent<MeshRenderer>();

        // _materialPropertyBlock.SetColor("_NoiseColor", color1);
        // _meshRenderer.SetPropertyBlock(_materialPropertyBlock,0);
        // StartCoroutine(ChangeColor());
        SetColor(UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
    }

    private IEnumerator ChangeColor()
    {
        while (true)
        {
            SetColor(UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
            yield return new WaitForSeconds(1f);
        }
    }

    public void SetColor(Color color)
    {
        if (_meshRenderer)
        {
            _materialPropertyBlock.SetColor("_NoiseColor", color);
            _meshRenderer.SetPropertyBlock(_materialPropertyBlock, 0);
            color1 = color;
        }
    }
}
