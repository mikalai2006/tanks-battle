using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    private Queue<GameObject> pool = new Queue<GameObject>();
    public Queue<GameObject> Pool => pool;
    [SerializeField] private int count;
    [SerializeField] private int poolLength;
    [SerializeField] private int countUsed;
    public List<GameObject> poolObjs = new List<GameObject>();

    void Start()
    {
        InitPool();
    }

    public GameObject GetObject()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            countUsed++;
            poolLength = pool.Count;

            return obj;
        }

        GameObject objNew = CreateElement(poolLength);
        ReturnObject(objNew);

        objNew = pool.Dequeue();
        countUsed++;

        poolLength = pool.Count;
        return objNew;
    }

    public void ReturnObject(GameObject obj)
    {

        Rigidbody rb = obj.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Debug.Log($"<color=yellow>Reset rigidbody {obj.name}</color>");

            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            else
            {
                obj.transform.position = new Vector3(0f, 0f, 0f);
                obj.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            }
            // bool isKinematic = rb.isKinematic;
            // rb.isKinematic = true;
            // rb.transform.position = new Vector3(0f, 0f, 0f);
            // rb.transform.rotation = Quaternion.Euler(new Vector3(0f,0f,0f));
            // rb.linearVelocity = new Vector3(0f,0f,0f);
            // rb.angularVelocity = new Vector3(0f,0f,0f);

            // if (!isKinematic)
            // {
            //     rb.isKinematic = false; // Re-enable physics
            // }

            // rb.WakeUp();
        }
        obj.SetActive(false);

        pool.Enqueue(obj);

        // poolObjs.Clear();
        // foreach (GameObject item in pool)
        // {
        //     poolObjs.Add(item);
        // }
    }

    GameObject CreateElement(int index = 0)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.name = $"Element-{index}";
        return obj;
    }

    public void InitPool()
    {
        for (int i = 0; i < count; i++)
        {
            // GameObject obj = Instantiate(prefab, transform);
            // obj.name = $"bullet-{i}";
            GameObject obj = CreateElement(i);
            ReturnObject(obj);
        }
    }

    void OnDestroy()
    {
        foreach (GameObject go in pool)
        {
            Destroy(go);
        }
    }
}
