using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aplimat_final_exam.Models
{
    public class Movable
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 Acceleration;
        public float Mass = 1;

        public void ApplyForce(Vector3 force)
        {
            // F = MA
            // A = F/M
            this.Acceleration += (force / Mass); //force accumulation
        }

        public void ApplyGravity(float scalar = 0.1f)
        {
            this.Acceleration += (new Vector3(0, -scalar * Mass, 0) / Mass);
        }

        private void ApplyBounceX(float bouncePosX)//Invert object X velocity
        {
            this.Position.x = bouncePosX;
            this.Velocity.x *= -1;
        }

        private void ApplyBounceY(float bouncePosY)//Invert object Y velocity
        {
            this.Position.y = bouncePosY;
            this.Velocity.y *= -1;
        }

        public void BounceXRight(float bouncePointX)//Apply Bounce along the Right X-axis
        {
            if (this.Position.x >= bouncePointX)
            {
                ApplyBounceX(bouncePointX);
            }
        }

        public void BounceXLeft(float bouncePointX)//Apply Bounce along the Left X-axis
        {
            if (this.Position.x <= bouncePointX)
            {
                ApplyBounceX(bouncePointX);
            }
        }

        public void BounceYTop(float bouncePointY)//Apply Bounce along the Top Y-axis
        {
            if (this.Position.y >= bouncePointY)
            {
                ApplyBounceY(bouncePointY);
            }
        }

        public void BounceYBottom(float bouncePointY)//Apply Bounce along the Bottom Y-axis
        {
            if (this.Position.y <= bouncePointY)
            {
                ApplyBounceY(bouncePointY);
            }
        }
        public void ApplyFriction(float frictionCoefficient = 0.05f, float normalForce = 1.0f)
        {
            var frictionMagnitude = frictionCoefficient * normalForce;
            var friction = this.Velocity;
        }


    }
}
