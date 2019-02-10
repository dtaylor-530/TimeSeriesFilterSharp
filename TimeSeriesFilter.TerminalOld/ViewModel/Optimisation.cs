using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GaussianProcess;
using MathNet.Numerics.Distributions;
using System.Collections.ObjectModel;
using System.ComponentModel;

using MathNet.Numerics.LinearAlgebra;

using FilterSharp.Model;
using GaussianProcess.Wrap;

using System.IO;
using System.Windows;
using System.Reflection;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Windows.Threading;
using System.Windows.Data;
using ReactiveUI;

namespace FilterSharp.ViewModel
{
    public class KernelOutput
    {
        public string Name { get; set; }

        public double Score { get; set; }

        public int Iteration { get; set; }


        public double[] Parameters { get; set; }

        public List<Estimate> Estimates { get; set; } = new List<Estimate>();
    }




    public class OptimisationViewModel : PredictionViewModel
    {
        [PropertyTools.DataAnnotations.Browsable(false)]

        private List<IObservable<long>> timers = new List<IObservable<long>>();
        private List<IObservable<long>> timers2 = new List<IObservable<long>>();
        public ObservableCollection<long> Timers { get; set; } = new ObservableCollection<long>();

        public ObservableCollection<Estimate> VelocityEstimates { get; set; }
        //[PropertyTools.DataAnnotations.Browsable(false)]
        //public List<KeyValuePair<string, WpfObservableRangeCollection<Estimate>>> Estimates { get; set; }

        public ObservableCollection<KernelOutput> Outputs { get; set; }


        public PredictionViewModel PredictionVM { get; set; }
        public int Count { get; set; } = 20;

        public int TimeOutSeconds { get; set; } = 5;

        //[PropertyTools.DataAnnotations.Browsable(false)]
        //public WpfObservableRangeCollection<Estimate> Estimates { get; set; } = new WpfObservableRangeCollection<Estimate>();



        private Type[] kernels;


        public OptimisationViewModel(List<KeyValuePair<DateTime, double>> points)
        {
            Measurements = new ObservableCollection<KeyValuePair<DateTime, double>>(points);

            kernels = GaussianProcess.KernelHelper.LoadKernels().ToArray();

            Outputs = new ObservableCollection<KernelOutput>();

            for (int i = 0; i < kernels.Count(); i++)
            {
                Outputs.Add(new KernelOutput { Name = kernels[i].Name });

                var timer = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1)).Take(TimeOutSeconds);
                //var timer2 = Observable.<long>().SkipUntil(timer);
                timers.Add(timer);
                //timers2.Add(timer2);
                if (Timers.Count < i + 1)
                    Timers.Add(0);

            }
        }




        public void Run(IScheduler s)
        {
            Parallel.For(0, 5, (i) => RunKernel(i, s));

           
        }

        public  void RunTest(IScheduler s)
        {
            PredictionServiceTest(Measurements.ToObservable(), TaskPoolScheduler.Default, s);

        }

        public void RunTest2(IScheduler s)
        {

           var ms= Measurements.Select(_ => new KeyValuePair<DateTime,Tuple< double,double>>(_.Key, Tuple.Create(_.Value-2,_.Value+2)));

            PredictionServiceTest(ms.ToObservable(), TaskPoolScheduler.Default, s);

        }




        private void RunKernel(int i, IScheduler s)
        {


            Action<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> action6 = (_) =>
            {
              
                    var o = _?.Output?.Select(sd =>
                         new Estimate(sd.Key, sd.Value[0].Item1, sd.Value[1].Item2));

                    Outputs[i] = new KernelOutput//.Estimates.AddRange(o);
                    {
                        Name = kernels[i].Name,
                        Iteration = (int)_?.Iteration,
                        Score = _.Score,
                        Parameters = _.Parameters,
                        Estimates = o.ToList()
                    };
                this.RaisePropertyChanged(nameof(Outputs));


             
            };

            bool TaskParallelLibrary =false;

            //if (TaskParallelLibrary)
            //{
            //    var kf = new Filter.Optimisation.TPL();
            //    kf.GetOptimisedSolutions(Measurements, Count, TimeSpan.FromSeconds(TimeOutSeconds), kernels[i]).ObserveOn(s).SubscribeOn(TaskPoolScheduler.Default)
            //        .Subscribe(_ => {
            //            //Application.Current.Dispatcher.Invoke(() =>
            //            //{
            //                action6(_);
            //           // });
            //        });
            //}
            //else
            //{
            //    var kf = Filter.Optimisation.BGWorker.GetOptimisedSolutions(Measurements, Count, TimeSpan.FromSeconds(TimeOutSeconds), kernels[i])
            //        .TakeUntil(timers[i].GetAwaiter())
            //        .ObserveOn(s)
            //       .Subscribe(_ => action6(_));
            //}

            //timers[i].ObserveOn(s).Subscribe(_ =>
            //{
            //    Timers[i] = _ + 1;
            //},()=>Console.WriteLine("completed timer"));


            //timers[i].GetAwaiter().Subscribe(_ =>
            //{
            //    Console.WriteLine("timer ignore elements");
            //});

        }



        //public void PredictionServiceTest(IObservable<KeyValuePair<DateTime, double>> series, IScheduler background, IScheduler ui)
        //{
        //    var ints = Observable.Interval(TimeSpan.FromMilliseconds(300));
        //    var se=series.Zip(ints, (a, b) => a);
        //    var ps = new Service.PredictionService(se, background);
        //    ps.Predictions.ObserveOn(ui).Subscribe(_ =>        
        //    {
        //        Estimates = new ObservableCollection<Estimate>();
        //        VelocityEstimates = new ObservableCollection<Estimate>();
        //        NotifyChanged(nameof(Estimates)); NotifyChanged(nameof(VelocityEstimates));
        //        Console.WriteLine("Size: " + _.Item2.Count());
        //        foreach (var __ in _.Item2)
        //        { Estimates.Add(new Estimate(__.Key, __.Value[0].Item1, __.Value[0].Item2));
        //            VelocityEstimates.Add(new Estimate(__.Key, __.Value[1].Item1, __.Value[1].Item2));
        //           /* Console.WriteLine(__.Key + " " + __.Value);*/ }
        //    });
        //    ps.LastPredictions.ObserveOn(ui).Subscribe(_ =>
        //    {
        //        Console.WriteLine("lastPrediction");
        //    });
        //}


        public void PredictionServiceTest(IObservable<KeyValuePair<DateTime, double>> series, IScheduler background, IScheduler ui)
        {

            var ints = Observable.Interval(TimeSpan.FromMilliseconds(300));

            var se = series.Zip(ints, (a, b) => a);


            //var ps = new Service.PredictionService2(se, background);

            //ps.Predictions.ObserveOn(ui).Subscribe(_ =>
            //{
            //    Estimates = new ObservableCollection<Estimate>();
            //    VelocityEstimates = new ObservableCollection<Estimate>();
            //    NotifyChanged(nameof(Estimates)); NotifyChanged(nameof(VelocityEstimates));
            //    //Console.WriteLine("Size: " + _.Item2.Count());
            //    foreach (var __ in _)
            //    {
            //        Estimates.Add(new Estimate(__.Key, __.Value[0].Item1, __.Value[0].Item2));
            //        VelocityEstimates.Add(new Estimate(__.Key, __.Value[1].Item1, __.Value[1].Item2));
            //        /* Console.WriteLine(__.Key + " " + __.Value);*/
            //    }
            //});



        }

        public void PredictionServiceTest(IObservable<KeyValuePair<DateTime, Tuple<double,double>>> series, IScheduler background, IScheduler ui)
        {

            var ints = Observable.Interval(TimeSpan.FromMilliseconds(300));

            var se = series.Zip(ints, (a, b) => a).Publish().RefCount();



            //var ps = new Service.PredictionService4(se, background);

            //ps.Predictions.ObserveOn(ui).Subscribe(_ =>
            //{
            //    Estimates = new ObservableCollection<Estimate>();
            //    VelocityEstimates = new ObservableCollection<Estimate>();
            //    NotifyChanged(nameof(Estimates)); NotifyChanged(nameof(VelocityEstimates));
            //    //Console.WriteLine("Size: " + _.Item2.Count());
            //    foreach (var __ in _)
            //    {
            //        Estimates.Add(new Estimate(__.Key, __.Value[0].Item1, __.Value[0].Item2));
            //        VelocityEstimates.Add(new Estimate(__.Key, __.Value[1].Item1, __.Value[1].Item2));
            //        /* Console.WriteLine(__.Key + " " + __.Value);*/
            //    }
            //});

       

        }

        public static Tuple<double, double>[] Combine(Tuple<double, double>[] a, Tuple<double, double>[] b)
        {

            return a.Zip(b, (c, d) => new Tuple<double, double>((c.Item1 + d.Item1) / 2, c.Item2 + d.Item2)).ToArray();
        }
    }

  

}