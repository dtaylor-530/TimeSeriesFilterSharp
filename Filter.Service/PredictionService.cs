using AccordGenetic.Wrapper;
using Filter.Common;
using Filter.Model;
using Filter.Optimisation;
using Filter.Service;
using GaussianProcess.Wrap;
using KalmanFilter.Wrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using UtilityEnum;
using UtilityHelper;
using UtilityReactive;

namespace Filter.Service
{



    //public class PredictionService
    //{

    //    public IObservable<Tuple<KeyValuePair<DateTime, double>[], IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> Predictions { get; }

    //    public IObservable<Tuple<KeyValuePair<DateTime, double>[], IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> LastPredictions { get; }



    //    public PredictionService(IObservable<KeyValuePair<DateTime, double>> series, IScheduler scheduler)
    //    {
    //        List<KeyValuePair<DateTime, double>> lst = new List<KeyValuePair<DateTime, double>>();

    //        series.SubscribeOn(scheduler).Subscribe(_ => lst.Add(_), () => Console.WriteLine("finished"));


    //        var y = ObservableFactory.Build(() => Predict(lst), TimeSpan.FromSeconds(5), scheduler)
    //            .Where(_ =>
    //            _ != null)
    //           .TakeUntil(series.Delay(TimeSpan.FromSeconds(10)).GetAwaiter());

    //        LastPredictions = y.Select(_ => _.GetAwaiter()).Switch();

    //        Predictions = y.Switch();
    //    }



    //    public static IObservable<Tuple<KeyValuePair<DateTime, double>[], IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> Predict(IEnumerable<KeyValuePair<DateTime, double>> alst)
    //    {
    //        KeyValuePair<DateTime, double>[] salst = null;

    //        if (alst.Count() > 100)
    //            salst = alst.TakeLast(100).ToArray();
    //        else if (alst.Count() <= 100 & alst.Count() > 1)
    //            salst = alst.ToArray();
    //        else
    //            return null;

    //        //var prds = Filter.Optimisation.BGWorker.GetOptimisedGPPredictions(salst, 10).Take(TimeSpan.FromSeconds(5));
    //        var prds = Filter.Optimisation.BGWorker.GetOptimisedKFPredictions(salst, 100).Take(TimeSpan.FromSeconds(15));
    //        return prds.Select(s => Tuple.Create(salst, s));
    //    }


    //}


    //public class PredictionService
    //{

    //    public IObservable<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> Predictions { get; }

    //    //public IObservable<Tuple<KeyValuePair<DateTime, double>[], IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> LastPredictions { get; }



    //    public PredictionService(IObservable<KeyValuePair<DateTime, double>> series, IScheduler scheduler)
    //    {
    //        List<KeyValuePair<DateTime, double>> lst = new List<KeyValuePair<DateTime, double>>();

    //        series.SubscribeOn(scheduler).Subscribe(_ => lst.Add(_), () => Console.WriteLine("finished"));


    //        var y = ObservableFactory.Build(() => Trim(lst), TimeSpan.FromSeconds(1), scheduler)
    //            .Where(_ =>
    //            _ != null).Distinct()
    //           .TakeUntil(series.Delay(TimeSpan.FromSeconds(0)).GetAwaiter());

    //        Predictions = Filter.Optimisation.BGWorker.GetOptimisedKFPredictions(y, 1)/*.Take(TimeSpan.FromSeconds(15))*/;

    //    }




    //    public static KeyValuePair<DateTime, T>[] Trim<T>(IEnumerable<KeyValuePair<DateTime, T>> alst)
    //    {

    //        if (alst.Count() > 100)
    //            return alst.TakeLast(100).ToArray();
    //        else if (alst.Count() <= 100 & alst.Count() > 1)
    //            return alst.ToArray();
    //        else
    //            return null;

    //    }


    //}

    public class PredictionService2
    {

        public IObservable<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> Predictions { get; }

        //public IObservable<Tuple<KeyValuePair<DateTime, double>[], IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> LastPredictions { get; }



        public PredictionService2(IObservable<KeyValuePair<DateTime, double>> series, IScheduler scheduler)
        {
            List<KeyValuePair<DateTime, double>> lst = new List<KeyValuePair<DateTime, double>>();

            series.SubscribeOn(scheduler).Subscribe(_ => lst.Add(_), () => Console.WriteLine("finished"));


            var y = ObservableFactory.Build(() => Helper.Trim(lst), TimeSpan.FromSeconds(1), scheduler)
                .Where(_ =>
                _ != null).Distinct()
               .TakeUntil(series.Delay(TimeSpan.FromSeconds(0)).GetAwaiter());

            Predictions = Filter.Optimisation.BGWorker.GetOptimisedGPPredictions(y, 1)/*.Take(TimeSpan.FromSeconds(15))*/;

        }







    }


    public class PredictionService3
    {

        public IObservable<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> Predictions { get; }

        //public IObservable<Tuple<KeyValuePair<DateTime, double>[], IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> LastPredictions { get; }



        public PredictionService3(IObservable<KeyValuePair<DateTime, double>> series, IScheduler scheduler)
        {
            List<KeyValuePair<DateTime, double>> lst = new List<KeyValuePair<DateTime, double>>();

            series.SubscribeOn(scheduler).Subscribe(_ => lst.Add(_), () => Console.WriteLine("finished"));


            var y = ObservableFactory.Build(() => Helper.Trim(lst), TimeSpan.FromSeconds(3), scheduler)
                .Where(_ =>
                _ != null);
            //.TakeUntil(series.Delay(TimeSpan.FromSeconds(0)).GetAwaiter());

            Predictions = Filter.Optimisation.BGWorker.GetOptimisedKFPredictions(y, 40).Select(_ => _.Output);/*.Take(TimeSpan.FromSeconds(15))*/;

        }





    }

    public class PredictionService4
    {

        public IObservable<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> Predictions { get; }

        //public IObservable<Tuple<KeyValuePair<DateTime, double>[], IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> LastPredictions { get; }



        public PredictionService4(IObservable<KeyValuePair<DateTime, Tuple<double, double>>> series, IScheduler scheduler)
        {
            List<KeyValuePair<DateTime, Tuple<double, double>>> lst = new List<KeyValuePair<DateTime, Tuple<double, double>>>();

            series.SubscribeOn(scheduler).Subscribe(_ => lst.Add(_), () => Console.WriteLine("finished"));


            var y = ObservableFactory.Build(() => Helper.Trim(lst), TimeSpan.FromSeconds(3), scheduler)
                .Where(_ =>
                _ != null);
            //.TakeUntil(series.Delay(TimeSpan.FromSeconds(0)).GetAwaiter());

            var prs1 = Filter.Optimisation.BGWorker.GetOptimisedKFPredictions(y.Select(_ => _.Select(_d => new KeyValuePair<DateTime, double>(_d.Key, _d.Value.Item1)).ToArray()), 5).Select(_ => _.Output);/*.Take(TimeSpan.FromSeconds(15))*/;

            var prs2 = Filter.Optimisation.BGWorker.GetOptimisedKFPredictions(y.Select(_ => _.Select(_d => new KeyValuePair<DateTime, double>(_d.Key, _d.Value.Item2)).ToArray()), 5).Select(_ => _.Output);

            Predictions = prs1.Zip(prs2, (c, d) => c.Zip(d, (e, f) => new KeyValuePair<DateTime, Tuple<double, double>[]>(f.Key, Combine(e.Value, f.Value))));

        }

        public static Tuple<double, double>[] Combine(Tuple<double, double>[] a, Tuple<double, double>[] b)
        {

            return a.Zip(b, (c, d) => new Tuple<double, double>((c.Item1 + d.Item1) / 2, (c.Item2 + d.Item2) / 256)).ToArray();
        }




    }




    public class PredictionService5
    {

        public IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>> Predictions { get; }


        public PredictionService5(IObservable<KeyValuePair<DateTime, Tuple<double, double>>> series, IScheduler scheduler)
        {
            var mw1 = new KalmanFilter.Wrap.DiscreteOuterWrapper(5, 5, 1);
            var mw2 = new KalmanFilter.Wrap.DiscreteOuterWrapper(5, 5, 1);

            var prs1 = mw1.Run(series.Select(_d => new KeyValuePair<DateTime, double?>(_d.Key, _d.Value.Item1)));
            var prs2= mw2.Run(series.Select(_d => new KeyValuePair<DateTime, double?>(_d.Key, _d.Value.Item2)));

            Predictions = prs1.Zip(prs2, (c, d) =>  new KeyValuePair<DateTime, Tuple<double, double>[]>(c.Key, Combine(c.Value, d.Value)));

        }


        public static Tuple<double, double>[] Combine(Tuple<double, double>[] a, Tuple<double, double>[] b)
        {

            return a.Zip(b, (c, d) => new Tuple<double, double>((c.Item1 + d.Item1) / 2, (c.Item2 + d.Item2) / 256)).ToArray();
        }




    }
    public class PredictionService6
    {

        public IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>> Predictions { get; }


        public PredictionService6(IObservable<KeyValuePair<DateTime,double>> series, IScheduler scheduler)
        {

            var mw1 = new KalmanFilter.Wrap.DiscreteOuterWrapper(5, 5, 1);
    
            Predictions = mw1.Run(series.Select(_=>new  KeyValuePair < DateTime, double? >(_.Key,_.Value)));

        }


        public static Tuple<double, double>[] Combine(Tuple<double, double>[] a, Tuple<double, double>[] b)
        {

            return a.Zip(b, (c, d) => new Tuple<double, double>((c.Item1 + d.Item1) / 2, (c.Item2 + d.Item2) / 256)).ToArray();
        }




    }


    static class Helper
        {

        static object lck = new object();
                public static KeyValuePair<DateTime, T>[] Trim<T>(IEnumerable<KeyValuePair<DateTime, T>> alst)
    {
            lock (lck)
            {
                if (alst.Count() > 100)
                    return alst.TakeLast(100).ToArray();
                else if (alst.Count() <= 100 & alst.Count() > 1)
                    return alst.ToArray();
                else
                    return null;
            }
    }

}





    //public class PredictionService  // : IPredictionService
    //{

    //    public IObservable<IEnumerable<Estimate>> PositionPredictions { get; set; }
    //    public IObservable<IEnumerable<Estimate>> VelocityPredictions { get; set; }


    //    public PredictionService(IEnumerable<KeyValuePair<DateTime, double>> measurements, IScheduler scheduler)
    //    {
    //        var av = Class1.MakeOptimisedPredictions(measurements, scheduler);


    //        PositionPredictions = av.Select(b => b.Select(a => new Estimate(a.Key, a.Value[0].Item1, a.Value[0].Item2)));
    //        VelocityPredictions = av.Select(b => b.Select(a => new Estimate(a.Key, a.Value[1].Item1, a.Value[1].Item2)));
    //    }


    //    public PredictionService(IObservable<KeyValuePair<DateTime, double>> measurements, IScheduler scheduler)
    //    {
    //        var av = Class1.MakeOptimisedPredictions(measurements, scheduler);

    //        PositionPredictions = av.Select(b => b.Select(a => new Estimate(a.Key, a.Value[0].Item1, a.Value[0].Item2)));
    //        VelocityPredictions = av.Select(b => b.Select(a => new Estimate(a.Key, a.Value[1].Item1, a.Value[1].Item2)));
    //    }

    //}




    //public class PredictionService2  // : IPredictionService
    //{

    //    public ISubject<IEnumerable<Estimate>> PositionPredictions { get; set; }
    //    public ISubject<IEnumerable<Estimate>> VelocityPredictions { get; set; }


    //    public PredictionService2(IObservable<IEnumerable<KeyValuePair<DateTime, double>>> measurements, IScheduler scheduler)
    //    {

    //        var avd = measurements.Select(meas => Class1.MakeOptimisedPredictions(meas, scheduler));
    //        avd.Subscribe(av =>
    //        {
    //            var xxx = av.Subscribe(b =>
    //            {
    //                PositionPredictions.OnNext(b.Select(a => new Estimate(a.Key, a.Value[0].Item1, a.Value[0].Item2)));
    //                VelocityPredictions.OnNext(b.Select(a => new Estimate(a.Key, a.Value[1].Item1, a.Value[1].Item2)));
    //            });
    //        });
    //    }



    //}

    //static class Helper
    //{


    //    public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int n)
    //    {
    //        return source.Skip(Math.Max(0, source.Count() - n));
    //    }

    //}


}



//public static class ServiceHelper
//{


//    public static IObservable<KeyValuePair<DateTime, System.Windows.Point>> ToTimePoints(this IObservable<KeyValuePair<DateTime, double?>> meas)
//    {

//        return meas.Select(_ => new KeyValuePair<DateTime, System.Windows.Point>(_.Key, _.Value == null ? default(System.Windows.Point) : new System.Windows.Point(0, (double)_.Value)));
//    }



//}
