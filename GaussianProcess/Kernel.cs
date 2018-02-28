using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaussianProcess
{


    public interface IKernel
    {
       double  Main(double x, double y);


    }

    public class Kernel:IKernel
    {
        //Kernel from Bishop's Pattern Recognition and Machine Learning pg. 307 Eqn. 6.63.

        double Theta0;
        double Theta1;
        double Theta2;
        double Theta3;

        public Kernel(double theta0, double theta1, double theta2, double theta3)
        {

            Theta0 = theta0;
            Theta1 = theta1;
            Theta2 = theta2;
            Theta3 = theta3;
        }

        public double Main(double x, double y)
        {
            double exponential =  Theta0 * Math.Exp(-0.5 * Theta1 * (x- y) * (x - y));
            double linear = Theta3 * x * y;
            double constant = Theta2;
            return exponential+ constant + linear;
        }


    }

    public class OrnsteinKernel : IKernel
    {

        //Ornstein-Uhlenbeck process kernel.
        double Theta;
        public OrnsteinKernel(double theta)

        {
            Theta = theta;
        }


        public double Main(double x, double y)
        {
            return Math.Exp(-Theta * Math.Abs(x - y));

        }

    }

}
