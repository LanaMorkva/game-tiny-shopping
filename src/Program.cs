using System;

namespace TinyShopping {

    public static class Program {

        [STAThread]
        static void Main() {
            using (var game = new Renderer())
                game.Run();
        }
    }
}
