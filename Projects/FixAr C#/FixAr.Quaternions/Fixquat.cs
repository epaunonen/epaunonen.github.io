/*ø,¸,ø¤°º¤ø,¸¸,ø¤º°`°º¤ø,¸,ø¤°º¤ø,¸¸,ø¤º°`°º¤ø,¸,ø¤º°¤ø,¸¸,ø¤º°`°º¤ø,¸,*
 ¤                                                                      ¤
 *  Copyright (C) 2018-2019 Eemeli Paunonen <paunonen.eemeli@gmail.com> *
 ¤                                                                      ¤
 *                       All rights reserved            ≧◔◡◔≦﻿         * 
 ¤                                                                      ¤
 *                   This file is part of 'FixAr'                       *
 ¤                                                                      ¤ 
 *   'FixAr' can not be copied and/or distributed without the express   *
 ¤                   permission of Eemeli Paunonen                      ¤
 *                                                                      *
 *¤°º¤ø,¸¸,ø¤º°`°º¤ø,¸,ø¤°º¤ø,¸¸,ø¤º°`°º¤ø,¸,ø¤º°¤ø,¸¸,ø¤º°`°º¤ø,¸,ø¤º°¤*/



using System;
using System.ComponentModel;
using FixAr.Vectors;

namespace FixAr.Quaternions {

    /*
     http://www.technologicalutopia.com/sourcecode/xnageometry/quaternion.cs.htm
     http://www.dotnetframework.org/default.aspx/DotNET/DotNET/8@0/untmp/WIN_WINDOWS/lh_tools_devdiv_wpf/Windows/wcp/Core/System/Windows/Media3D/Quaternion@cs/1/Quaternion@cs
     https://www.codeproject.com/Articles/36868/Quaternion-Mathematics-and-D-Library-with-C-and-G
     https://gist.github.com/aeroson/043001ca12fe29ee911e !!!
     https://github.com/opentk/opentk/blob/develop/src/OpenTK/Math/Quaternion.cs
         */

    public partial struct Fixquat {

        #region Settings
        //  |=~=~=~=~=~=~ S E T T I N G S ~=~=~=~=~=~=|

        public enum EulerOrder { XYZ, XZY, YXZ, YZX, ZXY, ZYX }
        public const EulerOrder EULER_ORDER = EulerOrder.YXZ; //Unity käyttää YXZ:a (oikeesti ZXY mutta 'toiskätisesti')

        //  |=~=~=~=~=~=~=~=~=~  *  ~=~=~=~=~=~=~=~=~=|
        #endregion



        /// <summary>
        /// X component of the Quaternion.Don't modify this directly unless you know quaternions inside out.
        /// </summary>
        public Fixp x;

        /// <summary>
        /// Y component of the Quaternion.Don't modify this directly unless you know quaternions inside out.
        /// </summary>
        public Fixp y;

        /// <summary>
        /// Z component of the Quaternion.Don't modify this directly unless you know quaternions inside out.
        /// </summary>
        public Fixp z;

        /// <summary>
        /// W component of the Quaternion.Don't modify this directly unless you know quaternions inside out.
        /// </summary>
        public Fixp w;

        public Fixp this[int index] {
            get {
                switch (index) {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    case 3:
                        return w;
                    default:
                        throw new IndexOutOfRangeException("Invalid Quaternion index: " + index + ", can use only 0, 1, 2, 3");
                }
            }
            set {
                switch (index) {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Quaternion index: " + index + ", can use only 0, 1, 2, 3");
                }
            }
        }

        public static Fixquat Identity = new Fixquat(Fixp.Zero, Fixp.Zero, Fixp.Zero, Fixp.One);

        #region Properties

        public Fixv3 Xyz {
            get => new Fixv3(x, y, z);
            set {
                x = value.x;
                y = value.z;
                z = value.z;
            }
        }

        public Fixv3 EulerAngles { //LOL VÄÄRIN
            get => Fixquat.ToEulerRad(this) * Fixp.RadToDeg;
            set => this = Fixquat.FromEulerRad(value * Fixp.DegToRad);
        }

        public Fixp Length {
            get => Fixp.Sqrt(x * x + y * y + z * z + w * w);
        }

        public Fixp LengthSquared {
            get => x * x + y * y + z * z + w * w;
        }

        #endregion

        #region Constructors

        public Fixquat(Fixp x, Fixp y, Fixp z, Fixp w) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Fixquat(Fixv3 v, Fixp w) {
            x = v.x;
            y = v.y;
            z = v.z;
            this.w = w;
        }

        public void Set(Fixp x, Fixp y, Fixp z, Fixp w) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }


        #endregion

        #region Normalize

        public Fixquat Normalized {
            get => Normalize(this);
        }

        /// <summary>
        /// Scales the quaternion to unit length
        /// </summary>
        public void Normalize() {
            Fixp scale = Fixp.One / this.Length;
            Xyz *= scale;
            w *= scale;
        }

        /// <summary>
        /// Scale the given quaternion to unit length
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static Fixquat Normalize(Fixquat q) {
            Fixp scale = Fixp.One / q.Length;
            return new Fixquat(q.Xyz * scale, q.w * scale);
        }

        #endregion

        #region Dot

        public static Fixp Dot(Fixquat a, Fixquat b) {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * a.w;
        }

        #endregion

        #region Angle Axis

        public static Fixquat AngleAxis(Fixp angle, Fixv3 axis) {
            return AngleAxis(angle, ref axis);
        }

        private static Fixquat AngleAxis(Fixp degrees, ref Fixv3 axis) {
            if (axis.SqrMagnitude == Fixp.Zero) return Identity;

            Fixquat result = Identity;
            var radians = degrees * Fixp.DegToRad;
            radians /= 2;
            axis = axis.Normalized;
            axis = axis * Fixp.Sin(radians);
            result.x = axis.x;
            result.y = axis.y;
            result.z = axis.z;
            result.w = Fixp.Cos(radians);

            return Normalize(result);
        }

        public void ToAngleAxis(out Fixp angle, out Fixv3 axis) {
            ToAxisAngleRad(this, out axis, out angle);
            angle *= Fixp.RadToDeg;
        }

        #endregion

        #region Rotation

        /// <summary>
        /// <para> Creates a rotation which rotates from /fromDirection/ to /toDirection/.</para>
        /// </summary>
        /// <param name="fromDirection"></param>
        /// <param name="toDirection"></param>
        /// <returns></returns>
        public static Fixquat FromToRotation(Fixv3 fromDirection, Fixv3 toDirection) {
            return RotateTowards(LookRotation(fromDirection), LookRotation(toDirection), Fixp.MaxValue);
        }

        public void SetFromToRotation(Fixv3 fromDirection, Fixv3 toDirection) {
            this = FromToRotation(fromDirection, toDirection);
        }

        public static Fixquat LookRotation(Fixv3 forward, [DefaultValue("Fixv3.up")] Fixv3 upwards) {
            return LookRotation(ref forward, ref upwards);
        }

        public static Fixquat LookRotation(Fixv3 forward) {
            Fixv3 up = Fixv3.up;
            return LookRotation(ref forward, ref up);
        }

        private static Fixquat LookRotation(ref Fixv3 forward, ref Fixv3 up) {
            forward = forward.Normalized;
            Fixv3 right = Fixv3.Cross(up, forward).Normalized;
            up = Fixv3.Cross(forward, right);
            var m00 = right.x;
            var m01 = right.y;
            var m02 = right.z;
            var m10 = up.x;
            var m11 = up.y;
            var m12 = up.z;
            var m20 = forward.x;
            var m21 = forward.y;
            var m22 = forward.z;

            Fixp num8 = (m00 + m11) + m22;
            var quaternion = new Fixquat();
            if (num8 > 0) {
                var num = Fixp.Sqrt(num8 + Fixp.One);
                quaternion.w = num / 2;
                num = Fixp.One / 2 / num;
                quaternion.x = (m12 - m21) * num;
                quaternion.y = (m20 - m02) * num;
                quaternion.z = (m01 - m10) * num;
                return quaternion;
            }
            if (m00 >= m11 && m00 >= m22) {
                var num7 = Fixp.Sqrt(Fixp.One + m00 - m11 - m22);
                var num4 = Fixp.One / 2 / num7;
                quaternion.x = Fixp.One / 2 * num7;
                quaternion.y = (m01 + m10) * num4;
                quaternion.z = (m02 + m20) * num4;
                quaternion.w = (m12 - m21) * num4;
                return quaternion;
            }
            if (m11 > m22) {
                var num6 = Fixp.Sqrt(Fixp.One + m11 - m00 - m22);
                var num3 = Fixp.One / 2 / num6;
                quaternion.x = (m10 + m01) * num3;
                quaternion.y = Fixp.One / 2 * num6;
                quaternion.z = (m21 + m12) * num3;
                quaternion.w = (m20 - m02) * num3;
                return quaternion;
            }
            var num5 = Fixp.Sqrt(Fixp.One + m22 - m00 - m11);
            var num2 = Fixp.One / 2 / num5;
            quaternion.x = (m20 + m02) * num2;
            quaternion.y = (m21 + m12) * num2;
            quaternion.z = Fixp.One / 2 * num5;
            quaternion.w = (m01 - m10) * num2;
            return quaternion;
        }

        public void SetLookRotation(Fixv3 view) {
            this.SetLookRotation(view, Fixv3.up);
        }

        public void SetLookRotation(Fixv3 view, Fixv3 up) {
            this = LookRotation(view, up);
        }

        #endregion

        #region Slerp & Lerp

        public static Fixquat Slerp(Fixquat a, Fixquat b, Fixp t) {
            return Slerp(ref a, ref b, t);
        }

        private static Fixquat Slerp(ref Fixquat a, ref Fixquat b, Fixp t) {
            if (t > 1) t = Fixp.One;
            if (t < 0) t = Fixp.Zero;
            return SlerpUnclamped(ref a, ref b, t);
        }

        public static Fixquat SlerpUnclamped(Fixquat a, Fixquat b, Fixp t) {
            return SlerpUnclamped(ref a, ref b, t);
        }

        public static Fixquat SlerpUnclamped(ref Fixquat a, ref Fixquat b, Fixp t) {
            if (a.LengthSquared == 0) {
                if (b.LengthSquared == 0) return Identity;
                return b;
            } else if (b.LengthSquared == 0) return a;

            Fixp cosHalfAngle = a.w * b.w + Fixv3.Dot(a.Xyz, b.Xyz);

            if (cosHalfAngle >= 1 || cosHalfAngle <= -1) {
                return a;
            } else if (cosHalfAngle < 0) {
                b.Xyz = -b.Xyz;
                b.w = -b.w;
                cosHalfAngle = -cosHalfAngle;
            }

            Fixp blendA;
            Fixp blendB;

            if (cosHalfAngle < (Fixp.One - (Fixp.One / 100))) {
                Fixp halfAngle = Fixp.Acos(cosHalfAngle); //hömm onko tässä radit niinq pitäis
                Fixp sinHalfAngle = Fixp.Sin(halfAngle);
                Fixp oneOverSinHalfAngle = Fixp.One / sinHalfAngle;
                blendA = Fixp.One - t;
                blendB = t;
            } else {
                blendA = Fixp.One - t;
                blendB = t;
            }

            Fixquat result = new Fixquat(blendA * a.Xyz + blendB * b.Xyz, blendA * a.w + blendB * b.w);
            if (result.LengthSquared > 0) {
                return Normalize(result);
            } else {
                return Identity;
            }

        }

        public static Fixquat Lerp(Fixquat a, Fixquat b, Fixp t) {
            if (t > 1) t = Fixp.One;
            if (t < 0) t = Fixp.Zero;
            return Slerp(ref a, ref b, t);
        }

        public static Fixquat LerpUnclamped(Fixquat a, Fixquat b, Fixp t) {
            return Slerp(ref a, ref b, t);
        }

        #endregion

        #region Euler

        public static Fixquat FromEulerDeg(Fixv3 euler) {
            return FromEulerRad(new Fixv3(euler.x.ToRadians(), euler.y.ToRadians(), euler.z.ToRadians()));
        }

        public static Fixquat FromEulerRad(Fixv3 euler) {

            Fixp xOver2 = euler.x / 2;
            Fixp yOver2 = euler.y / 2;
            Fixp zOver2 = euler.z / 2;

            var c1 = Fixp.Cos(xOver2);
            var c2 = Fixp.Cos(yOver2);
            var c3 = Fixp.Cos(zOver2);

            var s1 = Fixp.Sin(xOver2);
            var s2 = Fixp.Sin(yOver2);
            var s3 = Fixp.Sin(zOver2);

            Fixquat result = new Fixquat();

            switch (EULER_ORDER) {
#pragma warning disable CS0162 // Unreachable code detected
                case EulerOrder.XYZ:
                    result.x = s1 * c2 * c3 + c1 * s2 * s3;
                    result.y = c1 * s2 * c3 - s1 * c2 * s3;
                    result.z = c1 * c2 * s3 + s1 * s2 * c3;
                    result.w = c1 * c2 * c3 - s1 * s2 * s3;
                    break;
                case EulerOrder.XZY:
                    result.x = s1 * c2 * c3 - c1 * s2 * s3;
                    result.y = c1 * s2 * c3 - s1 * c2 * s3;
                    result.z = c1 * c2 * s3 + s1 * s2 * c3;
                    result.w = c1 * c2 * c3 + s1 * s2 * s3;
                    break;
                case EulerOrder.YXZ:
                    result.x = s1 * c2 * c3 + c1 * s2 * s3;
                    result.y = c1 * s2 * c3 - s1 * c2 * s3;
                    result.z = c1 * c2 * s3 - s1 * s2 * c3;
                    result.w = c1 * c2 * c3 + s1 * s2 * s3;
                    break;
                case EulerOrder.YZX:
                    result.x = s1 * c2 * c3 + c1 * s2 * s3;
                    result.y = c1 * s2 * c3 + s1 * c2 * s3;
                    result.z = c1 * c2 * s3 - s1 * s2 * c3;
                    result.w = c1 * c2 * c3 - s1 * s2 * s3;
                    break;
                case EulerOrder.ZXY:
                    result.x = s1 * c2 * c3 - c1 * s2 * s3;
                    result.y = c1 * s2 * c3 + s1 * c2 * s3;
                    result.z = c1 * c2 * s3 + s1 * s2 * c3;
                    result.w = c1 * c2 * c3 - s1 * s2 * s3;
                    break;
                case EulerOrder.ZYX:
                    result.x = s1 * c2 * c3 - c1 * s2 * s3;
                    result.y = c1 * s2 * c3 + s1 * c2 * s3;
                    result.z = c1 * c2 * s3 - s1 * s2 * c3;
                    result.w = c1 * c2 * c3 + s1 * s2 * s3;
                    break;
#pragma warning restore CS0162 // Unreachable code detected
            }

            return result;

        }

        public static Fixv3 ToEulerRad(Fixquat rotation) {
            //Fixp sqw = rotation.w * rotation.w;
            //Fixp sqx = rotation.x * rotation.x;
            //Fixp sqy = rotation.y * rotation.y;
            //Fixp sqz = rotation.z * rotation.z;
            //Fixp unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
            //Fixp test = rotation.x * rotation.w - rotation.y * rotation.z;
            //Fixv3 v = new Fixv3();

            //if (test > new Fixp(0, 499) * unit) { //TODO: toi 0,499 ei ole kestävä!
            //    v.y = (Fixp)2 * Fixp.Atan2(rotation.y, rotation.x);
            //    v.x = Fixp.PiOver2;
            //    v.z = Fixp.Zero;
            //    return NormalizeAngles(v * Fixp.RadToDeg);
            //}
            //if (test < new Fixp(0, -499) * unit) {
            //    v.y = -(Fixp)2 * Fixp.Atan2(rotation.y, rotation.x);
            //    v.x = -Fixp.PiOver2;
            //    v.x = Fixp.Zero;
            //    return NormalizeAngles(v * Fixp.RadToDeg);
            //}
            //Fixquat q = new Fixquat(rotation.w, rotation.z, rotation.x, rotation.y);
            //v.y = Fixp.Atan2((Fixp)2 * q.x * q.w + (Fixp)2 * q.y * q.z, Fixp.One - (Fixp)2 * (q.z * q.z + q.w * q.w));
            //v.x = Fixp.Asin((Fixp)2 * (q.x * q.z - q.w * q.y));
            //v.z = Fixp.Atan2((Fixp)2 * q.x * q.y + (Fixp)2 * q.z * q.w, Fixp.One - (Fixp)2 * (q.y * q.y + q.z * q.z));
            //return NormalizeAngles(v * Fixp.RadToDeg);
            throw new NotImplementedException();
        }

        #endregion

        public static Fixquat RotateTowards(Fixquat from, Fixquat to, Fixp maxDegreesDelta) {
            Fixp num = Angle(from, to);
            if (num == 0) return to;
            Fixp t = Fixp.Min(Fixp.One, maxDegreesDelta / num);
            return SlerpUnclamped(from, to, t);
        }

        public static Fixquat Inverse(Fixquat rotation) {
            Fixp lengthSq = rotation.LengthSquared;
            if (lengthSq != 0) {
                Fixp i = Fixp.One / lengthSq;
                return new Fixquat(rotation.Xyz * -i, rotation.w * i);
            }
            return rotation;
        }

        public override string ToString() {
            return x + ", " + y + ", " + z + ", " + w;
        }

        public static Fixp Angle(Fixquat a, Fixquat b) {
            Fixp f = Dot(a, b);
            return Fixp.Acos(Fixp.Min(Fixp.Abs(f), Fixp.One)) * 2 * Fixp.RadToDeg;
        }

        public static Fixquat Euler(Fixp x, Fixp y, Fixp z) {
            return FromEulerRad(new Fixv3(x, y, z) * Fixp.DegToRad);
        }

        private static Fixv3 NormalizeAngles(Fixv3 angles) {
            angles.x = NormalizeAngle(angles.x);
            angles.y = NormalizeAngle(angles.y);
            angles.z = NormalizeAngle(angles.z);
            return angles;
        }

        private static Fixp NormalizeAngle(Fixp angle) {
            while (angle > Fixp.Deg360) angle -= Fixp.Deg360;
            while (angle < 0) angle += Fixp.Deg360;
            return angle;
        }

        private static void ToAxisAngleRad(Fixquat q, out Fixv3 axis, out Fixp angle) {
            if (Fixp.Abs(q.w) > 1) q.Normalize();
            angle = (Fixp)2 * Fixp.Acos(q.w);
            Fixp den = Fixp.Sqrt((Fixp)1 - q.w * q.w);
            if(den > 0) {
                axis = q.Xyz / den;
            } else {
                axis = new Fixv3(1, 0, 0);
            }
        }

        public override int GetHashCode() {
            return x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2 ^ w.GetHashCode() >> 1;
        }

        public override bool Equals(object obj) {
            if(!(obj is Fixquat)) {
                return false;
            }
            Fixquat quaternion = (Fixquat)obj;
            return x.Equals(quaternion.x) && y.Equals(quaternion.y) && z.Equals(quaternion.z) && w.Equals(quaternion.w);
        }

        public bool Equals(Fixquat other) {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
        }

        public static Fixquat operator *(Fixquat lhs, Fixquat rhs) {
            return new Fixquat(lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y, lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z, lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x, lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z);
        }

        public static Fixv3 operator *(Fixquat rotation, Fixv3 point) {
            Fixp num = rotation.x * 2;
            Fixp num2 = rotation.y * 2;
            Fixp num3 = rotation.z * 2;
            Fixp num4 = rotation.x * num;
            Fixp num5 = rotation.y * num2;
            Fixp num6 = rotation.z * num3;
            Fixp num7 = rotation.x * num2;
            Fixp num8 = rotation.x * num3;
            Fixp num9 = rotation.y * num3;
            Fixp num10 = rotation.w * num;
            Fixp num11 = rotation.w * num2;
            Fixp num12 = rotation.w * num3;
            Fixv3 result;
            result.x = ((Fixp)1 - (num5 + num6)) * point.x + (num7 - num12) * point.y + (num8 + num11) * point.z;
            result.y = (num7 + num12) * point.x + ((Fixp)1 - (num4 + num6)) * point.y + (num9 - num10) * point.z;
            result.z = (num8 - num11) * point.x + (num9 + num10) * point.y + ((Fixp)1 - (num4 + num5)) * point.z;
            return result;
        }

        public static bool operator ==(Fixquat q1, Fixquat q2) => q1.x == q2.x && q1.y == q2.y && q1.z == q2.z && q1.w == q2.w;

        public static bool operator !=(Fixquat q1, Fixquat q2) => q1.x != q2.x || q1.y != q2.y || q1.z != q2.z || q1.w != q2.w;

        public static Fixv3 RotateVector(Fixv3 vector, Fixquat quaternion) {
            Fixv3 u = new Fixv3(quaternion.x, quaternion.y, quaternion.z);
            Fixp s = quaternion.w;

            return Fixp.One * 2 * Fixv3.Dot(u, vector) * u + (s * s - Fixv3.Dot(u, u)) * vector + Fixp.One * 2 * s * Fixv3.Cross(u, vector);
        }

    }

}

/*	

 				  .
 			     /:\
 			    /;:.\    
 	       _--'/;:.. \'--_
 	     -_   '--___--'   _-
 		   '''--_____--'''
		   __.|    9 )_\  
	  _.-''          /	 
	<`'     ..._    <'
	 `._ .-'    `.  |
	  ; `.    .-'  /
	   \  `~~'  _.'
	    `'...''% _
		  \__ |`.   
		  /`.


*/
