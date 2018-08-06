using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Error
{
  class Helper
    {

        public static Func<double, double, double> ChooseLossFunction(Residual lf)
        {

            switch (lf)
            {

                case (Residual.MAE):
                    return LossFunctions.MeanAbsoluteError;
                case (Residual.MSE):
                    return LossFunctions.MeanSquareError;
                case (Residual.RMSE):
                    return LossFunctions.RootMeanSquareError;
                case (Residual.Hinge):
                    return LossFunctions.HingeError;
                case (Residual.CrossEntropy):
                    return LossFunctions.CrossEntropyError;
                default:
                    return LossFunctions.MeanSquareError;

            }

        }

    }
}
