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
using GaussianProcess.Wrap;

using System.IO;


namespace Filter.ViewModel
{
    public class GPViewModel : INPCBase
    {
        [PropertyTools.DataAnnotations.Browsable(false)]
        public ObservableCollection<Measurement> Measurements { get; set; } = new ObservableCollection<Measurement>();
        [PropertyTools.DataAnnotations.Browsable(false)]
        public ObservableCollection<Measurement> Estimates { get; set; } = new ObservableCollection<Measurement>();


        public Wrapper gpw { get; private set; }

        List<double> xpts;

        public double[] axpts;

        DateTime dt = new DateTime(2000, 1, 1);
        Random rand = new Random();

        GaussianProcess.GPModel gpwm = new GaussianProcess.GPModel();

        public GPViewModel(List<Tuple<double,double>> points = null, double scale = 100, IKernel kernel = null)
        {
        
            if (points == null)
            {
                axpts = new double[40];
                Normal.Samples(rand, axpts, 0, 2);
            }
            else
            {
                axpts = points.Select(_=>_.Item2).ToArray();
            
       
            }


            gpw = new GaussianProcess.Wrap.Wrapper(points, scale, kernel); // Process();
        }



        public void Sample()
        {


            Estimates.Clear();
            gpw.Sample().ForEach(_ => Estimates.Add(_));

        }



        public void Run(double[] xpts,double[] ypts)
        {

            Estimates.Clear();
            Measurements.Clear();


            var points = xpts.Zip(ypts, (a, b) => Tuple.Create(a, b));

            foreach (var xy in points )
                Measurements.Add(new Measurement(dt + TimeSpan.FromSeconds(xy.Item1), xy.Item2));

            var outgps = gpwm.AddTrainingPoints(points.Select(_ => _.Item1).ToArray(), points.Select(_ => _.Item2).ToArray());

            var xx = outgps.Select(_ => _.mu
              .Zip(_.sd95, (a, b) => new { Mu = a, Sigma = b })
              .Zip(gpwm.tePointsX, (c, d) =>
               new Measurement(dt + TimeSpan.FromSeconds(d), c.Mu, c.Sigma)));

            foreach (var _ in xx.First())
                Estimates.Add(_);

          
        }



        public void AddMeasurements()
        {
            Measurements.Clear();

            int k = 0;
            //foreach (var x_ in axpts)
            //{
            //    Measurements.Add(new Measurement
            //    (
            //        value: t[k],
            //        time: dt + TimeSpan.FromSeconds(x_)
            //    ));
            //    k++;
            //}


        }


        public void Add( double d)
        {

            //gp = new GaussianProcess.Process();
            //Some sample training points.
            xpts = axpts.ToList();

            //xpts.Add(Normal.Sample(rand, 0, 1));
            xpts.Add(d);
            axpts = xpts.ToArray();



            Estimates.Clear();
            gpw.MakePredictions().ForEach(_ => Estimates.Add(_));


        }


        public void AddRandomPoint()
        {

            //gp = new GaussianProcess.Process();
            //Some sample training points.

            var randomPoint = Normal.Sample(rand, 0, 1);


            Add(randomPoint);

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
