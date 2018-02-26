using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnscentedKalmanFilter
{
    public interface IFunction
    {
        Matrix<double> Process(Matrix<double> x);
        Matrix<double> Process(Matrix<double> x,double time);
        Vector<double> Process(Vector<double> x,double time);
    }
}
