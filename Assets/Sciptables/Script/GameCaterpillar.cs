using UnityEngine;

[CreateAssetMenu]
public class GameCaterpillar : ScriptableObject
{
  [Space(5)]
  [Header("Основная информация")]
  public string idObject;
  public TextLocalize text;
  public BaseCaterpillar prefab;
  [Tooltip("Спрайт дула")]
  public Sprite sprite;
  // [Tooltip("Аниматор дула")]
  // public AnimatorOverrideController animator;

  [Space(5)]
  [Header("Параметры")]
  [Tooltip("Цвет")]
  public Color color;
}
