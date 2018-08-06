using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalmanFilter
{
    public interface IFunction
    {
        Matrix<double> Process(Matrix<double> x);
        Vector<double> Process(Vector<double> x);
        Matrix<double> Matrix();

    }


    public interface ITimeFunction
    {

        Matrix<double> Process(Matrix<double> x, double time);
        Vector<double> Process(Vector<double> x, double time);
        Matrix<double> Matrix(double time);

    }



    public class StateFunctions
    {
        //private static readonly VectorBuilder<double> vBuilder = Vector<double>.Build;
        private static readonly MatrixBuilder<double> mBuilder = Matrix<double>.Build;

        public static Matrix<double> BuildTransition(int size)
        {
            switch (size)
            {
                case (3):
                    return mBuilder.DenseOfColumnArrays(new double[] { 1, 0, 0 }, new double[] { 1, 1, 0 }, new double[] { 0.5, 1, 1 });
                case (2):
                    return mBuilder.DenseOfColumnArrays(new double[] { 1, 0 }, new double[] { 1, 1 });
                case (1):
                    return mBuilder.DenseOfColumnArrays(new double[] { 1 });
                default:
                    throw new Exception("matrix size exceeds 3, the maximum");
            }
        }



        public static Matrix<double> BuildMeasurement(int size, int measurements)
        {
            //mBuilder = mBuilder ?? Matrix<double>.Build;

            List<double[]> lst = new List<double[]>();
            for (int j = 0; j < measurements; j++)
            {
                var x = Enumerable.Range(0, size).Select(_ => (double)0).ToArray();
                x[j] = 1;
                lst.Add(x);
            }

            return mBuilder.DenseOfRowArrays(lst);


        }






    }


    public static class StateFunctionsEx
    {

        //public static void UpdateTransition(this Matrix<double> F, TimeSpan ts)
        //{

        //    if (F.RowCount > 1)
        //        F[0, 1] = ts.TotalSeconds;

        //    if (F.RowCount > 2)
        //    {
        //        F[0, 2] = 0.5 * ts.TotalSeconds* ts.TotalSeconds;
        //        F[1, 2] = ts.TotalSeconds;
        //    }

        //}
    }
}
