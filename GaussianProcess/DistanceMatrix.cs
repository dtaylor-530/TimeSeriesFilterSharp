
using MathNet.Numerics.LinearAlgebra;

using System.IO;
using Newtonsoft.Json;
using UtilityHelper;
using System.Linq;

namespace GaussianProcess
{

    public sealed class DistanceMatrix
    {
        private static readonly DistanceMatrix instance = new DistanceMatrix();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static DistanceMatrix()
        {
        }

        private DistanceMatrix()
        {
            string fileName = "distance_matrix.json";
            var path = System.IO.Path.Combine(DirectoryHelper.GetProjectPath(), "Resources", fileName);

            if (File.Exists(path))
                using (StreamReader r = new StreamReader(path))
                {
                    string json = r.ReadToEnd();
                    var matrix = JsonConvert.DeserializeObject<double[][]>(json);
                    Matrix = Matrix<double>.Build.DenseOfRowArrays(matrix);
                    Size = Matrix.RowCount;
                }
            else
            {
                var x = Enumerable.Range(0, 100).Select(_ => _ * 0.05).ToArray();
                var y = new double[100][];
                for (int i = 0; i < 100; i++)
                {
                    y[i] = new double[100];
                    for (int j = 0; j < 100; j++)
                    {
                        y[i][j] = x[i] - x[j];
                    }
                }
          
                Matrix = Matrix<double>.Build.DenseOfRowArrays(y);

            }
        }


        public Matrix<double> Matrix { get; set; }

        public int Size { get; set; }

        
        public static DistanceMatrix Instance
        {
            get
            {
                return instance;
            }
        }
    }

}
