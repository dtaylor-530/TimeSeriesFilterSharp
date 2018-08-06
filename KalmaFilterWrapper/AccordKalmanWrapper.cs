
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using Accord.Math.Random;
using Accord.Statistics.Running;
using System.Collections.ObjectModel;

using System.Reactive.Linq;
using Filter.Model;

namespace KalmanFilter.Wrap
{
    public class AccordKalmanFilterWrapper:IFilterWrapper
    {


        private KalmanFilter2D kf;


        public AccordKalmanFilterWrapper(double noise)
        {
            // Create a new Kalman filter
            kf = new KalmanFilter2D();
            kf.NoiseY = noise;
       
        }

       

        public IEnumerable<KeyValuePair<DateTime,Tuple< double,double>[]>> BatchRun(IEnumerable<KeyValuePair<DateTime, double>> measurements)
        {
            //DateTime dt = measurements.First().Item1;

            // Push the points into the filter
            foreach(var meas in measurements)
            {
                kf.Push((meas.Key.Ticks*1000000), meas.Value );
                yield return new KeyValuePair<DateTime, Tuple<double, double>[]> (meas.Key,new[] { Tuple.Create(kf.Y, kf.NoiseY) , Tuple.Create(kf.Y, kf.YAxisVelocity) });

            }


        }


        public IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>> Run(IObservable<KeyValuePair<DateTime, double?>> measurements)
        {
            // Push the points into the filter
            return measurements.Select(meas =>
            {
                if(meas.Value!=null)
                kf.Push((meas.Key.Ticks * 1000000), (double)meas.Value);
                return new KeyValuePair<DateTime, Tuple<double, double>[]>(meas.Key, new[] { Tuple.Create(kf.Y, kf.NoiseY), Tuple.Create(kf.Y, kf.YAxisVelocity) });

            });


        }
    }
}
