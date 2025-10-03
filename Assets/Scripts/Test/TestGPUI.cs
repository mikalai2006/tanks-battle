using UnityEngine;

public class TestGPUI : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                GameObject obj = Instantiate(prefab, transform);
                obj.transform.SetPositionAndRotation(new Vector3(i, 0, j), Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
