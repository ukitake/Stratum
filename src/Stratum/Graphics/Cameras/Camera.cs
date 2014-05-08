using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;
using Stratum.Graphics;
using Stratum.Input;

namespace Stratum
{
    public class Camera
    {
        public Camera()
        {

        }

        public Camera(IGraphicsContext context, float fov, float aspectRatio, float nearPlane, float farPlane)
        {
            this.context = context;

            NearPlane = nearPlane;
            FarPlane = farPlane;
            FieldOfView = fov;
            AspectRatio = aspectRatio;

            XSensitivity = 0.001f;
            Up = Vector3.Up;
            Right = Vector3.Right;

            Matrix orient = Matrix.Identity;
            orient.Forward = -Vector3.UnitZ;
            orient.Right = this.Right;
            orient.Up = this.Up;
        }

        public Camera(IGraphicsContext context, Vector3 position, float fov, float aspectRatio, float nearPlane, float farPlane)
            : this(context, fov, aspectRatio, nearPlane, farPlane)
        {
            this.Position = position;
            this.PositionD = new Vector3D(position);
        }

        IGraphicsContext context;

        /// <summary>
        /// Default value is 0.001f
        /// </summary>
        public float XSensitivity { get; set; }

        public Vector3 Position { get; set; }
        public Vector3D PositionD { get; set; }
        public Vector3 LookAt { get; set; }
        public Vector3 Up { get; set; }
        public Vector3 Right { get; set; }

        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }

        public float NearPlane { get; set; }
        public float FarPlane { get; set; }
        public float FieldOfView { get; set; }
        public float AspectRatio { get; set; }

        public Matrix View { get; set; }
        public Matrix Proj { get; set; }

        public BoundingFrustum Frustum { get; set; }

        public virtual void Update(GameTime gameTime)
        {
            HandleInput(gameTime);

            //float distance = Position.Length();
            //NearPlane = Math.Max(distance - 6360, 1f);
            //if (NearPlane == 1f)
            //{
            //    FarPlane = 4000f;
            //}
            //else
            //{
            //    FarPlane = distance + 7000f;
            //}

            NearPlane = 1;
            FarPlane = 12000;

            View = Matrix.LookAtLH(Position, Position + LookAt, Up);
            Proj = Matrix.PerspectiveFovLH(FieldOfView, context.AspectRatio, NearPlane, FarPlane);
            Frustum = new BoundingFrustum(View * Proj);
        }

        protected virtual void HandleInput(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            IInputContext currentStates = Engine.InputContext;

            Vector2 r = currentStates.Gamepad.RStick();

            Vector2 orientationDelta = new Vector2();
            orientationDelta.X = r.X;
            orientationDelta.Y = -r.Y;

            this.Pitch -= dt * orientationDelta.Y * XSensitivity * 2f;
            this.Yaw += dt * orientationDelta.X * XSensitivity * 2f;
            
            Matrix rot = Matrix.RotationYawPitchRoll(this.Yaw, this.Pitch, this.Roll);
            this.LookAt = Vector3.TransformCoordinate(Vector3.ForwardRH, rot);
            this.Right = Vector3.TransformCoordinate(Vector3.Right, rot);
            this.Up = Vector3.Cross(this.Right, this.LookAt);

            this.LookAt.Normalize();
            this.Right.Normalize();
            this.Up.Normalize();

            Vector2 l = currentStates.Gamepad.LStick();

            float rtrigger = currentStates.Gamepad.RTrigger() == 0 ? 1f : currentStates.Gamepad.RTrigger();

            Vector3D force = new Vector3D(this.LookAt) * l.Y * 20.0; // thrust
            force -= new Vector3D(this.Right) * l.X * 20.0;

            force *= dt * 0.1f * rtrigger;

            this.PositionD += force * 0.01;
            this.Position = this.PositionD.ToVector3(); 
        }
    }
}
