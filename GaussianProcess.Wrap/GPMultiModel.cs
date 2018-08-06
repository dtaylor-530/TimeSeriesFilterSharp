using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaussianProcess.Wrap
{

    //public class GPMultiModel
    //{

    //    public List<GP2> GPs { get; set; } = new List<GP2>();


    //    public GPMultiModel(params GP2[] gps)
    //    {
    //        foreach(GP2 gp in gps)
    //        GPs.Add(gp);

    //    }
    //    List<Svd<double>> svds;

    //    //public IEnumerable<GPOut> AddTrainingPoints(double[] trainingPointsX, double[] trainingPointsY)
    //    //{
    //    //    //var frst = measurements.First().Item1;
    //    //    svds = svds ?? GPs.Select(_ => _.GetDefaultSvd()).ToList();

    //    //    foreach (var meas in trainingPointsX.Zip(trainingPointsY, (a, b) => Tuple.Create(a, b)))
    //    //    {
    //    //        var prd = GPs.Zip(svds, (gp, svd) => new { gp, svd }).Select(_ => _.gp.Predict(meas.Item1, _.svd,1)).ToList();
    //    //        svds = GPs.Update(meas.Item1, meas.Item2).ToList();
    //    //        yield return prd.Last();
    //    //    }

    //    //}
    //}




    public class GPMultiModelold
    {

        public List<GP> GPs { get; set; } = new List<GP>();


        public GPMultiModelold(params GP[] gps)
        {
            foreach(var gp in gps)
            GPs.Add(gp);

        }

        public List<GPOut> AddTrainingPoints(double[] trainingPointsX, double[] trainingPointsY)
        {
            var min = trainingPointsX.Min();
            var max = trainingPointsX.Max();

            //var extra = 0.1 * (max - min);

            var testPointsX = MathNet.Numerics.Generate.LinearSpaced(DistanceMatrix.Instance.Matrix.RowCount, min , max );            // added training points
            Matrix<double> dmTr = MathHelper.ComputeDistanceMatrix(trainingPointsX, trainingPointsX);
            Matrix<double> dmTeTr = MathHelper.ComputeDistanceMatrix(testPointsX, trainingPointsX);

            Vector<double> trY = Vector<double>.Build.DenseOfArray(trainingPointsY);

            var newGPouts= GPs.Select(_ => { var c = _.Compute(dmTr, dmTeTr, trY); c.X = testPointsX; return(c); });

            return newGPouts.ToList();



        }
    }




}
