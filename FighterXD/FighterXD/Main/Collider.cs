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
        protected GameObject gameObject;

        public void Init(GameObject gameobject)
        {
            this.gameObject = gameobject;
        }

        public bool Collide(Collider other)
        {
            return IsInside(other.ClosestPoint(gameObject.GlobalPosition));
        }

        public bool Collide(Collider other, out Vector2 globalPoint)
        {
            globalPoint = other.ClosestPoint(gameObject.GlobalPosition);
            return IsInside(globalPoint);
        }

        public virtual Vector2 ClosestPoint(Vector2 point)
        {
            return point;
        }

        public virtual bool IsInside(Vector2 point)
        {
            return false;
        }

        public virtual bool IsLocalInside(Vector2 point)
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
            point = gameObject.GlobalToLocal(point);
            if (IsLocalInside(point)) return point;
            return gameObject.GlobalPosition + Vector2.Normalize(point - gameObject.position) * radius;
        }

        public override bool IsInside(Vector2 point)
        {
            return Vector2.Distance(gameObject.GlobalPosition, point) <= radius;
        }

        public override bool IsLocalInside(Vector2 point)
        {
            return Vector2.Distance(gameObject.position, point) <= radius;
        }
    }


    public class RectangleCollider : Collider
    {

        private Vector2 top;
        private Vector2 bottom;

        public Rectangle LocalRect
        {
            get
            {
                return new Rectangle(top.ToPoint(), (top - bottom).ToPoint());
            }

            set
            {
                top = value.Location.ToVector2();
                bottom = value.Location.ToVector2() + value.Size.ToVector2();
            }
        }

        public Rectangle GlobalRect
        {
            get
            {
                return new Rectangle(gameObject.LocalToGlobal(top).ToPoint(), gameObject.LocalToGlobal(top - bottom).ToPoint());
            }

            set
            {
                top = gameObject.GlobalToLocal(value.Location.ToVector2());
                bottom = gameObject.GlobalToLocal(value.Location.ToVector2() + value.Size.ToVector2());
            }
        }

        public RectangleCollider(Rectangle rect, bool global)
        {
            if (global) GlobalRect = rect;
            else LocalRect = rect;
        }

        public override Vector2 ClosestPoint(Vector2 point)
        {
            point = gameObject.GlobalToLocal(point);

            if (IsLocalInside(point)) return gameObject.LocalToGlobal(point);

            Vector2 closest = XMath.ClosestPointOnLine(top, new Vector2(bottom.X, top.Y), point);
            float dis = Vector2.Distance(closest, point);

            Vector2 test = XMath.ClosestPointOnLine(top, new Vector2(top.X, bottom.Y), point);
            float testDis = Vector2.Distance(test, point);
            if (testDis < dis)
            {
                closest = test;
                dis = testDis;
            }

            test = XMath.ClosestPointOnLine(bottom, new Vector2(top.X, bottom.Y), point);
            testDis = Vector2.Distance(test, point);
            if (testDis < dis)
            {
                closest = test;
                dis = testDis;
            }

            test = XMath.ClosestPointOnLine(bottom, new Vector2(top.Y, bottom.X), point);
            testDis = Vector2.Distance(test, point);
            if (testDis < dis)
            {
                closest = test;
                dis = testDis;
            }

            return gameObject.LocalToGlobal(closest);
        }

        public override bool IsInside(Vector2 point)
        {
            point = gameObject.GlobalToLocal(point);
            return IsLocalInside(point);
        }

        public override bool IsLocalInside(Vector2 point)
        {
            return (point.X > top.X && point.Y > top.Y && point.X < bottom.X && point.Y < bottom.Y);
        }
    }
}
