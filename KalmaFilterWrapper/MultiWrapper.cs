using Filter.Utility;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalmanFilter.Wrap
{

    public class MultiWrapper
    {
        private List<DiscreteWrapper> kfs;


        public IEnumerable<Tuple<DateTime, Tuple<double[], double[]>>> Run(IEnumerable<Tuple<DateTime, double>> measurements)
        {

            kfs = KalmaFilterFactory.Build((1,300), (1, 2));

            List<Tuple<DateTime, Tuple<double[], double[]>>> u = new List<Tuple<DateTime, Tuple<double[],double[]>>>();

            DateTime dt = measurements.First().Item1;

            List<Matrix<double>> nweights =
                Enumerable.Range(0, kfs.Count()).Select(_ => Matrix<double>.Build.DenseOfRowArrays(new double[] { 0.00000000001d })).ToList();

            foreach (var meas in measurements.ToMatrices())
            {
                TimeSpan ts = meas.Item1 - dt;
                dt = meas.Item1;

                var prd = PredictWeighted(ts, dt, nweights);

                var eval = Evaluate(prd);

                var weights = Update(ts, meas.Item2).ToList();

                nweights = Filter.Utility.Normaliser.Normalise(weights.ToList()).ToList();

                u.Add(Tuple.Create(dt, eval));

            }
            return u;


        }



        public IEnumerable<(Matrix<double>, Matrix<double>)> PredictWeighted(TimeSpan ts, DateTime dt, List<Matrix<double>> weights)
        {
            for (int i = 0; i < kfs.Count; i++)
            {
                var xp = kfs[i].Predict(ts, dt);
                if (weights[i].RowCount == 1 & weights[i].ColumnCount == 1)
                    yield return (weights[i][0, 0] * xp.Item1,( weights[i][0, 0]/100000000) * xp.Item2);
                else
                    yield return (xp.Item1 * (weights[i]), (weights[i]) * xp.Item2);

            }

        }




        public Tuple<double[], double[]> Evaluate(IEnumerable<(Matrix<double>, Matrix<double>)> predictions)
        {

            return predictions
              .Aggregate((a, b) => (a.Item1 + b.Item1, a.Item2 + b.Item2)).ToTuple().ToArrays();

        }




        public IEnumerable<Matrix<double>> Update(TimeSpan ts, Matrix<double> measurement)
        {
            for (int i = 0; i < kfs.Count; i++)
            {
                var ddd = kfs[i].Update(ts, measurement);
                yield return ddd.Inverse();

            }
        }







    }



}
