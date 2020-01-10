using Microsoft.Xna.Framework;
using System; 

namespace GDLibrary
{
    public class ProjectionParameters : ICloneable 
    {
        #region Statics
        public static ProjectionParameters OrthographicDeepTenByTen
        {
            get
            {
                return new ProjectionParameters(10, 10, 0, 0, 0.1f, 2500.0f);
            }
        }

        public static ProjectionParameters OrthographicMediumTenByTen
        {
            get
            {
                return new ProjectionParameters(10, 10, 0, 0, 0.1f, 1000.0f);
            }
        }

        public static ProjectionParameters OrthographicShallowTenByTen
        {
            get
            {
                return new ProjectionParameters(10, 10, 0, 0, 0.1f, 500.0f);
            }
        }

        public static ProjectionParameters PerspectiveDeepFiveThree
        {
            get
            {
                return new ProjectionParameters(0, 0, MathHelper.PiOver2, 5.0f / 3, 0.1f, 2500);
            }
        }

        public static ProjectionParameters PerspectiveDeepFourThree
        {
            get
            {
                return new ProjectionParameters(0, 0, MathHelper.PiOver2, 4.0f / 3, 0.1f, 2500);
            }
        }


        public static ProjectionParameters PerspectiveDeepSixteenTen
        {
            get
            {
                return new ProjectionParameters(0, 0, MathHelper.PiOver2, 16.0f / 10, 0.1f, 2500);
            }
        }

        public static ProjectionParameters PerspectiveDeepSixteenNine
        {
            get
            {
                return new ProjectionParameters(0, 0, MathHelper.PiOver4, 16.0f / 9, 1, 2500);
            }
        }

        //Medium relates to the distance between the near and far clipping planes i.e. 1 to 1000
        public static ProjectionParameters PerspectiveMediumFiveThree
        {
            get
            {
                return new ProjectionParameters(0, 0, MathHelper.PiOver2, 5.0f / 3, 0.1f, 1000);
            }
        }

        public static ProjectionParameters PerspectiveMediumFourThree
        {
            get
            {
                return new ProjectionParameters(0, 0, MathHelper.PiOver2, 4.0f / 3, 0.1f, 1000);
            }
        }

        public static ProjectionParameters PerspectiveMediumSixteenTen
        {
            get
            {
                return new ProjectionParameters(0, 0, MathHelper.PiOver2, 16.0f / 10, 0.1f, 1000);
            }
        }

        public static ProjectionParameters PerspectiveMediumSixteenNine
        {
            get
            {
                return new ProjectionParameters(0, 0, MathHelper.PiOver4, 16.0f / 9, 0.1f, 1000);
            }
        }

        //Shallow relates to the distance between the near and far clipping planes i.e. 1 to 500
        public static ProjectionParameters PerspectiveShallowFiveThree
        {
            get
            {
                return new ProjectionParameters(0, 0, MathHelper.PiOver2, 5.0f / 3, 0.1f, 500);
            }
        }

        public static ProjectionParameters PerspectiveShallowFourThree
        {
            get
            {
                return new ProjectionParameters(0, 0, MathHelper.PiOver2, 4.0f / 3, 0.1f, 500);
            }
        }

        public static ProjectionParameters PerspectiveShallowSixteenTen
        {
            get
            {
                return new ProjectionParameters(0, 0, MathHelper.PiOver2, 16.0f / 10, 0.1f, 500);
            }
        }

        public static ProjectionParameters PerspectiveShallowSixteenNine
        {
            get
            {
                return new ProjectionParameters(0, 0, MathHelper.PiOver2, 16.0f / 9, 0.1f, 500);
            }
        }
        #endregion

        #region Fields
        private float width;
        private float height;
        private float fieldOfView;
        private float aspectRatio;
        private float nearClipPlane;
        private float farClipPlane;

        private Matrix projection;

        private bool isDirty = true;
        #endregion

        #region Properties
        public float Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
                this.isDirty = true;
            }
        }

        public float Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
                this.isDirty = true;
            }
        }

        public float FieldOfView
        {
            get
            {
                return this.fieldOfView;
            }
            set
            {
                this.fieldOfView = value <= 0 ? MathHelper.PiOver4 : value;
                this.isDirty = true;
            }
        }

        public float AspectRatio
        {
            get
            {
                return this.aspectRatio;
            }
            set
            {
                this.aspectRatio = value <= 0 ? 4.0f / 3 : value;
                this.isDirty = true;
            }
        }

        public float NearClipPlane
        {
            get
            {
                return this.nearClipPlane;
            }
            set
            {
                this.nearClipPlane = value < 0 ? 1.0f : value;
				this.isDirty = true;
            }
        }

        public float FarClipPlane
        {
            get
            {
                return this.farClipPlane;
            }
            set
            {
                this.farClipPlane = value >= 10 ? value : 10.0f;
				this.isDirty = true;				
            }
        }

        public Matrix Projection
        {
            get
            {
                if (this.IsDirty)
                {
                    //if (StateManager.IsOrthographic)
                    //{
                    //  this.projection = Matrix.CreateOrthographic(
                    //      this.Width,
                    //      this.Height,
                    //      this.NearClipPlane,
                    //      this.FarClipPlane
                    //  );
                    //}

                    //else
                    //{
                    //}

                    this.projection = Matrix.CreatePerspectiveFieldOfView(
                        this.FieldOfView,
                        this.AspectRatio,
                        this.NearClipPlane,
                        this.FarClipPlane
                    );

                    this.IsDirty = false;
                }

                return this.projection;
            }
        }

        public bool IsDirty
        {
            get
            {
                return this.isDirty;
            }
            set
            {
                this.isDirty = value;
            }
        }
        #endregion

        #region Constructors
        public ProjectionParameters(
            double width, 
            double height, 
            double nearClipPlane, 
            double farClipPlane
        ) : this ((float) width, (float) height, 0, 0, (float) nearClipPlane, (float) farClipPlane) {
        }

        public ProjectionParameters(
            float fieldOfView, 
            float aspectRatio, 
            float nearClipPlane, 
            float farClipPlane
        ) : this (0, 0, fieldOfView, aspectRatio, nearClipPlane, farClipPlane) {
        }

        public ProjectionParameters(float width, float height, float fieldOfView, float aspectRatio, float nearClipPlane, float farClipPlane)
        {
            this.Width = width;
            this.Height = height;
            this.FieldOfView = fieldOfView;
            this.AspectRatio = aspectRatio;
            this.NearClipPlane = nearClipPlane;
            this.FarClipPlane = farClipPlane;
        }
        #endregion

        #region Methods
        public override int GetHashCode()
        {
            int hash = 0;
            hash += 31 * this.width.GetHashCode();
            hash += 17 * this.height.GetHashCode();
            hash += 25 * this.fieldOfView.GetHashCode();
            hash += 49 * this.aspectRatio.GetHashCode();
            hash += 53 * this.nearClipPlane.GetHashCode();
            hash += 11 * this.farClipPlane.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            //Non-valid address
            if (obj == null) return false;

            //Matching memory addresses
            if (this == obj) return true;

            //Null if cast fails
            if (!(obj is ProjectionParameters other))
                return false;

            //TODO - comparison of floating-point values
            return this.width.Equals(other.Width)
                && this.height.Equals(other.Height)
                && this.fieldOfView.Equals(other.FieldOfView)
                && this.aspectRatio.Equals(other.AspectRatio)
                && this.nearClipPlane.Equals(other.NearClipPlane)
                && this.farClipPlane.Equals(other.FarClipPlane);
        }

        public virtual object Clone()
        {
            //Deep copy of the ProjectionParameters
            //Notice we call GETTER PROPERTY and not direct field access.
            return new ProjectionParameters(this.Width, this.Height, this.FieldOfView, this.AspectRatio, this.NearClipPlane, this.FarClipPlane);
        }
        #endregion
    }
}