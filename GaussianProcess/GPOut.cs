using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaussianProcess
{

    public class GPOut
    {


        public Matrix<double> proj { get; set; }
        public Vector<double> mu { get; set; }
        public Vector<double> sd95 { get; set; }
        public double[] X { get; set; }
    }



    public static class GPOutEx
    {

        
        public static IEnumerable<KeyValuePair<DateTime, Tuple<double, double>>> GetPositions(this GPOut gpout, DateTime dt)
        {

            using (var i1 = gpout.X.ToList().GetEnumerator())
            using (var i2 = gpout.mu.ToList().GetEnumerator())
            using (var i3 = gpout.sd95.ToList().GetEnumerator())
            {

                while (i1.MoveNext() && i2.MoveNext() && i3.MoveNext())
                {
                    yield return new KeyValuePair<DateTime, Tuple<double, double>>(dt + TimeSpan.FromSeconds(i1.Current), Tuple.Create(i2.Current, i3.Current));
                }

            }
        }

        public static IEnumerable<KeyValuePair<DateTime, Tuple<double, double>>> GetVelocities(this GPOut gpout, DateTime dt)
        {
            double lastposition = 0;
            double lasttime = 0;
            double lastvariance = 0;
            using (var i1 = gpout.X.ToList().GetEnumerator())
            using (var i2 = gpout.mu.ToList().GetEnumerator())
            using (var i3 = gpout.sd95.ToList().GetEnumerator())
            {

                lastposition = i2.Current;
                lasttime = i1.Current;
                lastvariance = i3.Current;
                while (i1.MoveNext() && i2.MoveNext() && i3.MoveNext())
                {
                    var velocity = (i2.Current - lastposition) / TimeSpan.FromDays(i1.Current - lasttime).TotalSeconds;

                    yield return new KeyValuePair<DateTime, Tuple<double, double>>(dt + TimeSpan.FromSeconds(i1.Current), Tuple.Create(i2.Current, i3.Current));
                    lastposition = i2.Current;
                    lasttime = i1.Current;
                    lastvariance = i3.Current;
                }
            }
        }


        public static IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> GetPositionsAndVelocities(this GPOut gpout,DateTime start,double control)
        {

            double lasttime = 0;
            double lastvariance;
            double lastposition;
            double velocity = 0;
    
            using (IEnumerator<double> i2 = gpout.mu.ToList().GetEnumerator())
            //using (IEnumerator<double> i1 = gpout.X.ToList().GetEnumerator())
            using (IEnumerator<double> i3 = gpout.sd95.ToList().GetEnumerator())
                foreach (double time in gpout.X.ToArray())
            {

                lastposition = i2.Current;
                //lasttime = i1.Current;
                lastvariance = i3.Current;
                    /* while (i1.MoveNext() &&*/
                    i2.MoveNext(); i3.MoveNext();
                //{
                    //double time=i1.Current;

                    if ((time - lasttime) != 0)
                        velocity = (i2.Current - lastposition) / (time - lasttime);

                    yield return new KeyValuePair<DateTime, Tuple<double, double>[]>(start + TimeSpan.FromSeconds(time),
                        new[] {
                    Tuple.Create(i2.Current+control, i3.Current),
                    Tuple.Create(velocity,i3.Current)
                //});
                });

                lastposition = i2.Current;
                    lasttime = time;// i1.Current;
                lastvariance = i3.Current;
            }


        }


        public static IEnumerable<KeyValuePair<double, Tuple<double, double>>> GetPositions(this GPOut gpout, double dt)
        {
            using (var i1 = gpout.X.ToList().GetEnumerator())
            using (var i2 = gpout.mu.ToList().GetEnumerator())
            using (var i3 = gpout.sd95.ToList().GetEnumerator())
            {
                while (i1.MoveNext() && i2.MoveNext() && i3.MoveNext())
                {
                    yield return new KeyValuePair<double, Tuple<double, double>>(i1.Current, Tuple.Create(i2.Current, i3.Current));

                }
            }
        }


        public static KeyValuePair<DateTime, Tuple<double, double>> Last(this GPOut gpout, DateTime dt)
        {

            return new KeyValuePair<DateTime, Tuple<double, double>>
           (dt + TimeSpan.FromDays(gpout.X.Last()), Tuple.Create(gpout.mu.Last(), gpout.sd95.Last()));



        }



    }
}
