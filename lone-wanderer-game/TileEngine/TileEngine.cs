using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using Newtonsoft.Json.Linq;
using MonoGame.Extended;
using System.Linq;
using System;

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

        public struct CollisionDirection
        {
            public Rectangle rect;
            public Point direction;

            public CollisionDirection(Rectangle rect, Point direction)
            {
                this.rect = rect;
                this.direction = direction;
            }
        }

        public struct CollisionResolution
        {
            public Vector2 position;
            public Vector2 velocity;

            public CollisionResolution(Vector2 position, Vector2 velocity)
            {
                this.position = position;
                this.velocity = velocity;
            }
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

        public List<Rectangle> GetCollisions(RectangleF collisionRect, Vector2 velocity)
        {
            int CullIndex(int index, int min, int max)
            {
                if (index < min) return min;
                if (index >= max) return max - 1;
                return index;
            }

            List<Rectangle> collisions = new List<Rectangle>();

            Layer collisionLayer = layers[2]; // hardcoded for now, because I can
            RectangleF futureRect = new RectangleF(
                collisionRect.X + velocity.X,
                collisionRect.Y + velocity.Y,
                collisionRect.Width,
                collisionRect.Height
            );
            Point indexPosition = new Point(
                (int)(futureRect.X / collisionLayer.TileWidth),
                (int)(futureRect.Y / collisionLayer.TileHeight)
            );
            // Tile culling, only check tiles around the rect
            int fromX = CullIndex(indexPosition.X, 0, collisionLayer.Width);
            int fromY = CullIndex(indexPosition.Y, 0, collisionLayer.Height);
            int toX = CullIndex(indexPosition.X + 2, 0, collisionLayer.Width);
            int toY = CullIndex(indexPosition.Y + 2, 0, collisionLayer.Height);

            for (int x = fromX; x <= toX; x++)
            {
                for (int y = fromY; y <= toY; y++)
                {
                    Tile tile = collisionLayer.GetTileAtIndex(x, y);
                    if (tile.Empty) continue;

                    Rectangle tileRect = tile.GetCollisionRectangle();
                    if (futureRect.Intersects(tileRect))
                        collisions.Add(tileRect);
                }
            }

            return collisions;
        }
        public List<CollisionDirection> DetermineCollisionDirections(RectangleF rect, List<Rectangle> collisions, Vector2 velocity)
        {
            RectangleF futureRect = new RectangleF(
                rect.X + velocity.X,
                rect.Y + velocity.Y,
                rect.Width,
                rect.Height
            );

            Point direction = new Point(0, 0);
            List<CollisionDirection> collisionDirections = new List<CollisionDirection>();

            foreach (Rectangle collisionRect in collisions)
            {
                if (velocity.Y != 0)
                {
                    if (futureRect.Top < collisionRect.Top
                        && futureRect.Bottom > collisionRect.Top
                        && futureRect.Left < collisionRect.Right
                        && futureRect.Right > collisionRect.Left)
                    {
                        direction.Y = 1; // Down
                    }
                    else if (futureRect.Top > collisionRect.Top
                        && futureRect.Top < collisionRect.Bottom
                        && futureRect.Left < collisionRect.Right
                        && futureRect.Right > collisionRect.Left)
                    {
                        direction.Y = -1; // Up
                    }
                }
                if (velocity.X != 0)
                {
                    if (futureRect.Left < collisionRect.Left
                        && futureRect.Right > collisionRect.Left
                        && futureRect.Top < collisionRect.Bottom
                        && futureRect.Bottom > collisionRect.Top)
                    {
                        direction.X = 1; // Right
                    }
                    else if (futureRect.Left > collisionRect.Left
                        && futureRect.Left < collisionRect.Right
                        && futureRect.Top < collisionRect.Bottom
                        && futureRect.Bottom > collisionRect.Top)
                    {
                        direction.X = -1; // Left
                    }
                }

                if (direction.X != 0 || direction.Y != 0)
                    collisionDirections.Add(new CollisionDirection(collisionRect, direction));
                direction.X = direction.Y = 0;
            }

            return collisionDirections;
        }

        public Vector2 StickToDirection(List<CollisionDirection> collisionDirections, RectangleF rect, Vector2 position, Vector2 origin)
        {
            Vector2 finalPosition = new Vector2(position.X, position.Y);
            foreach (CollisionDirection collisionDirection in collisionDirections)
            {
                if (collisionDirection.direction.X != 0)
                {
                    finalPosition.X = collisionDirection.direction.X == 1
                        ? collisionDirection.rect.Left - origin.X  // Right
                        : collisionDirection.rect.Right + origin.X; // Left
                }
                if (collisionDirection.direction.Y != 0)
                {
                    finalPosition.Y = collisionDirection.direction.Y == 1
                        ? collisionDirection.rect.Top - origin.Y  // Down
                        : collisionDirection.rect.Bottom + origin.Y; // Up
                }
            }
            return finalPosition;
        }

        public CollisionResolution ResolveCollisions(
            List<Rectangle> collisions, RectangleF rect, Vector2 position, Vector2 origin, Vector2 velocity)
        {
            CollisionResolution resolution = new CollisionResolution(position, velocity);
            // Need to check collisions on a per-axis basis to resolve correctly
            List<CollisionDirection> collisionDirectionsX = DetermineCollisionDirections(
                rect, collisions, new Vector2(velocity.X, 0));
            List<CollisionDirection> collisionDirectionsY = DetermineCollisionDirections(
                rect, collisions, new Vector2(0, velocity.Y));

            if (collisionDirectionsX.Count == 0 && collisionDirectionsY.Count == 0)
            {
                // If no single-axis collision, check collision in both axes at the same time
                List<CollisionDirection> collisionDirections = DetermineCollisionDirections(rect, collisions, velocity);
                if (collisionDirections.Count != 0)
                {
                    resolution.position = StickToDirection(collisionDirections, rect, position, origin);
                    resolution.velocity = Vector2.Zero;
                }

                return resolution;
            }

            if (collisionDirectionsX.Count != 0)
            {
                resolution.position.X = StickToDirection(collisionDirectionsX, rect, position, origin).X;
                resolution.velocity.X = 0f;
            }
            if (collisionDirectionsY.Count != 0)
            {
                resolution.position.Y = StickToDirection(collisionDirectionsY, rect, position, origin).Y;
                resolution.velocity.Y = 0f;
            }

            return resolution;
        }
    }
}
