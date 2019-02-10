using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using Statistics.Common;

namespace Statistics
{

    public class HistogramAggregator
    {

        private IEnumerable<Tuple<double, Normal>> Model;

        public bool Learn(IEnumerable<Tuple<double, double>> values)
        {

            var folds = values.KFolds(10);

            //  var dat = values.GroupBy(_ => _.Item1).Select(_ => new Tuple<double, double>(_.Average(a => a.Item1), _.Average(a => a.Item2))).ToList();
            //List<List<Tuple<double, double>>> inp = new List<List<Tuple<double, double>>>();

            List<KeyValuePair<IEnumerable<Tuple<double, double>>, double>> inp = new List<KeyValuePair<IEnumerable<Tuple<double, double>>, double>>();

            foreach (var fold in folds)
            {
                var vals = fold.Value.Train;
                for (int i = 0; i < 10; i++)
                {
                    var h = MathHelper.ToHistogram(vals, i+1 )/*.Select(_ => Tuple.Create((_.Key.Item1 + _.Key.Item2) / 2, _.Value))*/.ToList();


                    var xx = h.SelectMany(_ => new[] { Tuple.Create(_.Key.Item1, _.Value), Tuple.Create(_.Key.Item2, _.Value) }).OrderBy(a => a.Item1).ToList();
                    var test = fold.Value.Test.Select(_ => _.Item1);
                    var target = fold.Value.Test.Select(_ => _.Item2).ToArray();
                    var spline = MathNet.Numerics.Interpolate.Linear(xx.Select(_ => _.Item1), xx.Select(_ => _.Item2));
                    var predictions = test.Select(_ => spline.Interpolate(_)).ToArray();
                    var error = new SharpLearning.Metrics.Regression.MeanSquaredErrorRegressionMetric().Error(target, predictions);
                    if(error!=double.NaN)
                    inp.Add(new KeyValuePair<IEnumerable<Tuple<double, double>>, double>(xx, error));
                }
                // var xx = inp.SelectMany(_ => _).SelectMany(_ => new[] { Tuple.Create(_.Key.Item1, _.Value), Tuple.Create(_.Key.Item2, _.Value) }).OrderBy(a=>a.Item1);



            }


            Model = inp.SelectMany(_ =>
  _.Key
  .Select(a => Tuple.Create(a.Item1, a.Item2, _.Value)))
 .OrderBy(ob => ob.Item1)
 .GroupBy(gb => gb.Item1)
 .Where(a => a.Key != 0)
 .Select(s =>

 {
     Tuple<double, Normal> ty = null;
     try
     {
         ty = Tuple.Create(s.Key, UtilityMath.NormalExtension.Multiply(s.Where(sv => sv.Item2 != 0 && !double.IsNaN(sv.Item3)).Select(cv => Tuple.Create(cv.Item2, cv.Item3))));
     }
     catch
     {

     }
     return ty;
 }).Where(_ => _ != null);
            return true;
            //var xy = _regressionService.PredictWithCertainty(input2, targets);
            //Line = input2.Zip(xy, (a, b) => Tuple.Create(a, b.Prediction)).ToList();
            //UncertaintyLine = input2.Zip(xy, (a, b) => new { a, b }).Select(_ => Tuple.Create(_.a, _.b.Prediction - Math.Sqrt(_.b.Variance), _.b.Prediction + Math.Sqrt(_.b.Variance))).ToList();
            //});
        }




        public SharpLearning.Containers.CertaintyPrediction[] PredictCertainty(params double[] values)
        {

            var spline = MathNet.Numerics.Interpolate.Linear(Model.Select(_ => _.Item1), Model.Select(_ => _.Item2.Mean));

            return values.Select(_ =>
            {
                var prediction = spline.Interpolate(_);

                var vals = values.GetIntersected(_);

                var weight = 1;// new[] { Model.Single(s_ => s_.Item1 == vals.Item1).Item2.StdDev + Model.Single(s_ => s_.Item1 == vals.Item2).Item2.StdDev }.Average();

                return new SharpLearning.Containers.CertaintyPrediction(prediction, weight);
            }).ToArray();

        }





    }

}



