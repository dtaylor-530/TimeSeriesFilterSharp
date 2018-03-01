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
            IKernel kernel = new Kernel(1.0, 64.0, 0.0, 0.0);


            var gp = new GaussianProcess.Process();
            //Some sample training points.

            double[] xpts = new double[10];
            Normal.Samples(xpts, 0, 1);


            List<double> x = new List<double>(new double[] { 0.0 });

            //var  xpts =Normalrand(10) * 2 - 1

            //# In the context of Gaussian Processes training means simply
            //# constructing the kernel (or Gram) matrix.
            var mC = gp.Train(xpts, kernel);

            // # Now we draw from the distribution to sample from the gaussian prior.
            var t = gp.draw_multivariate_gaussian(mC.Item1, mC.Item2);

            //pylab.figure(0)

            //pylab.figure(0)
            var range = Enumerable.Range(-300, 600).Select(_ => ((double)_) / 100);

            foreach (var point in range)
            {

                var prediction = gp.predict(point, xpts, kernel, mC.Item2, t);

                Estimates.Add(new Measurement {
                    Value = prediction.Item1,
                    Variance = prediction.Item2,
                    Time = TimeSpan.FromSeconds(point)
                });

            }



            int k = 0;
            foreach (var x_ in xpts)
            {
                Measurements.Add(new Measurement
                {
                    Value = t[k],
                    Time = TimeSpan.FromSeconds(x_)
                });
                k++;
            }


        }




        public void Run2()
        {
            var gp = new GaussianProcess.Process2();

            List<double> x = new List<double>(new double[] { 0.0 });
            List<double> y = new List<double>(new double[] { 0.0 });



            var x_more = new double[] { -2.1, -1.5, 0.3, 1.8, 2.5 };
            var y_more = new List<double>();

            Random rand = new Random();

            IKernel kernel = new Kernel(1.0, 10.0, 0.0, 0.0);

            //double[] xpts = new double[10];
            //Normal.Samples(xpts, -1, 2.0);
            //List<Tuple<double, double>> mus = new List<Tuple<double, double>>();

            foreach (var xm in x_more)
            {
                var mus= gp.conditional(xm, x.ToArray(), y.ToArray());

                y_more.Add(  Normal.Sample(rand,mus.Item1,mus.Item2));
            }



            var σ_new = gp.exponential_cov(x_more.ToArray(), x_more.ToArray());


            var range = Enumerable.Range(-300,600).Select(_ =>((double) _)/100);


            //var predictions = new List<Tuple<double, double>>();

            foreach (var i in range)
            {
               var prediction= gp.predict(i, x_more, σ_new, y_more.ToArray());


                Estimates.Add(new Measurement
                {
                    Value = prediction.Item1,
                    Variance = prediction.Item2,
                    Time = TimeSpan.FromSeconds(i)
                });



           

            }

            int k = 0;
            foreach (var x_ in x_more)
            {
                Measurements.Add(new Measurement
                {
                    Value = y_more[k],
                    Time = TimeSpan.FromSeconds(x_)
                });
                k++;
            }








        }

     
    }
}
