using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Game
{
    class Medium : Tank
    {
        public Medium(GraphicsDevice device)
        {
            this.name = "Medium Tank";
            this.SetTopSpeed(30);
            this.SetReverseSpeed(10);
            this.SetEngineBraking(0.05f);
            this.SetRotationSpeed(0.025f);
            this.SetWeight(2500.0f);
            this.CalculateAccelleration(350.0f);
            this.hp = 320;

            this.ammo.ammoStore = 150;
            this.ammo.ammoRange = 150f;
            this.ammo.ammoSpeed = 6.9f;
            this.ammo.fireDelay = 0.6f;
            this.ammo.power = 0.4f;

            /*
            AmmoType.shell_3mm
            */            
        }

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
        }
    }
}
