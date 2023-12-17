using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using MonoGame.Extended;

namespace LoneWandererGame.SpatialHashGrids
{
    public class SpatialHashGrid
    {
        public RectangleF Bounds { get; set; }
        public Vector2 Dimensions { get; set; }
        private Dictionary<string, HashSet<int>> cells;

        public SpatialHashGrid(RectangleF bounds, Vector2 dimensions)
        {
            Bounds = bounds;
            Dimensions = dimensions;

            cells = new Dictionary<string, HashSet<int>>();
            for (int y = 0; y < dimensions.Y; y++)
                for (int x = 0; x < dimensions.X; x++)
                    cells[Key(x, y)] = new HashSet<int>();
        }

        private Point GetCellIndex(Vector2 position)
        {
            Vector2 minBound = new Vector2(Bounds.X, Bounds.Y);
            Vector2 maxBound = new Vector2(Bounds.X + Bounds.Width, Bounds.Y + Bounds.Height);
            float x = Math.Clamp((position.X - minBound.X) / (maxBound.X - minBound.X), 0f, 1f);
            float y = Math.Clamp((position.Y - minBound.Y) / (maxBound.Y - minBound.Y), 0f, 1f);

            return new Point(
                (int)Math.Floor(x * (Dimensions.X - 1)),
                (int)Math.Floor(y * (Dimensions.Y - 1))
            );
        }

        private string Key(int x, int y)
        {
            return $"{x}.{y}";
        }

        private void RemoveClient(List<Point> indices, int index)
        {
            int xn = indices[1].X;
            int yn = indices[1].Y;
            for (int x = indices[0].X; x <= xn; ++x)
                for (int y = indices[0].Y; y <= yn; ++y)
                    cells[Key(x, y)].Remove(index);
        }

        public List<Point> NewItem(RectangleF bounds, int index)
        {
            Point i1 = GetCellIndex(new Vector2(bounds.X, bounds.Y));
            Point i2 = GetCellIndex(new Vector2(bounds.X + bounds.Width, bounds.Y + bounds.Height));

            int xn = i2.X;
            int yn = i2.Y;

            for (int x = i1.X; x <= xn; ++x)
                for (int y = i1.Y; y <= yn; ++y)
                    cells[Key(x, y)].Add(index);

            return new List<Point>(){ i1, i2 };
        }

        public List<int> FindNear(RectangleF bounds)
        {
            Point i1 = GetCellIndex(new Vector2(bounds.X, bounds.Y));
            Point i2 = GetCellIndex(new Vector2(bounds.X + bounds.Width, bounds.Y + bounds.Height));
            HashSet<int> items = new HashSet<int>();

            int xn = i2.X;
            int yn = i2.Y;
            for (int x = i1.X; x <= xn; ++x)
            {
                for (int y = i1.Y; y <= yn; ++y)
                {
                    string key = Key(x, y);
                    if (cells.ContainsKey(key))
                        foreach (int item in cells[key])
                            items.Add(item);
                }
            }

            return items.ToList();
        }

        public List<Point> UpdateItem(List<Point> indices, RectangleF bounds, int index)
        {
            Point i1 = GetCellIndex(new Vector2(bounds.X, bounds.Y));
            Point i2 = GetCellIndex(new Vector2(bounds.X + bounds.Width, bounds.Y + bounds.Height));

            if (indices[0] == i1 && indices[1] == i2)
                return indices;

            RemoveClient(indices, index);
            return NewItem(bounds, index);
        }
    }
}
