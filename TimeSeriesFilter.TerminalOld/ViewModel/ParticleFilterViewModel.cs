using Filter.Common;
using Filter.Model;
using Filter.Service;

using MathNet.Numerics.LinearAlgebra;
using ParticleFilter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TimeSeries.Service;

namespace Filter.ViewModel
{
    public class ParticleFilterViewModel : PredictionViewModel
    {


        public ObservableCollection<Estimate> EstimatesAll { get; private set; }

     
        private ParticleFilter.Wrap.ParticleFilterWrapper _filter;

        public ParticleFilterViewModel(int particleCount, Tuple<int, int> x, Tuple<int, int> y)
        {


        }

        public void BatchRun(IEnumerable<KeyValuePair<DateTime, Point>> meas , int particleCount = 100, Tuple<int, int> range = null)
        {

            _filter = new ParticleFilter.Wrap.ParticleFilterWrapper();

            var u = _filter.BatchRun1D(meas);
            EstimatesAll=new ObservableCollection<Estimate>(u.SelectMany(_ => _.Value.OrderBy(s=>1d/s.Item2).Take(10).Select(__=>new Estimate(_.Key, __.Item1, __.Item2))));
            Estimates = new ObservableCollection<Estimate>(u.Select(_ => new Estimate(_.Key, _.Value.WeightedAverage(a=>a.Item1,a=>a.Item2), _.Value[0].Item2)));
             NotifyChanged(nameof(Estimates),nameof(EstimatesAll));
            
            Measurements = new ObservableCollection<KeyValuePair<DateTime, double>>(meas.Select(_=>new KeyValuePair<DateTime, double>(_.Key,_.Value.Y)));

            NotifyChanged(nameof(Measurements), nameof(Estimates));

           
        }




    }


    public static class StatisticalHelper
    {


        public static double WeightedAverage<T>(this IEnumerable<T> records, Func<T, double> value, Func<T, double> weight, double control = 0)
        {
            double weightedValueSum = records.Sum(x => (value(x) - control) * weight(x));
            double weightSum = records.Sum(x => weight(x));

            if (weightSum != 0)
                return weightedValueSum / weightSum;
            else
                throw new DivideByZeroException("Divide by zero exception calculating weighted average");
        }
    }

    

}
