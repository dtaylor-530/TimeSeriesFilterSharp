using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StarMathLib;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using MathNet.Numerics.LinearAlgebra.Factorization;
using GaussianProcess;
using System.Reflection;

using Filter.Common;

namespace GaussianProcess
{

    public class KernelHelper
    {

        public static IEnumerable<Type> LoadKernels()
        {


            return Assembly.GetAssembly(typeof(GaussianProcess.IMatrixkernel))
                .GetTypesInNamespace(nameof(GaussianProcess))
                .FilterByCategoryAttribute("MatrixKernel");

        }

        public static IEnumerable<Type> LoadDerivativeKernels()
        {


            return Assembly.GetAssembly(typeof(GaussianProcess.IMatrixkernel))
                .GetTypesInNamespace(nameof(GaussianProcess))
                .FilterByCategoryAttribute("DerivativeMatrixKernel");

        }
    }


    public static class MathHelper
    {

        public static double[] GenerateTestPoints(int results, double min, double max)
        {
            if (results > 1)
                return MathNet.Numerics.Generate.LinearSpaced(results, min, max);
            else
                return new double[] { max };
        }


        public static (Vector<double>, Svd<double>) Evaluate(double[] testPointsX, double[] trainingPointsX, double[] trainingPointsY, IMatrixkernel kernel, double length, Svd<double> svd1, Matrix<double> Kte)
        {
            var gpOut = new GPOut();
            Svd<double> svd;

            var dmTeTr = MathHelper.ComputeDistanceMatrix(testPointsX, trainingPointsX.ToArray());
            var tmp = kernel.Main(dmTeTr, length) * svd1.U;
            Vector<double> v = Vector<double>.Build.DenseOfEnumerable(trainingPointsY);

            var mu = tmp * (svd1.S.PointwiseMultiply(svd1.U.Transpose() * v));
            var cov = tmp * Matrix<double>.Build.DenseOfDiagonalVector(svd1.S.PointwiseSqrt());
            cov = cov * cov.Transpose();
            cov = Kte - cov;
            svd = cov.Svd();
            for (int i = 0; i < svd.S.Count; i++)
            {
                svd.S[i] = svd.S[i] < Double.Epsilon ? 0 : svd.S[i];

            }

            return (mu, svd);
        }


        public static Matrix<double> ComputeDistanceMatrix(double[] xdata1, double[] xdata2)
        {
            var dm = Matrix<double>.Build.Dense(xdata1.Length, xdata2.Length, 0);
            for (int i = 0; i < xdata1.Length; i++)
            {
                for (var j = 0; j < xdata2.Length; j++)
                {
                    var val = Math.Abs(xdata2[j] - xdata1[i]);
                    dm[i, j] = val;
                }
            }
            return dm;
        }
    }
}
