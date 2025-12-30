using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Mikalai2006.Voxel;
using UnityEngine;

public class WFCGenerator : MonoBehaviour {
    public static event Action<string> OnSetNotify;
    public static event Action<float> OnAddProgress;
    public static event Action OnCompleteTiled;
    GameManager _gameManager => GameManager.Instance;
    public LevelManager levelManager;
//    public int dimensions;
    public List<Tile3D> TilePrefabs;
    public List<Tile3D> TilePrefabsEmpty;
    public List<Tile3D> TilePrefabsInner;
    public List<Tile3D> TilePrefabsInnerTop;
    public Tile3DDebugger TileDebugger;
    public GameObject TileDebuggerWrapper;
    public Cell3D[] gridComponents;
    public Tile3D TilePlaceholderNone;
    System.Threading.CancellationTokenSource cts;

    int iterations = 0;

    void Awake()
    {
        cts = new CancellationTokenSource();
        
        TilePrefabs = new List<Tile3D>();
        // TilePrefabs.AddRange(_gameManager.LevelConfig.TilePrefabs);
        TilePrefabsEmpty = new List<Tile3D>();
        TilePrefabsEmpty.AddRange(_gameManager.LevelConfig.TilePrefabsEmpty);
        TilePrefabsInner = new List<Tile3D>();
        // TilePrefabsInner.AddRange(_gameManager.LevelConfig.TilePrefabsInner);
        TilePrefabsInnerTop = new List<Tile3D>();
        // TilePrefabsInnerTop.AddRange(_gameManager.LevelConfig.TilePrefabsInnerTop);

        
        // создаем префабы.
        foreach (var obj in _gameManager.LevelConfig.TilePrefabs)
        {
            var clone = Instantiate(obj, new Vector3(0,100,0), Quaternion.identity, transform);
            clone.name = $"{obj.UID}";
            clone.OnStart();
            TilePrefabs.Add(clone);
        }
        
        // создаем префабы inner.
        foreach (var obj in _gameManager.LevelConfig.TilePrefabsInner)
        {
            var clone = Instantiate(obj, new Vector3(0,100,0), Quaternion.identity, transform);
            clone.name = $"{obj.UID}";
            clone.OnStart();
            TilePrefabsInner.Add(clone);
        }
        foreach (var obj in _gameManager.LevelConfig.TilePrefabsInnerTop)
        {
            var clone = Instantiate(obj, new Vector3(0,100,0), Quaternion.identity, transform);
            clone.name = $"{obj.UID}";
            clone.OnStart();
            TilePrefabsInnerTop.Add(clone);
        }
    }

    void OnDestroy()
    {
        TilePrefabs.Clear();
        TilePrefabsEmpty.Clear();
        TilePrefabsInner.Clear();
        TilePrefabsInnerTop.Clear();

        cts.Cancel();
        cts.Dispose();
    }

    public void OnUpdateColors()
    {
        for (int i = 0; i < _gameManager.LevelConfig.TilePrefabs.Count; i++)
        {
            var vmRenderer = _gameManager.LevelConfig.TilePrefabs[i].GetComponentInChildren<VoxelMeshRender>();

            if (vmRenderer.Config.sOVoxelData.groups.Count > 0)
            {
                var a = vmRenderer.Config.sOVoxelData.groups[0];
                a.color = _gameManager.LevelConfig.colorWall;
                vmRenderer.Config.sOVoxelData.groups[0] = a;
            }
            if (vmRenderer.Config.sOVoxelData.groups.Count > 1)
            {
                var b = vmRenderer.Config.sOVoxelData.groups[1];
                b.color = _gameManager.LevelConfig.colorNature;
                vmRenderer.Config.sOVoxelData.groups[1] = b;
            }
        }

        for (int i = 0; i < _gameManager.LevelConfig.TilePrefabsInner.Count; i++)
        {
            var vmRenderer = _gameManager.LevelConfig.TilePrefabsInner[i].GetComponentInChildren<VoxelMeshRender>();

            if (vmRenderer.Config.sOVoxelData.groups.Count > 0)
            {
                var a = vmRenderer.Config.sOVoxelData.groups[0];
                a.color = _gameManager.LevelConfig.colorWall;
                vmRenderer.Config.sOVoxelData.groups[0] = a;
            }
            if (vmRenderer.Config.sOVoxelData.groups.Count > 1)
            {
                var b = vmRenderer.Config.sOVoxelData.groups[1];
                b.color = _gameManager.LevelConfig.colorNature;
                vmRenderer.Config.sOVoxelData.groups[1] = b;
            }
        }

        
        for (int i = 0; i < _gameManager.LevelConfig.TilePrefabsInnerTop.Count; i++)
        {
            var vmRenderer = _gameManager.LevelConfig.TilePrefabsInnerTop[i].GetComponentInChildren<VoxelMeshRender>();

            if (vmRenderer.Config.sOVoxelData.groups.Count > 0)
            {
                var a = vmRenderer.Config.sOVoxelData.groups[0];
                a.color = _gameManager.LevelConfig.colorWall;
                vmRenderer.Config.sOVoxelData.groups[0] = a;
            }
            if (vmRenderer.Config.sOVoxelData.groups.Count > 1)
            {
                var b = vmRenderer.Config.sOVoxelData.groups[1];
                b.color = _gameManager.LevelConfig.colorNature;
                vmRenderer.Config.sOVoxelData.groups[1] = b;
            }
        }
        for (int i = 0; i < _gameManager.LevelConfig.TilePrefabsEmpty.Count; i++)
        {
            var vmRenderer = _gameManager.LevelConfig.TilePrefabsEmpty[i].GetComponentInChildren<VoxelMeshRender>();

            if (vmRenderer.Config.sOVoxelData.groups.Count > 0)
            {
                var a = vmRenderer.Config.sOVoxelData.groups[0];
                a.color = _gameManager.LevelConfig.colorWall;
                vmRenderer.Config.sOVoxelData.groups[0] = a;
            }
            if (vmRenderer.Config.sOVoxelData.groups.Count > 1)
            {
                var b = vmRenderer.Config.sOVoxelData.groups[1];
                b.color = _gameManager.LevelConfig.colorNature;
                vmRenderer.Config.sOVoxelData.groups[1] = b;
            }
        }

        VoxelMeshRender vmGround = _gameManager.LevelConfig.planePrefab;

        if (vmGround.Config.sOVoxelData.groups.Count > 0)
        {
            var a = vmGround.Config.sOVoxelData.groups[0];
            a.color = _gameManager.LevelConfig.colorWall;
            vmGround.Config.sOVoxelData.groups[0] = a;
        }
        if (vmGround.Config.sOVoxelData.groups.Count > 1)
        {
            var b = vmGround.Config.sOVoxelData.groups[1];
            b.color = _gameManager.LevelConfig.colorNature;
            vmGround.Config.sOVoxelData.groups[1] = b;
        }
    }

    public void OnCreateVariantsPrefabs()
    {
        OnSetNotify?.Invoke("createVariantsTiles");
        
        // // создаем префабы.
        // foreach (var obj in _gameManager.LevelConfig.TilePrefabs)
        // {
        //     var clone = Instantiate(obj, new Vector3(0,100,0), Quaternion.identity, transform);
        //     clone.name = $"{obj.UID}";
        //     clone.OnStart();
        //     TilePrefabs.Add(clone);
        // }
        
        // // создаем префабы inner.
        // foreach (var obj in _gameManager.LevelConfig.TilePrefabsInner)
        // {
        //     var clone = Instantiate(obj, new Vector3(0,100,0), Quaternion.identity, transform);
        //     clone.name = $"{obj.UID}";
        //     clone.OnStart();
        //     TilePrefabsInner.Add(clone);
        // }
        // foreach (var obj in _gameManager.LevelConfig.TilePrefabsInnerTop)
        // {
        //     var clone = Instantiate(obj, new Vector3(0,100,0), Quaternion.identity, transform);
        //     clone.name = $"{obj.UID}";
        //     clone.OnStart();
        //     TilePrefabsInnerTop.Add(clone);
        // }
        
        int countBeforeAdding = _gameManager.LevelConfig.TilePrefabs.Count;

        for (int i = 0; i < countBeforeAdding; i++)
        {

            Vector3 pos = new Vector3(0, -100, 0);
            Tile3D tile = _gameManager.LevelConfig.TilePrefabs[i];

            // var a = tile.meshConfig.sOVoxelData.groups[0];
            // a.color = _gameManager.LevelConfig.colorGround;
            // tile.meshConfig.sOVoxelData.groups[0] = a;
            // if (tile.meshConfig.sOVoxelData.groups.Count > 1)
            // {
            //     var b = tile.meshConfig.sOVoxelData.groups[1];
            //     a.color = _gameManager.LevelConfig.colorNature;
            //     tile.meshConfig.sOVoxelData.groups[1] = b;
            // }
            // tile.OnRefreshData();

            switch (tile.Rotation)
            {
                case Tile3D.RotationType.OnlyRotation:
                    break;

                case Tile3D.RotationType.TwoRotations:
                    // tile.Weight /= 2;
                    // if (tile.Weight <= 0) tile.Weight = 1;

                    var clone = Instantiate(tile, pos,// + Vector3.right,
                        Quaternion.identity, transform);
                    clone.name = $"{tile.UID}_90";
                    clone.OnStart();
                    clone.Rotate90();
                    TilePrefabs.Add(clone);
                    break;

                case Tile3D.RotationType.FourRotations:
                    // tile.Weight /= 4;
                    // if (tile.Weight <= 0) tile.Weight = 1;

                    var clone2 = Instantiate(tile, pos,// + Vector3.right,
                        Quaternion.identity, transform);
                    clone2.name = $"{tile.UID}_90";
                    clone2.OnStart();
                    clone2.Rotate90();
                    TilePrefabs.Add(clone2);

                    var clone3 = Instantiate(tile, pos,// + Vector3.right * 2,
                        Quaternion.identity, transform);
                    clone3.name = $"{tile.UID}_180";
                    clone3.OnStart();
                    clone3.Rotate90();
                    clone3.Rotate90();
                    TilePrefabs.Add(clone3);

                    var clone4 = Instantiate(tile, pos,// + Vector3.right * 3,
                        Quaternion.identity, transform);
                    clone4.name = $"{tile.UID}_270";
                    clone4.OnStart();
                    clone4.Rotate90();
                    clone4.Rotate90();
                    clone4.Rotate90();
                    TilePrefabs.Add(clone4);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public async UniTask OnGenerateTiles(CancellationTokenSource cancelToken)
    {
        if (!_gameManager.Settings.DebugSettings.disableCreateTiles)
        {
            if (!cancelToken.IsCancellationRequested)
            {
                gridComponents = new Cell3D[_gameManager.LevelConfig.gridSize.x * _gameManager.LevelConfig.gridSize.z* _gameManager.LevelConfig.gridSize.y];

                // TilePrefabs.AddRange(_gameManager.LevelConfig.TilePrefabs);

                if (_gameManager.LevelConfig.saveTiled.nameMap == _gameManager.LevelConfig.tileSettings.nameMap)
                {
                    OnSetNotify?.Invoke("loadMap");
                    // если в сохраненных тайлах находится карта с текущим именем,
                    // значит карта уже сгенерирована и осталось только создать игровые объекты.
                    var savedTiles = _gameManager.LevelConfig.saveTiled.gridComponents;

                    var TiledsInners = TilePrefabsInner.Concat(TilePrefabsInnerTop).ToList();

                    Stack<Cell3D> stackCellNeedCreate = new Stack<Cell3D>();
                    for (int i = 0; i < savedTiles.Length; i++)
                    {
                        Cell3DData cell3DData = savedTiles[i];
                        Vector3Int position = new Vector3Int(cell3DData.position.x, cell3DData.position.y, cell3DData.position.z);
                        GridTileNode node = levelManager.mapManager.gridTileHelper.GetNode(cell3DData.position.x, cell3DData.position.y, cell3DData.position.z);

                        Cell3D loadCell = new Cell3D();
                        List<Tile3D> tileForCell = new List<Tile3D>();


                        switch (cell3DData.stateNode)
                        {
                            case (int)StateNode.TiledInner:
                                node.StateNode = StateNode.TiledInner;
                                tileForCell = TiledsInners.FindAll(x => x.UID == cell3DData.uid && x.transform.rotation.eulerAngles.y == cell3DData.RotationY);
                                break;
                            case (int)StateNode.Tiled:
                                node.StateNode = StateNode.Tiled;
                                tileForCell = TilePrefabs.FindAll(x => x.UID == cell3DData.uid && x.transform.rotation.eulerAngles.y == cell3DData.RotationY);
                                break;
                            default:
                                node.StateNode = (StateNode)cell3DData.stateNode;
                                tileForCell = TilePrefabsEmpty.FindAll(x => x.UID == cell3DData.uid && x.transform.rotation.eulerAngles.y == cell3DData.RotationY);
                                break;
                        }
                        if (tileForCell.Count == 0)
                        {

                            Debug.LogWarning($"node.StateNode={node.StateNode}[{cell3DData.stateNode}, {cell3DData.uid}, {cell3DData.RotationY}], position={position}, tileForCell={tileForCell.Count}");
                        }

                        loadCell.CreateCell(false, tileForCell, position, new Vector3(0, cell3DData.RotationY, 0));
                        node.isCollapsed = true;
                        gridComponents[i] = loadCell;
                        stackCellNeedCreate.Push(gridComponents[i]);
                    }

                    await CreateTiles(cts, stackCellNeedCreate);

                    OnCompleteTiled?.Invoke();
                }
                else
                {
                    OnSetNotify?.Invoke("generateMap");
                    // генерируем карту и создаем игровые объекты.
                    await InitializeGrid(cts);
                }
            }
        } else
        {
            OnCompleteTiled?.Invoke();
        }
    }

    async UniTask InitializeGrid(CancellationTokenSource cancelToken)
    {
        for (int depth = 0; depth < _gameManager.LevelConfig.gridSize.y; depth++)
        {
            for (int col = 0; col < _gameManager.LevelConfig.gridSize.z; col++)
            {
                for (int row = 0; row < _gameManager.LevelConfig.gridSize.x; row++)
                {
                    var position = new Vector3Int(row, depth, col);
                    GridTileNode node = levelManager.mapManager.gridTileHelper.GetNode(row, depth, col);
                    Cell3D newCell = new Cell3D(); //Instantiate(_gameManager.LevelConfig.prefabPlaceholder, position, Quaternion.identity);

                    newCell.isTop = node.isTop;

                    if (node.StateNode.HasFlag(StateNode.Tiled))
                    {
                        newCell.CreateCell(false, TilePrefabs, position, Vector3.zero);
                    }
                    else if (node.StateNode.HasFlag(StateNode.TiledInner))
                    {
                        // if (depth < _gameManager.LevelConfig.gridSize.y - 1)
                        if (!node.isTop)
                        {
                            newCell.CreateCell(true, TilePrefabsInner, position, Vector3.zero);
                        } else
                        {
                            newCell.CreateCell(true, TilePrefabsInnerTop, position, Vector3.zero);
                        }

                        node.isCollapsed = true;
                        newCell.collapsed = true;
                        // Tile3D selectedTile = GetRandomTile(newCell.tileOptions);
                        // newCell.tileOptions = new Tile3D[] { selectedTile };
                        // var obj = Instantiate(newCell.tileOptions[0], newCell.position, newCell.tileOptions[0].transform.rotation, transform);
                        // obj.name = $"{newCell.tileOptions[0].transform.position.x}x{newCell.tileOptions[0].transform.position.z}__{selectedTile.name}";
                    }
                    else
                    {
                        newCell.CreateCell(true, TilePrefabsEmpty, position, Vector3.zero);
                        node.isCollapsed = true;
                        newCell.collapsed = true;

                        // Tile3D selectedTile = GetRandomTile(newCell.tileOptions);
                        // newCell.tileOptions = new Tile3D[] { selectedTile };
                        // var obj = Instantiate(newCell.tileOptions[0], newCell.position, newCell.tileOptions[0].transform.rotation, transform);
                        // obj.name = $"{newCell.position.x}x{newCell.position.z}__{selectedTile.name}";
                    }
                    int index = Helpers.From3DTo1D(
                        row,
                        depth,
                        col,
                        _gameManager.LevelConfig.gridSize
                    ); //x + z * _gameManager.LevelConfig.gridSize.z;
                    
                    gridComponents[index] = newCell;

                    var obj = Instantiate(TileDebugger, newCell.position + new Vector3Int(0,depth,0), Quaternion.Euler(90,0,0), TileDebuggerWrapper.transform);
                    var allNeighbours = levelManager.mapManager.gridTileHelper.GetNeighbourListWithTiled(node, true);
                    obj.text.text = $"{row},{depth},{col}\r\n{node.StateNode}|{allNeighbours.Count}";
                }
            }
        }

        // находим все закрытые ячейки и создаем для них игровые объекты.
        var collapsedCell = gridComponents.ToList().Where(t => t.collapsed);
                        Debug.Log($"collapsed nodes count = {collapsedCell.Count()}");
        Stack<Cell3D> stackCellNeedCreate = new Stack<Cell3D>();
        for (int i = 0; i < gridComponents.Length; i++)
        {
            if (gridComponents[i].collapsed)
            {
                stackCellNeedCreate.Push(gridComponents[i]);
            }
        }

        UpdateGeneration();

        await CreateTiles(cancelToken, stackCellNeedCreate);

        // StartCoroutine(CheckEntropy());
        await CheckEntropy(cancelToken);
    }

    public async UniTask CreateTiles(CancellationTokenSource cancelToken, Stack<Cell3D> stackCellNeedCreate)
    {
        int count = 25;
        float stepProgress = 0.6f / (stackCellNeedCreate.Count / count);

        OnSetNotify?.Invoke("createGameObjects");
        while (stackCellNeedCreate.Count > 0)
        {
            if (cancelToken.IsCancellationRequested)
            {
                break;
            } else
            {
                var cell = stackCellNeedCreate.Pop();
                GridTileNode node = levelManager.mapManager.gridTileHelper.GetNode(cell.position.x, cell.position.y, cell.position.z);

                if (!node.StateNode.HasFlag(StateNode.Empty))
                {
                    // Tile3D selectedTile = GetRandomTile(cell.tileOptions);
                    // // cell.tileOptions = new Tile3D[] { selectedTile };
                    // var obj = Instantiate(cell.tileOptions[0], cell.position, cell.tileOptions[0].transform.rotation, transform);
                    // obj.name = $"{cell.position.x}x{cell.position.z}__{selectedTile.name}";
                    InstantiateTile(cell);
                }

                count--;

                if (count < 0)
                {
                    count = 25;
                    await UniTask.Delay(System.TimeSpan.FromSeconds(0.00001f));
                    OnAddProgress?.Invoke(stepProgress);
                }
            }
        }
    }

    // IEnumerator CheckEntropy()
    async UniTask CheckEntropy(CancellationTokenSource cancelToken)
    {
        int maxCountIteration = _gameManager.LevelConfig.gridSize.x * _gameManager.LevelConfig.gridSize.y * _gameManager.LevelConfig.gridSize.z;
        
        List<Cell3D> tempGrid = new List<Cell3D>(gridComponents);

        tempGrid.RemoveAll(c => c.collapsed);
        var nullOptions = tempGrid.RemoveAll(c => c.tileOptions.Length == 0);
            Debug.Log($"nullOptions.Length={nullOptions}");

        tempGrid.Sort((a, b) => { return a.tileOptions.Length - b.tileOptions.Length; });

        tempGrid.RemoveAll(a => a.tileOptions.Length != tempGrid[0].tileOptions.Length);

        if (tempGrid.Count > 0)
        {
            // int arrLength = tempGrid[0].tileOptions.Length;
            // int stopIndex = default;

            // for (int i = 1; i < tempGrid.Count; i++)
            // {
            //     if (tempGrid[i].tileOptions.Length > arrLength)

            //     {
            //         stopIndex = i;
            //         break;
            //     }
            // }

            // if (stopIndex > 0)
            // {
            //     tempGrid.RemoveRange(stopIndex, tempGrid.Count - stopIndex);
            // }
            Debug.Log($"tempGrid[0].tileOptions.Length={tempGrid[0].tileOptions.Length}/{tempGrid[0].collapsed}");
        
            // yield return new WaitForSeconds(0.0001f);
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.01f));
            // await UniTask.NextFrame();

            CollapseCell(tempGrid);


            iterations++;
            OnAddProgress(0.0001f);
            if(iterations < maxCountIteration)
            {
                // StartCoroutine(CheckEntropy());
                await CheckEntropy(cancelToken);
            }
        }
        else
        {
            // сохраняем созданные тайлы.
            OnSaveTiled();

            Debug.LogWarning($"Подбор тайлов остановлен на {iterations} итерации, так как оставшиеся ячейки не содержат тайлов для подбора!");
            
            OnCompleteTiled?.Invoke();
        }
    }

    void CollapseCell(List<Cell3D> tempGrid)
    {
        if (tempGrid.Count > 0)
        {
            int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);

            Cell3D cellToCollapse = tempGrid[0];

            cellToCollapse.collapsed = true;
            
            // Debug.Log($"Collapse.tileOptions.length= {cellToCollapse.tileOptions.Length} : tempGrid.Count= {tempGrid.Count}");
            if (cellToCollapse.position.y > 0)
            {
                Tile3D[] availableTiles = cellToCollapse.tileOptions.Where(t => t.isTop == cellToCollapse.isTop).ToArray();
                cellToCollapse.tileOptions = availableTiles;
            }
            InstantiateTile(cellToCollapse);

            UpdateGeneration();
        } else
        {
            // Tile3D foundTile = TilePlaceholderNone;
            //     var obj = Instantiate(foundTile, cellToCollapse.position, foundTile.transform.rotation, transform);
            //         obj.name = $"Placeholder_{cellToCollapse.position.x}x{cellToCollapse.position.y}x{cellToCollapse.position.z}__{selectedTile.name}__{cellToCollapse.rotation.y}";
                
                Debug.LogWarning($"Пропуск подбора!");
        }
    }

    private void InstantiateTile(Cell3D cellToCollapse)
    {
        if (cellToCollapse.tileOptions.Length > 0)
        {
            // Tile3D[] availableTiles = cellToCollapse.tileOptions.Where(t => t.isTop == cellToCollapse.isTop).ToArray();
            
            // if (cellToCollapse.isTop)
            // {
            //     availableTiles = cellToCollapse.tileOptions.Where(t => t.isTop).ToArray();
            // }

            Tile3D selectedTile = GetRandomTile(cellToCollapse.tileOptions);
            
            if (selectedTile)
            {
                GridTileNode node = levelManager.mapManager.gridTileHelper.GetNode(cellToCollapse.position.x, cellToCollapse.position.y, cellToCollapse.position.z);
                node.isCollapsed = true;
                //cellToCollapse.tileOptions[UnityEngine.Random.Range(0, cellToCollapse.tileOptions.Length)];
                cellToCollapse.tileOptions = new Tile3D[] { selectedTile };
                cellToCollapse.rotation = selectedTile.transform.eulerAngles;

                Tile3D foundTile = cellToCollapse.tileOptions[0];
                if (foundTile)
                {
                    var obj = Instantiate(foundTile, cellToCollapse.position, foundTile.transform.rotation, transform);
                    obj.name = $"{cellToCollapse.position.x}x{cellToCollapse.position.y}x{cellToCollapse.position.z}__{selectedTile.name}__{cellToCollapse.rotation.y}";
                }
            } else
            {
                Debug.LogWarning("Не найден префаб для создания тайла!");
            }
        }
    }

    void UpdateGeneration()
    {
        var dimensions = _gameManager.LevelConfig.gridSize.z;

        List<Cell3D> newGenerationCell = new List<Cell3D>(gridComponents);

        for (int y = 0; y < _gameManager.LevelConfig.gridSize.y; y++)
        {
            for (int z = 0; z < _gameManager.LevelConfig.gridSize.z; z++)
            {
                for (int x = 0; x < _gameManager.LevelConfig.gridSize.x; x++)
                {
                    var index = Helpers.From3DTo1D(x, y, z, _gameManager.LevelConfig.gridSize);// x + z * dimensions;

                    if (gridComponents[index].collapsed)
                    {
                        // Debug.Log("called");
                        newGenerationCell[index] = gridComponents[index];
                    }
                    else
                    {
                        List<Tile3D> options = new List<Tile3D>();
                        foreach (Tile3D t in TilePrefabs)
                        {
                            options.Add(t);
                        }

                        options.RemoveAll(t => !IsTilePossible(t, new Vector3Int(x, y, z)));

                        // //update above
                        // if (y > 0)
                        // {
                        //     Cell3D up = gridComponents[x + (y - 1) * dimensions];
                        //     List<Tile3D> validOptions = new List<Tile3D>();

                        //     // foreach (Tile3D possibleOptions in up.tileOptions)
                        //     // {
                        //     //     var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                        //     //     var valid = tileObjects[valOption].upNeighbours;

                        //     //     validOptions = validOptions.Concat(valid).ToList();
                        //     // }

                        //     // CheckValidity(options, validOptions);
                        //     options.RemoveAll(t => !IsTilePossible(t, new Vector3Int(x, 0, y - 1)));
                        // }

                        // //update right
                        // if (x < dimensions - 1)
                        // {
                        //     Cell3D right = gridComponents[x + 1 + y * dimensions];
                        //     List<Tile3D> validOptions = new List<Tile3D>();

                        //     // foreach (Tile3D possibleOptions in right.tileOptions)
                        //     // {
                        //     //     var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                        //     //     var valid = tileObjects[valOption].leftNeighbours;

                        //     //     validOptions = validOptions.Concat(valid).ToList();
                        //     // }

                        //     // CheckValidity(options, validOptions);
                            
                        //     options.RemoveAll(t => !IsTilePossible(t, new Vector3Int(x + 1, 0, y)));
                        // }

                        // //look down
                        // if (y < dimensions - 1)
                        // {
                        //     Cell3D down = gridComponents[x + (y + 1) * dimensions];
                        //     List<Tile3D> validOptions = new List<Tile3D>();

                        //     // foreach (Tile3D possibleOptions in down.tileOptions)
                        //     // {
                        //     //     var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                        //     //     var valid = tileObjects[valOption].downNeighbours;

                        //     //     validOptions = validOptions.Concat(valid).ToList();
                        //     // }

                        //     // CheckValidity(options, validOptions);
                        //     options.RemoveAll(t => !IsTilePossible(t, new Vector3Int(x, 0, y + 1)));
                        // }

                        // //look left
                        // if (x > 0)
                        // {
                        //     Cell3D left = gridComponents[x - 1 + y * dimensions];
                        //     List<Tile3D> validOptions = new List<Tile3D>();

                        //     // foreach (Tile3D possibleOptions in left.tileOptions)
                        //     // {
                        //     //     var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                        //     //     var valid = tileObjects[valOption].rightNeighbours;

                        //     //     validOptions = validOptions.Concat(valid).ToList();
                        //     // }

                        //     // CheckValidity(options, validOptions);
                        //     options.RemoveAll(t => !IsTilePossible(t, new Vector3Int(x - 1, 0, y)));
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

        gridComponents = newGenerationCell.ToArray();
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

    private bool IsTilePossible(Tile3D tile, Vector3Int position)
    {
        // var dimensions = _gameManager.LevelConfig.gridSize.z;
        int x = position.x;
        int y = position.y;
        int z = position.z;
        int index = default;

        // Debug.Log($"dismension={dimensions}, position={position}");
        
        // index = x - 1 + z * dimensions;
        GridTileNode nodeLeft = levelManager.mapManager.gridTileHelper.GetNode(x - 1, y, z);
        if (nodeLeft != default && nodeLeft.isCollapsed)
        {
            index = Helpers.From3DTo1D(x - 1, y, z, _gameManager.LevelConfig.gridSize);
            // Debug.Log($"{x+z*dimensions}>>>>>>{DirectionSideTile.Right}>>>>>>>{index}");
            bool isAllRightImpossible = gridComponents[index].tileOptions // possibleTiles[position.x - 1, position.z]
                .All(t => !CanAppendTile(tile, t, DirectionSideTile.Left));
            if (isAllRightImpossible) return false;
        }
        
        // index = x + 1 + z * dimensions;
        GridTileNode nodeRight = levelManager.mapManager.gridTileHelper.GetNode(x + 1, y, z);
        if (nodeRight != default && nodeRight.isCollapsed)
        {
            index = Helpers.From3DTo1D(x + 1, y, z, _gameManager.LevelConfig.gridSize);
            // Debug.Log($"{x+z*dimensions}>>>>>>{DirectionSideTile.Left}>>>>>>>{index}");
            bool isAllLeftImpossible = gridComponents[index].tileOptions // possibleTiles[position.x + 1, position.z]
                .All(t => !CanAppendTile(tile, t, DirectionSideTile.Right));
            if (isAllLeftImpossible) return false;
        }

        // index = x + (z - 1) * dimensions;
        GridTileNode nodeForward = levelManager.mapManager.gridTileHelper.GetNode(x, y, z - 1);
        if (nodeForward != default && nodeForward.isCollapsed)
        {
            index = Helpers.From3DTo1D(x, y, z - 1, _gameManager.LevelConfig.gridSize);
            // Debug.Log($"{x+z*dimensions}>>>>>>{DirectionSideTile.Forward}>>>>>>>{index}");
            bool isAllForwardImpossible = gridComponents[index].tileOptions // possibleTiles[position.x, position.z - 1]
                .All(t => !CanAppendTile(tile, t, DirectionSideTile.Forward));
            if (isAllForwardImpossible) return false;
        }
        
        // index = x + (z + 1) * dimensions;
        GridTileNode nodeBack = levelManager.mapManager.gridTileHelper.GetNode(x, y, z + 1);
        if (nodeBack != default && nodeBack.isCollapsed)
        {
            index = Helpers.From3DTo1D(x, y, z + 1, _gameManager.LevelConfig.gridSize);
            // Debug.Log($"{x+z*dimensions}>>>>>>{DirectionSideTile.Back}>>>>>>>{index}");
            bool isAllBackImpossible = gridComponents[index].tileOptions // possibleTiles[position.x, position.z + 1]
                .All(t => !CanAppendTile(tile, t, DirectionSideTile.Back));
            if (isAllBackImpossible) return false;
        }

        // check top node.
        GridTileNode nodeBottom = levelManager.mapManager.gridTileHelper.GetNode(x, y - 1, z);
        if (nodeBottom != default && nodeBottom.isCollapsed)
        {
            index = Helpers.From3DTo1D(x, y - 1, z, _gameManager.LevelConfig.gridSize);
            bool isAllBackImpossible = gridComponents[index].tileOptions // possibleTiles[position.x, position.z + 1]
                .All(t => !CanAppendTile(tile, t, DirectionSideTile.Bottom));
            if (isAllBackImpossible) return false;
        }

        
        // check bottom node.
        GridTileNode nodeTop = levelManager.mapManager.gridTileHelper.GetNode(x, y + 1, z);
        if (nodeTop != default && nodeTop.isCollapsed)
        {
            index = Helpers.From3DTo1D(x, y + 1, z, _gameManager.LevelConfig.gridSize);
            bool isAllBackImpossible = gridComponents[index].tileOptions // possibleTiles[position.x, position.z + 1]
                .All(t => !CanAppendTile(tile, t, DirectionSideTile.Top));
            if (isAllBackImpossible) return false;
        }

        return true;
    }

    private bool CanAppendTile(Tile3D existingTile, Tile3D tileToAppend, DirectionSideTile direction)
    {
        // if (existingTile == null) return true;

        if (direction == DirectionSideTile.Left)
        {
            return HelperVoxel.AreColorEqual(existingTile.ColorsLeft, tileToAppend.ColorsRight);
            //Enumerable.SequenceEqual(existingTile.ColorsRight, tileToAppend.ColorsLeft);
        }
        else if (direction == DirectionSideTile.Right)
        {
            return HelperVoxel.AreColorEqual(existingTile.ColorsRight, tileToAppend.ColorsLeft);
            // Enumerable.SequenceEqual(existingTile.ColorsLeft, tileToAppend.ColorsRight);
        }
        else if (direction == DirectionSideTile.Forward)
        {
            return HelperVoxel.AreColorEqual(existingTile.ColorsForward, tileToAppend.ColorsBack);
            // Enumerable.SequenceEqual(existingTile.ColorsForward, tileToAppend.ColorsBack);
        }
        else if (direction == DirectionSideTile.Back)
        {
            return HelperVoxel.AreColorEqual(existingTile.ColorsBack, tileToAppend.ColorsForward);
            // Enumerable.SequenceEqual(existingTile.ColorsBack, tileToAppend.ColorsForward);
        }
        else if (direction == DirectionSideTile.Top)
        {
            return HelperVoxel.AreColorEqual(existingTile.ColorsTop, tileToAppend.ColorsBottom);
        }
        else if (direction == DirectionSideTile.Bottom)
        {
            return HelperVoxel.AreColorEqual(existingTile.ColorsBottom, tileToAppend.ColorsTop);
        }
        else
        {
            throw new ArgumentException("Wrong direction value, should be Vector3.left/right/back/forward",
                nameof(direction));
        }
    }
    
    private void OnSaveTiled()
    {
        Cell3DData[] cellsData = new Cell3DData[gridComponents.Length];

        for (int i = 0; i < gridComponents.Length; i++)
        {
            Tile3D uidPrefabTile = gridComponents[i].tileOptions.Length > 0 ? gridComponents[i].tileOptions[0] : null;
            if (gridComponents[i].tileOptions.Length == 0)
            {
                Debug.LogWarning($"Не удалось найти подходящий префаб для позиции: {gridComponents[i].position}! Индекс в массиве -  {i}");
                
            }

            if (!gridComponents[i].collapsed)
            {
                 Tile3D foundTile = TilePlaceholderNone;
                Instantiate(foundTile, gridComponents[i].position, foundTile.transform.rotation, transform);
            }

            GridTileNode node = levelManager.mapManager.gridTileHelper.GetNode(gridComponents[i].position.x, gridComponents[i].position.y, gridComponents[i].position.z);   
            cellsData[i] = new Cell3DData()
            {
                position = gridComponents[i].position,
                uid = uidPrefabTile != null ? uidPrefabTile.UID : "",
                stateNode = (int)node.StateNode,
                RotationY = uidPrefabTile != null ? uidPrefabTile.transform.eulerAngles.y : 0
            };
        }

        _gameManager.LevelConfig.saveTiled = new SaveTiled()
        {
            nameMap = _gameManager.LevelConfig.tileSettings.nameMap,
            gridComponents = cellsData
        };
    }


    private Tile3D GetRandomTile(Tile3D[] availableTiles)
    {
        List<float> chances = new List<float>();
        for (int i = 0; i < availableTiles.Length; i++)
        {
            chances.Add(availableTiles[i].Weight);
        }

        float value = UnityEngine.Random.Range(0, chances.Sum());
        float sum = 0;

        for (int i = 0; i < chances.Count; i++)
        {
            sum += chances[i];
            if (value < sum)
            {
                return availableTiles[i];
            }
        }

        return availableTiles[availableTiles.Length - 1];
    }
}