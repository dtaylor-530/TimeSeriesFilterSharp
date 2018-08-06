using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter.Model
{
    public class TwoVariableInput
    {
        public double VarA { get; }
        public double VarB { get; }
        public Type Filter { get; }

        public TwoVariableInput(double a, double b, Type f)
        {
            VarA = a;
            VarB = b;
            Filter = f;
        }

    }
}
