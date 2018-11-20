using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FighterXD.Main
{
    class Enemy : RigidObject,IUpdateable
    {
        float speed;
        Player player;
  
       public Enemy(Texture2D zombie, Collider collider, Vector2 position, Vector2 imageScale, float speed, Player player) : base(collider, zombie, position, imageScale)
       {
            this.speed = speed;
            this.player = player;
       }

        public void Update(float delta)
        {
            AddForce(Vector2.Normalize(player.GlobalPosition - GlobalPosition));
        }
    }
}
