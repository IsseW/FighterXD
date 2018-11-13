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


        public Texture2D sprite;

        /// <summary>
        /// Sprite size in pixels.
        /// </summary>
        public Vector2 spriteSize;

        /// <summary>
        /// Where the center of the sprite defaults to center.
        /// </summary>
        public Vector2 localOrgin;

        public Color color;
        
        public GameObject() : base()
        {
            color = Color.White;
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
                Vector2 v = orgin - GlobalPosition;
            }
        }

        public void Draw(SpriteBatch spritebatch)
        {
            if (sprite != null)
            {
                Vector2 scale = spriteSize / new Vector2(sprite.Width, sprite.Height);

                //spritebatch.Draw(g.sprite, WorldToViewport(g.GlobalPosition), null, g.color, g.GlobalRotation, (g.localOrgin) * viewport.size, scale * viewport.size, g.effects, 0);
                spritebatch.Draw(sprite, world.WorldToViewport(GlobalPosition), null, color, GlobalRotation, new Vector2(sprite.Width / 2, sprite.Height / 2), scale * world.viewport.size, effects, 0);
            }
        }
    }

    public class PhysicalObject : GameObject
    {
        private Collider m_collider;

        public bool active = true;

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



