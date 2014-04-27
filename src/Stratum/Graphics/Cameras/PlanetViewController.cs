using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stratum
{
    public class PlanetViewController : TerrainViewController
    {
        public PlanetViewController(SceneNode node, double R)
            : base(node, R * 6.0)
        {
            this.R = R;
        }

        public double R { get; set; }

        public override double getHeight()
        {
            return position.Length() - R;
        }

        public override void move(Vector3D oldP, Vector3D newP)
        {
            oldP.Normalize();
            newP.Normalize();
            double oldlat = Math.Asin(oldP.Y); // safe_asin(oldp.z);
            double oldlon = Math.Atan2(oldP.Z, oldP.X); // atan2(oldp.y, oldp.x);
            double lat = Math.Asin(newP.Y);
            double lon = Math.Atan2(newP.Y, newP.X);
            x0 -= lon - oldlon;
            y0 -= lat - oldlat;
            y0 = Math.Max(-Math.PI / 2.0, Math.Min(Math.PI / 2.0, y0));
        }

        public override void moveForward(double distance)
        {
            double co = Math.Cos(x0); // x0 = longitude
            double so = Math.Sin(x0);
            double ca = Math.Cos(y0); // y0 = latitude
            double sa = Math.Sin(y0);
            Vector3D po = new Vector3D(co * ca, so * ca, sa) * R;
            Vector3D px = new Vector3D(-so, co, 0.0);
            Vector3D py = new Vector3D(-co * sa, -so * sa, ca);
            Vector3D pd = (po - px * Math.Sin(phi) * distance + py * Math.Cos(phi) * distance);
            pd.Normalize();
            x0 = Math.Atan2(pd.Z, pd.X);
            y0 = Math.Asin(pd.Y);
        }

        public override void turn(double angle)
        {
            double co = Math.Cos(x0); // x0 = longitude
            double so = Math.Sin(x0);
            double ca = Math.Cos(y0); // y0 = latitude
            double sa = Math.Sin(y0);
            double l = d * Math.Sin(theta);
            Vector3D po = new Vector3D(co * ca, so * ca, sa) * R;
            Vector3D px = new Vector3D(-so, co, 0.0);
            Vector3D py = new Vector3D(-co * sa, -so * sa, ca);
            Vector3D f = -px * Math.Sin(phi) + py * Math.Cos(phi);
            Vector3D r = px * Math.Cos(phi) + py * Math.Sin(phi);
            Vector3D pd = (po + f * (Math.Cos(angle) - 1.0) * l - r * Math.Sin(angle) * l);
            pd.Normalize();
            x0 = Math.Atan2(pd.Z, pd.X);
            y0 = Math.Asin(pd.Y);
            phi += angle;
        }

        public override double interpolate(double sx0, double sy0, double stheta, double sphi, double sd, double dx0, double dy0, double dtheta, double dphi, double dd, double t)
        {
            Vector3D s = new Vector3D(Math.Cos(sx0) * Math.Cos(sy0), Math.Sin(sx0) * Math.Cos(sy0), Math.Sin(sy0));
            Vector3D e = new Vector3D(Math.Cos(dx0) * Math.Cos(dy0), Math.Sin(dx0) * Math.Cos(dy0), Math.Sin(dy0));
            double dist = Math.Max(Math.Acos(Vector3D.Dot(s, e)) * R, 1e-3);

            t = Math.Min(t + Math.Min(0.1, 5000.0 / dist), 1.0);
            double T = 0.5 * Math.Atan(4.0 * (t - 0.5)) / Math.Atan(4.0 * 0.5) + 0.5;

            interpolateDirection(sx0, sy0, dx0, dy0, T, ref x0, ref y0);
            interpolateDirection(sphi, stheta, dphi, dtheta, T, ref phi, ref theta);

            double W = 10.0;
            d = sd * (1.0 - t) + dd * t + dist * (Math.Exp(-W * (t - 0.5) * (t - 0.5)) - Math.Exp(-W * 0.25));

            return t;
        }

        public override void interpolatePos(double sx0, double sy0, double dx0, double dy0, double t, ref double x0, ref double y0)
        {
            interpolateDirection(sx0, sy0, dx0, dy0, t, ref x0, ref y0);
        }

        public override void update()
        {
            double co = Math.Cos(x0); // x0 = longitude
            double so = Math.Sin(x0);
            double ca = Math.Cos(y0); // y0 = latitude
            double sa = Math.Sin(y0);
            Vector3D po = new Vector3D(co*ca, so*ca, sa) * (R + groundHeight);
            Vector3D px = new Vector3D(-so, co, 0.0);
            Vector3D py = new Vector3D(-co*sa, -so*sa, ca);
            Vector3D pz = new Vector3D(co*ca, so*ca, sa);

            double ct = Math.Cos(theta);
            double st = Math.Sin(theta);
            double cp = Math.Cos(phi);
            double sp = Math.Sin(phi);
            Vector3D cx = px * cp + py * sp;
            Vector3D cy = -px * sp*ct + py * cp*ct + pz * st;
            Vector3D cz = px * sp*st - py * cp*st + pz * ct;
            position = po + cz * d * zoom;

            if (position.Length() < R + 0.5 + groundHeight) 
            {
                position.Normalize();
                position *= R + 0.5 + groundHeight;
                //position = position.normalize(R + 0.5 + groundHeight);
           } 

            MatrixD view = new MatrixD(cx.X, cx.Y, cx.Z, 0.0,
                    cy.X, cy.Y, cy.Z, 0.0,
                    cz.X, cz.Y, cz.Z, 0.0,
                    0.0, 0.0, 0.0, 1.0);

            view = view * MatrixD.Translation(-position);
            
            node.Local = view;
        }
    }
}
