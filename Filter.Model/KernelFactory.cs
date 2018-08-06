using Filter.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter.Model
{





    public class KernelFactory
    {



        public static INoisyKernel MakeKernelRandom()
        {
            //var pair = Helper.GetRandomPair(Singleton.Instance.Random);
            var rand = Singleton.Instance.Random;
            var pair = new[] { rand.Next(1, 3), rand.Next(1, 3) };
            int i = rand.Next(2);
            switch (i)
            {
                default:
                case (0):
                    return MakeSinKernel(pair[0], pair[1]);
                case (1):
                    return MakeCosKernel(pair[0], pair[1]);
                case (2):
                    return MakeBrownianKernel(pair[0], pair[1]);

            };
        }


        public static Kernel MakeSinKernel(int factor, int sigma)
        {
            //var pair = Helper.GetRandomPair(Singleton.Instance.Random);
            var rand = Singleton.Instance.Random;
            return new Kernel
            {
                Factor = factor,
                Sigma = sigma,
                Function = (a, b) => NoisyEquation.Sin(a, b, rand),

            };

        }


        public static Kernel MakeCosKernel(int factor, int sigma)
        {
            var rand = Singleton.Instance.Random;
            return new Kernel
            {
                Factor = factor,
                Sigma = sigma,
                Function = (a, b) => NoisyEquation.Cos(a, b, rand),
            };

        }




        public static Kernel MakeExpKernel(int factor, int sigma)
        {
            var rand = Singleton.Instance.Random;
            return new Kernel
            {
                Factor = factor,
                Sigma = sigma,
                Function = (a, b) => NoisyEquation.Exp(a, b, rand)
            };

        }


        public static BKernel MakeBrownianKernel(int factor, int sigma)
        {
            var rand = Singleton.Instance.Random;
            //http://scipy-cookbook.readthedocs.io/items/BrownianMotion.html
            return new BKernel
            {
                Factor = factor,
                Sigma = sigma,

                Function = (a, b) => NoisyEquation.Brownian(a, b, rand)
            };

        }

    }





}


