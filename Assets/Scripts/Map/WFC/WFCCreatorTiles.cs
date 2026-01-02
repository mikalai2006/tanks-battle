using System.Collections.Generic;
using Mikalai2006.Voxel;
using UnityEditor;
using UnityEngine;

public class WFCCreatorTiles : MonoBehaviour
{
    public List<SOVoxelData> tilesConfigs;
    public GameObject wrapper;
    public int nextName;
    public string nameDirPrefabs = "Test";
    public Dictionary<string, Voxel[]> dictionarySocketsNames;
    public Material material;
    [Tooltip("Очищать ли массивы цветов границ в конфигах (больше не будет возможности использовать этот функционал)")]
    public bool isClearColors;
    [Tooltip("Добавлять ли массивы цветов границ в каждый тайл (будет много весить)")]
    public bool isAddColorsToTile;

    public void AnalyseSockets()
    {
        nextName = 0;

        dictionarySocketsNames = new Dictionary<string, Voxel[]>();

        foreach (SOVoxelData configTile in tilesConfigs)
        {
            // создаем розетки (идентификаторы для разрешения стыкования граней тайлов).
            CheckSocket(configTile.ColorsRight, ref configTile.tileSockets.posX);
            CheckSocket(configTile.ColorsLeft, ref configTile.tileSockets.negX);
            CheckSocket(configTile.ColorsForward, ref configTile.tileSockets.negZ);
            CheckSocket(configTile.ColorsBack, ref configTile.tileSockets.posZ);
            CheckSocket(configTile.ColorsTop, ref configTile.tileSockets.posY);
            CheckSocket(configTile.ColorsBottom, ref configTile.tileSockets.negY);
          
            var colorsDefault = new ColorsBorders
            {
                ColorsRight = configTile.ColorsRight,
                ColorsLeft = configTile.ColorsLeft,
                ColorsForward = configTile.ColorsForward,
                ColorsBack = configTile.ColorsBack,
            };

            Tile3D pref = CreatePrefab(configTile, 0, configTile.tileSockets, colorsDefault);

            if (configTile.Rotation == RotationType.FourRotations)
            {
                // 90 градусов.
                ColorsBorders colors90 = Rotate90Colors(colorsDefault, configTile);
                TileSockets tileSockets90 = new TileSockets
                {
                    rotation = 90,
                };
                CheckSocket(colors90.ColorsRight, ref tileSockets90.posX);
                CheckSocket(colors90.ColorsLeft, ref tileSockets90.negX);
                CheckSocket(colors90.ColorsForward, ref tileSockets90.negZ);
                CheckSocket(colors90.ColorsBack, ref tileSockets90.posZ);
                tileSockets90.posY = GetSocketByPrefixRotate(configTile.tileSockets.posY, 90);
                tileSockets90.negY = GetSocketByPrefixRotate(configTile.tileSockets.negY, 90);
                // TileSockets tileSockets90 = Rotate90Sockets(pref.tileSockets);
                Tile3D pref90 = CreatePrefab(configTile, 90, tileSockets90, colors90);

                // 180 градусов.
                ColorsBorders colors180 = Rotate90Colors(colors90, configTile);
                TileSockets tileSockets180 = new TileSockets
                {
                    rotation = 180,
                };
                CheckSocket(colors180.ColorsRight, ref tileSockets180.posX);
                CheckSocket(colors180.ColorsLeft, ref tileSockets180.negX);
                CheckSocket(colors180.ColorsForward, ref tileSockets180.negZ);
                CheckSocket(colors180.ColorsBack, ref tileSockets180.posZ);
                tileSockets180.posY = GetSocketByPrefixRotate(configTile.tileSockets.posY, 180);
                tileSockets180.negY = GetSocketByPrefixRotate(configTile.tileSockets.negY, 180);
                // TileSockets tileSockets180 = Rotate90Sockets(pref90.tileSockets);
                Tile3D pref180 = CreatePrefab(configTile, 180, tileSockets180, colors180);

                // 270 градусов.
                ColorsBorders colors270 = Rotate90Colors(colors180, configTile);
                TileSockets tileSockets270 = new TileSockets
                {
                    rotation = 270
                };
                CheckSocket(colors270.ColorsRight, ref tileSockets270.posX);
                CheckSocket(colors270.ColorsLeft, ref tileSockets270.negX);
                CheckSocket(colors270.ColorsForward, ref tileSockets270.negZ);
                CheckSocket(colors270.ColorsBack, ref tileSockets270.posZ);
                tileSockets270.posY = GetSocketByPrefixRotate(configTile.tileSockets.posY, 270);
                tileSockets270.negY = GetSocketByPrefixRotate(configTile.tileSockets.negY, 270);
                // TileSockets tileSockets270 = Rotate90Sockets(pref180.tileSockets);
                Tile3D pref270 = CreatePrefab(configTile, 270, tileSockets270, colors270);
            }

            // если нужно очищаем массивы цветов границ.
            if (isClearColors)
            {
                configTile.ColorsRight = new Voxel[0];
                configTile.ColorsLeft = new Voxel[0];
                configTile.ColorsForward = new Voxel[0];
                configTile.ColorsBack = new Voxel[0];
                configTile.ColorsTop = new Voxel[0];
                configTile.ColorsBottom = new Voxel[0];
            }
        }

    }

    /// <summary>
    /// поворот массивов цветов на 90 по часовой стрелке.
    /// </summary>
    /// <returns>ColorsBorders</returns>
    public ColorsBorders Rotate90Colors(ColorsBorders inputColors, SOVoxelData configTile)
    {
        var TileSideVoxels = configTile.Bounds.x;

        Voxel[] colorsRightNew = new Voxel[TileSideVoxels * TileSideVoxels];
        Voxel[] colorsForwardNew = new Voxel[TileSideVoxels * TileSideVoxels];
        Voxel[] colorsLeftNew = new Voxel[TileSideVoxels * TileSideVoxels];
        Voxel[] colorsBackNew = new Voxel[TileSideVoxels * TileSideVoxels];
        // Voxel[] colorsTopNew = new Voxel[TileSideVoxels * TileSideVoxels];
        // Voxel[] colorsBottomNew = new Voxel[TileSideVoxels * TileSideVoxels];

        for (int row = 0; row < TileSideVoxels; row++)
        {
            for (int column = 0; column < TileSideVoxels; column++)
            {
                // colorsRightNew[row * TileSideVoxels + column] = inputColors.ColorsForward[row * TileSideVoxels + TileSideVoxels - column - 1];
                // colorsForwardNew[row * TileSideVoxels + column] = inputColors.ColorsLeft[row * TileSideVoxels + column];
                // colorsLeftNew[row * TileSideVoxels + column] = inputColors.ColorsBack[row * TileSideVoxels + TileSideVoxels - column - 1];
                // colorsBackNew[row * TileSideVoxels + column] = inputColors.ColorsRight[row * TileSideVoxels + column];
                // // TODO
                colorsForwardNew[row * TileSideVoxels + column] = inputColors.ColorsRight[row * TileSideVoxels + column];
                colorsRightNew[row * TileSideVoxels + column] = inputColors.ColorsBack[row * TileSideVoxels + TileSideVoxels - column - 1];
                colorsBackNew[row * TileSideVoxels + column] = inputColors.ColorsLeft[row * TileSideVoxels + column];
                colorsLeftNew[row * TileSideVoxels + column] = inputColors.ColorsForward[row * TileSideVoxels + TileSideVoxels - column - 1];
                
                // colorsForwardNew[row * TileSideVoxels + column] = inputColors.ColorsRight[row * TileSideVoxels + column];
                // colorsRightNew[row * TileSideVoxels + column] = inputColors.ColorsBack[row * TileSideVoxels + column];
                // colorsBackNew[row * TileSideVoxels + column] = inputColors.ColorsLeft[row * TileSideVoxels + column];
                // colorsLeftNew[row * TileSideVoxels + column] = inputColors.ColorsForward[row * TileSideVoxels + column];
            }
        }

        return new ColorsBorders
        {
            ColorsRight = colorsRightNew,
            ColorsLeft = colorsLeftNew,
            ColorsForward = colorsForwardNew,
            ColorsBack = colorsBackNew,
        };
    }

    string GetSocketByPrefixRotate(string nameSocket, int angle)
    {
        string[] posYSlice = nameSocket.Split("_");
        string output = posYSlice[0] != "-1" ? $"{posYSlice[0]}_{angle}" : "-1";
        return output;
    }

    /// <summary>
    /// поворот розеток на 90 градусов.
    /// </summary>
    /// <param name="tileSockets"></param>
    /// <returns></returns>
    TileSockets Rotate90Sockets(TileSockets tileSockets)
    {
        string newNegZ = tileSockets.posX;
        string newPosX = tileSockets.posZ;
        string newPosZ = tileSockets.negX;
        string newNegX = tileSockets.negZ;
        

        
        int newAngle = tileSockets.rotation + 90;
        string[] posYSlice = tileSockets.posY.Split("_");
        string newPosY = posYSlice[0] != "-1" ? $"{posYSlice[0]}_{newAngle}" : "-1";
        string[] negYSlice = tileSockets.negY.Split("_");
        string newNegY = negYSlice[0] != "-1" ? $"{negYSlice[0]}_{newAngle}" : "-1";



        return new TileSockets()
        {
            name = tileSockets.name,
            posX = newPosX,
            negX = newNegX,
            posZ = newPosZ,
            negZ = newNegZ,
            posY = newPosY,
            negY = newNegY,
            rotation = newAngle,
            weight = tileSockets.weight
        };
    }

    Tile3D CreatePrefab(SOVoxelData inputConfig, int angle = 0, TileSockets tileSockets = default, ColorsBorders colorsBorders = default)
    {
        string newObjectName = $"{inputConfig.name}__{angle}";
        string OutputPath = "Assets/Prefabs";

        GameObject newObject = new GameObject(newObjectName);
        newObject.transform.parent = wrapper.transform;
        Tile3D newTile3D = newObject.AddComponent<Tile3D>();
        if (!EqualityComparer<TileSockets>.Default.Equals(tileSockets, default(TileSockets)))
        {
            newTile3D.tileSockets = tileSockets;
        } else
        {
            newTile3D.tileSockets = inputConfig.tileSockets;
        }
        
        GameObject wrapperPrefab = new GameObject("Wrapper");
        wrapperPrefab.transform.parent = newObject.transform;
        
        VoxelMeshRender newVMR = wrapperPrefab.AddComponent<VoxelMeshRender>();
        newVMR.Wrapper = wrapperPrefab;

        newVMR.Config.sOVoxelData = inputConfig;
        newVMR.Config._material = material;
        newVMR.Config.isGreedy = true;
        newVMR.Config.isOneMesh = true;
        newVMR.Config.isTile = true;
        newVMR.Config.existCollider = true;
        newVMR.Config.typeCollider = TypeCollider.MeshCollider;
        newVMR.Config.isDestructible = true;
        newVMR.Config.useGlobalScale = true;

        if (!EqualityComparer<ColorsBorders>.Default.Equals(colorsBorders, default(ColorsBorders)) && isAddColorsToTile)
        {
            newTile3D.ColorsBack = colorsBorders.ColorsBack;
            newTile3D.ColorsBottom = colorsBorders.ColorsBottom;
            newTile3D.ColorsForward = colorsBorders.ColorsForward;
            newTile3D.ColorsTop = colorsBorders.ColorsTop;
            newTile3D.ColorsLeft = colorsBorders.ColorsLeft;
            newTile3D.ColorsRight = colorsBorders.ColorsRight;
        }


        // GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ExistingPrefab.prefab");
        // var savePath = "Assets/Prefabs/ExistingPrefab.prefab";
        if (!AssetDatabase.IsValidFolder($"{OutputPath}/WFC/{nameDirPrefabs}"))
        {
            AssetDatabase.CreateFolder($"{OutputPath}/WFC", nameDirPrefabs);
        }

        string savePath = AssetDatabase.GenerateUniqueAssetPath($"{OutputPath}/WFC/{nameDirPrefabs}/{newObjectName}.prefab");

        GameObject prefabAsset1 = PrefabUtility.SaveAsPrefabAsset(newObject, savePath);
        Tile3D prefTile3D = prefabAsset1.GetComponent<Tile3D>();
        // GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabAsset);
        // instance.transform.position = Vector3.zero;

        return prefTile3D;
    }   

    void CheckSocket(Voxel[] listColors, ref string configOption)
    {
            if (HelperVoxel.AreExistColors(listColors))
            {
                // Debug.Log($"name={vmRenderer.Config.sOVoxelData.name}");
                bool isExist = false;

                // проходим по уже существующим розеткам.
                foreach (KeyValuePair<string, Voxel[]> item in dictionarySocketsNames)
                {
                    // если есть совпадения, присваиваем уже существующее имя розетки.
                    if (HelperVoxel.AreColorEqual(item.Value, listColors))
                    {
                        configOption = item.Key;
                        isExist = true;
                        break;
                    }
                }

                // если нет совпадений в существующих розетках.
                // добавляем новую розетку.
                if (!isExist)
                {
                    nextName = nextName + 1;
                    string newName = $"{nextName}";
                    dictionarySocketsNames.Add(newName, listColors);
                    configOption = newName;
                }
            } else
            // если цветов нет в массиве, присваиваем розетке имя по умолчанию как -1;
            {
                configOption = "-1";
            }
    }
}


[System.Serializable]
public struct ColorsBorders
{
    public Voxel[] ColorsRight;
    public Voxel[] ColorsForward;
    public Voxel[] ColorsLeft;
    public Voxel[] ColorsBack;
    public Voxel[] ColorsTop;
    public Voxel[] ColorsBottom;
}