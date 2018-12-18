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
}
