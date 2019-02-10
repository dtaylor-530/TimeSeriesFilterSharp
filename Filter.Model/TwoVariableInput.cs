using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterSharp.Model
{
    public class TwoVariableInput
    {
        public double VarA { get; }
        public double VarB { get; }
        public Type FilterSharp { get; }

        public TwoVariableInput(double a, double b, Type f)
        {
            VarA = a;
            VarB = b;
            FilterSharp = f;
        }

    }
}
