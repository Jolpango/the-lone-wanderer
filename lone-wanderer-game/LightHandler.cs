
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace LoneWandererGame
{
    public struct Light
    {
        public Vector2 position;
        public int size;
        public Color color;
        public float intensity; // 0 -> 1
        public bool used;

        public Light()
        {
            this.position = Vector2.Zero;
            this.size = 0;
            this.color = Color.White;
            this.intensity = 1f;
            used = false;
        }
        public Light(Vector2 position, int size, Color color, float intensity)
        {
            this.position = position;
            this.size = size;
            this.color = color;
            this.intensity = intensity;
            used = true;
        }
    }
    public class LightHandler
    {
        private Light[] lights;
        private Queue<int> freeList = new Queue<int>();

        public const int MAX_LIGHTS = 100;

        public Texture2D _blankTexture;

        public LightHandler(Game1 game)
        {
            _blankTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            _blankTexture.SetData(new[] { Color.White });

            lights = new Light[MAX_LIGHTS];
            for (int i = 0; i < MAX_LIGHTS; i++)
            {
                lights[i] = new Light();
                freeList.Enqueue(i);
            }
        }

        public bool hasEmptyLightSlots()
        {
            if (freeList.Count == 0)
                Debug.Write("Lights Buffer reached capacity (" + MAX_LIGHTS.ToString() + ")");
            return freeList.Count != 0;
        }

        public int AddLight(Vector2 position, int size, Color color, float intensity)
        {
            // If you hit this, YOU GOT TO MANY LIGHTS!
            Debug.Assert(freeList.Count != 0);
            
            int index = freeList.Dequeue();
            Light light = new Light(position, size, color, intensity);
            lights[index] = light;
            return index;
        }
        public void RemoveLight(int index)
        {
            lights[index].used = false;
            freeList.Enqueue(index);
        }

        public void updatePosition(int index, Vector2 position)
        {
            Debug.Assert(lights[index].used); // Checking if light is actually used
            lights[index].position = position;
        }

        public void updateSize(int index, int size)
        {
            Debug.Assert(lights[index].used); // Checking if light is actually used
            lights[index].size = size;
        }
        public void updateColor(int index, Color color)
        {
            Debug.Assert(lights[index].used); // Checking if light is actually used
            lights[index].color = color;
        }
        public void updateIntensity(int index, float intensity)
        {
            Debug.Assert(lights[index].used); // Checking if light is actually used
            lights[index].intensity = intensity;
        }

        public List<Light> getLights()
        {
            List<Light> lightList = new List<Light>(MAX_LIGHTS);
            for (int i = 0; i < MAX_LIGHTS; i++)
            {
                if (lights[i].used)
                {
                    lightList.Add(lights[i]);
                }
            }
            return lightList;
        }
        public void clearLights()
        {
            for (int i = 0; i < MAX_LIGHTS; i++)
            {
                if (lights[i].used)
                {
                    freeList.Enqueue(i);
                    lights[i].used = false;
                }
            }
        }
    }
}
