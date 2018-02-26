using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using UnscentedKalmanFilter;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;

namespace DemoApp
{
    public class MainWindowViewModel : INPCBase
    {
        [PropertyTools.DataAnnotations.Browsable(false)]
        public ObservableCollection<Measurement> Measurements { get; set; }
        [PropertyTools.DataAnnotations.Browsable(false)]
        public ObservableCollection<Measurement> Estimates { get; set; }



        Matrix<double> Q { get; set; }

        Matrix<double> R { get; set; }
        //FEquation f { get; set; }
        //HEquation h { get; set; }
        Matrix<double> x { get; set; }
        Matrix<double> P { get; set; }
        Matrix<double> H { get; set; }
        Matrix<double> F { get; set; }
        Matrix<double> G { get; set; }


        public int Dimensions { get; set; } = 1;
        public double MeanSquaredError { get; private set; }

        public double q { get; set; } = 0.05;//std of process 
        public double r { get; set; } = 2;
        public double SignalNoise { get; set; } = 1.3;

        //std of measurement

        private int N = 100;

        Random rand;

        VectorBuilder<double> Vbuilder;

        MatrixBuilder<double> Mbuilder;

        public MainWindowViewModel()
        {
            Measurements = new ObservableCollection<Measurement>();
            Estimates = new ObservableCollection<Measurement>();
            Mbuilder = Matrix<double>.Build;
            Vbuilder = Vector<double>.Build;
            rand = new Random();
        }



        public void Run()
        {
            int n = 2;
            int m = 1;
            double alpha = 0.3;
            var filter = new UKF(n, 1);
            var adaptivefilter = new AdaptiveKalmanFilter.Filter(alpha);

            Estimates.Clear();
            Measurements.Clear();

            //Q = Matrix.Build.Diagonal(Dimensions, Dimensions, q * q); //covariance of process
            NotifyChanged("Q");
      
            var f = new FEquation(); //nonlinear state equations
            var h = new HEquation(); //measurement equation
            var x = Vector<double>.Build.Random(n, 1);  //s + q * Matrix.Build.Random(1, 1); //initial state with noise
            var P = Matrix.Build.Diagonal(n, n, 1); //initial state covariance
            var ng = new NoiseGenerator(q,2);
      


            Estimates.Add(new Measurement() { Value = 0, Time = TimeSpan.FromSeconds(1), Variance = 0 });
            for (int k = 1; k < N; k++)
            {
                r = 5;
                var Q =10* ng.MakeWhiteNoise(1);
                R = Matrix.Build.Diagonal(n, n, r * r); //covariance of measurement  

                Tuple<Vector<double>, Matrix<double>> x_and_P = filter.Predict(x, P, f, Q, 1);
                //ukf 
                x = x_and_P.Item1;
                P = x_and_P.Item2;

                Estimates.Add(new Measurement() { Value = x_and_P.Item1[0], Time = TimeSpan.FromSeconds(k + 1), Variance = x_and_P.Item2[0, 0] });

                Vector<double> z = Vector<double>.Build.DenseOfArray(new double[] { ProcessBuilder.SineWave(k, r), 0 });
                //measurments

                Measurements.Add(new Measurement() { Value = z[0], Time = TimeSpan.FromSeconds(k) });

                // R = adaptivefilter.UpdateR(R, filter.E, h.eq,P);

                x_and_P = filter.Update(x, P, z, h, R);                //ukf 
                x = x_and_P.Item1;
                P = x_and_P.Item2;

                for (int p = 0; p < 3; p++)
                {
                    Q = 0.3 * Q;
                    R = 0.3 * R;
                    
                    //Q=adaptivefilter.UpdateQ(Q, filter.d, filter.K);
                    for (int l = k - 1; l > -1; l--)
                    {
                        x_and_P = filter.Predict(x, P, f, Q, 1);
                        //ukf 
                        x = x_and_P.Item1;
                        P = x_and_P.Item2;

                        z = Vector<double>.Build.DenseOfArray(new double[] { Measurements[l].Value, 0 });

                        x_and_P = filter.Update(x, P, z, h, R);                //ukf 
                        x = x_and_P.Item1;
                        P = x_and_P.Item2;


                    }

                    for (int o = 0; o < k; o++)
                    {
                        x_and_P = filter.Predict(x, P, f, Q, 1);
                        //ukf 
                        x = x_and_P.Item1;
                        P = x_and_P.Item2;

                        z = Vector<double>.Build.DenseOfArray(new double[] { Measurements[o].Value, 0 });


                        x_and_P = filter.Update(x, P, z, h, R);                //ukf 
                        x = x_and_P.Item1;
                        P = x_and_P.Item2;
                    }
                }
            }

        }





        public class NoiseGenerator
        {

            Matrix<double> q;

            Matrix<double> qout;

            public NoiseGenerator(double noise, int dimensions)
            {

                q = Matrix.Build.DenseOfColumnArrays(
                new double[][] {
                        new double[] {0.25,0.5},new double[]{0.5,1} });

                qout = Matrix.Build.Dense(2, 2).Multiply(noise);

            }

            public Matrix<double> MakeWhiteNoise(double time)
            {

                // Finally, let's assume that the process noise can be represented
                //by the discrete white noise model - that is, that over each time period 
                //the acceleration is constant.

                var t2 = Math.Pow(time, 2);
                var t3 = t2 * time;
                var t4 = t2 * t2;


                qout[0, 0] = q[0, 0] * t4;
                qout[0, 1] = q[0, 1] * t3;

                qout[1, 0] = q[0, 0] * t3;
                qout[1, 1] = q[0, 1] * t2;

                return qout;

            }
        }


        public class FEquation : IFunction
        {
            Matrix<double> eq = Matrix.Build.DenseOfColumnArrays(
                new double[][] {
                    new double[] { 1, 0 }, new double[] { 1, 1 } });

            public Matrix<double> Process(Matrix<double> x)
            {
                return x;
            }


            public Matrix<double> Process(Matrix<double> x, double time)
            {
                
           
                 var sd=x.EnumerateRows().Select((o,i)=> Process(x.Row(i), time));


                return Matrix.Build.DenseOfRowVectors(sd);


            }


            public Vector<double> Process(Vector<double> x, double time)
            {
                eq[0, 1] = time;

                return eq.Multiply(x);
            }
        }



        public class HEquation : IFunction
        {

            public Matrix<double> eq { get; set; } = Matrix.Build.DenseOfColumnArrays(
                new double[][] {
                    new double[] { 1, 0 }, new double[] { 0, 0 } });


            public Matrix<double> Process(Matrix<double> x)
            {
                var sd = x.EnumerateRows().Select((o, i) => Process(x.Row(i), 0));


                return Matrix.Build.DenseOfRowVectors(sd);
            }


            public Matrix<double> Process(Matrix<double> x, double time)
            {
                var sd = x.EnumerateRows().Select((o, i) => Process(x.Row(i), time));

                return Matrix.Build.DenseOfRowVectors(sd);
            }




            public Vector<double> Process(Vector<double> x, double time)
            {

                return eq.Multiply(x);
            }
        }



        public void Run2(double q, double r)
        {

            int h = 1;

            Estimates.Clear();
            Measurements.Clear();

            x = Mbuilder.Random(Dimensions, 1);  //s + q * Matrix.Build.Random(1, 1); //initial state with noise
            P = Mbuilder.Diagonal(Dimensions, Dimensions, 1); //initial state covariance


            var filter = new MathNet.Filtering.Kalman.DiscreteKalmanFilter(x, P);



            G = Mbuilder.Diagonal(Dimensions, Dimensions, 1); //covariance of process
            Q = Mbuilder.Diagonal(Dimensions, Dimensions, q * q); //covariance of process
            H = Mbuilder.Diagonal(Dimensions, Dimensions, h); //covariance of process

            if (Dimensions == 2)
                F = Matrix.Build.DenseOfColumnArrays(new double[] { 1, 0 }, new double[] { 1, 1 });
            else if (Dimensions == 1)
                F = Mbuilder.DenseOfColumnArrays(new double[] { 1 });


            R = Mbuilder.Diagonal(Dimensions, Dimensions, r * r); //covariance of measurement  



            double lastmeas = 0;
            double ErrorSumSquared = 0;





            for (int k = 1; k < N; k++)
            {

                List<double> ddf = new List<double>();

                var timespan = MathNet.Numerics.Distributions.Normal.Sample(rand, k, 1.0);
                var az = ProcessBuilder.SineWave(timespan, SignalNoise);
                ddf.Add(az);

                if (Dimensions == 2)
                {
                    ddf.Add(az - lastmeas);

                }

                Matrix<double> z = Mbuilder.DenseOfColumnArrays(ddf.ToArray());
                //measurments
                lastmeas = az;


                filter.Predict(F, G, Q);

                Estimates.Add(new Measurement() { Value = filter.State[0, 0], Time = TimeSpan.FromSeconds(k), Variance = filter.Cov[0, 0] });

                Measurements.Add(new Measurement() { Value = z[0, 0], Time = TimeSpan.FromSeconds(k) });

                ErrorSumSquared += Math.Pow(filter.State[0, 0] - z[0, 0], 2);

                filter.Update(z, H, R);             //ukf 



            }




            MeanSquaredError = ErrorSumSquared / N;

            NotifyChanged(nameof(MeanSquaredError));

        }



        public void Run3()
        {

            var population = GeneticAlgorithmFactory.MakeDefault(new UserFunction());


            population.RunEpoch();

            var x = population.BestChromosome;

            //var u= (x as AForge.Genetic.DoubleArrayChromosome).Value;

            //population.



        }


        public void Run4()
        {

            var population = GeneticAlgorithmFactory.MakeDefault(new UserFunction());


            population.RunEpoch();

            var x = population.BestChromosome;

            //population.



        }


    }

    public class UserFunction : AForge.Genetic.OptimizationFunction1D
    {
        public UserFunction() :
           base(new AForge.Range(0, 255))
        { }

        public override double OptimizationFunction(double r)
        {
            var Mbuilder = Matrix<double>.Build;
            Random rand = new Random();
            int Dimensions = 2;
            int h = 1;
            double SignalNoise = 1.2;

            var x = Mbuilder.Random(Dimensions, 1);  //s + q * Matrix.Build.Random(1, 1); //initial state with noise
            var P = Mbuilder.Diagonal(Dimensions, Dimensions, 1); //initial state covariance

            int N = 100;
            var filter = new MathNet.Filtering.Kalman.DiscreteKalmanFilter(x, P);

            List<Measurement> Measurements = new List<Measurement>();

            List<Measurement> Estimates = new List<Measurement>();


            double q = 0.05;//std of process 




            var G = Mbuilder.Diagonal(Dimensions, Dimensions, 1); //covariance of process
            var Q = Mbuilder.Diagonal(Dimensions, Dimensions, q * q); //covariance of process
            var H = Mbuilder.Diagonal(Dimensions, Dimensions, h); //covariance of process

            Matrix<double> F = null;

            if (Dimensions == 2)
                F = Matrix.Build.DenseOfColumnArrays(new double[] { 1, 0 }, new double[] { 1, 1 });
            else if (Dimensions == 1)
                F = Mbuilder.DenseOfColumnArrays(new double[] { 1 });


            var R = Mbuilder.Diagonal(Dimensions, Dimensions, r * r); //covariance of measurement  



            double lastmeas = 0;
            double ErrorSumSquared = 0;





            for (int k = 1; k < N; k++)
            {

                List<double> ddf = new List<double>();

                var timespan = MathNet.Numerics.Distributions.Normal.Sample(rand, k, 1.0);
                var az = ProcessBuilder.SineWave(timespan, SignalNoise);
                ddf.Add(az);

                if (Dimensions == 2)
                {
                    ddf.Add(az - lastmeas);

                }

                Matrix<double> z = Mbuilder.DenseOfColumnArrays(ddf.ToArray());
                //measurments
                lastmeas = az;


                filter.Predict(F, G, Q);

                Estimates.Add(new Measurement() { Value = filter.State[0, 0], Time = TimeSpan.FromSeconds(k), Variance = filter.Cov[0, 0] });

                Measurements.Add(new Measurement() { Value = z[0, 0], Time = TimeSpan.FromSeconds(k) });

                ErrorSumSquared += Math.Pow(filter.State[0, 0] - z[0, 0], 2);

                filter.Update(z, H, R);             //ukf 



            }




            var InverseMeanSquaredError = N / ErrorSumSquared;









            return InverseMeanSquaredError;

        }
    }







    public static class ProcessBuilder
    {
        static Random rand = new Random();

        public static double SineWave(double timespan, double Noise)
        {

            
            return 5*Math.Sin(timespan *10* 3.14/ 180) + MathNet.Numerics.Distributions.Normal.Sample(rand, 0.0, 1.0);



        }




    }


    public struct Measurement
    {
        private double variance;
        public double Value { get; set; }
        public TimeSpan Time { get; set; }
        public double Variance
        {
            get
            {
                return variance;
            }
            set
            {
                variance = value;
                UpperDeviation = Value + Math.Sqrt(variance);
                LowerDeviation = Value - Math.Sqrt(variance);
            }
        }
        public double UpperDeviation { get; private set; }
        public double LowerDeviation { get; private set; }
    }


}




