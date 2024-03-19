using TinyShopping.Game;
using System;

namespace TinyShopping {

    public static class Program {

        [STAThread]
        static void Main() {
            using (var game = new Game())
                game.Run();
        }
    }
}
