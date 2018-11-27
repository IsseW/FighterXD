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

        public Button(Texture2D xd, string text, Vector2 position, Vector2 Size, SpriteFont font, Color def, Color hov, Color cli) : base(font,position)
        {
            colors = new Color[]
            {
                def,hov,cli
            };
            this.sprite = xd;
            this.text = text;
            this.size = Size;

        }

        public void Update(float delta)
        {
            MouseState state = world.input.mouse;
            Vector2 mousePosition = world.ViewportToWorld(state.Position.ToVector2());
        }
    }
}
