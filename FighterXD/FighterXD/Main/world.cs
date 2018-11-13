using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterXD.Main
{
    public struct Input
    {
        public KeyboardState keyboard;
        public MouseState mouse;

        public Input(KeyboardState keyboard, MouseState mouse)
        {
            this.keyboard = keyboard;
            this.mouse = mouse;
        }
    }

    public struct Viewport
    {
        public Vector2 center;

        public Vector2 ratio;

        public float size;

        public Vector2 Size
        {
            get
            {
                return ratio * size;
            }
        }

        public Viewport (Vector2 center, Vector2 ratio)
        {
            this.center = center;
            this.ratio = ratio;
            size = 1;
        }
    }

    public class World
    {
        public Input input;
        public Texture2D background;
        public Vector2 worldSize;
        public float µ = 0.1f;

        public Vector2 g = new Vector2(0, 3000);

        private List<Object> objects;
        private List<PhysicalObject> physicalObjects;
        private List<RigidObject> rigidObjects;
        private List<ExplodableObject> explodableObjects;
        private List<IUpdateable> updateables;
        private List<IDrawable> drawables;

        public World(Texture2D background, Vector2 worldSize, Viewport viewport, params GameObject[] gameObjects)
        {
            this.background = background;
            this.worldSize = worldSize;
            this.viewport = viewport;
            this.objects = new List<Object>();
            physicalObjects = new List<PhysicalObject>();
            rigidObjects = new List<RigidObject>();
            explodableObjects = new List<ExplodableObject>();
            updateables = new List<IUpdateable>();
            drawables = new List<IDrawable>();
            foreach (GameObject g in gameObjects)
            {
                Initialize(g);
            }
        }

        public void Explode(PhysicalObject toCheck)
        {
            foreach (ExplodableObject e in explodableObjects.ToList())
            {
                if (toCheck.Collider.Collide(e.Collider))
                {
                    Remove(e);
                }
            }
        }

        public void Explode(Collider collider, Vector2 position)
        {
            collider.Update(position);
            foreach (ExplodableObject e in explodableObjects.ToList())
            {
                if (Vector2.DistanceSquared(position, e.position) <= collider.maxDistSquared + e.Collider.maxDistSquared && collider.Collide(e.Collider))
                {
                    Remove(e);
                }
            }
        }

        public void Initialize(Object @object)
        {
            if (!objects.Contains(@object))
            {
                @object.Init(this);
                objects.Add(@object);
                if (@object as PhysicalObject != null)
                {

                   if (@object as RigidObject != null)
                   {
                       rigidObjects.Add((RigidObject)@object);
                   }
                   else
                   {
                        physicalObjects.Add((PhysicalObject)@object);
                        if (@object as ExplodableObject != null)
                        {
                            explodableObjects.Add((ExplodableObject)@object);
                        }
                    }
                }
                if (@object.GetType().GetInterfaces().Contains(typeof(IUpdateable)))
                {
                    updateables.Add((IUpdateable)@object);
                }
                if (@object.GetType().GetInterfaces().Contains(typeof(IDrawable)))
                {
                    drawables.Add((IDrawable)@object);
                }
            }

            foreach(Object g in @object.children)
            {
                Initialize(g);
            }
        }

        public void Remove(Object @object)
        {
            if (objects.Contains(@object))
            {
                objects.Remove(@object);
                if (@object as PhysicalObject != null)
                {
                    if (@object as RigidObject != null)
                    {
                        rigidObjects.Remove((RigidObject)@object);
                    }
                    else
                    {
                        physicalObjects.Remove((PhysicalObject)@object);
                        if (@object as ExplodableObject != null)
                        {
                            explodableObjects.Remove((ExplodableObject)@object);
                        }
                    }
                }
            }
            if (@object.GetType().GetInterfaces().Contains(typeof(IUpdateable)))
            {
                updateables.Remove((IUpdateable)@object);
            }
            if (@object.GetType().GetInterfaces().Contains(typeof(IDrawable)))
            {
                drawables.Remove((IDrawable)@object);
            }
            foreach (Object g in @object.children)
            {
                Remove(g);
            }
        }

        public Rectangle Rect
        {
            get
            {
                return new Rectangle((-worldSize / 2).ToPoint(), worldSize.ToPoint());
            }
        }

        public Viewport viewport;

        void ZoomViewport(float x)
        {
            viewport.size *= 1 + x;
        }

        public void Update(float delta, KeyboardState state, MouseState mouseState, GameWindow window)
        {
            input = new Input(state, mouseState);
            //=========================================
            //==============CAMERA MOVEMENT============
            //=========================================
            if (mouseState.Position.X <= 0 && viewport.center.X > -worldSize.X/2)
            {
                viewport.center -= new Vector2(5, 0) / viewport.size;
            }
            if (mouseState.Position.X >= window.ClientBounds.Width - 1 && viewport.center.X < worldSize.X / 2)
            {
                viewport.center += new Vector2(5, 0) / viewport.size;
            }
            if (mouseState.Position.Y >= window.ClientBounds.Height - 1 && viewport.center.Y < worldSize.Y / 2)
            {
                viewport.center += new Vector2(0, 5) / viewport.size;
            }
            if (mouseState.Position.Y <= 0&& viewport.center.Y > -worldSize.Y / 2)
            {
                viewport.center -= new Vector2(0, 5) / viewport.size;
            }
            if (state.IsKeyDown(Keys.Q))
            {
                ZoomViewport(0.03f);
            }
            if (state.IsKeyDown(Keys.E))
            {
                ZoomViewport(-0.03f);
            }

            //=========================================
            //=================PHYSICS=================
            //=========================================
            foreach (RigidObject r in rigidObjects.ToList())
            {
                if (!Rect.Contains(r.position.ToPoint())) Remove(r);
                r.timeSinceLastCollision += delta;
                bool col = false;
                List<Vector2> no = new List<Vector2>();
                if (r.Collider != null)
                {
                    foreach (PhysicalObject p in physicalObjects.ToList())
                    {
                        if (p.active)
                        {
                            if (Vector2.DistanceSquared(p.position, r.position) <= r.Collider.maxDistSquared + p.Collider.maxDistSquared && r.Collider.Collide(p.Collider, out Vector2 otherPoint, out Vector2 myPoint, out Vector2 oNormal))
                            {
                                Vector2 velOld = r.velocity;
                                float velAlongNormal = Vector2.Dot(r.velocity, oNormal);
                                if (velAlongNormal < 0)
                                {
                                    r.AddForce(-oNormal * velAlongNormal * (delta) * Vector2.Distance(otherPoint, myPoint));
                                    if (!col) col = true;
                                    no.Add(oNormal);
                                    r.OnCollisionEnter(velOld, p.Collider, myPoint);
                                }


                            }
                        }
                    }
                    r.SetCollisionNormals(no.ToArray());
                }

                Vector2 add = r.velocity;
                if (r.GetCollisionNormals() != null)
                    foreach (Vector2 v in r.GetCollisionNormals())
                    {
                        float f = Vector2.Dot(add, v);
                        if (f < 0)
                            add -= v * f;
                    }

                r.position += add * delta;
                
                r.AddForce(g * delta);
                r.velocity /= 1 + µ * delta;
            }
            //=========================================
            //=================UPDATE=================
            //=========================================
            foreach (IUpdateable i in updateables)
            {
                i.Update(delta);
            }
        }


        public void Draw(SpriteBatch spritebatch, GameWindow window)
        {
            Texture2D sprite = null;
            if (background != null) sprite = background;
            else sprite = XMath.missingTexture;
            spritebatch.Draw(sprite, Vector2.Zero, Color.White);
            Rectangle v = new Rectangle((viewport.center - viewport.Size / (2 * viewport.size)).ToPoint(), viewport.Size.ToPoint());
            foreach (IDrawable d in drawables)
            {
                d.Draw(spritebatch);
            }
        }

        public Vector2 WorldToViewport(Vector2 point)
        {
            return ((point - viewport.center) * viewport.size + viewport.Size/(2 * viewport.size));
        }
        
        public Vector2 ViewportToWorld(Vector2 point)
        {
            return (point - viewport.Size / (2 * viewport.size)) / viewport.size + viewport.center;
        }
    }
}
