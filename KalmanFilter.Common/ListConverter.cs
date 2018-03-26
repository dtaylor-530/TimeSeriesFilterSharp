using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Filter.Utility
{
    public static class Helper
    {

        public static List<Tuple<long, Point>> ToPointTimeSpans(this List<Tuple<DateTime, double[]>> meas)
        {

            List<Tuple<long, Point>> ddf = new List<Tuple<long, Point>>();

            DateTime dt = meas.First().Item1;
            foreach (var x in meas)
            {
                var ts = x.Item1 - dt;
                if(x.Item2.Length==2)
                ddf.Add(Tuple.Create(ts.Ticks, new Point(x.Item2[0],x.Item2[1])));
                else if (x.Item2.Length == 1)
                    ddf.Add(Tuple.Create(ts.Ticks, new Point(0, x.Item2[0])));

                dt = x.Item1;

            }


            return ddf;
        }



        public static List<Tuple<long, Point>> ToPointTimeSpans(this List<Tuple<DateTime, double>> meas)
        {

            List<Tuple<long, Point>> ddf = new List<Tuple<long, Point>>();

            DateTime dt = meas.First().Item1;
            foreach (var x in meas)
            {
                var ts = x.Item1 - dt;
                    ddf.Add(Tuple.Create(ts.Ticks, new Point(0, x.Item2)));

                dt = x.Item1;

            }


            return ddf;
        }


        public static List<Tuple<DateTime, Point>> ToPointDateTimes(this List<Tuple<DateTime, double>> meas)
        {

            List<Tuple<DateTime, Point>> ddf = new List<Tuple<DateTime, Point>>();

            foreach (var x in meas)
            {
                ddf.Add(Tuple.Create(x.Item1, new Point(0, x.Item2)));

            }


            return ddf;
        }


        public static List<Tuple<long, double[]>> ToTimeSpans(this List<Tuple<DateTime, double[]>> meas)
        {

            List<Tuple<long, double[]>> ddf = new List<Tuple<long, double[]>>();

            DateTime dt = meas.First().Item1;
            foreach (var x in meas)
            {
                var ts = x.Item1 - dt;
                ddf.Add(Tuple.Create(ts.Ticks, x.Item2));

                dt = x.Item1;

            }


            return ddf;
        }


        public static List<Tuple<long, double[]>> ToTimeSpans(this List<Tuple<DateTime, Matrix<double>>> meas)
        {

            List<Tuple<long, double[]>> ddf = new List<Tuple<long, double[]>>();

            DateTime dt = meas.First().Item1;
            foreach (var x in meas)
            {
                var ts = x.Item1 - dt;
                ddf.Add(Tuple.Create(ts.Ticks, x.Item2.AsColumnArrays().First()));

                dt = x.Item1;

            }


            return ddf;
        }



        public static Tuple<double[],double[]> ToArrays(this Tuple<Matrix<double>, Matrix<double>> target )
        {
            return Tuple.Create(target.Item1.Column(0).ToArray(), target.Item2.Diagonal().ToArray());


        }

    }



    public class Quanitfier
    {

        public static void pcalc(IEnumerable<Measurement> measurements, IEnumerable<Measurement> velEstimates)
        {
            Queue<Measurement> bought = new Queue<Measurement>();
            Queue<Measurement> sold = new Queue<Measurement>();
            List<double> cashed = new List<double>();
            var zip = measurements.Join(measurements, a => a.Time, b => b.Time, (c, d) =>
            {
                if (d.Value > 0)
                {
                    bought.Enqueue(new Measurement(c.Time, c.Value, d.Variance));
                    if (sold.Count() > 0)
                    {
                        double amt = 0;
                        double sum = 0;
                        Measurement s = default(Measurement);
                        while (1 / d.Variance > amt & sold.Count() > 0)
                        {
                            s = sold.Dequeue();
                            amt += 1 / (s.Variance);
                            //var diff = (1 / d.Variance - amt);
                            sum += ((s.Value - c.Value) / s.Variance);

                        }
                        //var diff=amt - 1 / d.Variance;

                        // sold.Enqueue(new Measurement(c.Time, (1 - amt) * s.Value, d.Variance));


                        cashed.Add(sum);
                    }

                }
                else if (d.Value < 0)
                {
                    sold.Enqueue(new Measurement(c.Time, c.Value, d.Variance));
                    double amt = 0;
                    double sum = 0;
                    Measurement s = default(Measurement);
                    while (1 / d.Variance > amt & bought.Count() > 0)
                    {
                        s = bought.Dequeue();
                        amt += 1 / (s.Variance);
                        //var diff = (1 / d.Variance - amt);
                        sum += ((c.Value - s.Value) / s.Variance);

                    }
                    //var diff=amt - 1 / d.Variance;

                    // sold.Enqueue(new Measurement(c.Time, (1 - amt) * s.Value, d.Variance));


                    cashed.Add(sum);

                }
                return 0;
            }).ToList();



       /*     var profit = cashed.Where(_ => _ != 0).Average();*/ ;



        }
    }



}
