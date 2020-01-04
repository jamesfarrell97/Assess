using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    public class Camera3D : Actor3D
    {
        #region Fields
        private ProjectionParameters projectionParameters;
        private Viewport viewport;

        //Centre for each cameras viewport - important when deciding how much to turn the camera when a particular camera view, in a multi-screen layout, is in focus
        private Vector2 viewportCentre;

        //Used to sort cameras by depth on screen where 0 = top-most, 1 = bottom-most (i.e. 0 for rear-view mirror and > 0 for main game screen)
        private float drawDepth;
        #endregion

        #region Properties
        public Matrix Projection
        {
            get
            {
                return this.projectionParameters.Projection;
            }
        }

        public Matrix View
        {
            get
            {
                return Matrix.CreateLookAt(
                    this.Transform.Translation,
                    this.Transform.Translation + this.Transform.Look,
                    this.Transform.Up
                );
            }
        }

        public ProjectionParameters ProjectionParameters
        {
            get
            {
                return this.projectionParameters;
            }
            set
            {
                this.projectionParameters = value;
            }
        }

        public Viewport Viewport
        {
            get
            {
                return this.viewport;
            }
            set
            {
                this.viewport = value;
                this.viewportCentre = new Vector2(this.viewport.Width / 2.0f, this.viewport.Height / 2.0f);
            }
        }

        public Vector2 ViewportCentre
        {
            get
            {
                return this.viewportCentre;
            }
        }

        public float DrawDepth
        {
            get
            {
                return this.drawDepth;
            }
            set
            {
                this.drawDepth = value;
            }
        }

        public BoundingFrustum BoundingFrustum
        {
            get
            {
                return new BoundingFrustum(this.View * this.ProjectionParameters.Projection);
            }
        }
        #endregion

        #region Constructors
        public Camera3D(
            string id,
            ActorType actorType,
            Transform3D transform,
            ProjectionParameters projectionParameters,
            Viewport viewport,
            StatusType statusType
        ) : this(id, actorType, transform, projectionParameters, viewport, 0, statusType) {
        }

        public Camera3D(
            string id,
            ActorType actorType,
            Transform3D transform,
            ProjectionParameters projectionParameters,
            Viewport viewport,
            float drawDepth,
            StatusType statusType
        ) : base(id, actorType, transform, statusType) {
            this.Viewport = viewport;
            this.DrawDepth = drawDepth;
            this.ProjectionParameters = projectionParameters;
        }

        //Creates a default camera3D - we can use this for a fixed camera archetype i.e. one we will clone - see MainApp::InitialiseCameras()
        public Camera3D(
            string id,
            ActorType actorType,
            Viewport viewport
        ) : this(id, actorType, Transform3D.Zero, ProjectionParameters.OrthographicDeepTenByTen, viewport, 0, StatusType.Update) {
        }
        #endregion

        #region Methods
        public string GetDescription()
        {
            return "Camera: " + this.ID;
        }

        public override bool Equals(object obj)
        {
            Camera3D other = obj as Camera3D;
            return Vector3.Equals(this.Transform.Translation, other.Transform.Translation)
                && Vector3.Equals(this.Transform.Look, other.Transform.Look)
                && Vector3.Equals(this.Transform.Up, other.Transform.Up);
        }

        public override int GetHashCode()
        {
            int hash = 1;
            hash = hash * 31 + this.Transform.Translation.GetHashCode();
            hash = hash * 17 + this.Transform.Look.GetHashCode();
            hash = hash * 13 + this.Transform.Up.GetHashCode();
            return hash;
        }

        public new object Clone()
        {
            return new Camera3D(
                (string) ("Clone - " + this.ID),
                (ActorType) this.ActorType,
                (Transform3D) this.Transform.Clone(),
                this.ProjectionParameters,
                this.Viewport,
                0,
                StatusType.Update
            );
        }

        public override string ToString()
        {
            return this.ID
                + ", Translation: " + MathUtility.Round(this.Transform.Translation, 0)
                + ", Look: " + MathUtility.Round(this.Transform.Look, 0)
                + ", Up: " + MathUtility.Round(this.Transform.Up, 0)
                + ", Depth: " + this.drawDepth;
        }
        #endregion
    }
}