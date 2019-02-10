using System;
using System.Collections.Generic;
using System.Linq;



namespace Statistics
{

    public static class MathHelper
    {


        public static Dictionary<Tuple<double, double>, double> ToHistogram(this IEnumerable<Tuple<double, double>> dt, double binSize)
        {
            var ranges = dt.GetRanges(binSize).ToList();

            return dt.ToHistogram(ranges);

        }

        public static Dictionary<Tuple<double, double>, double> ToHistogram(this IEnumerable<Tuple<double, double>> dt, IEnumerable<Tuple<double, double>> ranges)
        {
            return dt
                .GroupBy(i => ranges.FirstOrDefault(r => r.Item1 <= i.Item1 & i.Item1 < r.Item2))
                .OrderBy(rt => rt.Key)
                .Where(_ => _.Key != null)
                .ToDictionary(a => a.Key, a => (double)a.Average(km => km.Item2));


        }

        static double dsf = double.Epsilon;

        public static IEnumerable<Tuple<double, double>> GetRanges(this IEnumerable<Tuple<double, double>> dt, double binSize)
        {

            var datax = dt.Select(_ => _.Item1);
            var min = datax.Min() - binSize / 2;
            int binCount = (int)((datax.Max() - min) / binSize) + (int)(binSize);

            return Enumerable.Range(0, binCount).Select(x => Tuple.Create(x * binSize + min, (x + 1) * binSize + min-dsf));
        }


        public static Tuple<double, double> GetIntersected(this IEnumerable<double> lst, double point)
        {
            var en = lst.GetEnumerator();
            en.MoveNext();
            double current = en.Current;
            while (en.Current < point)
            {
                current = en.Current;
                en.MoveNext();
            }
            return Tuple.Create(current, en.Current);

        }




        public static IEnumerable<KeyValuePair<Tuple<double, double>, double>> ToHistogramByBinCount(this IEnumerable<Tuple<double, double>> dt, int binCount)
        {
            var densities = dt.GroupBy(_ => _.Item1).OrderBy(a => a.Key).Select(_ => new
            {
                key = _.Key,
                density = _.Count(),
                average = _.Select(a => a.Item2).Average()
            }).ToList();
            var densitiesBin = densities.Select(_ => _.density).Sum() / binCount;
            var list = Enumerable.Empty<object>().Select(r => new { key = 0d, density = 0, average = 0d }).ToList();
            double sumdensity = 0;
            int dcount = densities.Count;
            for (int i = 0; i < dcount; i++)
            //foreach (var x in densities)
            {

                list.Add(densities[i]);
                sumdensity = list.Sum(_ => _.density);
                if (sumdensity > densitiesBin)
                {
                    double amt = (densitiesBin / sumdensity) * WeightedAverage(list, _ => _.average, _ => _.density);
                    var j = i + 1 == dcount ? i : i + 1;
                    yield return new KeyValuePair<Tuple<double, double>, double>(Tuple.Create(list.First().key, densities[j].key), amt);
                    sumdensity = densitiesBin - sumdensity;
                    list = Enumerable.Empty<object>().Select(r => new { key = 0d, density = 0, average = 0d }).ToList();
                }
            }
            if (list.Count > 0)
            {

                double amt = (densitiesBin / sumdensity) * WeightedAverage(list, _ => _.average, _ => _.density);
                yield return new KeyValuePair<Tuple<double, double>, double>(Tuple.Create(list.First().key, list.Last().key), amt);
            }

        }


        public static IEnumerable<KeyValuePair<Tuple<double, double>, double>> ToHistogramByBinCount(this IEnumerable<double> dt, int binCount, int min = 0)
        {
            var densities = dt.GroupBy(_ => _).OrderBy(_ => _.Key).Select(_ => new { key = _.Key, density = _.Count() });
            var densitiesBin = densities.Select(_ => _.density).Sum() / binCount;
            var list = Enumerable.Empty<object>().Select(r => new { key = 0d, density = 0 }).ToList();

            foreach (var x in densities)
            {
                list.Add(x);

                var sumdensity = list.Sum(_ => _.density);
                if (sumdensity > densitiesBin)
                {
                    yield return new KeyValuePair<Tuple<double, double>, double>(Tuple.Create(list.First().key, list.Last().key), list.Sum(_ => 1));
                    sumdensity = densitiesBin - sumdensity;
                    list = Enumerable.Empty<object>().Select(r => new { key = 0d, density = 0 }).ToList();
                }
            }
        }



        public static double WeightedAverage<T>(IEnumerable<T> records, Func<T, double> value, Func<T, double> weight, double control = 0)
        {
            double weightedValueSum = records.Sum(x => (value(x) - control) * weight(x));
            double weightSum = records.Sum(x => weight(x));

            if (weightSum != 0)
                return weightedValueSum / weightSum;
            else
                throw new DivideByZeroException("Divide by zero exception calculating weighted average");
        }

    }
}
