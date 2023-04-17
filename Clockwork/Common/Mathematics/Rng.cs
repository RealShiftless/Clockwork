using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Common.Mathematics
{
    public static class Rng
    {
        private static Random _random = new Random();

        public static int Next()
        {
            return _random.Next();
        }
        public static int Next(int n)
        {
            return _random.Next(n);
        }
        public static int Next(int min, int max)
        {
            return _random.Next(min, max);
        }
    }
}
