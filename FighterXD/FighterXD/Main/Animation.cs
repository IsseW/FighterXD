using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterXD.Main
{
    class Animation
    {
        protected List<Texture2D> keyframes;
        public int fps;

        public Texture2D[] Keyframes
        {
            get
            {
                return keyframes.ToArray();
            }
        }

        public Animation(Texture2D[] keyframes, int fps)
        {
            this.keyframes = new List<Texture2D>();

            foreach (Texture2D t in keyframes)
            {
                this.keyframes.Add(t);
            }

            this.fps = fps;
        }
        
    }
}
