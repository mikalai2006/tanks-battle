using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameLevel : ScriptableObject
{
    [Space(5)]
    [Header("Настройки карты")]
    public TextureHeightMapperSettings tileSettings;
    public SaveTiled saveTiled;
    public Vector3Int gridSize;
    public List<Tile3D> TilePrefabs;
    public List<Tile3D> TilePrefabsEmpty;
    public List<Tile3D> TilePrefabsInner;

    [Space(5)]
    [Header("Map")]
    // public TypeGround typeGround;
    public RuleTile tileRuleCave;
    public RuleTile tileLandscape;
    // public RuleTile tileSecondLandscape;
    // public RuleTile tileBorder;
    // public List<RuleTile> tileObstcles;
    // [Range(0.1f, 1f)] public float noiseScaleObstacleKoof = 0.2f;
    // [Range(0.1f, 1f)] public float noiseObstacleMaxKoof = 0.4f;
    [Range(0.1f, 1f)] public float noiseScaleKoof = 0.2f;
    [Range(0.1f, 1f)] public float noiseMaxKoof = 0.4f;
    [Range(0f, 1f)] public float light;

    // [Space(5)]
    // [Header("Prefabs GameObjects")]
    // public VoxelMeshRender[] testObjects;
    // public Cell3D prefabPlaceholder;

    [Space(5)]
    [Header("Player")]
    public TypeLevel typeLevel;
    public int countTeam;
    public int countPlayers;

    // public List<ItemProbabiliti<GameBonus>> bonuses;


}

[Serializable]
public struct SaveTiled
{
    public string nameMap;
    public Cell3DData[] gridComponents;
}

[Serializable]
public enum TypeLevel
{
    Command = 1,
    Alone = 2,
}