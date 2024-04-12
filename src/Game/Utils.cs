using Microsoft.Xna.Framework;
using System.Collections.Generic;
using MonoGame.Extended;

public static class Utilities {

        /// <summary>
        /// Translate isometric coordinates into screen(rectangular) space coordinates
        /// </summary>
        /// <param name="pos">Pos to translate</param>
        /// <param name="tileHeight">Height of the tile</param>
        /// <param name="tileWidth">Width of the tile.</param>
        public static Point2 worldToScreen(Point2 pos, int tileHeight, int tileWidth) {
            float tileX = pos.X / tileHeight;
            float tileY = pos.Y / tileHeight;
            var position = new Vector2((tileX - tileY) * tileWidth / 2, (tileX + tileY) * tileHeight / 2);
            return position;
        }
}