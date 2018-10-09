using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterXD.Main
{
    public class GameObject
    {
        public GameObject parent;

        public SpriteEffects effects;

        public Texture2D sprite;

        public float imageScale;

        public Vector2 localOrgin;

        public Vector2 position;

        private float m_rotation;

        public Color color;

        public float rotation
        {
            set
            {
                while (value >= 360) value -= 360;
                while (value < 0) value += 360;
            }

            get
            {
                return rotation;
            }
        }

        public GameObject()
        {
            color = Color.White;
        }

        public GameObject(Texture2D sprite) : this()
        {
            this.sprite = sprite;
        }

        public GameObject(Texture2D sprite, Vector2 position) : this(sprite)
        {
            this.position = position;
        }

        public GameObject(Texture2D sprite, Vector2 position, float imageScale) : this(sprite, position)
        {
            this.imageScale = imageScale;
        }

        public GameObject(Texture2D sprite, Vector2 position, float imageScale, float rotation) : this(sprite, position, imageScale)
        {
            this.rotation = rotation;
        }

        public GameObject(Texture2D sprite, Vector2 position, float imageScale, float rotation, Vector2 orgin, bool global) : this(sprite, position, imageScale, rotation)
        {

            if (!global)
                this.localOrgin = orgin;
            else
            {
                Vector2 v = orgin - GlobalPosition;
            }
        }

        public void Draw(SpriteBatch spritebatch)
        {
            if (sprite != null)
                spritebatch.Draw(sprite, GlobalPosition, null, color, GlobalRotation, localOrgin, imageScale, effects, 0);
        }

        public float GlobalRotation
        {
            get
            {
                if (parent != null)
                    return parent.rotation + rotation;
                else
                    return rotation;
            }
        }

        public Vector2 LocalToGlobal(Vector2 local)
        {
            local = XMath.RotateVector(local, rotation) + position;
            if (parent != null)
                return parent.LocalToGlobal(local);
            return local;
        }

        public Vector2 GlobalToLocal(Vector2 global)
        {
            GameObject[] h = Heritage;
            
            for (int i = 0; i < h.Length; i++)
            {
                global = XMath.RotateVector(global - position, -rotation);
            }

            return global;        }

        public GameObject[] Heritage
        {
            get
            {
                GameObject g = this;
                List<GameObject> gs = new List<GameObject>() { g };

                while (g.parent != null)
                {
                    gs.Add(g.parent);
                    g = g.parent;
                }

                List<GameObject> gss = new List<GameObject>();
                for (int i = gs.Count - 1; i >= 0; i--)
                {
                    gss.Add(gs[i]);
                }

                return gss.ToArray();
            }
        }

        public GameObject Ancestor
        {
            get
            {
                GameObject g = this;
                while (g.parent != null)
                {
                    g = g.parent;
                }
                return g;
            }
        }

        public Vector2 GlobalPosition
        {
            get
            {
                if (parent != null)
                    return parent.GlobalPosition + XMath.RotateVector(position, parent.rotation);
                else return position;
            }
        }
    }

    public class PhysicalObject : GameObject
    {
        public Collider collider;

        public PhysicalObject(Collider collider) : base()
        {
            this.collider = collider;
        }

        public PhysicalObject(Collider collider, Texture2D sprite) : base(sprite)
        {
            this.collider = collider;
        }

        public PhysicalObject(Collider collider, Texture2D sprite, Vector2 position) : base(sprite, position)
        {
            this.collider = collider;
        }

        public PhysicalObject(Collider collider, Texture2D sprite, Vector2 position, float imageScale) : base(sprite, position, imageScale)
        {
            this.collider = collider;
        }

        public PhysicalObject(Collider collider, Texture2D sprite, Vector2 position, float imageScale, float rotation) : base(sprite, position, imageScale, rotation)
        {
            this.collider = collider;
        }

        public PhysicalObject(Collider collider, Texture2D sprite, Vector2 position, float imageScale, float rotation, Vector2 orgin, bool global) : base(sprite, position, imageScale, rotation, orgin, global)
        {
            this.collider = collider;
        }
    }
}
