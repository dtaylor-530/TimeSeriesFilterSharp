using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalmanFilter.Wrap
{





    public class DefaultAdaptiveR : IAdaptiveR
    {
        // uses kalman FilterSharp to modify R And Q



        public DefaultAdaptiveR(double[] size)
        {

            //var x=Enumerable.Range(0, size).Select(_ => 100d).ToArray();
            R = Matrix<double>.Build.Diagonal(size).Map(_ => Math.Pow(_, 2));

        }

        private Matrix<double> R;

        public Matrix<double> Value { get { return R; } }



        public void Update(TimeSpan ts, Matrix<double> residual, Matrix<double> transform, Matrix<double> coVariance)
        {

            var measuredR = R;// (residual * residual.Transpose() + transform * coVariance * transform.Transpose());

            R = measuredR;

        }








    }


    public class DefaultAdaptiveQ : IAdaptiveQ
    {
        // uses kalman FilterSharp to modify R And Q




        public DefaultAdaptiveQ(double[] size)
        {

            //var x = Enumerable.Range(0, size).Select(_ => 0.01d).ToArray();
            Q = Matrix<double>.Build.Diagonal(size).Map(_ => Math.Pow(_, 2));

        }


        private Matrix<double> Q;

        public Matrix<double> Value { get { return Q; } }


        public void Update(TimeSpan ts, Matrix<double> innovation, Matrix<double> kalmanGain)
        {


            var measuredQ = Q;// (kalmanGain * innovation * innovation.Transpose() * kalmanGain.Transpose());

            Q = measuredQ;


        }



    }
    }
