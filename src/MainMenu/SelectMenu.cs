using System;
using Microsoft.Xna.Framework;

namespace TinyShopping.MainMenu {

    class MainSelectMenu: SelectMenu {
        public MainSelectMenu(Rectangle menuRegion, Vector2 centerOffset, Vector2 itemSize, Rectangle explanationRegion, SoundPlayer soundPlayer): base(menuRegion, centerOffset, itemSize, NoAction, explanationRegion, soundPlayer) {
        }

        public static void NoAction() {}
    }
}
