using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Tank_Game
{
    class AI
    {
        byte turnLeft = 0x01;
        byte turnRight = 0x02;
        byte moveForward = 0x03;
        byte moveBackward = 0x04;
        byte fire = 0x05;

        /*
        public byte action(bool edge, Vector2 closestTarget, Vector2 self)
        {
            if (edge)
            {
                return turnLeft;
            }
            if (Vector2.Distance(closestTarget, self) < 20.0f)
            {
                return moveBackward;
            }

            if (Vector2.Distance(closestTarget, self) > 20.0f)
            {
                return moveForward;
            }
        }
         */
    }
}
