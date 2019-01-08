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
    class Button : GameObject, IUpdateable
    {
        private enum ButtonState
        {
            Default,Hover,Click
        }
        Color[] colors;
        private ButtonState buttonstate;
        private Action onClick;
        SpriteFont font;
        public string text
        {
            get
            {
                return m_text;
            }
            set
            {
                m_text = value;
                Vector2 size = font.MeasureString(value);
                
                Vector2 s =  drawRectangle.Size.ToVector2() / size;
                if (s.X < s.Y)
                {
                    textScale = s.X;
                }
                else
                {
                    textScale = s.Y;
                }
            }
        }
        private string m_text;

        public Button(Texture2D texture, string text, Vector2 position, Vector2 size, SpriteFont font, Color def, Color hov, Color cli, Color textColor, Action onClick) : base(position, 0, texture, size)
        {
            colors = new Color[]
            {
                def,hov,cli
            };
            color = textColor;
            this.onClick = onClick;
            this.font = font;
            this.text = text;
        }

        public void Update(float delta)
        {
            MouseState state = world.input.mouse;
            Vector2 mousePosition = world.ViewportToWorld(state.Position.ToVector2());
            if (drawRectangle.Contains(mousePosition.ToPoint()))
            {
                if (state.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    buttonstate = ButtonState.Click;
                }
                else if (buttonstate == ButtonState.Click)
                {
                    buttonstate = ButtonState.Hover;
                    Game1.PlaySound("buttonClick");
                    onClick.Invoke();
                }
                else
                {
                    buttonstate = ButtonState.Hover;
                }
            }
            else
            {
                buttonstate = ButtonState.Default;
            }
        }
        float textScale;
        public override void Draw(SpriteBatch spritebatch)
        {
            if (enabled && sprite != null)
            {
                Vector2 viewportPos = world.WorldToViewport(Position - scale / 2);
                spritebatch.Draw(sprite, viewportPos, null, colors[(int)buttonstate], GlobalRotation, new Vector2(sprite.Width / 2, sprite.Height / 2), scale * world.viewport.size, effects, 0);
                spritebatch.DrawString(font, text, viewportPos - (new Vector2(textScale * world.viewport.size / 2)), color, 0, new Vector2((textScale / 2) * world.viewport.size), textScale * world.viewport.size, SpriteEffects.None, 0);
            }
        }
    }

    public class Slider : GameObject, IUpdateable
    {
        public float value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = XMath.Clamp(value, min, max);
                onChange.Invoke(m_value);
                m_buttonX = maxX * (value - min) / max + minX;
            }
        }

        float m_value;

        public float min { get; private set; }
        public float max { get; private set; }
        public float minX { get; private set; }
        public float maxX { get; private set; }

        public float buttonSize
        {
            get
            {
                return m_buttonSize;
            }
            set
            {
                m_buttonSize = value;
                buttonScale = value / buttonSprite.Width;
            }
        }
        private float m_buttonSize;
        Action<float> onChange;
        Texture2D buttonSprite;
        public Slider(float value, float min, float max, Action<float> onChange, Texture2D buttonSprite, Texture2D backSprite, Vector2 position, Vector2 sliderSize, float buttonSize)
        {
            this.sprite = backSprite;
            this.buttonSprite = buttonSprite;
            this.spriteSize = sliderSize;
            this.Position = position + sliderSize/2;
            this.localOrgin = new Vector2(0.5f, 0.5f);
            this.onChange = onChange;
            this.buttonSize = buttonSize;
            this.max = max;
            this.min = min;
            maxX = (spriteSize.X - spriteSize.X / 2) * ((max - min) / max) - buttonScale/2;
            minX = -maxX;
            this.value = value;
        }

        public Vector2 buttonPos
        {
            get
            {
                return LocalToGlobal(new Vector2(m_buttonX, 0)) - scale / 2;
            }
            set
            {
                m_buttonX = XMath.Clamp(value.X - buttonScale/2, minX, maxX);
                m_value = max*(m_buttonX - minX) / maxX + min;
                onChange.Invoke(m_value);
            }
        }
        private float m_buttonX;

        protected float buttonScale;

        public override void Draw(SpriteBatch spritebatch)
        {
            base.Draw(spritebatch);
            spritebatch.Draw(buttonSprite, world.WorldToViewport(buttonPos), 
                             null, Color.Blue, Rotation, new Vector2(0.5f, 0.5f), buttonScale * world.viewport.size, effects, 0);
            //Rotation += 0.1f;
        }

        public void Update(float delta)
        {
            MouseState state = world.input.mouse;
            if (state.LeftButton == ButtonState.Pressed && !click)
            {
                click = true;
                Vector2 mousePosition = world.ViewportToWorld(state.Position.ToVector2());
                if (Vector2.Distance(mousePosition, buttonPos) < buttonScale * world.viewport.size)
                {
                    hold = true;
                }
            }
            else if (click && state.LeftButton == ButtonState.Released)
            {
                click = false;
                hold = false;
            }
            if (hold)
            {
                buttonPos = world.ViewportToWorld(state.Position.ToVector2());
            }
        }
        bool click;
        bool hold;
    }
}
