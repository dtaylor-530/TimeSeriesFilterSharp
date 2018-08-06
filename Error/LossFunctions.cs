using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Error
{


    //https://makingnoiseandhearingthings.com/2017/12/30/how-to-be-wrong-measuring-error-in-machine-learning-models/
    //https://ml-cheatsheet.readthedocs.io/en/latest/loss_functions.html
    public class LossFunctions
    {
        /// <summary>
        /// Mean Squared Error, or L2 loss.
        /// </summary>

        public static double MeanSquareError(double est, double meas)
        {

            return Math.Pow(est - meas, 2);
        }


        /// <summary>
        /// Root Mean Squared Error
        /// </summary>
        public static double RootMeanSquareError(double est, double meas)
        {

            return Math.Sqrt(Math.Pow(est - meas, 2));
        }

        /// <summary>
        /// Mean Absolute Error, or L1 loss
        /// </summary>
        public static double MeanAbsoluteError(double est, double meas)
        {
            return Math.Abs(est - meas);

        }




       // for classification model whose output is a probability value between 0 and 1

        public static double HingeError(double est, double meas)
        {
            return Math.Max(0, 1 - est * meas);

        }

        // for classification model whose output is a probability value between 0 and 1

        public static double CrossEntropyError(double est, double meas)
        {
            if (meas == 1)
                return -Math.Log(est);
            else
                return -Math.Log(1 - est);

        }


  

   
        //def Huber(yHat, y):
        //   pass
        //Kullback-Leibler
        //Code



    }
}
