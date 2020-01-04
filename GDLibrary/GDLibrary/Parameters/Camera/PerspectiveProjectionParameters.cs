//using Microsoft.Xna.Framework;
//using System; 

//namespace GDLibrary
//{
//    public class PerspectiveProjectionParameters : ProjectionParameters
//    {
//        #region Statics
//        //Deep relates to the distance between the near and far clipping planes i.e. 1 to 2500
//        public static PerspectiveProjectionParameters StandardDeepFiveThree
//        {
//            get
//            {
//                return new PerspectiveProjectionParameters(MathHelper.PiOver2, 5.0f / 3, 0.1f, 2500);
//            }
//        }

//        public static PerspectiveProjectionParameters StandardDeepFourThree
//        {
//            get
//            {
//                return new PerspectiveProjectionParameters(MathHelper.PiOver2, 4.0f / 3, 0.1f, 2500);
//            }
//        }


//        public static PerspectiveProjectionParameters StandardDeepSixteenTen
//        {
//            get
//            {
//                return new PerspectiveProjectionParameters(MathHelper.PiOver2, 16.0f / 10, 0.1f, 2500);
//            }
//        }

//        public static PerspectiveProjectionParameters StandardDeepSixteenNine
//        {
//            get
//            {
//                return new PerspectiveProjectionParameters(MathHelper.PiOver4, 16.0f / 9, 1, 2500);
//            }
//        }

//        //Medium relates to the distance between the near and far clipping planes i.e. 1 to 1000
//        public static PerspectiveProjectionParameters StandardMediumFiveThree
//        {
//            get
//            {
//                return new PerspectiveProjectionParameters(MathHelper.PiOver2, 5.0f / 3, 0.1f, 1000);
//            }
//        }

//        public static PerspectiveProjectionParameters StandardMediumFourThree
//        {
//            get
//            {
//                return new PerspectiveProjectionParameters(MathHelper.PiOver2, 4.0f / 3, 0.1f, 1000);
//            }
//        }

//        public static PerspectiveProjectionParameters StandardMediumSixteenTen
//        {
//            get
//            {
//                return new PerspectiveProjectionParameters(MathHelper.PiOver2, 16.0f / 10, 0.1f, 1000);
//            }
//        }

//        public static PerspectiveProjectionParameters StandardMediumSixteenNine
//        {
//            get
//            {
//                return new PerspectiveProjectionParameters(MathHelper.PiOver4, 16.0f / 9, 0.1f, 1000);
//            }
//        }

//        //Shallow relates to the distance between the near and far clipping planes i.e. 1 to 500
//        public static PerspectiveProjectionParameters StandardShallowFiveThree
//        {
//            get
//            {
//                return new PerspectiveProjectionParameters(MathHelper.PiOver2, 5.0f / 3, 0.1f, 500);
//            }
//        }

//        public static PerspectiveProjectionParameters StandardShallowFourThree
//        {
//            get
//            {
//                return new PerspectiveProjectionParameters(MathHelper.PiOver2, 4.0f / 3, 0.1f, 500);
//            }
//        }

//        public static PerspectiveProjectionParameters StandardShallowSixteenTen
//        {
//            get
//            {
//                return new PerspectiveProjectionParameters(MathHelper.PiOver2, 16.0f / 10, 0.1f, 500);
//            }
//        }

//        public static PerspectiveProjectionParameters StandardShallowSixteenNine
//        {
//            get
//            {
//                return new PerspectiveProjectionParameters(MathHelper.PiOver2, 16.0f / 9, 0.1f, 500);
//            }
//        }
//        #endregion

//        #region Fields
//        private float fieldOfView;
//        private float aspectRatio;

//		private Matrix projection;
//        #endregion

//        #region Properties
//        public float FieldOfView
//        {
//            get
//            {
//                return this.fieldOfView;
//            }
//            set
//            {
//                this.fieldOfView = value <= 0 ? MathHelper.PiOver4 : value;
//				this.IsDirty = true;
//            }
//        }

//        public float AspectRatio
//        {
//            get
//            {
//                return this.aspectRatio;
//            }
//            set
//            {
//                this.aspectRatio = value <= 0 ? 4.0f/3 : value;
//				this.IsDirty = true;
//            }
//        }

//        public Matrix Projection
//        {
//            get
//            {
//				if (this.IsDirty)
//				{
//					this.projection = Matrix.CreatePerspectiveFieldOfView(
//                        this.FieldOfView, 
//                        this.AspectRatio,
//						this.NearClipPlane, 
//                        this.FarClipPlane
//                    );

//                    this.IsDirty = false;		
//				}			
//				return this.projection;
//            }
//        }
//        #endregion

//        #region Constructors
//        //public PerspectiveProjectionParameters(
//        //    float fieldOfView, 
//        //    float aspectRatio,
//        //    float nearClipPlane, 
//        //    float farClipPlane
//        //) : base (nearClipPlane, farClipPlane) {
//        //    FieldOfView = fieldOfView; 
//        //    AspectRatio = aspectRatio;
//        //}
//        #endregion

//        #region Methods
//        public override int GetHashCode()
//        {
//            int hash = 0;
//            hash += 31 * this.fieldOfView.GetHashCode();
//            hash += 17 * this.aspectRatio.GetHashCode();
//            return hash;
//        }

//        public override bool Equals(object obj)
//        {
//            //Non-valid address
//            if (obj == null) return false;

//            //Matching memory addresses
//            if (this == obj) return true;

//            //Null if cast fails
//            if (!(obj is PerspectiveProjectionParameters other))
//                return false;

//            //TODO - comparison of floating-point values
//            return this.fieldOfView.Equals(other.FieldOfView)
//                && this.aspectRatio.Equals(other.AspectRatio);
//        }

//        public override object Clone()
//        {
//            //Deep copy of the ProjectionParameters
//            //Notice we call GETTER PROPERTY and not direct field access.
//            return new PerspectiveProjectionParameters(this.FieldOfView, this.AspectRatio, this.NearClipPlane, this.FarClipPlane);
//        }
//        #endregion
//    }
//}