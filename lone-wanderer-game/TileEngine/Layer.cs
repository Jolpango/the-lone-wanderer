using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;

namespace LoneWandererGame.TileEngines
{
    public class Layer
    {
        private Game1 game;
        private List<Tile> tiles;
        private List<Texture2D> textures;
        private List<int> nonEmptyTileIndices;
        public List<int> Data { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        public int TileHeight { get; private set; }
        public int TileWidth { get; private set; }
        public string Name { get; private set; }
        public float Depth { get; private set; }
        public bool Collidable { get; private set; }

        public Layer(Game1 game, List<int> data, int height, int width, int tileHeight, int tileWidth, string name, float depth, bool collidable)
        {
            this.game = game;
            Data = data;
            Height = height;
            Width = width;
            TileHeight = tileHeight;
            TileWidth = tileWidth;
            Name = name;
            Depth = depth;
            Collidable = collidable;

            tiles = new List<Tile>();
            nonEmptyTileIndices = new List<int>();
        }

        public void LoadContent(List<Texture2D> sheets)
        {
            textures = sheets;
        }

        public void Draw(int fromX, int toX, int fromY, int toY)
        {
            for (int x = fromX; x <= toX; x++)
                for (int y = fromY; y <= toY; y++)
                    if (!TileIsEmptyAtIndex(x, y))
                        GetNonEmptyTileAtIndex(x, y).Draw();
        }

        public void AddTile(Vector2 position, int sheetIndex)
        {
            if (sheetIndex == 0) {
                nonEmptyTileIndices.Add(-1);
                return;
            }
            int tileSetTilesWide = textures[0].Width / TileWidth;
            int tileCount = tileSetTilesWide * tileSetTilesWide;
            int textureIndex = sheetIndex / tileCount;
            int tileIndex = sheetIndex % tileCount;
            
            Rectangle rect = new Rectangle(
                (tileIndex - 1) % tileSetTilesWide,
                (tileIndex - 1) / tileSetTilesWide,
                TileWidth,
                TileHeight
            );
            rect.X *= TileWidth;
            rect.Y *= TileHeight;
            position.X *= TileWidth;
            position.Y *= TileHeight;

            nonEmptyTileIndices.Add(tiles.Count);
            tiles.Add(new Tile(game, textures[textureIndex], position, rect, Depth));
        }

        public Tile GetNonEmptyTileAtIndex(int x, int y)
        {
            int tileIndex = nonEmptyTileIndices[x + y * Width];
            return tiles[tileIndex];
        }

        public bool TileIsEmptyAtIndex(int x, int y)
        {
            return nonEmptyTileIndices[x + y * Width] == -1;
        }
    }
}
