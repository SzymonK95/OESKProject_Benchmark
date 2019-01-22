using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark.Helpers
{
    public static class NoiseManager
    {
        private static bool IsPrime(long candidate)
        {
            if ((candidate & 1) == 0)
            {
                if (candidate == 2)
                    return true;
                else
                    return false;
            }

            for (int i = 3; (i * i) <= candidate; i += 2)
                if ((candidate % i) == 0)
                    return false;

            return candidate != 1;
        }

        private static long NWD(long a, long b)
        {
            while (a != b)
            {
                if (a > b)
                    a -= b;
                else
                    b -= a;
            }

            return a;
        }

        private static int PowerModuloFast(long a, long b, long m)
        {
            int i;
            long result = 1;
            long x = a % m;

            for (i = 1; i <= b; i <<= 1)
            {
                x %= m;
                if ((b & i) != 0)
                {
                    result *= x;
                    result %= m;
                }
                x *= x;
            }

            return (int)result;
        }

        private static char ConvertTo(long n, int newBase)
        {
            int MAX_BASE = 36;
            String pattern = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            String result = "";

            //base is too big or too small
            if ((newBase > MAX_BASE) || (newBase < 2))
                return 'A';

            //n is equal to 0, do not process, return "0"
            if (n == 0)
                return 'B';

            //process until n > 0
            while (n > 0)
            {
                result = pattern[(int)n % newBase] + result;
                n /= newBase;
            }
            if (result == "")
                return '0';
            return result[result.Length - 1];
        }

        public static string GenerateNoise(Int64 lengthOfNoise = 100000)
        {
            Random rnd = new Random();
            long p = 0;
            long q = 0;
            long x = 0;
            long N = 0;

            do
            {
                p = rnd.Next(100000, 500000);
            }
            while (!IsPrime(p) && p % 4 != 3);

            do
            {
                q = rnd.Next(100000, 500000);
            }
            while (!IsPrime(q) && q % 4 != 3);

            N = p * q;

            do
            {
                x = rnd.Next(100000, 500000);
            }
            while (NWD(x, N) != 1);

            long n = lengthOfNoise; //dlugosc ciagu

            long[] xtab = new long[n];

            xtab[0] = PowerModuloFast(x, 2, N);
            for (long i = 1; i < n; i++)
                xtab[i] = PowerModuloFast(xtab[i - 1], 2, N);

            string noise = "";

            foreach (var elem in xtab)
            {
                noise += ConvertTo(elem, 2).ToString();
            }

            return noise;
        }
    }
}