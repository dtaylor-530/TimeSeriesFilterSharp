using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GaussianProcess;
using MathNet.Numerics.Distributions;
using System.Collections.ObjectModel;
using System.ComponentModel;
using KalmanFilter.Common;

namespace DemoApp
{
    public class MainWindowViewModel2:INPCBase
    {
        public ObservableCollection<Measurement> Measurements { get; set; } = new ObservableCollection<Measurement>();
        public ObservableCollection<Measurement>Estimates { get; set; } = new ObservableCollection<Measurement>();
        public void Run()
        {


            // kernel = OrnsteinKernel(1.0)
            var kernel = new Kernel(1.0, 64.0, 0.0, 0.0);


            var gp = new GaussianProcess.Process();
            //Some sample training points.

            double[] xpts = new double[10];
            Normal.Samples(xpts, -1, 2.0);

            //var  xpts =Normalrand(10) * 2 - 1

            //# In the context of Gaussian Processes training means simply
            //# constructing the kernel (or Gram) matrix.
            var mC = gp.Train(xpts, kernel);

            // # Now we draw from the distribution to sample from the gaussian prior.
            var t = gp.draw_multivariate_gaussian(mC.Item1, mC.Item2);

            //pylab.figure(0)
            int i = 0;
            foreach (var elem in xpts)
            {
                i++;
                Measurements.Add(new Measurement { Value = elem ,Time= TimeSpan.FromSeconds(i) });

            }
            i = 0;
            //pylab.figure(0)
            var range = Enumerable.Range(-100, 200).Select(_ => ((double)_) / 1000);
            foreach (var point in range)
            {
                i++;
                var prediction = gp.predict(point, xpts, kernel, mC.Item2, t);
                Estimates.Add(new Measurement {
                    Value = prediction.Item1,
                    Variance = prediction.Item2,
                    Time = TimeSpan.FromSeconds(i)
                });

            }



                NotifyChanged(nameof(Measurements),nameof(Estimates));

        }
    }
}
