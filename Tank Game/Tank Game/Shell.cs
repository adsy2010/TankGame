using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Game
{
    public class Shell
    {
        public float speed, range, rotationAngle;
        Vector2 position, gunPosition, startPosition;
        Texture2D img;

        public Shell(Texture2D image, Vector2 pos, Vector2 gunPos, float dist, float spd, float angle)
        {
            img = image;
            startPosition = pos;
            position = startPosition;
            gunPosition = gunPos;
            range = dist;
            speed = spd;
            rotationAngle = angle;
        }
        public Vector2 Position { get { return position; } }
        private Vector2 CalculateDirection()
        {
            Vector2 calculatedDirection = new Vector2((float)Math.Cos(rotationAngle),
                (float)Math.Sin(rotationAngle));
            calculatedDirection.Normalize();
            return calculatedDirection;
        }
        public void Draw(SpriteBatch batch) 
        {
            gunPosition.X = img.Width / 2.0f;
            batch.Draw(img, position, null, Color.Gold, rotationAngle + (float)Math.PI / 2.0f, gunPosition, 1.0f, SpriteEffects.None, 0.0f);
            
        }
        public void Update(GameTime gameTime) 
        {
            //use this in Game1.cs not here
            position += CalculateDirection() * speed;
        }

        public bool OutOfRange()
        {
            if (Vector2.Distance(position, startPosition) > range)
            {
                return true;
            }
            else return false;
        }
    }
}
