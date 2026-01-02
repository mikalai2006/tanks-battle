using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WFCManager : MonoBehaviour
{
    public WFCCreatorMap wFCCreatorMap;
    public Vector3Int size;
    public GameObject prefabCellPlaceholder;
    public Button buttonCreateGrid;
    public List<Tile3D> tilePrefabs;

    public List<WFCToolsCell> toolsCells;

    void Awake()
    {
        buttonCreateGrid.onClick.AddListener(() =>
        {
            Init();
            CreateTools();
        });
    }

    void Init()
    {
        wFCCreatorMap.InitializeGrid(tilePrefabs.ToArray());
    }

    void CreateTools()
    {
        ResetCellsTools();

        for (int y = 0; y < 1; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    Vector3 position = new Vector3(x,y,z);
                    GameObject cellGameObject = Instantiate(prefabCellPlaceholder, position, Quaternion.identity, transform);
                    
                    cellGameObject.transform.localPosition = position;

                    WFCToolsCell cell = cellGameObject.GetComponent<WFCToolsCell>();
                    cell.Init(this);

                    toolsCells.Add(cell);
                }  
            }  
        }    
    }

    private void ResetCellsTools()
    {
        foreach (var item in toolsCells)
        {
            Destroy(item.gameObject);
        }

        toolsCells.Clear();
    }

    public void CreateCellTools(Vector3 position)
    {
        if (
            position.x < 0 ||
            position.x >= size.x ||
            position.y < 0 ||
            position.y >= size.y ||
            position.z < 0 ||
            position.z >= size.z
        )
        {
            Debug.LogWarning($"Попытка создать тайл за пределами заданной сетки!");
            return;
        }

        GameObject cellGameObject = Instantiate(prefabCellPlaceholder, position, Quaternion.identity, transform);
        cellGameObject.transform.localPosition = position;

        WFCToolsCell cell = cellGameObject.GetComponent<WFCToolsCell>();
        cell.Init(this);
        toolsCells.Add(cell);
        
        // вставка префаба.
        wFCCreatorMap.CreateTile(position);
        // Tile3D tile3D = Instantiate(tilePrefabs[UnityEngine.Random.Range(0, tilePrefabs.Count)], position, Quaternion.identity, transform);
        // tile3D.transform.localPosition = position - new Vector3(0, 0.5f, 0);
    }

    public void RemoveCellTools(WFCToolsCell cellTools)
    {
        WFCToolsCell cellForRemove = toolsCells.Find(x => x == cellTools);

        if (cellForRemove)
        {
            Destroy(cellForRemove.gameObject);
            toolsCells.Remove(cellForRemove);

            wFCCreatorMap.RemoveTile(cellTools.position);
        }
    }
}
