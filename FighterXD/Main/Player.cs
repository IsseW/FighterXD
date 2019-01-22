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
    public struct PlayerInfo
    {

        public Texture2D bulletTexture;
        public float bulletSpeed;
        public float shootingCooldown;

        public float jumpForce;
        public float speed;
        public float acceleration;
        public Keys left;
        public Keys right;
        public Keys jump;
        public Keys shoot;
        public Keys second;
        public float MaxHealth;
        public PlayerInfo(Keys left, Keys right, Keys jump, Keys shoot, Keys second, float jumpForce, float speed, float acceleration, float MaxHealth, Texture2D bulletTexture, float bulletSpeed, float shootingCooldown)
        {
            this.left = left;
            this.right = right;
            this.jump = jump;
            this.shoot = shoot;
            this.jumpForce = jumpForce;
            this.speed = speed;
            this.acceleration = acceleration;
            this.MaxHealth = MaxHealth;
            this.bulletTexture = bulletTexture;
            this.bulletSpeed = bulletSpeed;
            this.shootingCooldown = shootingCooldown;
            this.second = second;
        }
    }
    public class Player : RigidObject, IUpdateable
    {
        protected override void AddInfoToCopy(Object copy)
        {
            base.AddInfoToCopy(copy);
            ((Player)copy).controls = controls;
            ((Player)copy).InitShootingPlace(null);
        }

        public PlayerInfo controls;
        public float Health { get; private set; }

        private TextObject text;
        private Object shootingPlace;

        public void InitShootingPlace(Object o)
        {
            shootingPlace = o;
            text = GetChildOfType<TextObject>();
            text.text = Health.ToString();
        }


        public Player(PlayerInfo controls, Collider collider) : base(collider)
        {
            this.controls = controls;
            Health = controls.MaxHealth;
        }

        public Player() : base()
        {

        }

        public Player(PlayerInfo controls, Collider collider, Texture2D sprite) : base(collider, sprite)
        {

            this.controls = controls;
            Health = controls.MaxHealth;
        }

        public Player(PlayerInfo controls, Collider collider, Texture2D sprite, Vector2 position) : base(collider, sprite, position)
        {
            this.controls = controls;
            Health = controls.MaxHealth;

        }

        public Player(PlayerInfo controls, Collider collider, Texture2D sprite, Vector2 position, Vector2 imageScale) : base(collider, sprite, position, imageScale)
        {

            this.controls = controls;
            Health = controls.MaxHealth;
        }

        public Player(PlayerInfo controls, Collider collider, Texture2D sprite, Vector2 position, Vector2 imageScale, float rotation) : base(collider, sprite, position, imageScale, rotation)
        {

            this.controls = controls;
            Health = controls.MaxHealth;
        }

        public Player(PlayerInfo controls, Collider collider, Texture2D sprite, Vector2 position, Vector2 imageScale, float rotation, Vector2 orgin, bool global) : base(collider, sprite, position, imageScale, rotation, orgin, global)
        {
            this.controls = controls;
            Health = controls.MaxHealth;

        }


        float timeSinceLast;
        public void Update(float delta)
        {
            if (world.input.keyboard.IsKeyDown(controls.left))
            {
                WalkLeft(delta);
            }
            if (world.input.keyboard.IsKeyDown(controls.right))
            {
                WalkRight(delta);
            }
            if (timeSinceLastCollision < 0.1f && !world.input.keyboard.IsKeyDown(controls.right) && !world.input.keyboard.IsKeyDown(controls.left))
            {
                velocity.X *= 0.8f * (delta + 1);
            }
            if (timeSinceLastCollision < 0.02f && world.input.keyboard.IsKeyDown(controls.jump))
            {
                Jump(delta);
            }
            if (timeSinceLast <= 0 && (world.input.keyboard.IsKeyDown(controls.shoot) || world.input.mouse.LeftButton == ButtonState.Pressed))
            {
                timeSinceLast = controls.shootingCooldown;
                Shoot();
            }
            else if (timeSinceLast > 0)
            {
                timeSinceLast -= delta;
            }
            if (world.input.keyboard.IsKeyDown(controls.second) || world.input.mouse.RightButton == ButtonState.Pressed)
            {
                Dig();
            }
        }

        private void Dig()
        {
            world.Explode(this);
        }

        private void Shoot()
        {
            Vector2 pos = Vector2.Zero;
            Vector2 up = new Vector2(1, 0);
            float rot = 0;

            if (shootingPlace == null) pos = Position;
            else
            {
                pos = shootingPlace.Position;

                up = shootingPlace.LocalVectorToGlobal(up);
                rot = shootingPlace.GlobalRotation;
            }

            ExplodingObject e = new ExplodingObject(new CircleCollider(1), controls.bulletTexture, pos, new Vector2(25, 25), rot) { depth = -4 };
            e.Collider.SetSize();
            e.collisionsOn = false;
            world.Initialize(e);
            e.AddForce(up * controls.bulletSpeed + velocity);
        }

        private void WalkLeft(float delta)
        {
            float movement = -controls.speed * delta / controls.acceleration;
            if (velocity.X > -controls.speed) AddForce(new Vector2(movement, 0));
            Vector2[] c = GetCollisionNormals();
            if (c != null)
            {
                foreach (Vector2 v in c)
                {
                    float d = Vector2.Dot(v, new Vector2(1, 0));
                    if (d > 0)
                    {
                        AddForce(new Vector2(0, (movement - world.g.Y * delta) * d));
                        break;
                    }
                }
            }
        }

        private void WalkRight(float delta)
        {
            float movement = controls.speed * delta / controls.acceleration;
            if (velocity.X < controls.speed) AddForce(new Vector2(movement, 0));
            Vector2[] c = GetCollisionNormals();
            if (c != null)
            {
                foreach (Vector2 v in c)
                {
                    float d = Vector2.Dot(v, new Vector2(-1, 0));
                    if (d > 0)
                    {
                        AddForce(new Vector2(0, (-movement - world.g.Y * delta) * d));
                        break;
                    }
                }
            }
        }

        private void Jump(float delta)
        {
            if (velocity.Y > -controls.jumpForce / 50)AddForce(new Vector2(0, -1) * controls.jumpForce * delta);
        }

        public override void OnCollisionEnter(Vector2 force, Collider other, Vector2 point)
        {
            base.OnCollisionEnter(force, other, point);
            if (force.Length() > 10000)
            {
                Damage(force.Length() / 10000);
            }
        }

        public void Damage(float f)
        {
            if (f > 0)
            {
                Health -= f;
                if (Health < 0) Health = 0;
                text.text = Health.ToString();
            }
        }

        public void Heal(float f)
        {
            if (f > 0)
            {
                Health += f;
                if (Health > controls.MaxHealth) Health = controls.MaxHealth;
                text.text = Health.ToString();
            }
        }
    }

    public class RotateTowardsPoint : GameObject, IUpdateable
    {
        protected override void AddInfoToCopy(Object copy)
        {
            base.AddInfoToCopy(copy);
            ((RotateTowardsPoint)copy).point = point;
        }

        public Vector2 point;

        public virtual void Update(float delta)
        {
            Vector2 v = point - Position;
            if (children.Count > 0)
            {
                GameObject g = (GameObject)children[0];
                if (v.X < 0)
                {
                    g.effects = SpriteEffects.FlipVertically;
                    g.LocalPosition = new Vector2(-Math.Abs(g.LocalPosition.X), g.LocalPosition.Y);
                }
                else
                {
                    g.effects = SpriteEffects.None;
                    g.LocalPosition = new Vector2(Math.Abs(g.LocalPosition.X), g.LocalPosition.Y);
                }
            }
            up = new Vector2(v.X, v.Y);
        }

        public RotateTowardsPoint() : base()
        {
        }

        public RotateTowardsPoint(Texture2D sprite) : base(sprite)
        {
        }

        public RotateTowardsPoint(Texture2D sprite, Vector2 position) : base(position, 0, sprite)
        {
        }

        public RotateTowardsPoint(Texture2D sprite, Vector2 position, Vector2 imageScale) : base(position, 0, sprite, imageScale)
        {
        }

        public RotateTowardsPoint(Texture2D sprite, Vector2 position, Vector2 imageScale, float rotation) : base(position, rotation, sprite, imageScale)
        {
        }

        public RotateTowardsPoint(Texture2D sprite, Vector2 position, Vector2 imageScale, float rotation, Vector2 orgin, bool global) : base(sprite, position, imageScale, rotation, orgin, global)
        {
        }
    }

    public class RotateTowardsMouse : RotateTowardsPoint
    {
        public override void Update(float delta)
        {
            point = world.ViewportToWorld(world.input.mouse.Position.ToVector2());
            base.Update(delta);
        }

        public RotateTowardsMouse() : base()
        {
        }

        public RotateTowardsMouse(Texture2D sprite) : base(sprite)
        {
        }

        public RotateTowardsMouse(Texture2D sprite, Vector2 position) : base(sprite, position)
        {
        }

        public RotateTowardsMouse(Texture2D sprite, Vector2 position, Vector2 imageScale) : base(sprite, position, imageScale)
        {
        }

        public RotateTowardsMouse(Texture2D sprite, Vector2 position, Vector2 imageScale, float rotation) : base(sprite, position, imageScale, rotation)
        {
        }

        public RotateTowardsMouse(Texture2D sprite, Vector2 position, Vector2 imageScale, float rotation, Vector2 orgin, bool global) : base(sprite, position, imageScale, rotation, orgin, global)
        {
        }
    }

    public class ExplodingObject : RigidObject, IUpdateable
    {
        public ExplodingObject(Collider collider) : base(collider)
        {

        }

        public ExplodingObject(Collider collider, Texture2D sprite) : base(collider, sprite)
        {
            
        }

        public ExplodingObject(Collider collider, Texture2D sprite, Vector2 position) : base(collider, sprite, position)
        {


        }

        public ExplodingObject(Collider collider, Texture2D sprite, Vector2 position, Vector2 imageScale) : base(collider, sprite, position, imageScale)
        {
            
        }

        public ExplodingObject(Collider collider, Texture2D sprite, Vector2 position, Vector2 imageScale, float rotation) : base(collider, sprite, position, imageScale, rotation)
        {
            
        }

        public ExplodingObject(Collider collider, Texture2D sprite, Vector2 position, Vector2 imageScale, float rotation, Vector2 orgin, bool global) : base(collider, sprite, position, imageScale, rotation, orgin, global)
        {

        }

        public override void OnCollisionEnter(Vector2 force, Collider other, Vector2 point)
        {
            base.OnCollisionEnter(force, other, point);
            world.Remove(this);
        }

        public void Update(float delta)
        {
            foreach (Enemy e in world.GetEnemies())
            {
                if (Vector2.DistanceSquared(e.Position, Position) <= e.Collider.maxDistSquared + Collider.maxDistSquared && Collider.Collide(e.Collider))
                {
                    e.health -= 1;
                    world.Remove(this);
                }
            }
        }
    }
}
