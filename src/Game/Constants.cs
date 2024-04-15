namespace TinyShopping.Game {

    internal class Constants {

        /// <summary>
        /// How close an enemy insect must be to deal damage. Given in tiles.
        /// </summary>
        public static readonly int FIGHT_RANGE = 1;

        /// <summary>
        /// Attributes of ants.
        /// </summary>
        public static Attributes ANT_ATTRIBUTES = new Attributes { speed = 70, rotationSpeed = 100, maxHealth = 100, damage = 2 };

        /// <summary>
        /// Attributes of termites.
        /// </summary>
        public static Attributes TERMITE_ATTRIBUTES = new Attributes { speed = 50, rotationSpeed = 70, maxHealth = 120, damage = 4 };

        /// <summary>
        /// How close a fruit must be for the insect to see it. Given in tiles.
        /// </summary>
        public static readonly int PICKUP_RANGE = 2;

        /// <summary>
        /// The amount of fruit distributed on the map.
        /// </summary>
        public static readonly int FRUITS_NUM = 30;

        /// <summary>
        /// How close an enemy insect must be for it to be visible.
        /// </summary>
        public readonly static int ENEMY_VISIBILITY_RANGE = 2;

        /// <summary>
        /// The effect radius of pheromones. Given in tiles.
        /// </summary>
        public static readonly int PHEROMONE_RANGE = 8;

        /// <summary>
        /// How long a pheromone spot should exist, in miliseconds.
        /// </summary>
        public static readonly int PHEROMONE_DURATION = 5000;

        /// <summary>
        /// The size of the players' cursors.
        /// </summary>
        public static readonly int CURSOR_SIZE = 50;

        /// <summary>
        /// The speed of the players' cursors.
        /// </summary>
        public static readonly int CURSOR_SPEED = 600;

        /// <summary>
        /// The number of seconds each game lasts.
        /// </summary>
        public static readonly int TIME_LIMIT_S = 5 * 60;
    }
}
