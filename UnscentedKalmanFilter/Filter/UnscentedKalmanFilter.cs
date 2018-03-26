using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System;
using System.Linq;

using System.Collections.Generic;

namespace KalmanFilter
{
    public class Unscented
    {
        /// <summary>
        /// States number of parameter
        /// </summary>
        private int L;

        /// <summary>
        /// Measurements number of parameter
        /// </summary>
        private int m;

        /// <summary>
        /// The alpha coefficient, characterize sigma-points dispersion around mean
        /// </summary>
        private double alpha;

        /// <summary>
        /// The ki.
        /// </summary>
        private double ki;

        /// <summary>
        /// The beta coefficient, characterize type of distribution (2 for normal one) 
        /// </summary>
        private double beta;

        /// <summary>
        /// Scale factor
        /// </summary>
        private double lambda;

        /// <summary>
        /// Scale factor
        /// </summary>
        private double c;

        /// <summary>
        /// Means weights
        /// </summary>
        private Vector<double> Wm;

        /// <summary>
        /// Covariance weights
        /// </summary>
        private Vector<double> Wc;

   

        /// <summary>
        /// Kalman Gain
        /// </summary>
        public Matrix<double> K { get; set; }



        /// <summary>
        /// Innovation
        /// </summary>
        public Vector<double> d { get; set; }



        /// <summary>
        /// Residual
        /// </summary>
        public Vector<double> E { get; set; }



        /// <summary>
        /// Constructor of Unscented Kalman Filter
        /// </summary>
        /// <param name="L">States number</param>
        /// <param name="m">Measurements number</param>
        public Unscented(int L, int m)
        {
            this.L = L;
            this.m = m;
            alpha = 1e-3f;
            ki = 0;
            beta = 2f;
            lambda = alpha * alpha * (L + ki) - L;
            c = L + lambda;

            //weights for means
            Wm = Vector.Build.Dense((2 * L + 1), 0.5 / c);
            Wm[0] = lambda / c;

            //weights for covariance
            Wc = Vector.Build.Dense((2 * L + 1));
            Wm.CopyTo(Wc);
            Wc[0] = Wm[0] + 1 - alpha * alpha + beta;


           
            c = Math.Sqrt(c);


            //adap = new Adaptive1(0.3);
        }

        //Adaptive1 adap;



        public Tuple<Vector<double>, Matrix<double>> Predict(Vector<double> x, Matrix<double> P, ITimeFunction f, Matrix<double> Q, double time)
        {

            //sigma points around x
            Matrix<double> X = GetSigmaPoints(x, P, lambda, L);


            X = f.Process(X, time);

            //if(innovation!=null)
            //  Q = adap.UpdateQ(Q,innovation,kalmanGain);

            Tuple<Vector<double>, Matrix<double>> utmatrices = UnscentedTransform(X, Wm, Wc, Q);


            return utmatrices;

        }


        public void Update(ref Vector<double> x, ref Matrix<double> P, Vector<double> z, IFunction h, Matrix<double> R)
        {


            //sigma points around x
            Matrix<double> Xf = GetSigmaPoints(x, P, lambda, L);


            Matrix<double> Xh = h.Process(Xf);


            //if (residual != null)
            //    R = adap.UpdateR(R, residual, h, P);

            Tuple<Vector<double>, Matrix<double>> utmatrices = UnscentedTransform(Xh, Wm, Wc, R);

            var zp = utmatrices.Item1;
            var Pz = utmatrices.Item2;

            Matrix<double> Pxz = Matrix.Build.Dense(z.Count(), Xf.ColumnCount, 0);


            for (int i = 0; i < Xh.RowCount; i++)
            {
                Pxz += Wc[i] * ((Xf.Row(i).Subtract(x)).OuterProduct((Xh.Row(i).Subtract(x))));
            }

            K= Pxz.Multiply(Pz.Inverse());

            d= z - zp;
            //var correction =Xf+ K.Multiply(d);

            x = x + d;
            P = P - K.Multiply(Pz).Multiply(K.Transpose());

            //E = z - h.Process(correction);

   


        }






        /// <summary>
        /// Unscented Transformation with time
        /// see https://en.wikipedia.org/wiki/Unscented_transform for example
        /// </summary>
        /// <param name="f">nonlinear map</param>
        /// <param name="X">sigma points</param>
        /// <param name="Wm">Weights for means</param>
        /// <param name="Wc">Weights for covariance</param>
        /// <param name="n">number of outputs of f</param>
        /// <param name="R">additive covariance</param>
        /// <returns>[transformed mean, transformed smapling points, transformed covariance, transformed deviations</returns>
        private static Tuple<Vector<double>, Matrix<double>> UnscentedTransform(Matrix<double> X, Vector<double> Wm, Vector<double> Wc, Matrix<double> Noise)
        {

            //var x =Vector.Build.DenseOfArray( X.EnumerateRows().Select(_=>_.DotProduct  (Wm)).ToArray());
            var x = X.TransposeThisAndMultiply(Wm);

            Matrix<double> P = Matrix.Build.Dense(X.ColumnCount, X.ColumnCount, 0);

            for (int i = 0; i < X.RowCount; i++)
            {
                var k = X.Row(i).Subtract(x);
                P += Wc[i] * (k.OuterProduct(k));
            }

            P += Noise;


            return new Tuple<Vector<double>, Matrix<double>>(x, P);
        }






        /// <summary>
        /// Sigma points around reference point
        /// </summary>
        /// <param name="x">reference point</param>
        /// <param name="P">covariance</param>
        /// <param name="c">coefficient</param>
        /// <returns>Sigma points</returns>
        private static Matrix<double> GetSigmaPoints(Vector<double> x, Matrix<double> P, double lambda, int n)
        {

            Matrix<double> U = P.Multiply(n + lambda).Cholesky().Factor.Transpose();
          

            Matrix<double> X = Matrix.Build.Dense(2 * n + 1, n);

            X.SetRow(0, x);


            for (int i = 0; i < n; i++)
            {
                X.SetRow(i + 1, x + U.Row(i));
                X.SetRow(n + i + 1, x - U.Row(i));
            }


            return X;
        }



    }
}


