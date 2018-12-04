using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterXD.Main
{
    class Terrain : Object
    {
        int width;
        int height;
        TerrainObject[,] t;

        public Terrain(Vector2 start, Vector2 end, float blockSize, Texture2D texture) : base(start)
        {
            Vector2 blocks = (end - start) / blockSize;
            width = (int)Math.Ceiling(blocks.X);
            height = (int)Math.Ceiling(blocks.Y);

            Random random = new Random();
            t = new TerrainObject[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2 pos = start + new Vector2(x, y) * blockSize;

                    TerrainObject p = new TerrainObject(new RectangleCollider(new Rectangle(1, 1, 1, 1), false), texture, pos, new Vector2(blockSize), this, x, y) { color = new Color(random.Next(80, 220), random.Next(182, 210), random.Next(80, 100), 255), depth = -2 };
                    p.Collider.SetSize();
                    children.Add(p);
                    t[x, y] = p;
                }
            }
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x > 0)
                    {
                        t[x - 1, y].neighbors++;
                        if (t[x - 1, y].neighbors >= 4) t[x - 1, y].enabled = false;
                    }
                    if (x < width - 1)
                    {
                        t[x + 1, y].neighbors++;
                        if (t[x + 1, y].neighbors >= 4) t[x + 1, y].enabled = false;
                    }
                    if (y > 0)
                    {
                        t[x, y - 1].neighbors++;
                        if (t[x, y - 1].neighbors >= 4) t[x, y - 1].enabled = false;
                    }
                    if (y < height - 1)
                    {
                        t[x, y + 1].neighbors++;
                        if (t[x, y + 1].neighbors >= 4) t[x, y + 1].enabled = false;
                    }

                }
            }
        }

        public void OnDestroyTile(int x, int y)
        {
            if (x > 0 && t[x - 1, y] != null) t[x - 1, y].enabled = true;
            if (x < width - 1 && t[x + 1, y] != null) t[x + 1, y].enabled = true;
            if (y > 0 && t[x, y - 1] != null) t[x, y - 1].enabled = true;
            if (y < height - 1 && t[x, y + 1] != null) t[x, y + 1].enabled = true;
            t[x, y] = null;
        }
    }

    class TerrainObject : ExplodableObject
    {
        public int neighbors;
        Terrain terrain;
        int x, y;
        public TerrainObject(Collider collider, Texture2D tex, Vector2 position, Vector2 spriteSize, Terrain terrain, int x, int y) : base(collider, tex, position, spriteSize)
        {
            this.terrain = terrain;
            this.x = x;
            this.y = y;
        }

        public override void OnDestroy()
        {
            terrain.OnDestroyTile(x, y);
        }
    }
}
