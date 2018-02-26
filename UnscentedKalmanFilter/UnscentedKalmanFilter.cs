using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System;
using System.Linq;

using System.Collections.Generic;

namespace UnscentedKalmanFilter
{
	public class UKF
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
        public Matrix<double> d { get; set; }



        /// <summary>
        /// Residual
        /// </summary>
        public Matrix<double> E { get; set; }



        /// <summary>
        /// Constructor of Unscented Kalman Filter
        /// </summary>
        /// <param name="L">States number</param>
        /// <param name="m">Measurements number</param>
        public UKF(int L, int m)
		{
            this.L = L;
            this.m = m;
            alpha = 1e-3f;
            ki = 0;  
            beta = 2f;
            lambda = alpha * alpha * (L + ki) - L;
            c = L + lambda;

            //weights for means
            Wm = Vector.Build.Dense( (2 * L + 1), 0.5 / c);
            Wm[0] = lambda / c;

            //weights for covariance
            Wc = Vector.Build.Dense( (2 * L + 1));
            Wm.CopyTo(Wc);
            Wc[0] = Wm[0] + 1 - alpha * alpha + beta;

            E = Matrix.Build.Dense(L, L,0);
            K = Matrix.Build.Dense(L, L, 0);
            d = Matrix.Build.Dense(L, L, 0);
            c = Math.Sqrt(c);	
		}

     //   /// <summary>
     //   /// Update process
     //   /// </summary>
     //   /// <param name="f">nonlinear state equations</param>
     //   /// <param name="x_and_P">state and covariance</param>
     //   /// <param name="h">measurement equation</param>
     //   /// <param name="z">current measurement</param>
     //   /// <param name="Q">process noise covariance </param>
     //   /// <param name="R">measurement noise covariance </param>
     //   /// <returns></returns>
     //   public Matrix<double>[] Update(IFunction f, Matrix<double> x, Matrix<double> P, IFunction h, Matrix<double> z, Matrix<double> Q, Matrix<double> R) 
	    //{

     //       //sigma points around x
     //       Matrix<double> X = GetSigmaPoints(x, P, c);


     //       //unscented transformation of process
     //       // X1=sigmas(x1,P1,c) - sigma points around x1
     //       ////X2=X1-x1(:,ones(1,size(X1,2))) - deviation of X1
     //       //Matrix<double>[] ut_f_matrices = UnscentedTransform(f, X, Wm, Wc, L, Q);  
     //       //Matrix<double> x1 = ut_f_matrices[0];
     //       //Matrix<double> X1 = ut_f_matrices[1];
     //       //Matrix<double> P1 = ut_f_matrices[2];
     //       //Matrix<double> X2 = ut_f_matrices[3];

     //       ////unscented transformation of measurments
     //       //Matrix<double>[] ut_h_matrices = UnscentedTransform(h, X1, Wm, Wc, m, R);  
     //       //Matrix<double> z1 = ut_h_matrices[0];
     //       //Matrix<double> Z1 = ut_h_matrices[1];
     //       //Matrix<double> P2 = ut_h_matrices[2];
     //       //Matrix<double> Z2 = ut_h_matrices[3];

     //       ////transformed cross-covariance
     //       //Matrix<double> P12 = (X2.Multiply(Matrix.Build.Diagonal(Wc.Row(0).ToArray()))).Multiply(Z2.Transpose());

     //       //Matrix<double> K = P12.Multiply(P2.Inverse());

     //       //state update
     //       //var resultX = x1.Add(K.Multiply(z.Subtract(z1)));
     //       ////covariance update 
     //       //var resultP = P1.Subtract(K.Multiply(P12.Transpose()));

     //       //return new Matrix<double>[] { resultX, resultP };
     //       return null;
	    //}



        public Tuple<Vector<double>, Matrix<double>> Predict(Vector<double> x, Matrix<double> P, IFunction f, Matrix<double> Q,double time)
        {

            //sigma points around x
            Matrix<double> X = GetSigmaPoints2(x, P, lambda,L);


            X= f.Process(X, time);


            Tuple<Vector<double>,Matrix<double>> utmatrices = UnscentedTransform2(X, Wm,Wc,Q);


            return utmatrices;

        }


        public Tuple<Vector<double>, Matrix<double>> Update(Vector<double> x, Matrix<double> P, Vector<double> z,IFunction h, Matrix<double>R)
        {


            //sigma points around x
            Matrix<double> Xf = GetSigmaPoints2(x, P, lambda,L);


            Matrix<double> Xh = h.Process(Xf);


            Tuple<Vector<double>, Matrix<double>> utmatrices = UnscentedTransform2( Xh, Wm, Wc, R);

            var zp = utmatrices.Item1;
            var Pz = utmatrices.Item2;

            Matrix<double> Pxz = Matrix.Build.Dense(z.Count(), Xf.ColumnCount, 0);


            for (int i = 0; i < Xh.RowCount; i++)
            {
                Pxz += Wc[i] * ((Xf.Row(i).Subtract(x)).OuterProduct((Xh.Row(i).Subtract(x))));
            }

            var K = Pxz.Multiply(Pz.Inverse());

            var innovation= K.Multiply(z - zp);

            x = x + innovation;
            P = P- K.Multiply(Pz).Multiply(K.Transpose());

            return new Tuple<Vector<double>, Matrix<double>>(x, P);

        }


//        def update(self, z):
//    # rename for readability
//    sigmas_f = self.sigmas_f
//    sigmas_h = self.sigmas_h

//    # transform sigma points into measurement space
//    for i in range(self._num_sigmas):
//        sigmas_h[i] = self.hx(sigmas_f[i])

//# mean and covariance of prediction passed through UT
//    zp, Pz = unscented_transform(sigmas_h, self.Wm, self.Wc, self.R)

//    # compute cross variance of the state and the measurements
//        Pxz = np.zeros((self._dim_x, self._dim_z))
//    for i in range(self._num_sigmas):
//        Pxz += self.Wc[i] * np.outer(sigmas_f[i] - self.xp,
//                                    sigmas_h[i] - zp)

//    K = dot(Pxz, inv(Pz)) # Kalman gain

//    self.x = self.xp + dot(K, z-zp)
//    self.P = self.Pp - dot(K, Pz).dot(K.T)





        /// <summary>
        /// Unscented Transformation
        /// </summary>
        /// <param name="f">nonlinear map</param>
        /// <param name="X">sigma points</param>
        /// <param name="Wm">Weights for means</param>
        /// <param name="Wc">Weights for covariance</param>
        /// <param name="n">number of outputs of f</param>
        /// <param name="R">additive covariance</param>
        /// <returns>[transformed mean, transformed smapling points, transformed covariance, transformed deviations</returns>
        private static Matrix<double>[] UnscentedTransform(IFunction f, Matrix<double> X, Matrix<double> Wm, Matrix<double> Wc, int n, Matrix<double> R)
        {
            int L = X.ColumnCount;
            Matrix<double> y = Matrix.Build.Dense(n, 1, 0);
            Matrix<double> Y = Matrix.Build.Dense(n, L, 0);

            Matrix<double> row_in_X;
            for (int k = 0; k < L; k++)
            {
                row_in_X = X.SubMatrix(0, X.RowCount, k, 1);
                Y.SetSubMatrix(0, Y.RowCount, k, 1, f.Process(row_in_X));
                y = y.Add(Y.SubMatrix(0, Y.RowCount, k, 1).Multiply(Wm[0,k]));
            }

            Matrix<double> Y1 = Y.Subtract(y.Multiply(Matrix.Build.Dense(1,L,1)));
            Matrix<double> P = Y1.Multiply(Matrix.Build.Diagonal(Wc.Row(0).ToArray()));
            P = P.Multiply(Y1.Transpose());
            P = P.Add(R);

            Matrix<double>[] output = { y, Y, P, Y1 };
            return output;
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
        private static Matrix<double>[] UnscentedTransform(IFunction f,double time, Matrix<double> X, Matrix<double> Wm, Matrix<double> Wc, int n, Matrix<double> R)
        {
            int L = X.ColumnCount;
            Matrix<double> y = Matrix.Build.Dense(n, 1, 0);
            Matrix<double> Y = Matrix.Build.Dense(n, L, 0);

            Vector<double> row_in_X = null;
            List<Vector<double>> lvd = new List<Vector<double>>();
            for (int k = 0; k < L; k++)
            {
                row_in_X = X.Row(k);
                var fx = f.Process(row_in_X, time);


                //Y.SetSubMatrix(0, Y.RowCount, k, 1, f.Process(row_in_X));
                //y = y.Add(Y.SubMatrix(0, Y.RowCount, k, 1).Multiply(Wm[0, k]))
            }

   
            //process each row in the vector
            var xxx = X.EnumerateRows().Select((_, i) => f.Process(X.Row(i), time) );

            // average of each column in combined vector
            var gf = X.Row(0).Select((a, i) => lvd.Average(_ => _[i]));

            //to get mean
            var xfd = Vector.Build.DenseOfArray(gf.ToArray());

            //to get covariance
            var dfdf=xxx.Select(_ => 
            { var v = Vector.Build.DenseOfArray( _.Select((__, i) => __ - xfd[i]).ToArray());
                var opv = v.OuterProduct(v);
                return opv;
            });

           var fd= dfdf.Aggregate((_, __) => _.Append(__));

            Matrix<double> P = fd.Divide(X.RowCount);


            Matrix<double> Y1 = Y.Subtract(y.Multiply(Matrix.Build.Dense(1, L, 1)));
            P = Y1.Multiply(Matrix.Build.Diagonal(Wc.Row(0).ToArray()));
            P = P.Multiply(Y1.Transpose());
            P = P.Add(R);

            Matrix<double>[] output = { y, Y, P, Y1 };
            return output;
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
        private static Tuple<Vector<double>, Matrix<double>> UnscentedTransform2(  Matrix<double> X, Vector<double> Wm, Vector<double> Wc, Matrix<double> Noise)
        {

            //var x =Vector.Build.DenseOfArray( X.EnumerateRows().Select(_=>_.DotProduct  (Wm)).ToArray());
            var x= X.TransposeThisAndMultiply(Wm);

            Matrix<double> P = Matrix.Build.Dense(X.ColumnCount, X.ColumnCount, 0);

            for (int i=0;i<X.RowCount;i++)
            {
                var k = X.Row(i).Subtract(x);
                P+=Wc[i] * (k.OuterProduct(k));
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
        private static Matrix<double> GetSigmaPoints(Matrix<double> x, Matrix<double> P, double c)
        {
            Matrix<double> A = P.Cholesky().Factor;

            A = A.Multiply(c);
            A = A.Transpose();

            int n = x.RowCount;

            Matrix<double> Y = Matrix.Build.Dense(n, n, 1);
            for (int j = 0; j < n; j++)
            {
                Y.SetSubMatrix(0, n, j, 1, x);
            }

            Matrix<double> X = Matrix.Build.Dense(n, (2 * n + 1));
            X.SetSubMatrix(0, n, 0, 1, x);

            Matrix<double> Y_plus_A = Y.Add(A);
            X.SetSubMatrix(0, n, 1, n, Y_plus_A);

            Matrix<double> Y_minus_A = Y.Subtract(A);
            X.SetSubMatrix(0, n, n + 1, n, Y_minus_A);

            return X;
        }


        /// <summary>
        /// Sigma points around reference point
        /// </summary>
        /// <param name="x">reference point</param>
        /// <param name="P">covariance</param>
        /// <param name="c">coefficient</param>
        /// <returns>Sigma points</returns>
        private static Matrix<double> GetSigmaPoints2(Vector<double> x, Matrix<double> P, double lambda, int n )
        {

            Matrix<double> U = P.Multiply(n + lambda).Cholesky().Factor.Transpose();


  
            Matrix<double> X = Matrix.Build.Dense( 2 * n + 1,n);

            X.SetRow(0, x);


            for (int i = 0; i < n; i++)
            {
                X.SetRow(i + 1, x+U.Row(i));
                X.SetRow(n + i + 1, x-U.Row(i));
            }



//            sigmas = np.zeros((2 * n + 1, n))
//U = scipy.linalg.cholesky((n + lambda_) * P) # sqrt

//sigmas[0] = X
//for k in range(n):
//    sigmas[k + 1] = X + U[k]
//      sigmas[n + k + 1] = X - U[k]



            return X;
        }



    }
}




// void SetSubMatrix(int rowIndex, int sorceRowIndex, int rowCount, int columnIndex, int sourceColumnIndex, int columnCount, Matrix<T> subMatrix)

//        int rowIndex

//The row to start copying to.
//int sorceRowIndex

//The row of the sub-matrix to start copying from.
//int rowCount

//The number of rows to copy. Must be positive.
//int columnIndex

//The column to start copying to.
//int sourceColumnIndex

//The column of the sub-matrix to start copying from.
//int columnCount

//The number of columns to copy. Must be positive.
//Matrix<T> subMatrix

//The sub-matrix to copy from. 