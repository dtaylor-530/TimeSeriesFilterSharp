using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityHelper;


namespace Error
{
    public class Calculator
    {

        public static double GetErrorSum(IEnumerable<double> est, IEnumerable<double> meas, Residual lf)
        {

            var lossFunction = Helper.ChooseLossFunction(lf);

            return est.Zip(meas, (a, b) => new { a, b }).Aggregate(0d, (e, f) => e + lossFunction(f.a, f.b));


        }

        public static IEnumerable<double> GetError(IEnumerable<double> est, IEnumerable<double> meas, Residual lf)
        {

            var lossFunction = Helper.ChooseLossFunction(lf);

            return est.Zip(meas, (a, b) => new { a, b }).Scan(0d, (e, f) => e + lossFunction(f.a, f.b));


        }

        public static double GetErrorSum<T, R>(IEnumerable<T> est, IEnumerable<R> meas, Func<T, double> estf, Func<R, double> measf, Residual lf)
        {
            var lossFunction = Helper.ChooseLossFunction(lf);

            return est.Zip(meas, (a, b) => new { a, b }).Aggregate(0d, (e, f) => e + lossFunction(estf(f.a), measf(f.b)));

        }

        public static IEnumerable<double> GetError<T, R>(IEnumerable<T> est, IEnumerable<R> meas, Func<T, double> estf, Func<R, double> measf, Residual lf)
        {
            var lossFunction = Helper.ChooseLossFunction(lf);

            return est.Zip(meas, (a, b) => new { a, b }).Scan(0d, (e, f) => e + lossFunction(estf(f.a), measf(f.b)));

        }


        //public static double GetErrorSum<T, R>(IObservable<T> est, IObservable<R> meas, Func<T, double> estf, Func<R, double> measf, Residual lf)
        //{
        //    var lossFunction = Helper.ChooseLossFunction(lf);

        //    return est.Zip(meas, (a, b) => new { a, b }).Aggregate(0d, (e, f) => e + lossFunction(estf(f.a), measf(f.b)));

        //}
    }
}
