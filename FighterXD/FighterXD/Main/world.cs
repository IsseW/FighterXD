using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterXD.Main
{
    public class World
    {
        public Texture2D background;
        public Vector2 spriteSize;

        private List<GameObject> gameObjects;
        private List<PhysicalObject> physicalObjects;
        private List<RigidObject> rigidObjects;

        public World(Texture2D background, Vector2 spriteSize, Rectangle viewport, params GameObject[] gameObjects)
        {
            this.background = background;
            this.spriteSize = spriteSize;
            Viewport = viewport;

            this.gameObjects = new List<GameObject>();
            physicalObjects = new List<PhysicalObject>();
            rigidObjects = new List<RigidObject>();
            foreach(GameObject g in gameObjects)
            {
                Initialize(g);
            }
            
        }

        public void Initialize(GameObject gameObject)
        {
            if (!gameObjects.Contains(gameObject))
            {
                gameObject.Init(this);
                gameObjects.Add(gameObject);
                if (gameObject.GetType().IsAssignableFrom(typeof(PhysicalObject)))
                {
                    physicalObjects.Add((PhysicalObject)gameObject);
                    
                    if (gameObject.GetType().IsAssignableFrom(typeof(RigidObject)))
                    {
                        rigidObjects.Add((RigidObject)gameObject);
                    }
                }
            }
        }

        public void Remove(GameObject gameObject)
        {
            if (gameObjects.Contains(gameObject))
            {
                gameObjects.Remove(gameObject);
                if (gameObject.GetType().IsAssignableFrom(typeof(PhysicalObject)))
                {
                   physicalObjects.Remove((PhysicalObject)gameObject);

                    if (gameObject.GetType().IsAssignableFrom(typeof(RigidObject)))
                    {
                        rigidObjects.Remove((RigidObject)gameObject);
                    }
                }
            }
        }

        public Rectangle Rect
        {
            get
            {
                return new Rectangle((-spriteSize / 2).ToPoint(), spriteSize.ToPoint());
            }
        }

        private Rectangle m_viewport;

        public Rectangle Viewport
        {
            get
            {
                return m_viewport;
            }

            set
            {
                Rectangle r = Rect;
                Point p;
                Point s;

                if (value.Width > r.Width) s.X = r.Width;
                if (value.Height > r.Height) s.Y = r.Height;

                if (value.Location.X < r.X) p.X = r.X;
                if (value.Location.Y < r.Y) p.Y = r.Y;

                if (value.Location.X + value.Width > r.Left) p.X = r.Left - value.Width;
                if (value.Location.Y + value.Height > r.Bottom) p.Y = r.Bottom - value.Height;

                m_viewport = value;
            }
        }

        private float m_viewportZoom;

        /// <summary>
        /// Viewport zoom.
        /// for example if viewport zoom is 2 the size of each pixel in the vieport is 2x bigger than if the viewport covered the whole map.
        /// </summary>
        public float ViewportZoom
        {
            get
            {
                return m_viewportZoom;
            }
            set
            {
                if (value <= 1) throw new Exception("Viewport zoom must be larger than one");
                m_viewportZoom = value;
            }
        }

        private float m_viewportRatio;

        /// <summary>
        /// Height / width ratio
        /// </summary>
        public float ViewportRatio
        {
            get
            { 
                return m_viewportRatio;
            }
            set
            {
                if (value <= 0) throw new Exception("Viewport ratio must be larger than zero");
                ViewportRatio = value;
            }
        }

        public void Draw(SpriteBatch spritebatch)
        {
            Texture2D sprite = null;
            if (background != null) sprite = background;
            else sprite = XMath.missingTexture;

            Vector2 scale = spriteSize / new Vector2(sprite.Width, sprite.Height);

            spritebatch.Draw(sprite, WorldToViewport(- spriteSize / 2), null, Color.White, 0, Vector2.Zero, scale / ViewportZoom, SpriteEffects.None, 0);

            foreach (GameObject g in gameObjects)
            {
                g.Draw(spritebatch);
            }
        }

        public Vector2 WorldToViewport(Vector2 point)
        {

            return (point - Viewport.Location.ToVector2()) * ViewportZoom;
        }

    }
}
