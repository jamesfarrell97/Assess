//using Microsoft.Xna.Framework;
//using System; 

//namespace GDLibrary
//{
//    public class OrthographicProjectionParameters
//    {
//        #region Statics
//        public static OrthographicProjectionParameters StandardDeepHD
//        {
//            get
//            {
//                return new OrthographicProjectionParameters(1280, 720, 0.1f, 2500);
//            }
//        }
//        #endregion

//        #region Fields
//        private float width;
//        private float height;
//        #endregion

//        #region Properties
//        public float Width
//        {
//            get
//            {
//                return this.width;
//            }
//            set
//            {
//                this.width = value;
//				this.IsDirty = true;
//            }
//        }

//        public float Height
//        {
//            get
//            {
//                return this.height;
//            }
//            set
//            {
//                this.height = value;
//				this.IsDirty = true;
//            }
//        }
//        #endregion

//        #region Constructors
//        public OrthographicProjectionParameters(
//            float width, 
//            float height, 
//            float nearClipPlane, 
//            float farClipPlane
//        ) : base(nearClipPlane, farClipPlane) {
//            this.Width = width; 
//            this.Height = height;
//        }
//        #endregion

//        #region Methods
//        public override int GetHashCode()
//        {
//            int hash = 0;
//            hash += 31 * this.width.GetHashCode();
//            hash += 17 * this.height.GetHashCode();
//            return hash;
//        }

//        public override bool Equals(object obj)
//        {
//            //Non-valid address
//            if (obj == null) return false;

//            //Matching memory addresses
//            if (this == obj) return true;

//            //Null if cast fails
//            if (!(obj is OrthographicProjectionParameters other))
//                return false;

//            //TODO - comparison of floating-point values
//            return this.width.Equals(other.Width)
//                && this.height.Equals(other.Height);
//        }

//        public override object Clone()
//        {
//            //Deep copy of the OrthographicProjectionParameters
//            //Notice we call GETTER PROPERTY and not direct field access.
//            return new OrthographicProjectionParameters(this.Width, this.Height, this.NearClipPlane, this.FarClipPlane);
//        }
//        #endregion
//    }
//}