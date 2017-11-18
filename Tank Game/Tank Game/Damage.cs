using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Game
{
    public class Damage
    {
        Texture2D hole;
        Vector2 pos;
        float angle, size;

        public Damage(Texture2D Hole, Vector2 position, float Angle, float Size)
        {
            hole = Hole;
            pos = position;
            //pos.X =  pos.X - hole.Width;
            angle = Angle;
            size = Size;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(hole, pos, null, Color.White, angle + (float)Math.PI / 2.0f, new Vector2(hole.Width/2.0f,hole.Height), size, SpriteEffects.None, 0.0f);
            
        }
    }
}
