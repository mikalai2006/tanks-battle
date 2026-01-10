using UnityEngine.UIElements;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Localization.Settings;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Localization.Tables;
using UnityEngine;
using UnityEngine.Localization;

namespace UIToolkitLibrary
{
    public class UIView : IDisposable
    {
        protected GameManager _gameManager => GameManager.Instance;
        LocalizedStringTable _localization;
        private List<LocalizeObj> _elementList;
        protected bool m_HideOnAwake = true;

        // UI reveals other underlaying UIs, partially see-through
        protected bool m_IsOverlay;

        protected VisualElement m_TopElement;
        VisualElement m_Hint;
        static class StyleClasses
        {
            public static string Hint = "Hint";
        }

        // Properties
        public VisualElement Root => m_TopElement;
        public bool IsTransparent => m_IsOverlay;
        public bool IsHidden => m_TopElement.style.display == DisplayStyle.None;

        // Constructor
        /// <summary>
        /// Initializes a new instance of the UIView class.
        /// </summary>
        /// <param name="topElement">The topmost VisualElement in the UXML hierarchy.</param>
        public UIView(VisualElement topElement, LocalizedStringTable localization)
        {
            m_TopElement = topElement ?? throw new ArgumentNullException(nameof(topElement));

            _localization = localization;

            Initialize();

            Theming(m_TopElement);

            Localize(m_TopElement).Forget();
        }

        public virtual void Initialize()
        {
            if (m_HideOnAwake)
            {
                Hide();
            }

            m_Hint = m_TopElement.Q<VisualElement>(StyleClasses.Hint);

            SetVisualElements();
            RegisterButtonCallbacks();
        }

        // Sets up the VisualElements for the UI. Override to customize.
        protected virtual void SetVisualElements()
        {

        }

        // Registers callbacks for buttons in the UI. Override to customize.
        protected virtual void RegisterButtonCallbacks()
        {

        }

        // Displays the UI.
        public virtual void Show()
        {
            m_TopElement.style.display = DisplayStyle.Flex;
        }

        // Hides the UI.
        public virtual void Hide()
        {
            m_TopElement.style.display = DisplayStyle.None;
        }

        // Unregisters any callbacks or event handlers. Override to customize.
        public virtual void Dispose()
        {

        }
        
        protected void ShowHint(VisualElement hint)
        {
            m_Hint.Clear();
            m_Hint.style.display = DisplayStyle.Flex;
            
            m_Hint.AddToClassList("hint_wrapper");
            m_Hint.style.backgroundColor = new StyleColor(_gameManager.Theme.colorBgHint);

            m_Hint.Add(hint);
        }

        protected void HideHint()
        {
            m_Hint.style.display = DisplayStyle.None;
            m_Hint.Clear();
        }

        public async UniTask Localize(VisualElement root)
        {
            await LocalizationSettings.InitializationOperation.Task;

            var op = _localization.GetTableAsync();
            await op.Task;
            _elementList = HelperUI.FindAllTextElements(root);
            OnTableLoaded(op);
        }

        private void OnTableLoaded(AsyncOperationHandle<StringTable> op)
        {
            StringTable table = op.Result;

            foreach (var item in _elementList)
            {
            var entry = op.Result[item.Key];
            if (entry != null)
                item.Element.text = entry.LocalizedValue;
            else
                Debug.LogWarning($"No {op.Result.LocaleIdentifier.Code} translation for key: '{item.Key}'");
            }
        }

        public void Theming(VisualElement _root)
        {
            var _box = _root;

            UQueryBuilder<VisualElement> builder = new UQueryBuilder<VisualElement>(_box);
            List<VisualElement> list = builder.Class("text-primary").ToList();
            foreach (var item in list)
            {
            item.style.color = _gameManager.Theme.colorPrimary;
            }

            UQueryBuilder<VisualElement> builderSecondary = new UQueryBuilder<VisualElement>(_box);
            List<VisualElement> listSecondary = builderSecondary.Where(t => t.ClassListContains("text-secondary")).ToList();
            foreach (var item in listSecondary)
            {
            item.style.color = _gameManager.Theme.colorSecondary;
            }

            UQueryBuilder<VisualElement> builderDrag = new UQueryBuilder<VisualElement>(_box);
            List<VisualElement> listDragEl = builderDrag.Class("unity-base-slider__dragger").ToList();
            foreach (var item in listDragEl)
            {
            item.style.backgroundColor = _gameManager.Theme.colorSecondary;
            }

            UQueryBuilder<VisualElement> builderInput = new UQueryBuilder<VisualElement>(_box);
            List<VisualElement> listInputEl = builderInput.Class("unity-base-text-field__input").ToList();
            foreach (var item in listInputEl)
            {
            item.style.backgroundColor = _gameManager.Theme.colorBgInput;
            item.style.color = _gameManager.Theme.colorTextInput;
            }

            UQueryBuilder<VisualElement> builderPopup = new UQueryBuilder<VisualElement>(_box);
            List<VisualElement> listPopupEl = builderPopup.Class("unity-base-popup-field__input").ToList();
            foreach (var item in listPopupEl)
            {
            item.style.backgroundColor = _gameManager.Theme.colorBgInput;
            item.style.color = _gameManager.Theme.colorTextInput;
            }
            UQueryBuilder<VisualElement> builderArrow = new UQueryBuilder<VisualElement>(_box);
            List<VisualElement> listArrowEl = builderArrow.Class("unity-base-popup-field__arrow").ToList();
            foreach (var item in listArrowEl)
            {
            item.style.unityBackgroundImageTintColor = _gameManager.Theme.colorTextInput;
            }

            UQueryBuilder<VisualElement> builderCheck = new UQueryBuilder<VisualElement>(_box);
            List<VisualElement> listCheckEl = builderCheck.Class("unity-toggle__checkmark").ToList();
            foreach (var item in listCheckEl)
            {
            item.style.backgroundColor = _gameManager.Theme.colorBgInput;
            item.style.unityBackgroundImageTintColor = _gameManager.Theme.colorTextInput;
            item.style.color = _gameManager.Theme.colorTextInput;
            }

            UQueryBuilder<VisualElement> builderBtn = new UQueryBuilder<VisualElement>(_box);
            List<VisualElement> listBtnEl = builderBtn.Class("button").ToList();
            foreach (var item in listBtnEl)
            {
            item.style.backgroundColor = _gameManager.Theme.colorBgButton;
            }

            
            UQueryBuilder<VisualElement> builderBtnSuccess = new UQueryBuilder<VisualElement>(_box);
            List<VisualElement> listBtnSuccessEl = builderBtnSuccess.Class("button_success").ToList();
            foreach (var item in listBtnSuccessEl)
            {
                item.style.backgroundColor = _gameManager.Theme.colorCompleted;
            }

            UQueryBuilder<VisualElement> builderBtnAccent = new UQueryBuilder<VisualElement>(_box);
            List<VisualElement> listBtnAccentEl = builderBtnAccent.Class("button_accent").ToList();
            foreach (var item in listBtnAccentEl)
            {
                item.style.backgroundColor = _gameManager.Theme.colorAccent;
            }

            UQueryBuilder<VisualElement> builderLowBtns = new UQueryBuilder<VisualElement>(_box);
            List<VisualElement> listLowBtns = builderLowBtns.Class("unity-scroller__low-button").ToList();
            foreach (var item in listLowBtns)
            {
            item.style.unityBackgroundImageTintColor = _gameManager.Theme.colorTextInput;
            }

            UQueryBuilder<VisualElement> builderHighBtns = new UQueryBuilder<VisualElement>(_box);
            List<VisualElement> listHighBtns = builderHighBtns.Class("unity-scroller__high-button").ToList();
            foreach (var item in listHighBtns)
            {
            item.style.unityBackgroundImageTintColor = _gameManager.Theme.colorTextInput;
            }
        }
    }
}

