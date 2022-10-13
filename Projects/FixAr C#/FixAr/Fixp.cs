/*ø,¸,ø¤°º¤ø,¸¸,ø¤º°`°º¤ø,¸,ø¤°º¤ø,¸¸,ø¤º°`°º¤ø,¸,ø¤º°¤ø,¸¸,ø¤º°`°º¤ø,¸,*
 ¤                                                                      ¤
 *  Copyright (C) 2018-2019 Eemeli Paunonen <paunonen.eemeli@gmail.com> *
 ¤                                                                      ¤
 *                       All rights reserved            ≧◔◡◔≦         *
 ¤                                                                      ¤
 *                   This file is part of 'FixAr'                       *
 ¤                                                                      ¤
 *   'FixAr' can not be copied and/or distributed without the express   *
 ¤                   permission of Eemeli Paunonen                      ¤
 *                                                                      *
 *¤°º¤ø,¸¸,ø¤º°`°º¤ø,¸,ø¤°º¤ø,¸¸,ø¤º°`°º¤ø,¸,ø¤º°¤ø,¸¸,ø¤º°`°º¤ø,¸,ø¤º°¤*/

using System;

namespace FixAr { //FixedArithmetic
#pragma warning disable CS0162, CS0618

    public partial struct Fixp : IEquatable<Fixp>, IComparable<Fixp> {
        public readonly long rawValue;

        #region Settings

        //  |=====================================================================|
        //  |~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ S E T T I N G S ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~|
        //  |=====================================================================|

        private const bool ALWAYS_USE_FASTMATH = false; //Enable only if no overflow is possible (only using small enough numbers)

        //                                                                     |
        //                                                         EDIT THESE  V
        private const bool ALWAYS_USE_FASTADD = ALWAYS_USE_FASTMATH ? true : false;

        private const bool ALWAYS_USE_FASTSUB = ALWAYS_USE_FASTMATH ? true : false;
        private const bool ALWAYS_USE_FASTMUL = ALWAYS_USE_FASTMATH ? true : false;
        private const bool ALWAYS_USE_FASTDIV = ALWAYS_USE_FASTMATH ? true : true;

        private const bool ALWAYS_USE_SAFEPOW = false; //Does not overflow
        private const bool USE_FAST_TOSTRING = false; //Converts to double

        internal const int SHIFT_AMOUNT = 12; //12 is 4096 --> "1" = 4096 raw value
        internal const int FRACTIONAL_PRECISION = 1000; //eli millä tarkkuudella voidaan luoda fixp:eja

        //  |=====================================================================|
        //  |~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~|
        //  |=====================================================================|

        #endregion Settings

        private const long MAX_VALUE = long.MaxValue;
        private const long MIN_VALUE = long.MinValue;
        private const long LOG2MAX = 0x1F00000000;
        private const long LOG2MIN = -0x2000000000;

        private const int INTEGER_BITS = (sizeof(int) * 8) - SHIFT_AMOUNT;
        private const int FRACTION_MASK = (int)(uint.MaxValue >> INTEGER_BITS);
        private const int INTEGER_MASK = -1 & ~FRACTION_MASK;
        private const int FRACTION_RANGE = FRACTION_MASK - 1;

        //up to Shift Value 16
        private static readonly long[] PI_TABLE = { //Accessed directly with SHIFT_AMOUNT, up to SHIFT_AMOUNT 16 (manually generated goodness :)
			3, 6, 12, 25, 50, 100, 201, 402, 804, 1608,
            3216, 6433, 12867, 25735, 51471, 102943, 205887
        };

        private static readonly long[] E_TABLE = {
            2, 5, 10, 21, 43, 86, 173, 347, 695, 1391,
            2783, 5567, 11134, 22268, 44536, 89072, 178145,
        };

        private static readonly long[] LN2_TABLE = {
            0, 1, 2, 5, 11, 22, 44, 88, 177, 354, 709,
            1419, 2838, 5677, 11354, 22708, 45416
        };

        private static readonly long[] LOG10_2_TABLE = {
            0, 0, 1, 2, 4, 9, 19, 38, 77, 154, 308, 616,
            1233, 2466, 4932, 9864, 19728, 39456
        };

        private static readonly long[] LOG2_10_TABLE = {
            3, 6, 13, 26, 53, 106, 212, 425, 850, 1700,
            3401, 6803, 13606, 27213, 54426, 108852, 217705
        };

        private static readonly long[] LOG2_E_TABLE = {
            1, 2, 5, 11, 23, 46, 92, 184, 369, 738, 1477,
            2954, 5909, 11818, 23637, 47274, 95548
        };

        private const long RAW90 = 90L << SHIFT_AMOUNT;
        private const long RAW180 = 180L << SHIFT_AMOUNT;
        private const long RAW270 = 270L << SHIFT_AMOUNT;
        private const long RAW360 = 360L << SHIFT_AMOUNT;

        private const long ONE = 1 << SHIFT_AMOUNT;
        private const long HALF = ONE / 2;
        public static readonly Fixp Zero = new Fixp();
        public static readonly Fixp One = new Fixp(ONE);
        public static readonly Fixp Half = new Fixp(HALF);
        public static readonly Fixp Epsilon = new Fixp(0, 1);
        public static readonly Fixp Pi = new Fixp(PI_TABLE[SHIFT_AMOUNT]); //180
        public static readonly Fixp DoublePi = new Fixp(PI_TABLE[SHIFT_AMOUNT] * 2); //radian equivalent of 360 degrees
        public static readonly Fixp PiOver2 = new Fixp(PI_TABLE[SHIFT_AMOUNT] / 2); //90
        public static readonly Fixp MaxValue = new Fixp(MAX_VALUE);
        public static readonly Fixp MinValue = new Fixp(MIN_VALUE);
        public static readonly Fixp E = new Fixp(E_TABLE[SHIFT_AMOUNT]);
        public static readonly Fixp Ln2 = new Fixp(LN2_TABLE[SHIFT_AMOUNT]);
        public static readonly Fixp Log10_2 = new Fixp(LOG10_2_TABLE[SHIFT_AMOUNT]);
        public static readonly Fixp Log2_10 = new Fixp(LOG2_10_TABLE[SHIFT_AMOUNT]);
        public static readonly Fixp Log2_e = new Fixp(LOG2_E_TABLE[SHIFT_AMOUNT]);
        private static readonly Fixp Log2Max = new Fixp(LOG2MAX);
        private static readonly Fixp Log2Min = new Fixp(LOG2MIN);
        public static readonly Fixp Deg90 = new Fixp(RAW90); //deg
        public static readonly Fixp Deg180 = new Fixp(RAW180);
        public static readonly Fixp Deg270 = new Fixp(RAW270);
        public static readonly Fixp Deg360 = new Fixp(RAW360);
        public static readonly Fixp Rad90 = Deg90.ToRadians(); //rad
        public static readonly Fixp Rad180 = Deg180.ToRadians();
        public static readonly Fixp Rad270 = Deg270.ToRadians();
        public static readonly Fixp Rad360 = Deg360.ToRadians();
        public static readonly Fixp RadToDeg = new Fixp(ONE).ToDegrees(); //TODO: meneekö isommilla shift_amounteilla overflow:n puolelle
        public static readonly Fixp DegToRad = new Fixp(ONE).ToRadians(); //nää on semi epätarkkoja...

        #region Constructors

        /// <summary>
        /// Create a Fixp with defined raw value
        /// </summary>
        /// <param name="rawValue">Raw (shifted) value of this Fixp</param>
        public Fixp(int rawValue) {
            this.rawValue = rawValue;
        }

        public Fixp(long rawValue) {
            this.rawValue = rawValue;
        }

        //TODO: muuta summary että kertoo 0, -222 casen
        /// <summary>
        /// Create a Fixp with defined integer and fractional (0 - <see cref="FRACTIONAL_PRECISION"/>) parts
        /// </summary>
        /// <param name="integer">Integer part</param>
        /// <param name="fractional">Fractional part, should be between 0 and <see cref="FRACTIONAL_PRECISION"/> - 1 (usually 999)</param>
        public Fixp(int integer, int fractional) {
            if (integer == 0) {
                rawValue = (fractional << SHIFT_AMOUNT) / FRACTIONAL_PRECISION;
            } else {
                if (integer > 0) {
                    rawValue = (integer << SHIFT_AMOUNT) + ((fractional << SHIFT_AMOUNT) / FRACTIONAL_PRECISION);
                } else {
                    rawValue = (integer << SHIFT_AMOUNT) - ((fractional << SHIFT_AMOUNT) / FRACTIONAL_PRECISION);
                }
            }
        }

        public Fixp(float value) {
            rawValue = (long)(value * ONE);
        }

        public Fixp(double value) {
            rawValue = (long)(value * ONE);
        }

        #endregion Constructors

        #region Conversions

        public int ToInt() => (int)(rawValue >> SHIFT_AMOUNT);

        public float ToFloat() => rawValue / (float)ONE;

        public double ToDouble() => rawValue / (double)ONE;

        //This is very expensive shit, like VERY   EXPENSIVE   SHIT
        public override string ToString() {
            if (USE_FAST_TOSTRING) { //normaali on nopeampi jos frac == 0
                switch (FRACTIONAL_PRECISION) {
                    case 1:
                        return ((double)this).ToString("0");

                    case 10:
                        return ((double)this).ToString("0.#");

                    case 100:
                        return ((double)this).ToString("0.##");

                    case 1000:
                        return ((double)this).ToString("0.###");

                    case 10000:
                        return ((double)this).ToString("0.####");

                    case 100000:
                        return ((double)this).ToString("0.#####");

                    case 1000000:
                        return ((double)this).ToString("0.######");

                    case 10000000:
                        return ((double)this).ToString("0.#######");

                    default:
                        return ((double)this).ToString("0.########");
                }
            } else {
                long integ = Math.Abs(rawValue) / ONE; //precise (ei signiä)
                long frac = Math.Abs((RawFractionalPart * FRACTIONAL_PRECISION) >> SHIFT_AMOUNT); //precise

                string result = "";
                if (rawValue < 0) result += "-" + integ;
                else result += integ;

                if (frac == 0) {
                    return result;
                } else {
                    int i = FRACTIONAL_PRECISION / 10;
                    int j = 0;
                    while (true) {
                        if (frac >= i) {
                            result += ".";
                            for (int k = 0; k < j; k++) {
                                result += "0";
                            }
                            result += frac;
                            result = result.TrimEnd('0');
                            break;
                        }
                        i /= 10;
                        j++;
                    }
                    return result;
                }
            }
        }

        public static explicit operator Fixp(int src) => new Fixp(src << SHIFT_AMOUNT); //atm käsittelee int:iä "kokonaislukuna" (eli !rawvaluena)

        public static explicit operator Fixp(long src) => new Fixp(src << SHIFT_AMOUNT);

        public static explicit operator Fixp(float src) => new Fixp(src);

        public static explicit operator Fixp(double src) => new Fixp(src);

        public static explicit operator int(Fixp src) => (int)(src.rawValue >> SHIFT_AMOUNT);

        public static explicit operator long(Fixp src) => src.rawValue >> SHIFT_AMOUNT;

        public static explicit operator float(Fixp src) => src.rawValue / (float)ONE;

        public static explicit operator double(Fixp src) => src.rawValue / (double)ONE;

        public static explicit operator decimal(Fixp src) => (decimal)src.rawValue / ONE;

        public Fixp ToRadians() => this * Pi / new Fixp(RAW180);

        public Fixp ToDegrees() => this * new Fixp(RAW180) / Pi;

        #endregion Conversions

        #region Misc

        public static int Sign(Fixp value) => value.rawValue < 0 ? -1 : value.rawValue > 0 ? 1 : 0;

        public static Fixp Inverse(Fixp value) => new Fixp(-value.rawValue);

        public Fixp Inverse() => new Fixp(-rawValue);

        /// <summary>
        /// Returns the absolute value of a Fixp number.
        /// Note: Abs(Fixp.MinValue) == Fixp.MaxValue.
        /// </summary>
        public static Fixp Abs(Fixp value) {
            if (value.rawValue == MIN_VALUE) {
                return MaxValue;
            }

            // branchless implementation, see http://www.strchr.com/optimized_abs_function
            var mask = value.rawValue >> 63;
            return new Fixp((value.rawValue + mask) ^ mask);
        }

        /// <summary>
        /// Returns the absolute value of a Fixp number.
        /// FastAbs(Fixp.MinValue) is undefined.
        /// </summary>
        public static Fixp FastAbs(Fixp value) {
            // branchless implementation, see http://www.strchr.com/optimized_abs_function
            var mask = value.rawValue >> 63;
            return new Fixp((value.rawValue + mask) ^ mask);
        }

        public static long IAbs(long value) {
            var mask = value >> 63;
            return (value + mask) ^ mask;
        }

        public static Fixp operator <<(Fixp f, int amount) {
            return new Fixp(f.rawValue << amount);
        }

        public static Fixp operator >>(Fixp f, int amount) {
            return new Fixp(f.rawValue >> amount);
        }

        public static Fixp Min(Fixp a, Fixp b) {
            return a <= b ? a : b;
        }

        public static Fixp Max(Fixp a, Fixp b) {
            return a >= b ? a : b;
        }

        public long RawFractionalPart {
            get {
                return rawValue % ONE;
            }
        }

        public static Fixp Clamp(Fixp value, Fixp min, Fixp max) {
            if (value <= min) return min;
            if (value >= max) return max;
            return value;
        }

        #endregion Misc

        #region +

        public static Fixp operator +(Fixp x, Fixp y) {
            var xl = x.rawValue;
            var yl = y.rawValue;
            var sum = xl + yl;

            // if signs of operands are equal and signs of sum and x are different
            if (!ALWAYS_USE_FASTADD && ((~(xl ^ yl) & (xl ^ sum) & MIN_VALUE) != 0)) {
                sum = xl > 0 ? MAX_VALUE : MIN_VALUE;
            }

            return new Fixp(sum);
        }

        public static Fixp FastAdd(Fixp x, Fixp y) => new Fixp(x.rawValue + y.rawValue);

        public static Fixp operator +(Fixp x, int y) {
            var xl = x.rawValue;
            var yl = y << SHIFT_AMOUNT;
            var sum = xl + yl;

            if (!ALWAYS_USE_FASTADD && ((~(xl ^ yl) & (xl ^ sum) & MIN_VALUE) != 0)) {
                sum = xl > 0 ? MAX_VALUE : MIN_VALUE;
            }

            return new Fixp(sum);
        }

        public static int operator +(int x, Fixp y) {
            return x + (int)(y.rawValue >> SHIFT_AMOUNT);
        }

        public static Fixp operator +(Fixp x, long y) {
            var xl = x.rawValue;
            var yl = y << SHIFT_AMOUNT;
            var sum = xl + yl;

            if (!ALWAYS_USE_FASTADD && ((~(xl ^ yl) & (xl ^ sum) & MIN_VALUE) != 0)) {
                sum = xl > 0 ? MAX_VALUE : MIN_VALUE;
            }

            return new Fixp(sum);
        }

        public static long operator +(long x, Fixp y) {
            return x + (y.rawValue >> SHIFT_AMOUNT);
        }

        #endregion +

        #region -

        public static Fixp operator -(Fixp x, Fixp y) {
            var xl = x.rawValue;
            var yl = y.rawValue;
            var diff = xl - yl;

            // if signs of operands are different and signs of sum and x are different
            if (!ALWAYS_USE_FASTSUB && (((xl ^ yl) & (xl ^ diff) & MIN_VALUE) != 0)) {
                diff = xl < 0 ? MIN_VALUE : MAX_VALUE;
            }

            return new Fixp(diff);
        }

        public static Fixp FastSub(Fixp x, Fixp y) => new Fixp(x.rawValue - y.rawValue);

        public static Fixp operator -(Fixp x) {
            return x.rawValue == MIN_VALUE ? MaxValue : new Fixp(-x.rawValue);
        }

        public static Fixp operator -(Fixp x, int y) {
            var xl = x.rawValue;
            var yl = y << SHIFT_AMOUNT;
            var diff = xl - yl;

            if (!ALWAYS_USE_FASTSUB && (((xl ^ yl) & (xl ^ diff) & MIN_VALUE) != 0)) {
                diff = xl < 0 ? MIN_VALUE : MAX_VALUE;
            }

            return new Fixp(diff);
        }

        public static int operator -(int x, Fixp y) {
            return x - (int)(y.rawValue >> SHIFT_AMOUNT);
        }

        public static Fixp operator -(Fixp x, long y) {
            var xl = x.rawValue;
            var yl = y << SHIFT_AMOUNT;
            var diff = xl - yl;

            if (!ALWAYS_USE_FASTSUB && (((xl ^ yl) & (xl ^ diff) & MIN_VALUE) != 0)) {
                diff = xl < 0 ? MIN_VALUE : MAX_VALUE;
            }

            return new Fixp(diff);
        }

        public static long operator -(long x, Fixp y) {
            return x - (y.rawValue >> SHIFT_AMOUNT);
        }

        #endregion -

        #region *

        public static Fixp operator *(Fixp x, Fixp y) {
            if (ALWAYS_USE_FASTMUL) return new Fixp((x.rawValue * y.rawValue) >> SHIFT_AMOUNT);

            var xl = x.rawValue;
            var yl = y.rawValue;
            var sum = (xl * yl) >> SHIFT_AMOUNT;

            bool opSignsEqual = ((x.rawValue ^ y.rawValue) & MIN_VALUE) == 0;
            if (opSignsEqual) {
                if (sum < 0) {
                    return MaxValue;
                }
            } else {
                if (sum > 0) {
                    return MaxValue;
                } else {
                    long posOp, negOp;
                    if (xl > yl) {
                        posOp = xl;
                        negOp = yl;
                    } else {
                        posOp = yl;
                        negOp = xl;
                    }
                    if (sum > negOp && negOp < -ONE && posOp > ONE) {
                        return MinValue;
                    }
                }
            }

            return new Fixp(sum);
        }

        public static Fixp FastMul(Fixp x, Fixp y) {
            return new Fixp((x.rawValue * y.rawValue) >> SHIFT_AMOUNT);
        }

        public static Fixp operator *(Fixp x, int y) {
            return new Fixp(x.rawValue * y);
        }

        public static Fixp operator *(Fixp x, long y) {
            return new Fixp(x.rawValue * y);
        }

        public static Fixp operator *(int x, Fixp y) {
            return new Fixp(y.rawValue * x);
        }

        public static Fixp operator *(long x, Fixp y) {
            return new Fixp(y.rawValue * x);
        }

        public static long RawMul(long raw1, long raw2) {
            return (raw1 * raw2) >> SHIFT_AMOUNT;
        }

        #endregion *

        #region /

        public static Fixp operator /(Fixp x, Fixp y) {
            if (ALWAYS_USE_FASTDIV) return new Fixp((x.rawValue << SHIFT_AMOUNT) / y.rawValue);

            var xl = x.rawValue;
            var yl = y.rawValue;

            if (yl == 0) {
                throw new DivideByZeroException();
            }

            return new Fixp((xl << SHIFT_AMOUNT) / yl);
        }

        public static Fixp FastDiv(Fixp x, Fixp y) => new Fixp((x.rawValue << SHIFT_AMOUNT) / y.rawValue);

        public static Fixp operator /(Fixp x, int y) {
            if (y == 0) throw new DivideByZeroException();
            return new Fixp(x.rawValue / y);
        }

        public static Fixp operator /(Fixp x, long y) {
            if (y == 0) throw new DivideByZeroException();
            return new Fixp(x.rawValue / y);
        }

        #endregion /

        #region %

        public static Fixp operator %(Fixp x, Fixp y) {
            return new Fixp(x.rawValue == MIN_VALUE & y.rawValue == -1 ? 0 : x.rawValue % y.rawValue);
        }

        public static Fixp operator %(Fixp x, int y) {
            return new Fixp(x.rawValue == MIN_VALUE & (y << SHIFT_AMOUNT) == -1 ? 0 : x.rawValue % (y << SHIFT_AMOUNT));
        }

        public static Fixp operator %(Fixp x, long y) {
            return new Fixp(x.rawValue == MIN_VALUE & (y << SHIFT_AMOUNT) == -1 ? 0 : x.rawValue % (y << SHIFT_AMOUNT));
        }

        public static Fixp FastMod(Fixp x, Fixp y) {
            return new Fixp(x.rawValue & y.rawValue);
        }

        #endregion %

        #region == !=

        public static bool operator ==(Fixp x, Fixp y) => x.rawValue == y.rawValue;

        public static bool operator ==(Fixp x, long y) => x.rawValue == y << SHIFT_AMOUNT;

        public static bool operator ==(long y, Fixp x) => x.rawValue == y << SHIFT_AMOUNT;

        public static bool operator !=(Fixp x, Fixp y) => x.rawValue != y.rawValue;

        public static bool operator !=(Fixp x, long y) => x.rawValue != y << SHIFT_AMOUNT;

        public static bool operator !=(long y, Fixp x) => x.rawValue != y << SHIFT_AMOUNT;

        public override bool Equals(object obj) {
            if (obj is Fixp fixp)
                return (fixp).rawValue == rawValue;
            else
                return false;
        }

        public bool Equals(Fixp other) {
            return rawValue.Equals(other.rawValue);
        }

        public int CompareTo(Fixp other) {
            return rawValue.CompareTo(other.rawValue);
        }

        #endregion == !=

        #region < > <= >=

        #region Fixp - Fixp

        public static bool operator <(Fixp x, Fixp y) {
            return x.rawValue < y.rawValue;
        }

        public static bool operator >(Fixp x, Fixp y) {
            return x.rawValue > y.rawValue;
        }

        public static bool operator <=(Fixp x, Fixp y) {
            return x.rawValue <= y.rawValue;
        }

        public static bool operator >=(Fixp x, Fixp y) {
            return x.rawValue >= y.rawValue;
        }

        #endregion Fixp - Fixp

        #region Fixp - long

        public static bool operator <(Fixp x, long y) {
            return x.rawValue < y << SHIFT_AMOUNT;
        }

        public static bool operator >(Fixp x, long y) {
            return x.rawValue > y << SHIFT_AMOUNT;
        }

        public static bool operator <(long y, Fixp x) {
            return x.rawValue < y << SHIFT_AMOUNT;
        }

        public static bool operator >(long y, Fixp x) {
            return x.rawValue > y << SHIFT_AMOUNT;
        }

        public static bool operator <=(Fixp x, long y) {
            return x.rawValue <= y << SHIFT_AMOUNT;
        }

        public static bool operator >=(Fixp x, long y) {
            return x.rawValue >= y << SHIFT_AMOUNT;
        }

        public static bool operator <=(long y, Fixp x) {
            return x.rawValue <= y << SHIFT_AMOUNT;
        }

        public static bool operator >=(long y, Fixp x) {
            return x.rawValue >= y << SHIFT_AMOUNT;
        }

        #endregion Fixp - long

        #region Fixp - int

        public static bool operator <(Fixp x, int y) {
            return x.rawValue < y << SHIFT_AMOUNT;
        }

        public static bool operator >(Fixp x, int y) {
            return x.rawValue > y << SHIFT_AMOUNT;
        }

        public static bool operator <(int y, Fixp x) {
            return x.rawValue < y << SHIFT_AMOUNT;
        }

        public static bool operator >(int y, Fixp x) {
            return x.rawValue > y << SHIFT_AMOUNT;
        }

        public static bool operator <=(Fixp x, int y) {
            return x.rawValue <= y << SHIFT_AMOUNT;
        }

        public static bool operator >=(Fixp x, int y) {
            return x.rawValue >= y << SHIFT_AMOUNT;
        }

        public static bool operator <=(int y, Fixp x) {
            return x.rawValue <= y << SHIFT_AMOUNT;
        }

        public static bool operator >=(int y, Fixp x) {
            return x.rawValue >= y << SHIFT_AMOUNT;
        }

        #endregion Fixp - int

        #endregion < > <= >=

        #region √

        public static Fixp Sqrt(Fixp f) { //HUOM, tää ei heitä erroria jos f < 0, vaan 0.000
            var n = f.rawValue;
            uint c = 0x8000;
            uint g = 0x8000;

            for (; ; ) {
                if (g * g > n) {
                    g ^= c;
                }

                c >>= 1;

                if (c == 0) {
                    break;
                }

                g |= c;
            }

            return new Fixp(g << (SHIFT_AMOUNT >> 1));
        }

        public static Fixp NRoot(Fixp f, uint root) { //tarvitsee hyvän initial guessin
            Fixp x0 = (Fixp)2; //initial "guess" xd
            Fixp x1 = Zero;

            //Fixp x1 = f >> (int)root;
            while (x1 != x0) {
                x1 = x0;
                x0 = ((x1 * (root - 1)) + (f / (Pow(x1, root - 1)))) / root;
            }
            return x0;
        }

        #endregion √

        #region Rounding

        public static Fixp Floor(Fixp value) { //Aina pienempään
            return new Fixp(value.rawValue & INTEGER_MASK);
        }

        public static Fixp Ceiling(Fixp value) { //Aina suurempaan
            return new Fixp((value.rawValue + FRACTION_MASK) & INTEGER_MASK);
        }

        public static Fixp Round(Fixp value) { //Roundaa alaspäin 0.5ssä
            return new Fixp((value.rawValue + (FRACTION_RANGE >> 1)) & ~FRACTION_MASK);
        }

        public static Fixp Truncate(Fixp value) { //Aina nollaa kohti
            if (value < 0)
                return new Fixp((value.rawValue + FRACTION_RANGE) & INTEGER_MASK);
            else
                return new Fixp(value.rawValue & INTEGER_MASK);
        }

        //Truncaten vastakohta? ja normaali roundaus

        #endregion Rounding

        #region Pow (+exp)

        public static Fixp Pow2(Fixp x) {
            return new Fixp((x * x).rawValue);
        }

        public static Fixp FastPow2(Fixp x) {
            return new Fixp((x.rawValue * x.rawValue) >> SHIFT_AMOUNT);
        }

        /// <summary>
        /// Use if you need a whole exponent
        /// </summary>
        /// <param name="value"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
		public static Fixp Pow(Fixp value, uint exp) {
            if (ALWAYS_USE_SAFEPOW) {
                return SafePow(value, (int)exp);
            }

            Fixp result = new Fixp(1 << SHIFT_AMOUNT);
            for (; ; )
            {
                if ((exp & 1) != 0)
                    result *= value;
                exp >>= 1;
                if (exp == 0)
                    break;
                value *= value;
            }

            return result;
        }

        /// <summary>
        /// ONLY use if you need a fractional exponent! This is quite slow!
        /// </summary>
        /// <param name="value"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static Fixp Pow(Fixp value, Fixp exp) {
            return Exp(exp * Log(value));
        }

        //ei oikeesti oo fast vaan safe... eli jos overflowaa niin clamppaa maxiin
        public static Fixp SafePow(Fixp value, int exp) {
            //------ return values for special cases ----------
            if (value == Zero || value == One || exp == 1) return value;
            if (exp == 0) return One;
            if (exp < 0) return One / SafePow(value, -exp);

            //-------- powering done here ---------
            if (exp % 2 == 0) { //exp is even
                Fixp temp = SafePow(value, exp / 2);
                return temp * temp;
            } else {            //exp is odd
                return value * SafePow(value, exp - 1);
            }
        }

        public static long IPow(long value, uint exp) {
            long result = 1;
            for (; ; ) {
                if ((exp & 1) != 0)
                    result *= value;
                exp >>= 1;
                if (exp == 0)
                    break;
                value *= value;
            }

            return result;
        }

        /// <summary>
        /// e^x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Fixp Exp(Fixp x) {
            Fixp sum = One;

            Fixp f = x / 3;
            int n = 10 * (int)f;
            n = n >= 10 ? n : 10;

            for (int i = n - 1; i > 0; i--) {
                sum = One + (x * sum / (Fixp)i);
            }

            return sum;
        }

        #endregion Pow (+exp)

        #region Log

        /// <summary>
        /// Returns the base-2 logarithm of a specified number.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Fixp Log2(Fixp x) {
            if (x.rawValue <= 0)
                throw new ArgumentOutOfRangeException("value", "Value must be positive");

            long rawX = x.rawValue;
            long b = 1U << (SHIFT_AMOUNT - 1);
            long y = 0;

            while (rawX < ONE) {
                rawX <<= 1;
                y -= ONE;
            }

            while (rawX >= (ONE << 1)) {
                rawX >>= 1;
                y += ONE;
            }

            var z = new Fixp(rawX);

            for (int i = 0; i < SHIFT_AMOUNT; i++) {
                z = FastMul(z, z);
                if (z.rawValue >= (ONE << 1)) {
                    z = new Fixp(z.rawValue >> 1);
                    y += b;
                }
                b >>= 1;
            }

            return new Fixp(y);
        }

        /// <summary>
        /// Returns the natural logarithm of a specified number.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Fixp Ln(Fixp x) {
            return FastMul(Log2(x), Ln2);
        }

        public static Fixp Log10(Fixp value) {
            return Log2(value) * Log10_2;
        }

        public static Fixp Log(Fixp value) { //aka LogE
            return Log2(value) * Ln2;
        }

        public static Fixp Log(Fixp value, Fixp b) {
            if (b == 2)
                return Log2(value);
            else if (b == E)
                return Log(value);
            else if (b == 10)
                return Log10(value);
            else
                return Log2(value) / Log2(b);
        }

        #endregion Log

        #region Trig

        public static Fixp Sin(Fixp radians) {
            Fixp rad = (radians % Rad360); //raw value of actual degrees, huom aina vähemmän kuin 360, ELI jos esim 371.123 niin deg = 11.123 raakana

            bool isNegative = rad < 0; //jos neg niin asetetaan trueksi tässä ja tossa alla neg = !neg
            rad = Abs(rad);

            if (rad > Rad270) { //271 - 360
                rad = Rad360 - rad;
                isNegative = !isNegative;
            } else if (rad > Rad180) { //181 - 270
                rad -= Rad180;
                isNegative = !isNegative;
            } else if (rad > Rad90) { //91 - 180
                rad = Rad180 - rad;
            }

            long s1 = SIN_LUT[rad.rawValue];
            if (isNegative) s1 = -s1;
            return new Fixp(s1);
        }

        public static Fixp Cos(Fixp radians) {
            return Sin(new Fixp(radians.rawValue + Rad90.rawValue));
        }

        public static Fixp? Tan(Fixp radians) {
            Fixp rad = (radians % Rad360);

            bool isNegative = rad < 0;
            rad = Abs(rad);

            if (rad > Rad270) {
                rad = Rad360 - rad;
            } else if (rad > Rad180) {
                rad -= Rad180;
                isNegative = true;
            } else if (rad > Rad90) {
                rad = Rad180 - rad;
                isNegative = true;
            }

            long? t1 = TAN_LUT[rad.rawValue];
            if (t1 == null) return null;
            if (isNegative) t1 = -t1;
            return new Fixp((long)t1);
        }

        public static Fixp Asin(Fixp src) {
            bool isNegative = src < 0;
            src = Abs(src);
            if (src > One) throw new ArithmeticException("Bad Asin Input: " + src.ToDouble());

            int index = Array.BinarySearch(SIN_LUT, src.rawValue);
            if (index < 0) {
                index = ~index;
            }
            if (isNegative) return -(new Fixp(index));
            return new Fixp(index);
        }

        public static Fixp Acos(Fixp src) {
            return Rad90 - Asin(src);
        }

        public static Fixp Atan(Fixp? src) {
            if (src == null) {
                return Rad90;
            }

            var v = (Fixp)src;
            bool isNegative = v < 0;
            src = Abs(v);

            int index = Array.BinarySearch(TAN_LUT, Math.Abs(v.rawValue));
            if (index < 0) {
                index = ~index;
            }
            if (isNegative) return -(new Fixp(index));
            return new Fixp(index);
        }

        public static Fixp Atan2(Fixp y, Fixp x) {
            if (y.rawValue == 0 && x.rawValue == 0) return Zero;
            if (x > 0) {
                return Atan(y / x);
            } else if (x < 0) {
                if (y >= 0) return Pi - Atan(Abs(y / x));
                else return (Pi - Atan(Abs(y / x))).Inverse();
            } else {
                return (y >= 0 ? Pi : -Pi) / 2;
            }
        }

        public static Fixp Csc(Fixp radians) {
            return One / Sin(radians);
        }

        public static Fixp Sec(Fixp radians) {
            return One / Cos(radians);
        }

        public static Fixp? Cot(Fixp radians) {
            return One / Tan(radians);
        }

        #endregion Trig

        public override int GetHashCode() {
            return rawValue.GetHashCode();
        }

        #region LUT Generation

#if DEBUG

        /// <summary>
        /// Generates LUTs for SIN & TAN, ONLY USE WHEN SETTING UP A PROJECT!
        /// </summary>
        public static void GenerateLUTs() {
            GenerateSIN_LUT();
            GenerateTAN_LUT();
        }

        //näihin täytyy tehdä downscalaus
        internal static void GenerateSIN_LUT() {
            using (var writer = new System.IO.StreamWriter("FixAr_SIN_LUT.cs", false)) {
                WriteHeader(writer);
                writer.Write(
@"namespace FixAr {
	public partial struct Fixp {
		internal static readonly long[] SIN_LUT = new long[] {");

                int lineCounter = 0;
                const int n = (int)(1.5708 * ONE) + 1; //eli 1608 (1609(
                const double d = 1.5708 / n;

                for (int i = 0; i < n - 1; i++) {
                    double sin = Math.Sin(d * i);
                    var rawValue = ((Fixp)sin).rawValue;

                    if (lineCounter++ % 8 == 0) {
                        writer.WriteLine();
                        writer.Write("            ");
                    }

                    writer.Write(string.Format("0x{0:X}L, ", rawValue));
                }
                writer.Write(string.Format("0x{0:X}L, ", ONE));
                writer.Write(
@"
        };
    }
}");
                WriteBird(writer);
            }

            const string fileName = "FixAr_SIN_LUT.cs";
            System.IO.FileInfo f = new System.IO.FileInfo(fileName);
            string fullName = f.FullName;
            string fullPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), fileName);
            Console.WriteLine("\nSin LUT has been created at '" + fullPath + "'");
            Console.WriteLine("Use it to replace the current LUT file!\n");
        }

        internal static void GenerateTAN_LUT() {
            using (var writer = new System.IO.StreamWriter("FixAr_TAN_LUT.cs", false)) {
                WriteHeader(writer);
                writer.Write(
@"namespace FixAr {
	public partial struct Fixp {
		internal static readonly long?[] TAN_LUT = new long?[] {");

                int lineCounter = 0;
                const int n = (int)(1.5708 * ONE) + 1; //eli 1608(9)
                const double d = 1.5708 / n;

                for (int i = 0; i < n - 1; i++) {
                    double tan = Math.Tan(d * i);
                    var rawValue = ((Fixp)tan).rawValue;

                    if (lineCounter++ % 8 == 0) {
                        writer.WriteLine();
                        writer.Write("            ");
                    }

                    writer.Write(string.Format("0x{0:X}L, ", rawValue));
                }
                writer.Write("null, ");
                writer.Write(
@"
        };
    }
}");
                WriteBird(writer);
            }

            const string fileName = "FixAr_TAN_LUT.cs";
            System.IO.FileInfo f = new System.IO.FileInfo(fileName);
            string fullName = f.FullName;
            string fullPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), fileName);
            Console.WriteLine("\nTan LUT has been created at '" + fullPath + "'");
            Console.WriteLine("Use it to replace the current LUT file!\n");
        }

        private static void WriteHeader(System.IO.StreamWriter writer) {
            writer.Write(
@"
/*ø,¸,ø¤°º¤ø,¸¸,ø¤º°`°º¤ø,¸,ø¤°º¤ø,¸¸,ø¤º°`°º¤ø,¸,ø¤º°¤ø,¸¸,ø¤º°`°º¤ø,¸,*
 ¤                                                                      ¤
 *  Copyright (C) 2018-2019 Eemeli Paunonen <paunonen.eemeli@gmail.com> *
 ¤                                                                      ¤
 *                       All rights reserved            ≧◔◡◔≦         *
 ¤                                                                      ¤
 *                   This file is part of 'FixAr'                       *
 ¤                                                                      ¤
 *   'FixAr' can not be copied and/or distributed without the express   *
 ¤                   permission of Eemeli Paunonen                      ¤
 *                                                                      *
 *¤°º¤ø,¸¸,ø¤º°`°º¤ø,¸,ø¤°º¤ø,¸¸,ø¤º°`°º¤ø,¸,ø¤º°¤ø,¸¸,ø¤º°`°º¤ø,¸,ø¤º°¤*/

");
        }

        private static void WriteBird(System.IO.StreamWriter writer) {
            writer.Write(
@"

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

*/");
        }

#endif

        #endregion LUT Generation
    }

    //TODO: Safemul, Random

#pragma warning restore CS0162, CS0618
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