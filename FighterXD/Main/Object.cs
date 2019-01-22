using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace FighterXD.Main
{
    public abstract class CopyableObject
    {
        public abstract CopyableObject CreateCopy();
    }

    public class Object : CopyableObject
    {
        protected World world;
        public virtual void Init(World world)
        {
            this.world = world;
        }
        public bool enabled
        {
            get
            {
                return m_enabled;
            }
            set
            {
                m_enabled = value;
                foreach (Object o in children) o.enabled = value;
            }
        }

        public T GetChildOfType<T>()
        {
            foreach (Object o in children)
            {
                if (o.GetType() == typeof(T))
                {
                    return (T)Convert.ChangeType(o, typeof(T));
                }
            }
            return default(T);
        }

        private bool m_enabled = true;

        public virtual void OnDestroy() { }

        public Object Parent
        {
            get
            {
                return m_parent;
            }
            set
            {
                if (m_parent != null) m_parent.children.Remove(this);
                m_parent = value;
                value.children.Add(this);
            }
        }
        private Object m_parent;

        public List<Object> children;

        public Vector2 up
        {
            get
            {
                return LocalVectorToGlobal(new Vector2(0, -1));
            }
            set
            {
                GlobalRotation = (XMath.GetAngle(up, value) + XMath.pi / 2);
            }
        }

        /// <summary>
        /// The objects local position i.e it's position relative to it's parent.
        /// </summary>
        public Vector2 LocalPosition
        {
            get
            {
                return m_localPosition;
            }

            set
            {
                m_localPosition = value;
                if (Parent != null)
                    m_globalPosition = Parent.LocalToGlobal(value);
                else m_globalPosition = value;

                foreach (Object o in children)
                {
                    o.LocalPosition = o.LocalPosition;
                }


            }
        }

        private Vector2 m_localPosition;

        private Vector2 m_globalPosition;
        /// <summary>
        /// The object's global position i.e it's position relative to world zero.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return m_globalPosition;
            }

            set
            {
                m_globalPosition = value;
                if (Parent != null)
                    LocalPosition = Parent.GlobalToLocal(value);
                else LocalPosition = value;

                foreach (Object o in children)
                {
                    o.LocalPosition = o.LocalPosition;
                }
            }
        }

        private float m_localRotation;
        private float m_globalRotation;

        public float Rotation
        {
            get
            {
                return m_localRotation;
            }

            set
            {
                m_localRotation = value;
                if (Parent != null)
                    m_globalRotation = LocalRotationToGlobal(value);
                else
                    m_globalRotation = value;

                foreach (Object o in children)
                {
                    o.Rotation = o.Rotation;
                    o.LocalPosition = o.LocalPosition;
                }
            }
        }

        public float GlobalRotation
        {
            get
            {
                return m_globalRotation;
            }
            set
            {
                m_globalRotation = value;
                if (Parent != null)
                    m_localRotation = GlobalRotationToLocal(value);
                else
                    m_globalRotation = value;

                foreach (Object o in children)
                {
                    o.Rotation = o.Rotation;
                    o.LocalPosition = o.LocalPosition;
                }
            }
        }
        

        public float LocalRotationToGlobal(float local)
        {
            if (Parent != null)
                return local + Parent.GlobalRotation;
            else return local;
        }

        public float GlobalRotationToLocal(float global)
        {
            if (Parent != null)
                return global - Parent.GlobalRotation;
            else return global;
        }

        public Vector2 LocalVectorToGlobal(Vector2 local)
        {
            return XMath.RotateVector(local, GlobalRotation);
        }

        public Vector2 GlobalVectorToLocal(Vector2 global)
        {
            return XMath.RotateVector(global, -GlobalRotation);
        }

        public Vector2 LocalToGlobal(Vector2 local)
        {
            local = XMath.RotateVector(local, Rotation) + LocalPosition;
            if (m_parent != null)
                return m_parent.LocalToGlobal(local);
            return local;
        }

        public Vector2 GlobalToLocal(Vector2 global)
        {
            Object[] h = Heritage;

            for (int i = 0; i < h.Length; i++)
            {
                global = XMath.RotateVector(global - h[i].LocalPosition, -Rotation);
            }

            return global;
        }


        public Object[] Heritage
        {
            get
            {
                Object g = this;
                List<Object> gs = new List<Object>() { g };

                while (g.m_parent != null)
                {
                    gs.Add(g.m_parent);
                    g = g.m_parent;
                }

                List<Object> gss = new List<Object>();
                for (int i = gs.Count - 1; i >= 0; i--)
                {
                    gss.Add(gs[i]);
                }

                return gss.ToArray();
            }
        }

        public Object Ancestor
        {
            get
            {
                Object g = this;
                while (g.m_parent != null)
                {
                    g = g.m_parent;
                }
                return g;
            }
        }

        public Object()
        {
            children = new List<Object>();
        }

        public Object(Vector2 position) : this()
        {
            this.LocalPosition = position;
        }

        public Object(Vector2 position, float rotation) : this(position)
        {
            this.Rotation = rotation;
        }

        protected virtual void AddInfoToCopy(Object copy)
        {
            copy.Position = Position;
            copy.Rotation = Rotation;
            foreach (Object o in children)
            {
                ((Object)o.CreateCopy()).Parent = copy;
            }
        }

        public override CopyableObject CreateCopy()
        {
            Object copy = (Object)Activator.CreateInstance(GetType());
            AddInfoToCopy(copy);
            return copy;
        }
    }

    public class TextObject : Object, IDrawable
    {
        public string text;

        public SpriteFont spriteFont;

        public float scale = 1;

        public Color color;

        public SpriteEffects effects;

        public TextObject() : base()
        {

        }

        public TextObject(SpriteFont spriteFont) : base()
        {
            this.spriteFont = spriteFont;
        }

        public TextObject(SpriteFont spriteFont, Vector2 position) : base(position)
        {
            this.spriteFont = spriteFont;
        }

        public TextObject(SpriteFont spriteFont, Vector2 position, float rotation) : base(position, rotation)
        {
            this.spriteFont = spriteFont;
        }

        public TextObject(SpriteFont spriteFont, Vector2 position, float rotation, float scale) : base(position, rotation)
        {
            this.spriteFont = spriteFont;
            this.scale = scale;
        }

        public Rectangle drawRectangle => new Rectangle((Position - (spriteFont.MeasureString(text) * scale)/2).ToPoint(), (spriteFont.MeasureString(text) * scale).ToPoint());

        public float depth { get => m_depth; set { m_depth = value; if (world != null) { world.SortDepth(); }} }
        private float m_depth = 0;
        public virtual void Draw(SpriteBatch spritebatch)
        {
            if (enabled && text.Length > 0)
            {
                spritebatch.DrawString(spriteFont, text, world.WorldToViewport(Position - new Vector2(scale / 2) * spriteFont.MeasureString(text)), color, GlobalRotation, new Vector2(scale / 2), scale * world.viewport.size, effects, 0);
            }
        }

        protected override void AddInfoToCopy(Object copy)
        {
            base.AddInfoToCopy(copy);
            ((TextObject)copy).spriteFont = spriteFont;
            ((TextObject)copy).text = text;
            ((TextObject)copy).color = color;
            ((TextObject)copy).effects = effects;
            ((TextObject)copy).scale = scale;
        }
    }
}
