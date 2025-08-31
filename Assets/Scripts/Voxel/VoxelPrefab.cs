using UnityEngine;

public class VoxelPrefab : MonoBehaviour
{
    GameManager gameManager => GameManager.Instance;
    GPUInstanceEnabler gPUInstanceEnabler;
    void Start()
    {
        gPUInstanceEnabler = GetComponent<GPUInstanceEnabler>();

        Init();
    }

    public void Init()
    {

        transform.localScale = new Vector3(gameManager.Settings.scaleObjects, gameManager.Settings.scaleObjects, gameManager.Settings.scaleObjects);
    
    }
}
