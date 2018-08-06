using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter.Model
{
    public interface INoisyKernel
    {

        int Factor { get; set; }
        int Sigma { get; set; }
        // Func<int, int, Func<int, double>>  Function { get; set; }

        double Evaluate(int i);


    }




    public class Kernel : INoisyKernel
    {

        public int Factor { get; set; }
        public int Sigma { get; set; }
        public Func<int, int, Func<int, double>> Function { get; set; }

        public double Evaluate(int i)
        {
            return Function(Factor, Sigma)(i);

        }
    }

    public class BKernel : INoisyKernel
    {
        public BKernel(int startvalue = 0)
        {
            previousvalue = startvalue;
        }

        double previousvalue { get; set; } 
        public int Factor { get; set; }
        public int Sigma { get; set; }
        public Func<int, int, Func<int, double, double>> Function { get; set; }

        public double Evaluate(int i)
        {
            previousvalue = Function(Factor, Sigma)(i, previousvalue);
            return Function(Factor, Sigma)(i, previousvalue);

        }

    }


    public static class KernelHelper
    {

        public static IEnumerable<double> EvaluateMany(this Kernel k, int cnt)
        {
            for (int i = 0; i < cnt; i++)
                yield return k.Evaluate(i);

        }
    }
}
