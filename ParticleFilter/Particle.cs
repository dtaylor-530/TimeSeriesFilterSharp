using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ParticleFilter
{
    public class Particle : ICloneable

    {
        public double Weight { get; set; }
        private Point position;



        public Particle(double x, double y, double weight)
        {
            position = new Point(x, y);
            Weight = weight;
        }


        public double X
        {
            get { return position.X; }
            set { position.X = value; }
        }

        public double Y
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        public double Orientation { get; internal set; }
        public double Velocity { get; internal set; }

        public object Clone()
        {
            return new Particle
            (
                x: this.position.X,
                y: this.position.Y,
                weight: this.Weight


            )
            { Velocity = this.Velocity,Orientation=this.Orientation };
        }

        public static implicit operator Point(Particle p)
        {
            return p.position;

        }

    }




    public static class ParticleEx
    {


        public static double WeightedX(this Particle p)
        {

            return p.X * p.Weight;
        }
        public static double WeightedY(this Particle p)
        {

            return p.Y * p.Weight;
        }

        public static double WeightedXDeviation(this Particle p, double meanX)
        {

            return Math.Pow(p.X - meanX, 2) * p.Weight;
        }


        public static double WeightedYDeviation(this Particle p, double meanY)
        {

            return Math.Pow(p.Y - meanY, 2) * p.Weight;
        }


        public static double YVelocity(this Particle p)
        {

            return MathNet.Numerics.Trig.Sin(p.Orientation) * p.Velocity;
        }

        public static double XVelocity(this Particle p)
        {

            return MathNet.Numerics.Trig.Cos(p.Orientation) * p.Velocity;
        }


        public static Particle DiffuseUniform(this Particle p, double std)
        {

            p.X = (double)MathNet.Numerics.Distributions.ContinuousUniform.Sample(p.X - std, p.X + std);
            p.Y = (double)MathNet.Numerics.Distributions.ContinuousUniform.Sample(p.Y - std, p.Y + std);
            return p;
        }

        public static Particle DiffuseUniform1D(this Particle p, double std)
        {

            p.Y = (double)MathNet.Numerics.Distributions.ContinuousUniform.Sample(p.Y - std, p.Y + std);
            return p;
        }
        //public static void DiffuseNormal(this Particle p, double std)
        //{


        //    p.X = (double)Normal.Sample(p.X, std);
        //    p.Y = (double)Normal.Sample(p.Y, std);

        //}

        public static Particle DiffuseNormal2D(this Particle p, double std, double ostd, long ticks = 1)
        {


            p.X += (double)(MathNet.Numerics.Trig.Cos(p.Orientation) * p.Velocity * ticks);
            p.Y += (double)(MathNet.Numerics.Trig.Sin(p.Orientation) * p.Velocity * ticks);


            p.Orientation = Normal.Sample(p.Orientation, ostd) % 2 * Math.PI;
            p.Velocity = (double)Normal.Sample(p.Velocity, std);

            return p;
        }


        public static Particle DiffuseNormal1D(this Particle p, double std, long ticks = 1)
        {

            p.Y += p.Velocity * ticks;

            p.Velocity = (double)Normal.Sample(p.Velocity, std);

            return p;
        }


    }

}
