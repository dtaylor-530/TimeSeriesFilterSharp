using AccordGenetic.Wrapper;
using Filter.Common;
using FilterSharp.Model;
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
using UtilityReactive;



namespace Filter.Service
{
    public interface IPredictionService
    {

        IObservable<Point3D> Solutions { get; }
    }

    public class PredictionsService : IPredictionService
    {

        public IObservable<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> Predictions { get; set; }
        public IObservable<Point3D> Solutions { get; }


        public PredictionsService(IObservable<TwoVariableInput> bdds, IObservable<IEnumerable<KeyValuePair<DateTime, double>>> s, IObservable<bool> bs, IObservable<Type> types, IScheduler scheduler)
        {

            var x = PredictionServiceHelper.GetCombinedObservable(bdds, s, bs, types, scheduler).DistinctUntilChanged()
                .Select(_ => PredictionServiceHelper.GetStaticOutput(_.Optimisation, _.Type, _.Filter, _.Measurements, scheduler)
                   .SubscribeOn(scheduler));

            Predictions = x.SelectMany(_ => _.Select(__ => __.Output));

            Solutions = x.SelectMany(_s => _s.Select(_ => new Point3D(_.Parameters[0], _.Parameters[1], _.Score)));


        }
    }



    public class DynamicPredictionService : IPredictionService
    {

        public IObservable<IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>>> Predictions { get; set; }
        public IObservable<Point3D> Solutions { get; }


        public DynamicPredictionService(IObservable<TwoVariableInput> bdds, IObservable<IEnumerable<KeyValuePair<DateTime, double>>> s, IObservable<bool> bs, IObservable<Type> types, IScheduler scheduler)
        {

            var x = PredictionServiceHelper.GetCombinedObservable(bdds, s, bs, types, scheduler)
                .DistinctUntilChanged()
                .Select(_ => PredictionServiceHelper.GetDynamicOutput(_.Optimisation, _.Type, _.Filter, _.Measurements.ByTimeStamp().Publish(), scheduler)
                   .SubscribeOn(scheduler));

            Predictions = x.SelectMany(_ => _.Select(__ => __.Output));
            Solutions = x.SelectMany(_s => _s.Select(_ => new Point3D(_.Parameters[0], _.Parameters[1], _.Score)));

        }
    }


    //public class DynamicPredictionService2  // : IPredictionService
    //{

    //    public IObservable<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> Predictions { get; set; }
    //    public IObservable<Point3D> Solutions { get; }


    //    public DynamicPredictionService2(IObservable<IEnumerable<KeyValuePair<DateTime, double>>> measurements, IScheduler scheduler)
    //    {

    //        var xx = new Optimisation.TPL();
    //        var av = measurements.Select(_ => xx.GetOptimisedPredictions(_, 100));

    //        Predictions = av.Select(_ => _.Last());
    //    }

    //}
}
