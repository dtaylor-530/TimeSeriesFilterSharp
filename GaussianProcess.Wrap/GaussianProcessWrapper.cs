using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MathNet.Numerics.LinearAlgebra.Factorization;
using FilterSharp.Common;
using FilterSharp.Model;


namespace GaussianProcess.Wrap
{

    public class GaussianProcessWrapperBuilder:IFilterWrapperBuilder
    {
        public  IFilterWrapper Build(double a, double b)
        {

            return new GaussianProcessWrapper(a, b);

        }

      



    }




    public class GaussianProcessWrapper : FilterSharp.Model.IFilterWrapper, ITwoVariableInitialiser
    {

        public GP2 GaussianProcess { get; set; }



        public GaussianProcessWrapper(double a, double b)
        {
            Initialise(a, b);
        }

        public void Initialise(double a, double b)
        {
            GaussianProcess = new GP2(new ExponentiatedQuadratic(), a, b);

        }


        public GaussianProcessWrapper(GP2 gaussianProcess)
        {
            GaussianProcess = gaussianProcess;
        }



        public IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> BatchRun(IEnumerable<KeyValuePair<DateTime, double>> measurements)
        {

            //var mgp = new TimeDependentMultiGaussianProcess { FirstDate = measurements.First().Key, GaussianProcesses = gpds };
            //GaussianProcess.Reset();
            DateTime firstDate = measurements.First().Key;
            //Svd<double> svd = GaussianProcess.GetDefaultSvd();
            double lastposition = measurements.First().Value;
            DateTime lasttime = measurements.First().Key;

            List<double> x = new List<double>();
            List<double> y = new List<double>();


            foreach (var meas in measurements.TotalTimeOffsets())
            {
                //double timeSpan = (meas.Key - firstDate).TotalSeconds;
                //firstDate = default(DateTime) == firstDate ? meas.Key : firstDate;
                var time = meas.Key.Item2.TotalSeconds;
                var av =measurements.Average(_=>_.Value);
                var prd = GaussianProcess.Predict(x.ToArray(), y.Select(_=>_-av).ToArray(), time);
                var pvx = prd.GetPositionsAndVelocities(firstDate,av ).Last();
                //svd = GaussianProcess.Update(timeSpan, meas.Value);
                lastposition = meas.Value;
                lasttime = meas.Key.Item1;
                x.Add(time);
                y.Add(meas.Value);
                yield return pvx;

            }

        }



        public IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> Run(IEnumerable<KeyValuePair<DateTime, double>> measurements)
        {

            //var mgp = new TimeDependentMultiGaussianProcess { FirstDate = measurements.First().Key, GaussianProcesses = gpds };
            //GaussianProcess.Reset();
            DateTime firstDate = measurements.First().Key;
            //Svd<double> svd = GaussianProcess.GetDefaultSvd();
            double lastposition = measurements.First().Value;
            DateTime lasttime = measurements.First().Key;

            List<double> x = new List<double>();
            List<double> y = new List<double>();

            var av=measurements.Average(_=>_.Value);

            foreach (var meas in measurements.Select(_ => new KeyValuePair<DateTime, double>(_.Key, _.Value - av)).TotalTimeOffsets())
            {

                lastposition = meas.Value;
                lasttime = meas.Key.Item1;
                x.Add(meas.Key.Item2.TotalSeconds);
                y.Add(meas.Value);

            }

            var prd = GaussianProcess.Predict(x.ToArray(), y.ToArray(), x.ToArray());
            return prd.GetPositionsAndVelocities(firstDate, av);
        }





        //IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>>
        public IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>> Run(IObservable<KeyValuePair<DateTime, double?>> measurements)
        {
            //GaussianProcess.Reset();


            //var t = measurements.TotalTimeOffsets().Select(meas => meas.Value == null ? null : GaussianProcess.Update((meas.Key.Item2).TotalSeconds, (double)meas.Value)).Where(al => al != null);
            var seed = new
            {
                x = new List<double>(),
                y = new List<double>(),
                val = new KeyValuePair<DateTime, Tuple<double, double>[]>(default(DateTime), new[] { Tuple.Create(0d, 0d), Tuple.Create(0d, 0d) })
            };


            return
              FilterSharp.Common.Reactive
                .TotalTimeOffsets(   FilterSharp.Common.Reactive.IncrementalTimeOffsets(measurements),df => df.Key.Item1)
                .Scan(seed, (a, b) =>
               {
                   return Task.Run(() =>
                   {
                       var av = a.y.Average();
                       var prd = GaussianProcess.Predict(a.x.ToArray(), a.y.Select(_ => _ - av).ToArray(), b.Item1.TotalSeconds);
                       //var val = a.y.Count() > 0 ? prd.GetPositionsAndVelocities(b.Item2.Key.Item1 - b.Item1, a.y.Last(), b.Item2.Key.Item1 - b.Item2.Key.Item2).Last()
                       var val = a.y.Count() > 0 ? prd.GetPositionsAndVelocities(b.Item2.Key.Item1 - b.Item1,av).Last()
                    : new KeyValuePair<DateTime, Tuple<double, double>[]>(b.Item2.Key.Item1, new[] { Tuple.Create(0d, 0d), Tuple.Create(0d, 0d) });

                       if (b.Item2.Value != null)
                       {
                           a.x.Add(b.Item1.TotalSeconds);
                           a.y.Add((double)b.Item2.Value);
                       }
                       return new { x = a.x, y = a.y, val = val };
                   }).Result;

               }).Select(_ => _.val);

            //  var svd = meas.Value.Item1 != null ? GaussianProcess.Update((meas.Key.Item2).TotalSeconds, (double)meas.Value.Item1) : acc.Svd; }
            //x.Scan(_ =>new {DateTime=default(DateTime),}



        }
        





    }

}
