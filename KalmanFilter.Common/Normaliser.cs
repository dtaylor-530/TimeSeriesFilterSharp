using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter.Utility
{
    public class Normaliser
    {

        public static List<double> Normalise01(List<double> target)
        {

            var max = target.Max();
            var min = target.Min();
            double diff = max - min;

            return target.Select(_ => (_ - min) / (diff)).ToList();




        }

        public static List<double> Normalise(List<double> target)
        {


            double sum = target.Sum();

            return target.Select(_ => (_ / sum) + Double.Epsilon).ToList();
            // avoid round-off to zero

        }


        public static List<double[]> Normalise(List<double[]> target)
        {
            double[] sum = new double[target.First().Length];

            for (int i = 0; i < target.First().Length; i++)
            {
                sum[i] = target.Select(_ => _[i]).Sum();
            }

            return target.Select(_ => _.Select((__, i) => (__ / sum[i]) + Double.Epsilon).ToArray()).ToList();
        }


        public static List<Matrix<double>> Normalise(List<Matrix<double>> target)
        {
            var sum= target.Aggregate((a, b) => a + b);

            return target.Select(_ => _.PointwiseDivide(sum) + Double.Epsilon).ToList();
        }



        public static double[] Normalise(double[] target)
        {

            double sum = target.Sum();

            return target.Select(_ => (_ / sum) + Double.Epsilon).ToArray();
            // avoid round-off to zero

        }


    }

}
