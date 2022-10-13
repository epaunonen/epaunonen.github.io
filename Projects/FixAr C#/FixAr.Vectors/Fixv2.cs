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
using System.Collections.Generic;

namespace FixAr.Vectors {

	public partial struct Fixv2 : IEquatable<Fixv2>, IComparable<Fixv2> {

		public Fixp x;
		public Fixp y;


		public static readonly Fixv2 zero = new Fixv2(Fixp.Zero, Fixp.Zero);
		public static readonly Fixv2 one = new Fixv2(Fixp.One, Fixp.One);
		public static readonly Fixv2 up = new Fixv2(Fixp.Zero, Fixp.One);
		public static readonly Fixv2 down = new Fixv2(Fixp.Zero, -Fixp.One);
		public static readonly Fixv2 left = new Fixv2(-Fixp.One, Fixp.Zero);
		public static readonly Fixv2 right = new Fixv2(Fixp.One, Fixp.Zero);

		#region Constructors

		public Fixv2(Fixp x, Fixp y) {
			this.x = x;
			this.y = y;
		}

		public Fixv2(Fixp x) {
			this.x = x;
			this.y = Fixp.Zero;
		}

		public Fixv2(int x, int y) {
			this.x = (Fixp)x;
			this.y = (Fixp)y;
		}

		public Fixv2(int xint, int xfrac, int yint, int yfrac) {
			this.x = new Fixp(xint, xfrac);
			this.y = new Fixp(yint, yfrac);
		}

		#endregion

		#region Magnitude + Normalized

		public Fixp Magnitude => Fixp.Sqrt((x * x) + (y * y));

		public Fixp SqrMagnitude => (x * x) + (y * y);

		public Fixv2 Normalized {
            get {
                if (x == 0 && y == 0) return zero;
                Fixp mag = Magnitude;
                if (mag == Fixp.Zero) return zero;
                return new Fixv2(x / mag, y / mag);
            }
        }

		#endregion

		#region +

		public static Fixv2 operator +(Fixv2 v1, Fixv2 v2) => new Fixv2(v1.x + v2.x, v1.y + v2.y);

		#endregion

		#region -

		public static Fixv2 operator -(Fixv2 v1, Fixv2 v2) => new Fixv2(v1.x - v2.x, v1.y - v2.y);

		public static Fixv2 operator -(Fixv2 v) => new Fixv2(-v.x, -v.y);

		#endregion

		#region *

		public static Fixv2 operator *(Fixv2 v, long l) => new Fixv2(v.x * l, v.y * l);

		public static Fixv2 operator *(long l, Fixv2 v) => new Fixv2(v.x * l, v.y * l);

		public static Fixv2 operator *(Fixv2 v, int i) => new Fixv2(v.x * i, v.y * i);

		public static Fixv2 operator *(int i, Fixv2 v) => new Fixv2(v.x * i, v.y * i);

		public static Fixv2 operator *(Fixv2 v, Fixp f) => new Fixv2(v.x * f, v.y * f);

		public static Fixv2 operator *(Fixp f, Fixv2 v) => new Fixv2(v.x * f, v.y * f);

		public static Fixp operator *(Fixv2 v1, Fixv2 v2) => Dot(v1, v2);

		#endregion

		#region /

		public static Fixv2 operator /(Fixv2 v, long l) => new Fixv2(v.x / l, v.y / l);

		public static Fixv2 operator /(Fixv2 v, int i) => new Fixv2(v.x / i, v.y / i);

		public static Fixv2 operator /(Fixv2 v, Fixp f) => new Fixv2(v.x / f, v.y / f);

		#endregion

		#region == !=

		public static bool operator ==(Fixv2 v1, Fixv2 v2) => v1.x == v2.x && v1.y == v2.y;

		public static bool operator !=(Fixv2 v1, Fixv2 v2) => v1.x != v2.x || v1.y != v2.y;

		public override bool Equals(object obj) {
			return obj is Fixv2 && Equals((Fixv2)obj);
		}

		public bool Equals(Fixv2 other) {
			return x.Equals(other.x) &&
				   y.Equals(other.y);
		}

		public int CompareTo(Fixv2 other) {
			return SqrMagnitude.CompareTo(other.SqrMagnitude);
		}

		#endregion

		#region Dot & Cross

		public static Fixp Dot(Fixv2 v1, Fixv2 v2) {
			//return Fixp.Clamp((v1.x * v2.x) + (v1.y * v2.y), -Fixp.One, Fixp.One);
            return (v1.x * v2.x) + (v1.y * v2.y);
        }

		public static Fixp Cross(Fixv2 v1, Fixv2 v2) { //wtf tää on, ilmeisesti se on just näin
			return (v1.x * v2.y) - (v1.y * v2.x);
		}

		#endregion

		#region Misc

		public static Fixp Distance(Fixv2 a, Fixv2 b) {
			return (b - a).Magnitude;
		}

        public static Fixp SqDistance(Fixv2 a, Fixv2 b) {
            return (b - a).SqrMagnitude;
        }

        public static Fixp Angle(Fixv2 a, Fixv2 b) {
            return Fixp.Acos(Fixp.Clamp(Dot(a.Normalized, b.Normalized), -Fixp.One, Fixp.One));
        }

        /// <summary>
        /// Returns two NORMALIZED normal vectors
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        public void GetNormals(out Fixv2 n1, out Fixv2 n2) {
            Fixv2 unit = Normalized;
            n1 = new Fixv2(unit.y, -unit.x);
            n2 = new Fixv2(-unit.y, unit.x);
        }

        /// <summary>
        /// Return one NORMALIZED normal vector
        /// </summary>
        /// <returns></returns>
        public Fixv2 GetNormal() {
            Fixv2 unit = Normalized;
            return new Fixv2(unit.y, -unit.x);
        }

        #endregion

        #region Conversions

        public override string ToString() {
			return x + ", " + y;
		}

		public static explicit operator Fixv2(Fixp[] src) {
			return new Fixv2(src[0], src[1]);
		}

        public static implicit operator Fixv3(Fixv2 src) {
            return new Fixv3(src.x, src.y);
        }

        #endregion

        public override int GetHashCode() {
			var hashCode = 1502939027;
			hashCode = hashCode * -1521134295 + base.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<Fixp>.Default.GetHashCode(x);
			hashCode = hashCode * -1521134295 + EqualityComparer<Fixp>.Default.GetHashCode(y);
			return hashCode;
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
