using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  KalmanFilter
{
    //public class AEKF
    //{
    //    // <summary>
    //    // Adaptive Adjustment of Noise Covariance in Kalman 
    //    // FilterSharp for Dynamic State Estimation
    //    // https://arxiv.org/ftp/arxiv/papers/1702/1702.00884.pdf
    //    // </summary>

    //    double Alpha;

    //    public AEKF(double alpha)
    //    {
    //        Alpha = alpha;


    //    }


    //    public Matrix<double> UpdateR(Matrix<double> R, Matrix<double> residual, Matrix<double> transform, Matrix<double> coVariance)
    //    {


    //        var xx = R.Multiply(Alpha) + (1 - Alpha) * (residual * residual.Transpose() + transform * coVariance * transform.Transpose());

    //        return xx;


    //    }



    //    public Matrix<double> UpdateQ(Matrix<double> Q, Vector<double> innovation, Matrix<double> kalmanGain)
    //    {


    //        var xx = Q.Multiply(Alpha) + (1 - Alpha) * (kalmanGain * innovation * innovation* kalmanGain.Transpose());
  
    //        return xx;


    //    }





    //}











    //////Mohamed  and Schwarz(1999)

    ////public class Adaptive2
    ////{

    ////    double Alpha;

    ////    public Adaptive2(double alpha)
    ////    {
    ////        Alpha = alpha;


    ////    }


    ////    public Matrix<double> UpdateR(Matrix<double> R, Matrix<double> residual, Matrix<double> transform, Matrix<double> coVariance)
    ////    {


    ////        var xx = (R + R.Multiply(Alpha)) - transform * coVariance * transform.Transpose();

    ////        return xx;


    ////    }



    ////    public Matrix<double> UpdateQ(Matrix<double> R, Matrix<double> innovation, Matrix<double> kalmanGain)
    ////    {


    ////        var xx = (kalmanGain * (R + R.Multiply(Alpha)) * kalmanGain.Transpose());

    ////        return xx;


    ////    }





    ////}


    //public class Adaptive3
    //{

    //    double Alpha1;

    //    double Alpha2;

    //    public Adaptive3(double alpha1,double alpha2)
    //    {
    //        Alpha1 = alpha1;
    //        Alpha2 = alpha2;

    //    }


    //    public Matrix<double> UpdateR(Matrix<double> residual)
    //    {


    //        var xx = (residual) * Alpha1;

    //        return xx;


    //    }



    //    public Matrix<double> UpdateQ(Matrix<double> innovation)
    //    {


    //        var xx = innovation * Alpha2;

    //        return xx;


    //    }





    //}

    //public class Adaptive5
    //{



    //    public Adaptive5()
    //    {


    //    }


    //    public Matrix<double> UpdateR(Matrix<double> R, Matrix<double> residual, Matrix<double> transform, Matrix<double> coVariance)
    //    {


    //        return (residual * residual.Transpose() + transform * coVariance * transform.Transpose());

    


    //    }



    //    public Matrix<double> UpdateQ(Matrix<double> R, Matrix<double> innovation, Matrix<double> kalmanGain)
    //    {


    //        return (kalmanGain * innovation * innovation * kalmanGain.Transpose());



    //    }





    //}


}
