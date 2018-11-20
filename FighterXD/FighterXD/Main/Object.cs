using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace FighterXD.Main
{
    public class Object
    {
        protected World world;
        public void Init(World world)
        {
            this.world = world;
        }

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
                return GlobalVectorToLocal(new Vector2(0, -1));
            }
            set
            {
                float rot = XMath.GetAngle(new Vector2(0, -1), up) * XMath.degToRad;
                GlobalRotation = (rot + XMath.GetAngle(up, value));
            }
        }

        public Vector2 position;


        private float m_rotation;


        public float Rotation
        {
            set
            {
                m_rotation = value;
            }

            get
            {
                return m_rotation;
            }
        }


        public float GlobalRotation
        {
            get
            {
                if (m_parent != null)
                    return m_parent.GlobalRotation + Rotation;
                else
                    return Rotation;
            }
            set
            {
                if (m_parent != null) value -= m_parent.GlobalRotation;
                Rotation = value;
            }
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
            local = XMath.RotateVector(local, Rotation) + position;
            if (m_parent != null)
                return m_parent.LocalToGlobal(local);
            return local;
        }

        public Vector2 GlobalToLocal(Vector2 global)
        {
            Object[] h = Heritage;

            for (int i = 0; i < h.Length; i++)
            {
                global = XMath.RotateVector(global - position, -Rotation);
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

        public Vector2 GlobalPosition
        {
            get
            {
                if (m_parent != null)
                    return Parent.LocalToGlobal(position);
                else return position;
            }

            set
            {
                position = GlobalToLocal(value);
            }
        }

        public Object()
        {
            children = new List<Object>();
        }

        public Object(Vector2 position) : this()
        {
            this.position = position;
        }

        public Object(Vector2 position, float rotation) : this(position)
        {
            this.Rotation = rotation;
        }
    }

    public class TextObject : Object, IDrawable
    {
        public string text;

        public SpriteFont spriteFont;

        public Color color;

        public Vector2 size = new Vector2(1, 1);

        public SpriteEffects effects;

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

        public void Draw(SpriteBatch spritebatch)
        {
            
            spritebatch.DrawString(spriteFont, text, world.WorldToViewport(GlobalPosition), color, GlobalRotation, spriteFont.MeasureString(text) * 0.5f, size * world.viewport.size, effects, 0);
        }
    }
}
