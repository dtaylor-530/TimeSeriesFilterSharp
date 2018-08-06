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
    public class VMFactory
    {


        public static Filter.Model.Observable<IEnumerable<KeyValuePair<DateTime, double>>> BuildMeasurements()
        {
            var x = new Filter.Model.Observable<IEnumerable<KeyValuePair<DateTime, double>>>();

            return x;
        }

        public static IEnumerable<ButtonDefinition> BuildMeasurements(Filter.Model.Observable<IEnumerable<KeyValuePair<DateTime, double>>> x)
        {
            Action<IEnumerable<KeyValuePair<DateTime, double>>> av = (a) => x.Publish(a);

            return ButtonDefinitionFactory.Build(av, typeof(Filter.Service.SignalGenerator));
        }




        //public static IObservable<StaticViewModel> BuildStaticService(IObservable<IEnumerable<KeyValuePair<DateTime, double>>> observable,IScheduler uischeduler)
        //{
        //    return observable.Select(s =>
        //    {
        //       return BuildStatic(s, uischeduler);

        //    });
        //}




        public static StaticViewModel BuildStatic(/*IObservable<IEnumerable<KeyValuePair<DateTime, double>>> s,*/IScheduler uischeduler)
        {
            var s = new Filter.Model.Observable<IEnumerable<KeyValuePair<DateTime, double>>>();

            var ts = s.Select(_ => new TimeValueSeriesViewModel(_));


            Action<IEnumerable<KeyValuePair<DateTime, double>>> avi = (a) => s.Publish(a);

            var bds = ButtonDefinitionFactory.Build(avi, typeof(Filter.Service.SignalGenerator));



            var types = Helper.GetInheritingTypes(typeof(Filter.Model.IFilterWrapper));

            var vm = new GenericViewModel<Type>(types.ToObservable(), null, uischeduler);

            var bdds = vm.GetSelectedObjectSteam().Where(a=>a!=null).Select(_ => 
            ViewModelSelector(_));

            var dfs = bdds.Select(_ => _.Output).Select(v => v.WithLatestFrom(s, (a, b) => new { a, b }).Select(l => new PredictionsViewModel(l.a.BatchRun(l.b)))).SelectMany(l => l);


            //var xs = new Filter.Model.Observable<Type>();

            //Action<Type> at = (a) => xs.Publish(a);

            //var bds = ButtonDefinitionFactory.Build(avi, typeof(Filter.Service.SignalGenerator));


            //var x = new Filter.Model.Observable<PredictionsViewModel>();

            //Action<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> av = (a) => x.Publish(new PredictionsViewModel(a));

            //var defs = s.Select(_ => ButtonDefinitionFactory.Build(av, typeof(Filter.Service.StaticPredictionService), _));

            return new StaticViewModel(bds, vm, ts, dfs, bdds);
        }





        public static IFilterWrapperViewModel ViewModelSelector(Type type)
        {

            switch (type.FullName)

            {
                default:
                case (nameof(GaussianProcessWrapper)):
                    {
                        var kernels = GaussianProcess.KernelHelper.LoadKernels().ToDictionary(_ => _.GetDescription());
                        return new GaussianProcessViewModel(kernels);
                    }

            }




            //public static DynamicViewModel BuildDynamic(IScheduler uischeduler,IScheduler scheduler)
            //{
            //    var x = new Filter.Model.Observable<PredictionsViewModel>();

            //    Action<IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>>> av = (a) => x.Publish(new PredictionsViewModel(a,scheduler));

            //    var defs = ButtonDefinitionFactory.Build<IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>>>(av, typeof(Filter.Service.DynamicPredictionServiceFactory));

            //    return new DynamicViewModel(defs, x);

            //}






            //public static PredictionsViewModel MakeGaussianProcessViewModel(IEnumerable<Tuple<DateTime, double>> meas)
            //{
            //    var gpds = MultiGaussianProcessFactory.BuildDefault(200);
            //    var u = gpds.BatchRun(meas);
            //    return new PredictionsViewModel(u);

            //}











            //public static PredictionsViewModel MakeKalmanViewModel(IEnumerable<Tuple<DateTime, double>> meas)
            //{


            //    double[] q = new double[] { 10, 10 };
            //    double[] r = new double[] { 1 };
            //    var kf = new KalmanFilter.Wrap.DiscreteWrapper(r, q);

            //    //    //kf.AdaptiveQ = new AEKFKFQ(q);
            //    //    //kf.AdaptiveR = new AEKFKFR(r);

            //    return kf.BatchRun(meas);




            //}


            //public static PredictionsViewModel MakeMultiKalmanViewModel(IEnumerable<Tuple<DateTime,double>> meas)
            //{

            //    var mw = new KalmanFilter.Wrap.MultiWrapper();

            //    var u = mw.BatchRun(meas);

            //    //var ui = new DispatcherScheduler(dispatcher);
            //    return new PredictionsViewModel( u);


            //}



            ////public static PredictionsViewModel MakeRecursiveKalmanViewModel(Dispatcher dispatcher)
            ////{
            ////    var newThread = NewThreadScheduler.Default;
            ////    var meas = Filter.Service.TimeValueServiceFactory.MakeMeasurementUnknownServiceDefault(100, newThread);
            ////         var kf = new KalmanFilter.Wrap.DiscreteWrapper(r, q);

            ////    //    //kf.AdaptiveQ = new AEKFKFQ(q);
            ////    //    //kf.AdaptiveR = new AEKFKFR(r);

            ////    kf = new KalmanFilter.Wrap.DiscreteWrapper(new double[] { 1 }, new double[] { 10, 10 });

            ////    //kf.AdaptiveQ = new AEKFKFQ(q);
            ////    //kf.AdaptiveR = new AEKFKFR(r);

            ////    u = kf.BatchRunRecursive(meas);

            ////    var ui = new DispatcherScheduler(dispatcher);
            ////    return new PredictionsViewModel(meas, u, ui);


            ////}




            //public static PredictionsViewModel MakeAccordKalmanViewModel(IEnumerable<Tuple<DateTime, double>> meas)
            //{


            //    var kf = new KalmanFilter.Wrap.AccordKalmanFilterWrapper(2);
            //    var u = kf.BatchRun(meas);
            //    //var ui = new DispatcherScheduler(dispatcher);
            //    return new PredictionsViewModel( u);


            //}




            //public static PredictionsViewModel MakeParticleFilterViewModel(IEnumerable<Tuple<DateTime, double>> meas)
            //{


            //    var measpoints = meas.ToTimePoints();
            //    var f = new ParticleFilter.Wrap.Wrapper();
            //    var u = f.BatchRun1D(measpoints, -10, 10);
            //    //var ui = new DispatcherScheduler(dispatcher);
            //    return new PredictionsViewModel( u);


            //}



        }






    }
}