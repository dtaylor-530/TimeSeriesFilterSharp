using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Filter.Common
{
    public static class TimeValueMapper
    {


        public static IEnumerable<KeyValuePair<Tuple<DateTime, TimeSpan>, T>> TotalTimeOffsets<T>(this IEnumerable<KeyValuePair<DateTime, T>> scheduledTimes)
        {
            //var first= scheduledTimes.First().Key;
            DateTime dt = scheduledTimes.First().Key;
            foreach (var time in scheduledTimes.ToArray())
            {
                yield return new KeyValuePair<Tuple<DateTime, TimeSpan>, T>(new Tuple<DateTime, TimeSpan>(time.Key,time.Key-dt),time.Value);
            }
        }
        //        public static IEnumerable<KeyValuePair<long, Point>> ToPointTimeSpans(this IEnumerable<Tuple<DateTime, double[]>> meas)
        //        {

        //            DateTime dt = meas.First().Item1;
        //            foreach (var x in meas)
        //            {
        //                var ts = x.Item1 - dt;
        //                if (x.Item2.Length == 2)
        //                    yield return new KeyValuePair<long, Point>(ts.Ticks, new Point(x.Item2[0], x.Item2[1]));
        //                else if (x.Item2.Length == 1)
        //                    yield return new KeyValuePair<long, Point>(ts.Ticks, new Point(0, x.Item2[0]));

        //                dt = x.Item1;

        //            }


        //        }



        //        public static IEnumerable<KeyValuePair<long, Point>> ToTimeSpanPoints(this IEnumerable<Tuple<DateTime, double>> meas)
        //        {

        //            DateTime dt = meas.First().Item1;
        //            foreach (var x in meas)
        //            {
        //                var ts = x.Item1 - dt;
        //                yield return new KeyValuePair<long, Point>(ts.Ticks, new Point(0, x.Item2));

        //                dt = x.Item1;

        //            }

        //        }


        //        public static IEnumerable<KeyValuePair<DateTime, Point>> ToDateTimePoints(this IEnumerable<KeyValuePair<DateTime, double>> meas)
        //        {

        //            foreach (var x in meas)
        //            {
        //                yield return new KeyValuePair<DateTime, Point>(x.Key, new Point(0, x.Value));

        //            }

        //        }


        //        //public static IEnumerable<KeyValuePair<DateTime, Point>> ToPointDateTimes(this IEnumerable<Tuple<DateTime, double>> meas)
        //        //{

        //        //    foreach (var x in meas)
        //        //    {
        //        //       yield return Tuple.Create(x.Item1, new Point(0, x.Item2));

        //        //    }


        //        //}

        //        //public static IEnumerable<KeyValuePair<DateTime, Point>> ToPointDateTimes(this IEnumerable<KeyValuePair<DateTime, double>> meas)
        //        //{

        //        //    List<Tuple<DateTime, Point>> ddf = new List<Tuple<DateTime, Point>>();

        //        //    foreach (var x in meas)
        //        //    {
        //        //        yield return new KeyValuePair<DateTime, Point>(x.Key, new Point(0, x.Value));

        //        //    }


        //        //}


        //        public static IEnumerable<KeyValuePair<long, double[]>> ToTimeSpans(this List<Tuple<DateTime, double[]>> meas)
        //        {

        //            List<Tuple<long, double[]>> ddf = new List<Tuple<long, double[]>>();

        //            DateTime dt = meas.First().Item1;
        //            foreach (var x in meas)
        //            {
        //                var ts = x.Item1 - dt;
        //                yield return new KeyValuePair<long, double[]>(ts.Ticks, x.Item2);

        //                dt = x.Item1;

        //            }


        //        }


        //        public static IEnumerable<KeyValuePair<long, double[]>> ToTimeSpans(this List<KeyValuePair<DateTime, Matrix<double>>> meas)
        //        {

        //            List<Tuple<long, double[]>> ddf = new List<Tuple<long, double[]>>();

        //            DateTime dt = meas.First().Key;
        //            foreach (var x in meas)
        //            {
        //                var ts = x.Key - dt;
        //                yield return new KeyValuePair<long, double[]>(ts.Ticks, x.Value.AsColumnArrays().First());

        //                dt = x.Key;

        //            }


        //        }



        //        public static Tuple<double[], double[]> ToArrays(this Tuple<Matrix<double>, Matrix<double>> target)
        //        {
        //            return Tuple.Create(target.Item1.Column(0).ToArray(), target.Item2.Diagonal().ToArray());


        //        }



        //        public static IEnumerable<Tuple<double, double>> ToEnumerable(this Tuple<Matrix<double>, Matrix<double>> target)
        //        {
        //            return target.Item1.Column(0).ToArray().Zip(target.Item2.Diagonal().ToArray(), (a, b) => Tuple.Create(a, b));


        //        }






        //        public static IEnumerable<KeyValuePair<DateTime, double[]>> ToArrays(this IEnumerable<KeyValuePair<DateTime, double>> list)
        //        {

        //            return list.Select(_ => new KeyValuePair<DateTime, double[]>(_.Key, new double[] { _.Value }));


        //        }

        //        public static IEnumerable<KeyValuePair<DateTime, System.Windows.Point>> ToPoints(this IEnumerable<KeyValuePair<DateTime, double>> list)
        //        {

        //            return list.Select(_ => new KeyValuePair<DateTime, Point>(_.Key, new System.Windows.Point(0, _.Value)));


        //        }


        //        public static IEnumerable<KeyValuePair<DateTime, Matrix<double>>> ToMatrices(this IEnumerable<KeyValuePair<DateTime, double>> list)
        //        {

        //            return list.Select(_ => new KeyValuePair<DateTime, Matrix<double>>(
        //                _.Key,
        //                Matrix<double>.Build.DenseOfColumnArrays(new double[] { _.Value })));


        //        }

        //        public static IEnumerable<KeyValuePair<DateTime, Matrix<double>>> ToMatrices(this IEnumerable<KeyValuePair<DateTime, double?>> list)
        //        {

        //            return list.Select(_ => new KeyValuePair<DateTime, Matrix<double>>(
        //                _.Key, _.Value == null ? null :
        //                Matrix<double>.Build.DenseOfColumnArrays(new double[] { (double)_.Value })));


        //        }




        //        public static IEnumerable<KeyValuePair<DateTime, System.Windows.Point>> ToTimePoints(this IEnumerable<KeyValuePair<DateTime, double>> meas)
        //        {
        //            return meas.Select(_ => new KeyValuePair<DateTime, System.Windows.Point>(_.Key, new System.Windows.Point(0, _.Value)));
        //        }





    }






}