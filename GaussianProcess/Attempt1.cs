﻿using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaussianProcess
{

    //https://blog.dominodatalab.com/fitting-gaussian-process-models-python/



    public class Process
    {
        public Matrix<double> /*double[,]*/ CoVariance(IKernel kernel, double[] data)
        {
            List<List<double>> datarr = new List<List<double>>();
            //def covariance(kernel, data):
            for (int i = 0; i < data.Length; i++)
            {
                datarr.Add(new List<double>());

                for (int j = 0; j < data.Length; j++)
                    datarr[i].Add(kernel.Main(data[i], data[j]));
 
            }
            var twodarr = To2DArray<double>(datarr);
            return Matrix<double>.Build.DenseOfArray(twodarr);

        }






        public Vector<double> draw_multivariate_gaussian(Vector<double> mean, Matrix<double> C)
        {
            var ndim = mean.Count();
            //z = random.standard_normal(ndim)

            double[] z = new double[ndim];
            Normal.Samples(z, 0.0, 1.0);

            var zvector = Vector<double>.Build.Dense(z);

            // Better numerical stabability than cholskey decomposition for
            // near-singular matrices C.
            var svd = C.Svd();

            //[U, S, V] = linalg.svd(C);

            var sv = svd.S.PointwiseSqrt();
            var svs = Enumerable.Range(0, mean.Count).Select(_ => sv);

            var A = svd.U.Multiply(Matrix.Build.DenseOfColumnVectors(svs));
              

            return mean + A.Multiply(zvector);
        }








        public Tuple<Vector<double>, Matrix<double>> Train(double[] data, IKernel kernel)
        {
            var mean = Vector<double>.Build.Dense(data.Length, 0);
            var C = Helper.FuncOuter(data, data, kernel.Main);/* CoVariance(kernel, data);*/
            return new Tuple<Vector<double>, Matrix<double>>(mean, Matrix.Build.DenseOfArray(C));

        }



        public Tuple<double, double> predict(double x, double[] data, IKernel kernel, Matrix<double> C, Vector<double> t)
        {
            //
            //       The prediction equations are from Bishop pg 308.eqns. 6.66 and 6.67.
            //     """

            var k = new List<double>();

            foreach (var y in data)
                k.Add(kernel.Main(x, y));

            var kvector = Vector<double>.Build.Dense(k.ToArray());

            var Cinv = C.Inverse();
            var mu = Cinv.Multiply(kvector).DotProduct(t);
            var sigma = kernel.Main(x, x) - Cinv.Multiply(kvector).DotProduct(kvector);

            return new Tuple<double, double>(mu, sigma);
        }








        /// <summary>
        /// Converts nested list to 2D array.
        /// </summary>
        /// <typeparam name="T">
        /// The type of item that must exist in the source.
        /// </typeparam>
        /// <param name="source">
        /// The source to convert.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if source is null.
        /// </exception>
        /// <returns>
        /// The 2D array of source items.
        /// </returns>
        public static T[,] To2DArray<T>(List<List<T>> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            int max = source.Select(l => l).Max(l => l.Count());

            var result = new T[source.Count, max];

            for (int i = 0; i < source.Count; i++)
            {
                for (int j = 0; j < source[i].Count(); j++)
                {
                    result[i, j] = source[i][j];
                }
            }

            return result;
        }





        //# kernel = OrnsteinKernel(1.0)
        //kernel = Kernel(1.0, 64.0, 0.0, 0.0)

        //# Some sample training points.
        //xpts = random.rand(10) * 2 - 1

        //# In the context of Gaussian Processes training means simply
        //# constructing the kernel (or Gram) matrix.
        //(m, C) = train(xpts, kernel)

        //# Now we draw from the distribution to sample from the gaussian prior.
        //t = draw_multivariate_gaussian(m, C)

        //pylab.figure(0)
        //pylab.plot(xpts, t, "+")

        //# Instead of regressing against some known function, lets just see
        //# what happens when we predict based on the sampled prior. This seems
        //# to be what a lot of other demo code does.

        //# Explore the results of GP regression in the target domain.
        //predictions = [predict(i, xpts, kernel, C, t) for i in arange(-1, 1, 0.01)]

        //        pylab.figure(1)
        //x = [prediction[0] for prediction in predictions]
        //y = [prediction[1] for prediction in predictions]
        //sigma = [prediction[2] for prediction in predictions]
        //pylab.errorbar(x, y, yerr=sigma)

        //pylab.show()
    }
}

