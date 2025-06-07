using UnityEngine;

[CreateAssetMenu]
public class GameMachine : ScriptableObject
{
  [Space(5)]
  [Header("Основная информация")]
  public string idObject;
  public TextLocalize text;
  public GameObject machinePrefab;
  public IndicatorMachine indicatorPrefab;

  [Space(5)]
  [Header("Цвета")]
  [Tooltip("Цвет машины")]
  public Color colorBody;
  [Tooltip("Цвет башни машины")]
  public Color colorTower;


  [Space(5)]
  [Header("Параметры")]
  [Tooltip("Скорость")]
  [Range(1.5f, 6f)] public float speed;
  // [Tooltip("Максимально возможная скорость")]
  // [Range(1.5f, 6f)] public float maxSpeed;
  [Tooltip("Скорость вращения башни")]
  [Range(0f, 10f)] public float speedRotateTower;
  // [Tooltip("Максимально возможная скорость вращения башни")]
  // [Range(0f, 10f)] public float maxSpeedRotateTower;
  [Tooltip("Дистанция обнаружения противника")]
  [Range(1f, 15f)] public int distanceSearch;
  [Tooltip("Дистанция атаки")]
  [Range(1f, 10f)] public int distanceAttack;
  [Tooltip("Здоровье")]
  [Range(100f, 1000f)] public int hp;
  [Tooltip("Броня")]
  [Range(100f, 1000f)] public int armour;
  // [Tooltip("Задержка выстрела следующего ствола, чтобы имитировать очередь, а не стрелять сразу всеми стволами")]
  // [Range(0f, 0.5f)] public float timeDelayNextMuzzle;
  // [Tooltip("Возможные дулья")]
  // public GameMuzzle Muzzle;
}
