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

            XSensitivity = 0.001;
            Up = Vector3D.UnitY;
            Right = Vector3D.UnitX;

            MatrixD orient = MatrixD.Identity;
            orient.Forward = -Vector3D.UnitZ;
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
        /// Default value is 0.001
        /// </summary>
        public double XSensitivity { get; set; }

        public Vector3 Position { get; set; }
        public Vector3D PositionD { get; set; }
        public Vector3D LookAt { get; set; }
        public Vector3D Up { get; set; }
        public Vector3D Right { get; set; }

        public double Yaw { get; set; }
        public double Pitch { get; set; }
        public double Roll { get; set; }

        public double NearPlane { get; set; }
        public double FarPlane { get; set; }
        public double FieldOfView { get; set; }
        public double AspectRatio { get; set; }

        public MatrixD ViewD { get; set; }
        public MatrixD ProjD { get; set; }

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

            ViewD = MatrixD.LookAtLH(PositionD, PositionD + LookAt, Up);
            ProjD = MatrixD.PerspectiveFovLH(FieldOfView, context.AspectRatio, NearPlane, FarPlane);
            Frustum = new BoundingFrustum(ViewD.ToMatrix() * ProjD.ToMatrix()); // loss of precision
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
            
            MatrixD rot = MatrixD.RotationYawPitchRoll(this.Yaw, this.Pitch, this.Roll);
            this.LookAt = Vector3D.TransformCoordinate(Vector3D.UnitZ, rot);
            this.Right = Vector3D.TransformCoordinate(Vector3D.UnitX, rot);
            this.Up = Vector3D.Cross(this.Right, this.LookAt);

            this.LookAt.Normalize();
            this.Right.Normalize();
            this.Up.Normalize();

            Vector2 l = currentStates.Gamepad.LStick();

            float rtrigger = currentStates.Gamepad.RTrigger() == 0 ? 1f : currentStates.Gamepad.RTrigger();

            Vector3D force = this.LookAt * l.Y * 20.0; // thrust
            force -= this.Right * l.X * 20.0;

            force *= dt * 0.1f * rtrigger;

            this.PositionD += force * 0.01;
            this.Position = this.PositionD.ToVector3(); 
        }
    }
}
