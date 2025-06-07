using UnityEngine;

[CreateAssetMenu]
public class GameMuzzle : ScriptableObject
{
  [Space(5)]
  [Header("Основная информация")]
  public string idObject;
  public TextLocalize text;
  public GameObject prefab;

  [Space(5)]
  [Header("Параметры")]
  [Tooltip("Эффект выстрела")]
  public GameObject fireEffect;
  [Tooltip("Снаряды")]
  public GameBullet Bullet;
  [Tooltip("Материал")]
  public Material material;
  [Tooltip("Время перезарядки (сек)")]
  [Range(0f, 100f)] public float timeBetweenShot;
  [Tooltip("Цвет")]
  public Color color;
}
