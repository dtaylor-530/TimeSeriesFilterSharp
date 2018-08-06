using AccordGenetic.Wrapper;
using Filter.Common;
using Filter.Model;
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
using UtilityEnum;
using UtilityReactive;

namespace Filter.Service
{




    //public class PredictionServiceWrapper  // : IPredictionService
    //{

    //    public IObservable<IEnumerable<KeyValuePair<DateTime, double>>> PositionPredictions { get; set; }
    //    public IObservable<IEnumerable<KeyValuePair<DateTime, double>>> VelocityPredictions { get; set; }


    //    public PredictionServiceWrapper(IEnumerable<KeyValuePair<DateTime, double>> measurements, IScheduler scheduler)
    //    {
    //        var PredictionServe = new PredictionService(measurements, scheduler);

    //        PositionPredictions = PredictionServe.PositionPredictions.Select(_ => _.Select(d => new KeyValuePair<DateTime, double>(d.Time, d.Value)));
    //        VelocityPredictions = PredictionServe.PositionPredictions.Select(_ => _.Select(d => new KeyValuePair<DateTime, double>(d.Time, d.Value)));
    //    }


    //    public PredictionServiceWrapper(IObservable<KeyValuePair<DateTime, double>> measurements, IScheduler scheduler)
    //    {
    //        var PredictionServe = new PredictionService(measurements, scheduler);

    //        PositionPredictions = PredictionServe.PositionPredictions.Select(_ => _.Select(d => new KeyValuePair<DateTime, double>(d.Time, d.Value)));
    //        VelocityPredictions = PredictionServe.PositionPredictions.Select(_ => _.Select(d => new KeyValuePair<DateTime, double>(d.Time, d.Value)));
    //    }



    //}




    //public class PredictionServiceWrapper2  // : IPredictionService
    //{

    //    public IObservable<IEnumerable<KeyValuePair<DateTime, double>>> PositionPredictions { get; set; }
    //    public IObservable<IEnumerable<KeyValuePair<DateTime, double>>> VelocityPredictions { get; set; }


    //    public PredictionServiceWrapper2(IObservable<IEnumerable<KeyValuePair<DateTime, double>>> measurements, IScheduler scheduler)
    //    {
    //        var PredictionServe = new PredictionService2(measurements, scheduler);

    //        PositionPredictions = PredictionServe.PositionPredictions.Select(_ => _.Select(d => new KeyValuePair<DateTime, double>(d.Time, d.Value)));
    //        VelocityPredictions = PredictionServe.PositionPredictions.Select(_ => _.Select(d => new KeyValuePair<DateTime, double>(d.Time, d.Value)));
    //    }




    //}






}
