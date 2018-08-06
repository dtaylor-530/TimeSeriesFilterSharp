using Filter.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter.ViewModel
{
    public class TimeValueSeriesViewModel : INPCBase
    {

        public ObservableCollection<Measurement> Items { get; set; } = new ObservableCollection<Measurement>();



        public TimeValueSeriesViewModel(IObservable<KeyValuePair<DateTime, double?>> measurements, IScheduler ui)
        {
            measurements.ObserveOn(ui).Subscribe(meas =>
            {
                if (meas.Key != null)
                    Items.Add(new Measurement(meas.Key, (double)meas.Value));
            },
              ex => Console.WriteLine("error in observable")
           , () => Console.WriteLine("Observer has unsubscribed from timed observable")
        );


        }


        public TimeValueSeriesViewModel(IEnumerable<KeyValuePair<DateTime, double>> measurements)
        {

            foreach (var meas in measurements)
                Items.Add(new Measurement(meas.Key, meas.Value));

        }


        public TimeValueSeriesViewModel(IObservable<KeyValuePair<DateTime, double>> measurements,  IScheduler ui)
        {
            measurements.ObserveOn(ui).Subscribe(
                meas => Items.Add(new Measurement(meas.Key, (double)meas.Value)),
              ex => Console.WriteLine("error in observable"),
           () => Console.WriteLine("Observer has unsubscribed from timed observable")
        );


        }


    }

}
