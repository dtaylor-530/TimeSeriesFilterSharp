using FilterSharp.Common;
using FilterSharp.Model;
using GaussianProcess;
using GaussianProcess.Wrap;
using MathNet.Numerics.Distributions;
using PropertyTools.Wpf;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Input;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit
    { }
}

namespace FilterSharp.ViewModel
{
    public class GaussianProcessViewModel
    {
        public GaussianProcessViewModel()
        {
            ActionViewModel.Subscribe(ResultsViewModel);
            ConfigurationViewModel.Subscribe(ResultsViewModel);
            ResultsViewModel.Subscribe(ActionViewModel);
        }

        public GaussianProcessActionViewModel ActionViewModel { get; } = new();
        public GaussianProcessResultsViewModel ResultsViewModel { get; } = new();
        public GaussianProcessConfigurationViewModel ConfigurationViewModel { get; } = new();

        public ICollection Children => new object[] { ActionViewModel, ConfigurationViewModel, ResultsViewModel };
    }

    public record GaussianProcessConfiguration(Type Kernel, double Length, double Noise);

    public class GaussianProcessConfigurationViewModel : PredictionViewModel, IObservable<GaussianProcessConfiguration>
    {
        private Type kernel = typeof(RationalQuadratic);
        private double length = 3;
        private double noise = 3;

        public Dictionary<string, Type> Kernels { get; } = KernelHelper.LoadKernels().ToDictionary(a => a.GetDescription());

        public Type Kernel
        {
            get { return kernel; }
            set { this.RaiseAndSetIfChanged(ref kernel, value); }
        }

        public double Length
        {
            get { return length; }
            set { this.RaiseAndSetIfChanged(ref length, value); }
        }

        public double Noise
        {
            get { return noise; }
            set { this.RaiseAndSetIfChanged(ref noise, value); }
        }

        public IDisposable Subscribe(IObserver<GaussianProcessConfiguration> observer)
        {
            return this.WhenAnyValue(a => a.Kernel, a => a.Length, a => a.Noise)
                .Select(a => new GaussianProcessConfiguration(a.Item1, a.Item2, a.Item3))
                .Subscribe(a =>
                {
                    observer.OnNext(a);
                });
        }
    }

    public class GaussianProcessActionViewModel : ReactiveObject, IObservable<GaussianProcessAction>, IObserver<Exception?>
    {
        private readonly ReplaySubject<GaussianProcessAction> subject = new(1);
        private string result;

        public GaussianProcessActionViewModel()
        {
            Add = new DelegateCommand(() => subject.OnNext(GaussianProcessAction.Add));
            Run = new DelegateCommand(() => subject.OnNext(GaussianProcessAction.Run));
        }

        public ICommand Run { get; }
        public ICommand Add { get; }

        public string Result
        {
            get { return result; }
            set { this.RaiseAndSetIfChanged(ref result, value); }
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Exception? value)
        {
            if (value is { Message: var message })
                Result = message;
            else
                Result = "SUCCESS";
        }

        public IDisposable Subscribe(IObserver<GaussianProcessAction> observer)
        {
            return subject.Subscribe(observer);
        }
    }

    public enum GaussianProcessAction
    {
        Add, Run
    }

    public class GaussianProcessResultsViewModel : PredictionViewModel, IObserver<GaussianProcessConfiguration>, IObserver<GaussianProcessAction>, IObservable<Exception>
    {
        private DateTime dt = new(2000, 1, 1);
        private DateTime odt = new(2000, 1, 1);
        private GPMultiModelold multiModel;
        //private GaussianProcessConfiguration value;

        public DateTime Date
        {
            get { return dt; }
            set { this.RaiseAndSetIfChanged(ref dt, value); }
        }

        public GPMultiModelold MultiModel
        {
            get { return multiModel; }
            set { this.RaiseAndSetIfChanged(ref multiModel, value); }
        }

        public ObservableCollection<GP> Kernels { get; } = new();

        //public void Sample()
        //{
        //    //Estimates.Clear();
        //    //gpw.Sample().ForEach(_ => Estimates.Add(_));
        //}

        public Exception? Add(int cnt = 1)
        {
            try
            {
                foreach (var kvp in SignalGenerator.GetRandom(dt, cnt))
                {
                    Measurements.Add(kvp);
                    Date = kvp.Key;
                }

                Run();
            }
            catch (Exception ex)
            {
                return ex;
            }
            return null;
        }

        private Exception? Run()
        {
            if (Measurements.Count == 0)
            {
            }

            try
            {
                List<(double, double)> spoints = Measurements.Select(_ => ((double)(_.Key - odt).TotalSeconds, _.Value)).ToList();

                var outgps = multiModel.AddTrainingPoints(spoints.Select(_ => _.Item1).ToArray(), spoints.Select(_ => _.Item2).ToArray());

                var xx = outgps.SelectMany(a => a.mu
                  .Zip(a.sd95, (a, b) => new { Mu = a, Sigma = b })
                  .Zip(a.X, (c, d) =>
                   new Estimate(odt + TimeSpan.FromSeconds(d), c.Mu, c.Sigma)));

                AddToEstimates(xx);
            }
            catch (Exception ex)
            {
                return ex;
            }
            return null;
        }

        private async void AddToEstimates(IEnumerable<Estimate> estimates)
        {
            Estimates.Clear();
            foreach (var est in estimates)
            {
                Estimates.Add(est);
                await Task.Delay(5);
            }
        }

        private ReplaySubject<Exception?> subject = new(1);

        public void OnNext(GaussianProcessAction value)
        {
            switch (value)
            {
                case GaussianProcessAction.Add:
                    subject.OnNext(Add(20));
                    break;

                case GaussianProcessAction.Run:
                    subject.OnNext(Run());
                    break;

                default:
                    throw new Exception("fgdsf3 dfgfgs");
            }
        }

        public void OnNext(GaussianProcessConfiguration value)
        {
            KernelChanged(value.Kernel, value.Length, value.Noise);

            void KernelChanged(Type type, double length, double noise)
            {
                Kernels.Clear();
                Kernels.Add(new GP((IMatrixkernel)Activator.CreateInstance(type), length, noise));
                multiModel = new GPMultiModelold(Kernels.ToArray());

                if (Measurements.Count > 0)
                    Run();
            }
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(IObserver<Exception> observer)
        {
            return subject.Subscribe(observer);
        }
    }

    public class GPViewModel2 : PredictionViewModel
    {
        [PropertyTools.DataAnnotations.Browsable(false)]
        public Wrapper gpw { get; private set; }

        private double[] axpts;

        private DateTime dt = new DateTime(2000, 1, 1);
        private Random rand = new Random();

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