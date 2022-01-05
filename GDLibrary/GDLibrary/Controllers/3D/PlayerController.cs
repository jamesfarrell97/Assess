using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace GDLibrary
{
    public class PlayerController : UserInputController
    {
        public enum Direction
        {
            Left,
            Right
        }

        #region Fields
        Camera3D camera;
        ObjectManager objectManager;

        Vector3 targetFallBlock;
        Vector3 targetMoveBlock;
        Vector3 blockDimension;
        #endregion

        #region Properties
        public Camera3D Camera { get => camera; set => camera = value; }
        public ObjectManager ObjectManager { get => objectManager; set => objectManager = value; }

        public Vector3 TargetFallBlock { get => targetFallBlock; set => targetFallBlock = value; }
        public Vector3 TargetMoveBlock { get => targetMoveBlock; set => targetMoveBlock = value; }
        public Vector3 BlockDimension { get => blockDimension; set => blockDimension = value; }
        #endregion

        #region Constructors
        public PlayerController(
            string id,
            ControllerType controllerType,
            Keys[] moveKeys,
            InputManagerParameters inputManagerParameters,
            Camera3D camera,
            ObjectManager objectManager,
            Vector3 blockDimension
        ) : base (id, controllerType, moveKeys, inputManagerParameters) {
            this.Camera = camera;
            this.ObjectManager = objectManager;
            this.BlockDimension = blockDimension;
        }
        #endregion

        #region Methods
        public void CalculatePositionUpdate(GameTime gameTime, Actor3D parentActor)
        {
            if (!StateManager.IsCharacterMoving && !StateManager.IsCameraMoving && !StateManager.LevelClear && !StateManager.PlayerDied && StateManager.FinishedTracking)
            {
                //Store player position as collision poisition
                Transform3D collisionTransform = parentActor.Transform.Clone() as Transform3D;

                //Move collision to target location
                collisionTransform.Translation += (this.BlockDimension * -this.Camera.Transform.Up);

                //Update collision box
                (parentActor as CollidablePrimitiveObject).CollisionPrimitive.Update(gameTime, collisionTransform);

                //Check for collision
                Actor collidee = (parentActor as CollidablePrimitiveObject).CheckCollisions(gameTime);

                //If there have been no collisions with a block
                if (collidee == null || !collidee.GetID().Contains("Block"))
                {
                    //Set our target block equal to the updated collision position
                    this.TargetFallBlock = collisionTransform.Translation;
                }

                //If there is a collision with a block
                else
                {
                    //Set fall state to false
                    StateManager.IsFalling = false;

                    //Reset current goal in loop
                    StateManager.CurrentGoalInLoop = 0;

                    //Set collision position back to player position
                    collisionTransform.Translation = parentActor.Transform.Translation;

                    //Update collision
                    (parentActor as CollidablePrimitiveObject).CollisionPrimitive.Update(gameTime, collisionTransform);

                    //Set target fall block to 0
                    this.TargetFallBlock = Vector3.Zero;
                }
            }
        }

        public void HandlePositionUpdate(GameTime gameTime, Actor3D parentActor)
        {
            if (this.TargetFallBlock != Vector3.Zero)
            {
                if (Vector3.Distance(this.TargetFallBlock, parentActor.Transform.Translation) <= 0.001f)
                {
                    if (!StateManager.IsFalling)
                    {
                        EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "fall" }));
                        StateManager.IncompletePlayed = false;
                        StateManager.IsFalling = true;
                    }

                    parentActor.Transform.Translation = this.TargetFallBlock;
                    StateManager.IsCharacterMoving = false;
                }
                else
                {
                    parentActor.Transform.Translation += (this.BlockDimension * -this.Camera.Transform.Up) / 5;
                    StateManager.IsCharacterMoving = true;
                }
            }
        }

        public override void HandleKeyboardInput(GameTime gameTime, Actor3D parentActor)
        {
            //if (!StateManager.IsCharacterMoving)
            //{
            //    if (this.InputManagerParameters.KeyboardManager.IsFirstKeyPress(MoveKeys[0]))
            //    {
            //        this.TargetMoveBlock = parentActor.Transform.Translation + (this.BlockDimension * -this.Camera.Transform.Right);
            //        this.MoveIncrement = (this.BlockDimension * -this.Camera.Transform.Right) * (float)gameTime.ElapsedGameTime.Milliseconds / 1000;
            //    }

            //    else if (this.InputManagerParameters.KeyboardManager.IsFirstKeyPress(MoveKeys[1]))
            //    {
            //        this.TargetMoveBlock = parentActor.Transform.Translation + (this.BlockDimension * this.Camera.Transform.Right);
            //        this.MoveIncrement = (this.BlockDimension * this.Camera.Transform.Right) * (float)gameTime.ElapsedGameTime.Milliseconds / 1000;
            //    }
            //}
        }

        public void HandleMovement(GameTime gameTime, Actor3D parentActor)
        {
            //if (this.TargetMoveBlock != Vector3.Zero)
            //{
            //    Transform3D collisionTransform = parentActor.Transform.Clone() as Transform3D;

            //    collisionTransform.Translation = this.TargetMoveBlock;
            //    (parentActor as CollidablePrimitiveObject).CollisionPrimitive.Update(gameTime, collisionTransform);

            //    if (((parentActor as CollidablePrimitiveObject).CheckCollisions(gameTime) == null))
            //    {
            //        if (Vector3.Distance(this.TargetMoveBlock, parentActor.Transform.Translation) <= 0.001f)
            //        {
            //            parentActor.Transform.Translation = this.TargetMoveBlock;
            //            StateManager.IsCharacterMoving = false;
            //            this.TargetMoveBlock = Vector3.Zero;
            //        }
            //        else
            //        {
            //            parentActor.Transform.Translation += this.MoveIncrement;
            //            StateManager.IsCharacterMoving = true;
            //        }
            //    }
            //    else
            //    {
            //        collisionTransform.Translation = parentActor.Transform.Translation;
            //        (parentActor as CollidablePrimitiveObject).CollisionPrimitive.Update(gameTime, collisionTransform);

            //        StateManager.IsCharacterMoving = false;
            //        this.TargetMoveBlock = Vector3.Zero;
            //    }
            //}
        }

        public void CheckFallOffScreen(Actor3D parentActor)
        {
            if (this.Camera.Transform.Up.Equals(Vector3.UnitX))
            {
                if (parentActor.Transform.Translation.X <= -15)
                    StateManager.PlayerDied = true;
            }

            else if (this.Camera.Transform.Up.Equals(Vector3.UnitY))
            {
                if (parentActor.Transform.Translation.Y <= -15)
                    StateManager.PlayerDied = true;
            }

            else if (this.Camera.Transform.Up.Equals(Vector3.UnitZ))
            {
                if (parentActor.Transform.Translation.Z <= -15)
                    StateManager.PlayerDied = true;
            }

            else if (this.Camera.Transform.Up.Equals(-Vector3.UnitX))
            {
                if (parentActor.Transform.Translation.X >= 30)
                    StateManager.PlayerDied = true;
            }

            else if (this.Camera.Transform.Up.Equals(-Vector3.UnitY))
            {
                if (parentActor.Transform.Translation.Y >= 30)
                    StateManager.PlayerDied = true;
            }
                
            else if (this.Camera.Transform.Up.Equals(-Vector3.UnitZ))
            {
                if (parentActor.Transform.Translation.Z >= 30)
                    StateManager.PlayerDied = true;
            }
        }

        public void CheckCollision(GameTime gameTime, Actor3D parentActor)
        {
            //Check for collision
            Actor collidee = (parentActor as CollidablePrimitiveObject).CheckCollisions(gameTime);

            //IF there has been a collision
            if (collidee != null)
            {
                //If the collision has been with a goal
                if (collidee.GetID().Contains("Goal"))
                {
                    //Store an array of goal sounds
                    string[] goals = { "goal_001", "goal_002", "goal_003", "goal_004" };

                    //Create an audio emitter at this location
                    AudioEmitter audioEmitter = new AudioEmitter
                    {
                        Position = (collidee as Actor3D).Transform.Translation,
                        Forward = parentActor.Transform.Look
                    };

                    //Publish sound event
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound3D, new object[] { goals[StateManager.CurrentGoalInLoop], audioEmitter }));

                    //Remove goal from game
                    this.ObjectManager.Remove(collidee as Actor3D);

                    //Update goals remaining
                    StateManager.GoalsRemaining--;

                    //Update current goal
                    StateManager.CurrentGoalInLoop++;

                    //Update game
                    StateManager.GameUpdated = true;
                }

                //If there is a collision with an objective
                else if (collidee.GetID().Contains("Objective"))
                {
                    if (StateManager.GoalsRemaining <= 0)
                    {
                        //Publish sound event
                        EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "success" }));

                        //Remove self from game
                        this.ObjectManager.Remove(parentActor);

                        //Update game state
                        StateManager.LevelClear = true;

                        //Update game
                        StateManager.GameUpdated = true;
                    }

                    else if (!StateManager.IncompletePlayed)
                    {
                        //Publish sound event
                        EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "incomplete" }));

                        //Mark sound as played
                        StateManager.IncompletePlayed = true;
                    }
                }
            }
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            Actor3D parentActor = actor as Actor3D;
            HandleKeyboardInput(gameTime, parentActor);
            HandleMovement(gameTime, parentActor);

            CalculatePositionUpdate(gameTime, parentActor);
            HandlePositionUpdate(gameTime, parentActor);

            CheckCollision(gameTime, parentActor);
            CheckFallOffScreen(parentActor);

            base.Update(gameTime, actor);
        }
        #endregion
    }
}