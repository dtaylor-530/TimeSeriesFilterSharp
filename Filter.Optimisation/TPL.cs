using AccordGenetic.Wrapper;
using FilterSharp.Model;
using GaussianProcess;
using GaussianProcess.Wrap;
using Statistics.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Filter.Optimisation
{
    public class TPL
    {

        ISubject<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> Subject = new Subject<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>();

        ISubject<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> Subject2 = new Subject<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>>();



        public IObservable<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> GetOptimisedPredictions(IEnumerable<KeyValuePair<DateTime, double>> e, int count, Type kernel = null)
        {
            kernel = kernel ?? GaussianProcess.KernelHelper.LoadKernels().Last();

            Action<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> action = (a) => Subject.OnNext(a.Output);
            var x = Task.Run(() => TPL.GetOptimisedOutputKernel(kernel, e, action, count));
            x.ContinueWith((a) => Subject.OnCompleted());

            return Subject;
        }



        public IObservable<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> GetOptimisedSolutions(IEnumerable<KeyValuePair<DateTime, double>> e, int count, Type kernel = null)
        {
            kernel = kernel ?? GaussianProcess.KernelHelper.LoadKernels().Last();

            Action<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> action = (a) => Subject2.OnNext(a);
            var x = Task.Run(() => TPL.GetOptimisedOutputKernel(kernel, e, action, count));
            x.ContinueWith((a) => Subject2.OnCompleted());

            return Subject2;
        }


        public IObservable<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> GetOptimisedSolutions(IEnumerable<KeyValuePair<DateTime, double>> e, int count, TimeSpan timeout, Type kernel = null)
        {
            kernel = kernel ?? GaussianProcess.KernelHelper.LoadKernels().Last();
      
            Action<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> action = (a) => Subject2.OnNext(a);
            var x = Task.Run(() => TPL.GetOptimisedOutputKernel(kernel, e, action, count, timeout));
            x.ContinueWith((a) => Subject2.OnCompleted());
            //Class1.GetOptimisedOutputKernel(kernel, e, action, count, timeout);
            //Subject2.OnCompleted();
            return Subject2;
        }






        public static TimeSeriesOptimisation2DWrap<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> GetOptimisedOutputKernel(Type t, IEnumerable<KeyValuePair<DateTime, double>> e, Action<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> action, int count)
        {
            //Action<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> action
            Func<double, double, GP2> func = (a, b) => new GP2((IMatrixkernel)Activator.CreateInstance(t), a, b);

            var progress = Filter.Optimisation.TPL.GetProgress(t, e, action);



            return new TimeSeriesOptimisation2DWrap<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>
           (
                  func: (ss) => (a, b) => (new GaussianProcessWrapper(func(a, b)).Run(ss)),
                 input: e,
                 error: ErrorHelper.GetErrorSum,
                 progress: progress,

                 count: count
                 );


        }


        public static TimeSeriesOptimisation2DWrap<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> GetOptimisedOutputKernel(Type t, IEnumerable<KeyValuePair<DateTime, double>> e, Action<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> action, int count,TimeSpan timeout)
        {
            //Action<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> action
            Func<double, double, GP2> func = (a, b) => new GP2((IMatrixkernel)Activator.CreateInstance(t), a, b);

            var progress = Filter.Optimisation.TPL.GetProgress(t, e, action);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(timeout);

            return new TimeSeriesOptimisation2DWrap<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>
           (
                  func: (ss) => (a, b) =>  (new GaussianProcessWrapper(func(a, b)).Run(ss)),
                 input: e,
                 error: ErrorHelper.GetErrorSum,
                 progress: progress,
                 token:                 cts.Token,
                 count: count
                 );


        }





        //public static TimeSeriesOptimisation2DWrap<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> GetOptimisedOutputKernel(Type t, IEnumerable<KeyValuePair<DateTime, double>> e, Action<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> action,int count)
        //{
        //    //Action<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> action
        //    Func<double, double, GP2> func = (a, b) => new GP2((IMatrixkernel)Activator.CreateInstance(t), a, b);

        //    var progress = Filter.Optimisation.Class1.GetProgress(t, e, action);

        //    return new TimeSeriesOptimisation2DWrap<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>
        //   (
        //         func: (ss) => (a, b) => (new GaussianProcessWrapper(func(a, b)).BatchRun(ss)),
        //         input: e,
        //         error: ErrorHelper.GetErrorSum,
        //         progress: progress,
        //         count:count);


        //}



        public static Progress<KeyValuePair<int, Result>> GetProgress(Type t, IEnumerable<KeyValuePair<DateTime, double>> e, Action<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> action)
        {
            Func<double, double, GP2> func1 = (a, b) => new GP2((IMatrixkernel)Activator.CreateInstance(t), a, b);

            Func<double, double, IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> act1ion = (a, b) => (new GaussianProcessWrapper(func1(a, b)).Run(e));

            return TimeSeriesOptimisation2DWrap<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>
              .BuildProgress
           (
                 func: act1ion,
                 input: e,
                 a: action
      );



        }





        public static Task<AccordGenetic.Wrapper.TimeSeriesOptimisation2DWrap<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>>
    GetOptimised(IEnumerable<KeyValuePair<DateTime, double>> e, Action<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> action, Type kernel = null)
        {
            kernel = kernel ?? GaussianProcess.KernelHelper.LoadKernels().Last();
            return Task.Run(() =>
             TPL.GetOptimisedOutputKernel(kernel, e, action, 100));



        }

        //public static IObservable<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> GetOptimisedOutputKernel2(Type t, IEnumerable<KeyValuePair<DateTime, double>> e)
        //{
        //    Func<double, double, GP2> func = (a, b) => new GP2((IMatrixkernel)Activator.CreateInstance(t), a, b);
        //    return TimeSeriesOptimisation2DWrap<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>
        //        .Optimise2D
        //(
        //      func: (ss) => (a, b) => (new GaussianProcessWrapper(func(a, b)).BatchRun(ss)),
        //      input: e,
        //      error: ErrorHelper.GetErrorSum,
        //      count:2);


        //}










        //    public static IObservable<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> GetOptimisedOutput(Type t, IEnumerable<KeyValuePair<DateTime, double>> e, Progress<KeyValuePair<int, Result>> progress)
        //    {
        //        return new AccordGenetic.Wrapper.Optimisation2DWrap<IEnumerable<KeyValuePair<DateTime, double>>, IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>
        //            ((ss) => (a, b) => TypeHelper.GetInstance<IFilterWrapper>(t, a, b).BatchRun(ss), e, ErrorHelper.GetErrorSum,progress)
        //            .GetSubjectAsObservable();

        //    }


        //    public static IObservable<IO<IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> GetOptimisedOutput(IObservable<KeyValuePair<DateTime, double>> e, Progress<KeyValuePair<int, Result>> progress)
        //    {
        //        var z = new List<KeyValuePair<DateTime, double>>();
        //        var seed = new { a = z, b = GetOptimiser(z,progress) };

        //        return
        //            e.Scan(seed, (acc, nw) =>
        //            {
        //                var x = GetOptimiser(acc.a.ToArray(),progress);
        //                acc.a.Add(nw);
        //                return new { a = acc.a, b = x };
        //            })
        //               .SelectMany(_ => _.b.GetSubjectAsObservable())
        //               .Select(_ =>
        //               {
        //                   return new IO<IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>>>
        //                   {
        //                       Output = _.Output.ToObservable(),
        //                       Parameters = _.Parameters,
        //                       Score = _.Score
        //                   };
        //               });
        //    }







        //    public static AccordGenetic.Wrapper.Optimisation2DWrap<IEnumerable<KeyValuePair<DateTime, double>>, IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>
        //          GetOptimiser(IEnumerable<KeyValuePair<DateTime, double>> z,
        //        Progress<KeyValuePair<int, Result>> progress)
        //    {
        //        return new AccordGenetic.Wrapper.Optimisation2DWrap<IEnumerable<KeyValuePair<DateTime, double>>, IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>
        //                   ((ss) => (a, b) => new GaussianProcessWrapper(a, b).BatchRun(ss), z, ErrorHelper.GetErrorSum, progress);

        //    }
        //public IObservable<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> GetOptimisedSolutions(IEnumerable<KeyValuePair<DateTime, double>> e, int count, TimeSpan timeout, Type kernel = null)
        //{
        //    kernel = kernel ?? GaussianProcess.KernelHelper.LoadKernels().Last();

        //    Action<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> action = (a) => Subject2.OnNext(a);
        //    var x = Task.Run(() => Class1.GetOptimisedOutputKernel(kernel, e, action, count, timeout),);
        //    x.ContinueWith((a) => Subject2.OnCompleted());
        //    //Class1.GetOptimisedOutputKernel(kernel, e, action, count, timeout);
        //    //Subject2.OnCompleted();
        //    return Subject2;
        //}








        //public static IObservable<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> MakeOptimisedPredictions(IEnumerable<KeyValuePair<DateTime, double>> measurements, IScheduler scheduler)
        //{
        //    var xx = new Optimisation.Class1();
        //    var av = UtilityReactive.ObservableFactory.Build(() =>
        //    {
        //        if (measurements.Count() > 0)
        //            return xx.GetOptimisedPredictions(measurements.ToArray(), 100);
        //        else return null;
        //    }, TimeSpan.FromSeconds(5), scheduler);
        //    return av.SelectMany(_ => _);


        //}

        //public static IObservable<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> MakeOptimisedPredictions(IObservable<KeyValuePair<DateTime, double>> measurements, IScheduler scheduler)
        //{
        //    var xx = new Optimisation.Class1();
        //    var av = UtilityReactive.ObservableFactory.Build(() =>
        //    {
        //        var x = measurements.ToEnumerable();
        //        if (measurements.ToEnumerable().Count() > 0)
        //            return xx.GetOptimisedPredictions(measurements.ToEnumerable(), 100);
        //        else return null;
        //    }, TimeSpan.FromSeconds(5), scheduler);
        //    return av.SelectMany(_ => _);




        //}





        //public static IObservable<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> GetOptimisedOutputKernel(Type t, IEnumerable<KeyValuePair<DateTime, double>> e)
        //{
        //    Func<double, double, GP2> func = (a, b) => new GP2((IMatrixkernel)Activator.CreateInstance(t), a, b);
        //    var x = new TimeSeriesOptimisation2DWrap<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>
        // (
        //       func: (ss) => (a, b) => (new GaussianProcessWrapper(func(a, b)).BatchRun(ss)),
        //       input: e,
        //       error: ErrorHelper.GetErrorSum,
        //       count: 20);

        //    x.GetSubjectAsObservable().Subscribe(_ =>
        //    Console.WriteLine());

        //    return x.GetSubjectAsObservable();
        //}





        //public IObservable<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> GetOptimised(IEnumerable<KeyValuePair<DateTime, double>> e, int count, Type kernel = null)
        //{
        //    kernel = kernel ?? GaussianProcess.KernelHelper.LoadKernels().Last();

        //    Action<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> action = (a) =>
        //    Subject.OnNext(a.Output);
        //    var x = Task.Run(() =>
        //      Class1.GetOptimisedOutputKernel(kernel, e,count));
        //    x.ContinueWith((a) => Subject.OnCompleted());

        //    return Subject;
        //}



        //public static TimeSeriesOptimisation2DWrap<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> GetOptimisedOutputKernel(Type t, IEnumerable<KeyValuePair<DateTime, double>> e,int count)
        //{
        //    //Action<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> action
        //    Func<double, double, GP2> func = (a, b) => new GP2((IMatrixkernel)Activator.CreateInstance(t), a, b);

        //    //var tsw = Filter.Optimisation.Class1.GetProgress(kernel, e, action);
        //    return new TimeSeriesOptimisation2DWrap<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>
        //   (
        //         func: (ss) => (a, b) => (new GaussianProcessWrapper(func(a, b)).BatchRun(ss)),
        //         input: e,
        //         error: ErrorHelper.GetErrorSum,
        //         count: 20);


        //}
    }
}
