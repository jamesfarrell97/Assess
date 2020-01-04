using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary
{
    public class PlayerController : UserInputController
    {
        #region Fields
        Camera3D camera;
        Vector3 blockDimension;
        Vector3 targetBlock;
        #endregion

        #region Properties
        public Camera3D Camera { get => camera; set => camera = value; }
        public Vector3 BlockDimension { get => blockDimension; set => blockDimension = value; }
        public Vector3 TargetBlock { get => targetBlock; set => targetBlock = value; }
        #endregion

        #region Constructors
        public PlayerController(
            string id,
            ControllerType controllerType,
            Keys[] moveKeys,
            InputManagerParameters inputManagerParameters,
            Vector3 blockDimension,
            Camera3D camera
        ) : base (id, controllerType, moveKeys, inputManagerParameters) {
            this.BlockDimension = blockDimension;
            this.Camera = camera;
        }
        #endregion

        #region Methods
        public override void HandleKeyboardInput(GameTime gameTime, Actor3D parentActor)
        {
            if (this.InputManagerParameters.KeyboardManager.IsFirstKeyPress(MoveKeys[0]))
            {

            }

            else if (this.InputManagerParameters.KeyboardManager.IsFirstKeyPress(MoveKeys[1]))
            {

            }

            else if (this.InputManagerParameters.KeyboardManager.IsFirstKeyPress(MoveKeys[2]))
            {

            }

            else if (this.InputManagerParameters.KeyboardManager.IsFirstKeyPress(MoveKeys[3]))
            {

            }

            base.HandleKeyboardInput(gameTime, parentActor);
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            Actor3D parentActor = actor as Actor3D;
            HandleKeyboardInput(gameTime, parentActor);
            HandleMovement(gameTime, parentActor);
            CalculateFall(gameTime, parentActor);
            Fall(gameTime, parentActor);
            base.Update(gameTime, actor);
        }

        public void CalculateFall(GameTime gameTime, Actor3D parentActor)
        {
            Transform3D collisionTransform = parentActor.Transform.Clone() as Transform3D;

            if (this.Camera != null && !StateManager.IsMoving && !StateManager.IsRotating && !StateManager.BlockMoved)
            {
                collisionTransform.Translation += (this.BlockDimension * -this.Camera.Transform.Up);
                (parentActor as CollidablePrimitiveObject).CollisionPrimitive.Update(gameTime, collisionTransform);

                if (((parentActor as CollidablePrimitiveObject).CheckCollisions(gameTime) == null))
                {
                    this.TargetBlock = collisionTransform.Translation;
                }

                else
                {
                    collisionTransform.Translation -= (this.BlockDimension * -this.Camera.Transform.Up);
                    (parentActor as CollidablePrimitiveObject).CollisionPrimitive.Update(gameTime, collisionTransform);
                }
            }
        }

        public void Fall(GameTime gameTime, Actor3D parentActor)
        {
            if (this.TargetBlock != Vector3.Zero)
            {
                if (Vector3.Distance(this.TargetBlock, parentActor.Transform.Translation) <= 0.001f)
                {
                    parentActor.Transform.Translation = this.TargetBlock;
                    StateManager.IsMoving = false;
                }
                else
                {
                    parentActor.Transform.Translation += (this.BlockDimension * -this.Camera.Transform.Up) / 5;
                    StateManager.IsMoving = true;
                }
            }
        }

        public void HandleMovement(GameTime gameTime, Actor3D parentActor)
        {

        }
        #endregion
    }
}