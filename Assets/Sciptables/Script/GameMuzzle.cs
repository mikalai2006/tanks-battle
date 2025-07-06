using UnityEngine;

[CreateAssetMenu]
public class GameMuzzle : ScriptableObject
{
  [Space(5)]
  [Header("Основная информация")]
  public string idObject;
  public TextLocalize text;
  public BaseMuzzle prefab;
  [Tooltip("Спрайт дула")]
  public Sprite spriteMuzzle;
  // [Tooltip("Аниматор дула")]
  // public AnimatorOverrideController animator;

  [Space(5)]
  [Header("Аудио")]
  [Tooltip("Звук выстрела")]
  public AudioClip soundShot;

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
