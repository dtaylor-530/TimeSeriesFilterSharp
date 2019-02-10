using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MathNet.Numerics.LinearAlgebra.Factorization;
using UtilityHelper;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using SharpLearning.Common.Interfaces;
using SharpLearning.Containers.Matrices;
using SharpLearning.Containers;
using SharpLearning.Extension.Interface;

namespace Statistics
{



    public class HistogramOptimisationService : ISharpLearningPredictionService
    {

        public ICertaintyPredictor BuildPredictor(IList<double> input, IList<double> targets)
        {
            var diff = (input.Max() - input.Min()) / 2;
            var parameters = new SharpLearning.Optimization.ParameterBounds[] {
                new SharpLearning.Optimization.ParameterBounds(
                min: 0, max: diff,
                transform: SharpLearning.Optimization.Transform.Linear,
                parameterType: SharpLearning.Optimization.ParameterType.Continuous),


            }; // iterations

            var optimizer = new SharpLearning.Optimization.RandomSearchOptimizer(parameters, iterations: 30, runParallel: true);
            //var optimizer = new SharpLearning.Optimization.ParticleSwarmOptimizer(parameters, maxIterations: 100);
            var best = optimizer.Optimize((d) => new HistogramRegressionService(input, targets).GetOptimizerResult(d));

            return new SharpLearning.Extension.AggregatePredictor(best.Take(10).Select(_ =>Tuple.Create( new HistogramRegressionService(input, targets).Build(_.ParameterSet),_.Error)));
        }
    }


    public class HistogramRegressionService : SharpLearning.Extension.SharpLearningRegressionService
    {

        public HistogramRegressionService(IList<double> input, IList<double> targets) : base(input, targets, new HistogramBuilder())
        {
        }

 
    }



  


    public class HistogramBuilder : ILearnerBuilder
    {

        public SharpLearning.Common.Interfaces.ILearner<double> Build(double[] d)
        {
            return new Statistics.HistogramLearner(d[0]);
        }
    }



    public class HistogramLearner : ILearner<double>
    {
        double _size;
        //int _fold;

        public HistogramLearner(double size)
        {
            _size = size;
            //_fold = 10;

        }

        public IPredictorModel<double> Learn(F64Matrix observations, double[] targets)
        {

            var h = MathHelper.ToHistogram(observations.Column(0).Zip(targets, (a, b) => Tuple.Create(a, b)), _size);

            return new HistogramPredictor(h);
        }
    }


   

    public class HistogramPredictor : IPredictorModel<double>
    {
        private Dictionary<Tuple<double, double>, double> _model;

        public HistogramPredictor(Dictionary<Tuple<double,double>,double> model)
        {
            _model = model;


        }


        public double[] GetRawVariableImportance()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, double> GetVariableImportance(Dictionary<string, int> featureNameToIndex)
        {
            throw new NotImplementedException();
        }

        public double Predict(double[] observation)
        {
            throw new NotImplementedException();
        }

        public double[] Predict(F64Matrix observations)
        {

            var spline2 = Interpolate.Linear(_model.SelectMany(_ =>new[] { _.Key.Item1, _.Key.Item2 }).OrderBy(a=>a), _model.SelectMany(_ => new[] { _.Value, _.Value }));

            return observations.Column(0).Select(_ =>
            {
                var prediction = spline2.Interpolate(_);

                //var vals =  observations.Column(0).Select(v => v.Item1).GetIntersected(_);

                var weight = 1;// new[] { Model.Single(s_ => s_.Item1 == vals.Item1).Item2.StdDev + Model.Single(s_ => s_.Item1 == vals.Item2).Item2.StdDev }.Average();

                return new { a = _, b = new SharpLearning.Containers.CertaintyPrediction(prediction, weight) };
            }).Select(z => z.b.Prediction).ToArray();


        }

   
    }














    //        public IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> BatchRun(IEnumerable<KeyValuePair<DateTime, double>> values)
    //        {
    //            var values2 = values.Select(_ => Tuple.Create((double)_.Key.Ticks, _.Value)).ToList();

    //            var folds = values2.KFolds(_fold);

    //            List<KeyValuePair<IEnumerable<Tuple<double, double>>, double>> inp = new List<KeyValuePair<IEnumerable<Tuple<double, double>>, double>>();

    //            foreach (var fold in folds)
    //            {
    //                var vals = fold.Train;

    //                    var h = MathHelper.ToHistogram(vals, _size)/*.Select(_ => Tuple.Create((_.Key.Item1 + _.Key.Item2) / 2, _.Value))*/.ToList();

    //                    var xx = h.SelectMany(_ => new[] { Tuple.Create(_.Key.Item1, _.Value), Tuple.Create(_.Key.Item2, _.Value) }).OrderBy(a => a.Item1).ToList();
    //                    var test = fold.Test.Select(_ => _.Item1);
    //                    var target = fold.Test.Select(_ => _.Item2).ToArray();
    //                    var spline = Interpolate.Linear(xx.Select(_ => _.Item1), xx.Select(_ => _.Item2));
    //                    var predictions = test.Select(_ => spline.Interpolate(_)).ToArray();
    //                    var error = new SharpLearning.Metrics.Regression.MeanSquaredErrorRegressionMetric().Error(target, predictions);
    //                    if (error != double.NaN)
    //                        inp.Add(new KeyValuePair<IEnumerable<Tuple<double, double>>, double>(xx, error));
    //                // var xx = inp.SelectMany(_ => _).SelectMany(_ => new[] { Tuple.Create(_.Key.Item1, _.Value), Tuple.Create(_.Key.Item2, _.Value) }).OrderBy(a=>a.Item1);

    //            }

    //            var Model = inp.SelectMany(_ =>
    // _.Key
    // .Select(a => Tuple.Create(a.Item1, a.Item2, _.Value)))
    //.OrderBy(ob => ob.Item1)
    //.GroupBy(gb => gb.Item1)
    //.Where(a => a.Key != 0)
    //.Select(s =>

    //{
    //    Tuple<double, Normal> ty = null;
    //    try
    //    {
    //        ty = Tuple.Create(s.Key, UtilityMath.NormalExtension.Multiply(s.Where(sv => sv.Item2 != 0 && !double.IsNaN(sv.Item3)).Select(cv => Tuple.Create(cv.Item2, cv.Item3))));
    //    }
    //    catch
    //    {

    //    }
    //    return ty;
    //}).Where(_ => _ != null);



    //            var spline2 = Interpolate.CubicSpline(Model.Select(_ => _.Item1), Model.Select(_ => _.Item2.Mean));

    //            return values2.Select(_ =>
    //            {
    //                var prediction = spline2.Interpolate(_.Item1);

    //                var vals = values2.Select(v => v.Item1).GetIntersected(_.Item1);

    //                var weight = 1;// new[] { Model.Single(s_ => s_.Item1 == vals.Item1).Item2.StdDev + Model.Single(s_ => s_.Item1 == vals.Item2).Item2.StdDev }.Average();

    //                return new {a= _, b=new SharpLearning.Containers.CertaintyPrediction(prediction, weight) };
    //            }).Select(z => new KeyValuePair<DateTime, Tuple<double, double>[]>(new DateTime((long)z.a.Item1), new[] { Tuple.Create(z.b.Prediction, + z.b.Variance) }));




    //        }
}



