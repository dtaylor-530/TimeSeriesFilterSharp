
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
using Filter.Common;
using KalmanFilter.Wrap;
using Filter.Service;

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

         int d = 1;
        private IEnumerable<KeyValuePair<DateTime,double>> measurements;


        public OuterWrap(params Tuple<double, double>[] minmax)
        {
            latestFitness = 0.0;

            ga = GAFactory.MakeDefault(FunctionToOptimize, minmax);

            ga.GenerationRan += NewGeneration;


            measurements = SignalGenerator.GetPeriodicDefault();
        
           
            NotifyOfImprovement += GeneticAlgorithm_NotifyOfImprovement;


        }



        public  double FunctionToOptimize(double[] vals)
        {
   

            var r = vals.Skip(1).Take(d).Select(_ => _ ).ToArray();
            var q = vals.Skip(1 + d).Take(d).Select(_ => _ ).ToArray();


            var dkf = new KalmanFilter.Wrap.DiscreteWrapper(r,q);

           

            var estimates=  dkf.BatchRun( measurements);

            var ErrorSumSquared = measurements.Zip(estimates, (a, b) => Math.Pow(a.Value- b.Value[0].Item1, 2)).Sum();

            var InverseMeanSquaredError = 1000 / ErrorSumSquared;


            //var TrueErrorSumSquared = measurements.Zip(trueValues, (a, b) => Math.Pow(a.Value - b.Value, 2)).Sum();


            return InverseMeanSquaredError;


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
                            ga.GenerationsNumber, x[1], x[3], latestFitness);
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





    //public class ProcessBuilder
    //{
    //    static Random rand = new Random();

    //    int Dimensions;
    //    double SignalNoise;
    //    Equation Equation;
    //    int i = 0;
    //    double[] lastmeas;
    //    MatrixBuilder<double> mbuilder;

    //    public ProcessBuilder(int dimensions, double signalNoise, Equation equation)
    //    {

    //        Dimensions = dimensions;
    //        mbuilder = Matrix<double>.Build;
    //        SignalNoise = signalNoise;
    //        Equation = equation;
    //        //Arr = Enumerable.Range(0, Dimensions).ToArray();


    //    }




    //    public static double SineWave(double timespan, double Noise)
    //    {

    //        var z = 3 * (Math.Sin(timespan * 3.14 * 5 / 180) + MathNet.Numerics.Distributions.Normal.Sample(rand, 0.0, Noise));


    //        return z;

    //    }


    //    public Matrix<double> Next(out double actual, out TimeSpan timespan)
    //    {

    //        i++;

    //        var iteration = MathNet.Numerics.Distributions.Normal.Sample(rand, i, 1.0);
    //        timespan = TimeSpan.FromSeconds(iteration);
    //        actual = ProcessBuilder.SineWave(iteration, SignalNoise);

    //        var ddf = new List<double> { actual };

    //        for (int i = 1; i < Dimensions; i++)
    //        {
    //            ddf.Add((actual - lastmeas[i]) / iteration);

    //        }


    //        lastmeas = ddf.ToArray();

    //        Matrix<double> z = mbuilder.DenseOfColumnArrays(lastmeas);



    //        return z;

    //    }




    //}

    public class GAMeasurement
    {

        public double q { get; set; }
        public double r { get; set; }

        public double Error { get; set; }
        public double Innovation { get; set; }
    }

    //    public struct Measurement
    //{
    //    private double variance;
    //    public double Value { get; set; }
    //    public TimeSpan Time { get; set; }
    //    public double Variance
    //    {
    //        get
    //        {
    //            return variance;
    //        }
    //        set
    //        {
    //            variance = value;
    //            UpperDeviation = Value + Math.Sqrt(variance);
    //            LowerDeviation = Value - Math.Sqrt(variance);
    //        }
    //    }
    //    public double UpperDeviation { get; private set; }
    //    public double LowerDeviation { get; private set; }
    //}


    public enum Equation { Sine }



}
