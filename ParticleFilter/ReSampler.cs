using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParticleFilter
{

    public interface IReSampler
    {

        List<Particle> Sample(int sampleCount, List<Particle> Particles);
    }



    public class DefaultReSampler : IReSampler
    {


        public List<Particle> Sample(int sampleCount, List<Particle> Particles)
        {
            double[] cumulativeWeights = new double[Particles.Count];

            int cumSumIdx = 0;
            double cumSum = 0;
            foreach (var p in Particles)
            {
                cumSum += p.Weight;
                cumulativeWeights[cumSumIdx++] = cumSum;
            }

            var maxCumWeight = cumulativeWeights[Particles.Count - 1];
            var minCumWeight = cumulativeWeights[0];

            var filteredParticles = new List<Particle>();

            double initialWeight = 1d / Particles.Count;

            for (int i = 0; i < sampleCount; i++)
            {
                var randWeight = minCumWeight + SingleRandom.Instance.NextDouble() * (maxCumWeight - minCumWeight);

                int particleIdx = 0;
                while (cumulativeWeights[particleIdx] < randWeight)
                {
                    particleIdx++;
                }

                var p = Particles[particleIdx];
                p.Weight = 1d / Particles.Count;

                filteredParticles.Add(p);
            }


            return filteredParticles;
        }






    }




    public class SystematicReSampler : IReSampler
    {


        public List<Particle> Sample(int sampleCount, List<Particle> Particles)
        {

            // make N subdivisions, choose positions 
            // with a consistent random offset
            var positions = Enumerable.Range(0, Particles.Count).Select(_ => (_ + SingleRandom.Instance.NextDouble()) / Particles.Count).ToArray();

            //indexes = np.zeros(N, 'i')
            var cumulative_sum = Particles.Select(_ => _.Weight).CumulativeSum().ToArray();
            int i = 0, j = 0;

            int[] indexes = new int[Particles.Count];

            while (i < Particles.Count)
            {
                if (positions[i] < cumulative_sum[j])
                {
                    indexes[i] = j;
                    i += 1;
                }
                else
                    j += 1;
            }

            return ReSampleHelper.ReSampleFromIndex(Particles, indexes);

        }


    }

    // a stochastic universal sampler
    // search online for fast perfect weighted resampling algorithm paper
    public class StochasticReSampler : IReSampler
    {
        public List<Particle> Sample(int sampleCount, List<Particle> Particles)
        {


            int inputLength = Particles.Count;
            var maxweight = Particles.Select(_ => _.Weight).Max();

            int index = SingleRandom.Instance.Next(inputLength);
            double beta = 0.0;


            List<Particle> particles = new List<Particle>();
            for (int i = 0; i < Particles.Count; i++)
            {
                beta = beta + SingleRandom.Instance.NextDouble() * 2.0 * maxweight;
                while (beta > Particles[index].Weight)
                {
                    //while beta > fst input.[index] do
                    beta = beta - Particles[index].Weight;
                    index = (index + 1) % inputLength;
                    particles.Add(Particles[index]);
                }

            }

            return particles;
        }

    }


    //https://homes.cs.washington.edu/~todorov/courses/cseP590/16_ParticleFilter.pdf
    public class LowVarianceReSampler:IReSampler
    {

        /*
Given the current state estimate X and weight vector w, resample a new set of
states.We use the low-variance resampling algorithm from Thrun, Burgard, and Fox's
"Probabilistic Robotics". By default it will keep the number of samples constant
*/
        public List<Particle> Sample(int sampleCount, List<Particle> Particles)
        {

            var r = SingleRandom.Instance.NextDouble() / Particles.Count;
            int[] indexes = new int[Particles.Count];

            for (int i = 0; i < Particles.Count; i++)
            {
                var u = r + (i - 1) / Particles.Count;
                double c = 0;
                int j = 0;
                while (c < u)
                {
                    j++;
                    c = +Particles[i].Weight;


                }
                indexes[i] = j;

            }
            return ReSampleHelper.ReSampleFromIndex(Particles, indexes);

        }
        //            function sample(X, w, M= size(X, 2))
        //    X2 = similar(X, (size(X, 1), M))
        //    w2 = zeros(M)
        //    r = rand(Uniform(0, 1/M))
        //    c = w[1]
        //    i = 1

        //    for 
        //// calculate the next sample point
        //        U = r + (m - 1) * (1 / M)
        //        # find the first weight that puts us past the sample point
        //        while c<U
        //            i += 1
        //            c = c + w[i]
        //        end
        //        X2[:, m] = X[:, i]
        //        w2[m] = w[i]
        //    end
        //    X2, w2
        //end

    }
}