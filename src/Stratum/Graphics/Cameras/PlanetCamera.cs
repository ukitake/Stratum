using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpDX;
using SharpDX.DirectInput;
using SharpDX.Toolkit;
using Stratum.Graphics;
using Stratum.Input;

namespace Stratum
{
    public class PlanetCamera : Camera
    {
        private double R;
        private double zoom;
        private float fov;

        public PlanetCamera(double R)
        {
            this.R = R;
            this._h = 6000f;
            this.zoom = 0f;
            this.fov = 80;
            Longitude = 180;
            Pitch = -MathUtil.PiOverTwo;
            Yaw = 0;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        private double _h;

        public double HeightAboveGround { get { return Position.Length() - R; } }

        public static Vector3D fromLatLon(double lon, double lat)
        {
            Vector3D ret = new Vector3D();
            ret.X = 6359.99 * Math.Cos(MathUtilD.DegreesToRadians(lat)) * Math.Cos(MathUtilD.DegreesToRadians(lon));
            ret.Y = 6359.99 * Math.Sin(MathUtilD.DegreesToRadians(lat));
            ret.Z = 6359.99 * Math.Cos(MathUtilD.DegreesToRadians(lat)) * Math.Sin(MathUtilD.DegreesToRadians(lon));
            return ret;
        }

        public override void Update(GameTime gameTime)
        {
            HandleInput(gameTime);

            Vector3D pos = fromLatLon(Longitude, Latitude);
            pos.Normalize();
            Vector3D N = pos;
            N.Normalize();

            if (_h < 0.001f)
                _h = 0.001f;

            pos = pos * (R + _h);

            Position = pos.ToVector3();

            Vector3D right = Vector3D.Cross(Vector3D.UnitY, N);
            Vector3D look = Vector3D.Cross(N, right);
            look.Normalize();
            Vector3D up = N;

            var pitchMat = Quaternion.RotationAxis(right.ToVector3(), Pitch);
            var yawMat = Quaternion.RotationAxis(up.ToVector3(), Yaw);
            var yawPitchMat = Quaternion.Multiply(yawMat, pitchMat);

            Vector3D rotLook = Vector3D.Transform(look, yawPitchMat);
            Vector3D rotUp = Vector3D.Transform(up, yawPitchMat);

            var view = MatrixD.LookAtLH(pos, pos + rotLook, rotUp);
            View = view.ToMatrix();
            var graphicsContext = Engine.GraphicsContext;
            ViewportF vp;
            vp = graphicsContext.Device.Viewport;
            float width = (float)vp.Width;
            float height = (float)vp.Height;
            float vfov = MathUtil.RadiansToDegrees(2 * (float)Math.Atan(height / width * Math.Tan(MathUtil.DegreesToRadians((float)fov / 2))));

            float znear, zfar;
            float h = (float)HeightAboveGround;// - TerrainNode.groundHeightAtCamera);

            if (Pitch > -MathUtil.PiOverTwo)
            {
                znear = h * 0.5f;
            }
            else 
            {
                znear = h - 0.1f;
            }
            
            //if (zfar == 0.0f)
            {
                if (h < 1000)
                {
                    zfar = h + 2000;
                }
                else
                {
                    zfar = h + 6000;
                }
            }

            //if (zoom > 1.0)
            //{
            //    vfov = MathUtil.RadiansToDegrees(2f * (float)Math.Atan(height / width * Math.Tan(MathUtil.DegreesToRadians((float)fov / 2f)) / zoom));
            //    znear = (float)(R * zoom * Math.Max(1.0 - 10.0 * Math.Tan(MathUtil.DegreesToRadians((float)fov / 2)) / zoom, 0.1));
            //    zfar = (float)(R * zoom * Math.Min(1.0 + 10.0 * Math.Tan(MathUtil.DegreesToRadians((float)fov / 2)) / zoom, 10.0));
            //}

            //MatrixD clip = MatrixD.OrthoOffCenterRH(viewport.Y, viewport.X, viewport.W, viewport.Z, 1.0f, -1.0f);
            MatrixD cameraToScreen = MatrixD.PerspectiveFovLH(MathUtil.PiOverFour, width / height, znear, zfar);

            Proj = cameraToScreen.ToMatrix();
            Frustum = new BoundingFrustum((view * cameraToScreen).ToMatrix());
            NearPlane = znear;
            FarPlane = zfar;
            FieldOfView = vfov;
        }

        private Vector2 previousPoint;

        protected override void HandleInput(GameTime gameTime)
        {
            IInputContext input = Engine.InputContext;
            MouseContext mouse = input.Mouse;
            KeyboardContext key = input.Keyboard;

            if (mouse.WheelDelta != 0)
            {
                _h += -mouse.WheelDelta * gameTime.ElapsedGameTime.TotalSeconds * _h * 0.0843f;
            }

            if (mouse.WasLeftButtonPressed)
            {
                // start drag
                previousPoint = mouse.Location;
            }

            if (mouse.IsLeftButtonDown && !mouse.IsRightButtonDown)
            {
                // dragging
                float diffX = mouse.Location.X - previousPoint.X;
                float diffY = mouse.Location.Y - previousPoint.Y;

                Longitude -= diffX * 0.016f * HeightAboveGround * 2f;
                if (Longitude > 180)
                {
                    Longitude = -180 + (Longitude - 180);
                }
                if (Longitude < -180)
                {
                    Longitude = 180 + (Longitude - -180);
                }

                Latitude += diffY * 0.016f * HeightAboveGround * 2f;
                if (Latitude > 85)
                {
                    Latitude = 85;
                }
                if (Latitude < -85)
                {
                    Latitude = -85;
                }

                previousPoint = mouse.Location;
            }
            else if (!mouse.IsRightButtonDown)
            {
                previousPoint = Vector2.Zero;
            }

            if (mouse.WasRightButtonPressed)
            {
                previousPoint = mouse.Location;
            }

            if (mouse.IsRightButtonDown && !mouse.IsLeftButtonDown)
            {
                // dragging
                float diffX = mouse.Location.X - previousPoint.X;
                float diffY = mouse.Location.Y - previousPoint.Y;

                Rotate(diffX, diffY);

                previousPoint = mouse.Location;
            }
            else if (!mouse.IsLeftButtonDown)
            {
                previousPoint = Vector2.Zero;
            }
        }

        public void Rotate(float headingRadians, float pitchRadians)
        {
            Pitch += pitchRadians;

            if (Pitch > MathUtil.PiOverTwo)
            {
                pitchRadians = MathUtil.PiOverTwo - (Pitch - pitchRadians);
                Pitch = MathUtil.PiOverTwo;
            }

            if (Pitch < -MathUtil.PiOverTwo)
            {
                pitchRadians = -MathUtil.PiOverTwo - (Pitch - pitchRadians);
                Pitch = -MathUtil.PiOverTwo;
            }

            Yaw += headingRadians * 1.5f;

            if (Yaw > MathUtil.Pi * 2)
                Yaw -= MathUtil.Pi * 2;

            if (Yaw < -MathUtil.Pi * 2)
                Yaw += MathUtil.Pi * 2;
        }
    }
}
