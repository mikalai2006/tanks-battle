using Mikalai2006.Voxel;
using UnityEngine;

[CreateAssetMenu]
public class GameWheel : ScriptableObject
{
  [Space(5)]
  [Header("Основная информация")]
  public TextLocalize text;
  public BaseWheel prefab;
  // [Tooltip("Будет ли вращаться")]
  // public bool isRotate;

  [Space(5)]
  [Header("Настройки меша")]
  public MeshConfig MeshConfig;

  // [Space(5)]
  // [Header("Параметры")]
  // [Tooltip("Цвет")]
  // public Color color;
}
