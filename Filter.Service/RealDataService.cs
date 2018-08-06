using Filter.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deedle;
using Filter.Common;

namespace Filter.Service
{
    public class RealDataEnumerableService
    {


        public static IEnumerable<KeyValuePair<DateTime,double>> GetAppleData50()
        {

            return AppleData.Instance.Data.ToKVPairs().Take(50);

        }



        public static IEnumerable<KeyValuePair<DateTime,double[]>> GetAppleDataAsArrays()
        {

            return AppleData.Instance.Data.ToKVPairs().ToArrays();

        }


        public static IEnumerable<KeyValuePair<DateTime, System.Windows.Point>> GetAlterededAppleData()
        {
            var adta = AppleData.Instance.Data;
            var d = adta.Values.Min();
            var y = ((adta - 60) / 60).StartAt(new DateTime(2007, 1, 1)).EndAt(new DateTime(2007, 6, 30));
            return y.ToKVPairs().ToDateTimePoints();


        }


    }



    class AppleData
    {
        private static AppleData instance;

        private AppleData()
        {
            data = GetAppleData();
        }

        private Series<DateTime, double> data;

        public Series<DateTime, double>  Data { get { return data; } }




        public static AppleData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AppleData();
                }
                return instance;
            }
        }


        private static Series<DateTime, double> GetAppleData()
        {
            string fileName = "AAPL200609.csv";
            var frame = Frame.ReadCsv(Path.Combine(DirectoryHelper.GetParentPathName(), "Resources", fileName));
            var frameDate = frame.IndexRows<DateTime>("date").SortRowsByKey();
            return frameDate.GetColumn<double>("open");



        }
    }
}
