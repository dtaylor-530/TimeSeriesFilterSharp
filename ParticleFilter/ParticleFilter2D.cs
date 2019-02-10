using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace ParticleFilterSharp
{
    public class ParticleFilterSharp2D:ParticleFilterSharp
    {
    


        public ParticleFilterSharp2D(int dimensions=1)
        {
            OrientationNoise  = 0.3;
        }



        public override void Update(System.Windows.Point measure)
        {
            foreach (var p in Particles)
            {
                var dX = p.X - measure.X;
                var dY = p.Y - measure.Y;
                p.Weight = 1 / (Math.Sqrt(dX * dX + dY * dY));

            }
        }









    }




}