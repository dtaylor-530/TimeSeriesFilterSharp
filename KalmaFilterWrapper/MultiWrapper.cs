
using KalmanFilter.Wrap;
using MathNet.Filtering.Kalman;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using FilterSharp.Common;

using UtilityMath;


namespace KalmanFilter.Wrap
{


    //public class MultiWrapperFactory
    //{
    //    public static MultiWrapper Build(int size)
    //    {
    //        //var x = 
    //        return new MultiWrapper
    //        {
    //            Filters =Enumerable.Range(0,size).Select(_ => DiscreteWrapperFactory.BuildDefault()).ToArray()
    //            //dfs = x.
    //        };
    //    }



    //}



    public class MultiWrapper
    {
        public DiscreteInnerWrapper[] Filters { get; set; }
        //public DiscreteKalmanFilter[] dfs { get; set; }

        public IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> BatchRun(IEnumerable<KeyValuePair<DateTime, double?>> measurements)
        {

            //List<Tuple<DateTime, Tuple<double[], double[]>>> u = new List<Tuple<DateTime, Tuple<double[],double[]>>>();

            DateTime dt = measurements.First().Key;

            List<Matrix<double>> nweights =
                Enumerable.Range(0, Filters.Count()).Select(_ => Matrix<double>.Build.DenseOfRowArrays(new double[] { 0.00000000001d })).ToList();

            foreach (var meas in measurements.ToMatrices())
            {
                TimeSpan ts = meas.Key - dt;
                dt = meas.Key;

                var prd = PredictWeighted(ts, dt, nweights);

                var eval = Evaluate(prd).ToArray();
                if (meas.Value != null)
                    Update(ts, meas.Value);


                var weights = GetDifferences(meas.Value);

                nweights = Normaliser.Normalise(weights.ToList()).ToList();

                yield return new KeyValuePair<DateTime, Tuple<double, double>[]>(dt, eval);

            }

        }

        public IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> BatchRun(IEnumerable<KeyValuePair<DateTime, double>> measurements)
        {

            //kfs = KalmanFilterWrapperFactory.BuildMany((1, 300), (1, 2)).ToArray();
            //dfs = kfs.Select(_ => DiscreteFactory.Build(_.Size)).ToArray();

            //List<Tuple<DateTime, Tuple<double[], double[]>>> u = new List<Tuple<DateTime, Tuple<double[],double[]>>>();

            DateTime dt = measurements.First().Key;

            List<Matrix<double>> nweights =
                Enumerable.Range(0, Filters.Count()).Select(_ => Matrix<double>.Build.DenseOfRowArrays(new double[] { 0.00000000001d })).ToList();

            foreach (var meas in measurements.ToMatrices())
            {
                TimeSpan ts = meas.Key - dt;
                dt = meas.Key;

                var prd = PredictWeighted(ts, dt, nweights);

                var eval = Evaluate(prd).ToArray();

                Update(ts, meas.Value);


                var weights = GetDifferences(meas.Value);

                nweights = Normaliser.Normalise(weights.ToList()).ToList();

                yield return new KeyValuePair<DateTime, Tuple<double, double>[]>(dt, eval);

            }

        }


        public IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>> Run(IObservable<KeyValuePair<DateTime, double?>> measurements)
        {

            //kfs = KalmanFilterWrapperFactory.BuildMany((1, 300), (1, 2)).ToArray();
            //dfs = kfs.Select(_ => DiscreteFactory.Build(_.Size)).ToArray();

            //List<Tuple<DateTime, Tuple<double[], double[]>>> u = new List<Tuple<DateTime, Tuple<double[],double[]>>>();

            //DateTime dt = measurements.First().Item1;

            List<Matrix<double>> nweights =
                Enumerable.Range(0, Filters.Count()).Select(_ => Matrix<double>.Build.DenseOfRowArrays(new double[] { 0.00000000001d })).ToList();

            return measurements.IncrementalTimeOffsets().Select(meas =>
            {

                var prd = PredictWeighted(meas.Key.Item2, meas.Key.Item1, nweights);

                var eval = Evaluate(prd).ToArray();


                if (meas.Value != null)
                {
                    var mtrx = meas.Value == null ? null : Matrix<double>.Build.DenseOfColumnArrays(new double[] { (double)meas.Value });
                    Update(meas.Key.Item2, mtrx);
                    var weights = GetDifferences(mtrx);
                    nweights = Normaliser.Normalise(weights.ToList()).ToList();
                }


                return new KeyValuePair<DateTime, Tuple<double, double>[]>(meas.Key.Item1, eval);

            });


        }




        //public dfsd(TimeSpan ts,DateTime dt,double[] dd)
        //{

        //    var prd = PredictWeighted(ts, dt, nweights);

        //    var eval = Evaluate(prd).ToArray();

        //    Update(ts, meas.Item2);


        //    var weights = GetDifferences(meas.Item2);

        //    nweights = Filter.Utility.Normaliser.Normalise(weights.ToList()).ToList();

        //    yield return new KeyValuePair<DateTime, Tuple<double, double>[]>(dt, eval);




        //}


        public IEnumerable<(Matrix<double>, Matrix<double>)> PredictWeighted(TimeSpan ts, DateTime dt, List<Matrix<double>> weights)
        {
            for (int i = 0; i < Filters.Length; i++)
            {
                var xp = Filters[i].Predict(ts);
                if (weights[i].RowCount == 1 & weights[i].ColumnCount == 1)
                    yield return (weights[i][0, 0] * xp.Item1, (weights[i][0, 0] / 100000000) * xp.Item2);
                else
                    yield return (xp.Item1 * (weights[i]), (weights[i]) * xp.Item2);

            }

        }



        public IEnumerable<Tuple<double, double>> Evaluate(IEnumerable<(Matrix<double>, Matrix<double>)> predictions)
        {

            return predictions
              .Aggregate((a, b) => (a.Item1 + b.Item1, a.Item2 + b.Item2)).ToTuple().ToEnumerable();

        }




        public void Update(TimeSpan ts, Matrix<double> measurement)
        {
            for (int i = 0; i < Filters.Length; i++)
            {
                Filters[i].Update(ts, measurement);

            }
        }



        public IEnumerable<Matrix<double>> GetDifferences(Matrix<double> measurement)
        {
            for (int i = 0; i < Filters.Length; i++)
            {
                yield return Filters[i].GetDifference( measurement);

            }
        }



    }

   static class Helper
    {

        public static IEnumerable<Tuple<double, double>> ToEnumerable(this Tuple<Matrix<double>, Matrix<double>> target)
        {
            return target.Item1.Column(0).ToArray().Zip(target.Item2.Diagonal().ToArray(), (a, b) => Tuple.Create(a, b));


        }

        public static IEnumerable<KeyValuePair<DateTime, Matrix<double>>> ToMatrices(this IEnumerable<KeyValuePair<DateTime, double>> list)
        {

            return list.Select(_ => new KeyValuePair<DateTime, Matrix<double>>(
                _.Key,
                Matrix<double>.Build.DenseOfColumnArrays(new double[] { _.Value })));


        }

        public static IEnumerable<KeyValuePair<DateTime, Matrix<double>>> ToMatrices(this IEnumerable<KeyValuePair<DateTime, double?>> list)
        {

            return list.Select(_ => new KeyValuePair<DateTime, Matrix<double>>(
                _.Key, _.Value == null ? null :
                Matrix<double>.Build.DenseOfColumnArrays(new double[] { (double)_.Value })));


        }
    }

}
