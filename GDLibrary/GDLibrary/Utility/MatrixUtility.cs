using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class MatrixUtility
    {
        public static Matrix CalculateRotationMatrix(GameTime gameTime, Axis axisOfRotation, float angleOfRotation)
        {
            switch (axisOfRotation)
            {
                case Axis.PosX:
                    return Matrix.CreateRotationX(MathHelper.ToRadians(angleOfRotation) * (float)gameTime.ElapsedGameTime.Milliseconds);

                case Axis.PosY:
                    return Matrix.CreateRotationY(MathHelper.ToRadians(angleOfRotation) * (float)gameTime.ElapsedGameTime.Milliseconds);

                case Axis.PosZ:
                    return Matrix.CreateRotationZ(MathHelper.ToRadians(angleOfRotation) * (float)gameTime.ElapsedGameTime.Milliseconds);

                case Axis.NegX:
                    return Matrix.CreateRotationX(MathHelper.ToRadians(-angleOfRotation) * (float)gameTime.ElapsedGameTime.Milliseconds);

                case Axis.NegY:
                    return Matrix.CreateRotationY(MathHelper.ToRadians(-angleOfRotation) * (float)gameTime.ElapsedGameTime.Milliseconds);

                case Axis.NegZ:
                    return Matrix.CreateRotationZ(MathHelper.ToRadians(-angleOfRotation) * (float)gameTime.ElapsedGameTime.Milliseconds);
            }

            return Matrix.Identity;
        }

        //Translates vector to origin, rotates vector by 90 degrees, translates vector back to orbit point
        public static Vector3 CalculateTargetPositionVector(Axis axisOfRotation, float angleOfRotation, Vector3 currentPosition, Vector3 pointOfRotation)
        {
            switch (axisOfRotation)
            {
                case Axis.PosX:
                    return Vector3.Transform((currentPosition - pointOfRotation), Matrix.CreateRotationX(MathHelper.ToRadians(angleOfRotation))) + pointOfRotation;

                case Axis.PosY:
                    return Vector3.Transform((currentPosition - pointOfRotation), Matrix.CreateRotationY(MathHelper.ToRadians(angleOfRotation))) + pointOfRotation;

                case Axis.PosZ:
                    return Vector3.Transform((currentPosition - pointOfRotation), Matrix.CreateRotationZ(MathHelper.ToRadians(angleOfRotation))) + pointOfRotation;

                case Axis.NegX:
                    return Vector3.Transform((currentPosition - pointOfRotation), Matrix.CreateRotationX(MathHelper.ToRadians(-angleOfRotation))) + pointOfRotation;

                case Axis.NegY:
                    return Vector3.Transform((currentPosition - pointOfRotation), Matrix.CreateRotationY(MathHelper.ToRadians(-angleOfRotation))) + pointOfRotation;

                case Axis.NegZ:
                    return Vector3.Transform((currentPosition - pointOfRotation), Matrix.CreateRotationZ(MathHelper.ToRadians(-angleOfRotation))) + pointOfRotation;
            }

            return Vector3.Zero;
        }

        public static Vector3 CalculateTargetLookVector(Axis axisOfRotation, float angleOfRotation, Vector3 currentLook)
        {
            switch (axisOfRotation)
            {
                case Axis.PosX:
                    return Vector3.Transform(currentLook, Matrix.CreateRotationX(MathHelper.ToRadians(angleOfRotation)));

                case Axis.PosY:
                    return Vector3.Transform(currentLook, Matrix.CreateRotationY(MathHelper.ToRadians(angleOfRotation)));

                case Axis.PosZ:
                    return Vector3.Transform(currentLook, Matrix.CreateRotationZ(MathHelper.ToRadians(angleOfRotation)));

                case Axis.NegX:
                    return Vector3.Transform(currentLook, Matrix.CreateRotationX(MathHelper.ToRadians(-angleOfRotation)));

                case Axis.NegY:
                    return Vector3.Transform(currentLook, Matrix.CreateRotationY(MathHelper.ToRadians(-angleOfRotation)));

                case Axis.NegZ:
                    return Vector3.Transform(currentLook, Matrix.CreateRotationZ(MathHelper.ToRadians(-angleOfRotation)));
            }

            return Vector3.Zero;
        }

        public static Vector3 CalculateTargetUpVector(Axis axisOfRotation, float angleOfRotation, Vector3 currentUp)
        {
            switch (axisOfRotation)
            {
                case Axis.PosX:
                    return Vector3.Transform(currentUp, Matrix.CreateRotationX(MathHelper.ToRadians(angleOfRotation)));

                case Axis.PosY:
                    return Vector3.Transform(currentUp, Matrix.CreateRotationY(MathHelper.ToRadians(angleOfRotation)));

                case Axis.PosZ:
                    return Vector3.Transform(currentUp, Matrix.CreateRotationZ(MathHelper.ToRadians(angleOfRotation)));

                case Axis.NegX:
                    return Vector3.Transform(currentUp, Matrix.CreateRotationX(MathHelper.ToRadians(-angleOfRotation)));

                case Axis.NegY:
                    return Vector3.Transform(currentUp, Matrix.CreateRotationY(MathHelper.ToRadians(-angleOfRotation)));

                case Axis.NegZ:
                    return Vector3.Transform(currentUp, Matrix.CreateRotationZ(MathHelper.ToRadians(-angleOfRotation)));
            }

            return Vector3.Zero;
        }
    }
}