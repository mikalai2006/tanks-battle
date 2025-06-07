using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class GameSetting : ScriptableObject
{
  public GameAudio Audio;
  // // [SerializeField] public AssetReferenceGameObject prefabHiddenChar;
  public GameTheme ThemeDefault;
  // public List<GamePlayerSetting> PlayerSettings;
  // public List<GameType> GamesTypes;
  // public ArcadaItemLayout rebusItemPrefab;
  // public ArcadaItemLayout rebusItemBigPrefab;
  // public GameObject rainbowPrefab;
  // public TextEffect textEffectPrefab;
  // public BottleStar bottleStarPrefab;
  // public BottleEyes bottleEyesPrefab;
  // public GameObject demandStarPrefab;
  // public GameBonus[] throphs;

  [Space(5)]
  [Header("System")]
  public List<Sprite> gerbs;
  public List<GameLevel> levels;
  public List<GameMachine> machines;
  public List<Color> commandColor;
  // public TypeGame typeGame;
  // [Range(0.1f, 1f)] public float lineWidth;
  // [Range(0.5f, 3f)] public float radius;
  // // [Range(0, 3)] public int addinitiallyRow;
  // [Range(10, 200)] public int maxCountHiddenChar;
  // [Tooltip("Количество ячеек для создания пульсирующей сущности")]
  // [Range(3, 6)] public int minCellForPulse;
  // [Tooltip("Сколько секунд ждать до подсказки, что совпадают ячейки")]
  // [Range(0, 5f)] public float delayCheckBoom;
  // [Tooltip("Задержка в секундах до изменения ячеек сетки для сущностей при драге")]
  // [Range(0, 1f)] public float delayChangeNodes;
  // [Tooltip("Задержка в милисекундах для очереди создания советов")]
  // public int debounceTimeHints;
  // [Tooltip("Задержка в милисекундах до заполнения нод сущностями после boom")]
  // [Range(0f, 1000f)] public int delayMoveNodes;
  // public int countHoursDailyGift;

  [Space(5)]
  [Header("Настройки области атаки")]
  public Color colorAreaAttackDefault;
  public Color colorAreaAttackViewed;
  public Color colorAreaAttackAttack;

  [Space(5)]
  [Header("Текст")]
  [Tooltip("Префаб всплывающего текста")]
  public TextDamage prefabTextDamage;
  [Tooltip("Цвет текста при получении урона")]
  public Color colorTextDamage;
  [Tooltip("Цвет текста при пополнении HP")]
  public Color colorTextDamagePlus;

  [Space(5)]
  [Header("Эффекты tilemap")]
  public GameObject boomEffect;

  
  [Space(5)]
  [Header("Маркеры HUD")]
  [Tooltip("Смещение маркеров по краям экрана (ед. - экранные единицы)")]
  public Vector2 offsetMarkerEdge;
  [Tooltip("Цвет фона маркера")]
  public Color colorMarkerBg;
  [Tooltip("Цвет уровня здоровья на маркере")]
  public Color colorMarkerProgress;


  [Space(5)]
  [Header("Плейгейм")]
  [Tooltip("Поворачивать башню по ходу машины, если нет врага")]
  public bool rotateTowerByBody;
  [Tooltip("Время обнаружения противника от и до, сек")]
  public Vector2 timeBeforeAddTarget;
  [Tooltip("Расстояние на котором запрещена атака, слишком близко")]
  [Range(0,3f)] public float distanceDisableAttack;
  [Tooltip("Показывать ли зоны поиска и атаки для ботов")]
  public bool drawAreaForBot;
  [Tooltip("Коэффициент смещения угла поворота башни при попадании")]
  [Range(0,10f)] public float koofChangeAngleTower;

  [Space(5)]
  [Header("Редактор")]
  [Tooltip("Рисовать вспомогательные линии, которые показывают направление выстрела")]
  public bool drawLineAttack;

  [Space(5)]
  [Header("Particle System")]
  public GameObject PopParticle;
  public ParticleSystem PopBig;
  public ParticleSystem PulseParticle;

  [Space(5)]
  [Header("Save&Load")]
  [Tooltip("Задержка в милисекундах для очереди сохранения")]
  public int debounceTimeSave;
  public string nameSaveData;
  public string nameSaveUserInfo;

  [Space(5)]
  [Header("Texts")]
  public TextLocalize noName;
  public List<string> names;

  [Space(5)]
  [Header("UI")]
  public Sprite spriteClose;
  // [Space(5)]
  // [Header("Shop")]
  // public List<ShopItem<GameEntity>> ShopItems;
  // public List<ShopItem<GameBonus>> ShopItemsBonus;

  // [Space(5)]
  // [Header("API Directory")]
  // public APIDirectory APIDirectory;


  [Space(5)]
  [Header("Ads")]
  public int adsPerTime;

  //   [Space(5)]
  //   [Header("Rate")]
  //   public int minRateForReview;
  //   public int countCoinForReview;
  [Space(5)]
  [Header("Test")]
  [Tooltip("Ограничить переход на следующие уровни если не пройдены предыдущие в пазлах")]
  public bool isDisableNextButton;
  [Tooltip("Количество видимых следующих пазлов")]
  public int countNextPuzzle;
  public TileBase tileSquare;
}

// [System.Serializable]
// public struct APIDirectory
// {
//   // public string host;
//   public string token;
//   // public string expression;
//   public string pathExpression;
// }

[System.Serializable]
public struct ShopItem<T>
{
  public T entity;
  public int count;
  public int cost;
}

[System.Serializable]
public struct ShopAdvBuyItem<T>
{
  public T typeItem;
  public int count;
}

[System.Serializable]
public struct TextLocalize
{
  public LocalizedString title;
  public LocalizedString description;
}