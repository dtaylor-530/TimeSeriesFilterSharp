using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Windows.Threading;
using GaussianProcess.Wrap;
using static MathNet.Numerics.Generate;
using Filter.Model;
using UtilityMath;
using System.Reactive.Subjects;

namespace Filter.Service
{


    public class TimeSeriesService
    {
        public ISubject<IEnumerable<KeyValuePair<DateTime, double>>> Measurements { get; set; }

        public TimeSeriesService()
        {
            var x = ServiceFactory.GetSubject();

            Measurements = x;

        }
    }



    public class ServiceFactory
    {


        public static ReplaySubject<IEnumerable<KeyValuePair<DateTime, double>>> GetSubject()
        {
            return new ReplaySubject<IEnumerable<KeyValuePair<DateTime, double>>>();
        }


    }




    

    public class TimeValueServices
    {

        public static IObservable<KeyValuePair<DateTime, T>> CombineServices<T>(IObservable<KeyValuePair<DateTime, T>> measurements, IObservable<DateTime> unknowns, IScheduler scheduler)
        {

            return
                measurements
                .Merge(
              unknowns
                .Select(_ => new KeyValuePair<DateTime, T>(_, default(T))), scheduler);

        }


        //public static IObservable<KeyValuePair<DateTime,T>> GetTimeStampedObservable<T>(IEnumerable<double> xpts, IEnumerable<T> ypts, DateTime dtn)
        //{

        //    return xpts
        //        .Zip(ypts, (x, y) => new { x, y })
        //        .Select(_ => new KeyValuePair<DateTime, T>(dtn + TimeSpan.FromSeconds(_.x - xpts.Min()), _.y))
        //        .ByTimeStamp();
        //}


        //public static IObservable<DateTime> GetTimeStampedObservable(IEnumerable<long> xpts)
        //{
        //    // unknowns arrive a second before the measurement arrives
        //    TimeSpan ts = TimeSpan.FromSeconds(-1);
        //    return xpts
        //           .Select(_ =>
        //           new DateTime(_))
        //           .ByTimeStamp(ts);

        //}


        //public static IObservable< KeyValuePair<DateTime, T>> GetTimeStampedObservable<T>(IEnumerable<long> xpts,Func<long,T> func)
        //{

        //    return xpts
        //    .Zip(
        //        xpts.Select(_=>func(_)), (x, y) => new { x, y })
        //       .Select(_ =>
        //     new KeyValuePair<DateTime, T>(new DateTime(_.x), _.y))
        //      .ByTimeStamp();


        //}




    }


    //public class TimeValueEnumeratorService
    //{
    //    IEnumerator<Tuple<DateTime, double>> Measurements { get; set; }
    //    IEnumerator<DateTime> Unknowns { get; set; }
    //    //IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>> Predictions { get; set; }


    //    public TimeValueEnumeratorService(double[] xpts, double[] ypts)
    //    {

    //        DateTime dtn = DateTime.Now;

    //        var measurements = xpts
    //            .Zip(ypts, (a, b) => new System.Windows.Point(a, b))
    //            .Select(_ => Tuple.Create(dtn + TimeSpan.FromSeconds(_.X - xpts.Min()), _.Y))
    //            .GetEnumerator();

    //        var unknowns = xpts
    //             .Zip(ypts, (a, b) => new System.Windows.Point(a, b))
    //             .Select(_ => dtn + TimeSpan.FromSeconds(_.X - xpts.Min()))
    //             .GetEnumerator();



    //        //var gpds = MultiGaussianProcessFactory.BuildDefault(200);
    //        //TimeDependentMultiGaussianProcess tdmgp = new TimeDependentMultiGaussianProcess { GaussianProcesses = gpds };



    //        //Predictions = filter.Run(measurements, unknowns, newThread);


    //    }




    //}



    //public class TimeValueService<T>
    //{
    //    IObservable<Tuple<DateTime, double>> Measurements { get; set; }
    //    IObservable<DateTime> Unknowns { get; set; }
    //    //IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>> Predictions { get; set; }


    //    public TimeValueService(double[] xpts, double[] ypts, T filter)
    //    {
    //        var newThread = NewThreadScheduler.Default;

    //        DateTime dtn = DateTime.Now;

    //        System.Reactive.Concurrency.IScheduler scheduler = System.Reactive.Concurrency.TaskPoolScheduler.Default;
    //        var measurements = xpts
    //            .Zip(ypts, (a, b) => new System.Windows.Point(a, b))
    //            .Select(_ => Tuple.Create(dtn + TimeSpan.FromSeconds(_.X - xpts.Min()), _.Y))
    //            .ByTimeStamp();

    //        var unknowns = xpts
    //             .Zip(ypts, (a, b) => new System.Windows.Point(a, b))
    //             .Select(_ => dtn + TimeSpan.FromSeconds(_.X - xpts.Min()))
    //             .ByTimeStamp();



    //        //var gpds = MultiGaussianProcessFactory.BuildDefault(200);
    //        //TimeDependentMultiGaussianProcess tdmgp = new TimeDependentMultiGaussianProcess { GaussianProcesses = gpds };



    //        //Predictions = filter.Run(measurements, unknowns, newThread);


    //    }




    //}



}
