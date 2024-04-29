namespace TinyShopping.Game {

    internal class Constants {

        /// <summary>
        /// The size of the ants.
        /// </summary>
        public static readonly int ANT_TEXTURE_SIZE = 32;

        /// <summary>
        /// The size of the fruits.
        /// </summary>
        public static readonly int FRUIT_TEXTURE_SIZE = 18;

        /// <summary>
        /// How close an enemy insect must be to deal damage.
        /// </summary>
        public static readonly int FIGHT_RANGE = 64;

        /// <summary>
        /// Attributes of ants.
        /// </summary>
        public static Attributes ANT_ATTRIBUTES = new Attributes { speed = 70, rotationSpeed = 200, maxHealth = 100, damage = 2 };

        /// <summary>
        /// Attributes of termites.
        /// </summary>
        public static Attributes TERMITE_ATTRIBUTES = new Attributes { speed = 50, rotationSpeed = 200, maxHealth = 120, damage = 4 };

        /// <summary>
        /// Speed of wandering
        /// </summary>
        public static readonly int WANDER_SPEED = 25;

         /// <summary>
        /// Insect price in fruits
        /// </summary>
        public static readonly int INSECT_PRICE = 3;

        /// <summary>
        /// How close a fruit must be for the insect to see it.
        /// </summary>
        public static readonly int FRUIT_VISIBILITY_RANGE = 64;

        /// <summary>
        /// How close a fruit must be for the insect to pick it up.
        /// </summary>
        public static readonly int PICKUP_RANGE = 24;

        /// <summary>
        /// How close a drop off location has to be for the ant to drop off the carried food.
        /// </summary>
        public static readonly int DROP_OFF_RANGE = 64;

        /// <summary>
        /// The amount of fruit distributed on the map.
        /// </summary>
        public static readonly int FRUITS_NUM = 15;

        /// <summary>
        /// The half height of the box's bottom 
        /// </summary>
        public static readonly int BOX_BOTTOM_H = 33;

        /// <summary>
        /// How close an enemy insect must be for it to be visible.
        /// </summary>
        public readonly static int ENEMY_VISIBILITY_RANGE = 96;

        /// <summary>
        /// The effect radius of pheromones.
        /// </summary>
        public static readonly int PHEROMONE_RANGE = 96;

        /// <summary>
        /// How long a pheromone spot should exist, in miliseconds.
        /// </summary>
        public static readonly int PHEROMONE_DURATION = 10000;

        /// <summary>
        /// How the length of a button press should influence the pheromone duration. The value is multiplied with the duration of the button press
        /// and added to the pheromone duration when a pheromone is placed.
        /// </summary>
        public static readonly int PHEROMONE_RANGE_COEFFICIENT = 256;

        /// <summary>
        /// The size of the players' cursors.
        /// </summary>
        public static readonly int CURSOR_SIZE = 50;

        /// <summary>
        /// The speed of the players' cursors.
        /// </summary>
        public static readonly int CURSOR_SPEED = 600;

        public static readonly float ZOOM_MIN = 1.1f;
        public static readonly float ZOOM_MAX = 2.5f;
        public static readonly float ZOOM_SPEED = 0.05f;
        /// <summary>
        /// The number of seconds each game lasts.
        /// </summary>
        public static readonly int TIME_LIMIT_S = 5 * 60;
    }
}
