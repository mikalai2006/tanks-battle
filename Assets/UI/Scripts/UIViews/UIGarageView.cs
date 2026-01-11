using System.Threading.Tasks;
using Mikalai2006.Voxel;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace UIToolkitLibrary
{
    public class UIGarageView : UIView
    {
        static class ClassNames
        {
            public static string MachineBox = "MachineBox";
            public static string ButtonPrev = "Prev";
            public static string ButtonNext = "Next";
            public static string ButtonSell = "Sell";
            public static string ButtonOpenColors = "OpenColors";
            public static string DialogWrapper = "DialogWrapper";
            public static string CancelColor = "CancelColor";
            public static string MachineName = "MachineName";
        }

        ScrollView m_ScrollViewParent;

        Button m_Button_Prev;
        Button m_Button_Next;
        Button m_Button_Sell;
        Button m_Button_CancelColor;
        Button m_Button_OpenColors;
        VisualElement m_DialogWrapper;
        Label m_MachineName;
        VisualElement m_MachineBox;
        VisualElement m_InventoryPanel;

        DropdownField m_InventoryRarityDropdown;
        DropdownField m_InventorySlotTypeDropdown;

        VisualTreeAsset m_GearItemAsset;
        ColorModifyItem activeColorItem;

        public UIGarageView(VisualElement topElement, LocalizedStringTable localization): base(topElement, localization)
        {
            // InventoryEvents.GearItemClicked += OnGearItemClicked;
            // InventoryEvents.InventorySetup += OnInventorySetup;
            // InventoryEvents.InventoryUpdated += OnInventoryUpdated;

            LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;
            
            // m_GearItemAsset = Resources.Load("GearItem") as VisualTreeAsset;
            
            GarageUIEvents.OnFocusMachine += ChangeInfoMachine;
        }

        void OnSelectedLocaleChanged(Locale obj)
        {
            UpdateLocalizedText();
        }

        public override void Dispose()
        {
            base.Dispose();
            // InventoryEvents.GearItemClicked -= OnGearItemClicked;
            // InventoryEvents.InventorySetup -= OnInventorySetup;
            // InventoryEvents.InventoryUpdated -= OnInventoryUpdated;
            
            LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
            
            UnregisterButtonCallbacks();

            GarageUIEvents.OnFocusMachine -= ChangeInfoMachine;
        }
        
        
        async protected override void SetVisualElements()
        {
            base.SetVisualElements();

            m_Button_Next = m_TopElement.Q<Button>(ClassNames.ButtonNext);
            m_Button_Prev = m_TopElement.Q<Button>(ClassNames.ButtonPrev);
            m_Button_Sell = m_TopElement.Q<Button>(ClassNames.ButtonSell);

            m_Button_CancelColor = m_TopElement.Q<Button>(ClassNames.CancelColor);
            m_Button_CancelColor.style.display = DisplayStyle.None;

            m_Button_OpenColors = m_TopElement.Q<Button>(ClassNames.ButtonOpenColors);
            m_DialogWrapper = m_TopElement.Q<VisualElement>(ClassNames.DialogWrapper);
            m_MachineName = m_TopElement.Q<Label>(ClassNames.MachineName);

            if (_gameManager.StateManager.statePlayer.machines.Count == 0)
            {
                m_Button_Next.style.display = DisplayStyle.None;
                m_Button_Prev.style.display = DisplayStyle.None;
                m_Button_Sell.style.display = DisplayStyle.None;
                m_Button_OpenColors.style.display = DisplayStyle.None;
                m_MachineName.style.display = DisplayStyle.None;

                VisualElement hintElement = new VisualElement();
                hintElement.style.flexDirection = FlexDirection.Row;
                hintElement.style.flexWrap = Wrap.Wrap;
                
                Label hint = new Label();
                hint.text = await Helpers.GetLocaledString("not_machine");
                hint.AddToClassList("font");
                hint.AddToClassList("text-lg");
                hintElement.Add(hint);

                var mBtn = new Button();
                mBtn.AddToClassList("button");
                mBtn.text = await Helpers.GetLocaledString("shop");
                mBtn.clickable.clicked += () =>
                {
                    MainMenuUIEvents.ShopScreenShown?.Invoke();
                };
                hintElement.Add(mBtn);

                base.ShowHint(hintElement);
            }

            // // create tabs.
            // m_Tabs = Root.Q<VisualElement>("Tabs");
            // var tabMachines = new Button();
            // tabMachines.AddToClassList("tabs-button");
            // tabMachines.text = await Helpers.GetLocaledString("tab_colors");
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

            // DrawColorsSide();

            UpdateLocalizedText();
        }

        private void ChangeInfoMachine(BaseMachine machine)
        {
            m_MachineName.text = machine.Config.text.title.GetLocalizedString();
        }

        private VisualElement  DrawColorsSide()
        {
            VisualElement elWrapper = new VisualElement();
            // m_DialogWrapper.Clear();
            m_MachineBox = new ScrollView(); // m_TopElement.Q(ClassNames.MachineBox);
            // m_MachineBox.style.width = 128 * 5;
            m_MachineBox.style.paddingLeft = 0;
            m_MachineBox.style.paddingRight = 0;
            var m_Content = m_MachineBox.Q("unity-content-container");
            m_Content.pickingMode = PickingMode.Ignore;
            m_Content.style.display = DisplayStyle.Flex;
            m_Content.style.flexDirection = FlexDirection.Row;
            m_Content.style.flexWrap = Wrap.Wrap;
            m_Content.Clear();

            for (int i = 0; i < _gameManager.Settings.colorsModify.Count; i++)
            {
                var col = _gameManager.Settings.colorsModify[i];

                var mBox = new Button();
                mBox.RegisterCallback<ClickEvent>(async (ClickEvent evt) => await OnClickColor(evt, col));
                mBox.AddToClassList("list-item-color");
                // VisualElement GarageWrapper = Root.Q<VisualElement>("GarageWrapper");
                // Debug.Log($"{GarageWrapper.style.width.value.value}");
                // mBox.style.width = GarageWrapper.style.width.value.value / 2;
                mBox.style.marginBottom = 0;
                mBox.style.marginTop = 0;
                mBox.style.marginLeft = 0;
                mBox.style.marginRight = 0;
                mBox.style.paddingTop = 0;
                mBox.style.paddingBottom = 0;
                mBox.style.paddingLeft = 0;
                mBox.style.paddingRight = 0;
                mBox.style.flexDirection = FlexDirection.Row;
                mBox.style.backgroundColor = new StyleColor(_gameManager.Theme.colorListItemBg);

                if (HelperVoxel.AreColorsApproximatelyEqual(activeColorItem.color, col.color))
                {
                    Color colActive = _gameManager.Theme.colorActive;
                    mBox.style.borderLeftColor = new StyleColor(colActive);
                    mBox.style.borderRightColor = new StyleColor(colActive);
                    mBox.style.borderTopColor = new StyleColor(colActive);
                    mBox.style.borderBottomColor = new StyleColor(colActive);
                    // colActive.a = 0.1f;
                    // mBox.style.backgroundColor = new StyleColor(colActive);
                }



                // VisualElement mach = DrawCurrentMachine(machine);
                // // mach.style.marginTop = 25;
                // // mach.style.marginRight = 25;
                // mBox.Add(mach);

                // add rank image.
                VisualElement rankImage = new VisualElement();
                rankImage.style.position = Position.Relative;
                rankImage.style.flexShrink = 1;
                rankImage.style.flexGrow = 0;
                // rankImage.style.top = 5;
                // rankImage.style.right = 0;
                rankImage.style.minHeight = 45;
                rankImage.style.minWidth = 45;
                rankImage.style.backgroundColor = new StyleColor(col.color);
                mBox.Add(rankImage);

                // // draw machine description.
                // var mBoxInfoMachine = new VisualElement();
                // mBoxInfoMachine.style.position = Position.Absolute;
                // mBoxInfoMachine.style.left = 5;
                // mBoxInfoMachine.style.bottom = 0;
                // var mBoxTitle = new Label();
                // mBoxTitle.AddToClassList("list-item-title");
                // mBoxTitle.text = col.cost.ToString(); //text.title.GetLocalizedString();
                // mBoxInfoMachine.Add(mBoxTitle);
                // // var mBoxDescription = new Label();
                // // mBoxDescription.AddToClassList("description");
                // // mBoxDescription.text = machine.text.description.GetLocalizedString();
                // // mBoxInfoMachine.Add(mBoxDescription);
                // mBox.Add(mBoxInfoMachine);
                
                
                // if (HelperVoxel.AreColorsApproximatelyEqual(activeColorItem.color, col.color))
                // {
                //     Color colActive = _gameManager.Theme.colorBg;
                //     colActive.a = 0.9f;
                //     var blockElement = new VisualElement();
                //     blockElement.style.position = Position.Absolute;
                //     blockElement.style.left = -3;
                //     blockElement.style.right = -3;
                //     blockElement.style.top = -3;
                //     blockElement.style.bottom = -3;
                //     blockElement.style.backgroundColor = new StyleColor(colActive);
                //     blockElement.style.borderLeftColor = new StyleColor(colActive);
                //     blockElement.style.borderRightColor = new StyleColor(colActive);
                //     blockElement.style.borderTopColor = new StyleColor(colActive);
                //     blockElement.style.borderBottomColor = new StyleColor(colActive);
                //     blockElement.style.backgroundColor = new StyleColor(colActive);
                //     mBox.Add(blockElement);
                // }

                m_Content.Add(mBox);
                elWrapper.Add(m_MachineBox);

            }

            return elWrapper;
        }



        void ClickPrev(ClickEvent evt)
        {
            GarageUIEvents.ClickButtonPrevMachine?.Invoke();
        }

        void ClickNext(ClickEvent evt)
        {
            GarageUIEvents.ClickButtonNextMachine?.Invoke();
        }

        void ClickSell(ClickEvent evt)
        {
            GarageUIEvents.ClickButtonSellActiveMachine?.Invoke();
        }

        async void ClickChooseColor(ClickEvent evt)
        {
            string title = await Helpers.GetLocaledString("colors_title");
            string descr = await Helpers.GetLocaledString("colors_description");
            var dialog = new DialogProvider(new DataDialog()
            {
                title = title,
                message = descr,
                showCancelButton = true,
                innerElement = DrawColorsSide(),
                width = 400,
                align = Align.FlexStart
            });
            GarageUIEvents.OpenColors?.Invoke();
            var dataResultDialog = await dialog.ShowAndHide();
            GarageUIEvents.CloseColors?.Invoke();
        }


        private async Task OnClickColor(ClickEvent evt, ColorModifyItem colorItem)
        {
            Debug.Log($"Click on the {colorItem.color}");

            activeColorItem = colorItem;
            
            GarageUIEvents.ClickByColor?.Invoke(colorItem);

            UIEvents.NeedCloseDialogs?.Invoke();

            // DrawColorsSide();
            VisualElement hintElement = new VisualElement();
            hintElement.style.flexDirection = FlexDirection.Row;
            hintElement.style.flexWrap = Wrap.Wrap;

            VisualElement hintColorElement = new VisualElement();
            hintColorElement.style.width = 50;
            hintColorElement.style.height = 50;
            hintColorElement.style.backgroundColor = new StyleColor(activeColorItem.color);
            hintElement.Add(hintColorElement);
            
            Label hint = new Label();
            hint.text = await Helpers.GetLocaledString("colors_choose_detal");
            hint.AddToClassList("font");
            hint.AddToClassList("text-lg");
            hintElement.Add(hint);

            var mBtn = new Button();
            mBtn.AddToClassList("button");
            mBtn.text = await Helpers.GetLocaledString("btn_cancel_colors");
            mBtn.RegisterCallback<ClickEvent>(OnCancelColorFill);
            hintElement.Add(mBtn);

            base.ShowHint(hintElement);

            m_Button_OpenColors.style.display = DisplayStyle.None;
            m_Button_CancelColor.style.display = DisplayStyle.Flex;
        }

        private void OnCancelColorFill(ClickEvent evt)
        {
            OnCancelColorFill();

            GarageUIEvents.FillCancel?.Invoke();
        }

        private void OnCancelColorFill()
        {
            m_Button_OpenColors.style.display = DisplayStyle.Flex;
            m_Button_CancelColor.style.display = DisplayStyle.None;

            activeColorItem = new ColorModifyItem()
            {
                color = Color.clear
            };
            base.HideHint();
        }


        // private void OnClickMachineItem(ClickEvent evt, GameMachine machine)
        // {
        //     Debug.Log($"Click on the {machine.name}");
            
        //     GarageUIEvents.MachineItemClicked?.Invoke(machine);
        // }

        // private VisualElement DrawCurrentMachine(GameMachine machine)
        // {
        //     VisualElement mBox = new VisualElement();
        //     mBox.style.flexGrow = 0;
        //     mBox.style.flexShrink = 0;
        //     mBox.style.justifyContent = Justify.Center;
        //     mBox.style.alignContent = Align.Center;
        //     mBox.style.alignItems = Align.Center;
        //     mBox.style.alignSelf = Align.Center;
        //     mBox.name = "Machine";

        //     // float pixelsPerUnit = machine.body.spriteBody.pixelsPerUnit;
        //     // var widthMachine = machine.body.spriteBody.bounds.size.x * pixelsPerUnit;
        //     // var heightMachine = machine.body.spriteBody.bounds.size.y * pixelsPerUnit;

        //     // // draw caterpillar.
        //     // for (int j = 0; j < machine.catterpillars.Count; j++)
        //     // {
        //     //     var catConfig = machine.catterpillars[j];
        //     //     VisualElement vC = new VisualElement();
        //     //     vC.name = "Caterpillar";
        //     //     vC.style.justifyContent = Justify.Center;
        //     //     vC.style.alignContent = Align.Center;
        //     //     vC.style.alignItems = Align.Center;
        //     //     vC.style.alignSelf = Align.Center;
        //     //     vC.style.flexShrink = 0;
        //     //     vC.style.flexGrow = 0;
        //     //     vC.style.justifyContent = Justify.Center;
        //     //     vC.style.alignContent = Align.Center;

        //     //     vC.style.backgroundImage = new StyleBackground(catConfig.Config.sprite);
        //     //     float pixelsPerUnit3 = catConfig.Config.sprite.pixelsPerUnit;
        //     //     vC.style.width = catConfig.Config.sprite.bounds.size.x * pixelsPerUnit3;
        //     //     vC.style.height = catConfig.Config.sprite.bounds.size.y * pixelsPerUnit3;
        //     //     vC.style.position = Position.Absolute;
        //     //     vC.style.translate = new Translate(
        //     //         new Length((catConfig.offsetCat.x * pixelsPerUnit3), LengthUnit.Pixel),
        //     //         new Length(catConfig.offsetCat.y * pixelsPerUnit3, LengthUnit.Pixel)
        //     //     );
        //     //     vC.style.unityBackgroundImageTintColor = new StyleColor(catConfig.colorCat);

        //     //     mBox.Add(vC);
        //     // }

        //     // // draw body.
        //     // VisualElement mBody = new VisualElement();
        //     // mBody.name = "Body";
        //     // mBody.style.backgroundImage = new StyleBackground(machine.body.spriteBody);
        //     // mBody.style.width = mBox.style.width = widthMachine;
        //     // mBody.style.height = mBox.style.height = heightMachine;
        //     // mBody.style.unityBackgroundImageTintColor = new StyleColor(machine.colorBody);
        //     // mBox.Add(mBody);

        //     // // draw towers.
        //     // for (int i = 0; i < machine.towers.Count; i++)
        //     // {
        //     //     var towerConfig = machine.towers[i];
        //     //     VisualElement vE = new VisualElement();
        //     //     vE.name = "Tower";
        //     //     vE.style.flexShrink = 0;
        //     //     vE.style.flexGrow = 0;
        //     //     vE.style.justifyContent = Justify.Center;
        //     //     vE.style.alignContent = Align.Center;

        //     //     vE.style.backgroundImage = new StyleBackground(towerConfig.Config.spriteTower);
        //     //     float pixelsPerUnit2 = towerConfig.Config.spriteTower.pixelsPerUnit;
        //     //     vE.style.width = towerConfig.Config.spriteTower.bounds.size.x * pixelsPerUnit2;
        //     //     vE.style.height = towerConfig.Config.spriteTower.bounds.size.y * pixelsPerUnit2;
        //     //     vE.style.position = Position.Absolute;
        //     //     // vE.style.left = 0;
        //     //     // vE.style.right = 0;
        //     //     // vE.style.top = 0;
        //     //     // vE.style.bottom = 0;
        //     //     var xTower = towerConfig.offsetTower.x * pixelsPerUnit2;
        //     //     var yTower = towerConfig.offsetTower.y * pixelsPerUnit2;
        //     //     vE.style.translate = new Translate(new Length(xTower, LengthUnit.Pixel), new Length(yTower, LengthUnit.Pixel));
        //     //     vE.style.unityBackgroundImageTintColor = new StyleColor(towerConfig.colorTower);


        //     //     for (int j = 0; j < towerConfig.muzzles.Count; j++)
        //     //     {
        //     //         var muzzleConfig = towerConfig.muzzles[j];
        //     //         VisualElement vM = new VisualElement();
        //     //         vM.name = "Muzzle";
        //     //         vM.style.flexShrink = 0;
        //     //         vM.style.flexGrow = 0;
        //     //         vM.style.justifyContent = Justify.Center;
        //     //         vM.style.alignContent = Align.Center;

        //     //         vM.style.backgroundImage = new StyleBackground(muzzleConfig.Config.spriteMuzzle);
        //     //         float pixelsPerUnit3 = muzzleConfig.Config.spriteMuzzle.pixelsPerUnit;
        //     //         vM.style.width = muzzleConfig.Config.spriteMuzzle.bounds.size.x * pixelsPerUnit3;
        //     //         vM.style.height = muzzleConfig.Config.spriteMuzzle.bounds.size.y * pixelsPerUnit3;
        //     //         vM.style.position = Position.Absolute;
        //     //         vM.style.translate = new Translate(
        //     //             new Length((muzzleConfig.offsetMuzzle.x * pixelsPerUnit3) + (muzzleConfig.Config.spriteMuzzle.bounds.size.x / 2 * pixelsPerUnit3), LengthUnit.Pixel),
        //     //             new Length(muzzleConfig.offsetMuzzle.y * pixelsPerUnit3, LengthUnit.Pixel)
        //     //         );
        //     //         vM.style.unityBackgroundImageTintColor = new StyleColor(muzzleConfig.Config.color);

        //     //         vE.Add(vM);
        //     //     }


        //     //     mBox.Add(vE);
        //     // }

        //     return mBox;
        // }

        protected override void RegisterButtonCallbacks()
        {
            m_Button_Next.RegisterCallback<ClickEvent>(ClickNext);
            m_Button_Prev.RegisterCallback<ClickEvent>(ClickPrev);
            m_Button_Sell.RegisterCallback<ClickEvent>(ClickSell);
            m_Button_CancelColor.RegisterCallback<ClickEvent>(OnCancelColorFill);
            m_Button_OpenColors.RegisterCallback<ClickEvent>(ClickChooseColor);
            // m_InventoryBackButton.RegisterCallback<ClickEvent>(CloseWindow);

            // register callbacks when value in a dropdown field changes
            // m_InventoryRarityDropdown.RegisterValueChangedCallback(UpdateFilters);
            // m_InventorySlotTypeDropdown.RegisterValueChangedCallback(UpdateFilters);

            GarageUIEvents.FillOk += OnCancelColorFill;
        }

        // Optional: Unregistering the button callbacks is not strictly necessary
        // in most cases and depends on your application's lifecycle management.
        // You can choose to unregister them if needed for specific scenarios.
        protected void UnregisterButtonCallbacks()
        {
            m_Button_Next.UnregisterCallback<ClickEvent>(ClickNext);
            m_Button_Prev.UnregisterCallback<ClickEvent>(ClickPrev);
            m_Button_Sell.UnregisterCallback<ClickEvent>(ClickSell);
            m_Button_CancelColor.UnregisterCallback<ClickEvent>(OnCancelColorFill);
            m_Button_OpenColors.UnregisterCallback<ClickEvent>(ClickChooseColor);
            // m_InventoryBackButton.UnregisterCallback<ClickEvent>(CloseWindow);

            // register callbacks when value in a dropdown field changes
            // m_InventoryRarityDropdown.UnregisterValueChangedCallback(UpdateFilters);
            // m_InventorySlotTypeDropdown.UnregisterValueChangedCallback(UpdateFilters);
            GarageUIEvents.FillOk -= OnCancelColorFill;
        }
        // // convert string to Rarity enum
        // Rarity GetRarity(string rarityString)
        // {

        //     Rarity rarity = Rarity.Common;

        //     if (!Enum.TryParse<Rarity>(rarityString, out rarity))
        //     {
        //         Debug.Log("String " + rarityString + " failed to convert");
        //     }
        //     return rarity;
        // }

        // convert string to EquipmentType enum
        // EquipmentType GetGearType(string gearTypeString)
        // {

        //     EquipmentType gearType = EquipmentType.Weapon;

        //     if (!Enum.TryParse<EquipmentType>(gearTypeString, out gearType))
        //     {
        //         Debug.LogWarning("Converted " + gearTypeString + " failed to convert");
        //     }
        //     return gearType;
        // }


        /// <summary>
        /// Updates filters based on dropdown selection. Uses array indices rather than string values
        /// to maintain correct mapping to localized display text.
        /// </summary>
        // void UpdateFilters(ChangeEvent<string> evt)
        // {
        //     string gearTypeKey = SlotTypeKeys[m_InventorySlotTypeDropdown.index];
        //     string rarityKey = RarityKeys[m_InventoryRarityDropdown.index];

        //     EquipmentType gearType = GetGearType(gearTypeKey);
        //     Rarity rarity = GetRarity(rarityKey);

        //     InventoryEvents.GearFiltered?.Invoke(rarity, gearType);
        // }

        // loop through the available slots and create a button for each gear item
        // void ShowGearItems(List<EquipmentSO> gearToShow)
        // {

        //     // Find the element under the ScrollView to store gear item buttons and clear existing inventory
        //     VisualElement contentContainer = m_ScrollViewParent.Q<VisualElement>("unity-content-container");
        //     contentContainer.Clear();

        //     for (int i = 0; i < gearToShow.Count; i++)
        //     {
        //         CreateGearItemButton(gearToShow[i], contentContainer);
        //     }
        // }

        // generate one item for the inventory and add a clickable button to select it
        // void CreateGearItemButton(EquipmentSO gearData, VisualElement container)
        // {
        //     if (container == null)
        //     {
        //         Debug.Log("InventoryScreen.CreateGearItemButton: missing parent element");
        //         return;
        //     }

        //     TemplateContainer gearUIElement = m_GearItemAsset.Instantiate();
        //     gearUIElement.AddToClassList("gear-item-spacing");

        //     GearItemComponent gearItem = new GearItemComponent(gearData);

        //     // set visual element for gearItemComponent
        //     gearItem.SetVisualElements(gearUIElement);
        //     gearItem.SetGameData(gearUIElement);
        //     gearItem.RegisterButtonCallbacks();

        //     // add to the parent element
        //     container.Add(gearUIElement);
        // }

        // select or deselect an item
        // void SelectGearItem(GearItemComponent gearItem, bool state)
        // {
        //     if (gearItem == null)
        //         return;

        //     m_SelectedGear = (state) ? gearItem : null;
        //     gearItem.CheckItem(state);
        // }

        // methods to hide and show the screen
        // public override void Show()
        // {
        //     base.Show();

        //     InventoryEvents.ScreenEnabled?.Invoke();
        //     UpdateFilters(null);

        //     // add short transition
        //     m_InventoryPanel.transform.scale = new Vector3(0.1f, 0.1f, 0.1f);
        //     m_InventoryPanel.experimental.animation.Scale(1f, 200);
        // }

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
        void OnInventorySetup()
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