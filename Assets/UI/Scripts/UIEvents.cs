using System;

namespace UIToolkitLibrary
{
    public static class UIEvents 
    {
        public static Action NeedCloseDialogs;

        public static Action<GameMachine> ClickButtonBuyInShop;
        public static Action ClickButtonTower;
        public static Action ClickButtonTowerClose;
        public static Action ClickButtonPrevTower;
        public static Action ClickButtonNextTower;
        public static Action<GameTowerOption>  FocusTower;
    }
}
