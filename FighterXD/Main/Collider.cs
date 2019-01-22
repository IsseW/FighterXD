using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterXD.Main
{
    public class Collider : CopyableObject
    {
        protected GameObject gameObject;

        public GameObject GameObject
        {
            get
            {
                return gameObject;
            }
        }

        public Vector2 position;

        public float maxDistSquared { get; protected set; }

        private void Update ()
        {
            if (gameObject != null)
                position = gameObject.Position;
        }

        protected Vector2 GlobalToLocal(Vector2 v)
        {
            if (gameObject != null) return gameObject.GlobalToLocal(v);
            else return v - position;
        }
        protected Vector2 LocalToGlobal(Vector2 v)
        {
            if (gameObject != null) return gameObject.LocalToGlobal(v);
            else return v + position;
        }

        protected Vector2 LocalVectorToGlobal(Vector2 v)
        {
            if (gameObject != null) return gameObject.LocalVectorToGlobal(v);
            else return v;
        }

        public void Update(Vector2 position)
        {
            this.position = position;
        }

        public void Init(GameObject gameobject)
        {
            this.gameObject = gameobject;
        }

        public virtual bool Contained(Collider other)
        {
            return false;
        }

        public bool Collide(Collider other)
        {
            Update();
            other.Update(); Update();
            other.Update();
            Vector2 otherPoint = other.ClosestPoint(position, out Vector2 otherNormal);
            Vector2 myPoint = ClosestPoint(other.position, out Vector2 myNormal);
            return IsInside(otherPoint) || other.IsInside(myPoint);
        }

        public bool Collide(Collider other, out Vector2 myPoint, out Vector2 otherPoint, out Vector2 otherNormal)
        {
            Update();
            other.Update();
            otherPoint = other.ClosestPoint(position, out otherNormal);
            myPoint = ClosestPoint(other.position, out Vector2 myNormal);
            return IsInside(otherPoint) || other.IsInside(myPoint);
        }

        protected virtual Vector2 ClosestPoint(Vector2 point, out Vector2 normal)
        {
            normal = Vector2.Zero;
            return point;
        }
        

        protected virtual bool IsInside(Vector2 point)
        {
            return false;
        }

        protected virtual bool IsInside(Vector2 point, out Vector2 closestNormal)
        {
            closestNormal = Vector2.Zero;
            return false;
        }

        protected virtual bool IsLocalInside(Vector2 point)
        {
            return false;
        }

        public virtual void SetSize() { }

        public override CopyableObject CreateCopy()
        {
            return new Collider();
        }
    }


    public class CircleCollider : Collider
    {
        public override void SetSize()
        {
            radius = gameObject.spriteSize.Y / 2;
        }

        public float radius
        {
            get
            {
                return m_radius;
            }
            set
            {
                m_radius = value;
                maxDistSquared = value * value;
            }
        }

        private float m_radius;

        public CircleCollider(float radius)
        {
            this.radius = radius;
        }


        public CircleCollider() : base()
        {

        }

        protected override Vector2 ClosestPoint(Vector2 point, out Vector2 normal)
        {
            point = GlobalToLocal(point);
            normal = Vector2.Normalize(point);
            return position +  normal * radius;
        }

        protected override bool IsInside(Vector2 point)
        {
            return Vector2.Distance(position, point) <= radius;
        }

        protected override bool IsInside(Vector2 point, out Vector2 closestNormal)
        {
            Vector2 dif = point - position;
            float m = dif.Length();
            closestNormal = dif / m;
            return m <= radius;
        }

        protected override bool IsLocalInside(Vector2 point)
        {
            return maxDistSquared <= radius;
        }

        public override CopyableObject CreateCopy()
        {
            return new CircleCollider(radius);
        }
    }


    public class RectangleCollider : Collider
    {
        public override void SetSize()
        {
            topLeft = gameObject.spriteSize / -2;
            bottomRight = gameObject.spriteSize / 2;
        }
        private Vector2 topLeft
        {
            get
            {
                return m_top;
            }
            set
            {
                m_top = value;
                float f = m_top.LengthSquared();
                if (maxDistSquared < f) maxDistSquared = f;
                f = topRight.Length();
                if (maxDistSquared < f) maxDistSquared = f;
                f = bottomLeft.Length();
                if (maxDistSquared < f) maxDistSquared = f;

            }
        }
        private Vector2 bottomRight
        {
            get
            {
                return m_bot;
            }
            set
            {
                m_bot = value;
                float f = m_bot.LengthSquared();
                if (maxDistSquared < f) maxDistSquared = f;
                f = topRight.Length();
                if (maxDistSquared < f) maxDistSquared = f;
                f = bottomLeft.Length();
                if (maxDistSquared < f) maxDistSquared = f;

            }
        }

        private Vector2 topRight => new Vector2(bottomRight.X, topLeft.Y);
        private Vector2 bottomLeft => new Vector2(topLeft.X, bottomRight.Y);

        private Vector2 m_top;
        private Vector2 m_bot;


        public Rectangle LocalRect
        {
            get
            {
                return new Rectangle(topLeft.ToPoint(), (topLeft - bottomRight).ToPoint());
            }

            set
            {
                topLeft = value.Location.ToVector2();
                bottomRight = value.Location.ToVector2() + value.Size.ToVector2();
            }
        }

        
        public Rectangle GlobalRect
        {
            get
            {
                return new Rectangle(LocalToGlobal(topLeft).ToPoint(), LocalToGlobal(topLeft - bottomRight).ToPoint());
            }

            set
            {
                topLeft = GlobalToLocal(value.Location.ToVector2());
                bottomRight = GlobalToLocal(value.Location.ToVector2() + value.Size.ToVector2());
            }
        }

        public RectangleCollider(Rectangle rect, bool global)
        {
            if (global) GlobalRect = rect;
            else LocalRect = rect;
        }

        public RectangleCollider() : base()
        {

        }

        protected override Vector2 ClosestPoint(Vector2 point, out Vector2 normal)
        {
            
            //converts point into local space
            point = GlobalToLocal(point);
            if (true)
            {
                Vector2 PBR = point - bottomRight;
                Vector2 PTL = point - topLeft;

                if (PTL.LengthSquared() < PBR.LengthSquared())
                {
                    if (Vector2.DistanceSquared(point, topRight) < Vector2.DistanceSquared(point, bottomLeft))
                    {
                        normal = LocalVectorToGlobal(new Vector2(0, -1));
                        return LocalToGlobal(XMath.ClosestPointOnLine(bottomRight, bottomLeft, point));
                    }
                    else
                    {
                        normal = LocalVectorToGlobal(new Vector2(-1, 0));
                        return LocalToGlobal(XMath.ClosestPointOnLine(bottomRight, topRight, point));
                    }
                }
                else
                {
                    if (Vector2.DistanceSquared(point, topRight) < Vector2.DistanceSquared(point, bottomLeft))
                    {
                        normal = LocalVectorToGlobal(new Vector2(1, 0));
                        return LocalToGlobal(XMath.ClosestPointOnLine(bottomRight, topRight, point));
                    }
                    else
                    {
                        normal = LocalVectorToGlobal(new Vector2(0, 1));
                        return LocalToGlobal(XMath.ClosestPointOnLine(bottomRight, bottomLeft, point));
                    }
                }

            }
            else
            {
                // gets the closest point on the line between top left corner and top right corner
                Vector2 closest = XMath.ClosestPointOnLine(topLeft, new Vector2(bottomRight.X, topLeft.Y), point);
                float dis = Vector2.DistanceSquared(closest, point);
                normal = new Vector2(0, -1);


                // test if the point on the line between the top right corner and the bottom left corner is closer
                Vector2 test = XMath.ClosestPointOnLine(new Vector2(topLeft.Y, bottomRight.X), bottomRight, point);
                float testDis = Vector2.DistanceSquared(test, point);
                if (testDis < dis)
                {
                    normal = new Vector2(1, 0);
                    closest = test;
                    dis = testDis;
                }

                //test if the point on the line between the top right corner and the bottom left corner is closer.
                test = XMath.ClosestPointOnLine(topLeft, new Vector2(topLeft.X, bottomRight.Y), point);
                testDis = Vector2.DistanceSquared(test, point);
                if (testDis < dis)
                {
                    // if it is assign the normal to be correct and assign test to closest
                    normal = new Vector2(-1, 0);
                    closest = test;
                    dis = testDis;
                }

                //test if the point on the line between the bottom right corner and the bottom left is closer
                test = XMath.ClosestPointOnLine(bottomRight, new Vector2(topLeft.X, bottomRight.Y), point);
                testDis = Vector2.DistanceSquared(test, point);
                if (testDis < dis)
                {
                    normal = new Vector2(0, 1);
                    closest = test;
                    dis = testDis;
                }

                if (gameObject != null)
                    normal = LocalVectorToGlobal(normal);
                return LocalToGlobal(closest);
            }
        }

        protected override bool IsInside(Vector2 point)
        {
            point = GlobalToLocal(point);
            return IsLocalInside(point);
        }

        protected override bool IsInside(Vector2 point, out Vector2 closestNormal)
        {
            ClosestPoint(point, out closestNormal);
            point = GlobalToLocal(point);
            return IsLocalInside(point);
        }

        protected override bool IsLocalInside(Vector2 point)
        {
            return (point.X >= topLeft.X && point.Y >= topLeft.Y && point.X <= bottomRight.X && point.Y <= bottomRight.Y);
        }

        public override CopyableObject CreateCopy()
        {
            return new RectangleCollider(LocalRect, false);
        }
    }
}
