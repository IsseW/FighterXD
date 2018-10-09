using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterXD.Main
{
    public class Collider
    {
        protected GameObject gameobject;

        public void Init(GameObject gameobject)
        {
            this.gameobject = gameobject;
        }

        public virtual bool Collide(Collider other)
        {
            return false;
        }

        public virtual Vector2 ClosestPoint(Vector2 point)
        {
            return point;
        }

        public virtual bool IsInside(Point point)
        {
            return false;
        }
    }


    public class CircleCollider : Collider
    {
        public float radius;

        public CircleCollider(float radius)
        {
            this.radius = radius;
        }
        public override Vector2 ClosestPoint(Vector2 point)
        {
            if (Vector2.Distance(gameobject.position, point)) 
        }

        public override bool IsInside(Point point)
        {
            return base.IsInside(point);
        }

        public override bool Collide(Collider other)
        {
            return base.Collide(other);
        }
    }
}
