using System;

namespace UIToolkitLibrary
{
    public static class UIEvents 
    {
        public static Action NeedCloseDialogs;

        public static Action<StateMachinePlayer> OnFocusMachineInGarage;
        public static Action<GameMachine> ClickButtonBuyInShop;
        public static Action ClickButtonRepair;
        public static Action ClickButtonRepairByAdv;
        public static Action ClickButtonTower;
        public static Action ClickButtonTowerClose;
        public static Action ClickButtonPrevTower;
        public static Action ClickButtonNextTower;
        public static Action<GameTowerOption>  FocusTower;
        public static Action ClickShopButtonNotMenuBar;
        public static Action UIShopPrevMachine;
        public static Action UIShopNextMachine;
        public static Action UIShopClickBuyMachine;
        public static Action<GameMachine> UIShopFocusMachine;
    }
}
