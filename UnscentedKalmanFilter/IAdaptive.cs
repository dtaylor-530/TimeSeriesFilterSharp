using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalmanFilter
{
    public interface IAdaptiveQ
    {
        Matrix<double> Value { get;  }

        void Update(TimeSpan ts, Matrix<double> innovation, Matrix<double> kalmanGain);



    }

    public interface IAdaptiveR
    {
        Matrix<double> Value { get; }

        void Update(TimeSpan ts, Matrix<double> residual, Matrix<double> transform, Matrix<double> coVariance);


    }

}
