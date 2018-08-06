using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaussianProcess.Wrap
{
    public class MultiGaussianProcessFactory
    {
        public static List<GP2> BuildDefault(int results)
        {
            return new List<GP2>
            {

                //new GPDynamic(new RationalQuadratic(), 9, 0.001, results),
                    new GP2(new RationalQuadratic(), 9, 0.001)
            };

          

        }




        //DateTime _dt = default(DateTime);
        //  private DateTime lastDate = default(DateTime);
        //double y1;

        //public List<GPDynamic> GPMMs { get; set; }





        //public static TimeDependentMultiGaussianProcess BuildDefault(int results)
        //{

        //    return new
        //        TimeDependentMultiGaussianProcess
        //    {
        //        GaussianProcesses = new List<GPDynamic>
        //    {
        //            //new GPDynamic(new RationalQuadratic(), 9, 0.001, results),
        //            new GPDynamic(new RationalQuadratic(), 9, 0.001, results)
        //        }
        //    };

        //}
    }


    public class GaussianProcessWrapperFactory
    {
        public static GaussianProcessWrapper BuildDefault(int results)
        {
            //new GPDynamic(new RationalQuadratic(), 9, 0.001, results),
            return new GaussianProcessWrapper
            (
                gaussianProcess : new GP2(new RationalQuadratic(), 9, 0.001)
            );
        }

    }



}
