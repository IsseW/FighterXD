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

        public Terrain(Vector2 start, Vector2 end, float blockSize, Texture2D texture) : base(start)
        {
            Vector2 blocks = (start - end) / blockSize;
            width = (int)Math.Ceiling(blocks.X);
            height = (int)Math.Ceiling(blocks.Y);

            Random random = new Random();
            int[,] i = new int[width, height];
            for (int y = 0; y <= height; y++)
            {
                for (int x = 0; x <= width; x++)
                {
                    Vector2 pos = start + new Vector2(x, y) * blockSize;

                    ExplodableObject p = new ExplodableObject(new RectangleCollider(new Rectangle(1, 1, 1, 1), false), texture, pos, new Vector2(blockSize)) { color = new Color(random.Next(80, 220), random.Next(182, 210), random.Next(80, 100), 255) };
                    p.Collider.SetSize();
                    children.Add(p);
                    i[x, y + 1]++;
                    if (i[x + 1, y] >= 4 && children.Count >= y * width + x + 1) children[y * width + x + 1].enabled = false;
                }
            }
        }
    }
}
