using Assets;

using Cysharp.Threading.Tasks;

public class UIMenuInGameOperation : LocalAssetLoader
{
  public async UniTask<DataDialogResult> ShowAndHide()
  {
    var window = await Load();
    var result = await window.ProcessAction();
    Unload();
    return result;
  }

  public UniTask<UIMenuInGame> Load()
  {
    return LoadInternal<UIMenuInGame>(ConstantsApp.UILabels.UI_MENUINGAME);
  }

  public void Unload()
  {
    UnloadInternal();
  }
}