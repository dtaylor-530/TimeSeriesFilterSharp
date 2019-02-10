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

using FilterSharp.Model;
using GaussianProcess.Wrap;

using System.IO;
using System.Windows;
using System.Reflection;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Windows.Threading;
using ReactiveUI;
using FilterSharp.Common;

namespace FilterSharp.ViewModel
{

    
    public class GPViewModelStatic : PredictionViewModel
    {
        private Type kernel = typeof(GaussianProcess.RationalQuadratic);
        private double length = 3;
        private double noise = 3;
        DateTime dt = new DateTime(2000, 1, 1);
        DateTime odt = new DateTime(2000, 1, 1);
        List<GP> gps = new List<GP>();
        GaussianProcess.Wrap.GPMultiModelold gpmm;



        public Dictionary<string, Type> Kernels { get; set; }
        
        public Type Kernel
        {
            get { return kernel; }
            set { if (kernel != value) { kernel = value; KernelChanged(value, Length, Noise); } }
        }


        public double Length
        {
            get { return length; }
            set { if (length != value) { this.RaiseAndSetIfChanged(ref length, value);  KernelChanged(Kernel, value, Noise); } }
        }

        public double Noise
        {
            get { return noise; }
            set { if (noise != value) { this.RaiseAndSetIfChanged(ref noise, value); KernelChanged(Kernel, Length, value); } }
        }




        public GPViewModelStatic()
        {

            Kernels = GaussianProcess.KernelHelper.LoadKernels().ToDictionary(_ => _.GetDescription());

            KernelChanged(Kernels.First().Value, Length, Noise);

        }



        private void KernelChanged(Type type, double length, double noise)
        {

            gps.Clear();
            gps.Add(new GP((IMatrixkernel)Activator.CreateInstance(type), length, noise));
            gpmm = new GaussianProcess.Wrap.GPMultiModelold(gps.ToArray());

            if (Measurements.Count > 0) Run();

        }


        public void Sample()
        {

            //Estimates.Clear();
            //gpw.Sample().ForEach(_ => Estimates.Add(_));

        }



        public void Add(int cnt = 1)
        {

            foreach (var kvp in SignalGenerator.GetRandom(dt, cnt))
            {
                Measurements.Add(kvp);
                dt = kvp.Key;
            }

            Run();
        }



        private void Run()
        {

            List<(double, double)> spoints = Measurements.Select(_ => ((double)(_.Key - odt).TotalSeconds, _.Value)).ToList();

            var outgps = gpmm.AddTrainingPoints(spoints.Select(_ => _.Item1).ToArray(), spoints.Select(_ => _.Item2).ToArray());

            var xx = outgps.Select(_ => _.mu
              .Zip(_.sd95, (a, b) => new { Mu = a, Sigma = b })
              .Zip(_.X, (c, d) =>
               new Estimate(odt + TimeSpan.FromSeconds(d), c.Mu, c.Sigma)));
            Estimates.Clear();
            foreach (var __ in xx)
                foreach (var ___ in __)
                    Estimates.Add(___);

        }

    }








    public class GPViewModel2 : PredictionViewModel
    {

        [PropertyTools.DataAnnotations.Browsable(false)]
        public Wrapper gpw { get; private set; }

        double[] axpts;

        DateTime dt = new DateTime(2000, 1, 1);
        Random rand = new Random();

        public GPViewModel2(List<Tuple<double, double>> points = null, double scale = 100, IKernel kernel = null)
        {
            if (points == null)
            {
                axpts = new double[40];
                Normal.Samples(rand, axpts, 0, 2);
            }
            else
                axpts = points.Select(_ => _.Item2).ToArray();

            gpw = new GaussianProcess.Wrap.Wrapper(points, scale, kernel); // Process();
        }

        public void Sample()
        {

            Estimates.Clear();
            gpw.Sample().ForEach(_ => Estimates.Add(_));

        }


    }







}



