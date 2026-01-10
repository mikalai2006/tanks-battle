using System;
using UIToolkitLibrary;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;


namespace UIToolkitDemo
{
    public class UserInfoView : UIView
    {
        static class ClassNames
        {
            public static string Name = "Name";
            public static string Coin = "Coin";
            public static string Cube = "Cube";
            public static string Rank = "Rank";
            public static string RankImage = "RankImage";
            public static string RankBg = "RankBg";
            public static string NameBox = "NameBox";
            public static string CubeBox = "CubeBox";
            public static string CoinBox = "CoinBox";
        }

        Label m_Name;
        Label m_Coin;
        Label m_Cube;
        Label m_Rank;
        VisualElement m_RankImage;
        VisualElement m_RankBg;
        VisualElement m_CubeBox;
        VisualElement m_CoinBox;
        VisualElement m_NameBox;

        public UserInfoView(VisualElement topElement, LocalizedStringTable localization): base(topElement, localization)
        {
            // m_ChatView = new ChatView(topElement);

            // HomeEvents.LevelInfoShown += OnShowLevelInfo;

            // Listen to locale changes
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        }

        /// <summary>
        /// Sets references to UI elements.
        /// </summary>
        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            m_Name = m_TopElement.Q<Label>(ClassNames.Name);
            m_Coin = m_TopElement.Q<Label>(ClassNames.Coin);
            m_Cube = m_TopElement.Q<Label>(ClassNames.Cube);
            m_RankImage = m_TopElement.Q<VisualElement>(ClassNames.RankImage);
            m_Rank = m_TopElement.Q<Label>(ClassNames.Rank);
            m_RankBg = m_TopElement.Q<VisualElement>(ClassNames.RankBg);
            m_NameBox = m_TopElement.Q<VisualElement>(ClassNames.NameBox);
            m_CubeBox = m_TopElement.Q<VisualElement>(ClassNames.CubeBox);
            m_CoinBox = m_TopElement.Q<VisualElement>(ClassNames.CoinBox);

            DrawUserInoBlok();
        }

        private void DrawUserInoBlok()
        {
            var userInfo = _gameManager.AppInfo.UserInfo;

            m_Name.text = userInfo.name;
            m_Coin.text = _gameManager.StateManager.statePlayer.coin.ToString();
            m_Cube.text = _gameManager.StateManager.statePlayer.coin.ToString();

            m_Rank.text = _gameManager.Settings.ranks[_gameManager.StateManager.statePlayer.rank].text.title.GetLocalizedString();
            m_NameBox.style.backgroundColor = new StyleColor(_gameManager.Theme.colorHead);
            m_RankBg.style.unityBackgroundImageTintColor = new StyleColor(_gameManager.Theme.colorHead);
            m_CubeBox.style.backgroundColor = new StyleColor(_gameManager.Theme.colorHead);
            m_CoinBox.style.backgroundColor = new StyleColor(_gameManager.Theme.colorHead);
        }

        /// <summary>
        /// Registers the play button click event to load the game scene.
        /// </summary>
        protected override void RegisterButtonCallbacks()
        {
            // m_PlayLevelButton.RegisterCallback<ClickEvent>(ClickPlayButton);
        }

        /// <summary>
        /// Unsubscribe and unregister to prevent memory leaks.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            // HomeEvents.LevelInfoShown -= OnShowLevelInfo;
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
            
            // m_PlayLevelButton.UnregisterCallback<ClickEvent>(ClickPlayButton);
        }

        /// <summary>
        /// Re-fetch and update localized strings as the locale changes.
        /// </summary>
        /// <param name="locale">The new Locale.</param>
        void OnLocaleChanged(Locale locale)
        {
            // ShowLevelInfo(m_CurrentLevelData.LevelNumberFormatted, m_CurrentLevelData.LevelSubtitle,
            //     m_CurrentLevelData.Thumbnail);
        }
    }
}