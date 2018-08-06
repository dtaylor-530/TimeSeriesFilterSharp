using Filter.Service;
using Filter.Utility;
using GaussianProcess.Wrap;
using KalmanFilter.Wrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Filter.ViewModel
{
    //public class VMDynamicFactory
    //{
    //    public static PredictionsMeasurementsViewModel MakeGaussianProcessViewModel(IScheduler scheduler)
    //    {

    //        var newThread = NewThreadScheduler.Default;
    //        var meas = Filter.Service.TimeValueServiceFactory.MakeMeasurementUnknownServiceDefault(100, newThread);
    //        //meas.Subscribe(_ => Console.WriteLine(_));


    //        var gpds = MultiGaussianProcessFactory.BuildDefault(200);
    //        var o = gpds.Run(meas, newThread).SelectMany(_ => _.SelectMany(__ => __));
    //        return new PredictionsMeasurementsViewModel(meas, o, scheduler);


    //    }



    //    public static PredictionsMeasurementsViewModel MakeKalmanViewModel(IScheduler scheduler)
    //    {

    //        var newThread = NewThreadScheduler.Default;
    //        var meas = Filter.Service.TimeValueServiceFactory.MakeMeasurementUnknownServiceDefault(100, newThread);

    //        double[] q = new double[] { 10, 10 };
    //        double[] r = new double[] { 1 };
    //        var kf = new KalmanFilter.Wrap.DiscreteWrapper(r, q);

    //        //    //kf.AdaptiveQ = new AEKFKFQ(q);
    //        //    //kf.AdaptiveR = new AEKFKFR(r);

    //        var u = kf.Run(meas, newThread);


    //        return new PredictionsMeasurementsViewModel(meas, u, scheduler);


    //    }


    //    public static PredictionsMeasurementsViewModel MakeMultiKalmanViewModel(IScheduler scheduler)
    //    {


    //        var meas = ServiceHelper.GetMeasurements();
    //        double[] q = new double[] { 10, 10 };
    //        double[] r = new double[] { 1 };
    //        var kf = new KalmanFilter.Wrap.DiscreteWrapper(r, q);

    //        var mw = new KalmanFilter.Wrap.MultiWrapper();

    //        var u = mw.Run(meas);


    //        return new PredictionsMeasurementsViewModel(meas, u, scheduler);


    //    }



    //    //public static PredictionsViewModel MakeRecursiveKalmanViewModel(IScheduler scheduler)
    //    //{
    //    //    var newThread = NewThreadScheduler.Default;
    //    //    var meas = Filter.Service.TimeValueServiceFactory.MakeMeasurementUnknownServiceDefault(100, newThread);
    //    //         var kf = new KalmanFilter.Wrap.DiscreteWrapper(r, q);

    //    //    //    //kf.AdaptiveQ = new AEKFKFQ(q);
    //    //    //    //kf.AdaptiveR = new AEKFKFR(r);

    //    //    kf = new KalmanFilter.Wrap.DiscreteWrapper(new double[] { 1 }, new double[] { 10, 10 });

    //    //    //kf.AdaptiveQ = new AEKFKFQ(q);
    //    //    //kf.AdaptiveR = new AEKFKFR(r);

    //    //    u = kf.BatchRunRecursive(meas);

    //    //    var scheduler = new DispatcherScheduler(dispatcher);
    //    //    return new PredictionsViewModel(meas, u, scheduler);


    //    //}




    //    public static PredictionsMeasurementsViewModel MakeAccordKalmanViewModel(IScheduler scheduler)
    //    {
    //        var newThread = NewThreadScheduler.Default;
    //        var meas = Filter.Service.TimeValueServiceFactory.MakeMeasurementServiceDefault(100, newThread);
    //        var kf = new KalmanFilter.Wrap.AccordKalmanFilterWrapper(2);
    //        var u = kf.Run(meas);

    //        return new PredictionsMeasurementsViewModel(meas, u, scheduler);


    //    }




    //    public static PredictionsMeasurementsViewModel MakeParticleFilterViewModel(IScheduler scheduler)
    //    {

    //        var meas = ServiceHelper.GetMeasurements();
    //        var measpoints = meas.ToTimePoints();
    //        var f = new ParticleFilter.Wrap.Wrapper();
    //        var u = f.BatchRun1D(measpoints, -10, 10);

    //        return new PredictionsMeasurementsViewModel(meas, u, scheduler);


    //    }



    //}






    //public static class ServiceHelper
    //{
    //    public static IObservable<Tuple<DateTime, double?>> GetMeasurements()
    //    {

    //        var newThread = NewThreadScheduler.Default;
    //        return Filter.Service.TimeValueServiceFactory.MakeMeasurementUnknownServiceDefault(100, newThread);
    //    }

    //    public static IObservable<Tuple<DateTime, System.Windows.Point>> ToTimePoints(this IObservable<Tuple<DateTime, double?>> meas)
    //    {

    //        return meas.Select(_ => Tuple.Create(_.Item1, _.Item2 == null ? default(System.Windows.Point) : new System.Windows.Point(0, (double)_.Item2)));
    //    }




    //}

}
