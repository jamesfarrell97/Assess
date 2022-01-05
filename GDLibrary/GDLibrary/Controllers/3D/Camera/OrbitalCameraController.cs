using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace GDLibrary
{
    public class OrbitalCameraController : UserInputController
    {
        #region Properties
        public float InitialProjectionHeight { get; set; }
        public float InitialProjectionWidth { get; set; }
        public SoundManager SoundManager { get; set; }

        public int MinZoom { get; set; }
        public int CurrentZoom { get; set; }
        public int MaxZoom { get; set; }

        public Matrix RotationMatrix { get; set; }
        public Vector3 RotationVector { get; set; }

        public Vector3 TargetPositionVector { get; set; }
        public Vector3 TargetLookVector { get; set; }
        public Vector3 TargetUpVector { get; set; }

        public bool IsRotating { get; set; }
        public bool IsMoving { get; set; }

        public float OrbitAngle { get; set; }
        public float OrbitSpeed { get; set; }
        #endregion

        #region Constructor
        public OrbitalCameraController(
            string id,
            ControllerType controllerType,
            float initialProjectionWidth,
            float initialProjectionHeight,
            float orbitSpeed,
            float orbitAngle,
            Keys[] moveKeys,
            InputManagerParameters inputManagerParameters,
            SoundManager soundManager
        ) : base(id, controllerType, moveKeys, inputManagerParameters) {
            this.InitialProjectionWidth = initialProjectionWidth;
            this.InitialProjectionHeight = initialProjectionHeight;
            this.OrbitSpeed = orbitSpeed;
            this.OrbitAngle = orbitAngle;

            this.SoundManager = soundManager;
            this.MinZoom = 0;
            this.MaxZoom = 15;

            StateManager.IsCameraMoving = false;
            StateManager.IsCharacterMoving = false;
        }
        #endregion

        #region Methods
        public override void HandleKeyboardInput(GameTime gameTime, Actor3D parentActor)
        {
            //If not already in motion
            if (!StateManager.IsCameraMoving && !StateManager.IsCharacterMoving && StateManager.FinishedTracking)
            {
                //Cast to orbital camera
                OrbitalCamera orbitalCamera = parentActor as OrbitalCamera;

                //Orbit clockwise about up vector
                if (this.InputManagerParameters.KeyboardManager.IsFirstKeyPress(this.MoveKeys[0]))
                {
                    this.OrbitCounterClockwiseAboutUpVector(gameTime, orbitalCamera);
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "orbit" }));
                }

                //Orbit counter-clockwise about up vector
                else if (this.InputManagerParameters.KeyboardManager.IsFirstKeyPress(this.MoveKeys[1]))
                {
                    this.OrbitClockwiseAboutUpVector(gameTime, orbitalCamera);
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "orbit" }));
                }

                //Orbit clockwise about right vector
                else if (this.InputManagerParameters.KeyboardManager.IsFirstKeyPress(this.MoveKeys[2]))
                {
                    this.OrbitCounterClockwiseAboutRightVector(gameTime, orbitalCamera);
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "orbit" }));
                }

                //Orbit counter-clockwise about right vector
                else if (this.InputManagerParameters.KeyboardManager.IsFirstKeyPress(this.MoveKeys[3]))
                {
                    this.OrbitClockwiseAboutRightVector(gameTime, orbitalCamera);
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "orbit" }));
                }

                //Rotate clockwise about look vector
                else if (this.InputManagerParameters.KeyboardManager.IsFirstKeyPress(this.MoveKeys[4]))
                {
                    this.RotateClockwiseAboutLookVector(gameTime, orbitalCamera);
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "orbit" }));
                }

                //Rotate counter clockwise about look vector
                else if (this.InputManagerParameters.KeyboardManager.IsFirstKeyPress(this.MoveKeys[5]))
                {
                    this.RotateCounterClockwiseAboutLookVector(gameTime, orbitalCamera);
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "orbit" }));
                }

                //Switch to orthographic view
                //else if (this.InputManagerParameters.KeyboardManager.IsFirstKeyPress(this.MoveKeys[6]))
                //{
                //    StateManager.IsOrthographic = false;
                //    parentActor.ProjectionParameters.IsDirty = true;
                //    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "move" }));
                //}

                //Switch to perspective view
                //else if (this.InputManagerParameters.KeyboardManager.IsFirstKeyPress(this.MoveKeys[7]))
                //{
                //    StateManager.IsOrthographic = true;
                //    parentActor.ProjectionParameters.IsDirty = true;
                //    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "move" }));
                //}
            }
        }

        public override void HandleMouseInput(GameTime gameTime, Actor3D parentActor)
        {
            //If not already in motion
            if (!StateManager.IsCameraMoving && !StateManager.IsCharacterMoving)
            {
                //Store scroll wheel delta
                int zoomDelta = -(this.InputManagerParameters.MouseManager.GetDeltaFromScrollWheel() / 120);

                //If zoom set, and if zoom within min/max range
                if (zoomDelta != 0 && (this.CurrentZoom + zoomDelta >= this.MinZoom) && (this.CurrentZoom + zoomDelta <= this.MaxZoom))
                {
                    //Update current zoom
                    this.CurrentZoom = (this.CurrentZoom + zoomDelta);

                    //Apply zoom - orthogrpahic
                    (parentActor as OrbitalCamera).ProjectionParameters.Width += (zoomDelta * InitialProjectionWidth) / 12;
                    (parentActor as OrbitalCamera).ProjectionParameters.Height += (zoomDelta * InitialProjectionHeight) / 12;

                    //Apply zoom - perspective
                    if (parentActor.Transform.Look.Equals(Vector3.UnitX))
                    {
                        parentActor.Transform.Translation += (zoomDelta * -Vector3.UnitX);
                    }

                    else if (parentActor.Transform.Look.Equals(Vector3.UnitY))
                    {
                        parentActor.Transform.Translation += (zoomDelta * -Vector3.UnitY);
                    }

                    else if (parentActor.Transform.Look.Equals(Vector3.UnitZ))
                    {
                        parentActor.Transform.Translation += (zoomDelta * -Vector3.UnitZ);
                    }

                    else if (parentActor.Transform.Look.Equals(-Vector3.UnitX))
                    {
                        parentActor.Transform.Translation += (zoomDelta * Vector3.UnitX);
                    }

                    else if (parentActor.Transform.Look.Equals(-Vector3.UnitY))
                    {
                        parentActor.Transform.Translation += (zoomDelta * Vector3.UnitY);
                    }

                    else if (parentActor.Transform.Look.Equals(-Vector3.UnitZ))
                    {
                        parentActor.Transform.Translation += (zoomDelta * Vector3.UnitZ);
                    }
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
        //Orbit counter-clockwise, about the up vector
        public void OrbitCounterClockwiseAboutUpVector(GameTime gameTime, OrbitalCamera parentActor)
        {
            //Orbit counter-clockwise, relative to the +X axis, about the X axis
            if (parentActor.Transform.Up.Equals(Vector3.UnitX))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.NegX, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.NegX, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.NegX, this.OrbitAngle, parentActor.Transform.Look);
            }

            //Orbit counter-clockwise, relative to the +Y axis, about the Y axis
            else if (parentActor.Transform.Up.Equals(Vector3.UnitY))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.NegY, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.NegY, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.NegY, this.OrbitAngle, parentActor.Transform.Look);
            }

            //Orbit counter-clockwise, relative to the +Z axis, about the Z axis
            else if (parentActor.Transform.Up.Equals(Vector3.UnitZ))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.NegZ, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.NegZ, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.NegZ, this.OrbitAngle, parentActor.Transform.Look);
            }

            //Orbit counter-clockwise, relative to the -X axis, about the X axis
            else if (parentActor.Transform.Up.Equals(-Vector3.UnitX))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.PosX, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.PosX, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.PosX, this.OrbitAngle, parentActor.Transform.Look);
            }

            //Orbit counter-clockwise, relative to the -Y axis, about the Y axis
            else if (parentActor.Transform.Up.Equals(-Vector3.UnitY))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.PosY, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.PosY, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.PosY, this.OrbitAngle, parentActor.Transform.Look);
            }

            //Orbit counter-clockwise, relative to the -Z axis, about the Z axis
            else if (parentActor.Transform.Up.Equals(-Vector3.UnitZ))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.PosZ, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.PosZ, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.PosZ, this.OrbitAngle, parentActor.Transform.Look);
            }
        }

        //Orbit clockwise, about the up vector
        public void OrbitClockwiseAboutUpVector(GameTime gameTime, OrbitalCamera parentActor)
        {
            //Orbit clockwise, relative to the +X axis, about the X axis
            if (parentActor.Transform.Up.Equals(Vector3.UnitX))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.PosX, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.PosX, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.PosX, this.OrbitAngle, parentActor.Transform.Look);
            }

            //Orbit clockwise, relative to the +Y axis, about the Y axis
            else if (parentActor.Transform.Up.Equals(Vector3.UnitY))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.PosY, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.PosY, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.PosY, this.OrbitAngle, parentActor.Transform.Look);
            }

            //Orbit clockwise, relative to the +Z axis, about the Z axis
            else if (parentActor.Transform.Up.Equals(Vector3.UnitZ))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.PosZ, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.PosZ, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.PosZ, this.OrbitAngle, parentActor.Transform.Look);
            }

            //Orbit clockwise, relative to the -X axis, about the X axis
            else if (parentActor.Transform.Up.Equals(-Vector3.UnitX))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.NegX, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.NegX, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.NegX, this.OrbitAngle, parentActor.Transform.Look);
            }

            //Orbit clockwise, relative to the -Y axis, about the Y axis
            else if (parentActor.Transform.Up.Equals(-Vector3.UnitY))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.NegY, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.NegY, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.NegY, this.OrbitAngle, parentActor.Transform.Look);
            }

            //Orbit clockwise, relative to the -Z axis, about the Z axis
            else if (parentActor.Transform.Up.Equals(-Vector3.UnitZ))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.NegZ, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.NegZ, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.NegZ, this.OrbitAngle, parentActor.Transform.Look);
            }
        }

        //Orbit counter-clockwise, about the right vector
        public void OrbitCounterClockwiseAboutRightVector(GameTime gameTime, OrbitalCamera parentActor)
        {
            //Orbit counter-clockwise, relative to the +X axis, about the X axis
            if (parentActor.Transform.Right.Equals(Vector3.UnitX))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.NegX, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.NegX, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.NegX, this.OrbitAngle, parentActor.Transform.Look);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.NegX, this.OrbitAngle, parentActor.Transform.Up);
            }

            //Orbit counter-clockwise, relative to the +Y axis, about the Y axis
            else if (parentActor.Transform.Right.Equals(Vector3.UnitY))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.NegY, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.NegY, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.NegY, this.OrbitAngle, parentActor.Transform.Look);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.NegY, this.OrbitAngle, parentActor.Transform.Up);
            }

            //Orbit counter-clockwise, relative to the +Z axis, about the Z axis
            else if (parentActor.Transform.Right.Equals(Vector3.UnitZ))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.NegZ, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.NegZ, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.NegZ, this.OrbitAngle, parentActor.Transform.Look);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.NegZ, this.OrbitAngle, parentActor.Transform.Up);
            }

            //Orbit counter-clockwise, relative to the -X axis, about the X axis
            else if (parentActor.Transform.Right.Equals(-Vector3.UnitX))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.PosX, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.PosX, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.PosX, this.OrbitAngle, parentActor.Transform.Look);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.PosX, this.OrbitAngle, parentActor.Transform.Up);
            }

            //Orbit counter-clockwise, relative to the -Y axis, about the Y axis
            else if (parentActor.Transform.Right.Equals(-Vector3.UnitY))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.PosY, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.PosY, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.PosY, this.OrbitAngle, parentActor.Transform.Look);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.PosY, this.OrbitAngle, parentActor.Transform.Up);
            }

            //Orbit counter-clockwise, relative to the -Z axis, about the Z axis
            else if (parentActor.Transform.Right.Equals(-Vector3.UnitZ))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.PosZ, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.PosZ, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.PosZ, this.OrbitAngle, parentActor.Transform.Look);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.PosZ, this.OrbitAngle, parentActor.Transform.Up);
            }
        }

        //Orbit down, about the right vector
        public void OrbitClockwiseAboutRightVector(GameTime gameTime, OrbitalCamera parentActor)
        {
            //Orbit about X
            if (parentActor.Transform.Right.Equals(Vector3.UnitX))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.PosX, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.PosX, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.PosX, this.OrbitAngle, parentActor.Transform.Look);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.PosX, this.OrbitAngle, parentActor.Transform.Up);
            }

            //Orbit about Y
            else if (parentActor.Transform.Right.Equals(Vector3.UnitY))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.PosY, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.PosY, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.PosY, this.OrbitAngle, parentActor.Transform.Look);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.PosY, this.OrbitAngle, parentActor.Transform.Up);
            }

            //Orbit about Z
            else if (parentActor.Transform.Right.Equals(Vector3.UnitZ))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.PosZ, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.PosZ, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.PosZ, this.OrbitAngle, parentActor.Transform.Look);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.PosZ, this.OrbitAngle, parentActor.Transform.Up);
            }

            //Orbit about -X
            else if (parentActor.Transform.Right.Equals(-Vector3.UnitX))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.NegX, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.NegX, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.NegX, this.OrbitAngle, parentActor.Transform.Look);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.NegX, this.OrbitAngle, parentActor.Transform.Up);
            }

            //Orbit about -Y
            else if (parentActor.Transform.Right.Equals(-Vector3.UnitY))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.NegY, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.NegY, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.NegY, this.OrbitAngle, parentActor.Transform.Look);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.NegY, this.OrbitAngle, parentActor.Transform.Up);
            }

            //Orbit about -Z
            else if (parentActor.Transform.Right.Equals(-Vector3.UnitZ))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.NegZ, this.OrbitSpeed);
                this.TargetPositionVector = MatrixUtility.CalculateTargetPositionVector(Axis.NegZ, this.OrbitAngle, parentActor.Transform.Translation, parentActor.OrbitPoint);
                this.TargetLookVector = MatrixUtility.CalculateTargetLookVector(Axis.NegZ, this.OrbitAngle, parentActor.Transform.Look);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.NegZ, this.OrbitAngle, parentActor.Transform.Up);
            }
        }

        //Rotate clockwise about the look vector
        public void RotateClockwiseAboutLookVector(GameTime gameTime, OrbitalCamera parentActor)
        {
            //Rotate about +X
            if (parentActor.Transform.Look.Equals(Vector3.UnitX))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.PosX, this.OrbitSpeed);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.PosX, this.OrbitAngle, parentActor.Transform.Up);
            }

            //Rotate about +Y
            else if (parentActor.Transform.Look.Equals(Vector3.UnitY))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.PosY, this.OrbitSpeed);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.PosY, this.OrbitAngle, parentActor.Transform.Up);
            }

            //Rotate about +Z
            else if (parentActor.Transform.Look.Equals(Vector3.UnitZ))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.PosZ, this.OrbitSpeed);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.PosZ, this.OrbitAngle, parentActor.Transform.Up);
            }

            //Rotate about -X
            else if (parentActor.Transform.Look.Equals(-Vector3.UnitX))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.NegX, this.OrbitSpeed);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.NegX, this.OrbitAngle, parentActor.Transform.Up);
            }

            //Rotate about -Y
            else if (parentActor.Transform.Look.Equals(-Vector3.UnitY))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.NegY, this.OrbitSpeed);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.NegY, this.OrbitAngle, parentActor.Transform.Up);
            }

            //Rotate about -Z
            else if (parentActor.Transform.Look.Equals(-Vector3.UnitZ))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.NegZ, this.OrbitSpeed);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.NegZ, this.OrbitAngle, parentActor.Transform.Up);
            }
        }

        //Rotate counter-clockwise about the look vector
        public void RotateCounterClockwiseAboutLookVector(GameTime gameTime, OrbitalCamera parentActor)
        {
            //Rotate about +X
            if (parentActor.Transform.Look.Equals(Vector3.UnitX))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.NegX, this.OrbitSpeed);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.NegX, this.OrbitAngle, parentActor.Transform.Up);
            }

            //Rotate about +Y
            else if (parentActor.Transform.Look.Equals(Vector3.UnitY))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.NegY, this.OrbitSpeed);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.NegY, this.OrbitAngle, parentActor.Transform.Up);
            }

            //Rotate about +Z
            else if (parentActor.Transform.Look.Equals(Vector3.UnitZ))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.NegZ, this.OrbitSpeed);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.NegZ, this.OrbitAngle, parentActor.Transform.Up);
            }

            //Rotate about -X
            else if (parentActor.Transform.Look.Equals(-Vector3.UnitX))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.PosX, this.OrbitSpeed);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.PosX, this.OrbitAngle, parentActor.Transform.Up);
            }

            //Rotate about -Y
            else if (parentActor.Transform.Look.Equals(-Vector3.UnitY))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.PosY, this.OrbitSpeed);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.PosY, this.OrbitAngle, parentActor.Transform.Up);
            }

            //Rotate about -Z
            else if (parentActor.Transform.Look.Equals(-Vector3.UnitZ))
            {
                this.RotationMatrix = MatrixUtility.CalculateRotationMatrix(gameTime, Axis.PosZ, this.OrbitSpeed);
                this.TargetUpVector = MatrixUtility.CalculateTargetUpVector(Axis.PosZ, this.OrbitAngle, parentActor.Transform.Up);
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
                StateManager.IsCameraMoving = false;
            }
            else
            {
                //Update up vector
                parentActor.Transform.Up = Vector3.Transform(parentActor.Transform.Up, this.RotationMatrix);

                //Update motion state
                StateManager.IsCameraMoving = true;
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
            if (Vector3.Distance(parentActor.Transform.Translation, this.TargetPositionVector) <= Math.Abs(0.2f + (this.CurrentZoom * this.OrbitSpeed)))
            {
                //Update camera position
                parentActor.Transform.Translation = this.TargetPositionVector;

                //Update look vector
                parentActor.Transform.Look = this.TargetLookVector;

                //Reset target vectors
                this.TargetPositionVector = Vector3.Zero;
                this.TargetLookVector = Vector3.Zero;

                //Update motion state
                StateManager.IsCameraMoving = false;
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
                StateManager.IsCameraMoving = true;
            }
        }

        public void UpdateListenerPosition(Actor3D parentActor)
        {
            //Update listener position
            this.SoundManager.UpdateListenerPosition(parentActor.Transform.Translation, parentActor.Transform.Look, parentActor.Transform.Up);
        }
        #endregion

        public override void Update(GameTime gameTime, IActor actor)
        {
            Actor3D parentActor = actor as Actor3D;
            this.HandleKeyboardInput(gameTime, parentActor);
            this.HandleMouseInput(gameTime, parentActor);
            this.HandleMovement(gameTime, parentActor);
            this.UpdateListenerPosition(parentActor);
        }
        #endregion
    }
}