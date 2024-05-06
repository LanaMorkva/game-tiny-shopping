using System;
using Microsoft.Xna.Framework;

namespace TinyShopping.MainMenu {

    class MainSelectMenu: SelectMenu {
        public MainSelectMenu(Rectangle menuRegion, Vector2 centerOffset, Vector2 itemSize, Rectangle explanationRegion): base(menuRegion, centerOffset, itemSize, NoAction, explanationRegion) {
        }

        public static void NoAction() {}
    }
}
