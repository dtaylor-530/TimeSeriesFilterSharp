using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ParticleFilterSharp
{



    public class SingleRandom : Random
    {
        static SingleRandom _Instance;
        public static SingleRandom Instance
        {
            get
            {
                if (_Instance == null) _Instance = new SingleRandom();
                return _Instance;
            }
        }

        private SingleRandom() { }
    }






    public static class ReSampleHelper
    {

        public static List<Particle> ReSampleFromIndex(List<Particle> particles, List<int> indeces)
        {

            var x = particles.Where((_, i) => indeces.Contains(i)).ToList();

            x.ForEach(__ => __.Weight = __.Weight / x.Count());

            return x.ToList();

        }
        public static List<Particle> ReSampleFromIndex(List<Particle> particles, int[] indeces)
        {

            var x = particles.Where((_, i) => indeces.Contains(i)).ToList();

            x.ForEach(__ => __.Weight = __.Weight / x.Count());

            return x.ToList();

        }





        public static bool DoResample(double effectiveCountMinRatio, IList<Particle> particles)
        {

            var effectiveCountRatio =
                (double)ReSampleHelper._effectiveParticleCount(ReSampleHelper.GetNormalizedWeights(particles)) / particles.Count;

            return (effectiveCountRatio > Single.Epsilon && effectiveCountRatio < effectiveCountMinRatio);


        }

        public static double _effectiveParticleCount(IEnumerable<double> weights)
        {
            var sumSqr = weights.Sum(x => x * x) + Single.Epsilon;
            return weights.Sum() / sumSqr;
        }




        public static IList<double> GetNormalizedWeights(IEnumerable<Particle> particles)
        {
            List<double> normalizedWeights = new List<double>();

            var weightSum = particles.Sum(x => x.Weight) + Single.Epsilon;

            foreach (var p in particles)
            {
                var normalizedWeight = p.Weight / weightSum;
                normalizedWeights.Add(normalizedWeight);
            }

            return normalizedWeights;
        }

    }






    public static class ParticleHelper
    {

        public static double[] Distances(Point dynamicpoint, Point[] staticpoints, double sensorError)
        {

            var lxy = staticpoints.Select(_ =>
                  MathHelper.VectorMagnitude(_, dynamicpoint) + SingleRandom.Instance.Next(staticpoints.Count()));

            return lxy.ToArray();

        }


        /// <summary>
        /// Averages mean and variance of particle swarm according to weight
        /// </summary>
        /// <param name="particles"></param>
        public static Tuple<Normal, Normal> Estimate(List<Particle> particles)
        {
            var xmean = particles.Average(_ => _.WeightedX());
            var ymean = particles.Average(_ => _.WeightedY());

            var xvariance = particles.Average(_ => _.WeightedXDeviation(xmean));
            var yvariance = particles.Average(_ => _.WeightedYDeviation(ymean));


            return Tuple.Create(
                new Normal(xmean, Math.Sqrt(xvariance)), new Normal(ymean, Math.Sqrt(yvariance)));


        }



    }


    public class ParticleFactory
    {

        //static VectorBuilder<double> vBuilder;


        //public static Particle[] Generate2D(int numberOfParticles, IDistribution distributionx, IDistribution distributiony, IDistribution distributionv, IDistribution distributiono)
        //{

        //    return Enumerable.Range(0, numberOfParticles).Select(_ =>
        //    new Particle
        //        (
        //          distributionx.Sample(), distributiony.Sample(), distributionv.Sample(), distributiono.Sample() % 2.0 * System.Math.PI, weight: 1d
        //        )

        //    ).ToArray();

        //}


        public static List<Particle> BuildSwarm(int numberOfParticles,
            IContinuousDistribution xdistribution, IContinuousDistribution ydistribution,
            IContinuousDistribution vdistribution=null, IContinuousDistribution odistribution=null)
        {



            return Enumerable.Range(0, numberOfParticles).Select(_ =>
            {

                var x = xdistribution.Sample();
                var y = ydistribution.Sample();
                var v = vdistribution?.Sample();
                var o = odistribution?.Sample();

                return new Particle(x, y, 1d / numberOfParticles) { Velocity = v??0, Orientation = o??0 };

            }).ToList();
        }


        public static List<Particle> BuildSwarm(int numberOfParticles, params int[][] ranges)
        {

            return Enumerable.Range(0, numberOfParticles).Select(_ =>
            {
                var randomParam = ranges.Select(__ => ContinuousUniform.Sample(__[0],__[1])).ToArray();


                return new Particle(randomParam.Length>1?randomParam[1]:0, randomParam[0], 1d / numberOfParticles);



            }).ToList();
        }


    }






    public static class MathHelper
    {

        public static IEnumerable<double> CumulativeSum(this IEnumerable<double> sequence)
        {
            double sum = 0;
            foreach (var item in sequence)
            {
                sum += item;
                yield return sum;
            }
        }

        public static double VectorMagnitude(Point p1, Point p2)
        {

            return Pythagoras(p1.X - p2.X, p1.Y - p2.Y);
        }



        public static double Pythagoras(double x, double y)
        {

            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

        }



        public static double Angle(this Point a, Point b)
        {

            var diff = b - a;
            if (diff.X == 0)
                return (diff.Y > 0) ? 0.5 * Math.PI : -0.5 * Math.PI;
            else if (diff.Y == 0)
                return (diff.X > 0) ? 0 : Math.PI;
            else
                return MathNet.Numerics.Trig.Atan(diff.Y / diff.X);


        }
    }


}



