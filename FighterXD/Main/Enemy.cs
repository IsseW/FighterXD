using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FighterXD.Main
{

    public struct EnemyData
    {
        public Texture2D sprite;
        public Vector2 scale;
        public float speed;
        public float maxHealth;
        public Vector2 textPlace;

        public EnemyData(Texture2D sprite, Vector2 scale,  float speed, float hp, Vector2 textPlace)
        {
            this.sprite = sprite;
            this.scale = scale;
            this.speed = speed;
            this.maxHealth = hp;
            this.textPlace = textPlace;
        }
    }

    public class Enemy : RigidObject, IUpdateable
    {


        protected override void AddInfoToCopy(Object copy)
        {
            base.AddInfoToCopy(copy);
            ((Enemy)copy).speed = speed;
            ((Enemy)copy).player = player;
            ((Enemy)copy).maxHealth = maxHealth;
            ((Enemy)copy).health = maxHealth;
        }

        public Enemy() : base()
        {

        }

        float speed;
        Player player;

        public float maxHealth;

        public float health
        {
            get
            {
                return m_health;
            }
            set
            {
                if (value <= 0)
                {
                    value = 0;
                }
                else if (value > maxHealth) value = maxHealth;
                m_health = value;

                if (text != null)
                {
                    text.text = health.ToString();
                }

                if (health == 0)
                {
                    OnDeath.Invoke();
                    world.Remove(this);
                }
            }
        }
        private float m_health;
        private TextObject text;


        public Action OnDeath;

        public Enemy(Texture2D sprite, Collider collider, Vector2 position, Vector2 imageScale, float speed, Player player, float health) : base(collider, sprite, position, imageScale)
        {
            this.speed = speed;
            this.player = player;
            this.maxHealth = health;
            this.health = health;
            
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
                player.Damage(1);
                Sincelastdamage = 0;
            }


            if (text == null)
            {
                text = GetChildOfType<TextObject>();
                if (text != null)
                    text.text = health.ToString();
            }

        }
        float Sincelastdamage;
    }
}
