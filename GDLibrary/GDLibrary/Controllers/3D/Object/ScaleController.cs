using Microsoft.Xna.Framework;
using System;

namespace GDLibrary
{
    public class ScaleController : Controller
    {
        #region Fields
        private readonly float scaleMagnitude;
        #endregion

        public ScaleController(
            string id,
            ControllerType controllerType,
            float scaleMagnitude
        ) : base(id, controllerType) {
            this.scaleMagnitude = scaleMagnitude;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            Actor3D parentActor = actor as Actor3D;

            //See: https://gamedev.stackexchange.com/questions/28163/can-someone-explain-this-pulsate-code-to-me
            //Accessed: January 2020
            if (actor != null)
            {
                double time = gameTime.TotalGameTime.TotalSeconds;
                float pulsate = (float)Math.Sin(time * 2) + 1;

                parentActor.Transform.Scale = new Vector3((1 + pulsate * scaleMagnitude), (1 + pulsate * scaleMagnitude), (1 + pulsate * scaleMagnitude));
            }
            //End of reference

            base.Update(gameTime, actor);
        }
    }
}