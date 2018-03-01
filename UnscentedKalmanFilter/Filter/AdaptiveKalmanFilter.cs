using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalmanFilter
{
    public class Adaptive1
    {

        double Alpha;

        public Adaptive1(double alpha)
        {
            Alpha = alpha;


        }


        public Matrix<double> UpdateR(Matrix<double> R, Matrix<double> residual, Matrix<double> transform, Matrix<double> coVariance)
        {


            var xx = R.Multiply(Alpha) + (1 - Alpha) * (residual * residual.Transpose() + transform * coVariance * transform.Transpose());

            return xx;


        }



        public Matrix<double> UpdateQ(Matrix<double> Q, Vector<double> innovation, Matrix<double> kalmanGain)
        {


            //var xx = Q.Multiply(Alpha) + (1 - Alpha) * (kalmanGain * innovation * innovation* kalmanGain.Transpose());
            var xx = Q.Multiply(Alpha) + (1 - Alpha) * (kalmanGain );
            return xx;


        }





    }

    //Mohamed  and Schwarz(1999)

    public class Adaptive2
    {

        double Alpha;

        public Adaptive2(double alpha)
        {
            Alpha = alpha;


        }


        public Matrix<double> UpdateR(Matrix<double> R, Matrix<double> residual, Matrix<double> transform, Matrix<double> coVariance)
        {


            var xx = (R + R.Multiply(Alpha)) - transform * coVariance * transform.Transpose();

            return xx;


        }



        public Matrix<double> UpdateQ(Matrix<double> R, Matrix<double> innovation, Matrix<double> kalmanGain)
        {


            var xx = (kalmanGain * (R + R.Multiply(Alpha)) * kalmanGain.Transpose());

            return xx;


        }





    }


    public class Adaptive3
    {

        double Alpha;

        public Adaptive3(double alpha)
        {
            Alpha = alpha;


        }


        public Matrix<double> UpdateR(Matrix<double> R, Matrix<double> residual, Matrix<double> transform, Matrix<double> coVariance)
        {


            var xx = (residual) * Alpha;

            return xx;


        }



        public Matrix<double> UpdateQ(Matrix<double> Q, Matrix<double> innovation, Matrix<double> kalmanGain)
        {


            var xx = innovation * Alpha;

            return xx;


        }





    }

}
