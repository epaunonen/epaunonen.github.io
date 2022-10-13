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
using FixAr.Vectors;

namespace FixAr.Extensions {

    public partial struct Fixrect {

        /// <summary>
        /// Top left corner X
        /// </summary>
        public Fixp x;
        /// <summary>
        /// Top Left corner Y
        /// </summary>
        public Fixp y;
        public Fixp width;
        public Fixp height;

        public Fixp Xmin { get => x; }
        public Fixp Ymin { get => y; }
        public Fixp Xmax { get => x + width; }
        public Fixp Ymax { get => y - height; }
        public Fixv2 Min { get => new Fixv2(x, y); }
        public Fixv2 Max { get => new Fixv2(Xmax, Ymax); }


        /// <summary>
        /// Creates a new Fixrect 
        /// </summary>
        /// <param name="x">Top left corner X</param>
        /// <param name="y">Top left corner Y</param>
        /// <param name="width">Rectangle width</param>
        /// <param name="height">Rectangle height</param>
        public Fixrect(Fixp x, Fixp y, Fixp width, Fixp height) {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public static Fixrect Translate(Fixrect rect, Fixv2 translation) {
            return new Fixrect(rect.x + translation.x, rect.y + translation.y, rect.width, rect.height);
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
