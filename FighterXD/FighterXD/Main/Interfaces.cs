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
    }
}
