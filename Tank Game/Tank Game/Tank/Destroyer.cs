using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Game
{
    class Destroyer : Tank
    {

        public Destroyer()
        {
            this.name = "Destroyer";
            this.SetTopSpeed(37);
            this.SetReverseSpeed(12);
            this.SetEngineBraking(0.03f);
            this.SetRotationSpeed(0.04f);
            this.SetWeight(1500.0f);
            this.CalculateAccelleration(320.0f);
            this.hp = 200;


            /*
            this.ammo = (int)ammotype;

            AmmoType.shell_12mm
            */

            this.ammo.ammoStore = 88;
            this.ammo.ammoRange = 200f;
            this.ammo.ammoSpeed = 4.0f;
            this.ammo.fireDelay = 2.0f;
            this.ammo.power = 1.5f;
        }

        public void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
        }
    }
}
