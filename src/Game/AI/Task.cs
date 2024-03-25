using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyShopping.Game.AI {

    internal abstract class Task {

        public Insect Insect { get; private set; }

        public World World { get; private set; }

        public Task(Insect insect, World world) {
            Insect = insect;
            World = world;
        }

        /// <summary>
        /// Executes the AI task.
        /// </summary>
        /// <returns>If any action was performed.</returns>
        public abstract bool Run(GameTime gameTime);
    }
}
