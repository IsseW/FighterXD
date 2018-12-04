using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterXD.Main
{
    interface IUpdateable
    {
        void Update(float delta);
    }
    interface IDrawable
    {
        void Draw(SpriteBatch spritebatch);
        Rectangle drawRectangle { get; }
        float depth { set; get; }
    }

    class Comparerer : IComparer<IDrawable>
    {
        public int Compare(IDrawable x, IDrawable y)
        {
            if (x == null || y == null) return 0;
            if (x.depth < y.depth) return -1;
            if (y.depth < x.depth) return 1;
            return 0;
        }
    }
}
