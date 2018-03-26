using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter.Utility
{
    public static class ProcessFactory
    {
        static Random rand = new Random();

        public static double SinePoint(double time, double Noise,double initialPoint)
        {


            return initialPoint + 5*Math.Sin(time * 10 * 3.14 / 180) + MathNet.Numerics.Distributions.Normal.Sample(rand, 0.0, Noise);



        }


        public static List<Tuple<DateTime, double>> SineWave( double Noise, int iterations, bool isRandom = true,double initialPoint=0)
        {

            List<Tuple<DateTime, double>> lst = new List<Tuple<DateTime, double>>();
            var dt = new DateTime(1000, 1, 1);
            double timespan = 1;
            double ntimespan = 0;
            double totalts = 0;

            for (int i = 0; i < iterations; i++)
            {



                var x = SinePoint(totalts, Noise, initialPoint);

                lst.Add(new Tuple<DateTime, double>(dt, x));



                if (isRandom)
                {
                    ntimespan = MathNet.Numerics.Distributions.Normal.Sample(rand, timespan, timespan / 2);

                }
                else
                {
                    ntimespan = timespan;
                }

                totalts+= ntimespan;

                dt += TimeSpan.FromSeconds(ntimespan);

            }

            return lst.OrderBy(_ => _.Item1).ToList();

        }



        public static double LinePoint(double time, double Noise, double initialPoint)
        {


            return initialPoint + (0.04 *time)+ MathNet.Numerics.Distributions.Normal.Sample(rand, 0.0, Noise);



        }


        public static List<Tuple<DateTime, double>> Line(double Noise, int iterations, bool isRandom = true, double initialPoint = 0d)
        {

            List<Tuple<DateTime, double>> lst = new List<Tuple<DateTime, double>>();
            var dt = new DateTime(1000, 1, 1);
            double timespan = 1;
            double ntimespan = 0;
            double totalts = 0;

            for (int i = 0; i < iterations; i++)
            {



                var x = LinePoint(totalts, Noise, initialPoint);

                lst.Add(new Tuple<DateTime, double>(dt, x));



                if (isRandom)
                {
                    ntimespan = MathNet.Numerics.Distributions.Normal.Sample(rand, timespan, timespan / 2);

                }
                else
                {
                    ntimespan = timespan;
                }

                totalts += ntimespan;

                dt += TimeSpan.FromSeconds(ntimespan);

            }

            return lst.OrderBy(_ => _.Item1).ToList();

        }


    }

    public static class ProcessFactoryEx
    {



        public static List<Tuple<DateTime, double[]>> ToArrays(this IEnumerable<Tuple<DateTime, double>> list)
        {

            return list.Select(_ => Tuple.Create(_.Item1, new double[] { _.Item2 })).ToList();


        }

        public static List<Tuple<DateTime, System.Windows.Point>> ToPoints(this IEnumerable<Tuple<DateTime, double>> list)
        {

            return list.Select(_ => Tuple.Create(_.Item1, new System.Windows.Point(0,_.Item2))).ToList();


        }


        public static IEnumerable<Tuple<DateTime, Matrix<double>>> ToMatrices(this IEnumerable<Tuple<DateTime, double>> list)
        {

            return list.Select(_ => Tuple.Create(
                _.Item1,
                Matrix<double>.Build.DenseOfColumnArrays(new double[] { _.Item2 })));


        }


    }

}
