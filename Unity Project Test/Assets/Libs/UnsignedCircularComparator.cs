using System;

namespace Libs
{
    public class UnsignedCircularComparator
    {
        public static int compareLong(ulong a, ulong b, ulong maxNumber)
        {
            uint bits = 0;
            for (ulong max = maxNumber; max > 0; max >>= 1) bits++;
            if (a != b)
            {
                ulong module = (ulong)Math.Pow(2,bits);
                ulong max = module - 1;
                ulong bitMostSignificant = (max - (max >> 1) );
                /* Eg1: a = 25 b = 245, con 255 como maximo (2**8 - 1); b es menor en lo circular
                 * Eg2: a = 25 b = 35, con 255 como maximo (2**8 - 1); b es mayor en lo circular
                 * Eg2: a = 25 b = 153, con 255 como maximo (2**8 - 1); b es el opuesto en lo circular
                 * Lo "llevo" a comparar con 0
                 * */
                ulong diff = (b - a) % module; // 220 = (base2) 11011100 // 10 = (base2) 00001010 // 128 (base2) 10000000
                /* Me importa el bit mas significativo
                 * Por lo que 11111111 - 01111111 = 10000000
                 * */
                
                if ((diff & bitMostSignificant) != 0)
                {
                    //En el caso opuesto en lo circular entra aca, por lo que hay que ser consistentes
                    if (diff == bitMostSignificant)
                    {
                        return a > b ? 1 : -1;
                    }
                    return 1;
                }
                return -1;
            }
            return 0;

        }

    }
}