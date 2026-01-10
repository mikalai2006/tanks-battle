using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace UIToolkitLibrary
{
    public class UIShopView : UIView
    {
        static class ClassNames
        {
            public static string MachineBox = "MachineBox";
            public static string MachineBody = "Body";
            public static string MachineCaterpillar = "Caterpillars";
            public static string MachineTowers = "Towers";
            public static string MachineMuzzles = "Muzzles";
            public static string ButtonPrev = "Prev";
            public static string ButtonNext = "Next";
            public static string ButtonBuy = "Buy";

            public static string Name_Machine = "NameMachine";
            public static string Cost_Machine = "CostMachine";
        }

        ScrollView m_ScrollViewParent;

        Button m_Button_Prev;
        Button m_Button_Next;
        Button m_Button_Buy;
        Label m_Name_Machine;
        Label m_Cost_Machine;
        VisualElement m_MachineBox;
        VisualElement m_MachineBody;
        VisualElement m_MachineCaterpillar;
        VisualElement m_MachineTowers;
        VisualElement m_InventoryPanel;

        DropdownField m_InventoryRarityDropdown;
        DropdownField m_InventorySlotTypeDropdown;

        // Template asset for each gear item 
        VisualTreeAsset m_GearItemAsset;

        // Actively checked gear
        // GearItemComponent m_SelectedGear;

        public UIShopView(VisualElement topElement, LocalizedStringTable localization): base(topElement, localization)
        {


            // InventoryEvents.GearItemClicked += OnGearItemClicked;
            // InventoryEvents.InventorySetup += OnInventorySetup;
            // InventoryEvents.InventoryUpdated += OnInventoryUpdated;

            LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;
            
            // m_GearItemAsset = Resources.Load("GearItem") as VisualTreeAsset;

            ShopUIEvents.FocusMachineInShop += FocusMachineInShop;
        }

        public override void Dispose()
        {
            base.Dispose();
            // InventoryEvents.GearItemClicked -= OnGearItemClicked;
            // InventoryEvents.InventorySetup -= OnInventorySetup;
            // InventoryEvents.InventoryUpdated -= OnInventoryUpdated;
            
            LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
            
            UnregisterButtonCallbacks();

            ShopUIEvents.FocusMachineInShop -= FocusMachineInShop;
        }

        private void FocusMachineInShop(GameMachine machine)
        {
            m_Name_Machine.text = machine.text.title.GetLocalizedString();
            m_Cost_Machine.text = "1000";
        }

        void OnSelectedLocaleChanged(Locale obj)
        {
            UpdateLocalizedText();
        }
        
        
        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            
            m_Button_Next = m_TopElement.Q<Button>(ClassNames.ButtonNext);
            m_Button_Prev = m_TopElement.Q<Button>(ClassNames.ButtonPrev);
            m_Button_Buy = m_TopElement.Q<Button>(ClassNames.ButtonBuy);
            m_Name_Machine = m_TopElement.Q<Label>(ClassNames.Name_Machine);
            m_Cost_Machine = m_TopElement.Q<Label>(ClassNames.Cost_Machine);

            // // create tabs.
            // m_Tabs = Root.Q<VisualElement>("Tabs");
            // var tabMachines = new Button();
            // tabMachines.AddToClassList("tabs-button");
            // tabMachines.text = await Helpers.GetLocaledString("tab_machines");
            // m_Tabs.Add(tabMachines);

            // m_MachineBox = m_TopElement.Q(ClassNames.MachineBox);
            // m_MachineBox.style.maxWidth = 128 * 5;
            // m_MachineBody = m_TopElement.Q(ClassNames.MachineBody);
            // m_MachineCaterpillar = m_TopElement.Q(ClassNames.MachineCaterpillar);
            // m_MachineTowers = m_TopElement.Q(ClassNames.MachineTowers);

            // for (int i = 0; i < _gameManager.Settings.machines.Count; i++)
            // {
            //     var machine = _gameManager.Settings.machines[i];

            //     var mBox = new Button();
            //     mBox.RegisterCallback<ClickEvent>((ClickEvent evt) => OnClickMachineItem(evt, machine));
            //     mBox.AddToClassList("list-item");
            //     // VisualElement GarageWrapper = Root.Q<VisualElement>("GarageWrapper");
            //     // Debug.Log($"{GarageWrapper.style.width.value.value}");
            //     // mBox.style.width = GarageWrapper.style.width.value.value / 2;
            //     mBox.style.marginBottom = _gameManager.Theme.margin / 2;
            //     mBox.style.marginTop = _gameManager.Theme.margin / 2;
            //     mBox.style.marginLeft = _gameManager.Theme.margin / 2;
            //     mBox.style.marginRight = _gameManager.Theme.margin / 2;
            //     mBox.style.paddingTop = _gameManager.Theme.padding;
            //     mBox.style.paddingBottom = _gameManager.Theme.padding;
            //     mBox.style.paddingLeft = _gameManager.Theme.padding;
            //     mBox.style.paddingRight = _gameManager.Theme.padding;
            //     mBox.style.flexDirection = FlexDirection.Row;
            //     mBox.style.backgroundColor = new StyleColor(_gameManager.Theme.colorListItemBg);

            //     if (i == 2)
            //     {
            //         Color colActive = _gameManager.Theme.colorActive;
            //         mBox.style.borderLeftColor = new StyleColor(colActive);
            //         mBox.style.borderRightColor = new StyleColor(colActive);
            //         mBox.style.borderTopColor = new StyleColor(colActive);
            //         mBox.style.borderBottomColor = new StyleColor(colActive);
            //         colActive.a = 0.1f;
            //         mBox.style.backgroundColor = new StyleColor(colActive);
            //     }



            //     // VisualElement mach = DrawCurrentMachine(machine);
            //     // // mach.style.marginTop = 25;
            //     // // mach.style.marginRight = 25;
            //     // mBox.Add(mach);

            //     // add rank image.
            //     VisualElement rankImage = new VisualElement();
            //     rankImage.style.position = Position.Absolute;
            //     rankImage.style.top = 5;
            //     rankImage.style.right = 0;
            //     rankImage.style.width = 40;
            //     rankImage.style.height = 40;
            //     rankImage.style.backgroundImage = new StyleBackground(machine.minRank.sprite);
            //     mBox.Add(rankImage);

            //     // draw machine description.
            //     var mBoxInfoMachine = new VisualElement();
            //     mBoxInfoMachine.style.position = Position.Absolute;
            //     mBoxInfoMachine.style.left = 5;
            //     mBoxInfoMachine.style.bottom = 0;
            //     var mBoxTitle = new Label();
            //     mBoxTitle.AddToClassList("list-item-title");
            //     mBoxTitle.text = machine.text.title.GetLocalizedString();
            //     mBoxInfoMachine.Add(mBoxTitle);
            //     // var mBoxDescription = new Label();
            //     // mBoxDescription.AddToClassList("description");
            //     // mBoxDescription.text = machine.text.description.GetLocalizedString();
            //     // mBoxInfoMachine.Add(mBoxDescription);
            //     mBox.Add(mBoxInfoMachine);
                
            //     if (i == 3)
            //     {
            //         Color colActive = _gameManager.Theme.colorBg;
            //         colActive.a = 0.9f;
            //         var blockElement = new VisualElement();
            //         blockElement.style.position = Position.Absolute;
            //         blockElement.style.left = -3;
            //         blockElement.style.right = -3;
            //         blockElement.style.top = -3;
            //         blockElement.style.bottom = -3;
            //         blockElement.style.backgroundColor = new StyleColor(colActive);
            //         blockElement.style.borderLeftColor = new StyleColor(colActive);
            //         blockElement.style.borderRightColor = new StyleColor(colActive);
            //         blockElement.style.borderTopColor = new StyleColor(colActive);
            //         blockElement.style.borderBottomColor = new StyleColor(colActive);
            //         blockElement.style.backgroundColor = new StyleColor(colActive);
            //         mBox.Add(blockElement);
            //     }

            //     m_MachineBox.Add(mBox);

            // }

            // m_InventoryPanel = m_TopElement.Q("inventory__screen");
            // m_InventoryRarityDropdown = m_TopElement.Q<DropdownField>("inventory__rarity-dropdown");
            // m_InventorySlotTypeDropdown = m_TopElement.Q<DropdownField>("inventory__slot-type-dropdown");

            // // define row elements under the scrollview
            // m_ScrollViewParent = m_TopElement.Q<ScrollView>("inventory__scrollview");

            UpdateLocalizedText();
        }

        private void OnClickMachineItem(ClickEvent evt, GameMachine machine)
        {
            Debug.Log($"Click on the {machine.name}");
            
            GarageUIEvents.MachineItemClicked?.Invoke(machine);
        }

        private VisualElement DrawCurrentMachine(GameMachine machine)
        {
            VisualElement mBox = new VisualElement();
            mBox.style.flexGrow = 0;
            mBox.style.flexShrink = 0;
            mBox.style.justifyContent = Justify.Center;
            mBox.style.alignContent = Align.Center;
            mBox.style.alignItems = Align.Center;
            mBox.style.alignSelf = Align.Center;
            mBox.name = "Machine";

            return mBox;
        }

        void ClickBuy(ClickEvent evt)
        {
            ShopUIEvents.ClickButtonBuyInShop?.Invoke();
        }

        void ClickPrev(ClickEvent evt)
        {
            ShopUIEvents.ClickButtonPrevInShop?.Invoke();
        }

        void ClickNext(ClickEvent evt)
        {
            ShopUIEvents.ClickButtonNextInShop?.Invoke();
        }
        protected override void RegisterButtonCallbacks()
        {
            m_Button_Next.RegisterCallback<ClickEvent>(ClickNext);
            m_Button_Prev.RegisterCallback<ClickEvent>(ClickPrev);
            m_Button_Buy.RegisterCallback<ClickEvent>(ClickBuy);
            // m_InventoryBackButton.RegisterCallback<ClickEvent>(CloseWindow);

            // register callbacks when value in a dropdown field changes
            // m_InventoryRarityDropdown.RegisterValueChangedCallback(UpdateFilters);
            // m_InventorySlotTypeDropdown.RegisterValueChangedCallback(UpdateFilters);
        }

        // Optional: Unregistering the button callbacks is not strictly necessary
        // in most cases and depends on your application's lifecycle management.
        // You can choose to unregister them if needed for specific scenarios.
        protected void UnregisterButtonCallbacks()
        {
            m_Button_Next.UnregisterCallback<ClickEvent>(ClickNext);
            m_Button_Prev.UnregisterCallback<ClickEvent>(ClickPrev);
            m_Button_Buy.UnregisterCallback<ClickEvent>(ClickBuy);
            // m_InventoryBackButton.UnregisterCallback<ClickEvent>(CloseWindow);

            // register callbacks when value in a dropdown field changes
            // m_InventoryRarityDropdown.UnregisterValueChangedCallback(UpdateFilters);
            // m_InventorySlotTypeDropdown.UnregisterValueChangedCallback(UpdateFilters);
        }

        void CloseWindow(ClickEvent evt)
        {
            Hide();
        }

        public override void Hide()
        {
            base.Hide();

            // AudioManager.PlayDefaultButtonSound();

            // // set the selected Gear, notify the InventoryScreenController
            // if (m_SelectedGear != null)
            //     InventoryEvents.GearSelected?.Invoke(m_SelectedGear.GearData);

            // m_SelectedGear = null;

        }

        // event handling methods
        void OnSetup()
        {
            SetVisualElements();
            RegisterButtonCallbacks();
        }

        // Load a list of Equipment ScriptableObjects to show in the Inventory
        // void OnInventoryUpdated(List<EquipmentSO> gearToLoad)
        // {
        //     ShowGearItems(gearToLoad);
        // }

        // Add a check mark on a GearItem to show selection
        // void OnGearItemClicked(GearItemComponent gearItem)
        // {

        //     AudioManager.PlayAltButtonSound();

        //     // deselect previously selected
        //     SelectGearItem(m_SelectedGear, false);

        //     // select the new gear item
        //     SelectGearItem(gearItem, true);
        // }

        void UpdateLocalizedText()
        {
            
            
            // if (m_InventoryRarityDropdown == null || m_InventorySlotTypeDropdown == null)
            //     return;

            // // Update Rarity dropdown using an extension method
            // string[] rarityChoices = new string[]
            // {
            //     LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_Rarity_All"),
            //     LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_Rarity_Common"),
            //     LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_Rarity_Rare"),
            //     LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_Rarity_Special")
            // };
            // m_InventoryRarityDropdown.UpdateLocalizedChoices(rarityChoices, RarityKeys[m_InventoryRarityDropdown.index], RarityKeys);

            // // Update Slot Type dropdown using an extension method
            // string[] slotTypeChoices = new string[]
            // {
            //     LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_SlotType_All"),
            //     LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_SlotType_Weapon"),
            //     LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_SlotType_Shield"),
            //     LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_SlotType_Helmet"),
            //     LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_SlotType_Boots"),
            //     LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_SlotType_Gloves")
            // };
            // m_InventorySlotTypeDropdown.UpdateLocalizedChoices( slotTypeChoices, SlotTypeKeys[m_InventorySlotTypeDropdown.index], SlotTypeKeys);

        }
        
    }
}