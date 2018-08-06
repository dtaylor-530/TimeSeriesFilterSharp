
using Filter.Common;

using KalmanFilter.Wrap;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Filter.Model;
using Filter.Service;
using System.Reactive.Linq;

namespace Filter.ViewModel
{



    public class KalmanFilterViewModel : PredictionViewModel
    {


        public ObservableCollection<Estimate> VelocityEstimates { get; set; }

        public void RunOptimised()
        {
            var meas = MathNet.Numerics.Generate.Sinusoidal(40, 1000, 100, 10)
                .Select((_, i) => new KeyValuePair<DateTime, double>(new DateTime() + TimeSpan.FromSeconds(i), 20 + MathNet.Numerics.Distributions.Normal.Sample(_, 1)));

            var x = Filter.Optimisation.BGWorker.GetOptimisedKFPredictions(meas, 100);

            x.Subscribe(u =>
            {
                Estimates = new ObservableCollection<Estimate>(u.Select(_ => new Estimate(_.Key, _.Value[0].Item1, _.Value[0].Item2)));
                VelocityEstimates = new ObservableCollection<Estimate>(u.Select(_ => new Estimate(_.Key, _.Value[1].Item1, _.Value[1].Item2)));
                NotifyChanged(nameof(Estimates));
                NotifyChanged(nameof(VelocityEstimates));
            });
            Measurements = new ObservableCollection<KeyValuePair<DateTime, double>>(meas);


            NotifyChanged(nameof(Measurements));

        }







        public void Run(double c, double d)
        {

            var meas = MathNet.Numerics.Generate.Sinusoidal(40, 1000, 100, 10)
                .Select((_, i) => new KeyValuePair<DateTime, double>(new DateTime() + TimeSpan.FromSeconds(i), 20 + MathNet.Numerics.Distributions.Normal.Sample(_, 1)));

            Func<double, double, IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> f = (a, b) =>
            {
                var mw = new KalmanFilter.Wrap.DiscreteOuterWrapper(a, b, 1);
                return mw.BatchRun(meas);
            };

            var fcd = f(c, d);
            Estimates = new ObservableCollection<Estimate>(fcd.Select(_ => new Estimate(_.Key, _.Value[0].Item1, _.Value[0].Item2)));
            VelocityEstimates = new ObservableCollection<Estimate>(fcd.Select(_ => new Estimate(_.Key, _.Value[1].Item1, _.Value[1].Item2)));

            NotifyChanged(nameof(Estimates));
            NotifyChanged(nameof(VelocityEstimates));
            Measurements = new ObservableCollection<KeyValuePair<DateTime, double>>(meas);


            NotifyChanged(nameof(Measurements));


        }



        public void RunDelayed(double c, double d)
        {

            var meas = MathNet.Numerics.Generate.Sinusoidal(40, 1000, 100, 10)
                .Select((_, i) => new KeyValuePair<DateTime, double>(new DateTime() + TimeSpan.FromHours(i), 20 + MathNet.Numerics.Distributions.Normal.Sample(_, 1)));

            var omeas = Observable.Interval(TimeSpan.FromSeconds(0.3)).Zip(meas.ToObservable(), (a, b) => b);

            var mw = new KalmanFilter.Wrap.DiscreteOuterWrapper(c, d, 1);

            var otpt = mw.Run(omeas.Select(_=> new KeyValuePair<DateTime, double?>(_.Key,_.Value)));
            Estimates = new ObservableCollection<Estimate>();
            VelocityEstimates = new ObservableCollection<Estimate>();
            otpt.ObserveOnDispatcher().Subscribe(_ =>
            {
                Estimates.Add(new Estimate(_.Key, _.Value[0].Item1, _.Value[0].Item2));
                VelocityEstimates.Add(new Estimate(_.Key, _.Value[1].Item1, _.Value[1].Item2));
            });

            NotifyChanged(nameof(Estimates));
            NotifyChanged(nameof(VelocityEstimates));

            Measurements = new ObservableCollection<KeyValuePair<DateTime, double>>(meas);


            NotifyChanged(nameof(Measurements));


        }








        public void Smooth()
        {
            //if (u == null){ MessageBox.Show("No data to smooth"); return; }

            //var Q = Matrix<double>.Build.DenseDiagonal(q.Length, q.Length, 0.1d);


            //int k = u.First().Value.Item1.RowCount;
            //for (int i = 0; i < k; i++)
            //{

            //    var us = KalmanFilter.Smoother.Smooth(u, KalmanFilter.StateFunctions.BuildTransition(k), Q);


            //    Mes[i].SmoothedEstimates = new ObservableCollection<Estimate>(us.Select(_ =>
            //      new Estimate(_.Key, _.Value.Item1[i, 0], _.Value.Item2[i, i])));
            //    Mes[i].n();


            //}

            //NotifyChanged(nameof(Mes), nameof(MeanSquaredError));



        }








    




    }













}
