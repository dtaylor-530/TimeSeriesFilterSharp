using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ParticleFilter
{


    public abstract class ParticleFilter
    {


        public List<Particle> Particles { get; set; }
        public int StdDev = 1;


        protected readonly double velocityfactor=0.000000000001;
        public  double VelocityNoise { get; set; } = 6;

        public double OrientationNoise { get; set; }
        public IReSampler ReSampler { get; set; } = new DefaultReSampler();
        // public IReSampler ReSampler { get; set; } = new LowVarianceReSampler();
        //public IReSampler ReSampler { get; set; } = new SystematicReSampler();


        public ParticleFilter()
        {
            //switch (dimensions)
            //{
            //    case (1):
            //        OrientationNoise = 0;
            //        break;
            //    case (2):
            //        OrientationNoise = 0.3;
            //        break;
            //    default:
            //        throw new ArgumentException("dimesions can be only one of two values: 1/2");
            //}
        }


        public virtual void Predict(double effectiveCountMinRatio)
        {
            NextEpoch(effectiveCountMinRatio);

            Particles.ForEach(p => p.DiffuseUniform(StdDev));

        }

        public virtual void Predict(double effectiveCountMinRatio, TimeSpan time)
        {
            NextEpoch(effectiveCountMinRatio);

            Particles.ForEach(p => p.DiffuseNormal2D(VelocityNoise, OrientationNoise, time.Ticks));

        }


        public abstract void Update(System.Windows.Point measure);

        



        protected void NextEpoch(double effectiveCountMinRatio)
        {
            ////resample if too few effective particles
            //if (Neff(particles) < N / 2)
            //    particles = particleSampler.Sample(particles);

            Particles = ReSampleHelper.DoResample(effectiveCountMinRatio, Particles) ?
             ReSampler.Sample(Particles.Count, Particles) :
             Particles;

            Particles = Particles.Select(p => (Particle)p.Clone()).ToList();
        }



    }





    public class ParticleFilter1D:ParticleFilter
    {
   


        public ParticleFilter1D()
        {
            OrientationNoise = 0;

          
        }

        public override void Predict(double effectiveCountMinRatio)
        {
            NextEpoch(effectiveCountMinRatio);

            Particles = Particles.Select(p => p.DiffuseUniform1D(StdDev)).ToList                ();

        }

        public override void Predict(double effectiveCountMinRatio, TimeSpan time)
        {
            NextEpoch(effectiveCountMinRatio);

            Particles=Particles.Select(p => p.DiffuseNormal1D(VelocityNoise* velocityfactor,  time.Ticks)).ToList();

        }

        public override void Update(System.Windows.Point measure)
        {
            var x = new List<Particle>();

            foreach (var p in Particles)
            {
                p.Weight = 1 / (Math.Abs(p.Y - measure.Y));
                
                x.Add(p);

            }

            Particles = x;
        }





    }
}
