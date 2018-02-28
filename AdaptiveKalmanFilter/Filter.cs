using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveKalmanFilter
{
    // based on:
    //https://arxiv.org/ftp/arxiv/papers/1702/1702.00884.pdf

    //public class Filter
    //{

    //    double Alpha;

    //    public Filter(double alpha)
    //    {
    //        Alpha = alpha;


    //    }


    //    public Matrix<double>  UpdateR(Matrix<double> R,  Matrix<double> residual , Matrix<double> transform,Matrix<double> coVariance )
    //    {


    //        var xx = R.Multiply(Alpha) + (1 - Alpha) * (residual * residual.Transpose() + transform * coVariance * transform.Transpose() );

    //        return xx;


    //    }



    //    public  Matrix<double> UpdateQ(Matrix<double> Q, Matrix<double> innovation, Matrix<double> kalmanGain)
    //    {


    //        var xx = Q.Multiply(Alpha) + (1 - Alpha) * (kalmanGain * innovation*innovation.Transpose() * kalmanGain.Transpose());

    //        return xx;


    //    }





    //}

    ////Mohamed  and Schwarz(1999)

    //public class Filter2
    //{

    //    double Alpha;

    //    public Filter2(double alpha)
    //    {
    //        Alpha = alpha;


    //    }


    //    public Matrix<double> UpdateR(Matrix<double> R, Matrix<double> residual, Matrix<double> transform, Matrix<double> coVariance)
    //    {


    //        var xx = (R + R.Multiply(Alpha)) - transform * coVariance * transform.Transpose();

    //        return xx;


    //    }



    //    public Matrix<double> UpdateQ(Matrix<double> R, Matrix<double> innovation, Matrix<double> kalmanGain)
    //    {


    //        var xx =  (kalmanGain * (R+R.Multiply(Alpha)) *  kalmanGain.Transpose());

    //        return xx;


    //    }





    //}


}
