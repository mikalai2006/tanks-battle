using UnityEngine;

public class VoxelPrefab : MonoBehaviour
{
    GameManager gameManager => GameManager.Instance;
    public GPUInstanceEnabler gPUInstanceEnabler;
    void Awake()
    {
        gPUInstanceEnabler = GetComponent<GPUInstanceEnabler>();

        Init();
    }

    public void Init()
    {

        transform.localScale = new Vector3(gameManager.Settings.scaleObjects, gameManager.Settings.scaleObjects, gameManager.Settings.scaleObjects);

    }

    public void SetColor(Color color)
    {
        if (gPUInstanceEnabler)
        {
            gPUInstanceEnabler.SetColor(color);
        }
    }
}
