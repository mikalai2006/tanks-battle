using System.Diagnostics;
using Cysharp.Threading.Tasks;
using Loader;
using UnityEngine.Localization.Settings;

public class CreateMapOperation : ILoadingOperation
{
    private LevelManager levelManager;
    System.Action<float> onProgress;
    System.Action<string> onSetNotify;
    float progressValue = 0.1f;
    System.Threading.CancellationTokenSource cancelToken;

    public CreateMapOperation(LevelManager _levelManager)
    {
        levelManager = _levelManager;

        cancelToken = new System.Threading.CancellationTokenSource();

        MapManager.OnSetNotify += SetNotifyLoader;
        LevelManager.OnSetNotify += SetNotifyLoader;
        WFCGenerator.OnSetNotify += SetNotifyLoader;
        WFCGenerator.OnAddProgress += OnAddProgress;
    }

    public async UniTask Load(System.Action<float> _onProgress, System.Action<string> _onSetNotify)
    {
        onProgress = _onProgress;
        onSetNotify = _onSetNotify;

        var _gameManager = GameManager.Instance;

        await LocalizationSettings.InitializationOperation.Task;

        onProgress?.Invoke(progressValue);

        var t = await Helpers.GetLocaledString("createmap");
        onSetNotify?.Invoke(t);

        await levelManager.StartGame(cancelToken);

        // LevelManager LevelManager = GameObject.FindGameObjectWithTag("LevelManager")?.GetComponent<LevelManager>();

        // if (LevelManager != null)
        // {
        //   _gameManager.InitGameGrid(LevelManager, environment);
        //   // GameManager.Instance.LevelManager = LevelManager;
        //   // GameManager.Instance.environment = environment;
        //   // // LevelManager.CreateLevel();
        // }


        onProgress?.Invoke(1f);
    }

    private async void SetNotifyLoader(string key)
    {
        var t = await Helpers.GetLocaledString(key);
        onSetNotify?.Invoke(t);
    }

    private void OnAddProgress(float addValue)
    {
        progressValue = progressValue + addValue;
        // UnityEngine.Debug.Log($"OnAddProgress : {addValue}, {progressValue}");
        onProgress?.Invoke(progressValue);
    }

    public void Dispose()
    {
        cancelToken.Cancel();
        cancelToken.Dispose();


        MapManager.OnSetNotify -= SetNotifyLoader;
        LevelManager.OnSetNotify -= SetNotifyLoader;
        WFCGenerator.OnSetNotify -= SetNotifyLoader;
        WFCGenerator.OnAddProgress -= OnAddProgress;
    }
}
