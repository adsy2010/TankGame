using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tank_Game
{
    public class Ammo
    {
        public int ammo { get; set; } //type
        public int ammoStore { get; set; } //stored amount
        public float ammoSpeed { get; set; } //speed of shell
        public float ammoRange { get; set; } //ammo range
        public float fireDelay { get; set; }
        public double fireTime { get; set; }
        public float power;


        public Ammo()
        {

        }


    }
}
