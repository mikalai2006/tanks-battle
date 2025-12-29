// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using Mikalai2006.Voxel;
// using UnityEngine;
// using Random = UnityEngine.Random;

// public class Tile3DGenerator : MonoBehaviour
// {
//     GameManager _gameManager => GameManager.Instance;
//     public LevelManager levelManager;
//     public List<Tile3D> TilePrefabs;
//     public List<Tile3D> TilePrefabsEmpty;
//     public List<Tile3D> TilePrefabsInner;

//     private Tile3D[,] spawnedTiles;

//     private Queue<Vector3Int> recalcPossibleTilesQueue = new Queue<Vector3Int>();
//     private List<Tile3D>[,] possibleTiles;
//     private Dictionary<Vector3Int, int> countProbePosition;
//     private Dictionary<Vector3Int, Tile3D> collapsedPosition;

//     private void Start()
//     {
//         countProbePosition = new Dictionary<Vector3Int, int>();
//         collapsedPosition = new Dictionary<Vector3Int, Tile3D>();

//         TilePrefabs = new List<Tile3D>(_gameManager.LevelConfig.TilePrefabs);
//         TilePrefabsEmpty = new List<Tile3D>(_gameManager.LevelConfig.TilePrefabsEmpty);
//         TilePrefabsInner = new List<Tile3D>(_gameManager.LevelConfig.TilePrefabsInner);

//         spawnedTiles = new Tile3D[_gameManager.LevelConfig.gridSize.x, _gameManager.LevelConfig.gridSize.z];

//         // foreach (Tile3D tilePrefab in TilePrefabs)
//         // {
//         //     tilePrefab.CalculateSidesColors();
//         // }

//         int countBeforeAdding = _gameManager.LevelConfig.TilePrefabs.Count;
//         for (int i = 0; i < countBeforeAdding; i++)
//         {

//             Vector3 pos = new Vector3(0, -100, 0);
//             switch (_gameManager.LevelConfig.TilePrefabs[i].Rotation)
//             {
//                 case Tile3D.RotationType.OnlyRotation:
//                     break;

//                 case Tile3D.RotationType.TwoRotations:
//                     _gameManager.LevelConfig.TilePrefabs[i].Weight /= 2;
//                     if (_gameManager.LevelConfig.TilePrefabs[i].Weight <= 0) _gameManager.LevelConfig.TilePrefabs[i].Weight = 1;

//                     var clone = Instantiate(_gameManager.LevelConfig.TilePrefabs[i], pos,// + Vector3.right,
//                         Quaternion.identity, transform);
//                     clone.name = $"{clone.name}_90";
//                     clone.OnStart();
//                     clone.Rotate90();
//                     TilePrefabs.Add(clone);
//                     break;

//                 case Tile3D.RotationType.FourRotations:
//                     _gameManager.LevelConfig.TilePrefabs[i].Weight /= 4;
//                     if (_gameManager.LevelConfig.TilePrefabs[i].Weight <= 0) _gameManager.LevelConfig.TilePrefabs[i].Weight = 1;

//                     var clone2 = Instantiate(_gameManager.LevelConfig.TilePrefabs[i], pos,// + Vector3.right,
//                         Quaternion.identity, transform);
//                     clone2.name = $"{clone2.name}_90";
//                     clone2.OnStart();
//                     clone2.Rotate90();
//                     TilePrefabs.Add(clone2);

//                     var clone3 = Instantiate(_gameManager.LevelConfig.TilePrefabs[i], pos,// + Vector3.right * 2,
//                         Quaternion.identity, transform);
//                     clone3.name = $"{clone3.name}_180";
//                     clone3.OnStart();
//                     clone3.Rotate90();
//                     clone3.Rotate90();
//                     TilePrefabs.Add(clone3);

//                     var clone4 = Instantiate(_gameManager.LevelConfig.TilePrefabs[i], pos,// + Vector3.right * 3,
//                         Quaternion.identity, transform);
//                     clone4.name = $"{clone4.name}_270";
//                     clone4.OnStart();
//                     clone4.Rotate90();
//                     clone4.Rotate90();
//                     clone4.Rotate90();
//                     TilePrefabs.Add(clone4);
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException();
//             }
//         }

//     }

//     public void CreateMap()
//     {
//         // Generate();
//         // StartCoroutine(GenerateSimple());
//     }


//     // void InitializeGrid()
//     // {
//     //     // ... (Код для создания сетки и заполнения PossibleTiles для каждой ячейки всеми параметрами TileSet)
//     //     for (int x = 0; x < _gameManager.LevelConfig.gridSize.x; x++)
//     //     for (int z = 0; z < _gameManager.LevelConfig.gridSize.z; z++)
//     //     {
//     //         GridTileNode node = levelManager.mapManager.gridTileHelper.GetNode(x, z);
//     //         if (node.StateNode.HasFlag(StateNode.Tiled))
//     //         {
//     //             possibleTiles[x, z] = new List<Tile3D>(TilePrefabs);
//     //         }
//     //         else if (node.StateNode.HasFlag(StateNode.TiledInner))
//     //         {
//     //                 possibleTiles[x, z] = new List<Tile3D>(TilePrefabsInner);

//     //         }
//     //         else
//     //         {
//     //             possibleTiles[x, z] = new List<Tile3D>(TilePrefabsEmpty);
//     //         }
//     //         // possibleTiles[x, z] = new List<Tile3D>(TilePrefabs);
//     //     }
//     // }

//     // void GenerateMap()
//     // {
//     //     while (!AllCellsCollapsed())
//     //     {
//     //         GridTileNode cellToCollapse = FindLowestEntropyCell();
//     //         if (cellToCollapse == null) break; // Больше нет ячеек для схлопывания или возникло противоречие

//     //         CollapseCell(cellToCollapse);
//     //         PropagateConstraints(cellToCollapse);
//     //     }
//     // }

//     // List<Tile3D> FindLowestEntropyCell()
//     // {
//     //     // ... (Код для поиска ячейки с наименьшим оставшимся числом PossibleTiles)
//     //     List<Tile3D> minCountTile = possibleTiles[1, 1];
//     //     Vector3Int minCountTilePosition = new Vector3Int(1, 0, 1);

//     //     for (int x = 1; x < _gameManager.LevelConfig.gridSize.x - 1; x++)
//     //     for (int z = 1; z < _gameManager.LevelConfig.gridSize.z - 1; z++)
//     //     {
//     //         if (possibleTiles[x, z].Count < minCountTile.Count)
//     //         {
//     //             minCountTile = possibleTiles[x, z];
//     //             minCountTilePosition = new Vector3Int(x, 0, z);
//     //         }
//     //     }
//     //     return minCountTile;
//     // }

//     // void CollapseCell(WFCCell cell)
//     // {
//     //     // ... (Код для случайного выбора плитки из PossibleTiles, установки ChosenTile и создания экземпляра префаба)
//     // }

//     // void PropagateConstraints(WFCCell collapsedCell)
//     // {
//     //     // ... (Код для обновления PossibleTiles соседних ячеек на основе правил смежности)
//     // }

//     // bool AllCellsCollapsed()
//     // {
//     //     var possibleTilesWithOne = possibleTiles
//     //     // ... (Код для проверки того, для всех ли ячеек сетки свойство IsCollapsed установлено в значение true)
//     //     return false;
//     // }




//     // private void Update()
//     // {
//     //     if (Input.GetKeyDown(KeyCode.D))
//     //     {
//     //         foreach (Tile3D spawnedTile in spawnedTiles)
//     //         {
//     //             if (spawnedTile != null) Destroy(spawnedTile.gameObject);
//     //         }

//     //         Generate();
//     //     }
//     // }

//     private void Generate()
//     {
//         possibleTiles = new List<Tile3D>[_gameManager.LevelConfig.gridSize.x, _gameManager.LevelConfig.gridSize.z];

//         int maxAttempts = 10;
//         int attempts = 0;
//         Vector3Int startPosition = Vector3Int.zero;
//         while (attempts++ < maxAttempts)
//         {
//             for (int y = 0; y < _gameManager.LevelConfig.gridSize.y; y++)
//             {
//                 for (int x = 0; x < _gameManager.LevelConfig.gridSize.x; x++)
//                     for (int z = 0; z < _gameManager.LevelConfig.gridSize.z; z++)
//                     {
//                         GridTileNode node = levelManager.mapManager.gridTileHelper.GetNode(x, z, y);
//                         if (node.StateNode.HasFlag(StateNode.Tiled))
//                         {
//                             possibleTiles[x, z] = new List<Tile3D>(TilePrefabs);
//                             if (startPosition == Vector3Int.zero && x > 0 && z > 0) startPosition = new Vector3Int(x, 0, z);
//                         }
//                         else if (node.StateNode.HasFlag(StateNode.TiledInner))
//                         {
//                             possibleTiles[x, z] = new List<Tile3D>(TilePrefabsInner);
//                         }
//                         else
//                         {
//                             possibleTiles[x, z] = new List<Tile3D>(TilePrefabsEmpty);
//                         }
//                         // possibleTiles[x, z] = new List<Tile3D>(TilePrefabs);
//                     }
//             }
//             // Tile3D tileInCenter = GetRandomTile(TilePrefabs);
//             // possibleTiles[_gameManager.LevelConfig.gridSize.x / 2, _gameManager.LevelConfig.gridSize.z / 2] = new List<Tile3D> {tileInCenter};
//             Tile3D startTile = GetRandomTile(TilePrefabs);
//             possibleTiles[startPosition.x, startPosition.z] = new List<Tile3D> { startTile };

//             recalcPossibleTilesQueue.Clear();
//             // EnqueueNeighboursToRecalc(new Vector3Int(_gameManager.LevelConfig.gridSize.x / 2, 0, _gameManager.LevelConfig.gridSize.z / 2));
//             EnqueueNeighboursToRecalc(startPosition);
//             Debug.Log($"Start Position={startPosition}");

//             bool success = GenerateAllPossibleTiles();

//             if (success) break;
//         }

//         PlaceAllTiles();
//     }

//     private bool GenerateAllPossibleTiles()
//     {
//         int maxIterations = _gameManager.LevelConfig.gridSize.x * _gameManager.LevelConfig.gridSize.z;
//         int iterations = 0;
//         int backtracks = 0;
        
//         while (iterations++ < maxIterations)
//         {
//             int maxInnerIterations = 50;
//             int innerIterations = 0;

//             while (recalcPossibleTilesQueue.Count > 0 && innerIterations++ < maxInnerIterations)
//             {
//                 Vector3Int position = recalcPossibleTilesQueue.Dequeue();
//                 GridTileNode node = levelManager.mapManager.gridTileHelper.GetNode(position.x, position.z, position.y);
//                 // Debug.Log($"PosibleTiles:::::::::::::::::{position}");


//                 if (countProbePosition.ContainsKey(position))
//                 {
//                     countProbePosition[position]++;
//                     // continue;
//                 } else
//                 {
//                     countProbePosition.Add(position, 1);
//                 }

//                 // if (!node.StateNode.HasFlag(StateNode.Tiled))
//                 // {
//                 //     // innerIterations = maxInnerIterations;
//                 //     // possibleTiles[position.x, position.z].Clear();
//                 //     // continue;
//                 //     break;
//                 // }
//                 // if (!node.StateNode.HasFlag(StateNode.Tiled))
//                 // {
//                 //     continue;
//                 // }
//                 if (!node.StateNode.HasFlag(StateNode.Tiled))
//                 {
//                     EnqueueNeighboursToRecalc(position);
//                     continue;
//                 }

//                 if (position.x == 0 || position.z == 0 ||
//                     position.x == _gameManager.LevelConfig.gridSize.x - 1 || position.z == _gameManager.LevelConfig.gridSize.z - 1
//                     )
//                 {
//                     continue;
//                 }

//                 List<Tile3D> possibleTilesHere = possibleTiles[position.x, position.z];

//                 if (node.StateNode.HasFlag(StateNode.Tiled))
//                 {
//                     int countRemoved = possibleTilesHere.RemoveAll(t => !IsTilePossible(t, position));

//                     if (countRemoved > 0) EnqueueNeighboursToRecalc(position);
//                     // Debug.Log($"PosibleTiles={possibleTilesHere.Count}, countRemoved={countRemoved}, node.StateNode={node.StateNode}[{position}]<{countProbePosition[position]}>");
//                 }

//                 if (possibleTilesHere.Count == 0)
//                 {
//                     // Зашли в тупик, в этих координатах невозможен ни один тайл. Попробуем ещё раз, разрешим все тайлы
//                     // в этих и соседних координатах, и посмотрим устаканится ли всё
//                     if (node.StateNode.HasFlag(StateNode.Tiled))
//                     {
//                         possibleTilesHere.AddRange(TilePrefabs);
//                     }
//                     // else if (node.StateNode.HasFlag(StateNode.TiledInner))
//                     // {
//                     //     possibleTilesHere.AddRange(TilePrefabsInner);
//                     // }
//                     // else
//                     // {
//                     //     possibleTilesHere.AddRange(TilePrefabsEmpty);
//                     // }
                   
//                     Vector3Int pos1 = new Vector3Int(position.x + 1, 0, position.z);
//                     GridTileNode node1 = levelManager.mapManager.gridTileHelper.GetNode(pos1.x, pos1.z, pos1.y);
//                     if (node1.StateNode.HasFlag(StateNode.Tiled))
//                     {
//                         possibleTiles[position.x + 1, position.z] = new List<Tile3D>(TilePrefabs);
//                     } else
//                     {
//                         // possibleTiles[position.x + 1, position.z] = new List<Tile3D>(TilePrefabsEmpty);
//                     }
//                     Vector3Int pos2 = new Vector3Int(position.x - 1, 0, position.z);
//                     GridTileNode node2 = levelManager.mapManager.gridTileHelper.GetNode(pos2.x, pos2.z, pos2.y);
//                     if (node2.StateNode.HasFlag(StateNode.Tiled))
//                     {
//                         possibleTiles[position.x - 1, position.z] = new List<Tile3D>(TilePrefabs);
//                     } else
//                     {
//                         // possibleTiles[position.x - 1, position.z] = new List<Tile3D>(TilePrefabsEmpty);
//                     }
//                     Vector3Int pos3 = new Vector3Int(position.x, 0, position.z + 1);
//                     GridTileNode node3 = levelManager.mapManager.gridTileHelper.GetNode(pos3.x, pos3.z, pos3.y);
//                     if (node3.StateNode.HasFlag(StateNode.Tiled))
//                     {
//                         possibleTiles[position.x, position.z + 1] = new List<Tile3D>(TilePrefabs);
//                     } else
//                     {
//                         // possibleTiles[position.x, position.z + 1] = new List<Tile3D>(TilePrefabsEmpty);
//                     }
//                     Vector3Int pos4 = new Vector3Int(position.x, 0, position.z - 1);
//                     GridTileNode node4 = levelManager.mapManager.gridTileHelper.GetNode(pos4.x, pos4.z, pos4.y);
//                     if (node4.StateNode.HasFlag(StateNode.Tiled))
//                     {
//                         possibleTiles[position.x, position.z - 1] = new List<Tile3D>(TilePrefabs);
//                     } else
//                     {
//                         // possibleTiles[position.x, position.z - 1] = new List<Tile3D>(TilePrefabsEmpty);
//                     }
//                     // possibleTiles[position.x + 1, position.z] = new List<Tile3D>(TilePrefabs);
//                     // possibleTiles[position.x - 1, position.z] = new List<Tile3D>(TilePrefabs);
//                     // possibleTiles[position.x, position.z + 1] = new List<Tile3D>(TilePrefabs);
//                     // possibleTiles[position.x, position.z - 1] = new List<Tile3D>(TilePrefabs);

//                     EnqueueNeighboursToRecalc(position);

//                     backtracks++;
//                 }
//             }

//             if (innerIterations == maxInnerIterations) break;

//             List<Tile3D> maxCountTile = possibleTiles[1, 1];
//             Vector3Int maxCountTilePosition = new Vector3Int(1, 0, 1);

//             for (int x = 1; x < _gameManager.LevelConfig.gridSize.x - 1; x++)
//             for (int z = 1; z < _gameManager.LevelConfig.gridSize.z - 1; z++)
//             {
//                 if (possibleTiles[x, z].Count > maxCountTile.Count)
//                 {
//                     maxCountTile = possibleTiles[x, z];
//                     maxCountTilePosition = new Vector3Int(x, 0, z);
//                 }
//             }

//             if (maxCountTile.Count == 1)
//             {
//                 Debug.Log($"Generated for {iterations} iterations, with {backtracks} backtracks");
//                 return true;
//             }

//             Tile3D tileToCollapse = GetRandomTile(maxCountTile);
//             possibleTiles[maxCountTilePosition.x, maxCountTilePosition.z] = new List<Tile3D> {tileToCollapse};
//             EnqueueNeighboursToRecalc(maxCountTilePosition);
//         }
        
//         Debug.Log($"Failed, run out of iterations with {backtracks} backtracks");
//         return false;
//     }

//     private bool IsTilePossible(Tile3D tile, Vector3Int position)
//     {
//         bool isAllRightImpossible = possibleTiles[position.x - 1, position.z]
//             .All(rightTile => !CanAppendTile(tile, rightTile, DirectionSideTile.Right));
//         if (isAllRightImpossible) return false;
        
//         bool isAllLeftImpossible = possibleTiles[position.x + 1, position.z]
//             .All(leftTile => !CanAppendTile(tile, leftTile, DirectionSideTile.Left));
//         if (isAllLeftImpossible) return false;
        
//         bool isAllForwardImpossible = possibleTiles[position.x, position.z - 1]
//             .All(fwdTile => !CanAppendTile(tile, fwdTile, DirectionSideTile.Forward));
//         if (isAllForwardImpossible) return false;
        
//         bool isAllBackImpossible = possibleTiles[position.x, position.z + 1]
//             .All(backTile => !CanAppendTile(tile, backTile, DirectionSideTile.Back));
//         if (isAllBackImpossible) return false;

//         return true;
//     }

//     private void PlaceAllTiles()
//     {
//         for (int x = 1; x < _gameManager.LevelConfig.gridSize.x - 1; x++)
//         for (int z = 1; z < _gameManager.LevelConfig.gridSize.z - 1; z++)
//         {
//             PlaceTile(x, 0, z);
//         }
//     }

//     private void EnqueueNeighboursToRecalc(Vector3Int position)
//     {
//         Vector3Int pos1 = new Vector3Int(position.x + 1, 0, position.z);
//         GridTileNode node1 = levelManager.mapManager.gridTileHelper.GetNode(pos1.x, pos1.z, pos1.y);
//         if (node1 != default)
//         {
//             if (node1.StateNode.HasFlag(StateNode.Tiled) || !countProbePosition.ContainsKey(pos1))
//             {
//                 recalcPossibleTilesQueue.Enqueue(pos1);
//             }
//         }

//         Vector3Int pos2 = new Vector3Int(position.x - 1, 0, position.z);
//         GridTileNode node2 = levelManager.mapManager.gridTileHelper.GetNode(pos2.x, pos2.z, pos2.y);
//         // if (node2 != default) //.StateNode.HasFlag(StateNode.Tiled))
//         if (node2 != default)
//         {
//             if (node2.StateNode.HasFlag(StateNode.Tiled) || !countProbePosition.ContainsKey(pos2))
//             {
//                 recalcPossibleTilesQueue.Enqueue(pos2);
//             }
//         }

//         Vector3Int pos3 = new Vector3Int(position.x, 0, position.z + 1);
//         GridTileNode node3 = levelManager.mapManager.gridTileHelper.GetNode(pos3.x, pos3.z, pos3.y);
//         // if (node3 != default) //.StateNode.HasFlag(StateNode.Tiled))
//         if (node3 != default)
//         {
//             if (node3.StateNode.HasFlag(StateNode.Tiled) || !countProbePosition.ContainsKey(pos3))
//             {
//                 recalcPossibleTilesQueue.Enqueue(pos3);
//             }
//         }

//         Vector3Int pos4 = new Vector3Int(position.x, 0, position.z - 1);
//         GridTileNode node4 = levelManager.mapManager.gridTileHelper.GetNode(pos4.x, pos4.z, pos4.y);
//         // if (node4 != default) //.StateNode.HasFlag(StateNode.Tiled))
//         if (node4 != default)
//         {
//             if (node4.StateNode.HasFlag(StateNode.Tiled) || !countProbePosition.ContainsKey(pos4))
//             {
//                 recalcPossibleTilesQueue.Enqueue(pos4);
//             }
//         }
//     }

    
//     public IEnumerator GenerateSimple()
//     {
//         for (int x = 1; x < _gameManager.LevelConfig.gridSize.x - 1; x++)
//         {
//             for (int z = 1; z < _gameManager.LevelConfig.gridSize.z - 1; z++)
//             {
//                 yield return new WaitForSeconds(0.02f);

//                 PlaceTileSimple(x, z);
//             }
//         }
        
//         yield return new WaitForSeconds(0.8f);
//         foreach (Tile3D spawnedTile in spawnedTiles)
//         {
//             if (spawnedTile != null) Destroy(spawnedTile.gameObject);
//         }

//         StartCoroutine(GenerateSimple());
//     }

//     private void PlaceTileSimple(int x, int z)
//     {
//         List<Tile3D> availableTiles = new List<Tile3D>();

//         foreach (Tile3D tilePrefab in TilePrefabs)
//         {
//             if (CanAppendTile(spawnedTiles[x - 1, z], tilePrefab, DirectionSideTile.Left) &&
//                 CanAppendTile(spawnedTiles[x + 1, z], tilePrefab, DirectionSideTile.Right) &&
//                 CanAppendTile(spawnedTiles[x, z - 1], tilePrefab, DirectionSideTile.Back) &&
//                 CanAppendTile(spawnedTiles[x, z + 1], tilePrefab, DirectionSideTile.Forward))
//             {
//                 availableTiles.Add(tilePrefab);

//                 Debug.Log($"============================>Tile {x},{z}: {tilePrefab.name}");
//                 if (spawnedTiles[x - 1, z] != null)
//                 {

//                     Debug.Log($"left --  {x - 1},{z} - {spawnedTiles[x - 1, z].name} - {CanAppendTile(spawnedTiles[x - 1, z], tilePrefab, DirectionSideTile.Left)}");
//                 // Debug.Log($"left ---  {spawnedTiles[x - 1, z].ColorsLeft}, {tilePrefab.ColorsLeft}");
                
//                 } else
//                 {
//                     Debug.Log($"left -  {x -1},{z} - NOne");
//                 }
//             }
//         }

//         if (availableTiles.Count == 0) return;

//         Tile3D selectedTile = GetRandomTile(availableTiles);
//         Vector3 position = _gameManager.Settings.scaleObjects * selectedTile.TileSideVoxels * new Vector3(x, 0, z);
//         spawnedTiles[x, z] = Instantiate(selectedTile, position, selectedTile.transform.rotation);
//     }

//     private void PlaceTile(int x, int y, int z)
//     {
//         if (possibleTiles[x, z].Count == 0) return;

//         Tile3D selectedTile = GetRandomTile(possibleTiles[x, z]);
//         Vector3 position = _gameManager.Settings.scaleObjects * selectedTile.TileSideVoxels * new Vector3(x, 0, z);
//         spawnedTiles[x, z] = Instantiate(selectedTile, position, selectedTile.transform.rotation, transform);
//         spawnedTiles[x, z].name = $"{x}x{z}__{spawnedTiles[x, z].name}";
//                 Debug.Log($"PlaceTile::: {possibleTiles[x, z].Count} [{position}]");
//     }

//     private Tile3D GetRandomTile(List<Tile3D> availableTiles)
//     {
//         List<float> chances = new List<float>();
//         for (int i = 0; i < availableTiles.Count; i++)
//         {
//             chances.Add(availableTiles[i].Weight);
//         }

//         float value = Random.Range(0, chances.Sum());
//         float sum = 0;

//         for (int i = 0; i < chances.Count; i++)
//         {
//             sum += chances[i];
//             if (value < sum)
//             {
//                 return availableTiles[i];
//             }
//         }

//         return availableTiles[availableTiles.Count - 1];
//     }

//     private bool CanAppendTile(Tile3D existingTile, Tile3D tileToAppend, DirectionSideTile direction)
//     {
//         if (existingTile == null) return true;

//         if (direction == DirectionSideTile.Right)
//         {
//             return HelperVoxel.AreColorEqual(existingTile.ColorsLeft, tileToAppend.ColorsRight);
//             //Enumerable.SequenceEqual(existingTile.ColorsRight, tileToAppend.ColorsLeft);
//         }
//         else if (direction == DirectionSideTile.Left)
//         {
//             return HelperVoxel.AreColorEqual(existingTile.ColorsRight, tileToAppend.ColorsLeft);
//             // Enumerable.SequenceEqual(existingTile.ColorsLeft, tileToAppend.ColorsRight);
//         }
//         else if (direction == DirectionSideTile.Forward)
//         {
//             return HelperVoxel.AreColorEqual(existingTile.ColorsForward, tileToAppend.ColorsBack);
//             // Enumerable.SequenceEqual(existingTile.ColorsForward, tileToAppend.ColorsBack);
//         }
//         else if (direction == DirectionSideTile.Back)
//         {
//             return HelperVoxel.AreColorEqual(existingTile.ColorsBack, tileToAppend.ColorsForward);
//             // Enumerable.SequenceEqual(existingTile.ColorsBack, tileToAppend.ColorsForward);
//         }
//         else if (direction == DirectionSideTile.Top)
//         {
//             return HelperVoxel.AreColorEqual(existingTile.ColorsTop, tileToAppend.ColorsBottom);
//         }
//         else if (direction == DirectionSideTile.Bottom)
//         {
//             return HelperVoxel.AreColorEqual(existingTile.ColorsBottom, tileToAppend.ColorsTop);
//         }
//         else
//         {
//             throw new ArgumentException("Wrong direction value, should be Vector3.left/right/back/forward",
//                 nameof(direction));
//         }
//     }
// }