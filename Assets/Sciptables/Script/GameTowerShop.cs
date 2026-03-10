using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameTowerShop : ScriptableObject
{
  [Space(5)]
  [Header("Основная информация")]
  public TextLocalize text;
  public List<GameTowerOption> items;

  [Space(5)]
  [Header("Настройки для магазина")]
  public int cost;


}
