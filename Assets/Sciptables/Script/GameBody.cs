using System.Collections.Generic;
using Mikalai2006.Voxel;
using UnityEngine;

[CreateAssetMenu]
public class GameBody : ScriptableObject
{
  [Space(5)]
  [Header("Основная информация")]
  public string idObject;
  public TextLocalize text;
  public BaseBody prefab;
  
  [Space(5)]
  [Header("Настройки меша")]
  public MeshConfig MeshConfig;

  [Space(5)]
  [Header("Цвета")]
  [Tooltip("Цвет машины")]
  public Color colorBody;


  // [Space(5)]
  // [Header("Допустимые улучшения")]
  // [Tooltip("Список улучшений")]
  // public GameMachine parent;
  // public List<MachineUpdateItem> updates;

  // [Tooltip("Задержка выстрела следующего ствола, чтобы имитировать очередь, а не стрелять сразу всеми стволами")]
  // [Range(0f, 0.5f)] public float timeDelayNextMuzzle;
  // [Tooltip("Возможные дулья")]
  // public GameMuzzle Muzzle;
}

