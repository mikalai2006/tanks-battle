using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameLevel : ScriptableObject
{
    [Space(5)]
    [Header("General")]
    public string idObject;

    [Space(5)]
    [Header("Map")]
    public TypeGround typeGround;
    public Vector2Int gridSize;
    public RuleTile tileLandscape;
    public RuleTile tileSecondLandscape;
    public RuleTile tileBorder;
    public List<RuleTile> tileObstcles;
    [Range(0.1f, 1f)] public float noiseScaleKoof = 0.2f;
    [Range(0.1f, 1f)] public float noiseMaxKoof = 0.4f;
    [Range(0f, 1f)] public float light;
    [Range(0.1f, 1f)] public float noiseScaleObstacleKoof = 0.2f;
    [Range(0.1f, 1f)] public float noiseObstacleMaxKoof = 0.4f;

    [Space(5)]
    [Header("Player")]
    public TypeLevel typeLevel;
    public int countTeam;
    public int countPlayers;

    public List<ItemProbabiliti<GameBonus>> bonuses;


}

[Serializable]
public enum TypeLevel
{
    Command = 1,
    Alone = 2,
}