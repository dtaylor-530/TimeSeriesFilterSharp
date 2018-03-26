using AForge.Genetic;
using Filter.Utility;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeEx
{


    // define optimization function
    public class UserFunctionDefault : OptimizationFunction1D
    {
        public UserFunctionDefault() :
           base(new AForge.Range(0, 255))
        {
        }

        public override double OptimizationFunction(double x)
        {
            return Math.Cos(x / 23) * Math.Sin(x / 50) + 2;
        }
    }




    public class GeneticAlgorithmFactory
    {



        public static Population MakeDefault(UserFunction userfunction)
        {

            userfunction = userfunction ?? new UserFunction();
            // create genetic population
            Population population = new Population(40, new BinaryChromosome(32), userfunction, new EliteSelection());

            return population;

        }

        // run one epoch of the population

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



            var dt = new DateTime();

            for (int k = 1; k < N; k++)
            {

                List<double> ddf = new List<double>();

                var timespan = MathNet.Numerics.Distributions.Normal.Sample(rand, k, 1.0);
                var az = ProcessFactory.SinePoint(timespan, SignalNoise,0);
                ddf.Add(az);

                if (Dimensions == 2)
                {
                    ddf.Add(az - lastmeas);

                }

                Matrix<double> z = Mbuilder.DenseOfColumnArrays(ddf.ToArray());
                //measurments
                lastmeas = az;


                filter.Predict(F, G, Q);

                Estimates.Add(new Measurement(value : filter.State[0, 0], time : dt+TimeSpan.FromSeconds(k), variance : filter.Cov[0, 0] ));

                Measurements.Add(new Measurement( value : z[0, 0], time : dt+TimeSpan.FromSeconds(k) ));

                ErrorSumSquared += Math.Pow(filter.State[0, 0] - z[0, 0], 2);

                filter.Update(z, H, R);             //ukf 



            }




            var InverseMeanSquaredError = N / ErrorSumSquared;









            return InverseMeanSquaredError;

        }


    }


}