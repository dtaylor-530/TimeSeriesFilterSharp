using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StarMathLib;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;

namespace GaussianProcess
{
    //http://www.tmpl.fi/gp/



    public class GPModel
    {

        public double[] tePointsX { get; set; } = MathNet.Numerics.Generate.LinearSpaced(Singleton.Instance.MatrixColumnLength, -5, 5);
        

        public GPModels gpms = new GPModels();

        //public void DrawMeanAndVar()
        //{
        //    foreach (var gp in gpms.GPs)
        //    {
        //        var xf = gp.ComputeProjection();


        //        List<Tuple<double, double>> x = tePointsX.Zip(xf.mu.ToArray(), (a, b) => Tuple.Create(a, b)).ToList();


        //    }
        //}


        public List<GPOut> AddTrainingPoints(double[] trPointsX, double[] trPointsY)
        {
            var min = trPointsX.Min();
            var max = trPointsX.Max();

            var extra = 0.1 * (max - min);


            tePointsX = MathNet.Numerics.Generate.LinearSpaced(Singleton.Instance.DistanceMatrix.RowCount, min - extra, max + extra);
            // added training points
            var dmTr = Helper2.computeDistanceMatrix(trPointsX, trPointsX);
            var dmTeTr = Helper2.computeDistanceMatrix(tePointsX, trPointsX);

            Vector<double> v = Vector<double>.Build.DenseOfArray(trPointsY);
            var newGPs = gpms.ComputeProjections(dmTr, dmTeTr, v);

            return newGPs;

            //    Dictionary<string, cf> cfs =
            //new Dictionary<string, GaussianProcess.cf> { { "Exponentiated quadratic", new cf() } };

        }



    }


    public class GPModels
    {


        public List<GP> GPs { get; set; } = new List<GP>();


        public GPModels()
        {

            GPs.Add(new GP(new cf(), 0, 1, 10));

        }


        public List<GPOut> ComputeProjections(Matrix<double> dmTr, Matrix<double> dmTeTr, Vector<double> trY)
        {

            return GPs.Select(_ => _.ComputeProjection(dmTr, dmTeTr, trY)).ToList();


        }

    }




    public class GPOut
    {


        public Matrix<double> proj { get; set; }
        public Vector<double> mu { get; set; }
        public Vector<double> sd95 { get; set; }


    }




    public class GP
    {
        public int Mte { get; set; }

        public double[] z { get; set; }
        public double[] p { get; set; }
        private Ikernel kernel;
        private double length;
        private double noise;
        private int id;

            

        public GP(Ikernel kernel, int id, double length, double noise)
        {
            Mte = Singleton.Instance.MatrixColumnLength;
            this.z = Helper2.randnArray(Mte);
            this.p = Helper2.randnArray(Mte);
            this.kernel = kernel;
            this.length = length;
            this.noise = noise;
            this.id = id;

        }




        public GPOut ComputeProjection(Matrix<double> dmTr, Matrix<double> dmTeTr, Vector<double> trY)
        {
            var Mtr = (dmTr).RowCount;

            var gpOut = new GPOut();


            var Kte = kernel.f(Singleton.Instance.DistanceMatrix, length);

            if (Mtr > 0)
            {
                var Kxx_p_noise = kernel.f(dmTr, length);
                for (int i = 0; i < Mtr; i++)
                    Kxx_p_noise[i, i] += noise;

                var svd1 = Kxx_p_noise.Svd();

                for (int i = 0; i < Mtr; i++)
                    svd1.S[i] = svd1.S[i] > Double.Epsilon ? 1.0 / svd1.S[i] : 0;


                var tmp = kernel.f(dmTeTr, length) * svd1.U;
                // there seems to be a bug in numeric.svd: svd1.U and transpose(svd1.V) are not always equal for a symmetric matrix


                gpOut.mu = tmp * (svd1.S.PointwiseMultiply(svd1.U.Transpose() * (trY)));
                var cov = tmp * Matrix<double>.Build.DenseOfDiagonalVector(svd1.S.PointwiseSqrt());
                cov = cov * cov.Transpose();
                cov = Kte - cov;
                var svd2 = cov.Svd();
                for (int i = 0; i < Mte; i++)
                {
                    if (svd2.S[i] < Double.Epsilon)
                    {
                        svd2.S[i] = 0.0;
                    }
                }
                gpOut.proj = svd2.U * Matrix<double>.Build.DenseOfDiagonalVector(svd2.S.PointwiseSqrt());
                gpOut.sd95 = 1.98 * (gpOut.proj * gpOut.proj.Transpose()).Diagonal().PointwiseSqrt();
            }
            else
            {
                gpOut.sd95 = 1.98 * (Kte.Diagonal()).PointwiseSqrt();
                var svd = Kte.Svd();
                gpOut.proj = svd.U * Matrix<double>.Build.DenseOfDiagonalVector(svd.S.PointwiseSqrt());
                gpOut.mu = Vector<double>.Build.DenseOfArray(Enumerable.Range(0, Mte).Select(_ => 0d).ToArray());
            }


            return gpOut;

        }

    }





    static class Helper2
    {

        static Random rand = new Random();


        public static Matrix<double> computeDistanceMatrix(double[] xdata1, double[] xdata2)
        {
            var dm = Matrix<double>.Build.Dense(xdata1.Length, xdata2.Length, 0);
            for (var i = 0; i < xdata1.Length; i++)
            {
                for (var j = 0; j < xdata2.Length; j++)
                {
                    var val = Math.Abs(xdata2[j] - xdata1[i]);
                    dm[i, j] = val;
                }
            }
            return dm;
        }


        public static double[] randnArray(int size)
        {
            var zs = new double[size];
            //var randn = d3.random.normal();

            for (var i = 0; i < size; i++)
            {
                zs[i] = Normal.Sample(rand, 60, 1);
            }
            return zs;
        }



    }


    ///** @jsx React.DOM */
    //var tePointsX = numeric.linspace(-5, 5, numeric.dim(DistanceMatrix)[0]);

    public class cf : Ikernel
    {

        public Matrix<double> f(Matrix<double> r, params double[] vars)
        {
            var x = r.PointwisePower(2) * ((-0.5 / (vars[0] * vars[0])));

            return x.PointwiseExp();
        }


    }


    public interface Ikernel
    {

        Matrix<double> f(Matrix<double> r, params double[] vars);


    }


    public sealed class Singleton
    {
        private static readonly Lazy<Singleton> lazy =
            new Lazy<Singleton>(() => new Singleton());

        public static Singleton Instance { get { return lazy.Value; } }

        public Matrix<double> DistanceMatrix { get; set; }

        public int MatrixColumnLength { get; set; }

        private Singleton()
        {
            string fileName = "distance_matrix.json";
            var path = System.IO.Path.Combine(Filter.Utility.PathHelper.GetParentPathName(), "Resources", fileName);


            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                var matrix = JsonConvert.DeserializeObject<double[][]>(json);
                DistanceMatrix = Matrix<double>.Build.DenseOfRowArrays(matrix) ;
                MatrixColumnLength = DistanceMatrix.RowCount;
            }


        }






    }


}



//    public class Helper2
//    {
//        // ids must be in order of the array
//        var cfs = [
//      {'id': 0,
//           'name': 'Exponentiated quadratic',
//           'f': function(r, params) {
//         return numeric.exp(numeric.mul(-0.5 / (params[0] * params[0]), numeric.pow(r, 2)));
//       }
//},
//      {'id': 1,
//       'name': 'Exponential',
//       'f': function(r, params)
//{
//    return numeric.exp(numeric.mul(-0.5 / params[0], r));
//}
//      },
//      {'id': 2,
//       'name': 'Matern 3/2',
//       'f': function(r, params)
//{
//    var tmp = numeric.mul(Math.sqrt(3.0) / params[0], r);
//    return numeric.mul(numeric.add(1.0, tmp), numeric.exp(numeric.neg(tmp)));
//}
//      },
//      {'id': 3,
//       'name': 'Matern 5/2',
//       'f': function(r, params)
//{
//    var tmp = numeric.mul(Math.sqrt(5.0) / params[0], r);
//    var tmp2 = numeric.div(numeric.mul(tmp, tmp), 3.0);
//    return numeric.mul(numeric.add(numeric.add(1, tmp), tmp2), numeric.exp(numeric.neg(tmp)));
//}
//      },
//      {'id': 4,
//       'name': 'Rational quadratic (alpha=1)',
//       'f': function(r, params)
//{
//    return numeric.pow(numeric.add(1.0, numeric.div(numeric.pow(r, 2), 2.0 * params[0] * params[0])), -1);
//}
//      },
//      {'id': 5,
//       'name': 'Piecewise polynomial (q=0)',
//       'f': function(r, params)
//{
//    var tmp = numeric.sub(1.0, numeric.div(r, params[0]));
//    var dims = numeric.dim(tmp);
//    for (var i = 0; i < dims[0]; i++)
//    {
//        for (var j = 0; j < dims[1]; j++)
//        {
//            tmp[i][j] = tmp[i][j] > 0.0 ? tmp[i][j] : 0.0;
//        }
//    }
//    return tmp;
//}
//      },
//      {'id': 6,
//       'name': 'Piecewise polynomial (q=1)',
//       'f': function(r, params)
//{
//    var tmp1 = numeric.div(r, params[0]);
//    var tmp = numeric.sub(1.0, tmp1);
//    var dims = numeric.dim(tmp);
//    for (var i = 0; i < dims[0]; i++)
//    {
//        for (var j = 0; j < dims[1]; j++)
//        {
//            tmp[i][j] = tmp[i][j] > 0.0 ? tmp[i][j] : 0.0;
//        }
//    }
//    return numeric.mul(numeric.pow(tmp, 3), numeric.add(numeric.mul(3.0, tmp1), 1.0));
//}
//      },
//      {'id': 7,
//       'name': 'Periodic (period=pi)',
//       'f': function(r, params)
//{
//    return numeric.exp(numeric.mul(-2.0 / (params[0] *params[0]), numeric.pow(numeric.sin(r), 2)));
//}
//      },
//      {'id': 8,
//       'name': 'Periodic (period=1)',
//       'f': function(r, params)
//{
//    return numeric.exp(numeric.mul(-2.0 / (params[0] *params[0]), numeric.pow(numeric.sin(numeric.mul(Math.PI, r)), 2)));
//}
//      }
//    ];




//        var GPAxis = React.createClass({
//  render: function() {
//    return (< svg ></ svg >);
//    },
//  shouldComponentUpdate: function() { return false; },
//  drawTrPoints: function(pointsX, pointsY)
//    {
//        var x = this.scales.x;
//        var y = this.scales.y;
//        var p = this.trPoints.selectAll("circle.trpoints")
//                             .data(d3.zip(pointsX, pointsY))
//                             .attr("cx", function(d) { return x(d[0]); })
//                         .attr("cy", function(d) { return y(d[1]); });
//        p.enter().append("circle")
//                 .attr("class", "trpoints")
//                 .attr("r", 2)
//                 .attr("cx", function(d) { return x(d[0]); })
//             .attr("cy", function(d) { return y(d[1]); });
//        p.exit().remove();
//    },
//  animationId: 0,
//  componentWillReceiveProps: function(props)
//    {
//        // bind events
//        if (props.state.addTrPoints)
//        {
//            d3.select(this.getDOMNode()).on("click", this.addTrPoint);
//        }
//        else
//        {
//            d3.select(this.getDOMNode()).on("click", null);
//        }
//        // redraw training points
//        this.drawTrPoints(props.state.trPointsX, props.state.trPointsY);

//        this.drawMeanAndVar(props);

//        if (this.props.state.showSamples !== props.state.showSamples)
//        {
//            this.drawPaths(props);
//        }

//        if (this.props.state.samplingState !== props.state.samplingState)
//        {
//            clearInterval(this.animationId);
//            if (props.state.samplingState === 1)
//            {
//                this.animationId = setInterval((function() { this.updateState(); this.drawPaths(); }).bind(this), 500);
//            }
//            else if (props.state.samplingState === 2)
//            {
//                this.animationId = setInterval((function() { this.contUpdateState(); this.drawPaths(); }).bind(this), 50);
//            }
//        }
//    },
//  addTrPoint: function()
//    {
//        var mousePos = d3.mouse(this.getDOMNode());
//        var x = this.scales.x;
//        var y = this.scales.y;

//        // x is transformed to a point on a grid of 200 points between -5 and 5
//        this.props.addTrPoint(Math.round((x.invert(mousePos[0] - 50) + 5) / 10 * 199) / 199 * 10 - 5, y.invert(mousePos[1] - 50));
//    },
//  updateState: function()
//    {
//        var M = numeric.dim(DistanceMatrix)[1];
//        for (var i = 0; i < this.props.state.GPs.length; i++)
//        {
//            var gp = this.props.state.GPs[i];
//            gp.z = randnArray(M);
//        }
//    },
//  stepState: 0,
//  contUpdateState: function()
//    {
//        var M = numeric.dim(DistanceMatrix)[1];
//        var alfa = 1.0 - this.props.state.alfa;
//        var n_steps = this.props.state.NSteps;
//        var t_step = this.props.state.stepSize / n_steps;
//        this.stepState = this.stepState % n_steps;

//        for (var i = 0; i < this.props.state.GPs.length; i++)
//        {
//            var gp = this.props.state.GPs[i];

//            // refresh momentum: p = alfa * p + sqrt(1 - alfa^2) * randn(size(p))
//            if (this.stepState == (n_steps - 1))
//                gp.p = numeric.add(numeric.mul(alfa, gp.p), numeric.mul(Math.sqrt(1 - alfa * alfa), randnArray(M)));

//            var a = gp.p.slice(0),
//                b = gp.z.slice(0),
//                c = numeric.mul(-1, gp.z.slice(0)),
//                d = gp.p.slice(0);

//            gp.z = numeric.add(numeric.mul(a, Math.sin(t_step)), numeric.mul(b, Math.cos(t_step)));
//            gp.p = numeric.add(numeric.mul(c, Math.sin(t_step)), numeric.mul(d, Math.cos(t_step)));
//        }
//        this.stepState = this.stepState + 1;
//    },
//  drawMeanAndVar: function(props)
//    {
//        var gpline = this.gpline;
//        if (props.state.showMeanAndVar)
//        {
//            var gps = props.state.GPs;
//        }
//        else
//        {
//            var gps = [];
//        }

//        var paths = this.meanLines.selectAll("path").data(gps, function(d) { return d.id; })
//                          .attr("d", function(d) {
//            var datay = d.mu;
//            return gpline(d3.zip(tePointsX, datay));
//        });
//        paths.enter().append("path").attr("d", function(d) {
//            var datay = d.mu;
//            return gpline(d3.zip(tePointsX, datay));
//        })
//                                .attr("class", function(d) {
//            return "muline line line" + d.id;
//        });
//        paths.exit().remove();

//        var pathsUp = this.upSd95Lines.selectAll("path").data(gps, function(d) { return d.id; })
//                          .attr("d", function(d) {
//            var datay = numeric.add(d.mu, d.sd95);
//            return gpline(d3.zip(tePointsX, datay));
//        });
//        pathsUp.enter().append("path").attr("d", function(d) {
//            var datay = numeric.add(d.mu, d.sd95);
//            return gpline(d3.zip(tePointsX, datay));
//        })
//                                .attr("class", function(d) {
//            return "sdline line line" + d.id;
//        });
//        pathsUp.exit().remove();

//        var pathsDown = this.downSd95Lines.selectAll("path").data(gps, function(d) { return d.id; })
//                          .attr("d", function(d) {
//            var datay = numeric.sub(d.mu, d.sd95);
//            return gpline(d3.zip(tePointsX, datay));
//        });
//        pathsDown.enter().append("path").attr("d", function(d) {
//            var datay = numeric.sub(d.mu, d.sd95);
//            return gpline(d3.zip(tePointsX, datay));
//        })
//                                .attr("class", function(d) {
//            return "sdline line line" + d.id;
//        });
//        pathsDown.exit().remove();
//    },
//  drawPaths: function(props)
//    {
//        if (!props) var props = this.props;
//        var gpline = this.gpline;
//        if (props.state.showSamples)
//        {
//            var gps = props.state.GPs;
//        }
//        else
//        {
//            var gps = [];
//        }
//        var paths = this.lines.selectAll("path").data(gps, function(d) { return d.id; })
//                          .attr("d", function(d) {
//            var datay = numeric.add(numeric.dot(d.proj, d.z), d.mu);
//            return gpline(d3.zip(tePointsX, datay));
//        });
//        paths.enter().append("path").attr("d", function(d) {
//            var datay = numeric.add(numeric.dot(d.proj, d.z), d.mu);
//            return gpline(d3.zip(tePointsX, datay));
//        })
//                                .attr("class", function(d) {
//            return "line line" + d.id;
//        });
//        paths.exit().remove();
//    },
//  scales: { x: null, y: null },
//  componentDidMount: function()
//{
//    var svg = d3.select(this.getDOMNode());
//    var height = svg.attr("height"),
//        width = svg.attr("width");
//    if (!height)
//    {
//        height = 300;
//        svg.attr("height", height);
//    }
//    if (!width)
//    {
//        width = 500;
//        svg.attr("width", width);
//    }
//    var margin = 50;
//    svg = svg.append("g")
//             .attr("transform", "translate(" + margin + "," + margin + ")");
//    this.svg = svg;
//    var fig_height = height - 2 * margin,
//        fig_width = width - 2 * margin;

//    // helper functions
//    var x = d3.scale.linear().range([0, fig_width]).domain([-5, 5]);
//    var y = d3.scale.linear().range([fig_height, 0]).domain([-3, 3]);
//    this.scales.x = x;
//    this.scales.y = y;
//    var xAxis = d3.svg.axis()
//                      .scale(x)
//                      .orient("bottom");
//    var yAxis = d3.svg.axis()
//                      .scale(y)
//                      .orient("left");
//    this.gpline = d3.svg.line()
//                        .x(function(d) { return x(d[0]); })
//                        .y(function(d) { return y(d[1]); });

//    // axes
//    svg.append("g")
//       .attr("class", "x axis")
//       .attr("transform", "translate(0," + fig_height + ")")
//       .call(xAxis);

//    svg.append("g")
//       .attr("class", "y axis")
//       .call(yAxis);

//    this.meanLines = svg.append("g");
//    this.upSd95Lines = svg.append("g");
//    this.downSd95Lines = svg.append("g");
//    this.lines = svg.append("g");
//    this.trPoints = svg.append("g");
//    this.drawTrPoints(this.props.state.trPointsX, this.props.state.trPointsY);
//    this.drawPaths();
//}
//});


//var GPList = React.createClass({
//  render: function() {
//    var delGP = this.props.delGP;
//var gplist = this.props.GPs.map(function(gp) {
//      return (< tr key ={gp.id}>
//                <td className = { "tr" + gp.id }>{gp.id}</td><td>{cfs[gp.cf].name}</td><td>{gp.params[0].toFixed(2)}</td><td>{gp.params[1].toFixed(2)}</td><td><button onClick = { delGP(gp.id) }>remove</button></td>
//              </tr>);
//    });
//    return (<tbody>{gplist}</tbody>);
//  }

