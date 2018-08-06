using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using System.Reactive.Concurrency;
using Filter.Model;
using System.Reactive.Linq;

namespace Filter.Service
{

    public class TimeValueServiceFactory
    {

        public static IObservable<KeyValuePair<DateTime, double?>> MakeMeasurementUnknownServiceDefault(int count, IScheduler scheduler)
        {

            INoisyKernel kernel = Filter.Model.KernelFactory.MakeKernelRandom();
            long start = DateTime.Now.Ticks;
            var xpts = Enumerable.Range(0, 60).Select(_ => DateTime.Now.Ticks + _*100000);
            var measurements = TimeValueServices.GetTimeStampedObservable(xpts,(i) => kernel.Evaluate((int)(i - start)))
            .Select(_ => new KeyValuePair<DateTime,double?>(_.Key, (double?)_.Value));
            var unknowns = TimeValueServices. GetTimeStampedObservable(xpts);
            //unknowns.Subscribe(_ => Console.WriteLine(_.Second));
            return TimeValueServices.CombineServices<double?>(measurements, unknowns, scheduler);

        }




        public static IObservable<KeyValuePair<DateTime, double>> MakeMeasurementServiceDefault(int count, IScheduler scheduler)
        {

            INoisyKernel kernel = Filter.Model.KernelFactory.MakeKernelRandom();
            long start = DateTime.Now.Ticks;
            var xpts = Enumerable.Range(0, count).Select(_ => DateTime.Now.Ticks + _ * 100000);
            return TimeValueServices.GetTimeStampedObservable(xpts,(i) => kernel.Evaluate((int)(i - start)))
                .Select(_ => new KeyValuePair<DateTime, double>(_.Key, _.Value))
                .SubscribeOn(scheduler);




        }

    }
}
