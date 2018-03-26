using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalmanFilter.Wrap
{


    public class KalmaFilterFactory
    {


        public static List<DiscreteWrapper> Build((int, int) qrange, (int, int) rrange)
        {


            List<DiscreteWrapper> kfs = new List<DiscreteWrapper>();

            for (int i = qrange.Item1; i < qrange.Item2; i++)
                for (int j = rrange.Item1; j < rrange.Item2; j++)
                {
                    var r = new double[] { j*5 }; ;
                    var q = new double[] { (double)i/5, (double)i/5 };
                    var kf = new KalmanFilter.Wrap.DiscreteWrapper(r, q);

                    kfs.Add(kf);
                }

            return kfs;
        }

    }

}
