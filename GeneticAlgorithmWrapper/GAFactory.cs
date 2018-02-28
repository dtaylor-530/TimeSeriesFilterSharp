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

namespace GeneticAlgorithmWrapper
{




       public  partial class chromosome
        {
        //private double maxWidth = 98;
        //private double maxHeight = 68;

        //public chromosome(double maxWidth, double maxHeight)
        //{

        //    this.maxWidth = maxWidth;
        //    this.maxHeight = maxHeight;

        //}
        public static FloatingPointChromosome CreateIntChromosone(Tuple<double, double>[] minmax)
        {
            //     public FloatingPointChromosome(double[] minValue, double[] maxValue, int[] totalBits, int[] fractionBits);

            var chromosome = new FloatingPointChromosome(
            minValue: minmax.Select(_ => _.Item1).ToArray(),
            maxValue: minmax.Select(_ => _.Item2).ToArray(),
            totalBits: minmax.Select(_ => 10).ToArray(),
            fractionDigits: minmax.Select(_ => 0).ToArray());

            return chromosome;

        }


        public static FloatingPointChromosome CreateLargeIntChromosone(Tuple<double, double>[] minmax)
        {
            //     public FloatingPointChromosome(double[] minValue, double[] maxValue, int[] totalBits, int[] fractionBits);

            var chromosome = new FloatingPointChromosome(
            minValue: minmax.Select(_ => _.Item1).ToArray(),
            maxValue: minmax.Select(_ => _.Item2).ToArray(),
            totalBits: minmax.Select(_ => 20).ToArray(),
            fractionDigits: minmax.Select(_ => 0).ToArray());

            return chromosome;

        }


        public static FloatingPointChromosome CreateFloatChromosone(Tuple<double, double>[] minmax)
        {
            //     public FloatingPointChromosome(double[] minValue, double[] maxValue, int[] totalBits, int[] fractionBits);

            var chromosome = new FloatingPointChromosome(
            minValue: minmax.Select(_ => _.Item1).ToArray(),
            maxValue: minmax.Select(_ => _.Item2).ToArray(),
            totalBits: minmax.Select(_ => 40).ToArray(),
            fractionDigits: minmax.Select(_ => 10).ToArray());

            return chromosome;

        }


    }




        public class GAFactory
        {

            public static GeneticSharp.Domain.GeneticAlgorithm MakeDefault(Func<double[], double> func, Tuple<double, double>[] minmax)
            {


            var population = new Population(20, 40, chromosome.CreateLargeIntChromosone(minmax));

                var fitness = new FuncFitness((c) =>
                {
                    var fc = c as FloatingPointChromosome;

                    var values = fc.ToFloatingPoints();
                    return func(values);

                });

                var selection = new RouletteWheelSelection();
                var crossover = new UniformCrossover(0.5f);
                var mutation = new FlipBitMutation();

                var ga = new GeneticSharp.Domain.GeneticAlgorithm(
                   population,
                   fitness,
                   selection,
                   crossover,
                   mutation);

                var termination =new GenerationNumberTermination(1000);

                ga.Termination = termination;


                Console.WriteLine("Generation: (x1, y1), (x2, y2) = distance");

                return ga;

            }


        }






    
}

