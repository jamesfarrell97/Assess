/*
Function: 		This child class for drawing primitives where the vertex data is buffered on the GFX card in VRAM.
                Note: 
                - The class is generic and can be used to draw VertexPositionColor, VertexPositionColorTexture, VertexPositionColorNormal types etc.
                - For each draw the GFX card refers to vertex data that has already been buffered to VRAM 
                - This is a more efficient approach than either using the VertexData or DynamicBufferedVertexData classes if
                  you wish to draw a large number of primitives on screen.

Author: 		NMCG
Version:		1.0
Date Updated:	
Bugs:			None
Fixes:			None
See:            http://rbwhitaker.wikidot.com/index-and-vertex-buffers
*/

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class BufferedVertexData<T> : VertexData<T> where T : struct, IVertexType
    {
        #region Variables
        private VertexBuffer vertexBuffer;
        private GraphicsDevice graphicsDevice;
        #endregion

        #region Properties
        public VertexBuffer VertexBuffer
        {
            get
            {
                return vertexBuffer;
            }
            set
            {
                vertexBuffer = value;

            }
        }
        public GraphicsDevice GraphicsDevice
        {
            get
            {
                return this.graphicsDevice;
            }
        }
        #endregion

        #region Constructor
        public BufferedVertexData(
            GraphicsDevice graphicsDevice, 
            T[] vertices, 
            VertexBuffer vertexBuffer, 
            PrimitiveType primitiveType, 
            int primitiveCount
        ) : base(vertices, primitiveType, primitiveCount) {
            this.graphicsDevice = graphicsDevice;
            this.VertexBuffer = vertexBuffer;
            
            //Set data on the reserved space
            this.vertexBuffer.SetData(this.Vertices);
        }
    
        public BufferedVertexData(
            GraphicsDevice graphicsDevice, 
            T[] vertices, 
            PrimitiveType primitiveType, 
            int primitiveCount
        ) : base(vertices, primitiveType, primitiveCount) {
            this.graphicsDevice = graphicsDevice;
            this.VertexBuffer = new VertexBuffer(graphicsDevice, typeof(T), vertices.Length, BufferUsage.None);
            
            //Set data on the reserved space
            this.vertexBuffer.SetData(this.Vertices);
        }
        #endregion

        #region Methods
        public void SetData(T[] vertices)
        {
            this.Vertices = vertices;

            //Set data on the reserved space
            this.vertexBuffer.SetData(this.Vertices);
        }

        public override void Draw(GameTime gameTime, Effect effect)
        {
            //This is what we want GFX to draw
            effect.GraphicsDevice.SetVertexBuffer(this.vertexBuffer);

            //Draw
            effect.GraphicsDevice.DrawPrimitives(this.PrimitiveType, 0, this.PrimitiveCount);           
        }

        public new object Clone()
        {
            return new BufferedVertexData<T>(
                this.graphicsDevice,    //Shallow - Reference
                this.Vertices,          //Shallow - Reference
                this.PrimitiveType,     //Deep - Struct
                this.PrimitiveCount     //Deep - Primitive
            );
        }
        #endregion
    }
}