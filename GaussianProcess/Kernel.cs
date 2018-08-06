using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaussianProcess
{


    public interface IKernel
    {
       double  Main(double x, double y);


    }

    public class Kernel:IKernel
    {
        //Kernel from Bishop's Pattern Recognition and Machine Learning pg. 307 Eqn. 6.63.

        double Theta0;
        double Theta1;
        double Theta2;
        double Theta3;

        public Kernel(double theta0, double theta1, double theta2, double theta3)
        {

            Theta0 = theta0;
            Theta1 = theta1;
            Theta2 = theta2;
            Theta3 = theta3;
        }

        public double Main(double x, double y)
        {
            double exponential =  Theta0 * Math.Exp(-0.5 * Theta1 * (x- y) * (x - y));
            double linear = Theta3 * x * y;
            double constant = Theta2;
            return exponential+ constant + linear;
        }


    }

    public class OrnsteinKernel : IKernel
    {

        //Ornstein-Uhlenbeck process kernel.
        double Theta;
        public OrnsteinKernel(double theta)

        {
            Theta = theta;
        }


        public double Main(double x, double y)
        {
            return Math.Exp(-Theta * Math.Abs(x - y));

        }

    }



    public interface IMatrixkernel
    {

        Matrix<double> Main(Matrix<double> r,  double var);



    }


    //[Description("Ornstein"), Category("MultiMatrixKernel")]
    //public class OrnsteinMatrixKernel : IMultiMatrixkernel
    //{
    //    public Matrix<double> Main(Matrix<double> r, double[] vars)
    //    {
    //        var exponential =vars[0] *(-0.5 * vars[1] * r.PointwisePower(2)).PointwiseExp();
    //        double linear = vars[3] * r.PointwisePower(2);
    //        double constant = Theta2;
    //        return exponential + constant + linear;

    //    }
    //}


    [Description("Exponentiated Quadratic"), Category("MatrixKernel")]
    public class ExponentiatedQuadratic : IMatrixkernel
    {
        public Matrix<double> Main(Matrix<double> r,  double var)
        {
            return ((-0.5 / var * var) * r.PointwisePower(2)).PointwiseExp();

        }
    }

    // Derivative of the above
    //https://stats.stackexchange.com/questions/187975/calculating-the-expression-for-the-derivative-of-a-gaussian-process
    [Description("Exponentiated Quadratic Derivative"), Category("DerivativeMatrixKernel")]
    public class ExponentiatedQuadraticDerivative : IMatrixkernel
    {
        public Matrix<double> Main(Matrix<double> r, double var)
        {
            var tmp=((1/ var * var) * r.PointwisePower(2));

            return (1 - tmp).PointwiseMultiply((-0.5 * tmp).PointwiseExp());

        }
    }





    [Description("Exponential"), Category("MatrixKernel")]
    public class Exponential : IMatrixkernel
    {

        public Matrix<double> Main(Matrix<double> r,double var)
        {
            return (r * (-0.5 / var )).PointwiseExp();

        }

    }




    [Description("Matern 3/2"), Category("MatrixKernel")]
    public class Matern32 : IMatrixkernel
    {
        static double sqrt3 = Math.Sqrt(3);


        public Matrix<double> Main(Matrix<double> r, double var)
        {

            var tmp = (r * (sqrt3 / var)) + 1;
            return tmp.PointwiseMultiply (-tmp).PointwiseExp();

        }

    }


    [Description("Matern 5/2"), Category("MatrixKernel")]
    public class Matern52 : IMatrixkernel
    {
        static double sqrt5 = Math.Sqrt(3);

        public Matrix<double> Main(Matrix<double> r, double var)
        {

            var tmp = (r * (sqrt5 / var));
            var tmp2 = (tmp.PointwiseMultiply( tmp) / 3);
            return ((tmp+1)+tmp2).PointwiseMultiply (-tmp).PointwiseExp();

        }

    }



    [Description("Rational Quadratic"), Category("MatrixKernel")]
    public class RationalQuadratic : IMatrixkernel
    {

        public Matrix<double> Main(Matrix<double> r, double var)
        {

            return (r.PointwisePower(2) / (2 * var * var) + 1).PointwisePower(-1);


        }

    }


    [Description("Rational Quadratic Derivative"), Category("DerivativeMatrixKernel")]
    public class RationalQuadraticDerivative : IMatrixkernel
    {

        public Matrix<double> Main(Matrix<double> r, double var)
        {

            var tmp=-(r.PointwisePower(2) / (2 * var * var) + 1).PointwisePower(-2)/2;
            var tmp2 = r / (var * var);
            return tmp.PointwiseMultiply(tmp2);

        }

    }

    //       'name': 'Rational quadratic (alpha=1)',
    //       'f': function(r, params)
    //{
    //    return numeric.pow(numeric.add(1.0, numeric.div(numeric.pow(r, 2), 2.0 * params[0] * params[0])), -1);





    //    public class Helper2
    //    {
    //        // ids must be in order of the array
    //        var cfs = [
    //      {'id': 0,
    //         ,
    //           'f': function(r, params) {
    //         return numeric.exp(numeric.mul(-0.5 / (params[0] * params[0]), numeric.pow(r, 2)));
    //       }
    //},
    //      {'id': 1,
    //       'name': 'Exponential',
    //       'f': function(r, params)
    //{
    //    return numeric.exp(numeric.mul(-0.5 / params[0], r));
    //}
    //      },
    //      {'id': 2,
    //       'name': 'Matern 3/2',
    //       'f': function(r, params)
    //{
    //    var tmp = numeric.mul(Math.sqrt(3.0) / params[0], r);
    //    return numeric.mul(numeric.add(1.0, tmp), numeric.exp(numeric.neg(tmp)));
    //}
    //      },
    //      {'id': 3,
    //       'name': 'Matern 5/2',
    //       'f': function(r, params)
    //{
    //    var tmp = numeric.mul(Math.sqrt(5.0) / params[0], r);
    //    var tmp2 = numeric.div(numeric.mul(tmp, tmp), 3.0);
    //    return numeric.mul(numeric.add(numeric.add(1, tmp), tmp2), numeric.exp(numeric.neg(tmp)));
    //}
    //      },
    //      {'id': 4,
    //       'name': 'Rational quadratic (alpha=1)',
    //       'f': function(r, params)
    //{
    //    return numeric.pow(numeric.add(1.0, numeric.div(numeric.pow(r, 2), 2.0 * params[0] * params[0])), -1);
    //}
    //      },
    //      {'id': 5,
    //       'name': 'Piecewise polynomial (q=0)',
    //       'f': function(r, params)
    //{
    //    var tmp = numeric.sub(1.0, numeric.div(r, params[0]));
    //    var dims = numeric.dim(tmp);
    //    for (var i = 0; i < dims[0]; i++)
    //    {
    //        for (var j = 0; j < dims[1]; j++)
    //        {
    //            tmp[i][j] = tmp[i][j] > 0.0 ? tmp[i][j] : 0.0;
    //        }
    //    }
    //    return tmp;
    //}
    //      },
    //      {'id': 6,
    //       'name': 'Piecewise polynomial (q=1)',
    //       'f': function(r, params)
    //{
    //    var tmp1 = numeric.div(r, params[0]);
    //    var tmp = numeric.sub(1.0, tmp1);
    //    var dims = numeric.dim(tmp);
    //    for (var i = 0; i < dims[0]; i++)
    //    {
    //        for (var j = 0; j < dims[1]; j++)
    //        {
    //            tmp[i][j] = tmp[i][j] > 0.0 ? tmp[i][j] : 0.0;
    //        }
    //    }
    //    return numeric.mul(numeric.pow(tmp, 3), numeric.add(numeric.mul(3.0, tmp1), 1.0));
    //}
    //      },
    //      {'id': 7,
    //       'name': 'Periodic (period=pi)',
    //       'f': function(r, params)
    //{
    //    return numeric.exp(numeric.mul(-2.0 / (params[0] *params[0]), numeric.pow(numeric.sin(r), 2)));
    //}
    //      },
    //      {'id': 8,
    //       'name': 'Periodic (period=1)',
    //       'f': function(r, params)
    //{
    //    return numeric.exp(numeric.mul(-2.0 / (params[0] *params[0]), numeric.pow(numeric.sin(numeric.mul(Math.PI, r)), 2)));
    //}
    //      }
    //    ];











}
