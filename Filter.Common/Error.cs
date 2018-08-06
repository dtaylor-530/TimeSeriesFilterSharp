using Filter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter.Common
{
    public static class ErrorHelper
    {

        public static IEnumerable<KeyValuePair<DateTime,double>> GetError(IEnumerable<KeyValuePair<DateTime, double>> a, IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> b)
        {
            return Error.Calculator.GetError(a, b, c => c.Value, d => d.Value[0].Item1,Error.Residual.RMSE).Zip(a,(c,d)=>new KeyValuePair<DateTime, double>(d.Key,c));

        }
        public static double GetErrorSum(IEnumerable<KeyValuePair<DateTime, double>> a, IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> b)
        {
            return Error.Calculator.GetErrorSum(a, b, c => c.Value, d => d.Value[0].Item1, Error.Residual.RMSE);

        }

        //public static double GetErrorSum(IObservable<KeyValuePair<DateTime, double>> a, IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>> b)
        //{
        //    return Error.Calculator.GetErrorSum(a, b, c => c.Value, d => d.Value[0].Item1, Error.Residual.RMSE);

        //}


        //public static Func<double, double, IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>> GetFunction(Type tp, IEnumerable<KeyValuePair<DateTime, double>> ss)
        //{

        //    return (a, b) => GetInstance<IFilterWrapper>(tp, a, b).BatchRun(ss);

        //}



    }


}
