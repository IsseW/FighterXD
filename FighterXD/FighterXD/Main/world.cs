using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
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
        private List<Enemy> enemies;

        public List<T> GetObjectsOfType<T>()
        {
            List<T> o = new List<T>();
            if (typeof(T).IsAssignableFrom(typeof(RigidObject)))
            {
                foreach (RigidObject r in rigidObjects)
                {
                    if (r.GetType().IsAssignableFrom(typeof(T)))
                    {
                        o.Add((T)Convert.ChangeType(r, typeof(T)));
                    }
                }
            }
            else if (typeof(T).IsAssignableFrom(typeof(PhysicalObject)))
            {
                foreach (PhysicalObject p in physicalObjects)
                {
                    if (p.GetType().IsAssignableFrom(typeof(T)))
                    {
                        o.Add((T)Convert.ChangeType(p, typeof(T)));
                    }
                }
            }
            else if (typeof(T).IsAssignableFrom(typeof(Object)))
            {
                foreach (Object obj in objects)
                {
                    if (obj.GetType().IsAssignableFrom(typeof(T)))
                    {
                        o.Add((T)Convert.ChangeType(obj, typeof(T)));
                    }
                }
            }

            return o;
        }

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
            enemies = new List<Enemy>();
            comparer = new Comparerer();
            foreach (GameObject g in gameObjects)
            {
                Initialize(g);
            }
        }
        
        public async void Explode(PhysicalObject toCheck)
        {
            Task t = new Task(new Action(delegate { M_Explode(toCheck); }));
            t.Start();
            await t;
        }

        private void M_Explode(PhysicalObject toCheck)
        {
            foreach (ExplodableObject e in explodableObjects.ToList())
            {
                if (e != null && Vector2.DistanceSquared(toCheck.LocalPosition, e.LocalPosition) <= toCheck.Collider.maxDistSquared + e.Collider.maxDistSquared && toCheck.Collider.Collide(e.Collider))
                {
                    Remove(e);
                }
            }
        }

        private async void M_Explode(Collider collider, Vector2 position)
        {
            Task t = new Task(new Action(delegate { M_Explode(collider, position); }));
            t.Start();
            await t;

        }

        public void Explode(Collider collider, Vector2 position)
        {
            collider.Update(position);
            foreach (ExplodableObject e in explodableObjects.ToList())
            {
                if (e != null && Vector2.DistanceSquared(position, e.LocalPosition) <= collider.maxDistSquared + e.Collider.maxDistSquared && collider.Collide(e.Collider))
                {
                    Remove(e);
                }
            }
        }

        public Enemy[] GetEnemies()
        {
            return enemies.ToArray();
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
                        if (@object as Enemy != null)
                        {
                            enemies.Add((Enemy)@object);
                        }
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
            @object.Rotation = @object.Rotation;
            foreach(Object g in @object.children)
            {
                Initialize(g);
            }
        }

        public void SortDepth()
        {
            drawables.Sort(comparer);
        }

        public void Remove(Object @object)
        {
            try
            {
                if (objects.Contains(@object))
                {
                    @object.OnDestroy();
                    objects.Remove(@object);
                    if (@object as PhysicalObject != null)
                    {
                        if (@object as RigidObject != null)
                        {
                            rigidObjects.Remove((RigidObject)@object);
                            if (@object as Enemy != null)
                            {
                                enemies.Remove((Enemy)@object);
                            }
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
                    int index = drawables.IndexOf((IDrawable)@object);
                    if (index >= 0 && index < drawables.Count)
                        drawables.RemoveAt(index);
                }
                foreach (Object g in @object.children)
                {
                    Remove(g);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("idk what is going on - " + e);
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

        public async void Update(float delta, KeyboardState state, MouseState mouseState, GameWindow window)
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
            List<Task> physics = new List<Task>();
            PhysicalObject[] p = physicalObjects.ToArray();
            for (int i = 0; i < rigidObjects.Count; i++)
            {
                RigidObject r = rigidObjects[i];
                if (r.enabled)
                    physics.Add(new Task(new Action(delegate { ValidateRigidObject(r, delta, p); })));
            }
            foreach (Task t in physics)
            {
                t.Start();
            }
            await Task.WhenAll(physics);
            //=========================================
            //=================UPDATE=================
            //=========================================
            foreach (IUpdateable i in updateables.ToList())
            {
                if (i != null)
                    i.Update(delta);
            }
        }

        private void ValidateRigidObject(RigidObject r, float delta, PhysicalObject[] phys)
        {
            if (r.enabled)
            {
                if (!Rect.Contains(r.LocalPosition.ToPoint())) r.enabled = false;

                r.timeSinceLastCollision += delta;
                bool col = false;
                List<Vector2> no = new List<Vector2>();
                if (r.Collider != null)
                {
                    foreach (PhysicalObject p in phys)
                    {
                        if (p != null && p.enabled)
                        {
                            Vector2 dif = p.Position - r.Position;
                            if (dif.LengthSquared() <= r.Collider.maxDistSquared + p.Collider.maxDistSquared && Vector2.Dot(dif, r.velocity) > 0.5f && r.Collider.Collide(p.Collider, out Vector2 otherPoint, out Vector2 myPoint, out Vector2 oNormal))
                            {
                                Vector2 velOld = r.velocity;
                                float velAlongNormal = Vector2.Dot(r.velocity, oNormal);
                                if (velAlongNormal < 0)
                                {
                                    r.AddForce(-oNormal * velAlongNormal * (delta) * Vector2.Distance(otherPoint, myPoint));
                                    col = true;
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

                r.LocalPosition += add * delta;

                r.AddForce(g * delta);
                r.velocity /= 1 + µ * delta;
            }
        }
        Comparerer comparer;
        public void Draw(SpriteBatch spritebatch, GameWindow window)
        {
            Texture2D sprite = null;
            if (background != null) sprite = background;
            else sprite = XMath.missingTexture;
            spritebatch.Draw(sprite, new Rectangle(Point.Zero, window.ClientBounds.Size), Color.White);
            Rectangle v = new Rectangle(ViewportToWorld(Vector2.Zero).ToPoint(), new Point((int)(window.ClientBounds.Width / viewport.size), (int)(window.ClientBounds.Height / viewport.size)));

            foreach (IDrawable d in drawables.ToList())
            {
                if (d != null && v.Intersects(d.drawRectangle))
                {
                    d.Draw(spritebatch);
                }
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
