using UnityEngine;

[CreateAssetMenu]
public class GameBullet : ScriptableObject
{
  [Space(5)]
  [Header("Основная информация")]
  public string idObject;
  public TextLocalize text;
  public GameObject prefab;

  [Space(5)]
  [Header("Параметры")]
  [Tooltip("Время жизни")]
  [Range(0.1f, 5f)] public float lifeTime;
  [Tooltip("Скорость")]
  [Range(1f, 20f)] public float speed;
  [Tooltip("Радиус поражения вокселей (Влияет на количество вокселей, которые будут разрушены)")]
  [Range(1f, 20f)] public int damageRadius;
  [Tooltip("Максимальное количество коллизий")]
  [Range(1f, 5f)] public int countCollisions;

  
  [Space(5)]
  [Header("Эффекты")]
  [Tooltip("След от взрыва")]
  public GameObject effectBoom;
  [Tooltip("Эффект взрыва")]
  public GameObject particleBoom;
}
