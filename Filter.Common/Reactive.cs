using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterSharp.Common
{
    public static class Reactive
    {
        public static IObservable<KeyValuePair<Tuple<DateTime, TimeSpan>, T>> IncrementalTimeOffsets<T>(this IObservable<KeyValuePair<DateTime, T>> scheduledTimes)
        {
            return scheduledTimes
                 .Scan(new KeyValuePair<Tuple<DateTime, TimeSpan>, T>(Tuple.Create(default(DateTime), default(TimeSpan)), default(T)), (acc, nw) =>
                 {
                     var ts = (acc.Key.Item1 == default(DateTime)) ? new TimeSpan(0) : nw.Key - acc.Key.Item1;
                     return new KeyValuePair<Tuple<DateTime, TimeSpan>, T>(new Tuple<DateTime, TimeSpan>(nw.Key, ts), nw.Value);
                 });
        }

        //public static IObservable<KeyValuePair<Tuple<DateTime, TimeSpan>, T>> TotalTimeOffsets<T>(this IObservable<KeyValuePair<DateTime, T>> scheduledTimes)
        //{
        //    //var first= scheduledTimes.First().Key;
        //    DateTime dt = default(DateTime);
        //    return scheduledTimes
        //         .Scan(new KeyValuePair<Tuple<DateTime, TimeSpan>, T>(Tuple.Create(default(DateTime), default(TimeSpan)), default(T)), (acc, nw) =>
        //         {
        //             if (acc.Key.Item1 == default(DateTime)) dt = nw.Key;
        //             return new KeyValuePair<Tuple<DateTime, TimeSpan>, T>(new Tuple<DateTime, TimeSpan>(nw.Key, acc.Key.Item1 - dt), nw.Value);
        //         });
        //}     
        public static IObservable<Tuple< TimeSpan, T>> TotalTimeOffsets<T>(this IObservable<T> scheduledTimes, Func<T,DateTime> func )
        {
            //var first= scheduledTimes.First().Key;
            DateTime dt = default(DateTime);
            return scheduledTimes
                 .Scan(Tuple.Create( default(TimeSpan), default(T)), (acc, nw) =>
                 {
                     if (acc.Item1 == default(TimeSpan)) dt = func(nw);
                     return Tuple.Create(func(nw) - dt, nw);
                 });
        }

    }
}
