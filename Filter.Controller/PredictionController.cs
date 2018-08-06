using Filter.Service;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Filter.Model;

using UtilityWpf.ViewModel;
using Filter.Common;
using UtilityReactive;
using System.Reactive.Subjects;
using System.Reactive.Disposables;

namespace Filter.Controller
{







    public class PredictionsController
    {

        public IObservable<CollectionViewModel<Estimate>> PositionPredictionsVM { get; set; }
        public IObservable<CollectionViewModel<Estimate>> VelocityPredictionsVM { get; set; }

        public PredictionsController(IObservable<IEnumerable< KeyValuePair<DateTime, Tuple<double, double>[]>>> subject, IScheduler s)
        {
            PositionPredictionsVM = subject.Select(_ => new CollectionViewModel<Estimate>(_.Select(a => new Estimate(a.Key, a.Value[0].Item1, a.Value[0].Item2))));

            VelocityPredictionsVM = subject.Select(_ => new CollectionViewModel<Estimate>(_.Select(a => new Estimate(a.Key, a.Value[1].Item1, a.Value[1].Item2))));

        }



        public PredictionsController(IObservable<IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>>> subject, IScheduler s)
        {
            PositionPredictionsVM = subject.Select(_ => new CollectionViewModel<Estimate>(_.Select(a => new Estimate(a.Key, a.Value[0].Item1, a.Value[0].Item2)), s));

            VelocityPredictionsVM = subject.Select(_ => new CollectionViewModel<Estimate>(_.Select(a => new Estimate(a.Key, a.Value[1].Item1, a.Value[1].Item2)), s));

        }


    }



}