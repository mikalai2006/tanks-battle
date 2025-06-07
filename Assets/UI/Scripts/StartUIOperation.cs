using Assets;

using Cysharp.Threading.Tasks;

public class StartUIOperation : LocalAssetLoader
{

  public async UniTask<DataDialogResult> ShowAndHide()
  {
    var window = await Load();
    var result = await window.ProcessAction();
    Unload();
    return result;
  }

  public UniTask<StartUI> Load()
  {
    return LoadInternal<StartUI>(ConstantsApp.UILabels.UI_STARTMENU);
  }

  public void Unload()
  {
    UnloadInternal();
  }
}