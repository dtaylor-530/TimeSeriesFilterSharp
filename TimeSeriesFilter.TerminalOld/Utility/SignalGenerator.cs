using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter.ViewModel
{
    class SignalGenerator
    {
        static Random rand = new Random();

        public static IEnumerable<KeyValuePair<DateTime,double>> GetRandom(DateTime dt,int cnt = 1)
        {

            var points = Enumerable.Repeat(0d, cnt).Select(_ => new { a = _, b = _ });
            var p1 = points.Select(_ => _.a).ToArray();
            var p2 = points.Select(_ => _.b).ToArray();
            Normal.Samples(rand, p1, 1, 1);
            Normal.Samples(rand, p2,rand.Next(0,3), 1);


            foreach (var xy in p1.Zip(p2, (a, b) => (a, b)))
            {
                dt += TimeSpan.FromSeconds(Math.Abs(xy.Item1));

                yield return new KeyValuePair<DateTime, double>(dt, xy.Item2); 
            }

        }


        public static IEnumerable<KeyValuePair<DateTime, double>> GetPeriodic(DateTime dt, int cnt = 1)
        {

            var points = Enumerable.Range(0, cnt).Select(_ => new { a = (double)_, b =(double) _ });
            var p1 = points.Select(_ => _.a).ToArray();
            var p2 = points.Select(_ =>_.b).ToArray();
            //Normal.Samples(rand, p1, 1, 1);
            //Normal.Samples(rand, p2,0, 1);


            foreach (var xy in p1.Zip(p2, (a, b) => (a, b)))
            {
                dt += TimeSpan.FromSeconds(Math.Abs(xy.Item1));

                yield return new KeyValuePair<DateTime, double>(dt, Math.Sin(xy.Item2/3));
            }

        }

    }
}
