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
    public struct PlayerControls
    {
        public Keys left;
        public Keys right;
        public Keys jump;
        public Keys shoot;

        public PlayerControls(Keys left, Keys right, Keys jump, Keys shoot)
        {
            this.left = left;
            this.right = right;
            this.jump = jump;
            this.shoot = shoot;
        }
    }
    public class Player : RigidObject
    {
        public PlayerControls controlls;

        public Player(Collider collider) : base(collider)
        {

        }

        public Player(Collider collider, Texture2D sprite) : base(collider, sprite)
        {

        }

        public Player(Collider collider, Texture2D sprite, Vector2 position) : base(collider, sprite, position)
        {

        }

        public Player(Collider collider, Texture2D sprite, Vector2 position, Vector2 imageScale) : base(collider, sprite, position, imageScale)
        {

        }

        public Player(Collider collider, Texture2D sprite, Vector2 position, Vector2 imageScale, float rotation) : base(collider, sprite, position, imageScale, rotation)
        {

        }

        public Player(Collider collider, Texture2D sprite, Vector2 position, Vector2 imageScale, float rotation, Vector2 orgin, bool global) : base(collider, sprite, position, imageScale, rotation, orgin, global)
        {

        }

        public override void Update(float delta)
        {

        }

    }
}
