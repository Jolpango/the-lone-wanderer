using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LoneWandererGame.TileEngines
{
    public class Layer
    {
        private Game1 game;
        private List<Tile> tiles;
        private Texture2D texture;
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
        }

        public void LoadContent(string sheetPath)
        {
            texture = game.Content.Load<Texture2D>(sheetPath);
        }

        public void Draw()
        {
            foreach (Tile tile in tiles)
                tile.Draw();
        }

        public void AddTile(Vector2 position, int sheetIndex)
        {
            int tileSetTilesWide = texture.Width / TileWidth;
            Rectangle rect = new Rectangle(
                (sheetIndex -1) % tileSetTilesWide,
                (sheetIndex -1) / tileSetTilesWide,
                TileWidth,
                TileHeight
            );
            rect.X *= TileWidth;
            rect.Y *= TileHeight;
            position.X *= TileWidth;
            position.Y *= TileHeight;

            tiles.Add(new Tile(game, texture, position, rect, Depth));
        }
    }
}
