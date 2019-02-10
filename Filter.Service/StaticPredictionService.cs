
using Filter.Common;
using FilterSharp.Model;
using Filter.Service;

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
    public class StaticPredictionService
    {



        //public static IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> RunDefaultGaussianProcess(IEnumerable<KeyValuePair<DateTime, double>> meas)
        //{
        //    var gpds = MultiGaussianProcessFactory.BuildDefault(200);
        //    return gpds.BatchRun(meas);

        //}

        //public static IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> RunDefaultKalman(IEnumerable<KeyValuePair<DateTime, double>> meas)
        //{
        //    var kf =new DiscreteOuterWrapper ( di: DiscreteWrapperFactory.BuildDefault() );

        //    return kf.BatchRun(meas);

        //}

        public static IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> RunMultiKalman(IEnumerable<KeyValuePair<DateTime, double>> meas)
        {

            var mw = new KalmanFilter.Wrap.MultiWrapper();

            return mw.BatchRun(meas);

        }


        public static IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> RunAccordKalman(IEnumerable<KeyValuePair<DateTime, double>> meas)
        {

            var kf = new KalmanFilter.Wrap.AccordKalmanFilterWrapper(2);
            return kf.BatchRun(meas);

        }


        public static IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> RunDefaultParticleFilter(IEnumerable<KeyValuePair<DateTime, double>> meas)
        {
            var f = new ParticleFilter.Wrap.ParticleFilterWrapper();
            return f.BatchRun(meas);

        }



        //public static PredictionsViewModel MakeRecursiveKalmanViewModel(Dispatcher dispatcher)
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

        //    var ui = new DispatcherScheduler(dispatcher);
        //    return new PredictionsViewModel(meas, u, ui);


        //}


    }



}


