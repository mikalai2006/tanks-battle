using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace UIToolkitLibrary
{
    // Высокоуровневый менеджер для различных частей интерфейса игровой сцены.
    public class UIManagerGameScene : MonoBehaviour
    {
        GameManager _gameManager => GameManager.Instance;
        private TaskCompletionSource<DataDialogResult> _processCompletionSource;
        // private DataDialogResult _result;
        [SerializeField] private TopSideBarController topSideBarController; 
        void OnEnable()
        {
            SetupViews();
            
            SubscribeToEvents();
      
            // Start with the home screen
            // ShowModalView(m_HomeView);

        }

        void SubscribeToEvents()
        {
            MainMenuUIEvents.GameSideBarShown += ShowSideBar;
        }

        void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        void UnsubscribeFromEvents()
        {
            MainMenuUIEvents.GameSideBarShown -= ShowSideBar;
        }
        
        void Start()
        {
            Time.timeScale = 1f;

        }

        void SetupViews()
        {
            // VisualElement root = m_MainMenuDocument.rootVisualElement;

            // root.style.backgroundImage = new StyleBackground(GameManager.Instance.Theme.bgImage);
            // var wrapper = root.Q<VisualElement>("Wrapper");
            // if (wrapper != null)
            // {
            //     wrapper.style.backgroundColor = new StyleColor(GameManager.Instance.Theme.colorBg);
            // }
        }

        // // Toggle modal screens on/off
        // void ShowModalView(UIView newView)
        // {
        //     if (m_CurrentView != null)
        //         m_CurrentView.Hide();

        //     m_PreviousView = m_CurrentView;
        //     m_CurrentView = newView;

        //     // Show the screen and notify any listeners that the main menu has updated

        //     if (m_CurrentView != null)
        //     {
        //         m_CurrentView.Show();
        //         MainMenuUIEvents.CurrentViewChanged?.Invoke(m_CurrentView.GetType().Name);
        //     }
        // }

        // // Modal screen methods. 
        // void OnHomeScreenShown()
        // {
        //     ShowModalView(m_HomeView);
        // }

        private void OnGoToStartMenu()
        {
            AudioManager.Instance.Click();

            _gameManager.ChangeState(GameState.CloseLevel);

            // var dashBoard = new StartUIOperation();
            // dashBoard.ShowAndHide().Forget();

            var uiManager = new UIManagerOperation();
            uiManager.ShowAndHide().Forget();
        }

        private void ShowSideBar()
        {
            Debug.LogWarning("Show sideBar");
        }

        public async UniTask<DataDialogResult> ProcessAction()
        {
            // _result = new DataDialogResult();

            // #if ysdk
            //         GetLeaderBoard();
            // #endif

            _processCompletionSource = new TaskCompletionSource<DataDialogResult>();

            return await _processCompletionSource.Task;
        }

    }
}