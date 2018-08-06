using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Filtering.Kalman;

namespace KalmanFilter
{
    public class Smoother
    {



        public static  List<Tuple<Vector<double>, Matrix<double>>> Smooth(List<Tuple<Vector<double>, Matrix<double>>> estimates,
       Matrix<double> Q, ITimeFunction f)
        {


            var smoothedEstimates = new List<Tuple<Vector<double>, Matrix<double>>>();

            for (int i = estimates.Count() - 2; i > 0; i--)
            {


                var P = estimates[i].Item2;
                var x = estimates[i].Item1;

                var F= f.Matrix(1);

                // predicted covariance
                var Pp = F.Multiply(P).Multiply(F.Transpose()) + Q;


                var K = P.Multiply(F.Transpose()).Multiply(Pp.Inverse());
                x += K.Multiply(estimates[i + 1].Item1 - F.Multiply(x));
                P += K.Multiply(estimates[i + 1].Item2 - Pp).Multiply(K.Transpose());


                smoothedEstimates.Add(Tuple.Create(x,P));

            }

            return smoothedEstimates;

        }



    }
}
