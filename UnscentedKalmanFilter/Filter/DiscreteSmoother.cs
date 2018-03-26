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
        private static MatrixBuilder<double> _mBuilder;

        static Smoother()
        {
            _mBuilder =Matrix<double>.Build;

        }

        public static Dictionary<DateTime, Tuple<Matrix<double>, Matrix<double>>>
            Smooth(IList<KeyValuePair<DateTime, Tuple<Matrix<double>, Matrix<double>>>> estimates,Matrix<double> F,Matrix<double> Q)
        {


            var smoothedEstimates = new Dictionary<DateTime, Tuple<Matrix<double>, Matrix<double>>>();


            DateTime dt = estimates.Last().Key;

            var keys = estimates.Select(_=>_.Key).ToArray();
            var values = estimates.Select(_ => _.Value).ToArray();

            var x = values.Last().Item1;
            var P = values.Last().Item2;

            return SmoothEstimates(x, P, keys, values, F, Q);

        }



        public static Dictionary<DateTime, Tuple<Matrix<double>, Matrix<double>>>
     Smooth(IDictionary<DateTime, Tuple<Matrix<double>, Matrix<double>>> estimates, Matrix<double> F, Matrix<double> Q)
        {

            var keys = estimates.Keys.ToArray();
            var values = estimates.Values.ToArray();

            var x = values.Last().Item1;
            var P = values.Last().Item2;

            return SmoothEstimates(x, P, keys, values, F, Q);

        }


        public static Dictionary<DateTime, Tuple<Matrix<double>, Matrix<double>>>
            SmoothEstimates(Matrix<double> x,Matrix<double> P,DateTime[] keys,Tuple<Matrix<double>,Matrix<double>>[] values, Matrix<double> F, Matrix<double> Q)
        {
            DateTime dt = keys.Last();
            var smoothedEstimates = new Dictionary<DateTime, Tuple<Matrix<double>, Matrix<double>>>();
            for (int i = keys.Count() - 1; i > -1; i--)
            {

                Smooth(keys[i] - dt, F, Q, values[i].Item1, values[i].Item2, ref x, ref P);
                dt = keys[i];
            }

            for (int i = 0; i <keys.Count(); i++)
            {

                Smooth(keys[i] - dt, F, Q, values[i].Item1, values[i].Item2, ref x, ref P);
                dt = keys[i];
                smoothedEstimates.Add(dt, Tuple.Create(x, P));

            }
            return smoothedEstimates;
        }




        public static void Smooth(TimeSpan ts, Matrix<double> F, Matrix<double> Q, Matrix<double> oldx, Matrix<double> oldP, ref Matrix<double> x, ref Matrix<double> P)
        {


            F.UpdateTransition(ts);


            // predicted covariance
            var Pp = F.Multiply(oldP).Multiply(F.Transpose()) + Q;


            var K = oldP.Multiply(F.Transpose()).Multiply(Pp.Inverse());
            x += K.Multiply(oldx - F.Multiply(x));
            P += K.Multiply(oldP - Pp).Multiply(K.Transpose());



        }
    }

    //public class Smoother
    //{



    //    public static  List<Tuple<Vector<double>, Matrix<double>>> Smooth(List<Tuple<Vector<double>, Matrix<double>>> estimates,
    //   Matrix<double> Q, ITimeFunction f)
    //    {


    //        var smoothedEstimates = new List<Tuple<Vector<double>, Matrix<double>>>();

    //        for (int i = estimates.Count() - 2; i > 0; i--)
    //        {


    //            var P = estimates[i].Item2;
    //            var x = estimates[i].Item1;

    //            var F= f.Matrix(1);

    //            // predicted covariance
    //            var Pp = F.Multiply(P).Multiply(F.Transpose()) + Q;


    //            var K = P.Multiply(F.Transpose()).Multiply(Pp.Inverse());
    //            x += K.Multiply(estimates[i + 1].Item1 - F.Multiply(x));
    //            P += K.Multiply(estimates[i + 1].Item2 - Pp).Multiply(K.Transpose());


    //            smoothedEstimates.Add(Tuple.Create(x,P));

    //        }

    //        return smoothedEstimates;

    //    }



    //}










}
