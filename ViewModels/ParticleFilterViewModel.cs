using Filter.Utility;
using MathNet.Numerics.LinearAlgebra;
using ParticleFilter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Filter.ViewModel
{
    public class ParticleFilterViewModel : INPCBase
    {




        public ObservableCollection<MeasurementsEstimates> Mes { get; set; } = new ObservableCollection<MeasurementsEstimates>();


        public double MeanSquaredError { get; set; }

        //public double[] q { get; set; }


        //public double[] r { get; set; }




        int cnt = 0;
        private ParticleFilter.Wrap.Wrapper _filter;



        public ParticleFilterViewModel(int particleCount, Tuple<int, int> x, Tuple<int, int> y)
        {


        }


        public void BatchRun(List<Tuple<DateTime, Point>> meas = null, int particleCount = 100, Tuple<int, int> range = null)
        {

            meas = meas ?? ProcessFactory.SineWave(0, 100, false, 0.2).ToPointDateTimes();


            _filter = new ParticleFilter.Wrap.Wrapper();


            var u = _filter.BatchRun1D(meas.ToList(), range?.Item1 ?? -10, range?.Item2 ?? 10);



            //estimates.Add(new KeyValuePair<DateTime, Tuple<Normal, Normal>>(dt, est));

            for (int i = 0; i < 1; i++)
            {
                var est = u.Select(_ => new { a = _.Key, b = ParticleFilter.Wrap.Wrapper.ToWeightedEstimate(_.Value) });

                if (Mes.Count() < i + 1)
                    Mes.Add(new MeasurementsEstimates());

                //Mes[i].PlotModel = CreatePlotModel(u);
                Mes[i].Estimates = new ObservableCollection<Measurement>(est.Select(_ => new Measurement(_.a, _.b.Item1, _.b.Item2)));
                Mes[i].Measurements = new ObservableCollection<Measurement>(meas.Select((_) => new Measurement(_.Item1, _.Item2.Y)));


            }

            NotifyChanged(nameof(Mes), nameof(MeanSquaredError));



        }


        public OxyPlot.PlotModel CreatePlotModel(IList<KeyValuePair<DateTime, List<Tuple<double, double>>>> u)
        {
            var pm = new OxyPlot.PlotModel();
            var scatterSeries = new OxyPlot.Series.ScatterSeries { MarkerType = OxyPlot.MarkerType.Circle };
            var first = u.First().Key;
            scatterSeries.Points.AddRange(u.SelectMany(_ =>
              {
                  return _.Value.Select(__ =>
                {
                    var size = 2;
                    var colorValue = __.Item2;
                    return new OxyPlot.Series.ScatterPoint((_.Key - first).TotalDays, __.Item1, size, colorValue);
                });

              }));

            //scatterSeries.Points=cx;

            pm.Series.Add(scatterSeries);

            return pm;

        }




        //            for (int i = 0; i< 2; i++)
        //            {
        //                var est = u.Select(_ => new { a = _.Key, b = ParticleFilter.Wrap.Wrapper.ToWeightedEstimate(_.Value[i]) });

        //                if (Mes.Count() < i + 1)
        //                    Mes.Add(new MeasurementsEstimates());

        //             //   Mes[i].PlotModel = CreatePlotModel(u, i);
        //                Mes[i].Estimates = new ObservableCollection<Measurement>(est.Select(_ => new Measurement(_.a, _.b.Item1, _.b.Item2)));

        //                if (i==0)
        //                Mes[i].Measurements = new ObservableCollection<Measurement>(meas.Select((_) => new Measurement(_.Item1, _.Item2.Y)));
        //                //Mes[i].n();

        //            }

        //            NotifyChanged(nameof(Mes), nameof(MeanSquaredError));



        //}




        public OxyPlot.PlotModel CreatePlotModel(IEnumerable<KeyValuePair<DateTime, List<List<(double, double)>>>> u, int i)
        {
            var pm = new OxyPlot.PlotModel();
            var scatterSeries = new OxyPlot.Series.ScatterSeries { MarkerType = OxyPlot.MarkerType.Circle };
            var first = u.First().Key;
            scatterSeries.Points.AddRange(u.SelectMany(_ =>
            {
                return _.Value.Select(__ =>
                {
                    var size = 2;
                    var colorValue = __[i].Item2;
                    return new OxyPlot.Series.ScatterPoint((_.Key - first).TotalDays, __[i].Item1, size, colorValue);
                });
            }));

            //scatterSeries.Points=cx;

            pm.Series.Add(scatterSeries);

            return pm;

        }



    }

}
