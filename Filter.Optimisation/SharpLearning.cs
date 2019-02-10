
//using SharpLearning.Extension.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter.Optimisation
{
    //class Sharp
    //{
    //}

    //public interface ISharpLearningPredictionService
    //{
    //    SharpLearning.Containers.CertaintyPrediction[] PredictWithCertainty(IList<double> input, IList<double> targets);
    //}


    //public class GaussianProcessOptimisationService : ISharpLearningPredictionService
    //{
    //    public ICertaintyPredictor BuildPredictor(IList<double> input, IList<double> targets)
    //    {
    //        var parameters = new SharpLearning.Optimization.ParameterBounds[] { new SharpLearning.Optimization.ParameterBounds(
    //            min: 0, max: 30,
    //            transform: SharpLearning.Optimization.Transform.Linear,
    //            parameterType: SharpLearning.Optimization.ParameterType.Continuous),
    //        new SharpLearning.Optimization.ParameterBounds(
    //            min: 0, max: 30,
    //            transform: SharpLearning.Optimization.Transform.Linear,
    //            parameterType: SharpLearning.Optimization.ParameterType.Continuous)
    //        }; // iterations


    //        var optimizer = new SharpLearning.Optimization.RandomSearchOptimizer(parameters, iterations: 100, runParallel: true);
    //        //var optimizer = new SharpLearning.Optimization.ParticleSwarmOptimizer(parameters, maxIterations: 100);
    //        var best = optimizer.Optimize((d) => new GaussianProcessRegressionService(input, targets).GetOptimizerResult(d));


    //        return    new SharpLearning.Extension.AggregatePredictor(best.Take(10).Select(_ => Tuple.Create(new GaussianProcessRegressionService(input, targets).Build(_.ParameterSet), _.Error))); ;
    //        //return new GaussianProcessRegressionService(input, targets).GetOutput(best.First().ParameterSet);
    //    }



    //    //public SharpLearning.Containers.CertaintyPrediction[] PredictWithCertainty(IList<double> input, IList<double> targets)
    //    //{
    //    //    var parameters = new SharpLearning.Optimization.ParameterBounds[] { new SharpLearning.Optimization.ParameterBounds(
    //    //        min: 0, max: 30,
    //    //        transform: SharpLearning.Optimization.Transform.Linear,
    //    //        parameterType: SharpLearning.Optimization.ParameterType.Continuous),
    //    //    new SharpLearning.Optimization.ParameterBounds(
    //    //        min: 0, max: 30,
    //    //        transform: SharpLearning.Optimization.Transform.Linear,
    //    //        parameterType: SharpLearning.Optimization.ParameterType.Continuous)
    //    //    }; // iterations


    //    //    var optimizer = new SharpLearning.Optimization.RandomSearchOptimizer(parameters, iterations: 100, runParallel: true);
    //    //    //var optimizer = new SharpLearning.Optimization.ParticleSwarmOptimizer(parameters, maxIterations: 100);
    //    //    var best = optimizer.Optimize((d) => new GaussianProcessRegressionService(input, targets).GetOptimizerResult(d));

    //    //    return new GaussianProcessRegressionService(input, targets).GetOutput(best.First().ParameterSet);
    //    //}


    //}





    //public class GaussianProcessRegressionService:FilterRegressionService
    //{

    //    public GaussianProcessRegressionService(IList<double> input, IList<double> targets ):base(input,targets,new GaussianProcess.Wrap.GaussianProcessWrapperBuilder())
    //    {


    //    }

    //}




    //public class FilterRegressionService
    //{
    //    private IList<double> _input;
    //    private IList<double> _targets;
    //    private Model.IFilterWrapperBuilder _filterbuilderservice;

    //    public FilterRegressionService(IList<double> input, IList<double> targets, FilterSharp.Model.IFilterWrapperBuilder filterbuilderservice)
    //    {
    //        _input = input;
    //        _targets = targets;
    //        _filterbuilderservice = filterbuilderservice;
    //    }

    //    public SharpLearning.Containers.CertaintyPrediction[] Build(double[] d) =>
    //         _filterbuilderservice.Build(d[0], d[1]);
  
 


    //    public SharpLearning.Containers.CertaintyPrediction[] GetOutput(double[] d) =>

    //     _filterbuilderservice.Build(d[0], d[1])
    //        .BatchRun(_input.Zip(_targets, (a, b) => new KeyValuePair<DateTime, double>(new DateTime() + TimeSpan.FromSeconds(a), b)))
    //        .Select(_ => new SharpLearning.Containers.CertaintyPrediction(_.Value[0].Item1, _.Value[0].Item2))
    //        .ToArray();



    //    public double GetError(double[] d) =>
    //        new SharpLearning.Metrics.Regression.MeanSquaredErrorRegressionMetric()
    //            .Error(_input.ToArray(), GetOutput(d).Select(_ => _.Prediction).ToArray());



    //    public SharpLearning.Optimization.OptimizerResult GetOptimizerResult(double[] d) =>   new SharpLearning.Optimization.OptimizerResult(d, GetError(d));


    //}




    //public class AggregatePredictor : ICertaintyPredictor
    //{
    //    IEnumerable<Tuple<IPredictor<double>, double>> _model;

    //    public AggregatePredictor(IEnumerable<Tuple<IPredictor<double>, double>> results)
    //    {

    //        _model = results;
    //    }

    //    public CertaintyPrediction[] Predict(double[] targets)
    //    {
    //        var enm = _model.GetEnumerator();
    //        enm.MoveNext();
    //        var f64 = SharpLearning.Extension.Utility.GetF64(targets);
    //        double error = enm.Current.Item2;
    //        CertaintyPrediction[] set = enm.Current.Item1.Predict(f64).Select(_ => new CertaintyPrediction(_, error)).ToArray();

    //        while (enm.MoveNext())
    //        {
    //            error = enm.Current.Item2;
    //            set = enm.Current.Item1.Predict(f64).Select(_ => new CertaintyPrediction(_, error)).Zip(set, (a, b) => UtilityMath.NormalExtension.Multiply(new Normal(a.Prediction, a.Variance), new Normal(b.Prediction, b.Variance)).ToCertaintyPrediction()).ToArray();

    //        }
    //        return set;
    //        //int i = 1;
    //        //var error = _model[0]
    //        //var set = new HistogramRegressionService(input, targets).Build(best[0].ParameterSet);

    //        //while (i < 10 && i < best.Count())
    //        //{
    //        //    set = new HistogramRegressionService(input, targets).Build(best[i].ParameterSet);//.Zip(set, (a, b) => UtilityMath.NormalExtension.Multiply(new Normal(a.Prediction, error), new Normal(b.Prediction, best[i].Error)).ToCertaintyPrediction()).ToArray();
    //        //    i++;
    //        //}

    //        //return set;
    //    }
    //}

    //public static class Conversion
    //{

    //    public static CertaintyPrediction ToCertaintyPrediction(this Normal normal)
    //    {
    //        return new CertaintyPrediction(normal.Mean, normal.StdDev);

    //    }

    //    public static Normal ToNormal(this CertaintyPrediction cp)
    //    {
    //        return new Normal(cp.Prediction, cp.Variance);

    //    }
    //}

}

