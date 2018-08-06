using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Filter.Utility;

namespace Filter.Service
{


    public static class SignalGenerator
    {
        public static IEnumerable<KeyValuePair<DateTime, double>> GetPeriodicDefault()
        {

            return MathNet.Numerics.Generate.Sinusoidal(30, 1000.0, 50.0, 10.0)
                    .Select((_, i) =>
                    new KeyValuePair<DateTime, double>(new DateTime(i * 1000000), MathNet.Numerics.Distributions.Normal.Sample(Singleton.Instance.Random,_, 0.1)));
        }


        public static IEnumerable<KeyValuePair<DateTime, double>> GetBrownianDefault()
        {
            double y = 0;int i = 0;
             foreach(var x in Enumerable.Range(0, 30))
            {
                yield return new KeyValuePair < DateTime, double>(new DateTime(i * 1000000), MathNet.Numerics.Distributions.Normal.Sample(Singleton.Instance.Random, y, 0.1));
                i++;
                y += MathNet.Numerics.Distributions.Normal.Sample(Singleton.Instance.Random, 0, 1);
            }
            
        }


        public static IEnumerable<KeyValuePair<DateTime, double>> GetParabolicDefault()
        {

            return Enumerable.Range(0,30)
                    .Select((_) =>
                 new KeyValuePair < DateTime, double > (new DateTime(_ * 1000000), MathNet.Numerics.Distributions.Normal.Sample(Singleton.Instance.Random,Math.Pow(_,2), 0.1)));
        }



    }




}
