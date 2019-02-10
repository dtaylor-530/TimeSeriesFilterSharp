using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterSharp.Model
{
    public interface IFilterWrapperBuilder
    {
        IFilterWrapper Build(double a, double b);



    }
}
