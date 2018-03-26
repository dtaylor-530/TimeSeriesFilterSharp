
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GaussianProcess;
using MathNet.Numerics.Distributions;
using System.Collections.ObjectModel;
using System.ComponentModel;

using MathNet.Numerics.LinearAlgebra;
using Filter.Utility;

namespace GaussianProcess.Wrap
{
    public class Wrapper
    {



        IKernel _kernel;

        Tuple<MathNet.Numerics.LinearAlgebra.Vector<double>, MathNet.Numerics.LinearAlgebra.Matrix<double>> mC;

        MathNet.Numerics.LinearAlgebra.Vector<double> t;

        List<double> xpts;

        public double[] axpts;
        GaussianProcess.Process gp;

        double _scale;

        Random rand = new Random();

        DateTime dt = new DateTime(2000, 12, 12);



        public Wrapper(List<Tuple<double, double>> points = null, double scale = 100, IKernel kernel = null)
        {
            _scale = scale;
            // kernel = OrnsteinKernel(1.0)
            _kernel = kernel ?? new Kernel(2.0, 60.0, 0.0, 0.0);

            if (points == null)
            {
                axpts = new double[40];
                Normal.Samples(rand, axpts, 0, 2);
            }
            else
            {
                axpts = points.Select(_ => _.Item2).ToArray();
                t = Vector<double>.Build.DenseOfArray(points.Select(_ => _.Item1).ToArray());
                AddMeasurements();
            }

            Initialise();

        }


        private void Initialise()
        {
            gp = new GaussianProcess.Process();

            //# In the context of Gaussian Processes training means simply
            //# constructing the kernel (or Gram) matrix.
            mC = gp.Train(axpts, _kernel);




        }



        public List<Measurement> Sample()
        {

            // # Now we draw from the distribution to sample from the gaussian prior.
            t = gp.draw_multivariate_gaussian(mC.Item1, mC.Item2);

            return MakePredictions();

        }


        public List<Measurement> MakePredictions()
        {

            return MakeRange(_scale).Select(p =>
            {

                //double p = point / 100;
                var prediction = gp.predict(p, axpts, _kernel, mC.Item2, t);

                return new Measurement(
                    value: prediction.Item1,
                    variance: prediction.Item2,
                    time: dt + TimeSpan.FromSeconds(p)
                );

            }).ToList();

        }



        public List<Measurement> AddMeasurements()
        {
            return axpts.Select((_, k) =>

               new Measurement
                (
                    value: t[k],
                    time: dt + TimeSpan.FromSeconds(_)
                )

            ).ToList();


        }


        public void Add(double d)
        {

            //gp = new GaussianProcess.Process();
            //Some sample training points.
            xpts = axpts.ToList();

            //xpts.Add(Normal.Sample(rand, 0, 1));
            xpts.Add(d);
            axpts = xpts.ToArray();


            Initialise();

            MakePredictions();



        }


        public void AddRandomPoint()
        {

            //gp = new GaussianProcess.Process();
            //Some sample training points.
            xpts = axpts.ToList();

            xpts.Add(Normal.Sample(rand, 0, 1));

            axpts = xpts.ToArray();



            Initialise();

            MakePredictions();



        }






        private IEnumerable<double> MakeRange(double scale)
        {


            var min = axpts.Min() * scale;

            var max = axpts.Max() * scale;


            var range = Enumerable.Range((int)min, (int)max - (int)min).Select(_ => ((double)_ / scale));

            return range;
        }





        //public void Run2()
        //{
        //    var gp = new GaussianProcess.Process2();

        //    List<double> x = new List<double>(new double[] { 0.0 });
        //    List<double> y = new List<double>(new double[] { 0.0 });



        //    var x_more = new double[] { -2.1, -1.5, 0.3, 1.8, 2.5 };
        //    var y_more = new List<double>();

        //    Random rand = new Random();

        //    IKernel kernel = new Kernel(1.0, 10.0, 0.0, 0.0);

        //    //double[] xpts = new double[10];
        //    //Normal.Samples(xpts, -1, 2.0);
        //    //List<Tuple<double, double>> mus = new List<Tuple<double, double>>();

        //    foreach (var xm in x_more)
        //    {
        //        var mus = gp.conditional(xm, x.ToArray(), y.ToArray());

        //        y_more.Add(Normal.Sample(rand, mus.Item1, mus.Item2));
        //    }



        //    var σ_new = gp.exponential_cov(x_more.ToArray(), x_more.ToArray());


        //    var range = Enumerable.Range(-300, 600).Select(_ => ((double)_) / 100);


        //    //var predictions = new List<Tuple<double, double>>();
        //    DateTime dt = new DateTime();

        //    foreach (var i in range)
        //    {
        //        var prediction = gp.predict(i, x_more, σ_new, y_more.ToArray());


        //        Estimates.Add(new Measurement
        //        (
        //            value: prediction.Item1,
        //            variance: prediction.Item2,
        //            time: dt + TimeSpan.FromSeconds(i)
        //        ));





        //    }

        //    int k = 0;
        //    foreach (var x_ in x_more)
        //    {
        //        Measurements.Add(new Measurement
        //        (
        //            value: y_more[k],
        //            time: dt + TimeSpan.FromSeconds(x_)
        //        ));
        //        k++;
        //    }








        //}


    }
}


