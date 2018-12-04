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
    class Button : TextObject, IUpdateable
    {
        public class OnClickEventArgs : EventArgs
        {

        }
        private enum ButtonState
        {
            Default,Hover,Click
        }
        Color[] colors;
        Texture2D sprite;
        private ButtonState buttonstate;

        public EventHandler<OnClickEventArgs> Onclick;

        public Button(Texture2D xd, string text, Vector2 position, Vector2 Size, SpriteFont font, Color def, Color hov, Color cli, Color textColor) : base(font,position)
        {
            colors = new Color[]
            {
                def,hov,cli
            };
            this.sprite = xd;
            this.text = text;
            this.size = Size;
            color = textColor;

        }

        public void Update(float delta)
        {
            MouseState state = world.input.mouse;
            Vector2 mousePosition = world.ViewportToWorld(state.Position.ToVector2());

        }
        public override void Draw(SpriteBatch spritebatch)
        {
            if (sprite != null)
            {
                Vector2 scale = size / new Vector2(sprite.Width, sprite.Height);
                spritebatch.Draw(sprite, world.WorldToViewport(Position - scale/2), null, colors[(int)buttonstate], GlobalRotation, new Vector2(sprite.Width / 2, sprite.Height / 2), scale * world.viewport.size, effects, 0);
            }
            base.Draw(spritebatch);
        }
    }
}
