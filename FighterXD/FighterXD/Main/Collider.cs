using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterXD.Main
{
    class Collider
    {
        public virtual bool Collide(Collider other)
        {
            return false;
        }

        public virtual bool ClosestPoint(Vector2 point)
        {
            return false;
        }
    }
}
