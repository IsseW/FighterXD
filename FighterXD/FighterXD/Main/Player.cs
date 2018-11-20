﻿using Microsoft.Xna.Framework;
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
        public float MaxHealth;
        public PlayerInfo(Keys left, Keys right, Keys jump, Keys shoot, float jumpForce, float speed, float acceleration, float MaxHealth, Texture2D bulletTexture, float bulletSpeed, float shootingCooldown)
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
        }
    }
    public class Player : RigidObject, IUpdateable
    {
        public PlayerInfo controls;
        public float Health { get; private set; }

        private Object shootingPlace;

        public void InitShootingPlace(Object o)
        {
            shootingPlace = o;
        }


        public Player(PlayerInfo controls, Collider collider) : base(collider)
        {
            this.controls = controls;
            Health = controls.MaxHealth;
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
        }

        private void Shoot()
        {
            Vector2 pos = Vector2.Zero;
            Vector2 up = new Vector2(1, 0);
            float rot = 0;

            if (shootingPlace == null) pos = GlobalPosition;
            else
            {
                pos = shootingPlace.GlobalPosition;

                up = shootingPlace.LocalVectorToGlobal(up);
                Console.WriteLine(up);
                rot = shootingPlace.GlobalRotation;
            }

            ExplodingObject e = new ExplodingObject(new CircleCollider(1), controls.bulletTexture, pos, new Vector2(25, 25), rot);
            e.Collider.SetSize();
            world.Initialize(e);
            e.AddForce(up * controls.bulletSpeed + velocity);
        }

        private void WalkLeft(float delta)
        {
            if (velocity.X > -controls.speed) AddForce(new Vector2(-controls.speed * delta / controls.acceleration, 0));
        }

        private void WalkRight(float delta)
        {
            if (velocity.X < controls.speed) AddForce(new Vector2(controls.speed * delta / controls.acceleration, 0));
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
                world.Explode(this);
            }
        }

        public void Damage(float f)
        {
            if (f > 0)
            {
                Health -= f;
                if (Health < 0) Health = 0;
            }
        }

        public void Heal(float f)
        {
            if (f > 0)
            {
                Health += f;
                if (Health > controls.MaxHealth) Health = controls.MaxHealth;
            }
        }
    }

    public class RotateTowardsPoint : GameObject, IUpdateable
    {
        public Vector2 point
        {
            get
            {
                return LocalToGlobal(local);
            }
            set
            {
                local = GlobalToLocal(value);
            }
        }
        private Vector2 local;
        public virtual void Update(float delta)
        {
            up = point - GlobalPosition;
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
            point = world.input.mouse.Position.ToVector2() - world.WorldToViewport(GlobalPosition);
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

    public class ExplodingObject : RigidObject
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

            world.Remove(other.GameObject);

        }

    }
}
