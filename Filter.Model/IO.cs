using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterSharp.Model
{
    public class IO<R>
    {

        public double[] Parameters { get; set; }

        public double Score { get; set; }

        public int Iteration { get; set; }

        public R Output { get; set; }

    }
}
