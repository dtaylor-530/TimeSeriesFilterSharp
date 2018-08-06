using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using MathNet.Numerics.LinearAlgebra.Factorization;

namespace GaussianProcess.Wrap
{







    public static class MultiGaussianProcessEx
    {





        //public static IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> BatchRun(this List<GPDynamic> gpds, IEnumerable<KeyValuePair<DateTime, double>> measurements)
        //{
        //    DateTime firstDate = default(DateTime);
        //    IEnumerable<Svd<double>> svd = gpds.Select(_ => _.GetDefaultSvd());
        //    double lastposition = measurements.First().Value;
        //    DateTime lasttime = measurements.First().Key;
        //    foreach (var meas in measurements)
        //    {
        //        double timeSpan = (meas.Key - firstDate).TotalSeconds;
        //        firstDate = default(DateTime) == firstDate ? meas.Key : firstDate;
        //        //yield return gpds.Predict(timeSpan, svd).Last();
        //        yield return gpds.GetPositionsAndVelocities(firstDate, lastposition, lasttime).Last();
        //        svd = gpds.Update(timeSpan, meas.Value);
        //        lastposition = meas.Value;
        //        lasttime = meas.Key;

        //        //var prd = gpds.Predict(svd, meas.Key);
        //        //svd = gpds.Update(meas.Key - firstDate, meas.Value);
        //        //yield return prd.Select(_ => _.Last()).Last();
        //    }

        //}





        //public static IEnumerable<KeyValuePair<double, Tuple<double, double>>> BatchRun(this List<GPDynamic> gpds, IEnumerable<KeyValuePair<double, double?>> measurements)
        //{
        //    //var frst = measurements.First().Key;
        //    IEnumerable<Svd<double>> svd = gpds.Select(_ => _.GetDefaultSvd());
        //    foreach (var meas in measurements)
        //    {
        //        var prd = gpds.Predict(meas.Key, svd);
        //        if (meas.Value != null)
        //            svd = gpds.Update(meas.Key, (double)meas.Value);
        //        yield return prd.Select(_ => _.Last()).Last();
        //    }

        //}


      //  public static IObservable<IEnumerable<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>> Run(this List<GPDynamic> gpds, IObservable<KeyValuePair<DateTime, double?>> measurements, IScheduler scheduler)
      //  {

      //      DateTime firstDate = default(DateTime);
      //      IEnumerable<Svd<double>> svd = gpds.Select(_ => _.GetDefaultSvd());
      //      return measurements
      //         .SubscribeOn(scheduler)
      //         .Select(meas =>
      //{
      //    if (meas.Key != null)
      //        svd = gpds.Update(meas.Key - firstDate, (double)meas.Value);

      //    return gpds.Predict(svd, meas.Key);

      //});




      //  }





        //public static IEnumerable<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> Predict(this List<GPDynamic> gpds, IEnumerable<Svd<double>> svd, DateTime dt,DateTime firstDate, DateTime lastDate,double lastPosition)
        //{
        //    //DateTime firstDate = default(DateTime);
          

        //    return gpds
        //        .Zip(svd, (a, b) => new { a, b })
        //        .Select(_ =>
        //        {
        //            //if (firstDate == default(DateTime)) firstDate = dt;
        //            return
        //            _.a.Predict((dt - firstDate).TotalSeconds, _.b)
        //            .GetPositionsAndVelocities(firstDate,lastPosition,lastDate);
        //        });

        //}





        //public static IEnumerable<Svd<double>> Update(this List<GPDynamic> gpds, TimeSpan ts, double y)
        //{
        //    //if (gpds.FirstDate == default(DateTime))
        //    //    gpds.FirstDate = dt;
        //    return gpds.Select(gpd => gpd.Update((ts).TotalSeconds, y));


        //}



        //public static IEnumerable<KeyValuePair<double, Tuple<double, double>>>[] Predict(this List<GPDynamic> gpds, double x, IEnumerable<Svd<double>> svds)
        //{

        //    return gpds.Zip(svds, (gp, svd) => new { gp, svd }).Select(_ => _.gp.Predict(x, _.svd).GetPositions(x)).ToArray();

        //}




        //public static IEnumerable<Svd<double>> Update(this List<GPDynamic> gpds, double x, double y)
        //{
        //    return gpds.Select(gpd => gpd.Update(x, y));


        //}



    }





}
