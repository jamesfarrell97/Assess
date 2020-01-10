/*
Function: 		Parent class for all controllers which accept keyboard input and apply to an actor (e.g. a FirstPersonCameraController inherits from this class).
Author: 		NMCG
Version:		1.0
Date Updated:	
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class UserInputController : Controller
    {
        #region Fields
        private Keys[] moveKeys;
        private float moveSpeed;
        private float strafeSpeed;
        private float rotationSpeed;
        private readonly InputManagerParameters inputManagerParameters;
        #endregion

        #region Properties
        public InputManagerParameters InputManagerParameters
        {
            get
            {
                return this.inputManagerParameters;
            }
        }

        public Keys[] MoveKeys
        {
            get
            {
                return this.moveKeys;
            }
            set
            {
                this.moveKeys = value;
            }
        }

        public float MoveSpeed
        {
            get
            {
                return this.moveSpeed;
            }
            set
            {
                this.moveSpeed = value;
            }
        }

        public float StrafeSpeed
        {
            get
            {
                return this.strafeSpeed;
            }
            set
            {
                this.strafeSpeed = value;
            }
        }

        public float RotationSpeed
        {
            get
            {
                return this.rotationSpeed;
            }
            set
            {
                this.rotationSpeed = value;
            }
        }
        #endregion

        #region Constructors
        public UserInputController(
            string id,
            ControllerType controllerType, 
            Keys[] moveKeys,
            float moveSpeed, 
            float strafeSpeed, 
            float rotationSpeed, 
            InputManagerParameters inputManagerParameters
        ) : base(id, controllerType) {
            this.moveKeys = moveKeys;
            this.moveSpeed = moveSpeed;
            this.strafeSpeed = strafeSpeed;
            this.rotationSpeed = rotationSpeed;

            this.inputManagerParameters = inputManagerParameters;
        }

        public UserInputController(
            string id,
            ControllerType controllerType,
            Keys[] moveKeys,
            InputManagerParameters inputManagerParameters
        ) : base(id, controllerType) {
            this.moveKeys = moveKeys;
            this.inputManagerParameters = inputManagerParameters;
        }
        #endregion

        #region Methods
        public override void Update(GameTime gameTime, IActor actor)
        {
            Actor3D parentActor = actor as Actor3D;
            HandleMouseInput(gameTime, parentActor);
            HandleKeyboardInput(gameTime, parentActor);
            HandleGamePadInput(gameTime, parentActor);
            base.Update(gameTime, actor);
        }

        public virtual void HandleGamePadInput(GameTime gameTime, Actor3D parentActor)
        {
        }

        public virtual void HandleMouseInput(GameTime gameTime, Actor3D parentActor)
        {
        }

        public virtual void HandleKeyboardInput(GameTime gameTime, Actor3D parentActor)
        {
        }
        #endregion
    }
}