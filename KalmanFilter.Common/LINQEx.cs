using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace Filter.Utility
{




    //public static class LINQEx
    //{

    //    public static IEnumerable<U> Map<T, U>(this IEnumerable<T> s, Func<T, U> f)
    //    {
    //        foreach (var item in s)
    //            yield return f(item);
    //    }






    //    public static IEnumerable<double> SelectDifferences(this double[] sequence)
    //    {
    //        for (int i = 0; i < sequence.Length - 1; i++)
    //        {
    //            yield return sequence[i + 1] - sequence[i];

    //        }
    //    }
    //    public static IEnumerable<double> SelectDifferences(this List<double> sequence)
    //    {
    //        for (int i = 0; i < sequence.Count() - 1; i++)
    //        {
    //            yield return sequence[i + 1] - sequence[i];

    //        }
    //    }



    //    public static IEnumerable<TAccumulate> SelectAggregate<TSource, TAccumulate>(
    //this IEnumerable<TSource> source,
    //TAccumulate seed,
    //Func<TAccumulate, TSource, TAccumulate> func)
    //    {
    //        //source.CheckArgumentNull("source");
    //        //func.CheckArgumentNull("func");
    //        return source.SelectAggregateIterator(seed, func);
    //    }

    //    private static IEnumerable<TAccumulate> SelectAggregateIterator<TSource, TAccumulate>(
    //        this IEnumerable<TSource> source,
    //        TAccumulate seed,
    //        Func<TAccumulate, TSource, TAccumulate> func)
    //    {
    //        TAccumulate previous = seed;
    //        foreach (var item in source)
    //        {
    //            TAccumulate result = func(previous, item);
    //            previous = result;
    //            yield return result;
    //        }
    //    }








    //}


}
