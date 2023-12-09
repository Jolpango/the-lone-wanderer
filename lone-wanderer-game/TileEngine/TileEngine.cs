using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace LoneWandererGame.TileEngines
{
    public class TileEngine
    {
        private Game1 game;
        private JObject jsonMap;
        private List<Layer> layers;
        public TileEngine(Game1 game)
        {
            this.game = game;

            layers = new List<Layer>();
            
        }

        public void LoadContent()
        {
            jsonMap = JObject.Parse(File.ReadAllText("Content/Tilemaps/map1.json"));

            int tileHeight = (int)jsonMap.Root["tileheight"];
            int tileWidth = (int)jsonMap.Root["tilewidth"];

            JToken tileset = jsonMap.Root["tilesets"][0];
            string tilesheetPath = (string)tileset["image"];
            int sheetTileHeight = (int)tileset["tileheight"];
            int sheetTileWidth = (int)tileset["tilewidth"];
            int sheetImageHeight = (int)tileset["imageheight"];
            int sheetImageWidth = (int)tileset["imagewidth"];

            foreach (JToken layer in jsonMap.Root["layers"].ToList())
            {
                List<int> data = layer["data"].Select(datapoint => (int)datapoint).ToList();
                bool collidable = (bool)layer["properties"][0]["value"];
                int height = (int)layer["height"];
                int width = (int)layer["width"];
                string name = (string)layer["name"];
                float depth = (layers.Count + 1) / 10f;
                Layer newLayer = new Layer(game, data.ToList(), height, width, sheetTileHeight, sheetTileWidth, name, depth, collidable);
                newLayer.LoadContent("Tilemaps/mountain_landscape");
                for (int i = 0; i < data.Count; i++)
                {
                    if (data.ElementAt(i) == 0) continue;

                    int mapX = i % width;
                    int mapY = i / width;

                    Vector2 tilePosition = new Vector2(mapX, mapY);
                    newLayer.AddTile(tilePosition, data.ElementAt(i));
                }
                layers.Add(newLayer);
            }
        }

        public void Draw()
        {
            foreach (Layer layer in layers)
                layer.Draw();
        }
    }
}
