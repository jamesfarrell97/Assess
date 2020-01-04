/*
Function: 		Represents a simple static camera in our 3D world to which we will later attach controllers. 
Author: 		NMCG
Version:		1.1
Date Updated:	
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    //Represents the base camera class to which controllers can be attached (to do...)
    public class Perspective3DCamera : Camera3D
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public Perspective3DCamera(
            string id, 
            ActorType actorType,
            Transform3D transform, 
            ProjectionParameters ProjectionParameters,
            Viewport viewport, 
            StatusType statusType
        ) : this(id, actorType, transform, ProjectionParameters, viewport, 0, statusType) {
        }

        public Perspective3DCamera(
            string id,
            ActorType actorType,
            Transform3D transform,
            ProjectionParameters ProjectionParameters,
            Viewport viewport,
            float drawDepth,
            StatusType statusType
        ) : base(id, actorType, transform, ProjectionParameters, viewport, drawDepth, statusType) {
            this.ProjectionParameters = ProjectionParameters;
        }

        //Creates a default camera3D - we can use this for a fixed camera archetype i.e. one we will clone - see MainApp::InitialiseCameras()
        public Perspective3DCamera(
            string id, 
            ActorType actorType, 
            Viewport viewport
        ) : this(id, actorType, Transform3D.Zero, ProjectionParameters.PerspectiveMediumFourThree, viewport, 0, StatusType.Update) {
        }
        #endregion

        #region Methods
        public override bool Equals(object obj) {

            Perspective3DCamera other = obj as Perspective3DCamera;
            return Equals(this.Transform.Translation, other.Transform.Translation)
                && Equals(this.Transform.Look, other.Transform.Look)
                && Equals(this.Transform.Up, other.Transform.Up)
                && this.ProjectionParameters.Equals(other.ProjectionParameters);
        }

        public override int GetHashCode() {

            int hash = 1;
            hash = hash * 31 + this.Transform.Translation.GetHashCode();
            hash = hash * 17 + this.Transform.Look.GetHashCode();
            hash = hash * 13 + this.Transform.Up.GetHashCode();
            hash = hash * 53 + this.ProjectionParameters.GetHashCode();
            return hash;
        }

        public new object Clone() {

            return new Perspective3DCamera(
                "Clone - " + this.ID,
                ActorType,
                (Transform3D) this.Transform.Clone(),
                (ProjectionParameters) this.ProjectionParameters.Clone(),
                this.Viewport,
                0,
                StatusType.Update
            );
        }
        #endregion
    }
}