using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WFCCreatorMap : MonoBehaviour
{
    public WFCManager wFCManager;
    public Tile3D[] tileObjects;
    public List<WFCCell> gridComponents;
    public WFCCell cellObj;
    public Vector3 scaleTiles;
    public GameObject WrapperTiles;

    int iterations = 0;

    void Awake()
    {
        gridComponents = new List<WFCCell>();
    }

    public void InitializeGrid(Tile3D[] prefabTiles)
    {
        tileObjects = prefabTiles;

        for (int y = 0; y < wFCManager.size.y; y++)
        {
            for (int x = 0; x < wFCManager.size.x; x++)
            {
                for (int z = 0; z < wFCManager.size.z; z++)
                {
                    WFCCell newCell =  new WFCCell(false, tileObjects, new Vector3Int(x, y, z));
                    gridComponents.Add(newCell);
                }
            }
        }

        // StartCoroutine(CheckEntropy());
    }


    IEnumerator CheckEntropy()
    {
        List<WFCCell> tempGrid = new List<WFCCell>(gridComponents);

        tempGrid.RemoveAll(c => c.collapsed);

        tempGrid.Sort((a, b) => { return a.tileOptions.Length - b.tileOptions.Length; });

        int arrLength = tempGrid[0].tileOptions.Length;
        int stopIndex = default;

        for (int i = 1; i < tempGrid.Count; i++)
        {
            if (tempGrid[i].tileOptions.Length > arrLength)
            {
                stopIndex = i;
                break;
            }
        }

        if (stopIndex > 0)
        {
            tempGrid.RemoveRange(stopIndex, tempGrid.Count - stopIndex);
        }

        yield return new WaitForSeconds(0.01f);

        CollapseCell(tempGrid);
    }

    void CollapseCell(List<WFCCell> tempGrid)
    {
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);

        WFCCell cellToCollapse = tempGrid[randIndex];

        // cellToCollapse.collapsed = true;
        // Tile3D selectedTile = cellToCollapse.tileOptions[UnityEngine.Random.Range(0, cellToCollapse.tileOptions.Length)];
        // cellToCollapse.tileOptions = new Tile3D[] { selectedTile };

        // Tile3D foundTile = cellToCollapse.tileOptions[0];
        // Tile3D go = Instantiate(foundTile, cellToCollapse.position, Quaternion.identity);
        // cellToCollapse.SetMBObject(go);
        InstantiatePrefabToCell(cellToCollapse);

        UpdateGeneration();
    }
    
    void InstantiatePrefabToCell(WFCCell cellToCollapse)
    {
        cellToCollapse.collapsed = true;
        var index = UnityEngine.Random.Range(0, cellToCollapse.tileOptions.Length);
        Debug.Log($"InstantiatePrefabToCell (cell: {cellToCollapse.position}): index={index} из возможных={cellToCollapse.tileOptions.Length}");
        Tile3D selectedTile = cellToCollapse.tileOptions[index];
        cellToCollapse.tileOptions = new Tile3D[] { selectedTile };

        Tile3D foundTile = cellToCollapse.tileOptions[0];
        var obj = Instantiate(foundTile, cellToCollapse.position, Quaternion.identity, WrapperTiles.transform);
        obj.transform.localPosition = cellToCollapse.position - new Vector3(0, 0.5f, 0);
        obj.transform.rotation = Quaternion.Euler(0, foundTile.tileSockets.rotation, 0);
        obj.transform.localScale = scaleTiles;

        cellToCollapse.SetMBObject(obj);
    }

    void UpdateGeneration()
    {
        List<WFCCell> newGenerationCell = new List<WFCCell>(gridComponents);

        for (int y = 0; y < wFCManager.size.y; y++)
        {
            for (int x = 0; x < wFCManager.size.x; x++)
            {
                for (int z = 0; z < wFCManager.size.z; z++)
                {
                    var index = Helpers.From3DTo1D(x, y, z, wFCManager.size);
                    
                    if (gridComponents[index].collapsed)
                    {
                        // Debug.Log("called");
                        newGenerationCell[index] = gridComponents[index];
                    }
                    else
                    {
                        List<Tile3D> options = new List<Tile3D>();
                        foreach (Tile3D t in tileObjects)
                        {
                            options.Add(t);
                        }
                        
                        options.RemoveAll(t => !IsTilePossible(t, new Vector3Int(x, y, z)));

                        // //update above
                        // if (y > 0)
                        // {
                        //     WFCCell up = gridComponents[x + (y - 1) * dimensions];
                        //     List<Tile3D> validOptions = new List<Tile3D>();

                        //     foreach (Tile3D possibleOptions in up.tileOptions)
                        //     {
                        //         var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                        //         var valid = tileObjects[valOption].upNeighbours;

                        //         validOptions = validOptions.Concat(valid).ToList();
                        //     }

                        //     CheckValidity(options, validOptions);
                        // }

                        // //update right
                        // if (x < dimensions - 1)
                        // {
                        //     WFCCell right = gridComponents[x + 1 + y * dimensions];
                        //     List<Tile3D> validOptions = new List<Tile3D>();

                        //     foreach (Tile3D possibleOptions in right.tileOptions)
                        //     {
                        //         var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                        //         var valid = tileObjects[valOption].leftNeighbours;

                        //         validOptions = validOptions.Concat(valid).ToList();
                        //     }

                        //     CheckValidity(options, validOptions);
                        // }

                        // //look down
                        // if (y < dimensions - 1)
                        // {
                        //     WFCCell down = gridComponents[x + (y + 1) * dimensions];
                        //     List<Tile3D> validOptions = new List<Tile3D>();

                        //     foreach (Tile3D possibleOptions in down.tileOptions)
                        //     {
                        //         var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                        //         var valid = tileObjects[valOption].downNeighbours;

                        //         validOptions = validOptions.Concat(valid).ToList();
                        //     }

                        //     CheckValidity(options, validOptions);
                        // }

                        // //look left
                        // if (x > 0)
                        // {
                        //     WFCCell left = gridComponents[x - 1 + y * dimensions];
                        //     List<Tile3D> validOptions = new List<Tile3D>();

                        //     foreach (Tile3D possibleOptions in left.tileOptions)
                        //     {
                        //         var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                        //         var valid = tileObjects[valOption].rightNeighbours;

                        //         validOptions = validOptions.Concat(valid).ToList();
                        //     }

                        //     CheckValidity(options, validOptions);
                        // }

                        Tile3D[] newTileList = new Tile3D[options.Count];

                        for (int i = 0; i < options.Count; i++)
                        {
                            newTileList[i] = options[i];
                        }

                        newGenerationCell[index].RecreateCell(newTileList);
                    }
                }
            }
        }

        gridComponents = newGenerationCell;
        iterations++;

        // if(iterations < wFCManager.size.x * wFCManager.size.y * wFCManager.size.z)
        // {
        //     StartCoroutine(CheckEntropy());
        // }
    }


    private bool IsTilePossible(Tile3D tile, Vector3Int position)
    {
        // var dimensions = _gameManager.LevelConfig.gridSize.z;
        int x = position.x;
        int y = position.y;
        int z = position.z;
        int index = default;

        int maxIndex = wFCManager.size.x * wFCManager.size.y * wFCManager.size.z;

        index = Helpers.From3DTo1D(x - 1, y, z, wFCManager.size);
        if (index >= 0 && index < maxIndex && gridComponents[index].collapsed)
        {
            bool isAllRightImpossible = gridComponents[index].tileOptions
                .All(t => !CanAppendTile(t, tile, DirectionSideTile.Left));
            if (isAllRightImpossible) return false;
        }
        
        index = Helpers.From3DTo1D(x + 1, y, z, wFCManager.size);
        if (index >= 0 && index < maxIndex && gridComponents[index].collapsed)
        {
            bool isAllLeftImpossible = gridComponents[index].tileOptions // possibleTiles[position.x + 1, position.z]
                .All(t => !CanAppendTile(t, tile, DirectionSideTile.Right));
            if (isAllLeftImpossible) return false;
        }

        index = Helpers.From3DTo1D(x, y, z - 1, wFCManager.size);
        if (index >= 0 && index < maxIndex && gridComponents[index].collapsed)
        {
            bool isAllForwardImpossible = gridComponents[index].tileOptions // possibleTiles[position.x, position.z - 1]
                .All(t => !CanAppendTile(t, tile, DirectionSideTile.Forward));
            if (isAllForwardImpossible) return false;
        }
        
        index = Helpers.From3DTo1D(x, y, z + 1, wFCManager.size);
        if (index >= 0 && index < maxIndex && gridComponents[index].collapsed)
        {
            bool isAllBackImpossible = gridComponents[index].tileOptions // possibleTiles[position.x, position.z + 1]
                .All(t => !CanAppendTile(t, tile, DirectionSideTile.Back));
            if (isAllBackImpossible) return false;
        }

        index = Helpers.From3DTo1D(x, y - 1, z, wFCManager.size);
        if (index >= 0 && index < maxIndex && gridComponents[index].collapsed)
        {
            bool isAllBackImpossible = gridComponents[index].tileOptions // possibleTiles[position.x, position.z + 1]
                .All(t => !CanAppendTile(t, tile, DirectionSideTile.Bottom));
            if (isAllBackImpossible) return false;
        }

        index = Helpers.From3DTo1D(x, y + 1, z, wFCManager.size);
        if (index >= 0 && index < maxIndex && gridComponents[index].collapsed)
        {
            bool isAllBackImpossible = gridComponents[index].tileOptions // possibleTiles[position.x, position.z + 1]
                .All(t => !CanAppendTile(t, tile, DirectionSideTile.Top));
            if (isAllBackImpossible) return false;
        }

        return true;
    }

/// <summary>
/// проверка могут ли соединится тайлы
/// </summary>
/// <param name="existingTile">тайл, из ячейки, которая уже сформирована (collapsed)</param>
/// <param name="tileToAppend">тайл, который проверяется, можно ли поставить в текущую ячейку</param>
/// <param name="direction">направление проверки</param>
/// <returns></returns>
/// <exception cref="System.ArgumentException"></exception>
    private bool CanAppendTile(Tile3D existingTile, Tile3D tileToAppend, DirectionSideTile direction)
    {
        //
        
        // if (existingTile == null) return true;

        if (direction == DirectionSideTile.Left)
        {
            return existingTile.tileSockets.posX == tileToAppend.tileSockets.negX;
        }
        else if (direction == DirectionSideTile.Right)
        {
            return existingTile.tileSockets.negX == tileToAppend.tileSockets.posX;
        }
        else if (direction == DirectionSideTile.Forward)
        {
            return existingTile.tileSockets.posZ == tileToAppend.tileSockets.negZ;
        }
        else if (direction == DirectionSideTile.Back)
        {
            return existingTile.tileSockets.negZ == tileToAppend.tileSockets.posZ;
        }
        else if (direction == DirectionSideTile.Top)
        {
            return existingTile.tileSockets.negY == tileToAppend.tileSockets.posY;
        }
        else if (direction == DirectionSideTile.Bottom)
        {
            return existingTile.tileSockets.posY == tileToAppend.tileSockets.negY;
        }
        else
        {
            throw new System.ArgumentException("Wrong direction value, should be Vector3.left/right/back/forward",
                nameof(direction));
        }
    }

    // void CheckValidity(List<Tile3D> optionList, List<Tile3D> validOption)
    // {
    //     for (int x = optionList.Count - 1; x >= 0; x--)
    //     {
    //         var element = optionList[x];
    //         if (!validOption.Contains(element))
    //         {
    //             optionList.RemoveAt(x);
    //         }
    //     }
    // }

    public void CreateTile(Vector3 position)
    {
        int randIndex = Helpers.From3DTo1D((int)position.x, (int)position.y, (int)position.z, wFCManager.size);

        WFCCell cellToCollapse = gridComponents[randIndex];

        InstantiatePrefabToCell(cellToCollapse);

        UpdateGeneration();
    }

    public void RemoveTile(Vector3 position)
    {
        int index = Helpers.From3DTo1D((int)position.x, (int)position.y, (int)position.z, wFCManager.size);

        WFCCell cellToRemove = gridComponents[index];

        cellToRemove.collapsed = false;
        cellToRemove.RecreateCell(tileObjects);

        Debug.Log($"Remove cell {position}: {index}, {cellToRemove.MBObject.name}-{cellToRemove.MBObject != null}");

        Destroy(cellToRemove.MBObject.gameObject);
        cellToRemove.MBObject = null;
        
        UpdateGeneration();
    }
}