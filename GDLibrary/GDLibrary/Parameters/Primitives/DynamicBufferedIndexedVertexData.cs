/*
Function: 		This child class for drawing primitives where the vertex data is buffered on the GFX card in VRAM
                and the number of vertices required is reduced through the use of an index buffer.
                Note:
                - The class is generic and can be used to draw VertexPositionColor, VertexPositionColorTexture, VertexPositionColorNormal types etc.
                - For each draw the GFX card refers to vertex and index data that has already been buffered to VRAM
                - This is a more efficient approach than either using the VertexData, BufferedVertexData, or DynamicBufferedVertexData classes if
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
    public class DynamicBufferedIndexedVertexData<T> : VertexData<T> where T : struct, IVertexType
    {
        #region Variables
        private DynamicVertexBuffer vertexBuffer;
        private DynamicIndexBuffer indexBuffer;
        private GraphicsDevice graphicsDevice;
        private short[] indices;
        #endregion

        #region Properties
        public DynamicVertexBuffer VertexBuffer
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

        public DynamicIndexBuffer IndexBuffer
        {
            get
            {
                return indexBuffer;
            }
            set
            {
                indexBuffer = value;

            }
        }

        public GraphicsDevice GraphicsDevice
        {
            get
            {
                return this.graphicsDevice;
            }
            set
            {
                this.graphicsDevice = value;
            }
        }

        public short[] Indices
        {
            get
            {
                return this.indices;
            }
            set
            {
                this.indices = value;
            }
        }
        #endregion

        #region Constructors
        //Allows developer to pass in vertices AND buffer - more efficient since buffer is defined ONCE outside of the object instead of a new VertexBuffer for EACH instance of the class
        public DynamicBufferedIndexedVertexData(
            GraphicsDevice graphicsDevice,
            T[] vertices,
            DynamicVertexBuffer vertexBuffer,
            short[] indices,
            PrimitiveType primitiveType,
            int primitiveCount
        ) : base(vertices, primitiveType, primitiveCount) {
            this.GraphicsDevice = graphicsDevice;
            this.VertexBuffer = vertexBuffer;
 
            //Set vertex and index buffer data on the reserved space
            this.VertexBuffer.SetData(this.Vertices);
            this.IndexBuffer = new DynamicIndexBuffer(graphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            this.IndexBuffer.SetData(indices);
        }

        //Buffer is created INSIDE the class so each class has a buffer - not efficient
        public DynamicBufferedIndexedVertexData(
            GraphicsDevice graphicsDevice,
            T[] vertices,
            short[] indices, 
            PrimitiveType primitiveType, 
            int primitiveCount
        ) : base(vertices, primitiveType, primitiveCount) {
            this.GraphicsDevice = graphicsDevice;
            this.VertexBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(T), vertices.Length, BufferUsage.None);

            //Set vertex and index buffer data on the reserved space
            this.VertexBuffer.SetData(this.Vertices);
            this.IndexBuffer = new DynamicIndexBuffer(graphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            this.IndexBuffer.SetData(indices);
        }
        #endregion

        #region Methods
        public void SetData(T[] vertices, short[] indices)
        {
            this.Vertices = vertices;

            if(this.VertexBuffer == null)
                this.VertexBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(T), vertices.Length, BufferUsage.None);

            this.VertexBuffer.SetData(this.Vertices);

            if(this.IndexBuffer == null)
                this.IndexBuffer = new DynamicIndexBuffer(graphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);

            this.IndexBuffer.SetData(indices);
        }

        public override void Draw(GameTime gameTime, Effect effect)
        {
            effect.GraphicsDevice.Indices = this.IndexBuffer;
            effect.GraphicsDevice.SetVertexBuffer(this.VertexBuffer);
            effect.GraphicsDevice.DrawIndexedPrimitives(this.PrimitiveType, 0, 0, this.Vertices.Length, 0, this.PrimitiveCount);
        }

        public new object Clone()
        {
            return new DynamicBufferedIndexedVertexData<T>(
                this.GraphicsDevice,
                this.Vertices,
                this.Indices,
                this.PrimitiveType,
                this.PrimitiveCount
            );
        }
        #endregion
    }
}