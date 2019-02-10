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





        public static Tuple<Matrix<double>, Matrix<double>> PredictState(this DiscreteKalmanFilter dFilterSharp, TimeSpan ts, Matrix<double> F, Matrix<double> G, Matrix<double> Q)
        {

           // F.UpdateTransition(ts);

            dFilterSharp.Predict(F, G, Q);

            return Tuple.Create(dFilterSharp.State, dFilterSharp.Cov);
        }



        public static Tuple<Matrix<double>, Matrix<double>> UpdateState(this DiscreteKalmanFilter dFilterSharp, Matrix<double> z, Matrix<double> H, Matrix<double> R)
        {

            dFilterSharp.Update(z, H, R);

            return Tuple.Create(dFilterSharp.State, dFilterSharp.Cov);

        }

        public static void UpdateQ(this DiscreteKalmanFilter dFilterSharp, IAdaptiveQ adaptiveQ, TimeSpan ts, Matrix<double> z, Matrix<double> H, Matrix<double> R)
        {
            //Matrix<double> z = mBuilder.DenseOfColumnArrays(newMeas);
            var innovation = z - H * dFilterSharp.State;

            var kalmanGain = dFilterSharp.Cov * H.Transpose() * (H * dFilterSharp.Cov * H.Transpose() + R).Inverse();

            adaptiveQ.Update(ts, innovation, kalmanGain);/*Map(_ => Math.Abs(_)).AsColumnMajorArray();*/


        }


        public static void UpdateR(this DiscreteKalmanFilter dFilterSharp, IAdaptiveR adaptiveR, TimeSpan ts, Matrix<double> z, Matrix<double> H)
        {


            var residual = z - H * dFilterSharp.State;


            adaptiveR.Update(ts, residual, H, dFilterSharp.Cov);/*Map(_ => Math.Abs(_)).AsColumnMajorArray();*/


        }






        public static List<Tuple<long, double[]>> ToTimeSpans(List<Tuple<DateTime, Matrix<double>>> meas)
        {


            List<Tuple<long, double[]>> ddf = new List<Tuple<long, double[]>>();

            DateTime dt = meas.First().Item1;
            foreach (var x in meas)
            {
                var ts = x.Item1 - dt;
                ddf.Add(Tuple.Create(ts.Ticks, x.Item2.AsColumnArrays().First()));

                dt = x.Item1;

            }


            return ddf;



        }






        //a = adap.UpdateR(residual).Map(_ => Math.Abs(_)).AsColumnMajorArray();

        //R = Mbuilder.DenseOfDiagonalArray(R.RowCount, R.ColumnCount, a);

        //return Tuple.Create(dFilterSharp.State, dFilterSharp.Cov);


    }
}
