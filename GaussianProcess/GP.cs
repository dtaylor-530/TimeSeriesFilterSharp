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


namespace GaussianProcess
{

    //tomi.peltola@tmpl.fi 
    //http://www.tmpl.fi/gp/

    public class GP
    {
        public static int Mte { get; set; }

        //public static double[] z { get; }
        //public static double[] p { get; }


        private IMatrixkernel _kernel;
        private double _length;
        private double _noise;

        Matrix<double> _Kte;



        static GP()
        {

            Mte = DistanceMatrix.Instance.Size;
            //z = RandomHelper.randnArray(Mte);
            //p = RandomHelper.randnArray(Mte);
            //Normal.Samples(rand, 60, 1)
        }


        public GP(IMatrixkernel kernel, double length, double noise)
        {

            this._kernel = kernel;
            this._length = length;
            this._noise = noise;
            this._Kte = kernel.Main(DistanceMatrix.Instance.Matrix, length);
        }




        public GPOut Compute(Matrix<double> mTr, Matrix<double> mTeTr, Vector<double> vTrY)
        {
            var Mtr = (mTr).RowCount;

            var gpOut = new GPOut();

            var Kte = _kernel.Main(DistanceMatrix.Instance.Matrix, _length);

            if (Mtr > 0)
            {

                var Kxx_p_noise = _kernel.Main(mTr, _length);
                for (int i = 0; i < Mtr; i++)
                    Kxx_p_noise[i, i] += _noise;

                var svd1 = Kxx_p_noise.Svd();

                for (int i = 0; i < Mtr; i++)
                    svd1.S[i] = svd1.S[i] > Double.Epsilon ? 1.0 / svd1.S[i] : 0;

                var tmp = _kernel.Main(mTeTr, _length) * svd1.U;

                gpOut.mu = tmp * (svd1.S.PointwiseMultiply(svd1.U.Transpose() * (vTrY)));
                var cov = tmp * Matrix<double>.Build.DenseOfDiagonalVector(svd1.S.PointwiseSqrt());
                cov = cov * cov.Transpose();
                cov = Kte/*.SubMatrix(0,100,0,100) */- cov;
                var svd2 = cov.Svd();
                for (int i = 0; i < Mte; i++)
                {
                    if (svd2.S[i] < Double.Epsilon)
                    {
                        svd2.S[i] = 0.0;
                    }
                }
                gpOut.proj = svd2.U * Matrix<double>.Build.DenseOfDiagonalVector(svd2.S.PointwiseSqrt());
                gpOut.sd95 = 1.98 * (gpOut.proj * gpOut.proj.Transpose()).Diagonal().PointwiseSqrt();
            }
            else
            {
                gpOut.sd95 = 1.98 * (Kte.Diagonal()).PointwiseSqrt();
                var svd = Kte.Svd();
                gpOut.proj = svd.U * Matrix<double>.Build.DenseOfDiagonalVector(svd.S.PointwiseSqrt());
                gpOut.mu = Vector<double>.Build.DenseOfArray(Enumerable.Range(0, Mte).Select(_ => 0d).ToArray());
            }


            return gpOut;

        }


    }

}