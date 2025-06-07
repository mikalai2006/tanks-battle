using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu]
public class GameTheme : ScriptableObject
{

  [Space(5)]
  [Header("Game")]
  public Sprite bgImage;
  public Sprite timerProgressBg;
  public Sprite timerProgress;
  public Sprite timerBg;
  public Sprite bgGrid;
  public Sprite imageWarning;
  public Color colorLockLine;

  [Space(5)]
  [Header("Hints")]
  public Color colorHintCircleFrom;
  public Color colorHintCircleTo;
  public Color colorHintLineStart;
  public Color colorHintLineEnd;
  public Color colorHintStarBtn;
  public Color colorHintStarBtnSecond;

  [Space(5)]
  [Header("Entity")]
  public Color colorEntityBliss;
  public Color colorEntitySymbol;
  public Color entityColor;

  [Space(5)]
  [Header("Game")]
  public Color colorActive;
  public Color colorCompleted;
  public Color bgColor;
  public Color colorBgGrid;
  // public Color colorBgControl;
  public Color colorPrimary;
  public Color colorSecondary;
  public Color colorAccent;

  [Space(5)]
  [Header("UI")]
  public Color colorBgInput;
  public Color colorTextInput;
  // public Color colorBgTopSide;
  public Color colorBgButton;
  public Color colorBgDialog;
}
