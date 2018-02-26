using AForge.Genetic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApp
{


        //// define optimization function
        //public class UserFunction : OptimizationFunction1D
        //{
        //    public UserFunction() :
        //       base(new AForge.Range(0, 255))
        //    { }

        //    public override double OptimizationFunction(double x)
        //    {
        //        return Math.Cos(x / 23) * Math.Sin(x / 50) + 2;
        //    }
        //}


    public class GeneticAlgorithmFactory
    {



        public static Population MakeDefault(UserFunction userfunction)
        {

            userfunction = userfunction ?? new UserFunction();
            // create genetic population
            Population population = new Population(40, new BinaryChromosome(32),  userfunction, new EliteSelection());

            return population;

        }

        // run one epoch of the population

    }
}


