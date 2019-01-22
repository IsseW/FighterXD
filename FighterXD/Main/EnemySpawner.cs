using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterXD.Main
{
    class EnemySpawner : GameObject, IUpdateable
    {
        int toSpawn;
        float spawnTime;
        Enemy[] enemies;
        Player player;
        public EnemySpawner(Texture2D tex, Vector2 spriteSize, Vector2 pos, int spawnCount, float spawnTime, Enemy[] enemies, Player player)
        {
            this.sprite = tex;
            this.spriteSize = spriteSize;
            this.Position = pos;
            toSpawn = spawnCount;
            this.spawnTime = spawnTime;
            this.enemies = enemies;
            this.player = player;
            time = spawnTime;
        }
        float time;

        public void Update(float delta)
        {
            Rotation += 0.005f;
            if (time <= 0 && toSpawn > 0)
            {
                time = spawnTime;
                int i = new Random().Next(0, enemies.Length);
                Enemy e = (Enemy)enemies[i].CreateCopy();
                e.Position = Position;
                e.Collider.SetSize();
                toSpawn--;
                Console.WriteLine("Spawned");
                e.OnDeath = () => { toSpawn++; };
                world.Initialize(e);
            }
            else
            {
                time -= delta;
            }
        }
    }
}
