using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace GDLibrary
{
    public enum Direction
    {
        PosX,
        PosY,
        PosZ,
        NegX,
        NegY,
        NegZ
    }

    public class OrbitalCameraController : UserInputController
    {
        #region Fields
        private float initialProjectionWidth;
        private float initialProjectionHeight;

        private int minZoom;
        private int currentZoom;
        private int maxZoom;

        private Matrix rotationMatrix;
        private Vector3 rotationVector;

        private Vector3 targetPositionVector;
        private Vector3 targetLookVector;
        private Vector3 targetUpVector;

        private bool isRotating;
        private bool isMoving;
        #endregion

        #region Properties
        public float InitialProjectionHeight { get => initialProjectionHeight; set => initialProjectionHeight = value; }
        public float InitialProjectionWidth { get => initialProjectionWidth; set => initialProjectionWidth = value; }

        public int MinZoom { get => minZoom; set => minZoom = value; }
        public int CurrentZoom { get => currentZoom; set => currentZoom = value; }
        public int MaxZoom { get => maxZoom; set => maxZoom = value; }

        public Matrix RotationMatrix { get => rotationMatrix; set => rotationMatrix = value; }
        public Vector3 RotationVector { get => rotationVector; set => rotationVector = value; }

        public Vector3 TargetPositionVector { get => targetPositionVector; set => targetPositionVector = value; }
        public Vector3 TargetLookVector { get => targetLookVector; set => targetLookVector = value; }
        public Vector3 TargetUpVector { get => targetUpVector; set => targetUpVector = value; }

        public bool IsRotating { get => isRotating; set => isRotating = value; }
        public bool IsMoving { get => isMoving; set => isMoving = value; }
        #endregion

        #region Constructor
        public OrbitalCameraController(
            string id,
            ControllerType controllerType,
            float initialProjectionWidth,
            float initialProjectionHeight,
            Keys[] moveKeys,
            float moveSpeed,
            float strafeSpeed,
            float rotationSpeed,
            InputManagerParameters inputManagerParameters
        ) : base(id, controllerType, moveKeys, moveSpeed, strafeSpeed, rotationSpeed, inputManagerParameters) {
            this.InitialProjectionWidth = initialProjectionWidth;
            this.InitialProjectionHeight = initialProjectionHeight;

            this.MinZoom = 0;
            this.MaxZoom = 15;

            StateManager.IsRotating = false;
            StateManager.IsMoving = false;
        }
        #endregion

        #region Methods
        public override void HandleKeyboardInput(GameTime gameTime, Actor3D parentActor)
        {
            //If not already in motion
            if (!StateManager.IsRotating && !StateManager.IsMoving)
            {
                //Orbit counter clockwise about up vector
                if (this.InputManagerParameters.KeyboardManager.IsFirstKeyPress(this.MoveKeys[0]))
                {
                    this.OrbitCounterClockwiseAboutUpVector(gameTime, parentActor);
                }

                //Orbit clockwise about up vector
                else if (this.InputManagerParameters.KeyboardManager.IsFirstKeyPress(this.MoveKeys[1]))
                {
                    this.OrbitClockwiseAboutUpVector(gameTime, parentActor);
                }

                //Orbit counter clockwise about right vector
                else if (this.InputManagerParameters.KeyboardManager.IsFirstKeyPress(this.MoveKeys[2]))
                {
                    this.OrbitCounterClockwiseAboutRightVector(gameTime, parentActor);
                }

                //Orbit clockwise about right vector
                else if (this.InputManagerParameters.KeyboardManager.IsFirstKeyPress(this.MoveKeys[3]))
                {
                    this.OrbitClockwiseAboutRightVector(gameTime, parentActor);
                }

                //Rotate clockwise about look vector
                else if (this.InputManagerParameters.KeyboardManager.IsFirstKeyPress(this.MoveKeys[4]))
                {
                    this.RotateCounterClockwiseAboutLookVector(gameTime, parentActor);
                }

                //Rotate counter clockwise about look vector
                else if (this.InputManagerParameters.KeyboardManager.IsFirstKeyPress(this.MoveKeys[5]))
                {
                    this.RotateClockwiseAboutLookVector(gameTime, parentActor);
                }

                //Switch to orthographic view
                else if (this.InputManagerParameters.KeyboardManager.IsFirstKeyPress(this.MoveKeys[6]))
                {
                    StateManager.IsPerspective = false;
                    (parentActor as OrbitalCamera).ProjectionParameters.IsDirty = true;
                }

                //Switch to perspective view
                else if (this.InputManagerParameters.KeyboardManager.IsFirstKeyPress(this.MoveKeys[7]))
                {
                    StateManager.IsPerspective = true;
                    (parentActor as OrbitalCamera).ProjectionParameters.IsDirty = true;
                }
            }
        }

        public override void HandleMouseInput(GameTime gameTime, Actor3D parentActor)
        {
            //If not already in motion
            if (!StateManager.IsRotating && !StateManager.IsMoving)
            {
                //Store scroll wheel delta
                int zoomDelta = -(this.InputManagerParameters.MouseManager.GetDeltaFromScrollWheel() / 120);

                //If zoom set, and if zoom within range
                if (zoomDelta != 0 && (this.CurrentZoom + zoomDelta >= this.MinZoom) && (this.CurrentZoom + zoomDelta <= this.MaxZoom))
                {
                    //Update current zoom
                    this.CurrentZoom = (this.CurrentZoom + zoomDelta);

                    //Apply zoom
                    (parentActor as OrbitalCamera).ProjectionParameters.Width += zoomDelta * InitialProjectionWidth / 10;
                    (parentActor as OrbitalCamera).ProjectionParameters.Height += zoomDelta * InitialProjectionHeight / 10;
                }
            }
        }

        public void HandleMovement(GameTime gameTime, Actor3D parentActor)
        {
            //Look & Up vectors
            if (this.TargetLookVector != Vector3.Zero && this.TargetUpVector != Vector3.Zero)
            {
                UpdateTranslationAndLookVectors(gameTime, parentActor);
                UpdateUpVector(gameTime, parentActor);
            }

            //Look vector
            else if (this.TargetLookVector != Vector3.Zero)
            {
                UpdateTranslationAndLookVectors(gameTime, parentActor);
            }

            //Up vector
            else if (this.TargetUpVector != Vector3.Zero)
            {
                UpdateUpVector(gameTime, parentActor);
            }
        }

        #region Calculate Transformations
        public void OrbitCounterClockwiseAboutUpVector(GameTime gameTime, Actor3D parentActor)
        {
            //Orbit counter-clockwise, relative to the +X axis, about the X axis
            if (parentActor.Transform.Up.Equals(Vector3.UnitX))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.NegX, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.NegX, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.NegX, parentActor);
            }

            //Orbit counter-clockwise, relative to the +Y axis, about the Y axis
            else if (parentActor.Transform.Up.Equals(Vector3.UnitY))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.NegY, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.NegY, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.NegY, parentActor);
            }

            //Orbit counter-clockwise, relative to the +Z axis, about the Z axis
            else if (parentActor.Transform.Up.Equals(Vector3.UnitZ))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.NegZ, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.NegZ, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.NegZ, parentActor);
            }

            //Orbit counter-clockwise, relative to the -X axis, about the X axis
            else if (parentActor.Transform.Up.Equals(-Vector3.UnitX))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.PosX, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.PosX, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.PosX, parentActor);
            }

            //Orbit counter-clockwise, relative to the -Y axis, about the Y axis
            else if (parentActor.Transform.Up.Equals(-Vector3.UnitY))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.PosY, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.PosY, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.PosY, parentActor);
            }

            //Orbit counter-clockwise, relative to the -Z axis, about the Z axis
            else if (parentActor.Transform.Up.Equals(-Vector3.UnitZ))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.PosZ, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.PosZ, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.PosZ, parentActor);
            }
        }

        //Orbit clockwise, about the up vector
        public void OrbitClockwiseAboutUpVector(GameTime gameTime, Actor3D parentActor)
        {
            //Orbit clockwise, relative to the +X axis, about the X axis
            if (parentActor.Transform.Up.Equals(Vector3.UnitX))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.PosX, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.PosX, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.PosX, parentActor);
            }

            //Orbit clockwise, relative to the +Y axis, about the Y axis
            else if (parentActor.Transform.Up.Equals(Vector3.UnitY))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.PosY, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.PosY, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.PosY, parentActor);
            }

            //Orbit clockwise, relative to the +Z axis, about the Z axis
            else if (parentActor.Transform.Up.Equals(Vector3.UnitZ))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.PosZ, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.PosZ, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.PosZ, parentActor);
            }

            //Orbit clockwise, relative to the -X axis, about the X axis
            else if (parentActor.Transform.Up.Equals(-Vector3.UnitX))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.NegX, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.NegX, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.NegX, parentActor);
            }

            //Orbit clockwise, relative to the -Y axis, about the Y axis
            else if (parentActor.Transform.Up.Equals(-Vector3.UnitY))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.NegY, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.NegY, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.NegY, parentActor);
            }

            //Orbit clockwise, relative to the -Z axis, about the Z axis
            else if (parentActor.Transform.Up.Equals(-Vector3.UnitZ))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.NegZ, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.NegZ, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.NegZ, parentActor);
            }
        }

        //Orbit counter-clockwise, about the right vector
        public void OrbitCounterClockwiseAboutRightVector(GameTime gameTime, Actor3D parentActor)
        {
            //Orbit counter-clockwise, relative to the +X axis, about the X axis
            if (parentActor.Transform.Right.Equals(Vector3.UnitX))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.NegX, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.NegX, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.NegX, parentActor);
                this.TargetUpVector = CalculateTargetUpVector(Direction.NegX, parentActor);
            }

            //Orbit counter-clockwise, relative to the +Y axis, about the Y axis
            else if (parentActor.Transform.Right.Equals(Vector3.UnitY))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.NegY, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.NegY, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.NegY, parentActor);
                this.TargetUpVector = CalculateTargetUpVector(Direction.NegY, parentActor);
            }

            //Orbit counter-clockwise, relative to the +Z axis, about the Z axis
            else if (parentActor.Transform.Right.Equals(Vector3.UnitZ))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.NegZ, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.NegZ, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.NegZ, parentActor);
                this.TargetUpVector = CalculateTargetUpVector(Direction.NegZ, parentActor);
            }

            //Orbit counter-clockwise, relative to the -X axis, about the X axis
            else if (parentActor.Transform.Right.Equals(-Vector3.UnitX))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.PosX, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.PosX, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.PosX, parentActor);
                this.TargetUpVector = CalculateTargetUpVector(Direction.PosX, parentActor);
            }

            //Orbit counter-clockwise, relative to the -Y axis, about the Y axis
            else if (parentActor.Transform.Right.Equals(-Vector3.UnitY))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.PosY, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.PosY, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.PosY, parentActor);
                this.TargetUpVector = CalculateTargetUpVector(Direction.PosY, parentActor);
            }

            //Orbit counter-clockwise, relative to the -Z axis, about the Z axis
            else if (parentActor.Transform.Right.Equals(-Vector3.UnitZ))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.PosZ, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.PosZ, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.PosZ, parentActor);
                this.TargetUpVector = CalculateTargetUpVector(Direction.PosZ, parentActor);
            }
        }

        //Orbit down, about the right vector
        public void OrbitClockwiseAboutRightVector(GameTime gameTime, Actor3D parentActor)
        {
            //Orbit about X
            if (parentActor.Transform.Right.Equals(Vector3.UnitX))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.PosX, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.PosX, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.PosX, parentActor);
                this.TargetUpVector = CalculateTargetUpVector(Direction.PosX, parentActor);
            }

            //Orbit about Y
            else if (parentActor.Transform.Right.Equals(Vector3.UnitY))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.PosY, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.PosY, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.PosY, parentActor);
                this.TargetUpVector = CalculateTargetUpVector(Direction.PosY, parentActor);
            }

            //Orbit about Z
            else if (parentActor.Transform.Right.Equals(Vector3.UnitZ))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.PosZ, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.PosZ, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.PosZ, parentActor);
                this.TargetUpVector = CalculateTargetUpVector(Direction.PosZ, parentActor);
            }

            //Orbit about -X
            else if (parentActor.Transform.Right.Equals(-Vector3.UnitX))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.NegX, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.NegX, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.NegX, parentActor);
                this.TargetUpVector = CalculateTargetUpVector(Direction.NegX, parentActor);
            }

            //Orbit about -Y
            else if (parentActor.Transform.Right.Equals(-Vector3.UnitY))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.NegY, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.NegY, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.NegY, parentActor);
                this.TargetUpVector = CalculateTargetUpVector(Direction.NegY, parentActor);
            }

            //Orbit about -Z
            else if (parentActor.Transform.Right.Equals(-Vector3.UnitZ))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.NegZ, gameTime);
                this.TargetPositionVector = CalculateTargetPositionVector(Direction.NegZ, parentActor);
                this.TargetLookVector = CalculateTargetLookVector(Direction.NegZ, parentActor);
                this.TargetUpVector = CalculateTargetUpVector(Direction.NegZ, parentActor);
            }
        }

        //Rotate clockwise about the look vector
        public void RotateClockwiseAboutLookVector(GameTime gameTime, Actor3D parentActor)
        {
            //Rotate about +X
            if (parentActor.Transform.Look.Equals(Vector3.UnitX))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.PosX, gameTime);
                this.TargetUpVector = CalculateTargetUpVector(Direction.PosX, parentActor);
            }

            //Rotate about +Y
            else if (parentActor.Transform.Look.Equals(Vector3.UnitY))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.PosY, gameTime);
                this.TargetUpVector = CalculateTargetUpVector(Direction.PosY, parentActor);
            }

            //Rotate about +Z
            else if (parentActor.Transform.Look.Equals(Vector3.UnitZ))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.PosZ, gameTime);
                this.TargetUpVector = CalculateTargetUpVector(Direction.PosZ, parentActor);
            }

            //Rotate about -X
            else if (parentActor.Transform.Look.Equals(-Vector3.UnitX))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.NegX, gameTime);
                this.TargetUpVector = CalculateTargetUpVector(Direction.NegX, parentActor);
            }

            //Rotate about -Y
            else if (parentActor.Transform.Look.Equals(-Vector3.UnitY))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.NegY, gameTime);
                this.TargetUpVector = CalculateTargetUpVector(Direction.NegY, parentActor);
            }

            //Rotate about -Z
            else if (parentActor.Transform.Look.Equals(-Vector3.UnitZ))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.NegZ, gameTime);
                this.TargetUpVector = CalculateTargetUpVector(Direction.NegZ, parentActor);
            }
        }

        //Rotate counter-clockwise about the look vector
        public void RotateCounterClockwiseAboutLookVector(GameTime gameTime, Actor3D parentActor)
        {
            //Rotate about +X
            if (parentActor.Transform.Look.Equals(Vector3.UnitX))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.NegX, gameTime);
                this.TargetUpVector = CalculateTargetUpVector(Direction.NegX, parentActor);
            }

            //Rotate about +Y
            else if (parentActor.Transform.Look.Equals(Vector3.UnitY))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.NegY, gameTime);
                this.TargetUpVector = CalculateTargetUpVector(Direction.NegY, parentActor);
            }

            //Rotate about +Z
            else if (parentActor.Transform.Look.Equals(Vector3.UnitZ))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.NegZ, gameTime);
                this.TargetUpVector = CalculateTargetUpVector(Direction.NegZ, parentActor);
            }

            //Rotate about -X
            else if (parentActor.Transform.Look.Equals(-Vector3.UnitX))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.PosX, gameTime);
                this.TargetUpVector = CalculateTargetUpVector(Direction.PosX, parentActor);
            }

            //Rotate about -Y
            else if (parentActor.Transform.Look.Equals(-Vector3.UnitY))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.PosY, gameTime);
                this.TargetUpVector = CalculateTargetUpVector(Direction.PosY, parentActor);
            }

            //Rotate about -Z
            else if (parentActor.Transform.Look.Equals(-Vector3.UnitZ))
            {
                this.RotationMatrix = CalculateRotationMatrix(Direction.PosZ, gameTime);
                this.TargetUpVector = CalculateTargetUpVector(Direction.PosZ, parentActor);
            }
        }
        #endregion

        #region Apply Transformations
        public void UpdateUpVector(GameTime gameTime, Actor3D parentActor)
        {
            //Round out
            this.TargetUpVector = new Vector3(
                (int) Math.Round(this.TargetUpVector.X, 0),
                (int) Math.Round(this.TargetUpVector.Y, 0),
                (int) Math.Round(this.TargetUpVector.Z, 0)
            );

            //If the current vector is near the target vector
            if (Vector3.Distance(parentActor.Transform.Up, this.TargetUpVector) <= 0.05f)
            {
                //Update up vector
                parentActor.Transform.Up = this.TargetUpVector;

                //Reset target vector
                this.TargetUpVector = Vector3.Zero;

                //Update motion state
                StateManager.IsRotating = false;
            }
            else
            {
                //Update up vector
                parentActor.Transform.Up = Vector3.Transform(parentActor.Transform.Up, this.RotationMatrix);

                //Update motion state
                StateManager.IsRotating = true;
            }
        }

        public void UpdateTranslationAndLookVectors(GameTime gameTime, Actor3D parentActor)
        {
            //Round out
            this.TargetPositionVector = new Vector3(
                (float) Math.Round(this.TargetPositionVector.X, 2),
                (float) Math.Round(this.TargetPositionVector.Y, 2),
                (float) Math.Round(this.TargetPositionVector.Z, 2)
            );

            //Round out
            this.TargetLookVector = new Vector3(
                (int) Math.Round(this.TargetLookVector.X, 0),
                (int) Math.Round(this.TargetLookVector.Y, 0),
                (int) Math.Round(this.TargetLookVector.Z, 0)
            );

            //If the current vector is near the target vector (magnified by the current zoom)
            if (Vector3.Distance(parentActor.Transform.Translation, this.TargetPositionVector) <= Math.Abs(0.2f + (this.CurrentZoom * 0.1f)))
            {
                //Update translation vector
                parentActor.Transform.Translation = this.TargetPositionVector;

                //Update look vector
                parentActor.Transform.Look = this.TargetLookVector;

                //Reset target vectors
                this.TargetPositionVector = Vector3.Zero;
                this.TargetLookVector = Vector3.Zero;

                //Update motion state
                StateManager.IsMoving = false;
            }
            else
            {
                //Translate to origin
                parentActor.Transform.Translation -= (parentActor as OrbitalCamera).OrbitPoint;

                //Rotate by rotation matrix
                parentActor.Transform.Translation = Vector3.Transform(parentActor.Transform.Translation, this.RotationMatrix);

                //Update look
                parentActor.Transform.Look = -Vector3.Normalize(parentActor.Transform.Translation);

                //Translate to orbit point
                parentActor.Transform.Translation += (parentActor as OrbitalCamera).OrbitPoint;

                //Update motion state
                StateManager.IsMoving = true;
            }
        }
        #endregion

        #region Utility Methods
        private Matrix CalculateRotationMatrix(Direction axis, GameTime gameTime)
        {
            switch (axis)
            {
                case Direction.PosX:
                    return Matrix.CreateRotationX(MathHelper.ToRadians(0.1f) * (float)gameTime.ElapsedGameTime.Milliseconds);

                case Direction.PosY:
                    return Matrix.CreateRotationY(MathHelper.ToRadians(0.1f) * (float)gameTime.ElapsedGameTime.Milliseconds);

                case Direction.PosZ:
                    return Matrix.CreateRotationZ(MathHelper.ToRadians(0.1f) * (float)gameTime.ElapsedGameTime.Milliseconds);

                case Direction.NegX:
                    return Matrix.CreateRotationX(MathHelper.ToRadians(-0.1f) * (float)gameTime.ElapsedGameTime.Milliseconds);

                case Direction.NegY:
                    return Matrix.CreateRotationY(MathHelper.ToRadians(-0.1f) * (float)gameTime.ElapsedGameTime.Milliseconds);

                case Direction.NegZ:
                    return Matrix.CreateRotationZ(MathHelper.ToRadians(-0.1f) * (float)gameTime.ElapsedGameTime.Milliseconds);
            }

            return Matrix.Identity;
        }

        //Translates vector to origin, rotates vector by 90 degrees, translates vector back to orbit point
        private Vector3 CalculateTargetPositionVector(Direction axis, Actor3D parentActor)
        {
            switch (axis)
            {
                case Direction.PosX:
                    return Vector3.Transform((parentActor.Transform.Translation - (parentActor as OrbitalCamera).OrbitPoint), Matrix.CreateRotationX(MathHelper.ToRadians(90))) + (parentActor as OrbitalCamera).OrbitPoint;

                case Direction.PosY:
                    return Vector3.Transform((parentActor.Transform.Translation - (parentActor as OrbitalCamera).OrbitPoint), Matrix.CreateRotationY(MathHelper.ToRadians(90))) + (parentActor as OrbitalCamera).OrbitPoint;

                case Direction.PosZ:
                    return Vector3.Transform((parentActor.Transform.Translation - (parentActor as OrbitalCamera).OrbitPoint), Matrix.CreateRotationZ(MathHelper.ToRadians(90))) + (parentActor as OrbitalCamera).OrbitPoint;

                case Direction.NegX:
                    return Vector3.Transform((parentActor.Transform.Translation - (parentActor as OrbitalCamera).OrbitPoint), Matrix.CreateRotationX(MathHelper.ToRadians(-90))) + (parentActor as OrbitalCamera).OrbitPoint;

                case Direction.NegY:
                    return Vector3.Transform((parentActor.Transform.Translation - (parentActor as OrbitalCamera).OrbitPoint), Matrix.CreateRotationY(MathHelper.ToRadians(-90))) + (parentActor as OrbitalCamera).OrbitPoint;

                case Direction.NegZ:
                    return Vector3.Transform((parentActor.Transform.Translation - (parentActor as OrbitalCamera).OrbitPoint), Matrix.CreateRotationZ(MathHelper.ToRadians(-90))) + (parentActor as OrbitalCamera).OrbitPoint;
            }

            return Vector3.Zero;
        }

        private Vector3 CalculateTargetLookVector(Direction axis, Actor3D parentActor)
        {
            switch(axis)
            {
                case Direction.PosX:
                    return Vector3.Transform(parentActor.Transform.Look, Matrix.CreateRotationX(MathHelper.ToRadians(90)));

                case Direction.PosY:
                    return Vector3.Transform(parentActor.Transform.Look, Matrix.CreateRotationY(MathHelper.ToRadians(90)));

                case Direction.PosZ:
                    return Vector3.Transform(parentActor.Transform.Look, Matrix.CreateRotationZ(MathHelper.ToRadians(90)));

                case Direction.NegX:
                    return Vector3.Transform(parentActor.Transform.Look, Matrix.CreateRotationX(MathHelper.ToRadians(-90)));

                case Direction.NegY:
                    return Vector3.Transform(parentActor.Transform.Look, Matrix.CreateRotationY(MathHelper.ToRadians(-90)));

                case Direction.NegZ:
                    return Vector3.Transform(parentActor.Transform.Look, Matrix.CreateRotationZ(MathHelper.ToRadians(-90)));
            }

            return Vector3.Zero;
        }

        private Vector3 CalculateTargetUpVector(Direction axis, Actor3D parentActor)
        {
            switch (axis)
            {
                case Direction.PosX:
                    return Vector3.Transform(parentActor.Transform.Up, Matrix.CreateRotationX(MathHelper.ToRadians(90)));

                case Direction.PosY:
                    return Vector3.Transform(parentActor.Transform.Up, Matrix.CreateRotationY(MathHelper.ToRadians(90)));

                case Direction.PosZ:
                    return Vector3.Transform(parentActor.Transform.Up, Matrix.CreateRotationZ(MathHelper.ToRadians(90)));

                case Direction.NegX:
                    return Vector3.Transform(parentActor.Transform.Up, Matrix.CreateRotationX(MathHelper.ToRadians(-90)));

                case Direction.NegY:
                    return Vector3.Transform(parentActor.Transform.Up, Matrix.CreateRotationY(MathHelper.ToRadians(-90)));

                case Direction.NegZ:
                    return Vector3.Transform(parentActor.Transform.Up, Matrix.CreateRotationZ(MathHelper.ToRadians(-90)));
            }

            return Vector3.Zero;
        }
        #endregion

        public override void Update(GameTime gameTime, IActor actor)
        {
            Actor3D parentActor = actor as Actor3D;
            this.HandleKeyboardInput(gameTime, parentActor);
            this.HandleMouseInput(gameTime, parentActor);
            this.HandleMovement(gameTime, parentActor);
        }
        #endregion
    }
}