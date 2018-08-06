using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter.Model
{


    public struct Estimate
    {
        public Estimate(DateTime time, double value, double variance = 0)
        {
            Time = time;
            Value = value;
            Variance = variance;
            UpperDeviation = value + Math.Sqrt(variance);
            LowerDeviation = value - Math.Sqrt(variance);
        }

        //private double variance;
        public double Value { get; private set; }
        public DateTime Time { get; private set; }
        public double Variance { get; private set; }

        public double UpperDeviation { get; private set; }
        public double LowerDeviation { get; private set; }
    }

}
