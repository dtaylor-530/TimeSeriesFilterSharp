using Filter.Utility;
using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using StarMathLib;

namespace ParticleFilter
{
    public class TwoDParticleFilter //: IFilter<Particle>
    {
        public static double VelocityError = 0.0000001;

        public static double OrientationError = 0.5;

        //Point lastPoint = new Point(0, 0);
        //double lastOrientation = 0;
        //Vector lastSpeed = new Vector();
        //long lastTime = 1;

        public void Predict(float effectiveCountMinRatio,ref List<Particle> particles)
        {
            List<Particle> newParticles = null;
            var effectiveCountRatio = (double)_effectiveParticleCount(GetNormalizedWeights(particles)) / particles.Count;
            if (effectiveCountRatio > Single.Epsilon &&
                effectiveCountRatio < effectiveCountMinRatio)
            {
                newParticles = Resample(particles).ToList();
            }
            else
            {
                newParticles = particles;
                               
            }

            foreach (var p in newParticles)
            {
                p.Diffuse(10);
            }
            particles = new List<Particle>(newParticles);
        }



        private double _effectiveParticleCount(IEnumerable<double> weights)
        {
            var sumSqr = weights.Sum(x => x * x) + Single.Epsilon;
            return weights.Sum() / sumSqr;
        }

        public IList<double> GetNormalizedWeights(IEnumerable<Particle> particles)
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



        public void Predict(ref List<Particle> particles, long ticks)
        {


            //particles.Map(p => p.Orientation = Normal.Sample(p.Orientation, OrientationError) % 2 * Math.PI);
            ////var newOrientations = particles.Select(p => p.Orientation);

            //particles.Map(p => p.Velocity = Normal.Sample(p.Velocity, VelocityError));

            //var newVelocities = particles.Select(p => new { p.XVelocity, p.YVelocity });

            particles = particles.Map(p =>
             {
                 p.X += MathNet.Numerics.Trig.Cos(p.Orientation) * p.Velocity * ticks;
                 p.Y += MathNet.Numerics.Trig.Sin(p.Orientation) * p.Velocity * ticks;
                 return p;
             }).ToList();



            //var normalDistanceSamples = Normal.Samples(lastSpeed.Length * ticks, SensorError);

            //var deltaXY = normalDistanceSamples.Zip(newOrientations, (d, o) =>
            //new Point(MathNet.Numerics.Trig.Cos(o) * d, MathNet.Numerics.Trig.Sin(o) * d));

            // move in the (noisy) commanded direction
            //particles = particles.Zip(deltaXY, (p, d) => { p.YVel += d.Y; p.XVel += d.X; return p; }).ToList();

            //particles = particles.Zip(deltaXY, (p, d) => { p.Y += d.Y; p.X += d.X; return p; }).ToList();


            //lastTime = ticks;
            //int cnt = particles.Count();


        }






        //public void Update(double[] likelihood, double[] prior)
        //{
        //    var posterior = prior * likelihood;

        //    return Normaliser.Normalise(posterior);
        //}


        public void Update(ref List<Particle> particles, Point robot)
        {
            foreach (var p in particles)
            {
                var dX = p.X - robot.X;
                var dY = p.Y - robot.Y;
                p.Weight = 1 / (Math.Sqrt(dX * dX + dY * dY));

            }

            particles = particles.Zip(Normaliser.Normalise(particles.Select(_ => _.Weight).ToList()), (a, b) => { a.Weight = b; return a; }).ToList();
            //particles = particles.Zip(Normaliser.Normalise(weights), (a, b) => { a.Weight = b; return a; }).ToList();
        }









        /// <summary>
        ///  Update the the weights assigned to the particles
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="robot"></param>
        /// <param name="R"></param>
        /// <param name="landmarks"></param>
        //public void Update(ref List<Particle> particles, Point robot, double R)
        //{


        //    //lastOrientation = lastPoint.Angle(robot);
        //    //lastSpeed = (robot - lastPoint) / lastTime;
        //    //lastPoint = robot;

        //    //var rldist = ParticleFilterHelper.Distances(robot, landmarks, 1);

        //    var weights = particles.Select(p =>
        //    {
        //        var x = ParticleFilterHelper.Distances(p, landmarks, 1);
        //        var y = x.Select((_, i) => (new Normal(_, R)).Density(rldist[i]));
        //        return y.Aggregate((a, b) => a * b);
        //        //var x = ParticleFilterHelper.Distances(p, landmarks, 1);
        //        //var y = x.Select((_, i) => (new Normal(_, R)).Density(rldist[i]));
        //        //return y.Aggregate((a, b) => a * b);
        //    }).ToList();



        //    particles = particles.Zip(Normaliser.Normalise(weights), (a, b) => { a.Weight = b; return a; }).ToList();



        //}






        public List<Particle> Resample(List<Particle> particles)
        {
            double[] cumulativeWeights = new double[particles.Count];

            int cumSumIdx = 0;
            double cumSum = 0;
            foreach (var p in particles)
            {
                cumSum += p.Weight;
                cumulativeWeights[cumSumIdx++] = cumSum;
            }

            var maxCumWeight = cumulativeWeights[particles.Count - 1];
            var minCumWeight = cumulativeWeights[0];

            var filteredParticles = new List<Particle>();

            double initialWeight = 1d / particles.Count;

            for (int i = 0; i < particles.Count; i++)
            {
                var randWeight = minCumWeight + RandomProportional.NextDouble(1) * (maxCumWeight - minCumWeight);

                int particleIdx = 0;
                while (cumulativeWeights[particleIdx] < randWeight)
                {
                    particleIdx++;
                }

                var p = particles[particleIdx];
                filteredParticles.Add(p);
            }


            foreach (var dP in filteredParticles)
            {
                dP.Weight = 1d / particles.Count;
            }

            return filteredParticles;
        }


    }



    public static class ParticleFilterHelper
    {

        public static double[] Distances(Point dynamicpoint, Point[] staticpoints, double sensorError)
        {

            var lxy = staticpoints.Map(_ =>
           MathHelper.VectorMagnitude(_, dynamicpoint)  SingleRandom.Instance.Next(staticpoints.Count()) * sensorError*/);

            return lxy.ToArray();

        }


        /// <summary>
        /// Averages mean and variance according to weight
        /// </summary>
        /// <param name="particles"></param>
        public static Tuple<Normal, Normal> Estimate(List<Particle> particles)
        {
            var xmean = particles.Average(_ => _.WeightedX());
            var ymean = particles.Average(_ => _.WeightedY());

            var xvariance = particles.Average(_ => _.WeightedXVariance(xmean));
            var yvariance = particles.Average(_ => _.WeightedYVariance(ymean));


            return Tuple.Create(
                new Normal(xmean, Math.Sqrt(xvariance)), new Normal(ymean, Math.Sqrt(yvariance)));


        }


        public static void Diffuse(this Particle p,double variance)
        {
            p.X += RandomProportional.NextDouble(-variance, variance);
            p.Y += RandomProportional.NextDouble(-variance, variance);

        }


    }


    public static class MathHelper
    {


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

        //public static IList<double> GetNormalizedWeights(IEnumerable<Particle> particles)
        //{
        //    List<double> normalizedWeights = new List<double>();

        //    var weightSum = particles.Sum(x => x.Weight) + Single.Epsilon;

        //    foreach (var p in particles)
        //    {
        //        var normalizedWeight = p.Weight / weightSum;
        //        normalizedWeights.Add(normalizedWeight);
        //    }

        //    return normalizedWeights;
        //}




        //public static double EffectiveParticleCount(IEnumerable<double> weights)
        //{
        //    var sumSqr = weights.Sum(x => x * x) + Single.Epsilon;
        //    return weights.Sum() / sumSqr;
        //}



    }
}
