using Accord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using Accord.Math.Random;
using Accord.Statistics.Running;
using System.Collections.ObjectModel;
using Filter.Utility;
using Filter.ViewModel;

namespace DemoApp
{
    public class AccordKalmanFilterViewModel
    {
        int delay = 50;
        double noise = 1.3;

        public ObservableCollection<MeasurementsEstimates> Mes { get; set; } = new ObservableCollection<MeasurementsEstimates>();

        public AccordKalmanFilterViewModel()
        {
            Mes.Add(new MeasurementsEstimates { Measurements = new ObservableCollection<Measurement>(), Estimates = new ObservableCollection<Measurement>() });
            Mes.Add(new MeasurementsEstimates { Estimates = new ObservableCollection<Measurement>() });

        }

        DateTime dt = new DateTime(2000, 1, 1);


        public async void Run()
        {

            // Create a new Kalman filter
            var kf = new KalmanFilter2D();

            // sets initial y to near to first measurement
            // kf.Y= Filter.Utility.ProcessFactory.SinePoint(0, noise); 

            double newX = 0, newY = 0, YVar = 0, velX, velY = 0;

            // Push the points into the filter
            for (int i = 0; i < 200; i++)
            {
                double x = 0;
                double y = Filter.Utility.ProcessFactory.SinePoint(i, noise,0);

                Mes[0].Measurements.Add(new Measurement(dt + TimeSpan.FromSeconds(i), y));


                Mes[0].Estimates.Add(new Measurement(dt + TimeSpan.FromSeconds(i), newY, YVar));
                Mes[1].Estimates.Add(new Measurement(dt + TimeSpan.FromSeconds(i), velY, 1));


                kf.Push(x, y);

                // Estimate the points location
                newX = kf.X;
                newY = kf.Y;

                // Estimate the points velocity
                velX = kf.XAxisVelocity;
                velY = kf.YAxisVelocity;

                await Task.Run(() => System.Threading.Thread.Sleep(delay));

            }



            Queue<Measurement> bought = new Queue<Measurement>();
            Queue<Measurement> sold = new Queue<Measurement>();
            List<double> cashed = new List<double>();
            var zip = Mes[0].Measurements.Join(Mes[1].Estimates, a => a.Time, b => b.Time, (c, d) =>
            {
                if (d.Value > 0)
                {
                    bought.Enqueue(new Measurement(c.Time, c.Value, d.Variance));
                    if (sold.Count() > 0)
                    {
                        double amt = 0;
                        double sum = 0;
                        Measurement s = default(Measurement);
                        while (1 / d.Variance > amt & sold.Count() > 0)
                        {
                            s = sold.Dequeue();
                            amt += 1 / (s.Variance);
                            //var diff = (1 / d.Variance - amt);
                            sum += ((s.Value - c.Value) / s.Variance);

                        }
                        //var diff=amt - 1 / d.Variance;

                        // sold.Enqueue(new Measurement(c.Time, (1 - amt) * s.Value, d.Variance));


                        cashed.Add(sum);
                    }

                }
                else if (d.Value < 0)
                {
                    sold.Enqueue(new Measurement(c.Time, c.Value, d.Variance));
                    double amt = 0;
                    double sum = 0;
                    Measurement s = default(Measurement);
                    while (1 / d.Variance > amt & bought.Count() > 0)
                    {
                        s = bought.Dequeue();
                        amt += 1 / (s.Variance);
                        //var diff = (1 / d.Variance - amt);
                        sum += ((c.Value - s.Value) / s.Variance);

                    }
                    //var diff=amt - 1 / d.Variance;

                    // sold.Enqueue(new Measurement(c.Time, (1 - amt) * s.Value, d.Variance));


                    cashed.Add(sum);

                }
                return 0;
            }).ToList();



            var profit = cashed.Where(_ => _ != 0).Average(); ;

        }
    }
}
