using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Filtering.Kalman;
using System.Reactive.Linq;
using System.Reactive.Concurrency;

namespace KalmanFilter.Wrap
{



    public static class DiscreteFactory
    {

        public static MathNet.Filtering.Kalman.DiscreteKalmanFilter Build(int size, double[] x = null)
        {

            Matrix<double> mx = UtilityMath.MatrixBuilder.Instance.Builder.DenseOfColumnArrays(x);
            Matrix<double> P = UtilityMath.MatrixBuilder.Instance.Builder.Diagonal(size, size, x); //initial state covariance

            return new MathNet.Filtering.Kalman.DiscreteKalmanFilter(mx, P);
        }


        public static MathNet.Filtering.Kalman.DiscreteKalmanFilter Build(int size)
        {

            Matrix<double> mx = UtilityMath.MatrixBuilder.Instance.Builder.Dense(size, 1, 0);  //s + q * Matrix.Build.Random(1, 1); //initial state with noise
            Matrix<double> P = UtilityMath.MatrixBuilder.Instance.Builder.Diagonal(size, size, 1); //initial state covariance

            return new MathNet.Filtering.Kalman.DiscreteKalmanFilter(mx, P);
        }


    }




    public static class DiscreteWrapperEx
    {






        //public static IEnumerable<KeyValuePair<DateTime, Tuple<Matrix<double>, Matrix<double>>>> BatchRun(this DiscreteWrapper wrap, IEnumerable<Tuple<DateTime, Matrix<double>>> measurements)
        //{
        //    DateTime dt = measurements.First().Item1;

        //    var dFilterSharp = DiscreteFactory.Build(wrap.Size);
        //    //IList<KeyValuePair<DateTime, Tuple<Matrix<double>, Matrix<double>>>> estimates = new List<KeyValuePair<DateTime, Tuple<Matrix<double>, Matrix<double>>>>();
        //    foreach (var meas in measurements)
        //    {

        //        TimeSpan ts = meas.Item1 - dt;
        //        dt = meas.Item1;

        //        var xP = wrap.Predict(dFilterSharp, ts);

        //        yield return new KeyValuePair<DateTime, Tuple<Matrix<double>, Matrix<double>>>(dt, Tuple.Create(xP.Item1, xP.Item2));

        //        dFilterSharp = wrap.Update(dFilterSharp, ts, meas.Item2);

        //    }

        //}





        //public static IEnumerable<KeyValuePair<DateTime, Tuple<Matrix<double>, Matrix<double>>>> BatchRunRecursive(this DiscreteOuterWrapper wrap, List<Tuple<DateTime, Matrix<double>>> meas)
        //{

        //    DateTime dt = meas.First().Item1;
        //    var dFilterSharp = DiscreteFactory.Build(wrap.Size);
        //    for (int k = 0; k < meas.Count(); k++)
        //    {

        //        TimeSpan ts = meas[k].Item1 - dt;
        //        dt = meas[k].Item1;

        //        var xP = wrap.Predict(dFilterSharp, ts);
        //        yield return new KeyValuePair<DateTime, Tuple<Matrix<double>, Matrix<double>>>(dt, Tuple.Create(xP.Item1, xP.Item2));


        //        for (int i = k; i > -1; i--)
        //            wrap.Run(ref dFilterSharp, dt - meas[i].Item1, meas[i].Item2);

        //        for (int j = 0; j < k; j++)
        //            wrap.Run(ref dFilterSharp, meas[j].Item1 - dt, meas[j].Item2);

        //    }



        //}



        //public static IEnumerable<KeyValuePair<DateTime, Tuple<double[], double[]>>> BatchArrayRun(this DiscreteInnerWrapper wrap, IEnumerable<Tuple<DateTime, double[]>> measurements)
        //{

        //    DateTime dt = measurements.First().Item1;
        //    var dFilterSharp = DiscreteFactory.Build(wrap.Size);
        //    foreach (var meas in measurements)
        //    {
        //        TimeSpan ts = meas.Item1 - dt;
        //        dt = meas.Item1;

        //        var xP = wrap.Predict(dFilterSharp, ts);
        //        yield return new KeyValuePair<DateTime, Tuple<double[], double[]>>(dt, Tuple.Create(xP.Item1.Column(0).AsArray(), xP.Item2.Diagonal().ToArray()));
        //        dFilterSharp = wrap.Update(dFilterSharp, ts, meas.Item2);

        //    }


        //}



        //public static Tuple<Matrix<double>, Matrix<double>> Run(this DiscreteWrapper wrap, ref DiscreteKalmanFilter dFilterSharp, TimeSpan ts, double[] z)
        //{
        //    var xP = wrap.Predict(dFilterSharp, ts);

        //    dFilterSharp = wrap.Update(dFilterSharp, ts, z);

        //    return xP;
        //}



        //public static Tuple<Matrix<double>, Matrix<double>> Run(this DiscreteWrapper wrap, ref DiscreteKalmanFilter dFilterSharp, TimeSpan ts, Matrix<double> z)
        //{

        //    var xP = wrap.Predict(dFilterSharp, ts);

        //    dFilterSharp = wrap.Update(dFilterSharp, ts, z);

        //    return xP;
        //}













        //    public async Task<IDictionary<DateTime, Tuple<Matrix<double>, Matrix<double>>>> BatchRunAsync(List<Tuple<DateTime, double[]>> meas,
        //IProgress<KeyValuePair<DateTime, Tuple<Matrix<double>, Matrix<double>>>> progressHandler = null, int delay = 500)
        //    {

        //        DateTime dt = meas.First().Item1;



        //        return await Task.Run(async () =>
        //        {
        //            IDictionary<DateTime, Tuple<Matrix<double>, Matrix<double>>> estimates = new Dictionary<DateTime, Tuple<Matrix<double>, Matrix<double>>>();

        //            for (int k = 0; k < meas.Count(); k++)
        //            {
        //                TimeSpan ts = meas[k].Item1 - dt;
        //                dt = meas[k].Item1;
        //                F.UpdateTransition(ts);
        //                var xP = dFilterSharp.PredictState(ts, F, G, AdaptiveQ.Value);

        //                var kvp = new KeyValuePair<DateTime, Tuple<Matrix<double>, Matrix<double>>>(dt, Tuple.Create(xP.Item1, xP.Item2));
        //                estimates.Add(kvp);


        //                progressHandler?.Report(kvp);

        //                if (delay > 0)
        //                {
        //                    await Task.Run(() => System.Threading.Thread.Sleep(delay));
        //                }

        //                Update(ts, meas[k].Item2);

        //            }

        //            return estimates;

        //        });


        //        //double meanSquaredError = errorSumSquared / meas.Count();

        //        //return meanSquaredError;
        //        //NotifyChanged(nameof(MeanSquaredError));

        //    }


    }
}
