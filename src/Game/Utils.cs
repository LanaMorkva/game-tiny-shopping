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
        public static Point2 worldPosToScreen(Point2 pos, int tileHeight, int tileWidth) {
            float tileX = pos.X / tileHeight;
            float tileY = pos.Y / tileHeight;
            return new Vector2((tileX - tileY) * tileWidth / 2, (tileX + tileY) * tileHeight / 2);
        }

        public static Point2 worldTileToScreen(Point2 tileIdx, int tileHeight, int tileWidth) {
            return new Vector2((tileIdx.X - tileIdx.Y) * tileWidth / 2, (tileIdx.X + tileIdx.Y) * tileHeight / 2);
        }
}