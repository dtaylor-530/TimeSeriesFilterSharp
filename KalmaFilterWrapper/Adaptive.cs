using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalmanFilter.Wrap
{


    public class AEKFKFR:IAdaptiveR
    {
        // uses kalman filter to modify R And Q

        DiscreteWrapper dR;


        public AEKFKFR(double[] size)
        {

            dR = new DiscreteWrapper(size, size);

            R = Matrix<double>.Build.Diagonal(size).Map(_ => Math.Pow(_, 2)); //covariance of measurement  
        }

        private Matrix<double> R;
   
        public Matrix<double> Value { get { return R; } }

        public void Update(TimeSpan ts, Matrix<double> residual, Matrix<double> transform, Matrix<double> coVariance)
        {
            var measuredR =0.5*R+0.5* (residual * residual.Transpose() + transform * coVariance * transform.Transpose());

             R = Matrix<double>.Build.DenseOfDiagonalArray(dR.Run(ts, measuredR.Diagonal().ToArray()).Item1.Column(0).ToArray()); ;

        }
    


    }



    public class AEKFKFQ: IAdaptiveQ
    {
        // uses kalman filter to modify R And Q


        DiscreteWrapper dQ;

        public AEKFKFQ(double[] size)
        {

            dQ = new DiscreteWrapper(size,size);
            Q = Matrix<double>.Build.Diagonal(size).Map(_ => Math.Pow(_, 2)); //covariance of measurement  
        }


        Matrix<double> Q;


        public Matrix<double> Value { get { return Q; } }


        public void Update(TimeSpan ts, Matrix<double> innovation, Matrix<double> kalmanGain)
        {


            var measuredQ =0.5 * Q + 0.5 * (kalmanGain * innovation * innovation * kalmanGain.Transpose());

            Q = Matrix<double>.Build.DenseOfDiagonalArray(dQ.Run(ts, measuredQ.Diagonal().ToArray()).Item1.Column(0).ToArray()); ;


        }





    }


    public class DefaultAdaptiveR : IAdaptiveR
    {
        // uses kalman filter to modify R And Q
        


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
        // uses kalman filter to modify R And Q




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
