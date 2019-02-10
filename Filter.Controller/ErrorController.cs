using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
//using UtilityWpf.ViewModel;

namespace Filter.Controller
{
    ////public class ErrorController
    ////{
    ////    public IObservable<CollectionViewModel<KeyValuePair<DateTime,double>>> Errors { get; set; }

    ////    public ErrorController(IObservable<IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>>> est,IObservable<IObservable<KeyValuePair<DateTime,double>>> meas, Dispatcher dis)
    ////    {
    ////        Errors = meas.Zip(est,
    ////            (a,b) => new CollectionViewModel<KeyValuePair<DateTime, double>>( Filter.Common.ErrorHelper.GetError(a.ToEnumerable(),b.ToEnumerable()),dis)); 

    ////    }
    ////}
}
