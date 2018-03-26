using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using KalmanFilter;
using KalmanFilter.Wrap;

using AForgeEx;
using System.Threading.Tasks;
using MathNet.Filtering.Kalman;
using Filter.Utility;
using KalmanFilter.Common;

namespace DemoApp
{




    public class MainWindowViewModel : Filter.ViewModel.INPCBase
    {
        [PropertyTools.DataAnnotations.Browsable(false)]
        

        public ObservableCollection<Measurement> Measurements { get; set; }
        [PropertyTools.DataAnnotations.Browsable(false)]
        public ObservableCollection<Measurement> Estimates { get; set; }


        private List<Tuple<Matrix<double>, Matrix<double>>> _estimates;


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

        private DiscreteKalmanFilter dfilter;

        Matrix<double> G { get; set; }


        //public int Dimensions { get; set; } = 1;
        public double MeanSquaredError { get; private set; }

        public double q { get; set; } = 2;//std of process 
        public double r { get; set; } = 1;
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
            //Estimates = new ObservableCollection<Measurement>();
            //Estimates = new ObservableCollection<Measurement>();
            Mbuilder = Matrix<double>.Build;
            Vbuilder = Vector<double>.Build;
            rand = new Random();

            Initialise();
        }

        public int n = 2;



        Unscented filter;



        int k = 0;



        public void Initialise()
        {

            //int m = 1;
            double alpha = 0.3;
            filter = new Unscented(n, 1);
            var adaptivefilter = new KalmanFilter.AEKF(alpha);


            R = Matrix.Build.Diagonal(n, n, r * r); //covariance of measurement  
            Q = Matrix.Build.Diagonal(n, n, q * q); //covariance of process


            f = new FEquation(); //nonlinear state equations
            h = new HEquation(); //measurement equation
            x = Vector<double>.Build.Random(n, 1);  //s + q * Matrix.Build.Random(1, 1); //initial state with noise
            P = Matrix.Build.Diagonal(n, n, 1); //initial state covariance
                                                //ng = ng ?? new NoiseGenerator(q, 2);




        }

        DateTime dt;

        public void Run()
        {

            //if(!initialiseFlag) Initialise();
            _estimates = new List<Tuple<Matrix<double>, Matrix<double>>>();
            Estimates.Clear();
            Measurements.Clear();
            dt = new DateTime();
            Estimates.Add(new Measurement(value: 0, time: dt, variance: 0));

            k = 0;
            while (k < N)
            {


                AddMeasurement();

                AddEstimate();


            }

        }



        public void AddMeasurement()
        {
            Vector<double> z = Vector<double>.Build.DenseOfArray(new double[] { ProcessFactory.SinePoint(k, r,0), 0 });

            filter.Update(ref x, ref P, z, h, R);                //ukf 


            Measurements.Add(new Measurement(value: z[0], time: dt + TimeSpan.FromSeconds(k)));
        }



        public void AddEstimate()
        {

            var x_and_P = filter.Predict(x, P, f, Q, 1);
            x = x_and_P.Item1;
            P = x_and_P.Item2;


            Estimates.Add(new Measurement(value: x_and_P.Item1[0], time: dt + TimeSpan.FromSeconds(k + 1), variance: x_and_P.Item2[0, 0]));

            //_estimates.Add(Tuple.Create(x, P));
            k++;

        }




        public void Smooth()
        {
            ////R = Mbuilder.DenseDiagonal(n, n, 0);

            Estimates.Clear();

            //var x = Smoother.Smooth(_estimates, Q, f);

            //int cnt = _estimates.Count();
            //Estimates = new ObservableCollection<Measurement>(x
            //    .Select((_, i) => new Measurement(
            //     value: _.Item1[0],
            //     time: dt + TimeSpan.FromSeconds(cnt - i),
            //     variance: _.Item2[0, 0])));

            NotifyChanged(nameof(Estimates));
        }





        public async void RunRecursion()
        {
            Estimates.Clear();
            for (int p = 0; p < 3; p++)
            {

                for (int l = k - 1; l > 1; l--)
                {

                    var x_and_P = filter.Predict(x, P, f, Q, 1);
                    x = x_and_P.Item1;
                    P = x_and_P.Item2;

                    var z = Vector<double>.Build.DenseOfArray(new double[] { Measurements[l - 1].Value, 0 });

                    filter.Update(ref x, ref P, z, h, R);                //ukf 

                    Estimates.Add(new Measurement(value: x[0], time: dt + TimeSpan.FromSeconds(l), variance: P[0, 0]));

                    for (int o = l; o < k; o++)
                    {
                        x_and_P = filter.Predict(x, P, f, Q, 1);
                        x = x_and_P.Item1;
                        P = x_and_P.Item2;


                        z = Vector<double>.Build.DenseOfArray(new double[] { Measurements[o].Value, 0 });

                        filter.Update(ref x, ref P, z, h, R);                //ukf 
                    }
                    await Task.Run(() =>

                    System.Threading.Thread.Sleep(500));

                }
                k = N;



                await Task.Run(() =>

       System.Threading.Thread.Sleep(5000));

                Estimates.Clear();

                await Task.Run(() =>

       System.Threading.Thread.Sleep(2000));
            }

            Estimates.Clear();



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















