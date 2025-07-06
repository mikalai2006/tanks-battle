using Assets;

using Cysharp.Threading.Tasks;

public class TreeMachineOperation : LocalAssetLoader
{
  // public UISettingsOperation()
  // {
  // }

  public async UniTask<DataDialogResult> ShowAndHide()
  {
    var window = await Load();
    var result = await window.ProcessAction();
    Unload();
    return result;
  }

  public UniTask<TreeMachine> Load()
  {
    return LoadInternal<TreeMachine>(ConstantsApp.UILabels.UI_TREE_MACHINE);
  }

  public void Unload()
  {
    UnloadInternal();
  }
}