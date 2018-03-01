
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using MathNet.Numerics.LinearAlgebra;


namespace GeneticAlgorithmWrapper
{
    public class OuterWrap
    {

        public event Action<double[]> NotifyOfImprovement;


        private GeneticSharp.Domain.GeneticAlgorithm ga;
        public double latestFitness { get; private set; }



        public int GenerationNumber
        {
            get
            {
                return ga.GenerationsNumber;
            }
        }

        public double[] Result
        {
            get
            {

                return (ga.BestChromosome as FloatingPointChromosome).ToFloatingPoints();



            }
        }


        public OuterWrap(params Tuple<double, double>[] minmax)
        {
            latestFitness = 0.0;

            ga = GAFactory.MakeDefault(FunctionToOptimize, minmax);

            ga.GenerationRan += NewGeneration;

            NotifyOfImprovement += GeneticAlgorithm_NotifyOfImprovement;


        }



        public static double FunctionToOptimize(double[] vals)
        {
            int d = 1;

            var r = vals.Skip(1).Take(d).Select(_ => _ / 1000).ToArray();
            var q = vals.Skip(1 + d).Take(d).Select(_ => _ / 1000).ToArray();

           var measurements = GenerateMeasurements(r, q,d);

            BatchUpdate(out List<Tuple<Matrix<double>, Matrix<double>>> estimates, measurements, r, q, d);

            var ErrorSumSquared = measurements.Zip(estimates, (a, b) => Math.Pow(a.Item1[0, 0] - b.Item1[0, 0], 2)).Sum();

            var InverseMeanSquaredError = 1000 / ErrorSumSquared;


            //var TrueErrorSumSquared = measurements.Zip(trueValues, (a, b) => Math.Pow(a.Value - b.Value, 2)).Sum();


            return InverseMeanSquaredError;


        }




        public static void BatchUpdate(out List<Tuple<Matrix<double>, Matrix<double>>> estimates,
            List<Tuple<Matrix<double>, TimeSpan>> measurements, double[] q, double[] r, int Dimensions = 1, double signalNoise = 1.3)
        {

            var filter = Initialise(r, q,
                out Matrix<double> G, out Matrix<double> Q, out Matrix<double> H, out Matrix<double> F, out Matrix<double> R,
                signalNoise, Dimensions);



            estimates = new List<Tuple<Matrix<double>, Matrix<double>>>();
            //measurements = new List<Measurement>();


            foreach (var meas in measurements)
            {

                List<double> ddf = new List<double>();



                filter.Predict(F, G, Q);

                //estimates.Add(new Measurement() { Value = filter.State[0, 0], Time = meas.Time, Variance = filter.Cov[0, 0] });
                estimates.Add(new Tuple<Matrix<double>, Matrix<double>>(filter.State, filter.Cov));



                //trueValues.Add(new Measurement() { Value = actual, Time = timespan, Variance=0 });

                filter.Update(meas.Item1, H, R);             //ukf 


            }



        }


        public static List<Tuple<Matrix<double>, TimeSpan>> GenerateMeasurements(double[] r, double[] q, int d = 1)
        {


 // (int)vals[0];
         
            var signalNoise = 1.3;

            var process = new ProcessBuilder(d, signalNoise, Equation.Sine);

            List<Tuple<Matrix<double>, TimeSpan>> measurements = new List<Tuple<Matrix<double>, TimeSpan>>(
            Enumerable.Range(0, 100).Select(i =>
            {
                var z = process.Next(out double actual, out TimeSpan timespan);

                return new Tuple<Matrix<double>, TimeSpan>(z, timespan);
            }));


            return measurements;
        }





        public static MathNet.Filtering.Kalman.DiscreteKalmanFilter Initialise(double[] r, double[] q
            , out Matrix<double> G, out Matrix<double> Q, out Matrix<double> H, out Matrix<double> F, out Matrix<double> R
            , double SignalNoise = 1.2, int Dimensions = 2, MatrixBuilder<double> Mbuilder = null)
        {


            Mbuilder = Mbuilder ?? Matrix<double>.Build;
            Random rand = new Random();



            var x = Mbuilder.Random(Dimensions, 1);  //s + q * Matrix.Build.Random(1, 1); //initial state with noise
            var P = Mbuilder.Diagonal(Dimensions, Dimensions, 1); //initial state covariance

     
            var filter = new MathNet.Filtering.Kalman.DiscreteKalmanFilter(x, P);


            G = Mbuilder.Diagonal(Dimensions, Dimensions, 1); //covariance of process
            Q = Mbuilder.Diagonal(Dimensions, Dimensions, q); //covariance of process
            H = Mbuilder.Diagonal(Dimensions, Dimensions, 1); //covariance of process

            F = null;

            if (Dimensions == 3)
                F = Mbuilder.DenseOfColumnArrays(new double[] { 1, 0, 0 }, new double[] { 1, 1, 0 }, new double[] { 1, 1, 1 });
            else if (Dimensions == 2)
                F = Mbuilder.DenseOfColumnArrays(new double[] { 1, 0 }, new double[] { 1, 1 });
            else if (Dimensions == 1)
                F = Mbuilder.DenseOfColumnArrays(new double[] { 1 });


            R = Mbuilder.Diagonal(Dimensions, Dimensions, r); //covariance of measurement  


            return filter;
        }


        // start the algorithm running/learning
        public void Run()
        {
            ga.Start();

            ga.GenerationRan += Ga_GenerationRan;


        }

        private void Ga_GenerationRan(object sender, EventArgs e)
        {



        }


        // notify subsribers of improvement to algorithm
        private void GeneticAlgorithm_NotifyOfImprovement(double[] x)
        {

            Console.WriteLine(
                            "Generation {0}: (q {1},r {2}) fitness{3}",
                            ga.GenerationsNumber, x[0], x[1], latestFitness);
        }



        // the procreation of new chromosomes
        private void NewGeneration(object sender, EventArgs e)
        {
            var bestChromosome = (sender as GeneticSharp.Domain.GeneticAlgorithm).BestChromosome as FloatingPointChromosome;
            var bestFitness = bestChromosome.Fitness.Value;

            if (bestFitness != latestFitness)
            {
                latestFitness = bestFitness;
                var phenotype = bestChromosome.ToFloatingPoints();

                NotifyOfImprovement(phenotype);

            }

        }




    }





    public class ProcessBuilder
    {
        static Random rand = new Random();

        int Dimensions;
        double SignalNoise;
        Equation Equation;
        int i = 0;
        double[] lastmeas;
        MatrixBuilder<double> mbuilder;

        public ProcessBuilder(int dimensions, double signalNoise, Equation equation)
        {

            Dimensions = dimensions;
            mbuilder = Matrix<double>.Build;
            SignalNoise = signalNoise;
            Equation = equation;
            //Arr = Enumerable.Range(0, Dimensions).ToArray();


        }




        public static double SineWave(double timespan, double Noise)
        {

            var z = 3 * (Math.Sin(timespan * 3.14 * 5 / 180) + MathNet.Numerics.Distributions.Normal.Sample(rand, 0.0, Noise));


            return z;

        }


        public Matrix<double> Next(out double actual, out TimeSpan timespan)
        {

            i++;

            var iteration = MathNet.Numerics.Distributions.Normal.Sample(rand, i, 1.0);
            timespan = TimeSpan.FromSeconds(iteration);
            actual = ProcessBuilder.SineWave(iteration, SignalNoise);

            var ddf = new List<double> { actual };

            for (int i = 1; i < Dimensions; i++)
            {
                ddf.Add((actual - lastmeas[i]) / iteration);

            }


            lastmeas = ddf.ToArray();

            Matrix<double> z = mbuilder.DenseOfColumnArrays(lastmeas);



            return z;

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


    public enum Equation { Sine }



}
