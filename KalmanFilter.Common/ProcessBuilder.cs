using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalmanFilter.Common
{
    public static class ProcessBuilder
    {
        static Random rand = new Random();

        public static double SineWave(double timespan, double Noise)
        {


            return 5 * Math.Sin(timespan * 10 * 3.14 / 180) + MathNet.Numerics.Distributions.Normal.Sample(rand, 0.0, 1.0);



        }


        public static List<Tuple<TimeSpan,double>> SineWave(double timespan, double Noise, int iterations)
        {

            List<Tuple<TimeSpan, double>> lst = new List<Tuple<TimeSpan, double>>();
            var ts = new TimeSpan();
            for (int i = 0; i < iterations; i++)
            {
                var x = 5 * Math.Sin(timespan * 10 * 3.14 / 180) + MathNet.Numerics.Distributions.Normal.Sample(rand, 0.0, 1.0);
                ts=ts.Add(TimeSpan.FromSeconds(timespan));
                lst.Add(new Tuple<TimeSpan,double>(ts,x));
            }

            return lst;

        }

    }


   
}
