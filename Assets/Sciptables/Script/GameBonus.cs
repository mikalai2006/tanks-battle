using UnityEngine;

[CreateAssetMenu]
public class GameBonus : ScriptableObject
{
  [Space(5)]
  [Header("Основная информация")]
  [Tooltip("Тип бонуса")]
  public TypeBonus typeBonus;
  public TextLocalize text;
  [Tooltip("Префаб UI")]
  public BonusLayoutItem prefabUI;
  [Tooltip("Префаб для карты")]
  public BaseBonus prefabMap;
  [Tooltip("Спрайт бонуса")]
  public Sprite sprite;

  [Space(5)]
  [Header("Параметры")]
  [Tooltip("Эффект появления на карте")]
  public GameObject effectCreate;
  [Tooltip("Эффект исчезновения с карты")]
  public GameObject effectDestroy;
  [Tooltip("Время действия, сек")]
  public float time;
  [Tooltip("Значение бонуса")]
  public float value;
  [Tooltip("Цвет бонуса")]
  public Color color;
}


[System.Serializable]
public enum TypeBonus
{
  HP = 1,
  Speed = 2,
  SpeedTower = 3,
  Medicine = 4,
  DistanceAttack = 5,
  distanceSearch = 6,
}