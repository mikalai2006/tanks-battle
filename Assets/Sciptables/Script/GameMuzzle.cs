using Mikalai2006.Voxel;
using UnityEngine;

[CreateAssetMenu]
public class GameMuzzle : ScriptableObject
{
  [Space(5)]
  [Header("Основная информация")]
  public string idObject;
  public TextLocalize text;
  public BaseMuzzle prefab;
  
  [Space(5)]
  [Header("Настройки меша")]
  public MeshConfig MeshConfig;

  [Space(5)]
  [Header("Аудио")]
  [Tooltip("Звук выстрела")]
  public AudioClip soundShot;


  [Space(5)]
  [Header("Эффекты")]
  [Tooltip("Эффект выстрела")]
  public GameObject fireEffect;

  [Space(5)]
  [Header("Параметры")]
  [Tooltip("Снаряды")]
  public GameBullet Bullet;
  [Tooltip("Материал")]
  public Material material;
  [Tooltip("Дистанция стрельбы")]
  [Range(0f, 100f)] public float distanceAttack;
  [Tooltip("Время перезарядки (сек)")]
  [Range(0f, 100f)] public float timeBetweenShot;
  [Tooltip("Скорость снарядов")]
  [Range(0f, 100f)] public float speedBullet;
  [Tooltip("Цвет")]
  public Color color;
}
