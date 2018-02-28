using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalmanFilter.Common
{


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
    
}
