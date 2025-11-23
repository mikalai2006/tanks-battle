using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InfoBoxRowComponentPool : MonoBehaviour
{
    // public VisualTreeAsset prefab;
    private Queue<InfoBoxRowComponent> pool = new Queue<InfoBoxRowComponent>();
    public Queue<InfoBoxRowComponent> Pool => pool;
    [SerializeField] private int count;
    [SerializeField] private int poolLength;
    [SerializeField] private int countUsed;
    public List<InfoBoxRowComponent> poolObjs = new List<InfoBoxRowComponent>();

    void Start()
    {
        InitPool();
    }

    public InfoBoxRowComponent GetObject()
    {
        if (pool.Count > 0)
        {
            InfoBoxRowComponent obj = pool.Dequeue();
            obj.style.display = DisplayStyle.Flex;
            countUsed++;

            poolLength = pool.Count;
            return obj;
        }

        var objNew = CreateElement(pool.Count);
        ReturnObject(objNew);

        objNew = pool.Dequeue();
        objNew.style.display = DisplayStyle.Flex;
        countUsed++;

        poolLength = pool.Count;
        return objNew;

        // return new InfoBoxRowComponent{ name = "InfoBoxRowComponent_"}; // prefab.Instantiate();
    }

    public void ReturnObject(InfoBoxRowComponent obj)
    {
        obj.style.display = DisplayStyle.None;

        pool.Enqueue(obj);

        poolLength = pool.Count;
    }

    InfoBoxRowComponent CreateElement(int index = 0)
    {
        InfoBoxRowComponent obj = new InfoBoxRowComponent{}; // prefab.Instantiate(); //
        obj.pickingMode = PickingMode.Ignore;
        obj.name = $"Element-{index}";
        return obj;
    }

    public void InitPool()
    {
        for (int i = 0; i < count; i++)
        {
            var obj = CreateElement(i);
            ReturnObject(obj);
        }
    }

    void OnDestroy()
    {
        // foreach (InfoBoxRowComponent el in pool)
        // {
        // }
    }
}
