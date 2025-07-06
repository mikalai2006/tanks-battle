using UnityEngine;

[CreateAssetMenu]
public class GameUpdate : ScriptableObject
{
  [Space(5)]
  [Header("Основная информация")]
  public TextLocalize text;
  public GameObject prefab;
  [Tooltip("Спрайт")]
  public Sprite sprite;

  // [Space(5)]
  // [Header("Параметры")]
  // [Tooltip("Эффект выстрела")]
  // public GameObject fireEffect;
  // [Tooltip("Снаряды")]
  // public GameBullet Bullet;
  // [Tooltip("Материал")]
  // public Material material;
  // [Tooltip("Время перезарядки (сек)")]
  // [Range(0f, 100f)] public float timeBetweenShot;
  // [Tooltip("Цвет")]
  // public Color color;
}
