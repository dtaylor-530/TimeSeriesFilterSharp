using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaussianProcess
{


    //public class GPMultiModel
    //{

    //    //public double[] tePointsX { get; set; };//= MathNet.Numerics.Generate.LinearSpaced(DistanceMatrix.Instance.MatrixColumnLength, -5, 5);


    //    public List<GP> GPs { get; set; } = new List<GP>();


    //    public GPMultiModel(GP gp)
    //    {

    //        GPs.Add(gp);

    //    }



    //    public List<GPOut> AddTrainingPoints(double[] trainingPointsX, double[] trainingPointsY)
    //    {
    //        var min = trainingPointsX.Min();
    //        var max = trainingPointsX.Max();

    //        var extra = 0.1 * (max - min);

    //        var testPointsX = MathNet.Numerics.Generate.LinearSpaced(DistanceMatrix.Instance.Matrix.RowCount, min - extra, max + extra);
    //        // added training points
    //        Matrix<double> dmTr = MathHelper.ComputeDistanceMatrix(trainingPointsX, trainingPointsX);
    //        Matrix<double> dmTeTr = MathHelper.ComputeDistanceMatrix(testPointsX, trainingPointsX);

    //        Vector<double> trY = Vector<double>.Build.DenseOfArray(trainingPointsY);

    //        var newGPs = new List<GPOut>();

    //        GPs.ForEach(_ => { var c = _.Compute(dmTr, dmTeTr, trY); c.X = testPointsX; newGPs.Add(c); });

    //        return newGPs;



    //    }
    //}

}
