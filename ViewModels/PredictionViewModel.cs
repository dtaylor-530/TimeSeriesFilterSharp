using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Reactive.Linq;
using GaussianProcess.Wrap;
using Filter.Model;
using UtilityWpf.ViewModel;
using DynamicData.Binding;
using System.Reactive.Subjects;



namespace Filter.ViewModel
{

    public class PredictionViewModel : INPCBase
    {


        public CollectionViewModel<KeyValuePair<DateTime, double>> SeriesVM { get; set; }
        public CollectionViewModel<Estimate> PositionPredictionsVM { get; set; }
        public CollectionViewModel<Estimate> VelocityPredictionsVM { get; set; }




        public PredictionViewModel(
            IObservable<CollectionViewModel<KeyValuePair<DateTime, double>>> measurements,
           IObservable<CollectionViewModel<Estimate>> positionestimates,
           IObservable<CollectionViewModel<Estimate>> velocityestimates = null)

        {
            measurements.Subscribe(a =>
            {
                this.SeriesVM = a;
                NotifyChanged(nameof(SeriesVM));
            });

            positionestimates.Subscribe(bc =>
            {
                this.PositionPredictionsVM = bc;
                NotifyChanged(nameof(PositionPredictionsVM));
            });

            velocityestimates?.Subscribe(cb =>
            {
                this.VelocityPredictionsVM = cb;
                NotifyChanged(nameof(VelocityPredictionsVM));
            });
        }


        public PredictionViewModel(
            IObservable<IEnumerable<KeyValuePair<DateTime, double>>> measurements,
            IObservable<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> estimates,
            IScheduler scheduler, Dispatcher dispatcher)
        {
            measurements.ObserveOn(scheduler).Subscribe(a =>
            {
            this.SeriesVM = new CollectionViewModel<KeyValuePair<DateTime, double>>(a, dispatcher);
                NotifyChanged(nameof(SeriesVM));
            });

           estimates.ObserveOn(scheduler).Subscribe(bc =>
            {
                this.PositionPredictionsVM = new CollectionViewModel<Estimate>(bc.GetPositionEstimates(), dispatcher);
        
                this.VelocityPredictionsVM = new CollectionViewModel<Estimate>(bc.GetVelocityEstimates(), dispatcher);

                NotifyChanged(nameof(PositionPredictionsVM));
                NotifyChanged(nameof(VelocityPredictionsVM));

            });
        }


        public PredictionViewModel(    IObservable<KeyValuePair<DateTime, double>> measurements,    IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>> estimates,    IScheduler scheduler)
        {
  
                this.SeriesVM = new CollectionViewModel<KeyValuePair<DateTime, double>>(measurements,scheduler);
                NotifyChanged(nameof(SeriesVM));
          

                this.PositionPredictionsVM = new CollectionViewModel<Estimate>(estimates.Select(_=>_.GetPositionEstimate()),scheduler);

                this.VelocityPredictionsVM = new CollectionViewModel<Estimate>(estimates.Select(_=>_.GetVelocityEstimate()),scheduler);

                NotifyChanged(nameof(PositionPredictionsVM));
                NotifyChanged(nameof(VelocityPredictionsVM));

        }


        public PredictionViewModel(IObservable<KeyValuePair<DateTime, double>> measurements, IObservable<IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> estimates, IScheduler scheduler, Dispatcher dispatcher)
        {

            this.SeriesVM = new CollectionViewModel<KeyValuePair<DateTime, double>>(measurements, scheduler);
            NotifyChanged(nameof(SeriesVM));

            estimates.ObserveOn(scheduler).Subscribe(bc =>
            {
                this.PositionPredictionsVM = new CollectionViewModel<Estimate>(bc.GetPositionEstimates(),dispatcher);

                //this.VelocityPredictionsVM = new CollectionViewModel<Estimate>(bc.GetVelocityEstimates());
                this.VelocityPredictionsVM = new CollectionViewModel<Estimate>(bc.GetDifferences(this.SeriesVM.Items), dispatcher);

                NotifyChanged(nameof(PositionPredictionsVM));
                NotifyChanged(nameof(VelocityPredictionsVM));

            });

        }
    }





    //public class PredictionViewModel2 : INPCBase
    //{


    //    public CollectionViewModel<KeyValuePair<DateTime, double>> SeriesVM { get; set; }
    //    public CollectionViewModel<KeyValuePair<DateTime, Tuple<double, double>>> PositionPredictionsVM { get; set; }
    //    public CollectionViewModel<KeyValuePair<DateTime, Tuple<double, double>>> VelocityPredictionsVM { get; set; }




    //    public PredictionViewModel2(
    //        IObservable<CollectionViewModel<KeyValuePair<DateTime, double>>> measurements,
    //       IObservable<CollectionViewModel<KeyValuePair<DateTime, Tuple<double,double>>>> positionestimates, 
    //       IObservable<CollectionViewModel<KeyValuePair<DateTime,Tuple< double,double>>>> velocityestimates=null)

    //    {
    //        measurements.Subscribe(a =>
    //        {
    //            this.SeriesVM = a;
    //            NotifyChanged(nameof(SeriesVM));
    //        });


    //        positionestimates.Subscribe(bc =>
    //        {
    //            this.PositionPredictionsVM = bc;
    //            NotifyChanged(nameof(PositionPredictionsVM));
    //        });

    //        velocityestimates?.Subscribe(cb =>
    //        {
    //            this.VelocityPredictionsVM = cb;
    //            NotifyChanged(nameof(VelocityPredictionsVM));
    //        });





    //    }



    //}

    static class Helper
    {


        public static IEnumerable<KeyValuePair<DateTime, Tuple<double, double, double>>> GetPositions(this IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> x)
        {
            var positions = x
            .Select(g => new KeyValuePair<DateTime, Tuple<double, double, double>>(g.Key,
            Tuple.Create(g.Value[0].Item1, g.Value[0].Item1 - g.Value[0].Item2, g.Value[0].Item1 + g.Value[0].Item2)));

            return positions;
        }


        public static IEnumerable<KeyValuePair<DateTime, Tuple<double, double, double>>> GetVelocities(this IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> x)
        {
            var vels = x.Select(g =>
           new KeyValuePair<DateTime, Tuple<double, double, double>>(g.Key,
            Tuple.Create(g.Value[1].Item1, g.Value[0].Item1 - g.Value[0].Item2, g.Value[1].Item1 + g.Value[1].Item2)));

            return vels;
        }


        public static IEnumerable<Estimate> GetPositionEstimates(this IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> x)
        {
            return x.Select(g => new Estimate(g.Key, g.Value[0].Item1, g.Value[0].Item2));
        }


        public static IEnumerable<Estimate> GetDifferences(this IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> x, IEnumerable<KeyValuePair<DateTime, double>> xx)
        {
            return x.Join(xx,a=>a.Key,b=>b.Key,(c,d)=>Tuple.Create(c.Key,c.Value[0].Item1-d.Value)). Select(g => new Estimate(g.Item1, g.Item2,0));
        }


        public static IEnumerable<Estimate> GetVelocityEstimates(this IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> x)
        {

      
                return x.Select(g =>           new Estimate(g.Key,            g.Value[1].Item1 , g.Value[1].Item2));


        }



        public static Estimate GetPositionEstimate(this KeyValuePair<DateTime, Tuple<double, double>[]> g)
        {
            return new Estimate(g.Key, g.Value[0].Item1, g.Value[0].Item2);
        }


        public static Estimate GetVelocityEstimate(this KeyValuePair<DateTime, Tuple<double, double>[]> g)
        {
            
            return new Estimate(g.Key, g.Value[1].Item1, g.Value[1].Item2);

        }
        //public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int n)
        //{
        //    return source.Skip(Math.Max(0, source.Count() - n));
        //}

    }
}
