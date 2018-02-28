using KalmanFilter.Common;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalmanFilter.Wrap
{
    public class Wrapper
    {
        public IFunction f { get; set; }
        public IFunction h { get; set; }

        Matrix<double> Q { get; set; }

        Matrix<double> R { get; set; }

        Vector<double> x;
        Matrix<double> P;

        //Vector<double> Mean { get { return x; } set { x = value; } }
        //Matrix<double> CoVariance { get { return P; } set { P = value; } }

        Matrix<double> H { get; set; }
        Matrix<double> F { get; set; }
        Matrix<double> G { get; set; }


        //public int Dimensions { get; set; } = 1;
        public double MeanSquaredError { get; private set; }

        public double ProcessNoise { get; set; } = 4;//std of process 
        public double EstimateNoise { get; set; } = 3;
        public double SignalNoise { get; set; } = 1.3;
        public Dictionary<TimeSpan, double[]> Means { get; private set; }
        public Dictionary<TimeSpan, double[,]> CoVariances { get; private set; }
        //Dictionary<TimeSpan, Vector<double>> Measurements;
        //std of measurement


        private int N ;

        Random rand;

        VectorBuilder<double> Vbuilder;

        MatrixBuilder<double> Mbuilder;


        NoiseGenerator ng;

        Unscented filter;




        public Wrapper(
            int dimensions = 2, double r = 3, double q = 1, double alpha = 0.3,
            double  estimateNoise=3)

        {
            InitialiseGeneral();

            InitialiseParameters(dimensions,r,q,alpha);
        }




        int k = 0;


        public void InitialiseGeneral()
        {
            Means = new Dictionary<TimeSpan,double[]>();
            CoVariances = new Dictionary<TimeSpan, double[,]>();
            //Measurements = new Dictionary<TimeSpan, Vector<double>>();
            Mbuilder = Matrix<double>.Build;
            Vbuilder = Vector<double>.Build;
            rand = new Random();


        }



        public void InitialiseParameters(int n=2,double r=3,double q=1,double alpha=0.3)
        {
            EstimateNoise = r;
            ProcessNoise = q;


            int m = 1;
            filter = new Unscented(n, 1);
            var adaptivefilter = new KalmanFilter.Adaptive1(alpha);


            R = Matrix.Build.Diagonal(n, n, EstimateNoise * EstimateNoise); //covariance of measurement  
            Q = Matrix.Build.Diagonal(n, n, ProcessNoise * ProcessNoise); //covariance of process


            f = new FEquation(); //nonlinear state equations
            h = new HEquation(); //measurement equation
            x = Vector<double>.Build.Random(n, 1);  //s + q * Matrix.Build.Random(1, 1); //initial state with noise
            P = Matrix.Build.Diagonal(n, n, 1); //initial state covariance
            ng = ng ?? new NoiseGenerator(ProcessNoise, 2);




        }



        public void Predict(out double[] x_, out double[,] P_,TimeSpan ts)
        {

            lastTimeSpan = ts;


            var xp = filter.Predict(x, P, f, Q, lastTimeSpan.Ticks);

            x_ = x.ToArray();
            Means[ts] = x_;

            P_ = P.ToArray();

            CoVariances[ts] = P_;





        }




        public void Update(double[] z)
        {
       
            var z_ = Vbuilder.DenseOfArray(z);
            filter.Update(ref x, ref P, z_, h, R);



        }


        TimeSpan lastTimeSpan;

        public void BatchUpdate( List<Tuple<TimeSpan,double[]>> z)
        {
             //var x_ =  new double[] { 1, 1 } ;
             //var P_ =  new double[] { 1, 1 } ;

            // x = Vbuilder.DenseOfArray(x_);
            //P = Mbuilder.DenseOfDiagonalArray(P_);
            //var R_= Mbuilder.DenseOfDiagonalArray(R);

            for (int i = 0; i < z.Count(); i++)
            {
                lastTimeSpan = z[i].Item1;
                var xp=filter.Predict(x,  P, f, Q, lastTimeSpan.Ticks);

                x = xp.Item1;
                P = xp.Item2;

                Means[z[i].Item1]= x.ToArray();
                CoVariances[z[i].Item1] = P.ToArray();
         

                var z_ = Vbuilder.DenseOfArray(z[i].Item2);
                filter.Update(ref x, ref P, z_, h, R);



            }


        }









    }
}
