using System;
using Microsoft.Xna.Framework;

namespace TinyShopping.MainMenu {

    class MainSelectMenu: SelectMenu {
        public MainSelectMenu(Rectangle menuRegion, Vector2 itemSize): base(menuRegion, itemSize, NoAction) {
        }

        public static void NoAction() {}
    }
}
