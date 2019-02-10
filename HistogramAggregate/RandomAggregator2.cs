using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics;
using UtilityMath;
using MathNet.Numerics.Distributions;
using SharpLearning.Common.Interfaces;
using SharpLearning.Containers.Matrices;
using SharpLearning.Containers;
using SharpLearning.Extension.Interface;
using Statistics.Common;

namespace Statistics.RandomHistogram
{
    public class HistogramOptimisation2Service : ISharpLearningPredictionService
    {

        public ICertaintyPredictor BuildPredictor(IList<double> input, IList<double> targets)
        {

            var parameters = new SharpLearning.Optimization.ParameterBounds[] { new SharpLearning.Optimization.ParameterBounds(
                min: 0, max: 3,
                transform: SharpLearning.Optimization.Transform.Linear,
                parameterType: SharpLearning.Optimization.ParameterType.Continuous) }; // iterations

            var optimizer = new SharpLearning.Optimization.RandomSearchOptimizer(parameters, iterations: 30, runParallel: true);
            //var optimizer = new SharpLearning.Optimization.ParticleSwarmOptimizer(parameters, maxIterations: 100);
            var best = optimizer.Optimize((d) => new HistogramRegression2Service(input, targets).GetOptimizerResult(d));
            var paramset = best.Last().ParameterSet[0];

            return new Histogram2Learner(paramset).Learn(SharpLearning.Extension.Utility.GetF64(input), targets.ToArray());
        }
    }



    public class HistogramRegression2Service : SharpLearning.Extension.SharpLearningRegressionWithVarianceService
    {

        public HistogramRegression2Service(IList<double> input, IList<double> targets) : base(input, targets, new Histogram2Builder())
        {
        }

        public override double GetError(double[] d)
        {

            return _input
                .Zip(_targets, (a, b) => Tuple.Create(a, b))
                .KFolds(_fold)
                .Select(fold =>
            {
                var trainx = fold.Value.Train.Select(_ => _.Item1).ToArray();
                var trainy = fold.Value.Train.Select(_ => _.Item2).ToArray();
                var testx = fold.Value.Test.Select(_ => _.Item1).ToArray();
                var testy = fold.Value.Test.Select(_ => _.Item2).ToArray();

                var predictions = _learnerbuilderservice.Build(d)
                     .Learn(SharpLearning.Extension.Utility.GetF64(trainx), trainy)
                     .Predict(testx);

                var profit = predictions.Zip(fold.Value.Test, (a, b) => { return a.Prediction > 0 ? (b.Item2 * a.Prediction / (a.Variance)) : 0; });
                return profit.Sum();

            }).Average();

        }
    }



    public class Histogram2Builder : ICertaintyLearnerBuilder
    {

        public ICertaintyLearner Build(double[] d)
        {
            return new Histogram2Learner(d[0]);
        }
    }



    public class Histogram2Learner : ICertaintyLearner
    {
        double _size;
        //int _fold;

        public Histogram2Learner(double size)
        {
            _size = size;
            //_fold = 10;

        }

        public ICertaintyPredictor Learn(F64Matrix observations, double[] targets)
        {

            //var his = MathHelper.ToHistogramByBinCount(observations.Column(0).Zip(targets, (a, b) => Tuple.Create(a, b)), (int)_size);

            var his = MathHelper.ToHistogram(observations.Column(0).Zip(targets, (a, b) => Tuple.Create(a, b)), _size);

            return SharpLearning.Extension.Utility.Learn(his);

        }



    }

}
