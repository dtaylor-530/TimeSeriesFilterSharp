//using MathNet.Numerics.LinearAlgebra;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using MathNet.Filtering.Kalman;

//namespace KalmanFilter
//{
//    public class Discrete
//    {



//        int _dimensions;
//        private DiscreteKalmanFilter filter;
//        private Matrix<double> G;

//        public Matrix<double> Q { get; private set; }
//        public Matrix<double> H { get; private set; }
//        public Matrix<double> F { get; private set; }
//        public Matrix<double> R { get; private set; }





//        public Discrete(double[] r, double[] q, int dimensions = 2)
//        {

//            _dimensions = dimensions;
//            Initialise(new double[] { 2,2 }, new double[] { 2, 2 });

//        }



//        public MathNet.Filtering.Kalman.DiscreteKalmanFilter Initialise(double[] r, double[] q
            
//            , MatrixBuilder<double> Mbuilder = null)
//        {


//            Mbuilder = Mbuilder ?? Matrix<double>.Build;
//            Random rand = new Random();



//            var x = Mbuilder.Random(_dimensions, 1);  //s + q * Matrix.Build.Random(1, 1); //initial state with noise
//            var P = Mbuilder.Diagonal(_dimensions, _dimensions, 1); //initial state covariance


//             filter = new MathNet.Filtering.Kalman.DiscreteKalmanFilter(x, P);


//            G = Mbuilder.Diagonal(_dimensions, _dimensions, 1); //covariance of process
//            Q = Mbuilder.Diagonal(_dimensions, _dimensions, q); //covariance of process
//            H = Mbuilder.Diagonal(_dimensions, _dimensions, 1); //covariance of process

//            F = null;

//            if (_dimensions == 3)
//                F = Mbuilder.DenseOfColumnArrays(new double[] { 1, 0, 0 }, new double[] { 1, 1, 0 }, new double[] { 1, 1, 1 });
//            else if (_dimensions == 2)
//                F = Mbuilder.DenseOfColumnArrays(new double[] { 1, 0 }, new double[] { 1, 1 });
//            else if (_dimensions == 1)
//                F = Mbuilder.DenseOfColumnArrays(new double[] { 1 });


//            R = Mbuilder.Diagonal(_dimensions, _dimensions, r); //covariance of measurement  


//            return filter;
//        }






//        public void BatchUpdate(out List<Tuple<Matrix<double>, Matrix<double>>> estimates,
//            List<Tuple<Matrix<double>, TimeSpan>> measurements, double[] q, double[] r, int _dimensions = 1, double signalNoise = 1.3)
//        {

  


//            estimates = new List<Tuple<Matrix<double>, Matrix<double>>>();
//            //measurements = new List<Measurement>();


//            foreach (var meas in measurements)
//            {

//                List<double> ddf = new List<double>();



//                filter.Predict(F, G, Q);

//                //estimates.Add(new Measurement() { Value = filter.State[0, 0], Time = meas.Time, Variance = filter.Cov[0, 0] });
//                estimates.Add(new Tuple<Matrix<double>, Matrix<double>>(filter.State, filter.Cov));



//                //trueValues.Add(new Measurement() { Value = actual, Time = timespan, Variance=0 });
//                var m = Matrix<double>.Build.DenseOfColumnArrays( new double[] { meas.Item1[0,0] , 0 });

//                filter.Update(m, H, R);             //ukf 


//            }



//        }



//    }
//}
