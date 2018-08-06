using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UtilityWpf.ViewModel;


namespace Filter.Controller
{
    public class TimeSeriesController
    {


        public IObservable<CollectionViewModel<KeyValuePair<DateTime, double>>> TimeValueSeries { get; set; }

        public IEnumerable<ButtonDefinition> Buttons { get; set; }


        public TimeSeriesController(ISubject<IEnumerable<KeyValuePair<DateTime,double>>> observable)
        {

            Buttons = VMFactory.BuildButtons(observable);

            TimeValueSeries = observable.Select(_ => new CollectionViewModel<KeyValuePair<DateTime, double>>(_)); 
        }


    }




    //public static class InputController
    //{

    //    public static IObservable<OutputService> Combine(IObservable<IEnumerable<KeyValuePair<DateTime, double>>> m, IObservable<Flux> b, IObservable<int> limit, IScheduler uis)
    //    {
    //        return m
    //             .Limit(limit)
    //             .CombineLatest(b, (c, d) => VMFactory.ViewModelSelector(c, d, uis));

    //    }
    //}
}
