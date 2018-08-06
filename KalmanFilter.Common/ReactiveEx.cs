//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Reactive.Linq;
//using System.Reactive.Concurrency;

//namespace Filter.Utility
//{
//    public static class ReactiveEx
//    {


//        public static IObservable<KeyValuePair<DateTime, T>> ByTimeStamp<T>(this IEnumerable<KeyValuePair<DateTime, T>> scheduledTimes, TimeSpan offset=default(TimeSpan))
//        {


//            return Observable.Generate(
//                scheduledTimes.GetEnumerator(),
//                e => e.MoveNext(),
//                e => e,
//                e => e.Current,
//                e => e.Current.Key+offset);

//        }


//        // randombly combines observable into groups of specified size 
//        public static IObservable<DateTime> ByTimeStamp(this IEnumerable<DateTime> scheduledTimes, TimeSpan offset = default(TimeSpan))
//        {


//            return Observable.Generate(
//                scheduledTimes.GetEnumerator(),
//                e => e.MoveNext(),
//                e => e,
//  e => e.Current,
//  e => e.Current+offset);

//        }


//        public static IObservable<KeyValuePair<DateTime, TimeSpan>> TimeOffsets(this IObservable<DateTime> scheduledTimes)
//        {


//            return scheduledTimes
//                 .Scan(
//                new KeyValuePair<DateTime, TimeSpan>(default(DateTime), new TimeSpan(0)),
//                (acc, nw) => new KeyValuePair<DateTime, TimeSpan>(nw, nw - (acc.Key == default(DateTime) ? nw : acc.Key)));



//        }

//        public static IObservable<Tuple<DateTime, TimeSpan,T>> TimeOffsets<T>(this IObservable<KeyValuePair<DateTime,T>> scheduledTimes)
//        {
//            return scheduledTimes
//                 .Scan(new Tuple<DateTime, TimeSpan, T>(default(DateTime), default(TimeSpan), default(T)), (acc, nw) =>
//                 {
//                     var ts = (acc.Item1 == default(DateTime)) ? new TimeSpan(0) : nw.Key - acc.Item1;
//                     return new Tuple<DateTime, TimeSpan, T>(nw.Key, ts, nw.Value);
//                 });



//        }



//        //public static IObservable<Tuple<DateTime, DateTime>> Offsets(this IObservable<DateTime> scheduledTimes,TimeSpan ts)
//        //{


//        //    return scheduledTimes.Select(_ => Tuple.Create(_, _ - ts));



//        //}

//        public static IScheduler MakeUIScheduler(this System.Windows.Threading.Dispatcher dispatcher)
//        {

//            return new System.Reactive.Concurrency.DispatcherScheduler(dispatcher);

//        }



//    }
//}