using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using UnscentedKalmanFilter;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using KalmanFilter;
using KalmanFilter.Common;
using AForgeEx;

namespace DemoApp
{
    public class MainWindowViewModel : INPCBase
    {
        [PropertyTools.DataAnnotations.Browsable(false)]
        public ObservableCollection<Measurement> Measurements { get; set; }
        [PropertyTools.DataAnnotations.Browsable(false)]
        public ObservableCollection<Measurement> Estimates { get; set; }

        [PropertyTools.DataAnnotations.Browsable(false)]
        ObservableCollection<ObservableCollection<Measurement>> SmoothedEstimates;
            
        Matrix<double> Q { get; set; }

        Matrix<double> R { get; set; }
        FEquation f { get; set; }
        HEquation h { get; set; }
        Vector<double> x;
        Matrix<double> P;
        Vector<double> Mean { get { return x; } set { x = value; NotifyChanged(nameof(Mean)); } }
        Matrix<double> CoVariance { get { return P; } set { P = value; NotifyChanged(nameof(CoVariance)); } }


        Matrix<double> H { get; set; }
        Matrix<double> F { get; set; }
        Matrix<double> G { get; set; }


        //public int Dimensions { get; set; } = 1;
        public double MeanSquaredError { get; private set; }

        public double q { get; set; } = 4;//std of process 
        public double r { get; set; } = 3;
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

            Initialise();
        }

        public int n = 2;

        NoiseGenerator ng;

        Unscented filter;



        int k = 0;



        public void Initialise()
        {

            int m = 1;
            double alpha = 0.3;
            filter = new Unscented(n, 1);
            var adaptivefilter = new AdaptiveKalmanFilter.Filter(alpha);


            R = Matrix.Build.Diagonal(n, n, r * r); //covariance of measurement  
            Q = Matrix.Build.Diagonal(n,n, q * q); //covariance of process
    

            f = new FEquation(); //nonlinear state equations
            h = new HEquation(); //measurement equation
            x = Vector<double>.Build.Random(n, 1);  //s + q * Matrix.Build.Random(1, 1); //initial state with noise
            P = Matrix.Build.Diagonal(n, n, 1); //initial state covariance
            ng = ng ?? new NoiseGenerator(q, 2);

    


        }


        public void Run()
        {

            //if(!initialiseFlag) Initialise();

            Estimates.Clear();
            Measurements.Clear();

            Estimates.Add(new Measurement() { Value = 0, Time = TimeSpan.FromSeconds(-1), Variance = 0 });

            k = 0;
            while (k<N)
            {


                AddMeasurement();

                AddEstimate();

              
            }

        }



        public void AddMeasurement()
        {
            Vector<double> z = Vector<double>.Build.DenseOfArray(new double[] { ProcessBuilder.SineWave(k, r), 0 });

            filter.Update(ref x, ref P, z, h, R);                //ukf 
   

            Measurements.Add(new Measurement() { Value = z[0], Time = TimeSpan.FromSeconds(k) });
        }



        public void AddEstimate()
        {

            var x_and_P = filter.Predict(x, P, f, Q, 1);
             x = x_and_P.Item1;
             P = x_and_P.Item2;


            Estimates.Add(new Measurement() { Value = x_and_P.Item1[0], Time = TimeSpan.FromSeconds(k + 1), Variance = x_and_P.Item2[0, 0] });


            k++;

        }





        public void Smoooth()
        {
            ObservableCollection<ObservableCollection<Measurement>> SmoothedEstimates = new ObservableCollection<ObservableCollection<Measurement>>();

            for (int p = 0; p < 1; p++)
            {

                for (int l = k - 1; l > -1; l--)
                {

                    var x_and_P = filter.Predict(x, P, f, Q, 1);
                    x = x_and_P.Item1;
                    P = x_and_P.Item2;

                    var z = Vector<double>.Build.DenseOfArray(new double[] { Measurements[l].Value, 0 });

                    filter.Update(ref x, ref P, z, h, R);                //ukf 
    

                }

                for (int o = 0; o < k; o++)
                {
                    var x_and_P = filter.Predict(x, P, f, Q, 1);
                    x = x_and_P.Item1;
                    P = x_and_P.Item2;

                    var z = Vector<double>.Build.DenseOfArray(new double[] { Measurements[o].Value, 0 });

                    filter.Update(ref x,ref P, z, h, R);                //ukf 
        



                }
            }





        }






        public void Run2(double q, double r)
        {

            int h = 1;

            Estimates.Clear();
            Measurements.Clear();

           var x = Mbuilder.Random(n, 1);  //s + q * Matrix.Build.Random(1, 1); //initial state with noise
          var  P = Mbuilder.Diagonal(n, n, 1); //initial state covariance


            var filter = new MathNet.Filtering.Kalman.DiscreteKalmanFilter(x, P);



            G = Mbuilder.Diagonal(n, n, 1); //covariance of process
            Q = Mbuilder.Diagonal(n, n, q * q); //covariance of process
            H = Mbuilder.Diagonal(n, n, h); //covariance of process

            if (n == 2)
                F = Matrix.Build.DenseOfColumnArrays(new double[] { 1, 0 }, new double[] { 1, 1 });
            else if (n == 1)
                F = Mbuilder.DenseOfColumnArrays(new double[] { 1 });


            R = Mbuilder.Diagonal(n, n, r * r); //covariance of measurement  



            double lastmeas = 0;
            double ErrorSumSquared = 0;





            for (int k = 1; k < N; k++)
            {

                List<double> ddf = new List<double>();

                var timespan = MathNet.Numerics.Distributions.Normal.Sample(rand, k, 1.0);
                var az = ProcessBuilder.SineWave(timespan, SignalNoise);
                ddf.Add(az);

                if (n == 2)
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

  
    }







   







