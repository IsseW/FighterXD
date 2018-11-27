using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FighterXD.Main
{
    public class Enemy : RigidObject, IUpdateable
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
            AddForce(Vector2.Normalize(player.Position - Position));

            if (player.Position.Y < Position.Y)
            {

                Vector2[] c = GetCollisionNormals();
                if (c != null)
                {
                    foreach (Vector2 v in c)
                    {
                        float d = Vector2.Dot(v, new Vector2(-1, 0));
                        if (d > 0)
                        {
                            AddForce(new Vector2(0, (-speed - world.g.Y * delta) * d));
                            break;
                        }
                        else if (d < 0)
                        {
                            AddForce(new Vector2(0, (speed - world.g.Y * delta) * d));
                            break;
                        }
                    }
                }
            }
            Sincelastdamage += delta;

            if (Sincelastdamage > 0.7f && Collider.Collide(player.Collider))
            {
                player.Damage(2);
                Sincelastdamage = 0;
            }
        }
        float Sincelastdamage;
    }
}
