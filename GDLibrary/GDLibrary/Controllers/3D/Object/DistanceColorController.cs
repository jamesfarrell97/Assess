using Microsoft.Xna.Framework;
using System;

namespace GDLibrary
{
    public class DistanceColorController : Controller
    {
        #region Fields
        ObjectManager objectManager;
        Vector3 blockDimension;
        bool colorUpdated;
        #endregion

        #region Properties
        public ObjectManager ObjectManager { get => objectManager; set => objectManager = value; }
        public Vector3 BlockDimension { get => blockDimension; set => blockDimension = value; }
        public bool ColorUpdated { get => colorUpdated; set => colorUpdated = value; }
        #endregion

        #region Constructors
        public DistanceColorController(
            string id, 
            ControllerType controllerType,
            ObjectManager objectManager,
            Vector3 blockDimension
        ) : base(id, controllerType) {
            this.ObjectManager = objectManager;
            this.BlockDimension = blockDimension;
            this.ColorUpdated = false;
        }

        #endregion

        #region Methods
        public void CalculateColorBasedOnDistance(Actor3D parentActor)
        {
            if (!this.ColorUpdated)
            {
                float distance0 = float.MaxValue;
                Vector3 position0 = Vector3.Zero;
                Vector3 objectColor = Vector3.Zero;
                Vector3 updatedColor = Vector3.Zero;
                Vector3 colorVector = new Vector3(0.1f, 0.1f, 0.1f);
                Vector3 lookVector;

                foreach (CollidablePrimitiveObject opaquePrimitiveObject in this.ObjectManager.OpaqueDrawList)
                {
                    if (Vector3.Distance(parentActor.Transform.Translation, opaquePrimitiveObject.Transform.Translation) <= distance0)
                    {
                        distance0 = Vector3.Distance(parentActor.Transform.Translation, opaquePrimitiveObject.Transform.Translation);
                        position0 = opaquePrimitiveObject.Transform.Translation;
                    }
                }

                foreach (CollidablePrimitiveObject transparentPrimitiveObject in this.ObjectManager.TransparentDrawList)
                {
                    if (Vector3.Distance(parentActor.Transform.Translation, transparentPrimitiveObject.Transform.Translation) <= distance0)
                    {
                        distance0 = Vector3.Distance(parentActor.Transform.Translation, transparentPrimitiveObject.Transform.Translation);
                        position0 = transparentPrimitiveObject.Transform.Translation;
                    }
                }

                foreach (CollidablePrimitiveObject opaquePrimitiveObject in this.ObjectManager.OpaqueDrawList)
                {
                    opaquePrimitiveObject.EffectParameters.DiffuseColor = opaquePrimitiveObject.EffectParameters.OriginalColor;
                    lookVector = parentActor.Transform.Look;

                    objectColor = opaquePrimitiveObject.EffectParameters.DiffuseColor.ToVector3();
                    updatedColor = ((opaquePrimitiveObject.Transform.Translation - position0) / this.BlockDimension);
                    updatedColor.X = Math.Abs(updatedColor.X);
                    updatedColor.Y = Math.Abs(updatedColor.Y);
                    updatedColor.Z = Math.Abs(updatedColor.Z);

                    lookVector.X = Math.Abs(lookVector.X);
                    lookVector.Y = Math.Abs(lookVector.Y);
                    lookVector.Z = Math.Abs(lookVector.Z);

                    updatedColor = updatedColor * lookVector;
                    objectColor -= colorVector * updatedColor;

                    if (objectColor.X <= 0) objectColor.X = 0;
                    if (objectColor.Y <= 0) objectColor.Y = 0;
                    if (objectColor.Z <= 0) objectColor.Z = 0;

                    opaquePrimitiveObject.EffectParameters.DiffuseColor = new Color(objectColor);
                }

                this.ColorUpdated = true;
            }
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            Actor3D parentActor = actor as Actor3D;
            CalculateColorBasedOnDistance(parentActor);

            if (StateManager.IsMoving) this.ColorUpdated = false;

            base.Update(gameTime, actor);
        }
        #endregion
    }
}
