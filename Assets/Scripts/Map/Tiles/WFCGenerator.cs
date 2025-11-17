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
    public Cell3D[] gridComponents;
    System.Threading.CancellationTokenSource cts;

    int iterations = 0;

    void Awake()
    {
        cts = new CancellationTokenSource();
        TilePrefabs = new List<Tile3D>(_gameManager.LevelConfig.TilePrefabs);
        TilePrefabsEmpty = new List<Tile3D>(_gameManager.LevelConfig.TilePrefabsEmpty);
        TilePrefabsInner = new List<Tile3D>(_gameManager.LevelConfig.TilePrefabsInner);
    }

    void OnDestroy()
    {
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
                gridComponents = new Cell3D[_gameManager.LevelConfig.gridSize.x * _gameManager.LevelConfig.gridSize.z];

                TilePrefabs.AddRange(_gameManager.LevelConfig.TilePrefabs);


                if (_gameManager.LevelConfig.saveTiled.nameMap == _gameManager.LevelConfig.tileSettings.nameMap)
                {
                    OnSetNotify?.Invoke("loadMap");
                    // если в сохраненных тайлах находится карта с текущим именем,
                    // значит карта уже сгенерирована и осталось только создать игровые объекты.
                    var savedTiles = _gameManager.LevelConfig.saveTiled.gridComponents;

                    Stack<Cell3D> stackCellNeedCreate = new Stack<Cell3D>();
                    for (int i = 0; i < savedTiles.Length; i++)
                    {
                        Cell3DData cell3DData = savedTiles[i];
                        Vector3Int position = new Vector3Int(cell3DData.position.x, 0, cell3DData.position.z);
                        GridTileNode node = levelManager.mapManager.gridTileHelper.GetNode(cell3DData.position.x, cell3DData.position.z);

                        Cell3D loadCell = new Cell3D();
                        List<Tile3D> tileForCell = new List<Tile3D>();

                        switch (cell3DData.stateNode)
                        {
                            case (int)StateNode.TiledInner:
                                node.StateNode = StateNode.TiledInner;
                                tileForCell = TilePrefabsInner.FindAll(x => x.UID == cell3DData.uid && x.transform.rotation.eulerAngles.y == cell3DData.RotationY);
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
        for (int z = 0; z < _gameManager.LevelConfig.gridSize.z; z++)
        {
            for (int x = 0; x < _gameManager.LevelConfig.gridSize.x; x++)
            {
                var position = new Vector3Int(x, 0, z);
                GridTileNode node = levelManager.mapManager.gridTileHelper.GetNode(x, z);
                Cell3D newCell = new Cell3D(); //Instantiate(_gameManager.LevelConfig.prefabPlaceholder, position, Quaternion.identity);

                if (node.StateNode.HasFlag(StateNode.Tiled))
                {
                    newCell.CreateCell(false, TilePrefabs, position, Vector3.zero);
                }
                else if (node.StateNode.HasFlag(StateNode.TiledInner))
                {
                    newCell.CreateCell(true, TilePrefabsInner, position, Vector3.zero);
                    node.isCollapsed = true;

                    // Tile3D selectedTile = GetRandomTile(newCell.tileOptions);
                    // newCell.tileOptions = new Tile3D[] { selectedTile };
                    // var obj = Instantiate(newCell.tileOptions[0], newCell.position, newCell.tileOptions[0].transform.rotation, transform);
                    // obj.name = $"{newCell.tileOptions[0].transform.position.x}x{newCell.tileOptions[0].transform.position.z}__{selectedTile.name}";
                }
                else
                {
                    newCell.CreateCell(true, TilePrefabsEmpty, position, Vector3.zero);
                    node.isCollapsed = true;

                    // Tile3D selectedTile = GetRandomTile(newCell.tileOptions);
                    // newCell.tileOptions = new Tile3D[] { selectedTile };
                    // var obj = Instantiate(newCell.tileOptions[0], newCell.position, newCell.tileOptions[0].transform.rotation, transform);
                    // obj.name = $"{newCell.position.x}x{newCell.position.z}__{selectedTile.name}";
                }
                int index = x + z * _gameManager.LevelConfig.gridSize.z;
                gridComponents[index] = newCell;
            }
        }

        // находим все закрытые ячейки и создаем для них игровые объекты.
        var collapsedCell = gridComponents.ToList().Where(t => t.collapsed);
        Stack<Cell3D> stackCellNeedCreate = new Stack<Cell3D>();
        for (int i = 0; i < gridComponents.Length; i++)
        {
            if (gridComponents[i].collapsed)
            {
                stackCellNeedCreate.Push(gridComponents[i]);
            }
        }

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
                GridTileNode node = levelManager.mapManager.gridTileHelper.GetNode(cell.position.x, cell.position.z);

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
        List<Cell3D> tempGrid = new List<Cell3D>(gridComponents);

        tempGrid.RemoveAll(c => c.collapsed);

        tempGrid.Sort((a, b) => { return a.tileOptions.Length - b.tileOptions.Length; });

        if (tempGrid.Count > 0)
        {


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
        
            // yield return new WaitForSeconds(0.0001f);
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.01f));
            // await UniTask.NextFrame();

            CollapseCell(tempGrid);


            iterations++;
            OnAddProgress(0.0001f);
            if(iterations < _gameManager.LevelConfig.gridSize.x * _gameManager.LevelConfig.gridSize.z)
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

            Cell3D cellToCollapse = tempGrid[randIndex];

            cellToCollapse.collapsed = true;

            // Debug.Log($"Collapse.tileOptions.length= {cellToCollapse.tileOptions.Length} : tempGrid.Count= {tempGrid.Count}");
            InstantiateTile(cellToCollapse);

            UpdateGeneration();
        }
    }

    private void InstantiateTile(Cell3D cellToCollapse)
    {
        if (cellToCollapse.tileOptions.Length > 0)
        {

            Tile3D selectedTile = GetRandomTile(cellToCollapse.tileOptions);
            if (selectedTile)
            {
                GridTileNode node = levelManager.mapManager.gridTileHelper.GetNode(cellToCollapse.position.x, cellToCollapse.position.z);
                node.isCollapsed = true;
                //cellToCollapse.tileOptions[UnityEngine.Random.Range(0, cellToCollapse.tileOptions.Length)];
                cellToCollapse.tileOptions = new Tile3D[] { selectedTile };
                cellToCollapse.rotation = selectedTile.transform.eulerAngles;

                Tile3D foundTile = cellToCollapse.tileOptions[0];
                if (foundTile)
                {
                    var obj = Instantiate(foundTile, cellToCollapse.position, foundTile.transform.rotation, transform);
                    obj.name = $"{cellToCollapse.position.x}x{cellToCollapse.position.z}__{selectedTile.name}__{cellToCollapse.rotation.y}";
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

        for (int z = 0; z < _gameManager.LevelConfig.gridSize.z; z++)
        {
            for (int x = 0; x < _gameManager.LevelConfig.gridSize.x; x++)
            {
                var index = x + z * dimensions;

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

                    options.RemoveAll(t => !IsTilePossible(t, new Vector3Int(x, 0, z)));

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
        var dimensions = _gameManager.LevelConfig.gridSize.z;
        int x = position.x;
        int y = position.y;
        int z = position.z;
        int index = default;

        // Debug.Log($"dismension={dimensions}, position={position}");
        
        index = x - 1 + z * dimensions;
        GridTileNode nodeRight = levelManager.mapManager.gridTileHelper.GetNode(x - 1, z);
        if (nodeRight != default && nodeRight.isCollapsed)
        {
            // Debug.Log($"{x+z*dimensions}>>>>>>{DirectionSideTile.Right}>>>>>>>{index}");
            bool isAllRightImpossible = gridComponents[index].tileOptions // possibleTiles[position.x - 1, position.z]
                .All(rightTile => !CanAppendTile(tile, rightTile, DirectionSideTile.Right));
            if (isAllRightImpossible) return false;
        }
        
        index = x + 1 + z * dimensions;
        GridTileNode nodeLeft = levelManager.mapManager.gridTileHelper.GetNode(x + 1, z);
        if (nodeLeft != default && nodeLeft.isCollapsed)
        {
            // Debug.Log($"{x+z*dimensions}>>>>>>{DirectionSideTile.Left}>>>>>>>{index}");
            bool isAllLeftImpossible = gridComponents[index].tileOptions // possibleTiles[position.x + 1, position.z]
                .All(leftTile => !CanAppendTile(tile, leftTile, DirectionSideTile.Left));
            if (isAllLeftImpossible) return false;
        }

        index = x + (z - 1) * dimensions;
        GridTileNode nodeForward = levelManager.mapManager.gridTileHelper.GetNode(x, z - 1);
        if (nodeForward != default && nodeForward.isCollapsed)
        {
            // Debug.Log($"{x+z*dimensions}>>>>>>{DirectionSideTile.Forward}>>>>>>>{index}");
            bool isAllForwardImpossible = gridComponents[index].tileOptions // possibleTiles[position.x, position.z - 1]
                .All(fwdTile => !CanAppendTile(tile, fwdTile, DirectionSideTile.Forward));
            if (isAllForwardImpossible) return false;
        }
        
        index = x + (z + 1) * dimensions;
        GridTileNode nodeBack = levelManager.mapManager.gridTileHelper.GetNode(x, z + 1);
        if (nodeBack != default && nodeBack.isCollapsed)
        {
            // Debug.Log($"{x+z*dimensions}>>>>>>{DirectionSideTile.Back}>>>>>>>{index}");
            bool isAllBackImpossible = gridComponents[index].tileOptions // possibleTiles[position.x, position.z + 1]
                .All(backTile => !CanAppendTile(tile, backTile, DirectionSideTile.Back));
            if (isAllBackImpossible) return false;
        }

        return true;
    }

    private bool CanAppendTile(Tile3D existingTile, Tile3D tileToAppend, DirectionSideTile direction)
    {
        if (existingTile == null) return true;

        if (direction == DirectionSideTile.Right)
        {
            return HelperVoxel.AreColorEqual(existingTile.ColorsLeft, tileToAppend.ColorsRight);
            //Enumerable.SequenceEqual(existingTile.ColorsRight, tileToAppend.ColorsLeft);
        }
        else if (direction == DirectionSideTile.Left)
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

            GridTileNode node = levelManager.mapManager.gridTileHelper.GetNode(gridComponents[i].position.x, gridComponents[i].position.z);   
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