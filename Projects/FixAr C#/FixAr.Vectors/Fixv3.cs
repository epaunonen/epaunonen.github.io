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

	public partial struct Fixv3 : IEquatable<Fixv3>, IComparable<Fixv3> {

		public Fixp x;
		public Fixp y;
		public Fixp z;


		public static readonly Fixv3 zero = new Fixv3(Fixp.Zero, Fixp.Zero, Fixp.Zero);
		public static readonly Fixv3 one = new Fixv3(Fixp.One, Fixp.One, Fixp.One);
		public static readonly Fixv3 up = new Fixv3(Fixp.Zero, Fixp.One, Fixp.Zero);
		public static readonly Fixv3 down = new Fixv3(Fixp.Zero, -Fixp.One, Fixp.Zero);
		public static readonly Fixv3 left = new Fixv3(-Fixp.One, Fixp.Zero, Fixp.Zero);
		public static readonly Fixv3 right = new Fixv3(Fixp.One, Fixp.Zero, Fixp.Zero);
		public static readonly Fixv3 back = new Fixv3(Fixp.Zero, Fixp.Zero, -Fixp.One);
		public static readonly Fixv3 front = new Fixv3(Fixp.Zero, Fixp.Zero, Fixp.One);

		#region Constructor

		public Fixv3(Fixp x, Fixp y, Fixp z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Fixv3(Fixp x, Fixp y) {
			this.x = x;
			this.y = y;
			this.z = Fixp.Zero;
		}

		public Fixv3(Fixp x) {
			this.x = x;
			this.y = Fixp.Zero;
			this.z = Fixp.Zero;
		}

		public Fixv3(int x, int y, int z) {
			this.x = (Fixp)x;
			this.y = (Fixp)y;
			this.z = (Fixp)z;
		}

		public Fixv3(int xint, int xfrac, int yint, int yfrac, int zint, int zfrac) {
			this.x = new Fixp(xint, xfrac);
			this.y = new Fixp(yint, yfrac);
			this.z = new Fixp(zint, zfrac);
		}

		#endregion

		#region Magnitude + Normalized

		public Fixp Magnitude => Fixp.Sqrt((x * x) + (y * y) + (z * z));

		public Fixp SqrMagnitude => (x * x) + (y * y) + (z * z);

		public Fixv3 Normalized {
			get {
				if (x == 0 && y == 0 && z == 0) return zero;
				Fixp mag = Magnitude;
				return new Fixv3(x / mag, y / mag, z / mag);
			}
		}

		#endregion

		#region +

		public static Fixv3 operator +(Fixv3 v1, Fixv3 v2) => new Fixv3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);

		#endregion

		#region -

		public static Fixv3 operator -(Fixv3 v1, Fixv3 v2) => new Fixv3(v1.x - v2.y, v1.y - v2.y, v1.z - v2.z);

		public static Fixv3 operator -(Fixv3 v) => new Fixv3(-v.x, -v.y, -v.z);

		#endregion

		#region *

		public static Fixv3 operator *(Fixv3 v, long l) => new Fixv3(v.x * l, v.y * l, v.z * l);

		public static Fixv3 operator *(long l, Fixv3 v) => new Fixv3(v.x * l, v.y * l, v.z * l);

		public static Fixv3 operator *(Fixv3 v, int i) => new Fixv3(v.x * i, v.y * i, v.z * i);

		public static Fixv3 operator *(int i, Fixv3 v) => new Fixv3(v.x * i, v.y * i, v.z * i);

		public static Fixv3 operator *(Fixv3 v, Fixp f) => new Fixv3(v.x * f, v.y * f, v.z * f);

		public static Fixv3 operator *(Fixp f, Fixv3 v) => new Fixv3(v.x * f, v.y * f, v.z * f);

		public static Fixp operator *(Fixv3 v1, Fixv3 v2) => Dot(v1, v2);

		#endregion

		#region /

		public static Fixv3 operator /(Fixv3 v, long l) => new Fixv3(v.x / l, v.y / l, v.z / l);

		public static Fixv3 operator /(Fixv3 v, int i) => new Fixv3(v.x / i, v.y / i, v.z / i);

		public static Fixv3 operator /(Fixv3 v, Fixp f) => new Fixv3(v.x / f, v.y / f, v.z / f);

		#endregion

		#region == !=

		public static bool operator ==(Fixv3 v1, Fixv3 v2) => v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;

		public static bool operator !=(Fixv3 v1, Fixv3 v2) => v1.x != v2.x || v1.y != v2.y || v1.z != v2.z;

		public override bool Equals(object obj) {
			return obj is Fixv3 && Equals((Fixv3)obj);
		}

		public bool Equals(Fixv3 other) {
			return x.Equals(other.x) &&
				   y.Equals(other.y) &&
				   z.Equals(other.z);
		}

		public int CompareTo(Fixv3 other) {
			return SqrMagnitude.CompareTo(other.SqrMagnitude);
		}

		#endregion

		#region Dot & Cross

		public static Fixp Dot(Fixv3 v1, Fixv3 v2) {
			return (v1.x * v2.x) + (v1.y * v2.y) + (v1.z * v2.z);
		}

		public static Fixv3 Cross(Fixv3 v1, Fixv3 v2) {
			return new Fixv3(v1.y * v2.z - v1.z * v2.y, v1.z * v2.x - v1.x * v2.z, v1.x * v2.y - v1.y * v2.x);
		}

		#endregion

		#region Misc

		public static Fixp Distance(Fixv3 a, Fixv3 b) {
			return (b - a).Magnitude;
		}

        public static Fixp Angle(Fixv3 a, Fixv3 b) {
            return new Fixp();
        }

		#endregion

		#region Conversions

		public override string ToString() {
			return x + ", " + y + ", " + z;
		}

		public static explicit operator Fixv3(Fixp[] src) {
			return new Fixv3(src[0], src[1], src[2]);
		}

		#endregion

		public override int GetHashCode() {
			var hashCode = 373119288;
			hashCode = hashCode * -1521134295 + base.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<Fixp>.Default.GetHashCode(x);
			hashCode = hashCode * -1521134295 + EqualityComparer<Fixp>.Default.GetHashCode(y);
			hashCode = hashCode * -1521134295 + EqualityComparer<Fixp>.Default.GetHashCode(z);
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
