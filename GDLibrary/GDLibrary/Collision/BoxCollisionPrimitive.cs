using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class BoxCollisionPrimitive : ICollisionPrimitive
    {
        #region Variables
        private static Vector3 min = Vector3.Zero;
        private static Vector3 max = new Vector3(0.975f, 0.975f, 0.975f);
        private BoundingBox boundingBox;
        private BoundingBox originalBoundingBox;
        #endregion

        #region Properties
        public static Vector3 Min { get => min; set => min = value; }
        public static Vector3 Max { get => max; set => max = value; }

        public BoundingBox BoundingBox { get => boundingBox; set => boundingBox = value; }
        public BoundingBox OriginalBoundingBox { get => originalBoundingBox; set => originalBoundingBox = value; }
        #endregion

        #region Constructors
        public BoxCollisionPrimitive() 
        {
        }
        #endregion

        #region Methods
        public bool Intersects(BoundingBox box)
        {
            return this.BoundingBox.Intersects(box);      
        }

        public bool Intersects(BoundingSphere sphere)
        {
            return this.BoundingBox.Intersects(sphere);
        }

        public bool Intersects(ICollisionPrimitive collisionPrimitive)
        {
            return collisionPrimitive.Intersects(this.BoundingBox);
        }

        //Tests if the bounding box for this primitive, when moved, will intersect with the collisionPrimitive passed into the method
        public bool Intersects(ICollisionPrimitive collisionPrimitive, Vector3 translation)
        {
            BoundingBox projectedBox = this.BoundingBox;
            projectedBox.Max += translation;
            projectedBox.Min += translation;
            return collisionPrimitive.Intersects(projectedBox);
        }

        public bool Intersects(Ray ray)
        {
            return (ray.Intersects(this.BoundingBox) > 0);
        }

        //Detect intersection and passes back distance to intersected primitive
        public bool Intersects(Ray ray, out float? distance)
        {
            distance = ray.Intersects(this.BoundingBox);
            return (distance > 0);
        }

        public bool Intersects(BoundingFrustum frustum)
        {
            return (frustum.Contains(this.BoundingBox) == ContainmentType.Contains) 
                || (frustum.Contains(this.BoundingBox) == ContainmentType.Intersects);
        }

        public void Update(GameTime gameTime, Transform3D transform)
        {
            this.originalBoundingBox = new BoundingBox(Min, Max);
            this.boundingBox.Max = OriginalBoundingBox.Max * transform.Scale;
            this.boundingBox.Min = OriginalBoundingBox.Min * transform.Scale;
            this.boundingBox.Max += transform.Translation;
            this.boundingBox.Min += transform.Translation;
        }

        public override string ToString()
        {
            return this.BoundingBox.ToString();
        }

        public object Clone()
        {
            return new BoxCollisionPrimitive();
        }
        #endregion
    }
}