using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Game
{
    class Heavy : Tank
    {
        public Heavy(GraphicsDevice device)
        {
            this.name = "Heavy Tank";
            this.SetTopSpeed(20);
            this.SetReverseSpeed(8);
            this.SetEngineBraking(0.04f);
            this.SetRotationSpeed(0.01f);
            this.SetWeight(3300.0f);
            this.CalculateAccelleration(4000.0f);
            this.hp = 450;
            
            /*
            AmmoType.shell_8mm
            */

            this.ammo.ammoStore = 68;
            this.ammo.ammoRange = 250f;
            this.ammo.ammoSpeed = 5.3f;
            this.ammo.fireDelay = 1.5f;
            this.ammo.power = 1.0f;
        }

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
        }
    }
}
