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
        protected World world;

        public void Init(World world)
        {
            this.world = world;
        }

        public GameObject parent;

        public SpriteEffects effects;

        public Texture2D sprite;

        /// <summary>
        /// Sprite size in pixels.
        /// </summary>
        public Vector2 spriteSize;

        /// <summary>
        /// Where the center of the sprite defaults to center.
        /// </summary>
        public Vector2 localOrgin;

        public Vector2 position;

        private float m_rotation;

        public Color color;

        public float Rotation
        {
            set
            {
                while (value >= 360) value -= 360;
                while (value < 0) value += 360;
                m_rotation = value;
            }

            get
            {
                return m_rotation;
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

        public GameObject(Texture2D sprite, Vector2 position, Vector2 imageScale) : this(sprite, position)
        {
            this.spriteSize = imageScale;
        }

        public GameObject(Texture2D sprite, Vector2 position, Vector2 imageScale, float rotation) : this(sprite, position, imageScale)
        {
            this.Rotation = rotation;
        }

        public GameObject(Texture2D sprite, Vector2 position, Vector2 imageScale, float rotation, Vector2 orgin, bool global) : this(sprite, position, imageScale, rotation)
        {

            if (!global)
                this.localOrgin = orgin;
            else
            {
                Vector2 v = orgin - GlobalPosition;
            }
        }

        public virtual void Draw(SpriteBatch spritebatch)
        {
            Texture2D sprite = null;
            if (this.sprite != null) sprite = this.sprite;
            else sprite = XMath.missingTexture;

            Vector2 scale = spriteSize / new Vector2(sprite.Width, sprite.Height);

            spritebatch.Draw(sprite, world.WorldToViewport(GlobalPosition - spriteSize / 2 - localOrgin), null, color, GlobalRotation, localOrgin, scale/world.ViewportZoom, effects, 0);
        }

        public float GlobalRotation
        {
            get
            {
                
                if (parent != null)
                    return parent.Rotation + Rotation;
                else
                    return Rotation;
            }
        }

        public Vector2 LocalVectorToGlobal(Vector2 local)
        {
            return XMath.RotateVector(local, GlobalRotation);
        }

        public Vector2 LocalToGlobal(Vector2 local)
        {
            local = XMath.RotateVector(local, Rotation) + position;
            if (parent != null)
                return parent.LocalToGlobal(local);
            return local;
        }

        public Vector2 GlobalToLocal(Vector2 global)
        {
            GameObject[] h = Heritage;
            
            for (int i = 0; i < h.Length; i++)
            {
                global = XMath.RotateVector(global - position, -Rotation);
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
                    return parent.GlobalPosition + XMath.RotateVector(position, parent.Rotation);
                else return position;
            }
        }
    }

    public class PhysicalObject : GameObject
    {
        private Collider m_collider;

        public Collider Collider
        {
            get
            {
                return m_collider;
            }
            protected set
            {
                value.Init(this);
                m_collider = value;
            }
        }

        public PhysicalObject(Collider collider) : base()
        {
            Collider = collider;
        }

        public PhysicalObject(Collider collider, Texture2D sprite) : base(sprite)
        {
            Collider = collider;
        }

        public PhysicalObject(Collider collider, Texture2D sprite, Vector2 position) : base(sprite, position)
        {
            Collider = collider;
        }

        public PhysicalObject(Collider collider, Texture2D sprite, Vector2 position, Vector2 imageScale) : base(sprite, position, imageScale)
        {
            Collider = collider;
        }

        public PhysicalObject(Collider collider, Texture2D sprite, Vector2 position, Vector2 imageScale, float rotation) : base(sprite, position, imageScale, rotation)
        {
            Collider = collider;
        }
        
        public PhysicalObject(Collider collider, Texture2D sprite, Vector2 position, Vector2 imageScale, float rotation, Vector2 orgin, bool global) : base(sprite, position, imageScale, rotation, orgin, global)
        {
            Collider = collider;
        }
    }

    public class RigidObject : PhysicalObject
    {
        public Vector2 velocity;

        public RigidObject(Collider collider) : base(collider)
        {

        }

        public RigidObject(Collider collider, Texture2D sprite) : base(collider, sprite)
        {

        }

        public RigidObject(Collider collider, Texture2D sprite, Vector2 position) : base(collider, sprite, position)
        {

        }

        public RigidObject(Collider collider, Texture2D sprite, Vector2 position, Vector2 imageScale) : base(collider, sprite, position, imageScale)
        {

        }

        public RigidObject(Collider collider, Texture2D sprite, Vector2 position, Vector2 imageScale, float rotation) : base(collider, sprite, position, imageScale, rotation)
        {

        }

        public virtual void Update(float delta)
        {

        }

        public RigidObject(Collider collider, Texture2D sprite, Vector2 position, Vector2 imageScale, float rotation, Vector2 orgin, bool global) : base(collider, sprite, position, imageScale, rotation, orgin, global)
        {

        }

        public void AddForce(Vector2 force)
        {
            velocity += force;
        }
    }

    public class ExplodableObject : PhysicalObject
    {
        public Vector2 velocity;

        public ExplodableObject(Collider collider) : base(collider)
        {

        }

        public ExplodableObject(Collider collider, Texture2D sprite) : base(collider, sprite)
        {

        }

        public ExplodableObject(Collider collider, Texture2D sprite, Vector2 position) : base(collider, sprite, position)
        {

        }

        public ExplodableObject(Collider collider, Texture2D sprite, Vector2 position, Vector2 imageScale) : base(collider, sprite, position, imageScale)
        {

        }

        public ExplodableObject(Collider collider, Texture2D sprite, Vector2 position, Vector2 imageScale, float rotation) : base(collider, sprite, position, imageScale, rotation)
        {

        }

        public virtual void Update(float delta)
        {

        }

        public ExplodableObject(Collider collider, Texture2D sprite, Vector2 position, Vector2 imageScale, float rotation, Vector2 orgin, bool global) : base(collider, sprite, position, imageScale, rotation, orgin, global)
        {

        }



        public void AddForce(Vector2 force)
        {
            velocity += force;
        }
    }
}



