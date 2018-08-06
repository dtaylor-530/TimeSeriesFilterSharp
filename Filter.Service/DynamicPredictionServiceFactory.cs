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

namespace Filter.Service
{
    public class DynamicPredictionServiceFactory
    {
        //public static IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>> MakeGaussianProcessViewModel(IScheduler scheduler)
        //{

        //    var meas = Filter.Service.TimeValueServiceFactory.MakeMeasurementUnknownServiceDefault(100, scheduler);
        //    //meas.Subscribe(_ => Console.WriteLine(_));


        //    var gpds = MultiGaussianProcessFactory.BuildDefault(200);
        //    return gpds.Run(meas).SelectMany(_ => _.SelectMany(__ => __));

        //}

        public static IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>> RunGaussianProcess(IScheduler scheduler)
        {

            var meas = Filter.Service.TimeValueServiceFactory.MakeMeasurementUnknownServiceDefault(100, scheduler);
  

            var gpds = GaussianProcessWrapperFactory.BuildDefault(20);
            return gpds.Run(meas);

        }

        public static IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>> RunKalman(IScheduler scheduler)
        {

            var meas = Filter.Service.TimeValueServiceFactory.MakeMeasurementUnknownServiceDefault(100, scheduler);

            var kf = DiscreteWrapperFactory.BuildDefault();

            return kf.Run(meas);




        }


        public static IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>> RunMultiKalman(IScheduler scheduler)
        {
            var meas = Filter.Service.TimeValueServiceFactory.MakeMeasurementUnknownServiceDefault(100, scheduler);

            var mw = new KalmanFilter.Wrap.MultiWrapper();

            return mw.Run(meas);


        }



        //public static PredictionsViewModel MakeRecursiveKalmanViewModel(IScheduler scheduler)
        //{
        //    var newThread = NewThreadScheduler.Default;
        //    var meas = Filter.Service.TimeValueServiceFactory.MakeMeasurementUnknownServiceDefault(100, newThread);
        //         var kf = new KalmanFilter.Wrap.DiscreteWrapper(r, q);

        //    //    //kf.AdaptiveQ = new AEKFKFQ(q);
        //    //    //kf.AdaptiveR = new AEKFKFR(r);

        //    kf = new KalmanFilter.Wrap.DiscreteWrapper(new double[] { 1 }, new double[] { 10, 10 });

        //    //kf.AdaptiveQ = new AEKFKFQ(q);
        //    //kf.AdaptiveR = new AEKFKFR(r);

        //    u = kf.BatchRunRecursive(meas);

        //    var scheduler = new DispatcherScheduler(dispatcher);
        //    return new PredictionsViewModel(meas, u, scheduler);


        //}




        public static IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>> RunAccordKalman(IScheduler scheduler)
        {
            var meas = Filter.Service.TimeValueServiceFactory.MakeMeasurementUnknownServiceDefault(100, scheduler);
            var kf = new KalmanFilter.Wrap.AccordKalmanFilterWrapper(2);
            return kf.Run(meas);


        }




        public static IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>> RunParticleFilter(IScheduler scheduler)
        {

            var meas = Filter.Service.TimeValueServiceFactory.MakeMeasurementUnknownServiceDefault(100, scheduler);
            var f = new ParticleFilter.Wrap.ParticleFilterWrapper();
            return f.Run(meas);




        }



    }







    public static class ServiceHelper
    {
        public static IObservable<KeyValuePair<DateTime, double?>> GetMeasurements(IScheduler scheduler)
        {

            return Filter.Service.TimeValueServiceFactory.MakeMeasurementUnknownServiceDefault(100, scheduler);
        }





        public static IObservable<KeyValuePair<DateTime, System.Windows.Point>> ToTimePoints(this IObservable<KeyValuePair<DateTime, double?>> meas)
        {

            return meas.Select(_ => new KeyValuePair<DateTime, System.Windows.Point>(_.Key, _.Value == null ? default(System.Windows.Point) : new System.Windows.Point(0, (double)_.Value)));
        }



    }

}
