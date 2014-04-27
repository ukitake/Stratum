using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using Stratum.Graphics;

namespace Stratum
{
    public class TerrainViewController
    {
        public double fov;
        public double x0;
        public double y0;
        public double theta;
        public double phi;
        public double d;
        public double zoom;
        public Vector3D position;

        protected float groundHeight;
        public float GroundHeight
        {
            get { return groundHeight; }
            set { groundHeight = value; }
        }

        protected SceneNode node;
        public SceneNode Node
        {
            get { return node; }
            set { node = value; }
        }

        public TerrainViewController(SceneNode sceneNode, double d0)
        {
            this.node = sceneNode;
            fov = 80.0;
            x0 = 0f;
            y0 = 0f;
            theta = 0.0;
            phi = 0.0;
            d = d0;
            zoom = 1.0;
            groundHeight = 0f;
        }

        public virtual double getHeight()
        {
            return position.Z;
        }

        public virtual void move(Vector3D oldP, Vector3D newP)
        {
            x0 -= newP.X - oldP.X;
            y0 -= newP.Y - oldP.Y;
        }

        public virtual void moveForward(double distance)
        {
            x0 -= Math.Sin(phi) * distance;
            y0 += Math.Cos(phi) * distance;
        }

        public virtual void turn(double angle)
        {
            double l = d * Math.Sin(theta);
            x0 -= (Math.Sin(phi) * (Math.Cos(angle) - 1.0) + Math.Cos(phi) * Math.Sin(angle)) * l;
            y0 += (Math.Cos(phi) * (Math.Cos(angle) - 1.0) - Math.Sin(phi) * Math.Sin(angle)) * l;
            phi += angle;
        }

        public virtual double interpolate(double sx0, double sy0, double stheta, double sphi, double sd,
                                          double dx0, double dy0, double dtheta, double dphi, double dd, double t)
        {
            // TODO interpolation
            x0 = dx0;
            y0 = dy0;
            theta = dtheta;
            phi = dphi;
            d = dd;
            return 1.0;
        }

        public virtual void interpolatePos(double sx0, double sy0, double dx0, double dy0, double t, ref double x0, ref double y0)
        {
            x0 = sx0 * (1.0 - t) + dx0 * t;
            y0 = sy0 * (1.0 - t) + dy0 * t;
        }

        public void interpolateDirection(double slon, double slat, double elon, double elat, double t, ref double lon, ref double lat)
        {
            Vector3D s = new Vector3D((Math.Cos(slon) * Math.Cos(slat)), (Math.Sin(slon) * Math.Cos(slat)), Math.Sin(slat));
            Vector3D e = new Vector3D((Math.Cos(elon) * Math.Cos(elat)), (Math.Sin(elon) * Math.Cos(elat)), Math.Sin(elat));
            Vector3D v = Vector3D.Add(s * (1d - t), e * t);
            v.Normalize();
            lat = Math.Asin(v.Z);
            lon = Math.Atan2(v.Y, v.X);
        }

        public virtual void update()
        {
            Vector3D po = new Vector3D(x0, y0, groundHeight);
            Vector3D px = new Vector3D(1.0, 0.0, 0.0);
            Vector3D py = new Vector3D(0.0, 1.0, 0.0);
            Vector3D pz = new Vector3D(0.0, 0.0, 1.0);

            double ct = Math.Cos(theta);
            double st = Math.Sin(theta);
            double cp = Math.Cos(phi);
            double sp = Math.Sin(phi);
            Vector3D cx = px * cp + py * sp;
            Vector3D cy = -px * sp * ct + py * cp * ct + pz * st;
            Vector3D cz = px * sp * st - py * cp * st + pz * ct;
            position = po + cz * d * zoom;

            if (position.Z < groundHeight + 1.0)
            {
                position.Z = groundHeight + 1.0;
            }

            MatrixD view = new MatrixD(cx.X, cx.Y, cx.Z, 0.0,
                    cy.X, cy.Y, cy.Z, 0.0,
                    cz.X, cz.Y, cz.Z, 0.0,
                    0.0, 0.0, 0.0, 1.0);

            view = view * MatrixD.Translation(-position);

            node.Local = view;
        }

        public virtual void setProjection(float znear, float zfar, Vector4 viewport)
        {
            var graphicsContext = Engine.GraphicsContext;
            var vp = graphicsContext.Device.Viewport;
            float width = (float)vp.Width;
            float height = (float)vp.Height;
            float vfov = MathUtil.RadiansToDegrees(2 * (float)Math.Atan(height / width * Math.Tan(MathUtil.DegreesToRadians((float)fov / 2))));

            float h = (float)(getHeight());// - TerrainNode.groundHeightAtCamera);
            if (znear == 0.0f)
            {
                znear = 0.1f * h;
            }
            if (zfar == 0.0f)
            {
                zfar = 1e6f * h;
            }

            if (zoom > 1.0)
            {
                vfov = MathUtil.RadiansToDegrees(2f * (float)Math.Atan(height / width * Math.Tan(MathUtil.DegreesToRadians((float)fov / 2f)) / zoom));
                znear = (float)(d * zoom * Math.Max(1.0 - 10.0 * Math.Tan(MathUtil.DegreesToRadians((float)fov / 2)) / zoom, 0.1));
                zfar = (float)(d * zoom * Math.Min(1.0 + 10.0 * Math.Tan(MathUtil.DegreesToRadians((float)fov / 2)) / zoom, 10.0));
            }

            MatrixD clip = MatrixD.OrthoOffCenterRH(viewport.Y, viewport.X, viewport.W, viewport.Z, 1.0f, -1.0f);
            MatrixD cameraToScreen = MatrixD.PerspectiveFovRH(vfov, width / height, znear, zfar);

            graphicsContext.CurrentCamera.Proj = (clip * cameraToScreen).ToMatrix();
        }
    }
}
