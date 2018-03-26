
using Filter.Utility;
using KalmanFilter.Wrap;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter.ViewModel
{



    public class KalmanFilterViewModel : INPCBase
    {


        public ObservableCollection<MeasurementsEstimates> Mes { get; set; } = new ObservableCollection<MeasurementsEstimates>();


        public double MeanSquaredError { get; set; }

        public double[] q { get; set; } = new double[] {10, 1000 };
    


        public double[] r { get; set; } = new double[] { 1 };

        private KalmanFilter.Wrap.DiscreteWrapper kf;


        private IList<KeyValuePair<DateTime, Tuple<Matrix<double>, Matrix<double>>>> u;




        int cnt = 0;





        public void Run()
        {
            var meas = ProcessFactory.SineWave(3.2, 200, true).ToMatrices();


            kf = new KalmanFilter.Wrap.DiscreteWrapper(r, q);

            //kf.AdaptiveQ = new AEKFKFQ(q);
            //kf.AdaptiveR = new AEKFKFR(r);



            var u = kf.BatchRun(meas.ToList());



            int k = (new int[] { u.First().Value.Item1.RowCount, meas.First().Item2.RowCount }).Max();
            for (int i = 0; i < k; i++)
            {
                if (Mes.Count() < i + 1)
                    Mes.Add(new MeasurementsEstimates());

                if (u.First().Value.Item1.RowCount > i)
                    Mes[i].Estimates = new ObservableCollection<Measurement>(u.Select(_ => new Measurement(_.Key, _.Value.Item1[i, 0], _.Value.Item2[i, i])));

                if (meas.First().Item2.RowCount > i)
                    Mes[i].Measurements = new ObservableCollection<Measurement>(meas.Select((_) => new Measurement(_.Item1, _.Item2[i, 0])));


            }

            NotifyChanged(nameof(Mes), nameof(MeanSquaredError));



        }


        public void RunRecursive()
        {


            var meas = ProcessFactory.SineWave(2.2, 200, true,30).ToMatrices();


            kf = new KalmanFilter.Wrap.DiscreteWrapper(r, q);

            kf.AdaptiveQ = new AEKFKFQ(q);
            kf.AdaptiveR = new AEKFKFR(r);



            var u = kf.BatchRunRecursive(meas.ToList());



            int k = (new int[] { u.First().Value.Item1.RowCount, meas.First().Item2.RowCount }).Max();
            for (int i = 0; i < k; i++)
            {
                if (Mes.Count() < i + 1)
                    Mes.Add(new MeasurementsEstimates());

                if (u.First().Value.Item1.RowCount > i)
                    Mes[i].Estimates = new ObservableCollection<Measurement>(u.Select(_ => new Measurement(_.Key, _.Value.Item1[i, 0], _.Value.Item2[i, i])));

                if (meas.First().Item2.RowCount > i)
                    Mes[i].Measurements = new ObservableCollection<Measurement>(meas.Select((_) => new Measurement(_.Item1, _.Item2[i, 0])));


            }

            NotifyChanged(nameof(Mes), nameof(MeanSquaredError));


        }








        public void Smooth()
        {

            var Q = Matrix<double>.Build.DenseDiagonal(q.Length, q.Length, 0.1d);


            int k = u.First().Value.Item1.RowCount;
            for (int i = 0; i < k; i++)
            {

                var us = KalmanFilter.Smoother.Smooth(u, KalmanFilter.StateFunctions.BuildTransition(k), Q);


                Mes[i].SmoothedEstimates = new ObservableCollection<Measurement>(us.Select(_ =>
                  new Measurement(_.Key, _.Value.Item1[i, 0], _.Value.Item2[i, i])));
                Mes[i].n();


            }

            NotifyChanged(nameof(Mes), nameof(MeanSquaredError));



        }








        public async void RunDelayed(List<Tuple<DateTime, Double[]>> meas = null, bool adapt = false)
        {
            meas = meas ?? ProcessFactory.SineWave(1.2, 200, true,0).ToArrays();

            IDictionary<DateTime, Tuple<Matrix<double>, Matrix<double>>> dict = new Dictionary<DateTime, Tuple<Matrix<double>, Matrix<double>>>();

            kf = new KalmanFilter.Wrap.DiscreteWrapper(r, q);

            if (adapt)
            {
                kf.AdaptiveQ = new AEKFKFQ(q);
                kf.AdaptiveR = new AEKFKFR(r);
            }



            var Q = Matrix<double>.Build.DenseDiagonal(q.Length, q.Length, 1);

            var ph = new Progress<KeyValuePair<DateTime, Tuple<Matrix<double>, Matrix<double>>>>((us) =>
            {
                int k = (new int[] { us.Value.Item1.RowCount, meas.First().Item2.Length }).Max();

                dict.Add(us);


                var uss = KalmanFilter.Smoother.Smooth(dict, KalmanFilter.StateFunctions.BuildTransition(us.Value.Item1.RowCount), Q);


                for (int i = 0; i < k; i++)
                {
                    if (Mes.Count() < i + 1)
                        Mes.Add(new MeasurementsEstimates());


                    if (meas.First().Item2.Length > i)
                    {
                        Mes[i].Measurements = Mes[i].Measurements ?? new ObservableCollection<Measurement>();
                        Mes[i].Measurements.Add(new Measurement(meas[cnt].Item1, meas[cnt].Item2[i]));
                    }

                    if (us.Value.Item1.RowCount > i)
                    {

                        Mes[i].Estimates = Mes[i].Estimates ?? new ObservableCollection<Measurement>();
                        Mes[i].Estimates.Add(new Measurement(us.Key, us.Value.Item1[i, 0], us.Value.Item2[i, i]));

                        Mes[i].SmoothedEstimates = Mes[i].SmoothedEstimates ?? new ObservableCollection<Measurement>();
                        Mes[i].SmoothedEstimates.Add(
                              new Measurement(uss.Last().Key, uss.Last().Value.Item1[i, 0], uss.Last().Value.Item2[i, i]));
                        Mes[i].n();
                    }



                }
                cnt++;

                Quanitfier.pcalc(Mes[0].Measurements, Mes[1].Estimates);

                NotifyChanged(nameof(Mes), nameof(MeanSquaredError));

            }

            );

            /* u =*/
            await kf.BatchRunAsync(meas, ph, 50);




        }



        public void RunEnsemble()
        {
            var meas = ProcessFactory.SineWave(1.2, 200, true,0);

            var mw = new KalmanFilter.Wrap.MultiWrapper();

           var u= mw.Run(meas);

            for (int i = 0; i < u.First().Item2.Item1.Count(); i++)
            {
                if (Mes.Count() < i + 1)
                    Mes.Add(new MeasurementsEstimates());

                if (u.First().Item2.Item1.Count() > i)
                    Mes[i].Estimates = new ObservableCollection<Measurement>(u.Select(_ => new Measurement(_.Item1, _.Item2.Item1[i], _.Item2.Item2[i])));

                if(i==0)
                    Mes[i].Measurements = new ObservableCollection<Measurement>(meas.Select((_) => new Measurement(_.Item1, _.Item2)));


            }

            NotifyChanged(nameof(Mes), nameof(MeanSquaredError));



        }






    }













}
