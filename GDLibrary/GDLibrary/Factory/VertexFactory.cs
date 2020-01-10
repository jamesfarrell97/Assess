using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace GDLibrary
{

    public static class VertexFactory
    {
        public static VertexPositionColor[] GetColoredTriangle(out PrimitiveType primitiveType, out int primitiveCount)
        {
            primitiveType = PrimitiveType.TriangleList;
            primitiveCount = 1;

            VertexPositionColor[] vertices = new VertexPositionColor[3];
            vertices[0] = new VertexPositionColor(new Vector3(0, 1, 0), Color.White); //T
            vertices[1] = new VertexPositionColor(new Vector3(1, 0, 0), Color.White); //R
            vertices[2] = new VertexPositionColor(new Vector3(-1, 0, 0), Color.White); //L
            return vertices;
        }

        //TriangleStrip
        public static VertexPositionColorTexture[] GetTextureQuadVertices(out PrimitiveType primitiveType, out int primitiveCount)
        {
            primitiveType = PrimitiveType.TriangleStrip;
            primitiveCount = 2;

            float halfLength = 0.5f;

            Vector3 topLeft = new Vector3(-halfLength, halfLength, 0);
            Vector3 topRight = new Vector3(halfLength, halfLength, 0);
            Vector3 bottomLeft = new Vector3(-halfLength, -halfLength, 0);
            Vector3 bottomRight = new Vector3(halfLength, -halfLength, 0);

            //quad coplanar with the XY-plane (i.e. forward facing normal along UnitZ)
            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];
            vertices[0] = new VertexPositionColorTexture(topLeft, Color.White, Vector2.Zero);
            vertices[1] = new VertexPositionColorTexture(topRight, Color.White, Vector2.UnitX);
            vertices[2] = new VertexPositionColorTexture(bottomLeft, Color.White, Vector2.UnitY);
            vertices[3] = new VertexPositionColorTexture(bottomRight, Color.White, Vector2.One);

            return vertices;
        }

        public static VertexPositionColor[] GetSpiralVertices(int radius, int angleInDegrees, float verticalIncrement, out PrimitiveType primitiveType, out int primitiveCount) {

            VertexPositionColor[] vertices = GetCircleVertices(radius, angleInDegrees, out primitiveType, out primitiveCount, OrientationType.XZAxis);

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Position.Y = verticalIncrement * i;
            }

            return vertices;
        }

        public static VertexPositionColor[] GetSphereVertices(int radius, int angleInDegrees, out PrimitiveType primitiveType, out int primitiveCount)
        {
            List<VertexPositionColor> vertList = new List<VertexPositionColor>();

            vertList.AddRange(GetCircleVertices(radius, angleInDegrees, out primitiveType, out primitiveCount, OrientationType.XYAxis));
            vertList.AddRange(GetCircleVertices(radius, angleInDegrees, out primitiveType, out primitiveCount, OrientationType.YZAxis));
            vertList.AddRange(GetCircleVertices(radius, angleInDegrees, out primitiveType, out primitiveCount, OrientationType.XZAxis));
            primitiveCount = vertList.Count - 1;

            return vertList.ToArray();
        }

        public static VertexPositionColor[] GetCircleVertices(int radius, int angleInDegrees, out PrimitiveType primitiveType, out int primitiveCount, OrientationType orientationType)
        {
            primitiveType = PrimitiveType.LineStrip;
            primitiveCount = 360 / angleInDegrees;
            VertexPositionColor[] vertices = new VertexPositionColor[primitiveCount + 1];

            Vector3 position = Vector3.Zero;
            float angleInRadians = MathHelper.ToRadians(angleInDegrees);

            for (int i = 0; i <= primitiveCount; i++)
            {
                if (orientationType == OrientationType.XYAxis)
                {
                    position.X = (float)(radius * Math.Cos(i * angleInRadians));
                    position.Y = (float)(radius * Math.Sin(i * angleInRadians));
                }
                else if (orientationType == OrientationType.XZAxis)
                {
                    position.X = (float)(radius * Math.Cos(i * angleInRadians));
                    position.Z = (float)(radius * Math.Sin(i * angleInRadians));
                }
                else
                {
                    position.Y = (float)(radius * Math.Cos(i * angleInRadians));
                    position.Z = (float)(radius * Math.Sin(i * angleInRadians));
                }

                vertices[i] = new VertexPositionColor(position, Color.White);
            }
            return vertices;
        }

        /******************************************** Textured - Quad, Cube & Pyramid ********************************************/

        public static VertexPositionColorTexture[] GetVerticesPositionColorTextureQuad(int sidelength, out PrimitiveType primitiveType, out int primitiveCount)
        {
            primitiveType = PrimitiveType.TriangleStrip;
            primitiveCount = 2;

            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];
            float halfSideLength = sidelength / 2.0f;

            Vector3 topLeft = new Vector3(-halfSideLength, halfSideLength, 0);
            Vector3 topRight = new Vector3(halfSideLength, halfSideLength, 0);
            Vector3 bottomLeft = new Vector3(-halfSideLength, -halfSideLength, 0);
            Vector3 bottomRight = new Vector3(halfSideLength, -halfSideLength, 0);

            //Quad co-planar with the XY-plane (i.e. forward facing normal along UnitZ)
            vertices[0] = new VertexPositionColorTexture(topLeft, Color.White, Vector2.Zero);
            vertices[1] = new VertexPositionColorTexture(topRight, Color.White, Vector2.UnitX);
            vertices[2] = new VertexPositionColorTexture(bottomLeft, Color.White, Vector2.UnitY);
            vertices[3] = new VertexPositionColorTexture(bottomRight, Color.White, Vector2.One);

            return vertices;
        }

        public static VertexPositionColorTexture[] GetPositionColorTextureCubeVertices(float sidelength, out PrimitiveType primitiveType, out int primitiveCount)
        {
            primitiveType = PrimitiveType.TriangleList;
            primitiveCount = 12;

            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[36];
            float halfSideLength = sidelength / 2.0f;

            Vector3 topLeftFront = new Vector3(-halfSideLength, halfSideLength, halfSideLength);
            Vector3 topLeftBack = new Vector3(-halfSideLength, halfSideLength, -halfSideLength);
            Vector3 topRightFront = new Vector3(halfSideLength, halfSideLength, halfSideLength);
            Vector3 topRightBack = new Vector3(halfSideLength, halfSideLength, -halfSideLength);

            Vector3 bottomLeftFront = new Vector3(-halfSideLength, -halfSideLength, halfSideLength);
            Vector3 bottomLeftBack = new Vector3(-halfSideLength, -halfSideLength, -halfSideLength);
            Vector3 bottomRightFront = new Vector3(halfSideLength, -halfSideLength, halfSideLength);
            Vector3 bottomRightBack = new Vector3(halfSideLength, -halfSideLength, -halfSideLength);

            //UV's
            Vector2 uvTopLeft = new Vector2(0, 0);
            Vector2 uvTopRight = new Vector2(1, 0);
            Vector2 uvBottomLeft = new Vector2(0, 1);
            Vector2 uvBottomRight = new Vector2(1, 1);

            //Top
            vertices[0] = new VertexPositionColorTexture(topLeftFront, Color.White, uvBottomLeft);
            vertices[1] = new VertexPositionColorTexture(topLeftBack, Color.White, uvTopLeft);
            vertices[2] = new VertexPositionColorTexture(topRightBack, Color.White, uvTopRight);

            vertices[3] = new VertexPositionColorTexture(topLeftFront, Color.White, uvBottomLeft);
            vertices[4] = new VertexPositionColorTexture(topRightBack, Color.White, uvTopRight);
            vertices[5] = new VertexPositionColorTexture(topRightFront, Color.White, uvBottomRight);
            
            //Front
            vertices[6] = new VertexPositionColorTexture(topLeftFront, Color.White, uvBottomLeft);
            vertices[7] = new VertexPositionColorTexture(topRightFront, Color.White, uvBottomRight);
            vertices[8] = new VertexPositionColorTexture(bottomLeftFront, Color.White, uvTopLeft);

            vertices[9] = new VertexPositionColorTexture(bottomLeftFront, Color.White, uvTopLeft);
            vertices[10] = new VertexPositionColorTexture(topRightFront, Color.White, uvBottomRight);
            vertices[11] = new VertexPositionColorTexture(bottomRightFront, Color.White, uvTopRight);

            //Back
            vertices[12] = new VertexPositionColorTexture(bottomRightBack, Color.White, uvBottomRight);
            vertices[13] = new VertexPositionColorTexture(topRightBack, Color.White, uvTopRight);
            vertices[14] = new VertexPositionColorTexture(topLeftBack, Color.White, uvTopLeft);

            vertices[15] = new VertexPositionColorTexture(bottomRightBack, Color.White, uvBottomRight);
            vertices[16] = new VertexPositionColorTexture(topLeftBack, Color.White, uvTopLeft);
            vertices[17] = new VertexPositionColorTexture(bottomLeftBack, Color.White, uvBottomLeft);

            //Left
            vertices[18] = new VertexPositionColorTexture(topLeftBack, Color.White, uvTopLeft);
            vertices[19] = new VertexPositionColorTexture(topLeftFront, Color.White, uvTopRight);
            vertices[20] = new VertexPositionColorTexture(bottomLeftFront, Color.White, uvBottomRight);

            vertices[21] = new VertexPositionColorTexture(bottomLeftBack, Color.White, uvBottomLeft);
            vertices[22] = new VertexPositionColorTexture(topLeftBack, Color.White, uvTopLeft);
            vertices[23] = new VertexPositionColorTexture(bottomLeftFront, Color.White, uvBottomRight);

            //Right
            vertices[24] = new VertexPositionColorTexture(bottomRightFront, Color.White, uvBottomLeft);
            vertices[25] = new VertexPositionColorTexture(topRightFront, Color.White, uvTopLeft);
            vertices[26] = new VertexPositionColorTexture(bottomRightBack, Color.White, uvBottomRight);

            vertices[27] = new VertexPositionColorTexture(topRightFront, Color.White, uvTopLeft);
            vertices[28] = new VertexPositionColorTexture(topRightBack, Color.White, uvTopRight);
            vertices[29] = new VertexPositionColorTexture(bottomRightBack, Color.White, uvBottomRight);

            //Bottom
            vertices[30] = new VertexPositionColorTexture(bottomLeftFront, Color.White, uvTopLeft);
            vertices[31] = new VertexPositionColorTexture(bottomRightFront, Color.White, uvTopRight);
            vertices[32] = new VertexPositionColorTexture(bottomRightBack, Color.White, uvBottomRight);

            vertices[33] = new VertexPositionColorTexture(bottomLeftFront, Color.White, uvTopLeft);
            vertices[34] = new VertexPositionColorTexture(bottomRightBack, Color.White, uvBottomRight);
            vertices[35] = new VertexPositionColorTexture(bottomLeftBack, Color.White, uvBottomLeft);

            return vertices;
        }

        public static VertexPositionColor[] GetVerticesPositionColorCube(float sidelength, out PrimitiveType primitiveType, out int primitiveCount)
        {
            primitiveType = PrimitiveType.TriangleList;
            primitiveCount = 12;

            VertexPositionColor[] vertices = new VertexPositionColor[36];
            float halfSideLength = sidelength / 2.0f;

            Vector3 topLeftFront = new Vector3(0, sidelength, sidelength);
            Vector3 topLeftBack = new Vector3(0, sidelength, 0);
            Vector3 topRightFront = new Vector3(sidelength, sidelength, sidelength);
            Vector3 topRightBack = new Vector3(sidelength, sidelength, 0);

            Vector3 bottomLeftFront = new Vector3(0, 0, sidelength);
            Vector3 bottomLeftBack = new Vector3(0, 0, 0);
            Vector3 bottomRightFront = new Vector3(sidelength, 0, sidelength);
            Vector3 bottomRightBack = new Vector3(sidelength, 0, 0);
            
            //Top
            vertices[0] = new VertexPositionColor(topLeftFront, Color.White);
            vertices[1] = new VertexPositionColor(topLeftBack, Color.White);
            vertices[2] = new VertexPositionColor(topRightBack, Color.White);

            vertices[3] = new VertexPositionColor(topLeftFront, Color.White);
            vertices[4] = new VertexPositionColor(topRightBack, Color.White);
            vertices[5] = new VertexPositionColor(topRightFront, Color.White);

            //Front
            vertices[6] = new VertexPositionColor(topLeftFront, Color.White);
            vertices[7] = new VertexPositionColor(topRightFront, Color.White);
            vertices[8] = new VertexPositionColor(bottomLeftFront, Color.White);

            vertices[9] = new VertexPositionColor(bottomLeftFront, Color.White);
            vertices[10] = new VertexPositionColor(topRightFront, Color.White);
            vertices[11] = new VertexPositionColor(bottomRightFront, Color.White);

            //Back
            vertices[12] = new VertexPositionColor(bottomRightBack, Color.White);
            vertices[13] = new VertexPositionColor(topRightBack, Color.White);
            vertices[14] = new VertexPositionColor(topLeftBack, Color.White);

            vertices[15] = new VertexPositionColor(bottomRightBack, Color.White);
            vertices[16] = new VertexPositionColor(topLeftBack, Color.White);
            vertices[17] = new VertexPositionColor(bottomLeftBack, Color.White);

            //Left
            vertices[18] = new VertexPositionColor(topLeftBack, Color.White);
            vertices[19] = new VertexPositionColor(topLeftFront, Color.White);
            vertices[20] = new VertexPositionColor(bottomLeftFront, Color.White);

            vertices[21] = new VertexPositionColor(bottomLeftBack, Color.White);
            vertices[22] = new VertexPositionColor(topLeftBack, Color.White);
            vertices[23] = new VertexPositionColor(bottomLeftFront, Color.White);

            //Right
            vertices[24] = new VertexPositionColor(bottomRightFront, Color.White);
            vertices[25] = new VertexPositionColor(topRightFront, Color.White);
            vertices[26] = new VertexPositionColor(bottomRightBack, Color.White);

            vertices[27] = new VertexPositionColor(topRightFront, Color.White);
            vertices[28] = new VertexPositionColor(topRightBack, Color.White);
            vertices[29] = new VertexPositionColor(bottomRightBack, Color.White);

            //Bottom
            vertices[30] = new VertexPositionColor(bottomLeftFront, Color.White);
            vertices[31] = new VertexPositionColor(bottomRightFront, Color.White);
            vertices[32] = new VertexPositionColor(bottomRightBack, Color.White);

            vertices[33] = new VertexPositionColor(bottomLeftFront, Color.White);
            vertices[34] = new VertexPositionColor(bottomRightBack, Color.White);
            vertices[35] = new VertexPositionColor(bottomLeftBack, Color.White);

            return vertices;
        }

        public static VertexPositionColorTexture[] GetVerticesPositionTexturedPyramidSquare(int sidelength, out PrimitiveType primitiveType, out int primitiveCount)
        {
            primitiveType = PrimitiveType.TriangleList;
            primitiveCount = 6;

            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[18];
            float halfSideLength = sidelength / 2.0f;

            Vector3 topCentre = new Vector3(0, 0.71f * sidelength, 0); //multiplier gives a pyramid where the length of the rising edges == length of the base edges
            Vector3 frontLeft = new Vector3(-halfSideLength, 0, halfSideLength);
            Vector3 frontRight = new Vector3(halfSideLength, 0, halfSideLength);
            Vector3 backLeft = new Vector3(-halfSideLength, 0, -halfSideLength);
            Vector3 backRight = new Vector3(halfSideLength, 0, -halfSideLength);

            Vector2 uvTopCentre = new Vector2(0.5f, 0);
            Vector2 uvTopLeft = new Vector2(0, 0);
            Vector2 uvTopRight = new Vector2(1, 0);
            Vector2 uvBottomLeft = new Vector2(0, 1);
            Vector2 uvBottomRight = new Vector2(1, 1);
            
            //Front
            vertices[0] = new VertexPositionColorTexture(topCentre, Color.White, uvTopCentre);
            vertices[1] = new VertexPositionColorTexture(frontRight, Color.White, uvBottomRight);
            vertices[2] = new VertexPositionColorTexture(frontLeft, Color.White, uvBottomLeft);
            
            //Left
            vertices[3] = new VertexPositionColorTexture(topCentre, Color.White, uvTopCentre);
            vertices[4] = new VertexPositionColorTexture(frontLeft, Color.White, uvBottomRight);
            vertices[5] = new VertexPositionColorTexture(backLeft, Color.White, uvBottomLeft);

            //Right
            vertices[6] = new VertexPositionColorTexture(topCentre, Color.White, uvTopCentre);
            vertices[7] = new VertexPositionColorTexture(backRight, Color.White, uvBottomRight);
            vertices[8] = new VertexPositionColorTexture(frontRight, Color.White, uvBottomLeft);
            
            //Back
            vertices[9] = new VertexPositionColorTexture(topCentre, Color.White, uvTopCentre);
            vertices[10] = new VertexPositionColorTexture(backLeft, Color.White, uvBottomRight);
            vertices[11] = new VertexPositionColorTexture(backRight, Color.White, uvBottomLeft);

            //Bottom
            vertices[12] = new VertexPositionColorTexture(frontLeft, Color.White, uvTopLeft);
            vertices[13] = new VertexPositionColorTexture(frontRight, Color.White, uvTopRight);
            vertices[14] = new VertexPositionColorTexture(backLeft, Color.White, uvBottomLeft);

            vertices[15] = new VertexPositionColorTexture(frontRight, Color.White, uvTopRight);
            vertices[16] = new VertexPositionColorTexture(backRight, Color.White, uvBottomRight);
            vertices[17] = new VertexPositionColorTexture(backLeft, Color.White, uvBottomLeft);

            return vertices;
        }

        /******************************************** Textured & Normal - Diamond *****************************************/

        //Source: https://stackoverflow.com/questions/1966587/given-3-pts-how-do-i-calculate-the-normal-vector
        //Accessed: January 2020
        class Triangle
        {
            private Vector3 a;
            private Vector3 b;
            private Vector3 c;

            public Triangle(Vector3 a, Vector3 b, Vector3 c)
            {
                this.a = a;
                this.b = b;
                this.c = c;
            }

            public Vector3 Normal
            {
                get
                {
                    var dir = Vector3.Cross(b - a, c - a);
                    var norm = Vector3.Normalize(dir);
                    return norm;
                }
            }
        }
        //End of reference

        public static VertexPositionNormalTexture[] GetPositionNormalTextureDiamondVertices(float sidelength, out PrimitiveType primitiveType, out int primitiveCount)
        {
            primitiveType = PrimitiveType.TriangleList;
            primitiveCount = 8;

            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[24];
            float halfSideLength = sidelength / 2.0f;
            float offset = 0.0f;

            //Vertex Declaration
            Vector3 top = new Vector3(offset, halfSideLength + offset, offset);
            Vector3 bottom = new Vector3(offset, -halfSideLength + offset, offset);
            Vector3 back = new Vector3(offset, offset, -halfSideLength + offset);
            Vector3 front = new Vector3(offset, offset, halfSideLength + offset);
            Vector3 left = new Vector3(-halfSideLength + offset, offset, offset);
            Vector3 right = new Vector3(halfSideLength + offset, offset, offset);

            //UV Coordinates
            Vector2 uvTop = new Vector2(0.5f, 0);
            Vector2 uvBottom = new Vector2(0.5f, 1);
            Vector2 uvLeft = new Vector2(0.25f, 0.5f);
            Vector2 uvRight = new Vector2(0.75f, 0.5f);
            Vector2 uvFront = new Vector2(0.5f, 0.5f);
            Vector2 uvLeftBack = new Vector2(0.0f, 0.5f);
            Vector2 uvRightBack = new Vector2(1.0f, 0.5f);

            //Top Normals
            Vector3 topFrontRightNormal = new Triangle(top, right, front).Normal;
            Vector3 topBackRightNormal = new Triangle(top, right, back).Normal;
            Vector3 topBackLeftNormal = new Triangle(top, left, back).Normal;
            Vector3 topFrontLeftNormal = new Triangle(top, left, front).Normal;

            //Bottom Normals
            Vector3 bottomFrontLeftNormal = new Triangle(top, left, front).Normal;
            Vector3 bottomFrontRightNormal = new Triangle(top, right, front).Normal;
            Vector3 bottomBackLeftNormal = new Triangle(top, left, back).Normal;
            Vector3 bottomBackRightNormal = new Triangle(top, right, back).Normal;

            //Top Front-Right
            vertices[0] = new VertexPositionNormalTexture(right, topFrontRightNormal, uvRight);
            vertices[1] = new VertexPositionNormalTexture(front, topFrontRightNormal, uvFront);
            vertices[2] = new VertexPositionNormalTexture(top, topFrontRightNormal, uvTop);

            //Top Back-Right
            vertices[3] = new VertexPositionNormalTexture(back, topBackRightNormal, uvRightBack);
            vertices[4] = new VertexPositionNormalTexture(right, topBackRightNormal, uvRight);
            vertices[5] = new VertexPositionNormalTexture(top, topBackRightNormal, uvTop);

            //Top Back-Left
            vertices[6] = new VertexPositionNormalTexture(left, topBackLeftNormal, uvLeft);
            vertices[7] = new VertexPositionNormalTexture(back, topBackLeftNormal, uvLeftBack);
            vertices[8] = new VertexPositionNormalTexture(top, topBackLeftNormal, uvTop);

            //Top Front-Left
            vertices[9] = new VertexPositionNormalTexture(front, topFrontLeftNormal, uvFront);
            vertices[10] = new VertexPositionNormalTexture(left, topFrontLeftNormal, uvLeft);
            vertices[11] = new VertexPositionNormalTexture(top, topFrontLeftNormal, uvTop);

            //Bottom Front-Left
            vertices[12] = new VertexPositionNormalTexture(front, bottomFrontLeftNormal, uvFront);
            vertices[13] = new VertexPositionNormalTexture(bottom, bottomFrontLeftNormal, uvBottom);
            vertices[14] = new VertexPositionNormalTexture(left, bottomFrontLeftNormal, uvLeft);

            //Bottom Front-Right
            vertices[15] = new VertexPositionNormalTexture(right, bottomFrontRightNormal, uvRight);
            vertices[16] = new VertexPositionNormalTexture(bottom, bottomFrontRightNormal, uvBottom);
            vertices[17] = new VertexPositionNormalTexture(front, bottomFrontRightNormal, uvFront);

            //Bottom Back-Right
            vertices[18] = new VertexPositionNormalTexture(back, bottomBackRightNormal, uvRightBack);
            vertices[19] = new VertexPositionNormalTexture(bottom, bottomBackRightNormal, uvBottom);
            vertices[20] = new VertexPositionNormalTexture(right, bottomBackRightNormal, uvRight);

            //Bottom Back-Left
            vertices[21] = new VertexPositionNormalTexture(left, bottomBackLeftNormal, uvLeft);
            vertices[22] = new VertexPositionNormalTexture(bottom, bottomBackLeftNormal, uvBottom);
            vertices[23] = new VertexPositionNormalTexture(back, bottomBackLeftNormal, uvLeftBack);

            return vertices;
        }

        public static VertexPositionNormalTexture[] GetPositionNormalTextureDiamondIndexedVertices(float sidelength, out PrimitiveType primitiveType, out int primitiveCount)
        {
            primitiveType = PrimitiveType.TriangleList;
            primitiveCount = 8;

            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[7];
            float halfSideLength = sidelength / 2.0f;
            float offset = 0.0f;

            //Vertex Declarations
            Vector3 top = new Vector3(offset, halfSideLength + offset, offset);
            Vector3 bottom = new Vector3(offset, -halfSideLength + offset, offset);
            Vector3 back = new Vector3(offset, offset, -halfSideLength + offset);
            Vector3 front = new Vector3(offset, offset, halfSideLength + offset);
            Vector3 left = new Vector3(-halfSideLength + offset, offset, offset);
            Vector3 right = new Vector3(halfSideLength + offset, offset, offset);

            //UV Coordinates
            Vector2 uvTop = new Vector2(0.5f, 0);
            Vector2 uvBottom = new Vector2(0.5f, 1);
            Vector2 uvLeft = new Vector2(0.25f, 0.5f);
            Vector2 uvRight = new Vector2(0.75f, 0.5f);
            Vector2 uvFront = new Vector2(0.5f, 0.5f);
            Vector2 uvLeftBack = new Vector2(0.0f, 0.5f);
            Vector2 uvRightBack = new Vector2(1.0f, 0.5f);

            //Vertex Assignment
            vertices[0] = new VertexPositionNormalTexture(top, Vector3.UnitY, uvTop);
            vertices[1] = new VertexPositionNormalTexture(bottom, -Vector3.UnitY, uvBottom);
            vertices[2] = new VertexPositionNormalTexture(left, -Vector3.UnitX, uvLeft);
            vertices[3] = new VertexPositionNormalTexture(front, Vector3.UnitZ, uvFront);
            vertices[4] = new VertexPositionNormalTexture(right, Vector3.UnitX, uvRight);
            vertices[5] = new VertexPositionNormalTexture(back, -Vector3.UnitZ, uvLeftBack);
            vertices[6] = new VertexPositionNormalTexture(back, -Vector3.UnitZ, uvRightBack);

            return vertices;
        }

        public static short[] GetDiamondIndices()
        {
            short[] indices = new short[24];

            //Bottom Front-Left
            indices[0] = 3;
            indices[1] = 1;
            indices[2] = 2;

            //Bottom Front-Right
            indices[3] = 4;
            indices[4] = 1;
            indices[5] = 3;

            //Bottom Back-Right
            indices[6] = 6;
            indices[7] = 1;
            indices[8] = 4;

            //Bottom Back-Left
            indices[9] = 2;
            indices[10] = 1;
            indices[11] = 5;

            //Top Front-Left
            indices[12] = 0;
            indices[13] = 3;
            indices[14] = 2;

            //Top Front-Right
            indices[15] = 0;
            indices[16] = 4;
            indices[17] = 3;

            //Top Back-Right
            indices[18] = 0;
            indices[19] = 6;
            indices[20] = 4;

            //Top Back-Left
            indices[21] = 0;
            indices[22] = 2;
            indices[23] = 5;

            return indices;
        }

        /******************************************** Textured & Normal - Cube ********************************************/
        
        public static VertexPositionNormalTexture[] GetPositionNormalTextureCubeVertices(float sidelength, out PrimitiveType primitiveType, out int primitiveCount)
        {
            primitiveType = PrimitiveType.TriangleList;
            primitiveCount = 12;

            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[36];

            float halfSideLength = sidelength / 2.0f;

            Vector3 topLeftFront = new Vector3(-halfSideLength, halfSideLength, halfSideLength);
            Vector3 topLeftBack = new Vector3(-halfSideLength, halfSideLength, -halfSideLength);
            Vector3 topRightFront = new Vector3(halfSideLength, halfSideLength, halfSideLength);
            Vector3 topRightBack = new Vector3(halfSideLength, halfSideLength, -halfSideLength);

            Vector3 bottomLeftFront = new Vector3(-halfSideLength, -halfSideLength, halfSideLength);
            Vector3 bottomLeftBack = new Vector3(-halfSideLength, -halfSideLength, -halfSideLength);
            Vector3 bottomRightFront = new Vector3(halfSideLength, -halfSideLength, halfSideLength);
            Vector3 bottomRightBack = new Vector3(halfSideLength, -halfSideLength, -halfSideLength);

            //UV's
            Vector2 uvTopLeft = new Vector2(0, 0);
            Vector2 uvTopRight = new Vector2(1, 0);
            Vector2 uvBottomLeft = new Vector2(0, 1);
            Vector2 uvBottomRight = new Vector2(1, 1);

            //Top
            vertices[0] = new VertexPositionNormalTexture(topLeftFront, Vector3.UnitY, uvBottomLeft);
            vertices[1] = new VertexPositionNormalTexture(topLeftBack, Vector3.UnitY, uvTopLeft);
            vertices[2] = new VertexPositionNormalTexture(topRightBack, Vector3.UnitY, uvTopRight);

            vertices[3] = new VertexPositionNormalTexture(topLeftFront, Vector3.UnitY, uvBottomLeft);
            vertices[4] = new VertexPositionNormalTexture(topRightBack, Vector3.UnitY, uvTopRight);
            vertices[5] = new VertexPositionNormalTexture(topRightFront, Vector3.UnitY, uvBottomRight);

            //Front
            vertices[6] = new VertexPositionNormalTexture(topLeftFront, Vector3.UnitZ, uvBottomLeft);
            vertices[7] = new VertexPositionNormalTexture(topRightFront, Vector3.UnitZ, uvBottomRight);
            vertices[8] = new VertexPositionNormalTexture(bottomLeftFront, Vector3.UnitZ, uvTopLeft);

            vertices[9] = new VertexPositionNormalTexture(bottomLeftFront, Vector3.UnitZ, uvTopLeft);
            vertices[10] = new VertexPositionNormalTexture(topRightFront, Vector3.UnitZ, uvBottomRight);
            vertices[11] = new VertexPositionNormalTexture(bottomRightFront, Vector3.UnitZ, uvTopRight);

            //Back
            vertices[12] = new VertexPositionNormalTexture(bottomRightBack, -Vector3.UnitZ, uvBottomRight);
            vertices[13] = new VertexPositionNormalTexture(topRightBack, -Vector3.UnitZ, uvTopRight);
            vertices[14] = new VertexPositionNormalTexture(topLeftBack, -Vector3.UnitZ, uvTopLeft);

            vertices[15] = new VertexPositionNormalTexture(bottomRightBack, -Vector3.UnitZ, uvBottomRight);
            vertices[16] = new VertexPositionNormalTexture(topLeftBack, -Vector3.UnitZ, uvTopLeft);
            vertices[17] = new VertexPositionNormalTexture(bottomLeftBack, -Vector3.UnitZ, uvBottomLeft);

            //Left 
            vertices[18] = new VertexPositionNormalTexture(topLeftBack, -Vector3.UnitX, uvTopLeft);
            vertices[19] = new VertexPositionNormalTexture(topLeftFront, -Vector3.UnitX, uvTopRight);
            vertices[20] = new VertexPositionNormalTexture(bottomLeftFront, -Vector3.UnitX, uvBottomRight);

            vertices[21] = new VertexPositionNormalTexture(bottomLeftBack, -Vector3.UnitX, uvBottomLeft);
            vertices[22] = new VertexPositionNormalTexture(topLeftBack, -Vector3.UnitX, uvTopLeft);
            vertices[23] = new VertexPositionNormalTexture(bottomLeftFront, -Vector3.UnitX, uvBottomRight);

            //Right
            vertices[24] = new VertexPositionNormalTexture(bottomRightFront, Vector3.UnitX, uvBottomLeft);
            vertices[25] = new VertexPositionNormalTexture(topRightFront, Vector3.UnitX, uvTopLeft);
            vertices[26] = new VertexPositionNormalTexture(bottomRightBack, Vector3.UnitX, uvBottomRight);

            vertices[27] = new VertexPositionNormalTexture(topRightFront, Vector3.UnitX, uvTopLeft);
            vertices[28] = new VertexPositionNormalTexture(topRightBack, Vector3.UnitX, uvTopRight);
            vertices[29] = new VertexPositionNormalTexture(bottomRightBack, Vector3.UnitX, uvBottomRight);

            //Bottom
            vertices[30] = new VertexPositionNormalTexture(bottomLeftFront, -Vector3.UnitY, uvTopLeft);
            vertices[31] = new VertexPositionNormalTexture(bottomRightFront, -Vector3.UnitY, uvTopRight);
            vertices[32] = new VertexPositionNormalTexture(bottomRightBack, -Vector3.UnitY, uvBottomRight);

            vertices[33] = new VertexPositionNormalTexture(bottomLeftFront, -Vector3.UnitY, uvTopLeft);
            vertices[34] = new VertexPositionNormalTexture(bottomRightBack, -Vector3.UnitY, uvBottomRight);
            vertices[35] = new VertexPositionNormalTexture(bottomLeftBack, -Vector3.UnitY, uvBottomLeft);

            return vertices;
        }

        public static VertexPositionNormalTexture[] GetPositionNormalTextureCubeIndexedVertices(int sidelength, out PrimitiveType primitiveType, out int primitiveCount)
        {
            primitiveType = PrimitiveType.TriangleList;
            primitiveCount = 12;

            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[8];
            float halfSideLength = sidelength / 2.0f;

            //Vertices
            Vector3 topLeftFront = new Vector3(-halfSideLength, halfSideLength, halfSideLength);
            Vector3 topLeftBack = new Vector3(-halfSideLength, halfSideLength, -halfSideLength);
            Vector3 topRightFront = new Vector3(halfSideLength, halfSideLength, halfSideLength);
            Vector3 topRightBack = new Vector3(halfSideLength, halfSideLength, -halfSideLength);

            Vector3 bottomLeftFront = new Vector3(-halfSideLength, -halfSideLength, halfSideLength);
            Vector3 bottomLeftBack = new Vector3(-halfSideLength, -halfSideLength, -halfSideLength);
            Vector3 bottomRightFront = new Vector3(halfSideLength, -halfSideLength, halfSideLength);
            Vector3 bottomRightBack = new Vector3(halfSideLength, -halfSideLength, -halfSideLength);

            //UV's
            Vector2 uvTopLeft = new Vector2(0, 0);
            Vector2 uvTopRight = new Vector2(1, 0);
            Vector2 uvBottomLeft = new Vector2(0, 1);
            Vector2 uvBottomRight = new Vector2(1, 1);

            //Top
            vertices[0] = new VertexPositionNormalTexture(topLeftFront, -Vector3.UnitX, uvBottomLeft);
            vertices[1] = new VertexPositionNormalTexture(topLeftBack, Vector3.UnitY, uvTopLeft);
            vertices[2] = new VertexPositionNormalTexture(topRightFront, Vector3.UnitY, uvBottomRight);
            vertices[3] = new VertexPositionNormalTexture(topRightBack, Vector3.UnitX, uvTopRight);

            //Bottom
            vertices[4] = new VertexPositionNormalTexture(bottomLeftFront, -Vector3.UnitY, uvTopLeft);
            vertices[5] = new VertexPositionNormalTexture(bottomLeftBack, -Vector3.UnitX, uvBottomLeft);
            vertices[6] = new VertexPositionNormalTexture(bottomRightFront, Vector3.UnitX, uvTopRight);
            vertices[7] = new VertexPositionNormalTexture(bottomRightBack, -Vector3.UnitY, uvBottomRight);

            return vertices;
        }

        public static short[] GetCubeIndices()
        {
            short[] indices = new short[36];

            //Left
            indices[0] = 0;
            indices[1] = 4;
            indices[2] = 1;

            indices[3] = 4;
            indices[4] = 5;
            indices[5] = 1;

            //Front
            indices[6] = 2;
            indices[7] = 6;
            indices[8] = 0;

            indices[9] = 6;
            indices[10] = 4;
            indices[11] = 0;

            //Top
            indices[12] = 3;
            indices[13] = 2;
            indices[14] = 1;

            indices[15] = 2;
            indices[16] = 0;
            indices[17] = 1;

            //Bottom
            indices[18] = 6;
            indices[19] = 7;
            indices[20] = 4;

            indices[21] = 7;
            indices[22] = 5;
            indices[23] = 4;

            //Right
            indices[24] = 3;
            indices[25] = 7;
            indices[26] = 2;

            indices[27] = 7;
            indices[28] = 6;
            indices[29] = 2;

            //Back
            indices[30] = 1;
            indices[31] = 5;
            indices[32] = 3;

            indices[33] = 5;
            indices[34] = 7;
            indices[35] = 3;

            return indices;
        }

        //** Textured & Normal - Sphere **/
        public static VertexPositionNormalTexture[] GetPositionNormalTextureUnitSphereIndexedVertices(out PrimitiveType primitiveType, out int primitiveCount)
        {
            primitiveType = PrimitiveType.TriangleList;
            primitiveCount = 12;

            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[8];

            //Vertices
            Vector3 top = new Vector3(0, 9.5f, 0);
            Vector3 bottom = new Vector3(0, -0.5f, 0);
            Vector3 left = new Vector3(-0.5f, 0, 0);
            Vector3 right = new Vector3(0.5f, 0, 0);
            Vector3 back = new Vector3(0, 0, -0.5f);
            Vector3 front = new Vector3(0, 0, 0.5f);

            Vector3 frontTop = new Vector3(0, 0.354f, 0.354f);
            Vector3 frontBottom = new Vector3(0, -0.354f, 0.354f);

            Vector3 backTop = new Vector3(0, 0.354f, -0.354f);
            Vector3 backBottom = new Vector3(0, -0.354f, -0.354f);

            Vector3 leftFront = new Vector3(-0.354f, 0, 0.354f);
            Vector3 rightFront = new Vector3(0.354f, 0, 0.354f);

            Vector3 leftBack = new Vector3(-0.354f, 0, -0.354f);
            Vector3 rightBack = new Vector3(0.354f, 0, -0.354f);

            Vector3 leftTop = new Vector3(-0.354f, 0.354f, 0);
            Vector3 rightTop = new Vector3(0.354f, 0.354f, 0);
            
            Vector3 leftBottom = new Vector3(-0.354f, -0.354f, 0);
            Vector3 rightBottom = new Vector3(0.354f, -0.354f, 0);

            Vector3 leftFrontTop = new Vector3(-0.25f, 0.354f, 0.25f);
            Vector3 leftFrontBottom = new Vector3(-0.25f, -0.354f, 0.25f);

            Vector3 rightFrontTop = new Vector3(0.25f, 0.354f, 0.25f);
            Vector3 rightFrontBottom = new Vector3(0.25f, 0.354f, 0.25f);

            Vector3 leftBackTop = new Vector3(-0.25f, 0.354f, -0.25f);
            Vector3 leftBackBottom = new Vector3(-0.25f, -0.354f, -0.25f);

            Vector3 rightBackTop = new Vector3(0.25f, 0.354f, -0.25f);
            Vector3 rightBackBottom = new Vector3(0.25f, -0.354f, -0.25f);

            return vertices;
        }

        public static short[] GetSphereIndices()
        {
            short[] indices = new short[200];

            //indices[0] = 

            return indices;
        }

        //See: https://gamedev.stackexchange.com/questions/44755/xna-shield-effect-with-a-primative-sphere-problem
        //Acessed: January 2020
        public static VertexPositionColorTexture[] GetPositionNormalTextureSphereVertices(float radius, int slices, out PrimitiveType primitiveType, out int primitiveCount)
        {
            return GetPositionNormalTextureSphereVerticesAndIndices(radius, slices, out primitiveType, out primitiveCount)[0] as VertexPositionColorTexture[];
        }

        public static short[] GetSphereIndices(float radius, int slices)
        {
            return GetPositionNormalTextureSphereVerticesAndIndices(radius, slices, out PrimitiveType primitiveType, out int primitiveCount)[1] as short[];
        }

        private static object[] GetPositionNormalTextureSphereVerticesAndIndices(float radius, int slices, out PrimitiveType primitiveType, out int primitiveCount)
        {
            int numberOfVertices = (slices + 1) * (slices + 1);
            int numberOfIndices = 6 * slices * (slices + 1);

            short[] indices = new short[numberOfIndices];
            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[numberOfVertices];

            float thetaStep = MathHelper.Pi / slices;
            float phiStep = MathHelper.TwoPi / slices;

            int iIndex = 0;
            int iVertex = 0;
            int iVertex2 = 0;

            for (int sliceTheta = 0; sliceTheta < slices + 1; sliceTheta++)
            {
                float r = (float)Math.Sin(sliceTheta * thetaStep);
                float y = (float)Math.Cos(sliceTheta * thetaStep);

                for (int slicePhi = 0; slicePhi < (slices + 1); slicePhi++)
                {
                    float x = r * (float)Math.Sin(slicePhi * phiStep);
                    float z = r * (float)Math.Cos(slicePhi * phiStep);

                    vertices[iVertex].Position = new Vector3(x, y, z) * radius;
                    vertices[iVertex].Color = Color.White;
                    vertices[iVertex].TextureCoordinate = new Vector2((float)slicePhi / slices, (float)sliceTheta / slices);
                    iVertex++;

                    if (sliceTheta != (slices - 1))
                    {
                        indices[iIndex++] = (short) (iVertex2 + (slices + 1));
                        indices[iIndex++] = (short) (iVertex2 + 1);
                        indices[iIndex++] = (short) (iVertex2);
                        indices[iIndex++] = (short) (iVertex2 + (slices));
                        indices[iIndex++] = (short) (iVertex2 + (slices + 1));
                        indices[iIndex++] = (short) (iVertex2);
                        iVertex2++;
                    }
                }
            }

            primitiveType = PrimitiveType.TriangleList;
            primitiveCount = numberOfIndices / 3;

            return new object[] { vertices, indices };
        }
        //End of reference

        //Returns the vertices for a billboard which has a custom vertex declaration
        public static VertexBillboard[] GetVertexBillboard(int sidelength, out PrimitiveType primitiveType, out int primitiveCount)
        {
            primitiveType = PrimitiveType.TriangleStrip;
            primitiveCount = 2;

            VertexBillboard[] vertices = new VertexBillboard[4];
            float halfSideLength = sidelength / 2.0f;

            Vector2 uvTopLeft = new Vector2(0, 0);
            Vector2 uvTopRight = new Vector2(1, 0);
            Vector2 uvBottomLeft = new Vector2(0, 1);
            Vector2 uvBottomRight = new Vector2(1, 1);

            //Quad coplanar with the XY-plane (i.e. forward facing normal along UnitZ)
            vertices[0] = new VertexBillboard(Vector3.Zero, new Vector4(uvTopLeft, -halfSideLength, halfSideLength));
            vertices[1] = new VertexBillboard(Vector3.Zero, new Vector4(uvTopRight, halfSideLength, halfSideLength));
            vertices[2] = new VertexBillboard(Vector3.Zero, new Vector4(uvBottomLeft, -halfSideLength, -halfSideLength));
            vertices[3] = new VertexBillboard(Vector3.Zero, new Vector4(uvBottomRight, halfSideLength, -halfSideLength));

            return vertices;
        }
    }
}