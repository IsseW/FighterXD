using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterXD.Main
{
    public class GameObject : Object, IDrawable
    {



        public SpriteEffects effects;


        public Texture2D sprite
        {
            get
            {
                return m_sprite;
            }
            set
            {
                m_sprite = value;
                scale = spriteSize / new Vector2(sprite.Width, sprite.Height);
            }
        }

        private Texture2D m_sprite;

        /// <summary>
        /// Sprite size in pixels.
        /// </summary>
        public Vector2 spriteSize
        {
            get
            {
                return m_spriteSize;
            }
            set
            {
                m_spriteSize = value;
                scale = spriteSize / new Vector2(sprite.Width, sprite.Height);
            }
        }
        

        public Rectangle drawRectangle => new Rectangle((Position - spriteSize/2 - localOrgin * spriteSize).ToPoint(), spriteSize.ToPoint());

        public float depth { get => m_depth; set { m_depth = value; if (world != null) { world.SortDepth(); }} }

        private float m_depth = 0;

        private Vector2 m_spriteSize;
        /// <summary>
        /// Where the center of the sprite defaults to center.
        /// </summary>
        public Vector2 localOrgin;

        public Color color = new Color(255, 255, 255, 255);
        
        public GameObject() : base()
        {

        }

        public GameObject(Texture2D sprite) : this()
        {
            this.sprite = sprite;
        }

        public GameObject(Vector2 position) : base(position)
        {

        }
        public GameObject(Vector2 position, float rotation) : base(position, rotation)
        {

        }
        public GameObject(Vector2 position, float rotation, Texture2D sprite) : this(position, rotation)
        {
            this.sprite = sprite;
        }

        public GameObject(Vector2 position, float rotation, Texture2D sprite, Vector2 spriteSize) : this(position, rotation, sprite)
        {
            this.spriteSize = spriteSize;
        }

        public GameObject(Texture2D sprite, Vector2 position, Vector2 spriteSize, float rotation, Vector2 orgin, bool global) : this(position, rotation, sprite, spriteSize)
        {

            if (!global)
                this.localOrgin = orgin;
            else
            {
                Vector2 v = orgin - Position;
            }
        }

        protected Vector2 scale;

        public virtual void Draw(SpriteBatch spritebatch)
        {
            if (enabled && sprite != null)
            {
                //spritebatch.Draw(g.sprite, WorldToViewport(g.GlobalPosition), null, g.color, g.GlobalRotation, (g.localOrgin) * viewport.size, scale * viewport.size, g.effects, 0);
                spritebatch.Draw(sprite, world.WorldToViewport(Position - scale / 2), null, color, GlobalRotation, new Vector2(sprite.Width / 2, sprite.Height / 2) + localOrgin, scale * world.viewport.size, effects, 0);
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

        public PhysicalObject(Collider collider, Texture2D sprite, Vector2 position) : base(position, 0, sprite)
        {
            Collider = collider;
        }

        public PhysicalObject(Collider collider, Texture2D sprite, Vector2 position, Vector2 imageScale) : base(position, 0, sprite, imageScale)
        {
            Collider = collider;
        }

        public PhysicalObject(Collider collider, Texture2D sprite, Vector2 position, Vector2 imageScale, float rotation) : base(position, rotation, sprite, imageScale)
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

        public float timeSinceLastCollision;

        private Vector2[] collisionNormals;

        public bool collisionsOn = true;

        public void SetCollisionNormals(params Vector2[] collisionNormals)
        {
            this.collisionNormals = collisionNormals;
        }

        public Vector2[] GetCollisionNormals()
        {
            return collisionNormals;
        }

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

        public RigidObject(Collider collider, Texture2D sprite, Vector2 position, Vector2 imageScale, float rotation, Vector2 orgin, bool global) : base(collider, sprite, position, imageScale, rotation, orgin, global)
        {

        }

        public void AddForce(Vector2 force)
        {
            Vector2 add = force;
            if (collisionNormals != null)
                foreach (Vector2 v in collisionNormals)
                {
                    float f = Vector2.Dot(add, v);
                    if (f < 0)
                        add -= v * f * 1.2f;
                }

            velocity += add;
        }

        public virtual void OnCollisionEnter(Vector2 force, Collider other, Vector2 point)
        {
            timeSinceLastCollision = 0;
        }
    }

    public class ExplodableObject : PhysicalObject
    {
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

        public ExplodableObject(Collider collider, Texture2D sprite, Vector2 position, Vector2 imageScale, float rotation, Vector2 orgin, bool global) : base(collider, sprite, position, imageScale, rotation, orgin, global)
        {

        }
    }
}



