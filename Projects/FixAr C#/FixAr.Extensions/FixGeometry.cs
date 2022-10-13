using System;
using System.Collections.Generic;
using System.Text;
using FixAr.Vectors;

namespace FixAr.Extensions {

    public static class FixGeometry {

        #region Intersection

        /// <summary>
        /// Calculates where two lines intersect, NOTE: if lines are collinear, returns FALSE
        /// </summary>
        /// <param name="p1">Line 1 point 1</param>
        /// <param name="p2">Line 1 point 2</param>
        /// <param name="p3">Line 2 point 1</param>
        /// <param name="p4">Line 2 point 2</param>
        /// <param name="intersection">Point of intersection</param>
        /// <returns></returns>
        public static bool IntersectLineLine(Fixv2 p1, Fixv2 p2, Fixv2 p3, Fixv2 p4, out Fixv2 intersection) {
            Fixp s1x, s1y, s2x, s2y;
            s1x = p2.x - p1.x;
            s2x = p4.x - p3.x;
            s1y = p2.y - p1.y;
            s2y = p4.y - p3.y;

            Fixp d = (-s2x * s1y + s1x * s2y);
            if (d == 0) { //parallel
                intersection = new Fixv2();
                return false;
            }
            Fixp t = (s2x * (p1.y - p3.y) - s2y * (p1.x - p3.x)) / d;

            intersection = new Fixv2(p1.x + (t * s1x), p1.y + (t * s1y));
            return true;
        }

        /// <summary>
        /// Calculates where two lines intersect, NOTE: if lines are collinear, returns TRUE and (0, 0)
        /// </summary>
        /// <param name="p1">Line 1 point 1</param>
        /// <param name="p2">Line 1 point 2</param>
        /// <param name="p3">Line 2 point 1</param>
        /// <param name="p4">Line 2 point 2</param>
        /// <param name="intersection">Point of intersection</param>
        /// <returns></returns>
        public static bool IntersectLineLine2(Fixv2 p1, Fixv2 p2, Fixv2 p3, Fixv2 p4, out Fixv2 intersection) {
            Fixp s1x, s1y, s2x, s2y;
            s1x = p2.x - p1.x;
            s2x = p4.x - p3.x;
            s1y = p2.y - p1.y;
            s2y = p4.y - p3.y;

            Fixp d = (-s2x * s1y + s1x * s2y);
            if (d == 0) { //parallel
                if (TriangleArea(p1, p2, p3) == 0) { //collinear
                    intersection = new Fixv2();
                    return true;
                }
                intersection = new Fixv2();
                return false;
            }
            Fixp t = (s2x * (p1.y - p3.y) - s2y * (p1.x - p3.x)) / d;

            intersection = new Fixv2(p1.x + (t * s1x), p1.y + (t * s1y));
            return true;
        }

        /// <summary>
        /// Calculates where two line segments intersect, NOTE: if lines are collinear, returns FALSE
        /// </summary>
        /// <param name="p1">Line 1 point 1</param>
        /// <param name="p2">Line 1 point 2</param>
        /// <param name="p3">Line 2 point 1</param>
        /// <param name="p4">Line 2 point 2</param>
        /// <param name="intersection">Point of intersection</param>
        /// <returns></returns>
        public static bool IntersectSegmentSegment(Fixv2 p1, Fixv2 p2, Fixv2 p3, Fixv2 p4, out Fixv2 intersection) {
            Fixv2 ins;
            if (IntersectLineLine(p1, p2, p3, p4, out ins)) { //lines intersect
                if (PointOnLineIsOnSegment(ins, p1, p2) && PointOnLineIsOnSegment(ins, p3, p4)) {
                    intersection = ins;
                    return true;
                }
            }
            intersection = new Fixv2();
            return false;
        }

        //returns first hit time
        public static bool IntersectLineCircleFirst(Fixv2 p1, Fixv2 p2, Fixv2 c, Fixp rad, out Fixp t) {

            Fixp l = (p2 - p1).Magnitude;
            Fixv2 d = (l == Fixp.Zero) ? Fixv2.zero : (p2 - p1) / l; //= normalize

            Fixv2 m = p1 - c;
            Fixp b = Fixv2.Dot(m, d);
            Fixp cc = Fixv2.Dot(m, m) - rad * rad;
            if (cc > Fixp.Zero && b > Fixp.Zero) {
                t = new Fixp();
                return false;
            }
            Fixp discr = b * b - cc;
            if (discr < Fixp.Zero) {
                t = new Fixp();
                return false;
            }
            t = -b - Fixp.Sqrt(discr); //( -b + sqrt... palauttaa sen kauemman!)
            if (t < Fixp.Zero) t = Fixp.Zero;
            if (t <= l) {
                if (l == Fixp.Zero) {
                    t = Fixp.Zero;
                } else {
                    t = t / l;
                }
                return true;
            }
            return false;

        }

        //returns last hit time
        public static bool IntersectLineCircleLast(Fixv2 p1, Fixv2 p2, Fixv2 c, Fixp rad, out Fixp t) {

            Fixp l = (p2 - p1).Magnitude;
            Fixv2 d = (p2 - p1) / l; //= normalize

            Fixv2 m = p1 - c;
            Fixp b = Fixv2.Dot(m, d);
            Fixp cc = Fixv2.Dot(m, m) - rad * rad;
            if (cc > Fixp.Zero && b > Fixp.Zero) {
                t = new Fixp();
                return false;
            }
            Fixp discr = b * b - cc;
            if (discr < Fixp.Zero) {
                t = new Fixp();
                return false;
            }
            t = -b + Fixp.Sqrt(discr); //( -b + sqrt... palauttaa sen kauemman!)
            if (t < Fixp.Zero) t = Fixp.Zero;
            if (t <= l) {
                if (l == Fixp.Zero) {
                    t = Fixp.Zero;
                } else {
                    t = t / l;
                }
                return true;
            }
            return false;
        }

        #endregion

        #region Point on Line

        /// <summary>
        /// Checks if given point is on INFINITE line
        /// </summary>
        /// <param name="point"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool PointIsOnLine(Fixv2 point, Fixv2 p1, Fixv2 p2) {
            Fixp dxc = point.x - p1.x;
            Fixp dyc = point.y - p1.y;
            Fixp dxl = p2.x - p1.x;
            Fixp dyl = p2.y - p1.y;
            Fixp cross = dxc * dyl - dyc * dxl;
            if (cross != 0) return false;
            return true;
        }

        /// <summary>
        /// Checks if given point is on line SEGMENT
        /// </summary>
        /// <param name="point"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool PointIsOnSegment(Fixv2 point, Fixv2 p1, Fixv2 p2) {
            Fixp dxc = point.x - p1.x;
            Fixp dyc = point.y - p1.y;
            Fixp dxl = p2.x - p1.x;
            Fixp dyl = p2.y - p1.y;
            Fixp cross = dxc * dyl - dyc * dxl;
            if (cross != 0) return false;
            if (Fixp.Abs(dxl) >= Fixp.Abs(dyl))
                return dxl > 0 ?
                  p1.x <= point.x && point.x <= p2.x :
                  p2.x <= point.x && point.x <= p1.x;
            else
                return dyl > 0 ?
                  p1.y <= point.y && point.y <= p2.y :
                  p2.y <= point.y && point.y <= p1.y;
        }

        /// <summary>
        /// Checks if given point that is KNOWN TO BE ON LINE is on given segment
        /// </summary>
        /// <param name="point"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool PointOnLineIsOnSegment(Fixv2 point, Fixv2 p1, Fixv2 p2) {
            if (point.x >= Fixp.Min(p1.x, p2.x) && point.x <= Fixp.Max(p1.x, p2.x) && point.y >= Fixp.Min(p1.y, p2.y) && point.y <= Fixp.Max(p1.y, p2.y)) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Calculates the closest point on infinite line (defined with p1 and p2) to given point
        /// </summary>
        /// <param name="point"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Fixv2 ClosestPtPointLine(Fixv2 point, Fixv2 p1, Fixv2 p2) {
            Fixp a = p2.y - p1.y;
            Fixp b = p1.x - p2.x;

            Fixp d = a * a + b * b;
            if (d == 0) {
                return point;
            }

            Fixp c1 = a * p1.x + b * p1.y;
            Fixp c2 = -b * point.x + a * point.y;

            Fixv2 result = new Fixv2((a * c1 - b * c2) / d, (a * c2 + b * c1) / d);

            return result;
        }

        //#1
        public static Fixv2 ClosestPtPointSegment(Fixv2 point, Fixv2 a, Fixv2 b) {
            Fixp t;
            Fixv2 d;
            Fixv2 ab = b - a;
            t = Fixv2.Dot(point - a, ab);
            if (t <= Fixp.Zero) {
                t = Fixp.Zero;
                d = a;
            } else {
                Fixp denom = Fixv2.Dot(ab, ab);
                if (t >= denom) {
                    t = Fixp.One;
                    d = b;
                } else {
                    t = t / denom;
                    d = a + t * ab;
                }
            }
            return d;

        }

        #endregion

        #region Distance

        public static Fixp DistPointLine(Fixv2 point, Fixv2 p1, Fixv2 p2) {
            return Fixp.Sqrt(SqDistPointLine(point, p1, p2));
        }

        public static Fixp DistPointSegment(Fixv2 point, Fixv2 p1, Fixv2 p2) {
            return Fixp.Sqrt(SqDistPointSegment(point, p1, p2));
        }

        public static Fixp SqDistPointLine(Fixv2 point, Fixv2 p1, Fixv2 p2) {
            return Fixv2.SqDistance(point, ClosestPtPointLine(point, p1, p2));
        }

        ////#1 EI TOIMI THO
        //public static Fixp SqDistPointSegment(Fixv2 point, Fixv2 p1, Fixv2 p2) {
        //    Fixv2 ab = p2 - p1, ac = point - p1, bc = point - p2;
        //    Fixp e = Fixv2.Dot(ac, ab);
        //    if (e <= Fixp.Zero) return Fixv2.Dot(ac, ac);
        //    Fixp f = Fixv2.Dot(ab, ab);
        //    if (e >= f) return Fixv2.Dot(bc, bc);
        //    return Fixv2.Dot(ac, ac) + e * e / f;
        //}

        public static Fixp SqDistPointSegment(Fixv2 point, Fixv2 p1, Fixv2 p2) {
            Fixp l2 = Fixv2.SqDistance(p1, p2);
            if (l2 == Fixp.Zero) return Fixv2.SqDistance(point, p1);
            Fixp t = Fixp.Max(Fixp.Zero, Fixp.Min(Fixp.One, Fixv2.Dot(point - p1, p2 - p1) / l2));
            return Fixv2.SqDistance(point, p1 + t * (p2 - p1));
        }

        public static Fixp ShortestSqDistSegmentSegment(Fixv2 p1, Fixv2 p2, Fixv2 p3, Fixv2 p4) {

            //First check if segments intersect
            Fixv2 b;
            if (IntersectSegmentSegment(p1, p2, p3, p4, out b)) {
                return Fixp.Zero;
            }

            //if not, get the shortest squared distance of the 4 possible ones
            Fixp shortest = Fixp.Min(
                Fixp.Min(
                    SqDistPointSegment(p1, p3, p4),
                    SqDistPointSegment(p2, p3, p4)),
                Fixp.Min(
                    SqDistPointSegment(p3, p1, p2),
                    SqDistPointSegment(p4, p1, p2)));

            return shortest;
        }

        public static Fixp ShortestDistSegmentSegment(Fixv2 p1, Fixv2 p2, Fixv2 p3, Fixv2 p4) {
            return Fixp.Sqrt(ShortestSqDistSegmentSegment(p1, p2, p3, p4));
        }

        #endregion

        #region Area

        /// <summary>
        /// Calculates Triangle area
        /// </summary>
        /// <param name="p1">Corner 1</param>
        /// <param name="p2">Corner 2</param>
        /// <param name="p3">Corner 3</param>
        /// <returns></returns>
        public static Fixp TriangleArea(Fixv2 p1, Fixv2 p2, Fixv2 p3) {
            return Fixp.Half * Fixp.Abs((p1.x - p3.x) * (p2.y - p1.y) - (p1.x - p2.x) * (p3.y - p1.y));
        }

        #endregion

    }

}
