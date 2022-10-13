using System;
using FixAr;
using FixAr.Vectors;
using FixAr.Quaternions;
using FixAr.Extensions;
using System.Diagnostics;
using static FixAr.Fixp;

namespace FixAr.Testbench {

    class Testbench {

        private const bool run = false;
        private const int testCycles = 10000000;

        static void Main(string[] args) {


            //KULMAN SAAMINEN YKSIKKÖYMPYRÄSSÄ 360 ZOMG
            //ei tarvii normalisoida, palauttaa aina sen kulman, suoraan ylös on 90 astetta, alas -90, oikee 0, vasen 180... (vissiin LUT ominaisuus...)
            //eli ylhäältä menee 0->180 ja alhaalt 0->-180 !
            //Fixp f1 = new Fixp(-2, 0); //x
            //Fixp f2 = new Fixp(0, 0); //y
            //Console.WriteLine(Atan2(f2, f1)); //(huom vaihda x ja y niin saat oikeat arvot)

            //Fixv2 f1 = new Fixv2(1, 0);
            //Fixv2 f2 = new Fixv2(0, -1);
            //Console.WriteLine(Fixv2.Angle(f1, f2)); //angle palauttaa aina kulman duh ja siis sen pienemmän


            //-----------------------
            if (run) Run();
        }

        static void Run() {

            //#-#  Variables  #-#

            Fixp x = new Fixp(0, 0), y = new Fixp(-1, 0);
            Fixp z = new Fixp(1, 370);

            Fixv2 p1 = new Fixv2(0, 0, -1, 0);
            Fixv2 p2 = new Fixv2(6, 0, 0, 0);
            Fixv2 p3 = new Fixv2(3, 0, 3, 0);
            Fixv2 p4 = new Fixv2(5, 0, -1, 0);
            Fixv2 ins;

            //Fixv2 testv = new Fixv2(new Fixp(1856, 0), new Fixp(44, 0));
            Fixp f1 = new Fixp(3, 500);
            Fixp f2 = new Fixp(2, 0);
            Fixp r;

            //#-# #-# #-# #-# #-# 

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < testCycles; i++) {

                //#-# Tests #-#

                //Fixp.Sqrt(Fixp.Half);
                //Fixp.NRoot(Fixp.Half, 30);
                Fixp.Atan2(f1, f2);

                //#-#-#-#-#-#-#

            }

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}
