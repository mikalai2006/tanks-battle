using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class UIMenuInGame : UIBase
{
    private Button _btnToSettings;
    private Button _btnToStartMenu;
    private Button _btnBackToGame;
    private TaskCompletionSource<DataDialogResult> _processCompletionSource;
    private DataDialogResult _result;
    // private InputManager _inputManager;

    public override void Awake()
    {
        UISettings.OnChangeLocale += Refresh;

        base.Awake();
    }

    public override void OnDestroy()
    {
        if (_gameManager != null)
        {
            _gameManager.ChangeState(GameState.StopPause);
        }

        UISettings.OnChangeLocale -= Refresh;

        base.OnDestroy();
    }

    public override async void Start()
    {
        _gameManager.ChangeState(GameState.StartPause);

        base.Start();
        // _inputManager = new InputManager();
        // _inputManager.Disable();

        Title.text = await Helpers.GetLocaledString("menu");

        _btnToSettings = Wrapper.Q<Button>("BtnToSettings");
        _btnToSettings.clickable.clicked += () =>
        {
            ClickToSettings();
        };

        _btnToStartMenu = Wrapper.Q<Button>("BtnToStartMenu");
        _btnToStartMenu.clickable.clicked += () =>
        {
            ClickToStartMenu();
        };

        _btnBackToGame = Wrapper.Q<Button>("BtnBackToGame");
        _btnBackToGame.clickable.clicked += () =>
        {
            ClickBackToGame();
        };
        base.Initialize(Wrapper);
    }

    private void Refresh()
    {
        Debug.Log($"MenuinGame");
        base.Initialize(Wrapper);
    }


    public async UniTask<DataDialogResult> ProcessAction()
    {
        _result = new DataDialogResult();

        _processCompletionSource = new TaskCompletionSource<DataDialogResult>();

        return await _processCompletionSource.Task;
    }

    private void ClickToStartMenu()
    {
        AudioManager.Instance.Click();

        _result.isOk = true;
        _processCompletionSource.SetResult(_result);


        // var dashBoard = new StartUIOperation();
        // dashBoard.ShowAndHide().Forget();

        // var allEntity = _gameManager.LevelManager.EntityManager.entities.ToList();
        // foreach (var ent in allEntity)
        // {
        //     ent.Value.PreDestroyGameObject();
        //     ent.Value.DestroyGameObject();
        // }
        _gameManager.ChangeState(GameState.CloseLevel);
    }

    private void ClickBackToGame()
    {
        AudioManager.Instance.Click();

        _result.isOk = false;

        _processCompletionSource.SetResult(_result);

        // _inputManager.Enable();
    }

    private async void ClickToSettings()
    {
        AudioManager.Instance.Click();

        // _gameManager.InputManager.Disable();
        var settingsDialog = new UISettingsOperation();
        await settingsDialog.ShowAndHide();
        // _gameManager.InputManager.Enable();
        // _result.isLoad = true;
        // _result.isOk = true;

        // _processCompletionSource.SetResult(_result);
    }
}
