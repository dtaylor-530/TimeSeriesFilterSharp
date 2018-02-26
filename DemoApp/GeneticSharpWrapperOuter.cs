
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


namespace GeneticAlgorithmSample
{
    public class GeneticAlgorithmWrapper
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

        public GeneticAlgorithmWrapper(params Tuple<double, double>[] minmax)
        {
            latestFitness = 0.0;

            ga = GAFactory.MakeDefault(FunctionToOptimize, minmax);

            ga.GenerationRan += NewGeneration;

            NotifyOfImprovement += GeneticAlgorithm_NotifyOfImprovement;


        }




        //public double FunctionToOptimise(double[] values)
        //{


        //    var x1 = values[0];
        //    var y1 = values[1];
        //    var x2 = values[2];
        //    var y2 = values[3];

        //    return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        //}


        public static double FunctionToOptimize(double[] vals)
        {

            var d = 1;// (int)vals[0];
            var r = vals.Skip(1).Take(d).Select(_ => _ / 1000).ToArray();
            var q = vals.Skip(1+d).Take(d).Select(_=>_/1000).ToArray();


            Run(out List<Measurement> measurements, out List<Measurement> estimates, out List<Measurement> trueValues, 
                
                r, q,100,d);
           
            var ErrorSumSquared = measurements.Zip(estimates,(a,b)=> Math.Pow(a.Value-b.Value, 2)).Sum();

            var InverseMeanSquaredError = 1000/ ErrorSumSquared;


            var TrueErrorSumSquared = measurements.Zip(trueValues, (a, b) => Math.Pow(a.Value - b.Value, 2)).Sum();


            return InverseMeanSquaredError;


        }

        public static void Run(out List<Measurement> measurements,out List<Measurement> estimates, out List<Measurement> trueValues, double[] q,double[] r, int N=100,int Dimensions=1,double signalNoise =1.3 )
        {


            var process = new ProcessBuilder(Dimensions, signalNoise,Equation.Sine);




            var filter = Initialise(r, q, 
                out Matrix<double> G,out Matrix<double> Q, out Matrix<double> H, out Matrix<double> F,  out Matrix<double> R,
                signalNoise, Dimensions);



            estimates = new List<Measurement>();
            measurements = new List<Measurement>();
            trueValues = new List<Measurement>();

            for (int k = 1; k < N; k++)
            {

                List<double> ddf = new List<double>();

                var z = process.Next(out double actual,out TimeSpan timespan);

                filter.Predict(F, G, Q);

                estimates.Add(new Measurement() { Value = filter.State[0, 0], Time = timespan, Variance = filter.Cov[0, 0] });

                measurements.Add(new Measurement() { Value = z[0, 0], Time = timespan });

                trueValues.Add(new Measurement() { Value = actual, Time = timespan, Variance=0 });

                filter.Update(z, H, R);             //ukf 



            }


        

        }



        public static MathNet.Filtering.Kalman.DiscreteKalmanFilter Initialise(double[] r , double[] q
            ,out Matrix<double>G, out Matrix<double> Q, out Matrix<double> H, out Matrix<double> F, out Matrix<double> R
            ,double SignalNoise = 1.2,int Dimensions = 2, MatrixBuilder<double> Mbuilder=null)
        {


            Mbuilder = Mbuilder??Matrix<double>.Build;
            Random rand = new Random();
    


            var x = Mbuilder.Random(Dimensions, 1);  //s + q * Matrix.Build.Random(1, 1); //initial state with noise
            var P = Mbuilder.Diagonal(Dimensions, Dimensions, 1); //initial state covariance

            int N = 100;
            var filter = new MathNet.Filtering.Kalman.DiscreteKalmanFilter(x, P);


            G = Mbuilder.Diagonal(Dimensions, Dimensions, 1); //covariance of process
           Q = Mbuilder.Diagonal(Dimensions, Dimensions, q ); //covariance of process
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
                            ga.GenerationsNumber, x[0],x[1] ,latestFitness);
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

        public ProcessBuilder( int dimensions,double signalNoise,Equation equation)
        {

            Dimensions = dimensions;
            mbuilder = Matrix<double>.Build;
            SignalNoise = signalNoise;
            Equation = equation;
            //Arr = Enumerable.Range(0, Dimensions).ToArray();


        }




        public static double SineWave(double timespan, double Noise)
        {

            var z=3*(Math.Sin(timespan * 3.14 * 5 / 180) + MathNet.Numerics.Distributions.Normal.Sample(rand, 0.0, Noise));


            return z;

        }


        public Matrix<double> Next(out double actual,out TimeSpan timespan)
        {

            i++;
    
            var iteration = MathNet.Numerics.Distributions.Normal.Sample(rand, i, 1.0);
            timespan = TimeSpan.FromSeconds(iteration);
            actual = ProcessBuilder.SineWave(iteration, SignalNoise);

            var ddf = new List<double>    {  actual };

            for (int i = 1; i < Dimensions; i++)
            { 
                ddf.Add((actual - lastmeas[i])/iteration);

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


    public enum Equation { Sine}



}
