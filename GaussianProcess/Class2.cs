using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using StarMathLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaussianProcess
{
    public class Process2

    {


        IKernel kernel;


        public Process2()
        {
           kernel = new Kernel(1.0,64.0, 0.0, 0.0);


          // kernel = new OrnsteinKernel(1.0);

        }




        public double[,] exponential_cov(double[] x, double[] y)
        {

            return Helper.FuncOuter(x, y, kernel.Main);

        }



        public Tuple<double, double> conditional(double x_new, double[] x, double[] y)
        {

            var B = Helper.FuncOuter(x_new, x, kernel.Main);

            var C = Helper.FuncOuter(x, x, kernel.Main);

            var A = kernel.Main(x_new, x_new);



            var CInv = Matrix.Build.DenseOfArray(C).Inverse();
            var BV = Vector.Build.DenseOfArray(B);
            
            var yV = Vector.Build.DenseOfArray(y);

        
            var gfgf = CInv.Multiply(BV);

            var mu = gfgf.DotProduct(yV);

            var sigma = Math.Sqrt( A - BV.DotProduct(gfgf));



            return new Tuple<double, double>(mu, sigma);



   //         cvec = kernel(interp_x - data_x)

   // # weight vector
   //         cinv = np.linalg.inv(cov)
   //wt = np.dot(cinv, cvec)

   // # interpolant and its std.dev.
   //         interp_y = np.dot(wt, data_y)
   //interp_u = np.sqrt(kernel(0) - np.dot(cvec, wt))
   // return interp_y, interp_u






        }


        public Tuple<double, double> predict(double x, double[] data,  double[,] C, double[] t)
        {

            var k = new List<double>();
            foreach (double y in data)
                k.Add(kernel.Main( x,y));


            var kvector = Vector<double>.Build.Dense(k.ToArray());

            var tvector = Vector<double>.Build.Dense(t);

            var CInv = Matrix.Build.DenseOfArray(C).Inverse();
            var mu = (CInv.Multiply(kvector)).DotProduct(tvector);

            var sigma = Math.Sqrt(kernel.Main(x, x) - CInv.Multiply(kvector).DotProduct(kvector));



            return new Tuple<double, double>(mu, sigma);


            

        }





    }








    public static class Helper
    {
        public static double[,] FuncOuter(double[] x, double[] y, Func<double, double, double> func)
        {
            double[,] arr = new double[x.Length, y.Length];

            for (int i = 0; i < x.Length; i++)
                for (int j = 0; j < y.Length; j++)
                {
                    arr[i, j] = func(x[i], y[j]);


                }

            return arr;
        }


        public static double[] FuncOuter(double x, double[] y, Func<double, double, double> func)
        {
            double[] arr = new double[y.Length];


            for (int j = 0; j < y.Length; j++)
            {
                arr[j] = func(x, y[j]);


            }

            return arr;
        }



    }
}
