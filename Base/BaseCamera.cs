using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace gameProject
{
    public class BaseCamera
    {
        public Vector3 Position = new Vector3(0, 0, 500);
        public Quaternion Rotation;
        public Matrix View { get; protected set; }
        public Matrix Projection { get; protected set; }
        public Matrix ViewProjection
        {
            get
            {
                return Matrix.Multiply(View, Projection);
            }

        }

        public Matrix InverseView
        {
            get
            {
                return Matrix.Invert(View);
            }

        }
            

        private Vector3 UpVec = Vector3.Up;

        public BaseCamera(RenderContext renderContext)
        {
            float aspectRatio = renderContext.GraphicsDevice.Viewport.AspectRatio;

            //Projection = Matrix.CreateOrthographic(1280, 720, 0.1f, 300f);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90.0f),
                aspectRatio, 1.0f, 1500f);           
        }

        public void SetUpVector(Vector3 up)
        {
            UpVec = up;
        }

        public virtual void BuildViewMatrix()
        {
            var lookAt = Vector3.Transform(Vector3.Forward, Rotation);
            lookAt.Normalize(); 

            View = Matrix.CreateLookAt(Position, (Position + lookAt), UpVec);
        }

        public void Update(RenderContext renderContext, Vector2 playerPos)
        {
            BuildViewMatrix();

            // Camera interpolates between current position and player position (smooth follow)

            Vector2 currentPos = new Vector2(Position.X, Position.Y);
            float lerpAmount = 0.05f;   // Adjust for faster / smoother following
            var lerpedPos = Vector2.Lerp(currentPos, playerPos, lerpAmount);
            Position = new Vector3(lerpedPos, Position.Z);
        }
    }
}
