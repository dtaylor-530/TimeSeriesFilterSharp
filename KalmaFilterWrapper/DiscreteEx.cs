using MathNet.Filtering.Kalman;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalmanFilter.Wrap
{



    public static class MathNetDiscreteKalmanFilterEx
    {





        public static Tuple<Matrix<double>, Matrix<double>> PredictState(this DiscreteKalmanFilter dfilter, TimeSpan ts, Matrix<double> F, Matrix<double> G, Matrix<double> Q)
        {

           // F.UpdateTransition(ts);

            dfilter.Predict(F, G, Q);

            return Tuple.Create(dfilter.State, dfilter.Cov);
        }



        public static Tuple<Matrix<double>, Matrix<double>> UpdateState(this DiscreteKalmanFilter dfilter, Matrix<double> z, Matrix<double> H, Matrix<double> R)
        {

            dfilter.Update(z, H, R);

            return Tuple.Create(dfilter.State, dfilter.Cov);

        }

        public static void UpdateQ(this DiscreteKalmanFilter dfilter, IAdaptiveQ adaptiveQ, TimeSpan ts, Matrix<double> z, Matrix<double> H, Matrix<double> R)
        {
            //Matrix<double> z = mBuilder.DenseOfColumnArrays(newMeas);
            var innovation = z - H * dfilter.State;

            var kalmanGain = dfilter.Cov * H.Transpose() * (H * dfilter.Cov * H.Transpose() + R).Inverse();

            adaptiveQ.Update(ts, innovation, kalmanGain);/*Map(_ => Math.Abs(_)).AsColumnMajorArray();*/


        }


        public static void UpdateR(this DiscreteKalmanFilter dfilter, IAdaptiveR adaptiveR, TimeSpan ts, Matrix<double> z, Matrix<double> H)
        {


            var residual = z - H * dfilter.State;


            adaptiveR.Update(ts, residual, H, dfilter.Cov);/*Map(_ => Math.Abs(_)).AsColumnMajorArray();*/


        }






        //public static List<Tuple<long, double[]>> ToTimeSpans(List<Tuple<DateTime, Matrix<double>>> meas)
        //{


        //    List<Tuple<long, double[]>> ddf = new List<Tuple<long, double[]>>();

        //    DateTime dt = meas.First().Item1;
        //    foreach (var x in meas)
        //    {
        //        var ts = x.Item1 - dt;
        //        ddf.Add(Tuple.Create(ts.Ticks, x.Item2.AsColumnArrays().First()));

        //        dt = x.Item1;

        //    }


        //    return ddf;



        //}






        //a = adap.UpdateR(residual).Map(_ => Math.Abs(_)).AsColumnMajorArray();

        //R = Mbuilder.DenseOfDiagonalArray(R.RowCount, R.ColumnCount, a);

        //return Tuple.Create(dfilter.State, dfilter.Cov);


    }
}
