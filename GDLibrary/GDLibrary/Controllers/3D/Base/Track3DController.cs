/*
Function: 		Creates a controller which moves the attached object along a user defined 3D track
Author: 		NMCG
Version:		1.0
Date Updated:	
Bugs:			
Fixes:			
*/

using Microsoft.Xna.Framework;
namespace GDLibrary
{
    //used to cause an object (or camera) to move along a predefined track
    public class Track3DController : Controller
    {
        private static readonly int DefaultCurveEvaluationPrecision = 4;

        #region Fields
        private Track3D transform3DCurve;
        private PlayStatusType playStatusType;
        private float elapsedTimeInMs;
        private int curveEvaluationPrecision;
        #endregion

        #region Properties
        public Track3D Transform3DCurve
        {
            get
            {
                return this.transform3DCurve;
            }
            set
            {
                this.transform3DCurve = value;
            }
        }
        public int CurveEvaluationPrecision
        {
            get
            {
                return this.curveEvaluationPrecision;
            }
            set
            {
                this.curveEvaluationPrecision = value;
            }
        }

        public float ElapsedTimeInMs { get => elapsedTimeInMs; set => elapsedTimeInMs = value; }
        #endregion

        //pre-curveEvaluationPrecision compatability constructor
        public Track3DController(
            string id, 
            ControllerType controllerType,
            Track3D transform3DCurve, 
            PlayStatusType playStatusType
        ) : this(id, controllerType, transform3DCurve, playStatusType, DefaultCurveEvaluationPrecision) {
            StateManager.FinishedTracking = false;
        }

        public Track3DController(
            string id, 
            ControllerType controllerType,
            Track3D transform3DCurve, 
            PlayStatusType playStatusType,
            int curveEvaluationPrecision
        ) : base(id, controllerType) {
            this.transform3DCurve = transform3DCurve;
            this.playStatusType = playStatusType;
            this.ElapsedTimeInMs = 0;
            this.curveEvaluationPrecision = curveEvaluationPrecision;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            if (actor is Actor3D parentActor)
            {
                if (this.playStatusType == PlayStatusType.Play)
                    UpdateTrack(gameTime, parentActor);
                else if ((this.playStatusType == PlayStatusType.Reset) || (this.playStatusType == PlayStatusType.Stop))
                    this.ElapsedTimeInMs = 0;
            }
        }

        Vector3 translation, look, up;
        private void UpdateTrack(GameTime gameTime, Actor3D parentActor)
        {
            this.ElapsedTimeInMs += gameTime.ElapsedGameTime.Milliseconds;
            this.transform3DCurve.Evalulate(ElapsedTimeInMs, this.curveEvaluationPrecision, out translation, out look, out up);
            parentActor.Transform.Translation = translation;
            parentActor.Transform.Look = look;
            parentActor.Transform.Up = up;
        }
    }
}