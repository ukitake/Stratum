using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Stratum.CompGeom
{
    public static class ClosestPoint
    {
        #region [ Triangle ]

        // finds closest point on triangle defined by [a,b,c] to point [p]
        public static Vector3 Find(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 ab = b - a;
            Vector3 ac = c - a;
            Vector3 bc = c - b;

            // Compute parametric position s for projection P' of P on AB,
            // P' = A + s*AB, s = snom/(snom+sdenom)
            float snom = Vector3.Dot(p - a, ab), sdenom = Vector3.Dot(p - b, a - b);

            // Compute parametric position t for projection P' of P on AC,
            // P' = A + t*AC, s = tnom/(tnom+tdenom)
            float tnom = Vector3.Dot(p - a, ac), tdenom = Vector3.Dot(p - c, a - c);

            if (snom <= 0.0f && tnom <= 0.0f) return a; // Vertex region early out

            // Compute parametric position u for projection P' of P on BC,
            // P' = B + u*BC, u = unom/(unom+udenom)
            float unom = Vector3.Dot(p - b, bc), udenom = Vector3.Dot(p - c, b - c);

            if (sdenom <= 0.0f && unom <= 0.0f) return b; // Vertex region early out
            if (tdenom <= 0.0f && udenom <= 0.0f) return c; // Vertex region early out


            // P is outside (or on) AB if the triple scalar product [N PA PB] <= 0
            Vector3 n = Vector3.Cross(b - a, c - a);
            float vc = Vector3.Dot(n, Vector3.Cross(a - p, b - p));
            // If P outside AB and within feature region of AB,
            // return projection of P onto AB
            if (vc <= 0.0f && snom >= 0.0f && sdenom >= 0.0f)
                return a + snom / (snom + sdenom) * ab;

            // P is outside (or on) BC if the triple scalar product [N PB PC] <= 0
            float va = Vector3.Dot(n, Vector3.Cross(b - p, c - p));
            // If P outside BC and within feature region of BC,
            // return projection of P onto BC
            if (va <= 0.0f && unom >= 0.0f && udenom >= 0.0f)
                return b + unom / (unom + udenom) * bc;

            // P is outside (or on) CA if the triple scalar product [N PC PA] <= 0
            float vb = Vector3.Dot(n, Vector3.Cross(c - p, a - p));
            // If P outside CA and within feature region of CA,
            // return projection of P onto CA
            if (vb <= 0.0f && tnom >= 0.0f && tdenom >= 0.0f)
                return a + tnom / (tnom + tdenom) * ac;

            // P must project inside face region. Compute Q using barycentric coordinates
            float u = va / (va + vb + vc);
            float v = vb / (va + vb + vc);
            float w = 1.0f - u - v; // = vc / (va + vb + vc)
            return u * a + v * b + w * c;
        }

        #endregion
    }

    /// <summary>
    /// This exists to keep the implementations of polymorphic intersection tests DRY
    /// </summary>
    public static class IntersectionTests
    {
        #region [ LineSegment ]

        public static bool Test(LineSegment seg, AxisAlignedBoundingBox bb)
        {
            Vector3 c = (bb.Min + bb.Max) * 0.5f;     // box center point
            Vector3 e = bb.Max - c;                      // box halflength extents
            Vector3 m = (seg.Point1 + seg.Point2) * 0.5f;           // segment midpoint
            Vector3 d = seg.Point1 - m;                         // segment halflength vector
            m = m - c;                                      // translate box and segment to origin

            // try world coordinate axes as seperating axes
            float adx = Math.Abs(d.X);
            if (Math.Abs(m.X) > e.X + adx)
                return false;
            float ady = Math.Abs(d.Y);
            if (Math.Abs(m.Y) > e.Y + ady)
                return false;
            float adz = Math.Abs(d.Z);
            if (Math.Abs(m.Z) > e.Z + adz)
                return false;

            // add in an epsilon term to counteract arithmetic errors when segment is (near) parallel to a coordinate axis
            adx += FPPrecisionHelper.EPSILON; ady += FPPrecisionHelper.EPSILON; adz += FPPrecisionHelper.EPSILON;
            
            // try cross products of segment direction vector with coordinate axes
            if (Math.Abs(m.Y * d.Z - m.Z * d.Y) > e.Y * adz + e.Z * ady)
                return false;
            if (Math.Abs(m.Z * d.X - m.X * d.Z) > e.X * adz + e.Z * adx)
                return false;
            if (Math.Abs(m.X * d.Y - m.Y * d.X) > e.X * ady + e.Y * adx)
                return false;

            // no separating axis found; segment must be overlapping AABB
            return true;
        }

        public static bool Test(LineSegment seg, BoundingFrustum frus)
        {
            ContainmentType ct1 = frus.Contains(seg.Point1);
            ContainmentType ct2 = frus.Contains(seg.Point2);
            return ct1 != ct2;
        }

        public static bool Test(LineSegment seg, BoundingSphere sphere)
        {
            ContainmentType ct1 = sphere.Contains(seg.Point1);
            ContainmentType ct2 = sphere.Contains(seg.Point2);
            return ct1 != ct2;
        }

        public static bool Test(LineSegment seg, Plane plane)
        {
            Vector3 q;
            float t;

            // Compute the t value for the directed line ab intersecting the plane
            Vector3 ab = seg.Point2 - seg.Point1;
            t = (plane.D - Vector3.Dot(plane.Normal, seg.Point1)) / Vector3.Dot(plane.Normal, ab);

            // If t in [0..1] compute and return intersection point
            if (t >= 0.0f && t <= 1.0f)
            {
                q = seg.Point1 + t * ab;
                return true;
            }
            // Else no intersection
            return false;
        }

        #endregion

        #region [ Triangle ]

        #region [ Moller '97 Tri-Tri paper ]

        private static void SORT(ref float a, ref float b)
        {
            if (a > b)
            {
                float c;
                c = a;
                a = b;
                b = c;
            }
        }

        private static bool POINT_IN_TRI(ref short i0, ref short i1, ref Vector3 V0, ref Vector3 U0, ref Vector3 U1, ref Vector3 U2)
        {
            float a, b, c, d0, d1, d2;
            /* is T1 completly inside T2? */
            /* check if V0 is inside tri(U0,U1,U2) */
            a = U1[i1] - U0[i1];
            b = -(U1[i0] - U0[i0]);
            c = -a * U0[i0] - b * U0[i1];
            d0 = a * V0[i0] + b * V0[i1] + c;

            a = U2[i1] - U1[i1];
            b = -(U2[i0] - U1[i0]);
            c = -a * U1[i0] - b * U1[i1];
            d1 = a * V0[i0] + b * V0[i1] + c;

            a = U0[i1] - U2[i1];
            b = -(U0[i0] - U2[i0]);
            c = -a * U2[i0] - b * U2[i1];
            d2 = a * V0[i0] + b * V0[i1] + c;
            if (d0 * d1 > 0.0)
            {
                if (d0 * d2 > 0.0) return true;
            }

            return false;
        }

        private static bool EDGE_EDGE_TEST(ref short i0, ref short i1, ref float Ax, ref float Ay, ref float Bx, ref float By, ref float Cx, ref float Cy, ref float e, ref float d, ref float f, ref Vector3 V0, ref Vector3 U0, ref Vector3 U1)
        {
            Bx = U0[i0] - U1[i0];
            By = U0[i1] - U1[i1];
            Cx = V0[i0] - U0[i0];
            Cy = V0[i1] - U0[i1];
            f = Ay * Bx - Ax * By;
            d = By * Cx - Bx * Cy;
            if ((f > 0 && d >= 0 && d <= f) || (f < 0 && d <= 0 && d >= f))
            {
                e = Ax * Cy - Ay * Cx;
                if (f > 0)
                {
                    if (e >= 0 && e <= f) return true;
                }
                else
                {
                    if (e <= 0 && e >= f) return true;
                }
            }

            return false;
        }

        private static bool EDGE_AGAINST_TRI_EDGES(ref short i0, ref short i1, ref Vector3 V0, ref Vector3 V1, ref Vector3 U0, ref Vector3 U1, ref Vector3 U2)
        {
            float Ax, Ay, Bx = 0, By = 0, Cx = 0, Cy = 0, e = 0, d = 0, f = 0;
            Ax = V1[i0] - V0[i0];
            Ay = V1[i1] - V0[i1];
            /* test edge U0,U1 against V0,V1 */
            if (EDGE_EDGE_TEST(ref i0, ref i1, ref Ax, ref Ay, ref Bx, ref By, ref Cx, ref Cy, ref e, ref d, ref f, ref V0, ref U0, ref U1))
                return true;
            /* test edge U1,U2 against V0,V1 */
            if (EDGE_EDGE_TEST(ref i0, ref i1, ref Ax, ref Ay, ref Bx, ref By, ref Cx, ref Cy, ref e, ref d, ref f, ref V0, ref U1, ref U2))
                return true;
            /* test edge U2,U1 against V0,V1 */
            if (EDGE_EDGE_TEST(ref i0, ref i1, ref Ax, ref Ay, ref Bx, ref By, ref Cx, ref Cy, ref e, ref d, ref f, ref V0, ref U2, ref U0))
                return true;

            return false;
        }

        private static bool coplanar_tri_tri(ref Vector3 N, ref Vector3 V0, ref Vector3 V1, ref Vector3 V2,
                     ref Vector3 U0, ref Vector3 U1, ref Vector3 U2)
        {
            Vector3 A = Vector3.Zero;
            short i0 = 0, i1 = 0;
            /* first project onto an axis-aligned plane, that maximizes the area */
            /* of the triangles, compute indices: i0,i1. */
            A[0] = Math.Abs(N[0]);
            A[1] = Math.Abs(N[1]);
            A[2] = Math.Abs(N[2]);
            if (A[0] > A[1])
            {
                if (A[0] > A[2])
                {
                    i0 = 1;      /* A[0] is greatest */
                    i1 = 2;
                }
                else
                {
                    i0 = 0;      /* A[2] is greatest */
                    i1 = 1;
                }
            }
            else   /* A[0]<=A[1] */
            {
                if (A[2] > A[1])
                {
                    i0 = 0;      /* A[2] is greatest */
                    i1 = 1;
                }
                else
                {
                    i0 = 0;      /* A[1] is greatest */
                    i1 = 2;
                }
            }

            /* test all edges of triangle 1 against the edges of triangle 2 */
            if (EDGE_AGAINST_TRI_EDGES(ref i0, ref i1, ref V0, ref V1, ref U0, ref U1, ref U2))
                return true;
            if (EDGE_AGAINST_TRI_EDGES(ref i0, ref i1, ref V1, ref V2, ref U0, ref U1, ref U2))
                return true;
            if (EDGE_AGAINST_TRI_EDGES(ref i0, ref i1, ref V2, ref V0, ref U0, ref U1, ref U2))
                return true;

            /* finally, test if tri1 is totally contained in tri2 or vice versa */
            if (POINT_IN_TRI(ref i0, ref i1, ref V0, ref U0, ref U1, ref U2))
                return true;
            if (POINT_IN_TRI(ref i0, ref i1, ref U0, ref V0, ref V1, ref V2))
                return true;

            return false;
        }

        private static bool NEWCOMPUTE_INTERVALS(ref Vector3 N1, ref Vector3 V0, ref Vector3 V1, ref Vector3 V2, ref Vector3 U0, ref Vector3 U1, ref Vector3 U2, ref float VV0, ref float VV1,
            ref float VV2, ref float D0, ref float D1, ref float D2, ref float D0D1, ref float D0D2, ref float A, ref float B, ref float C, ref float X0, ref float X1)
        {
            if (D0D1 > 0.0f)
            {
                /* here we know that D0D2<=0.0 */
                /* that is D0, D1 are on the same side, D2 on the other or on the plane */
                A = VV2; B = (VV0 - VV2) * D2; C = (VV1 - VV2) * D2; X0 = D2 - D0; X1 = D2 - D1;
                return true;
            }
            else if (D0D2 > 0.0f)
            {
                /* here we know that d0d1<=0.0 */
                A = VV1; B = (VV0 - VV1) * D1; C = (VV2 - VV1) * D1; X0 = D1 - D0; X1 = D1 - D2;
                return true;
            }
            else if (D1 * D2 > 0.0f || D0 != 0.0f)
            {
                /* here we know that d0d1<=0.0 or that D0!=0.0 */
                A = VV0; B = (VV1 - VV0) * D0; C = (VV2 - VV0) * D0; X0 = D0 - D1; X1 = D0 - D2;
                return true;
            }
            else if (D1 != 0.0f)
            {
                A = VV1; B = (VV0 - VV1) * D1; C = (VV2 - VV1) * D1; X0 = D1 - D0; X1 = D1 - D2;
                return true;
            }
            else if (D2 != 0.0f)
            {
                A = VV2; B = (VV0 - VV2) * D2; C = (VV1 - VV2) * D2; X0 = D2 - D0; X1 = D2 - D1;
                return true;
            }
            else
            {
                /* triangles are coplanar */
                return coplanar_tri_tri(ref N1, ref V0, ref V1, ref V2, ref U0, ref U1, ref U2);
            }
        }

        public static bool NoDivTriTriIsect(Vector3 V0, Vector3 V1, Vector3 V2,
                      Vector3 U0, Vector3 U1, Vector3 U2)
        {
            Vector3 E1, E2;
            Vector3 N1, N2;
            float d1, d2;
            float du0, du1, du2, dv0, dv1, dv2;
            Vector3 D;
            Vector2 isect1 = Vector2.Zero, isect2 = Vector2.Zero;
            float du0du1, du0du2, dv0dv1, dv0dv2;
            short index;
            float vp0, vp1, vp2;
            float up0, up1, up2;
            float bb, cc, max;
            float a = 0, b = 0, c = 0, x0 = 0, x1 = 0;
            float d = 0, e = 0, f = 0, y0 = 0, y1 = 0;
            float xx, yy, xxyy, tmp;

            /* compute plane equation of triangle(V0,V1,V2) */
            E1 = V1 - V0;
            E2 = V2 - V0;
            N1 = Vector3.Cross(E1, E2);
            d1 = -Vector3.Dot(N1, V0);
            /* plane equation 1: N1.X+d1=0 */

            /* put U0,U1,U2 into plane equation 1 to compute signed distances to the plane*/
            du0 = Vector3.Dot(N1, U0) + d1;
            du1 = Vector3.Dot(N1, U1) + d1;
            du2 = Vector3.Dot(N1, U2) + d1;

            /* coplanarity robustness check */
            if (Math.Abs(du0) < FPPrecisionHelper.EPSILON) du0 = 0f;
            if (Math.Abs(du1) < FPPrecisionHelper.EPSILON) du1 = 0f;
            if (Math.Abs(du2) < FPPrecisionHelper.EPSILON) du2 = 0f;

            du0du1 = du0 * du1;
            du0du2 = du0 * du2;

            if (du0du1 > 0.0f && du0du2 > 0.0f) /* same sign on all of them + not equal 0 ? */
                return false;                    /* no intersection occurs */

            /* compute plane of triangle (U0,U1,U2) */
            E1 = U1 - U0;
            E2 = U2 - U0;
            N2 = Vector3.Cross(E1, E2);
            d2 = -Vector3.Dot(N2, U0);
            /* plane equation 2: N2.X+d2=0 */

            /* put V0,V1,V2 into plane equation 2 */
            dv0 = Vector3.Dot(N2, V0) + d2;
            dv1 = Vector3.Dot(N2, V1) + d2;
            dv2 = Vector3.Dot(N2, V2) + d2;

            if (Math.Abs(dv0) < FPPrecisionHelper.EPSILON) dv0 = 0f;
            if (Math.Abs(dv1) < FPPrecisionHelper.EPSILON) dv1 = 0f;
            if (Math.Abs(dv2) < FPPrecisionHelper.EPSILON) dv2 = 0f;

            dv0dv1 = dv0 * dv1;
            dv0dv2 = dv0 * dv2;

            if (dv0dv1 > 0.0f && dv0dv2 > 0.0f) /* same sign on all of them + not equal 0 ? */
                return false;                    /* no intersection occurs */

            /* compute direction of intersection line */
            D = Vector3.Cross(N1, N2);

            /* compute and index to the largest component of D */
            max = (float)Math.Abs(D.X);
            index = 0;
            bb = (float)Math.Abs(D.Y);
            cc = (float)Math.Abs(D.Z);
            if (bb > max)
            {
                max = bb;
                index = 1;
            }
            if (cc > max)
            {
                max = cc;
                index = 2;
            }
            /* this is the simplified projection onto L*/
            vp0 = V0[index];
            vp1 = V1[index];
            vp2 = V2[index];

            up0 = U0[index];
            up1 = U1[index];
            up2 = U2[index];

            /* compute interval for triangle 1 */
            if (!NEWCOMPUTE_INTERVALS(ref N1, ref V0, ref V1, ref V2, ref U0, ref U1, ref U2, ref vp0, ref vp1, ref vp2, ref dv0, ref dv1, ref dv2, ref dv0dv1, ref dv0dv2, ref a, ref b, ref c, ref x0, ref x1))
                return false;

            /* compute interval for triangle 2 */
            if (!NEWCOMPUTE_INTERVALS(ref N1, ref V0, ref V1, ref V2, ref U0, ref U1, ref U2, ref up0, ref up1, ref up2, ref du0, ref du1, ref du2, ref du0du1, ref du0du2, ref d, ref e, ref f, ref y0, ref y1))
                return false;

            xx = x0 * x1;
            yy = y0 * y1;
            xxyy = xx * yy;

            tmp = a * xxyy;
            isect1[0] = tmp + b * x1 * yy;
            isect1[1] = tmp + c * x0 * yy;

            tmp = d * xxyy;
            isect2[0] = tmp + e * xx * y1;
            isect2[1] = tmp + f * xx * y0;

            float isect10 = isect1[0], isect11 = isect1[1], isect20 = isect2[0], isect21 = isect2[1];

            SORT(ref isect10, ref isect11);
            SORT(ref isect20, ref isect21);

            if (isect11 < isect20 || isect21 < isect10) return false;
            
            return true;
        }

        #endregion

        public static bool Test(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 u0, Vector3 u1, Vector3 u2)
        {
            return NoDivTriTriIsect(v0, v1, v2, u0, u1, u2);
        }

        // tests the aabb with triangle [v0,v1,v2] for intersection
        public static bool Test(AxisAlignedBoundingBox b, Vector3 v0, Vector3 v1, Vector3 v2)
        {
            float p0, p1, p2, r;

            // Compute box center and extents (if not already given in that format)
            Vector3 c = (b.Min + b.Max) * 0.5f;
            float e0 = (b.Max.X - b.Min.X) * 0.5f;
            float e1 = (b.Max.Y - b.Min.Y) * 0.5f;
            float e2 = (b.Max.Z - b.Min.Z) * 0.5f;

            // Translate triangle as conceptually moving AABB to origin
            v0 = v0 - c;
            v1 = v1 - c;
            v2 = v2 - c;

            // Compute edge vectors for triangle
            Vector3 f0 = v1 - v0, f1 = v2 - v1, f2 = v0 - v2;

            // Test axes a00..a22 (category 3)
            // Test axis a00
            p0 = v0.Z * v1.Y - v0.Y * v1.Z;
            p2 = v2.Z * (v1.Y - v0.Y) - v2.Y * (v1.Z - v0.Z);
            r = e1 * Math.Abs(f0.Z) + e2 * Math.Abs(f0.Y);
            if (Math.Max(-Math.Max(p0, p2), Math.Min(p0, p2)) > r) return false; // Axis is a separating axis

            // axis a01
            p0 = -v0.Y * f1.Z + v0.Z * f1.Y;
            p1 = -v1.Y * f1.Z + v1.Z * f1.Y;
            p2 = -v2.Y * f1.Z + v2.Z * f1.Y;
            r = e1 * Math.Abs(f1.Z) + e2 * Math.Abs(f1.Y);
            if (Math.Max(Math.Max(Math.Max(p0, p1), p2), Math.Min(Math.Min(p0, p1), p2)) > r) return false;

            // axis a02
            p0 = -v0.Y * f2.Z + v0.Z * f2.Y;
            p1 = -v1.Y * f2.Z + v1.Z * f2.Y;
            p2 = -v2.Y * f2.Z + v2.Z * f2.Y;
            r = e1 * Math.Abs(f2.Z) + e2 * Math.Abs(f2.Y);
            if (Math.Max(Math.Max(Math.Max(p0, p1), p2), Math.Min(Math.Min(p0, p1), p2)) > r) return false;

            // axis a10
            p0 = v0.X * f0.Z - v0.Z * f0.X;
            p1 = v1.X * f0.Z - v1.Z * f0.X;
            p2 = v2.X * f0.Z - v2.Z * f0.X;
            r = e0 * Math.Abs(f0.Z) + e2 * Math.Abs(f0.X);
            if (Math.Max(Math.Max(Math.Max(p0, p1), p2), Math.Min(Math.Min(p0, p1), p2)) > r) return false;

            // axis a11
            p0 = v0.X * f1.Z - v0.Z * f1.X;
            p1 = v1.X * f1.Z - v1.Z * f1.X;
            p2 = v2.X * f1.Z - v2.Z * f1.X;
            r = e0 * Math.Abs(f1.Z) + e2 * Math.Abs(f1.X);
            if (Math.Max(Math.Max(Math.Max(p0, p1), p2), Math.Min(Math.Min(p0, p1), p2)) > r) return false;

            // axis a12
            p0 = v0.X * f2.Z - v0.Z * f2.X;
            p1 = v1.X * f2.Z - v1.Z * f2.X;
            p2 = v2.X * f2.Z - v2.Z * f2.X;
            r = e0 * Math.Abs(f2.Z) + e2 * Math.Abs(f2.X);
            if (Math.Max(Math.Max(Math.Max(p0, p1), p2), Math.Min(Math.Min(p0, p1), p2)) > r) return false;

            // axis20
            p0 = -v0.X * f0.Y + v0.Y * f0.X;
            p1 = -v1.X * f0.Y + v1.Y * f0.X;
            p2 = -v2.X * f0.Y + v2.Y * f0.X;
            r = e0 * Math.Abs(f0.Y) + e1 * Math.Abs(f0.X);
            if (Math.Max(Math.Max(Math.Max(p0, p1), p2), Math.Min(Math.Min(p0, p1), p2)) > r) return false;

            // axis21
            p0 = -v0.X * f1.Y + v0.Y * f1.X;
            p1 = -v1.X * f1.Y + v1.Y * f1.X;
            p2 = -v2.X * f1.Y + v2.Y * f1.X;
            r = e0 * Math.Abs(f1.Y) + e1 * Math.Abs(f1.X);
            if (Math.Max(Math.Max(Math.Max(p0, p1), p2), Math.Min(Math.Min(p0, p1), p2)) > r) return false;

            // axis22
            p0 = -v0.X * f2.Y + v0.Y * f2.X;
            p1 = -v1.X * f2.Y + v1.Y * f2.X;
            p2 = -v2.X * f2.Y + v2.Y * f2.X;
            r = e0 * Math.Abs(f2.Y) + e1 * Math.Abs(f2.X);
            if (Math.Max(Math.Max(Math.Max(p0, p1), p2), Math.Min(Math.Min(p0, p1), p2)) > r) return false;

            // Test the three axes corresponding to the face normals of AABB b (category 1).
            // Exit if...
            // ... [-e0, e0] and [min(v0.x,v1.x,v2.x), max(v0.x,v1.x,v2.x)] do not overlap
            if (Math.Max(Math.Max(v0.X, v1.X), v2.X) < -e0 || Math.Min(Math.Min(v0.X, v1.X), v2.X) > e0) return false;
            // ... [-e1, e1] and [min(v0.y,v1.y,v2.y), max(v0.y,v1.y,v2.y)] do not overlap
            if (Math.Max(Math.Max(v0.Y, v1.Y), v2.Y) < -e1 || Math.Min(Math.Min(v0.Y, v1.Y), v2.Y) > e1) return false;
            // ... [-e2, e2] and [min(v0.z,v1.z,v2.z), max(v0.z,v1.z,v2.z)] do not overlap
            if (Math.Max(Math.Max(v0.Z, v1.Z), v2.Z) < -e2 || Math.Min(Math.Min(v0.Z, v1.Z), v2.Z) > e2) return false;

            // Test separating axis corresponding to triangle face normal (category 2)
            SharpDX.Plane p = new SharpDX.Plane();
            p.Normal = Vector3.Cross(f0, f1);
            p.D = Vector3.Dot(p.Normal, v0);

            return b.Test(p);
        }

        public static bool Test(BoundingSphere sphere, Vector3 a, Vector3 b, Vector3 c)
        {
            // Find point P on triangle ABC closest to sphere center
            Vector3 p = ClosestPoint.Find(sphere.Center, a, b, c);

            // Sphere and triangle intersect if the (squared) distance from sphere
            // center to point p is less than the (squared) sphere radius
            Vector3 v = p - sphere.Center;
            return Vector3.Dot(v, v) <= sphere.Radius * sphere.Radius;
        }

        public static float ScalarTripleProduct(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            return Vector3.Dot(v1, Vector3.Cross(v2, v3));
        }

        public static bool Test(Line line, Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 p = line.Point;
            Vector3 q = line.Point + line.rp.Direction * 1f;
            Vector3 pq = q - p;
            Vector3 pa = a - p;
            Vector3 pb = b - p;
            Vector3 pc = c - p;
            // Test if pq is inside the edges bc, ca and ab. Done by testing
            // that the signed tetrahedral volumes, computed using scalar triple
            // products, are all positive
            Vector3 m = Vector3.Cross(pq, pc);
            float u = Vector3.Dot(pb, m);  // scalar_triple(pq, pc, pb)
            if (u < 0.0f) return false;
            float v = -Vector3.Dot(pa, m); // scalar_triple(pq, pa, pc)
            if (v < 0.0f) return false;
            float w = ScalarTripleProduct(pq, pb, pa);
            if (w < 0.0f) return false;

            return true;
        }

        public static bool Test(Line line, Vector3 a, Vector3 b, Vector3 c, out float u, out float v, out float w)
        {
            Vector3 p = line.Point;
            Vector3 q = line.Point + line.rp.Direction * 1f;
            Vector3 pq = q - p;
            Vector3 pa = a - p;
            Vector3 pb = b - p;
            Vector3 pc = c - p;
            // Test if pq is inside the edges bc, ca and ab. Done by testing
            // that the signed tetrahedral volumes, computed using scalar triple
            // products, are all positive
            Vector3 m = Vector3.Cross(pq, pc);
            u = Vector3.Dot(pb, m);  // scalar_triple(pq, pc, pb)
            if (u < 0.0f)
            {
                v = 0; w = 0;
                return false;
            }
            v = -Vector3.Dot(pa, m); // scalar_triple(pq, pa, pc)
            if (v < 0.0f)
            {
                w = 0;
                return false;
            }
            w = ScalarTripleProduct(pq, pb, pa);
            if (w < 0.0f) return false;

            // Compute the barycentric coordinates (u, v, w) determining the
            // intersection point r, r = u*a + v*b + w*c
            float denom = 1.0f / (u + v + w);
            u *= denom;
            v *= denom;
            w *= denom; // w = 1.0f - u - v;
            return true;
        }

        public static bool Test(LineSegment seg, Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 ab = b - a;
            Vector3 ac = c - a;
            Vector3 qp = seg.Point1 - seg.Point2;

            // Compute triangle normal. Can be precalculated or cached if
            // intersecting multiple segments against the same triangle
            Vector3 n = Vector3.Cross(ab, ac);

            // Compute denominator d. If d <= 0, segment is parallel to or points
            // away from triangle, so exit early
            float d = Vector3.Dot(qp, n);
            if (d <= 0.0f) return false;

            // Compute intersection t value of pq with plane of triangle. A ray
            // intersects iff 0 <= t. Segment intersects iff 0 <= t <= 1. Delay
            // dividing by d until intersection has been found to pierce triangle
            Vector3 ap = seg.Point1 - a;
            float t = Vector3.Dot(ap, n);
            if (t < 0.0f) return false;
            if (t > d) return false; // For segment; exclude this code line for a ray test

            // Compute barycentric coordinate components and test if within bounds
            Vector3 e = Vector3.Cross(qp, ap);
            float v = Vector3.Dot(ac, e);
            if (v < 0.0f || v > d) return false;
            float w = -Vector3.Dot(ab, e);
            if (w < 0.0f || v + w > d) return false;

            return true;
        }

        public static bool Test(LineSegment seg, Vector3 a, Vector3 b, Vector3 c, out float u, out float v, out float w)
        {
            Vector3 ab = b - a;
            Vector3 ac = c - a;
            Vector3 qp = seg.Point1 - seg.Point2;

            // Compute triangle normal. Can be precalculated or cached if
            // intersecting multiple segments against the same triangle
            Vector3 n = Vector3.Cross(ab, ac);

            // Compute denominator d. If d <= 0, segment is parallel to or points
            // away from triangle, so exit early
            float d = Vector3.Dot(qp, n);
            if (d <= 0.0f)
            {
                u = 0; v = 0; w = 0;
                return false;
            }
            // Compute intersection t value of pq with plane of triangle. A ray
            // intersects iff 0 <= t. Segment intersects iff 0 <= t <= 1. Delay
            // dividing by d until intersection has been found to pierce triangle
            Vector3 ap = seg.Point1 - a;
            float t = Vector3.Dot(ap, n);
            if (t < 0.0f)
            {
                u = 0; v = 0; w = 0;
                return false;
            }
            if (t > d)
            {
                u = 0; v = 0; w = 0;
                return false; // For segment; exclude this code line for a ray test
            }
            // Compute barycentric coordinate components and test if within bounds
            Vector3 e = Vector3.Cross(qp, ap);
            v = Vector3.Dot(ac, e);
            if (v < 0.0f || v > d)
            {
                u = 0; w = 0;
                return false;
            }
            w = -Vector3.Dot(ab, e);
            if (w < 0.0f || v + w > d)
            {
                u = 0;
                return false;
            }
            // Segment/ray intersects triangle. Perform delayed division and
            // compute the last barycentric coordinate component
            float ood = 1.0f / d;
            t *= ood;
            v *= ood;
            w *= ood;
            u = 1.0f - v - w;
            return true;
        }

        public static bool Test(SharpDX.Ray ray, Vector3 a, Vector3 b, Vector3 c)
        {
            float u, v, w;
            Vector3 ab = b - a;
            Vector3 ac = c - a;
            Vector3 qp = ray.Position - (ray.Position + ray.Direction);

            // Compute triangle normal. Can be precalculated or cached if
            // intersecting multiple segments against the same triangle
            Vector3 n = Vector3.Cross(ab, ac);

            // Compute denominator d. If d <= 0, segment is parallel to or points
            // away from triangle, so exit early
            float d = Vector3.Dot(qp, n);
            if (d <= 0.0f)
                return false;
            // Compute intersection t value of pq with plane of triangle. A ray
            // intersects iff 0 <= t. Segment intersects iff 0 <= t <= 1. Delay
            // dividing by d until intersection has been found to pierce triangle
            Vector3 ap = ray.Position - a;
            float t = Vector3.Dot(ap, n);
            if (t < 0.0f)
                return false;

            // Compute barycentric coordinate components and test if within bounds
            Vector3 e = Vector3.Cross(qp, ap);
            v = Vector3.Dot(ac, e);
            if (v < 0.0f || v > d)
                return false;
            w = -Vector3.Dot(ab, e);
            if (w < 0.0f || v + w > d)
                return false;
            return true;
        }

        public static bool Test(SharpDX.Ray ray, Vector3 a, Vector3 b, Vector3 c, out float u, out float v, out float w)
        {
            Vector3 ab = b - a;
            Vector3 ac = c - a;
            Vector3 qp = ray.Position - (ray.Position + ray.Direction);

            // Compute triangle normal. Can be precalculated or cached if
            // intersecting multiple segments against the same triangle
            Vector3 n = Vector3.Cross(ab, ac);

            // Compute denominator d. If d <= 0, segment is parallel to or points
            // away from triangle, so exit early
            float d = Vector3.Dot(qp, n);
            if (d <= 0.0f)
            {
                u = 0; v = 0; w = 0;
                return false;
            }
            // Compute intersection t value of pq with plane of triangle. A ray
            // intersects iff 0 <= t. Segment intersects iff 0 <= t <= 1. Delay
            // dividing by d until intersection has been found to pierce triangle
            Vector3 ap = ray.Position - a;
            float t = Vector3.Dot(ap, n);
            if (t < 0.0f)
            {
                u = 0; v = 0; w = 0;
                return false;
            }

            // Compute barycentric coordinate components and test if within bounds
            Vector3 e = Vector3.Cross(qp, ap);
            v = Vector3.Dot(ac, e);
            if (v < 0.0f || v > d)
            {
                u = 0; w = 0;
                return false;
            }
            w = -Vector3.Dot(ab, e);
            if (w < 0.0f || v + w > d)
            {
                u = 0;
                return false;
            }
            // Segment/ray intersects triangle. Perform delayed division and
            // compute the last barycentric coordinate component
            float ood = 1.0f / d;
            t *= ood;
            v *= ood;
            w *= ood;
            u = 1.0f - v - w;
            return true;
        }

        public static bool Test(SharpDX.Plane plane, Vector3 a, Vector3 b, Vector3 c)
        {
            var p1 = plane.Intersects(ref a);
            var p2 = plane.Intersects(ref b);
            var p3 = plane.Intersects(ref c);

            // if all three points are on same side and none of them are on the plane then there is no intersection
            return !(p1 == p2 && p2 == p3 && p1 != PlaneIntersectionType.Intersecting);
        }

        #endregion
    }

    public static class DynamicIntersectionTests
    {
        #region [ Sphere ]

        public static bool Test(BoundingSphere s0, BoundingSphere s1, Vector3 v0, Vector3 v1, out float t)
        {
            // expand sphere s1 by the radius of s0
            s1.Radius += s0.Radius;

            // subtract movement of s1 from both s0 and s1, making s1 stationary
            Vector3 v = v0 - v1;

            // can now test directed segment s = s0.Center + v*t, v = (v0 - v1)/||v0 - v1|| against the expanded sphere for intersection
            float vlen = v.Length();
            SharpDX.Ray s = new SharpDX.Ray(s0.Center, v / vlen);
            if (s1.sSphere.Intersects(ref s, out t))
                return t <= vlen;

            return false;
        }

        #endregion
    }
}
