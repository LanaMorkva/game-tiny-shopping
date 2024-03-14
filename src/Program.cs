using System;

namespace GameLab
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new GameLabGame())
                game.Run();
        }
    }
}
