﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        public float µ = 0.1f;

        public Vector2 g = new Vector2(0, 100);

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
            foreach (GameObject g in gameObjects)
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
                if (gameObject as PhysicalObject != null)
                {
                   physicalObjects.Add((PhysicalObject)gameObject);

                   if (gameObject as RigidObject != null)
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
                if (gameObject as PhysicalObject != null)
                {
                    physicalObjects.Remove((PhysicalObject)gameObject);

                    if (gameObject as RigidObject != null)
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

        public Vector2 ViewportPosition
        {
            get
            {
                return Viewport.Location.ToVector2();
            }
            set
            {
                Viewport = new Rectangle(value.ToPoint(), Viewport.Size);
            }
        }

        public Rectangle Viewport
        {
            get
            {
                return m_viewport;
            }

            set
            {
                Rectangle r = Rect;
                Point p = value.Location;
                Point s = value.Size;

                if (s.X > r.Width) s.X = r.Width;
                if (s.Y > r.Height) s.Y = r.Height;

                if (p.X < r.X) p.X = r.X;
                if (p.Y < r.Y) p.Y = r.Y;

                if (p.X + s.X > r.Right) p.X = r.Right - s.X;
                if (p.Y + s.Y > r.Bottom) p.Y = r.Bottom - s.Y;

                m_viewport = new Rectangle(p, s);
            }
        }
        public void Update(float delta, KeyboardState state)
        {

            //=========================================
            //==============CAMERA MOVEMENT============
            //=========================================
            if (state.IsKeyDown(Keys.A))
            {
                ViewportPosition -= new Vector2(5, 0);
            }
            if (state.IsKeyDown(Keys.D))
            {
                ViewportPosition += new Vector2(5, 0);
            }
            if (state.IsKeyDown(Keys.S))
            {
                ViewportPosition += new Vector2(0, 5);
            }
            if (state.IsKeyDown(Keys.W))
            {
                ViewportPosition -= new Vector2(0, 5);
            }
            if (state.IsKeyDown(Keys.Q))
            {
                ViewportZoom += 0.5f;
            }
            if (state.IsKeyDown(Keys.E))
            {
                ViewportZoom -= 0.5f;
            }

            //=========================================
            //=================PHYSICS=================
            //=========================================
            foreach (RigidObject r in rigidObjects)
            {
                r.Update(delta);
                r.position += r.velocity * delta;
                foreach (PhysicalObject p in physicalObjects)
                {
                    if (r.Collider.Collide(p.Collider, out Vector2 point, out Vector2 myNomral, out Vector2 oNormal))
                    {
                        Vector2 normal = (myNomral + oNormal) / 2;
                        float a = XMath.GetAngle(normal, new Vector2(0, 1));
                        Vector2 rotated = XMath.RotateVector(r.velocity, a);
                        rotated.Y *= -0.5f;
                        r.velocity = XMath.RotateVector(rotated, -a);
                    }
                }
                r.velocity += g * delta;
                r.velocity /= 1 + µ;
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
                if (value <= 1) value = 1.0000001f;
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

        public void Draw(SpriteBatch spritebatch, GameWindow window)
        {
            Texture2D sprite = null;
            if (background != null) sprite = background;
            else sprite = XMath.missingTexture;
            spritebatch.Draw(sprite, Vector2.Zero, Color.White);
            //spritebatch.Draw(sprite, Vector2.Zero, null, Color.White, 0, Vector2.Zero, new Vector2(window.ClientBounds.Width, window.ClientBounds.Height), SpriteEffects.None, 0);
            Console.Write(ViewportPosition);
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
