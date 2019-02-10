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


    public static class PredictionServiceHelper
    {

        public static IObservable<KeyValuePair<DateTime,double>> GetPositions(this IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>> fgg)
        {

            return fgg.Select(_ => new KeyValuePair<DateTime, double>(_.Key, _.Value[0].Item1));

        }


        public static IObservable<KeyValuePair<DateTime, double>> GetVelocities(this IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>> fgg)
        {

            return fgg.Select(_ => new KeyValuePair<DateTime, double>(_.Key, _.Value[1].Item1));

        }





        internal static IObservable<Output> GetCombinedObservable(IObservable<TwoVariableInput> bdds, IObservable<IEnumerable<KeyValuePair<DateTime, double>>> s, IObservable<bool> bs, IObservable<Type> types, IScheduler scheduler)
        {

            return bdds.DistinctUntilChanged().SubscribeOn(scheduler)
                   //.TakeWhile(bs, false)
                   .CombineLatest(types.DistinctUntilChanged()/*.TakeWhile(bs, true)*/, (a, b) => new { Filter = a, Type = b })
                   .CombineLatest(s.DistinctUntilChanged(), (a, b) => new { FT = a, Measurements = b })
                   .CombineLatest(bs.DistinctUntilChanged(), (c, d) =>
                   new Output { Optimisation = d, Type = c.FT.Type, Filter = c.FT.Filter, Measurements = c.Measurements });
        }


        internal class Output
        {
            public Type Type { get; internal set; }
            public TwoVariableInput Filter { get; internal set; }
            public IEnumerable<KeyValuePair<DateTime, double>> Measurements { get; internal set; }
            public bool Optimisation { get; internal set; }
        }


        public static IObservable<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> GetStaticOutput(bool optimise, Type type, TwoVariableInput filter, IEnumerable<KeyValuePair<DateTime, double>> measurements, IScheduler scheduler)
        {

            if (optimise)
                //return Optimisation.Class1.GetOptimisedOutput(type, measurements).SubscribeOn(scheduler);
                return Observable.Repeat(PredictionServiceHelper.GetNonOptimisedOutput(filter, measurements), 1).SubscribeOn(scheduler);
            else
                return Observable.Repeat(PredictionServiceHelper.GetNonOptimisedOutput(filter, measurements), 1).SubscribeOn(scheduler);
        }



        public static IObservable<IO<IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> GetDynamicOutput(bool optimise, Type type, TwoVariableInput filter, IConnectableObservable<KeyValuePair<DateTime, double>> measurements, IScheduler scheduler)
        {
            measurements.Connect();
            measurements.Subscribe(ddd =>
               Console.WriteLine(ddd.Key + " " + (DateTime.Now - ddd.Key).Seconds));

            if (optimise)
                //    return Filter.Optimisation.Class1.GetOptimisedOutput(measurements);
                return Observable.Repeat(PredictionServiceHelper.GetNonOptimisedOutput(filter, measurements), 1).SubscribeOn(scheduler);
            else
                return Observable.Repeat(PredictionServiceHelper.GetNonOptimisedOutput(filter, measurements), 1).SubscribeOn(scheduler);
        }



        //  public static AccordGenetic.Wrapper.TimeSeries2DOptimisation<IObservable<KeyValuePair<DateTime, double>>, IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>>>
        //GetOptimiser(List<KeyValuePair<DateTime, double>> z)
        //  {
        //      return new AccordGenetic.Wrapper.TimeSeries2DOptimisation<IObservable<KeyValuePair<DateTime, double>>, IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>>>
        //                 ((ss) => (a, b) => new GaussianProcessWrapper(a, b).Run(ss.Select(_=>new KeyValuePair<DateTime, double?>(_.Key,_.Value))), z.ToObservable(), ErrorHelper.GetErrorSum);

        //  }


        public static IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> GetNonOptimisedOutput(TwoVariableInput t, IEnumerable<KeyValuePair<DateTime, double>> e)
        {

            var run = TypeHelper.GetInstance<IFilterWrapper>(t.Filter, t.VarA, t.VarB).BatchRun(e).ToList();
            return new IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>
            {
                Output = run,
                Parameters = new double[] { t.VarA, t.VarB },
                Score = ErrorHelper.GetErrorSum(e, run)
            };

        }




        public static IO<IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>>> GetNonOptimisedOutput(TwoVariableInput t, IObservable<KeyValuePair<DateTime, double>> e)
        {

            var run = TypeHelper.GetInstance<IFilterWrapper>(t.Filter, t.VarA, t.VarB).Run(e.Select(_ => new KeyValuePair<DateTime, double?>(_.Key, _.Value)));


            return new IO<IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>>>
            {
                Output = run,
                Parameters = new double[] { t.VarA, t.VarB },
                Score = 0//ErrorHelper.GetErrorSum(e, run)
            };

        }






    }

}
