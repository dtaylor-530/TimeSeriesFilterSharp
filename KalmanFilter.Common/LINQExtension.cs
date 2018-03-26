using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter.Utility
{
    public static class LINQExtension
    {

        public static IEnumerable<U> Map<T, U>(this IEnumerable<T> s, Func<T, U> f)
        {
            foreach (var item in s)
                yield return f(item);
        }






        public static IEnumerable<double> SelectDifferences(this double[] sequence)
        {
            for(int i=0;i<sequence.Length-1;i++)
            {
                yield return sequence[i+1] - sequence[i];
       
            }
        }
      public static IEnumerable<double> SelectDifferences(this List<double> sequence)
        {
            for (int i = 0; i < sequence.Count() - 1; i++)
            {
                yield return sequence[i + 1] - sequence[i];

            }
        }



        public static IEnumerable<TAccumulate> SelectAggregate<TSource, TAccumulate>(
    this IEnumerable<TSource> source,
    TAccumulate seed,
    Func<TAccumulate, TSource, TAccumulate> func)
        {
            //source.CheckArgumentNull("source");
            //func.CheckArgumentNull("func");
            return source.SelectAggregateIterator(seed, func);
        }

        private static IEnumerable<TAccumulate> SelectAggregateIterator<TSource, TAccumulate>(
            this IEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func)
        {
            TAccumulate previous = seed;
            foreach (var item in source)
            {
                TAccumulate result = func(previous, item);
                previous = result;
                yield return result;
            }
        }


        //public static IEnumerable<U> Map<T, U>(this IEnumerable<T> s, Func<T, U> f)
        //{
        //    foreach (var item in s)
        //        yield return f(item);
        //}




        //public static IEnumerable<TResult> ZipMap<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        //{

        //    return ZipIterator(first, second, resultSelector);
        //}

        //private static IEnumerable<TResult> ZipIterator<TFirst, TSecond, TResult>(IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        //{
        //    using (IEnumerator<TFirst> e1 = first.GetEnumerator())
        //    using (IEnumerator<TSecond> e2 = second.GetEnumerator())
        //    {
        //        while (e1.MoveNext() && e2.MoveNext())
        //        {
        //            yield return resultSelector(e1.Current, e2.Current);
        //        }
        //    }
        //}
    }


}
