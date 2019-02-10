using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using  KalmanFilter;
using KalmanFilter.Wrap;
using System.Threading.Tasks;
using MathNet.Filtering.Kalman;
using System.Reactive.Linq;
using FilterSharp.Model;
using UtilityMath;

namespace KalmanFilter.Wrap
{


    public class DiscreteOuterWrapper : IFilterWrapper,FilterSharp.Model.ITwoVariableInitialiser
    {
        public DiscreteInnerWrapper FilterSharp { get; set; }


        public DiscreteOuterWrapper(double a, double b)
        {
            FilterSharp = DiscreteWrapperFactory.BuildDefault(a, b);

        }

        public DiscreteOuterWrapper(double a, double b,double c)
        {
            FilterSharp = DiscreteWrapperFactory.BuildDefault(a, b,c);

        }

        public void Initialise(double a, double b)
        {
          
        }

        public DiscreteOuterWrapper(DiscreteInnerWrapper di)
        {
            FilterSharp = di;
        }


        public IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>> Run(IObservable<KeyValuePair<DateTime, double?>> measurements)
        {


            //public static IObservable<IEnumerable<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> Run(this List<GPDynamic> gpds, IObservable<Tuple<DateTime, double?>> measurements, IScheduler scheduler)
            //{

            DateTime firstDate = default(DateTime);
            //var dFilterSharp = DiscreteFactory.Build(this.Size);
            return measurements
                        .Select(meas =>
                        {
                            var xP = FilterSharp.Predict(meas.Key - firstDate);

                            if (meas.Value != null)
                                FilterSharp.Update(meas.Key - firstDate, new double[] { (double)meas.Value });

                            return new KeyValuePair<DateTime, Tuple<double, double>[]>(meas.Key,
                                    xP.Item1.Column(0).Zip(xP.Item2.Diagonal(), (a, b) => Tuple.Create(a, b)).ToArray());

                 
                        });


        }


        public IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> BatchRun(IEnumerable<KeyValuePair<DateTime, double>> measurements)
        {
            DateTime dt = measurements.First().Key;

            var y= measurements.First();


            var v=VectorBuilder.Instance.Builder.DenseOfArray(new double[] { y.Value,0 });
            // convert measurements to difference from inital value 
            foreach (var meas in measurements.Select(_=>new KeyValuePair<DateTime,double>(_.Key,_.Value-y.Value)))
            {

                TimeSpan ts = meas.Key - dt;
                dt = meas.Key;

                var xP = FilterSharp.Predict(ts);

                // add initial measurement back in
                yield return new KeyValuePair<DateTime, Tuple<double, double>[]>(meas.Key,
                           (xP.Item1.Column(0) + v).Zip(xP.Item2.Diagonal(), (a, b) => Tuple.Create(a, b)).ToArray());

                FilterSharp.Update(ts, new double[] { (double)meas.Value });

            }

        }


    }

    public class DiscreteWrapperFactory
    {
        public static KalmanFilter.Wrap.DiscreteInnerWrapper BuildDefault(double r = 1, double q = 10)
        {
            double[] vq = new double[] { q, q };
            double[] vr = new double[] { r };

            //    //kf.AdaptiveQ = new AEKFKFQ(q);
            //    //kf.AdaptiveR = new AEKFKFR(r);
            return new KalmanFilter.Wrap.DiscreteInnerWrapper(vr, vq);
        }


        public static KalmanFilter.Wrap.DiscreteInnerWrapper BuildDefault(double r = 1, double q1 = 10,double q2=10)
        {
            double[] vq = new double[] { q1, q2 };
            double[] vr = new double[] { r };

            //    //kf.AdaptiveQ = new AEKFKFQ(q);
            //    //kf.AdaptiveR = new AEKFKFR(r);
            return new KalmanFilter.Wrap.DiscreteInnerWrapper(vr, vq);
        }
    }


    public class DiscreteInnerWrapper
    {
        //Matrix<double> x;
        //Matrix<double> P;
        //Matrix<double> Mean { get { return x; } set { x = value; } }
        //Matrix<double> CoVariance { get { return P; } set { P = value;  } }

        public DiscreteKalmanFilter  KalmanFilter { get; }

        public Matrix<double> Q { get; }

        public Matrix<double> R { get; }

        public Matrix<double> F { get;}

        public Matrix<double> G { get;  }

        public Matrix<double> H { get; }


        //public IAdaptiveQ AdaptiveQ { get; set; }


        //public IAdaptiveR AdaptiveR { get; set; }

        //public int Size { get { return Q.ColumnCount; } }





        public DiscreteInnerWrapper(double[] measurementnoise, double[] processnoise)
        {
             KalmanFilter = DiscreteFactory.Build(processnoise.Length);


            R = MatrixBuilder.Instance.Builder.Diagonal(measurementnoise).Map(_ => Math.Pow(_, 2)); //covariance of measurement  
            Q = MatrixBuilder.Instance.Builder.Diagonal(processnoise).Map(_ => Math.Pow(_, 2)); //covariance of process

            //AdaptiveQ = new DefaultAdaptiveQ(q);
            //AdaptiveR = new DefaultAdaptiveR(r);

            F = StateFunctions.BuildTransition(processnoise.Length);

            G = MatrixBuilder.Instance.Builder.Diagonal(processnoise.Length, processnoise.Length, 1); //covariance of process
            H = StateFunctions.BuildMeasurement(processnoise.Length, measurementnoise.Length);

        }



        public Tuple<Matrix<double>, Matrix<double>> Predict(TimeSpan ts)
        {

            return  KalmanFilter.PredictState(ts, F, G, Q);

        }


        public void Update( TimeSpan ts, double[] z)
        {
            var mz = MatrixBuilder.Instance.Builder.DenseOfColumnArrays(z);

            //var diff = mz - H * dFilterSharp.State;

            Update( ts, mz);

            //return diff;
        }




        public void Update( TimeSpan ts, Matrix<double> z)
        {


            //dFilterSharp.UpdateQ(AdaptiveQ, ts, z, H, AdaptiveR.Value);

             KalmanFilter.UpdateState(z, H, R);


            //dFilterSharp.UpdateR(AdaptiveR, ts, z, H);

            //return dFilterSharp;


        }




        public Matrix<double> GetDifference( Matrix<double> z)
        {

            return (z - H *  KalmanFilter.State).PointwiseAbs();
        }



  

        //public void Predict(TimeSpan ts, DateTime dt, ref IDictionary<DateTime, Tuple<double[], double[]>> estimates)
        //{
        //    var xP = dFilterSharp.PredictState(ts, F, G, AdaptiveQ.Value);

        //    var kvp = new KeyValuePair<DateTime, Tuple<double[], double[]>>(dt, Tuple.Create(xP.Item1.Column(0).AsArray(), xP.Item2.Diagonal().ToArray()));

        //    estimates.Add(kvp);


        //}

        //public void Predict(TimeSpan ts, DateTime dt, ref IDictionary<DateTime, Tuple<Matrix<double>, Matrix<double>>> estimates)
        //{
        //    var xP = dFilterSharp.PredictState(ts, F, G, AdaptiveQ.Value);



        //    var kvp = new KeyValuePair<DateTime, Tuple<Matrix<double>, Matrix<double>>>(dt, Tuple.Create(xP.Item1, xP.Item2));
        //    estimates.Add(kvp);
        //}
    }











}



