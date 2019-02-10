
using System;

using System.Collections.ObjectModel;

using FilterSharp.Common;
using FilterSharp.ViewModel;
using System.Linq;

namespace TimeSeriesFilterSharp.Terminal
{
    //public class Accord KalmanFilterViewModel
    //{
    //    int delay = 50;
    //    readonly double noise = 3;

    //    public ObservableCollection<MeasurementsEstimates> Mes { get; set; } = new ObservableCollection<MeasurementsEstimates>();

    //    public Accord KalmanFilterViewModel()
    //    {
    //        Mes.Add(new MeasurementsEstimates { Measurements = new ObservableCollection<Measurement>(), Estimates = new ObservableCollection<Measurement>() });
    //        Mes.Add(new MeasurementsEstimates { Estimates = new ObservableCollection<Measurement>() });

    //    }

    //    DateTime dt = new DateTime(2000, 1, 1);


    //    public void Run()
    //    {

    //        var meas =FilterSharp.Service.TimeValueServices.MakeMeasurementServiceDefault(100);


    //        var kf = new KalmanFilter.Wrap.Accord KalmanFilterWrapper(noise);


    //        var u = kf.Run(meas);


    //        int i = 0;
    //        foreach(var rec in meas.Zip(kf.Run(meas),(a,b)=>new { a, b }))
    //        {

    //            Mes[0].Measurements.Add(new Measurement(dt + TimeSpan.FromSeconds(i), rec.a.Item2));
    //            Mes[0].Estimates.Add(new Measurement(dt + TimeSpan.FromSeconds(i), rec.b.Item2[0], rec.b.Item2[1]));
    //            Mes[1].Estimates.Add(new Measurement(dt + TimeSpan.FromSeconds(i), rec.b.Item2[2], 1));

    //            i++;

    //        }


    //    }
    //}
}
