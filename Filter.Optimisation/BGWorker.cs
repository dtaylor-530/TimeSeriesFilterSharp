using AccordGenetic.Wrapper;
using GaussianProcess;
using GaussianProcess.Wrap;
using System;
using System.Collections.Generic;
using System.ComponentModel.Custom.Generic;
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
    public static class BGWorker
    {

        //public static IObservable<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> GetOptimisedSolutions(IEnumerable<KeyValuePair<DateTime, double>> e, int count, TimeSpan timeout, Type kernel = null)
        //{
        //    kernel = kernel ?? GaussianProcess.KernelHelper.LoadKernels().Last();

        //    var x = BGWorker.GetOptimisedOutputFromKernel(kernel, e, count/*, timeout*/);

        //    return x;
        //}

        //// Gaussian Process
        //public static IObservable<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> GetOptimisedGPPredictions(IEnumerable<KeyValuePair<DateTime, double>> e, int count, Type kernel = null)
        //{
        //    kernel = kernel ?? GaussianProcess.KernelHelper.LoadKernels().Last();

        //    var x = BGWorker.GetOptimisedOutputFromKernel(kernel, e, count/*, timeout*/);

        //    return x.Select(_ => _.Output);
        //}


        //public static IObservable<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> GetOptimisedGPPredictions(IObservable<IEnumerable<KeyValuePair<DateTime, double>>> e, int count, Type kernel = null)
        //{
        //    kernel = kernel ?? GaussianProcess.KernelHelper.LoadKernels().Last();

        //    var x = BGWorker.GetOptimisedOutputFromKernel(kernel, e, count/*, timeout*/);

        //    return x.Select(_ => _.Output);
        //}



        //// Kalman Filter
        //public static IObservable<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> GetOptimisedKFPredictions(IEnumerable<KeyValuePair<DateTime, double>> e, int count)
        //{
        //    Func<double, double, IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> f = (a, b) =>
        //    {
        //        var mw = new KalmanFilter.Wrap.DiscreteOuterWrapper(a, b);
        //        return mw.BatchRun(e);
        //    };

        //    var x = BGWorker.GetOptimisedOutput(e, f, 100);

        //    return x.Select(_ => _.Output);
        //}




        //public static IObservable<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> GetOptimisedKFPredictions(IObservable<IEnumerable<KeyValuePair<DateTime, double>>> e, int count)
        //{
        //    var efunc3 = e.Select(ee =>
        //    {
        //        Func<double, double, IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> func2 = (a, b) =>
        //        {
        //            var mw = new KalmanFilter.Wrap.DiscreteOuterWrapper(a, b);
        //            return mw.BatchRun(ee);
        //        };
        //        return Tuple.Create(ee, func2);
        //    });

        //    return GetOptimisedOutput(efunc3, count);
        //}



        //public static IObservable<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> GetOptimisedOutputFromKernel(Type t, IEnumerable<KeyValuePair<DateTime, double>> e, int count)
        //{
        //    Func<double, double, GP2> func = (a, b) => new GP2((IMatrixkernel)Activator.CreateInstance(t), a, b);

        //    Func<double, double, IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> func2 = (a, b) => (new GaussianProcessWrapper(func(a, b)).Run(e));

        //    return GetOptimisedOutput(e, func2, count);
        //}




        //public static IObservable<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> GetOptimisedOutputFromKernel(Type t, IObservable<IEnumerable<KeyValuePair<DateTime, double>>> e, int count)
        //{
        //    Func<double, double, GP2> func = (a, b) => new GP2((IMatrixkernel)Activator.CreateInstance(t), a, b);

        //    var efunc3 = e.Select(ee =>
        //    {
        //        Func<double, double, IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> func2 = (a, b) => (new GaussianProcessWrapper(func(a, b)).Run(ee));
        //        return Tuple.Create(ee, func2);
        //    });

        //    return GetOptimisedOutput(efunc3, count);
        //}




        //public static IObservable<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> GetOptimisedOutput(IEnumerable<KeyValuePair<DateTime, double>> e,
        //    Func<double, double, IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> func, int count)
        //{

        //    var x = Optimisation2DWrap2.Create
        //   (
        //          func: func,
        //         input: e,
        //         error: ErrorHelper.GetErrorSum,
        //         count: count);


            
        //    double score = 100000;
        //    return x.Progress.Where(_=>_.Item2!=null).Select(_d =>
        //    {
        //        var _ = _d.Item2;
        //        if (_.Score < score)
        //        {
        //            score = _.Score;
        //            return new IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> { Parameters = _.Input, Score = _.Score, Output = func(_.Input[0], _.Input[1]) };
        //        }
        //        else
        //            return null;
        //    }).Where(de => de != null);
  

        //}




        //public static IObservable<IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> GetOptimisedOutput(
        //    IObservable<Tuple<IEnumerable<KeyValuePair<DateTime, double>>, Func<double, double, IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>>> funce,
        //    int count)
        //{



        //    var x = Optimisation2DWrap2.Create
        //       (
        //         funcinput: funce,
        //         error: ErrorHelper.GetErrorSum,
        //         count: count);


        //    //x.Success.Subscribe(_ =>
        //    //{
        //    //    Console.WriteLine(_);

        //    //});

        //    return x.Progress.Where(_ => _.Item2 != null).Window(count).Zip(funce,(a,b)=>new { a, b }).Select(_ =>
        //    {
        //        double score = 100000;
           
        //       return  _.a.Select(_j => {

        //            var _jj = _j.Item2;
        //            if (_jj.Score < score)
        //            {
        //               var xxx = _.b.Item2(_jj.Input[0], _jj.Input[1]);
        //               score = _jj.Score;
        //                return new IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> { Parameters = _jj.Input, Score = _jj.Score, Output =xxx };
        //            }
        //            else
        //                return null;
        //        }).Where(de => de != null);
        //    }).Switch();


        //    //return x.Progress.Select(_ =>
        //    //{
        //    //    return new IO<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> { Parameters = _.Item2.Parameters, Score = _.Item2.Score, Iteration = _.Item1, Output = _.Item2.Output };

        //    //}).Where(__ => __ != null);

        //}




    }
}