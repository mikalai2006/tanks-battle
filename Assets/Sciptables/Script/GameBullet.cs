using UnityEngine;

[CreateAssetMenu]
public class GameBullet : ScriptableObject
{
  [Space(5)]
  [Header("Основная информация")]
  public string idObject;
  public TextLocalize text;
  public BaseBullet prefab;

  [Space(5)]
  [Header("Параметры")]
  [Tooltip("Скорость")]
  [Range(6f, 20f)] public float speed;
  [Tooltip("Урон от одного попадания")]
  [Range(1f, 1000f)] public int damage;
  [Tooltip("След от взрыва")]
  public GameObject effectBoom;
  [Tooltip("Эффект взрыва")]
  public GameObject particleBoom;
}
