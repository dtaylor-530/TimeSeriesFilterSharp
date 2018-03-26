using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using KalmanFilter;
using KalmanFilter.Wrap;


using System.Threading.Tasks;
using MathNet.Filtering.Kalman;

namespace KalmanFilter.Wrap
{



    public class DiscreteWrapper
    {



        Matrix<double> Q { get; set; }

        Matrix<double> R { get; set; }


        //Matrix<double> x;
        //Matrix<double> P;
        //Matrix<double> Mean { get { return x; } set { x = value; } }
        //Matrix<double> CoVariance { get { return P; } set { P = value;  } }


        Matrix<double> H { get; set; }

        public IAdaptiveQ AdaptiveQ { get; set; }


        public IAdaptiveR AdaptiveR { get; set; }


        Matrix<double> F { get; set; }

        private DiscreteKalmanFilter dfilter;

        Matrix<double> G { get; set; }


        public double MeanSquaredError { get; private set; }



        VectorBuilder<double> Vbuilder;

        MatrixBuilder<double> Mbuilder;



        public DiscreteWrapper(double[] measurementnoise, double[] processnoise)
        {


            Mbuilder = Matrix<double>.Build;
            Vbuilder = Vector<double>.Build;

            Initialise(measurementnoise, processnoise);
        }






        public void Initialise(double[] r, double[] q, double[] x = null)
        {


            R = Mbuilder.Diagonal(r).Map(_ => Math.Pow(_, 2)); //covariance of measurement  
            Q = Mbuilder.Diagonal(q).Map(_ => Math.Pow(_, 2)); //covariance of process

            AdaptiveQ = new DefaultAdaptiveQ(q);
            AdaptiveR = new DefaultAdaptiveR(r);


            Matrix<double> mx;
            Matrix<double> P;

            if (x == null)
            {
                mx = Mbuilder.Dense(q.Length, 1, 0);  //s + q * Matrix.Build.Random(1, 1); //initial state with noise
                P = Mbuilder.Diagonal(q.Length, q.Length, 1); //initial state covariance
            }
            else
            {
                mx = Mbuilder.DenseOfColumnArrays(x);
                P = Mbuilder.Diagonal(q.Length, q.Length, x); //initial state covariance

            }

            dfilter = new MathNet.Filtering.Kalman.DiscreteKalmanFilter(mx, P);


            F = StateFunctions.BuildTransition(q.Length, Mbuilder);

            G = Mbuilder.Diagonal(q.Length, q.Length, 1); //covariance of process
            H = StateFunctions.BuildMeasurement(q.Length, r.Length, Mbuilder);



        }




        public async Task<IDictionary<DateTime, Tuple<Matrix<double>, Matrix<double>>>> BatchRunAsync(List<Tuple<DateTime, double[]>> meas,
            IProgress<KeyValuePair<DateTime, Tuple<Matrix<double>, Matrix<double>>>> progressHandler = null, int delay = 500)
        {

            DateTime dt = meas.First().Item1;



            return await Task.Run(async () =>
            {
                IDictionary<DateTime, Tuple<Matrix<double>, Matrix<double>>> estimates = new Dictionary<DateTime, Tuple<Matrix<double>, Matrix<double>>>();

                for (int k = 0; k < meas.Count(); k++)
                {
                    TimeSpan ts = meas[k].Item1 - dt;
                    dt = meas[k].Item1;
                    F.UpdateTransition(ts);
                    var xP = dfilter.PredictState(ts, F, G, AdaptiveQ.Value);

                    var kvp = new KeyValuePair<DateTime, Tuple<Matrix<double>, Matrix<double>>>(dt, Tuple.Create(xP.Item1, xP.Item2));
                    estimates.Add(kvp);


                    progressHandler?.Report(kvp);

                    if (delay > 0)
                    {
                        await Task.Run(() => System.Threading.Thread.Sleep(delay));
                    }

                    Update(ts, meas[k].Item2);

                }


                return estimates;

            });


            //double meanSquaredError = errorSumSquared / meas.Count();

            //return meanSquaredError;
            //NotifyChanged(nameof(MeanSquaredError));

        }







        public IDictionary<DateTime, Tuple<Matrix<double>, Matrix<double>>> BatchRun(IEnumerable<Tuple<DateTime, Matrix<double>>> measurements)
        {
            DateTime dt = measurements.First().Item1;


            IDictionary<DateTime, Tuple<Matrix<double>, Matrix<double>>> estimates = new Dictionary<DateTime, Tuple<Matrix<double>, Matrix<double>>>();
            foreach (var meas in measurements)
            {

                TimeSpan ts = meas.Item1 - dt;
                dt = meas.Item1;

                Predict(ts, dt, ref estimates);

                Update(ts, meas.Item2);

            }

            return estimates;
            //double meanSquaredError = errorSumSquared / meas.Count();

            //return meanSquaredError;
            //NotifyChanged(nameof(MeanSquaredError));

        }


        public IDictionary<DateTime, Tuple<Matrix<double>, Matrix<double>>> BatchRunRecursive(List<Tuple<DateTime, Matrix<double>>> meas)
        {

            DateTime dt = meas.First().Item1;


            IDictionary<DateTime, Tuple<Matrix<double>, Matrix<double>>> estimates = new Dictionary<DateTime, Tuple<Matrix<double>, Matrix<double>>>();

            for (int k = 0; k < meas.Count(); k++)
            {

                TimeSpan ts = meas[k].Item1 - dt;
                dt = meas[k].Item1;

                Predict(ts, dt, ref estimates);
                Update(ts, meas[k].Item2);


                for (int i=k;i>-1;i--)
                    Run(dt-meas[i].Item1, meas[i].Item2);
         
                for (int j = 0; j < k;j++)
                    Run(meas[j].Item1 - dt, meas[j].Item2);
                
            }

            return estimates;

        }



        public IDictionary<DateTime, Tuple<double[], double[]>> RunAsBatch(List<Tuple<DateTime, double[]>> meas)
        {

            DateTime dt = meas.First().Item1;

            IDictionary<DateTime, Tuple<double[], double[]>> estimates = new Dictionary<DateTime, Tuple<double[], double[]>>();


            for (int k = 0; k < meas.Count(); k++)
            {
                TimeSpan ts = meas[k].Item1 - dt;
                dt = meas[k].Item1;

                Predict(ts, dt, ref estimates);

                Update(ts, meas[k].Item2);

            }


            return estimates;


        }











        public Tuple<Matrix<double>, Matrix<double>> Run(TimeSpan ts, double[] z)
        {

            dfilter.PredictState(ts, F, G, AdaptiveQ.Value);

            Update(ts, z);

            return Tuple.Create(dfilter.State, dfilter.Cov);
        }



        public Tuple<Matrix<double>, Matrix<double>> Run(TimeSpan ts, Matrix<double> z)
        {

            dfilter.PredictState(ts, F, G, AdaptiveQ.Value);

            Update(ts, z);

            return Tuple.Create(dfilter.State, dfilter.Cov);
        }





        public void Predict(TimeSpan ts, DateTime dt, ref IDictionary<DateTime, Tuple<double[], double[]>> estimates)
        {
            var xP = dfilter.PredictState(ts, F, G, AdaptiveQ.Value);

            var kvp = new KeyValuePair<DateTime, Tuple<double[], double[]>>(dt, Tuple.Create(xP.Item1.Column(0).AsArray(), xP.Item2.Diagonal().ToArray()));

            estimates.Add(kvp);


        }


        public Tuple<Matrix<double>, Matrix<double>> Predict(TimeSpan ts, DateTime dt)
        {
            var xP = dfilter.PredictState(ts, F, G, AdaptiveQ.Value);


            return xP;
   
        }



        public Matrix<double> Update(TimeSpan ts, double[] z)
        {
            var mz = Mbuilder.DenseOfColumnArrays(z);

            var diff = mz - H * dfilter.State;

            Update(ts, mz);

            return diff;
        }







        public void Predict(TimeSpan ts, DateTime dt, ref IDictionary<DateTime, Tuple<Matrix<double>, Matrix<double>>> estimates)
        {
            var xP = dfilter.PredictState(ts, F, G, AdaptiveQ.Value);



            var kvp = new KeyValuePair<DateTime, Tuple<Matrix<double>, Matrix<double>>>(dt, Tuple.Create(xP.Item1, xP.Item2));
            estimates.Add(kvp);
        }



        public Matrix<double> Update(TimeSpan ts, Matrix<double> z)
        {

            var diff = (z - H * dfilter.State).PointwiseAbs();
            dfilter.UpdateQ(AdaptiveQ, ts, z, H, AdaptiveR.Value);

            dfilter.UpdateState(z, H, AdaptiveR.Value);


            dfilter.UpdateR(AdaptiveR, ts, z, H);

            return diff;


        }





    }










    public static class MathNetDiscreteKalmanFilterWrap
    {



        public static Tuple<Matrix<double>, Matrix<double>> PredictState(this DiscreteKalmanFilter dfilter, TimeSpan ts, Matrix<double> F, Matrix<double> G, Matrix<double> Q)
        {

            F.UpdateTransition(ts);

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



