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
    /// <summary>
    /// This is a base class for a functional unit of the UI. This can make up a full-screen interface or just
    /// part of one.
    /// </summary>

    public class UIView : IDisposable
    {
        protected GameManager _gameManager => GameManager.Instance;
        LocalizedStringTable _localization;
        private List<LocalizeObj> _elementList;
        protected bool m_HideOnAwake = true;

        // UI reveals other underlaying UIs, partially see-through
        protected bool m_IsOverlay;

        protected VisualElement m_TopElement;

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
        }

        public virtual void Initialize()
        {
            if (m_HideOnAwake)
            {
                Hide();
            }
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
    }
}

