using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterSharp.Common
{
    public class OxyPlotModelFactory
    {

        public static OxyPlot.PlotModel CreatePlotModel(IList<KeyValuePair<DateTime, Tuple<double, double>[]>> u)
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





        public static OxyPlot.PlotModel CreatePlotModel(IEnumerable<KeyValuePair<DateTime, (double, double)[]>> u, int i)
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


    }
}
