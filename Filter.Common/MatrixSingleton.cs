using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter.Common
{

    public sealed class MatrixBuilder
    {
        private static readonly MatrixBuilder instance = new MatrixBuilder();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static MatrixBuilder()
        {
        }

        private MatrixBuilder()
        {
            Builder= MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
        }


        public MathNet.Numerics.LinearAlgebra.MatrixBuilder<double> Builder { get; set; }




        public static MatrixBuilder Instance
        {
            get
            {
                return instance;
            }
        }
    }


    public sealed class VectorBuilder
    {
        private static readonly VectorBuilder instance = new VectorBuilder();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static VectorBuilder()
        {
        }

        private VectorBuilder()
        {
            Builder = MathNet.Numerics.LinearAlgebra.Vector<double>.Build;
        }


        public MathNet.Numerics.LinearAlgebra.VectorBuilder<double> Builder { get; set; }




        public static VectorBuilder Instance
        {
            get
            {
                return instance;
            }
        }
    }



  
}
