using Microsoft.Xna.Framework;

namespace TinyShopping.Game.AI {

    internal abstract class Task {

        public Insect Insect { get; private set; }

        public World World { get; private set; }

        public AIHandler AIHandler { get; private set; }

        public Task(Insect insect, World world, AIHandler aiHandler) {
            Insect = insect;
            World = world;
            AIHandler = aiHandler;
        }

        /// <summary>
        /// Executes the AI task.
        /// </summary>
        /// <returns>If any action was performed.</returns>
        public abstract bool Run(GameTime gameTime);
    }
}
