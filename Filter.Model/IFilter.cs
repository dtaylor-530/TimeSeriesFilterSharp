using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter.Model
{
    public interface IFilterWrapper
    {
        IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> BatchRun(IEnumerable<KeyValuePair<DateTime, double>> values);
        IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>> Run(IObservable<KeyValuePair<DateTime, double?>> values);
    }
}
