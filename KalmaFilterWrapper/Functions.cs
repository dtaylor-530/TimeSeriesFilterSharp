using  KalmanFilter;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  KalmanFilter.Common
{


    public class FEquation : ITimeFunction
    {
        Matrix<double> eq = Matrix<double>.Build.DenseOfColumnArrays(
            new double[][] {
                    new double[] { 1, 0 }, new double[] { 1, 1 } });

        public Matrix<double> Process(Matrix<double> x)
        {
            return x;
        }


        public Matrix<double> Process(Matrix<double> x, double time)
        {


            var sd = x.EnumerateRows().Select((o, i) => Process(x.Row(i), time));


            return Matrix<double>.Build.DenseOfRowVectors(sd);

        }

        public Vector<double> Process(Vector<double> x, double time)
        {
            eq[0, 1] = time;

            return eq.Multiply(x);
        }

        public Matrix<double> Matrix(double time)
        {
            eq[0, 1] = time;
            return eq;

        }
    }






    public class HEquation : IFunction
    {

        public Matrix<double> eq { get; set; } = Matrix<double>.Build.DenseOfColumnArrays(
            new double[][] {
                    new double[] { 1, 0 }, new double[] { 0, 0 } });


        public Matrix<double> Process(Matrix<double> x)
        {
            var sd = x.EnumerateRows().Select((o, i) => Process(x.Row(i)));


            return Matrix<double>.Build.DenseOfRowVectors(sd);
        }


        public Vector<double> Process(Vector<double> x)
        {

            return eq.Multiply(x);
        }

        public Matrix<double> Matrix()
        {

            return eq;

        }


    }

}
