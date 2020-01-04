using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class OrbitalCamera : Orthographic3DCamera
    {
        #region Fields
        Vector3 orbitPoint;
        #endregion

        #region Properties
        public Vector3 OrbitPoint { get => orbitPoint; set => orbitPoint = value; }
        #endregion

        #region Constructors
        public OrbitalCamera(
            string id,
            ActorType actorType,
            Transform3D transform,
            ProjectionParameters projectionParameters,
            Viewport viewport,
            float drawDepth,
            StatusType statusType,
            Vector3 orbitPoint
        ) : base(id, actorType, transform, projectionParameters, viewport, drawDepth, statusType) {
            this.OrbitPoint = orbitPoint;
        }
        #endregion

        #region Methods
        #endregion
    }
}