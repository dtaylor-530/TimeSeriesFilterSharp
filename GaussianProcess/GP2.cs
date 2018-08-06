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
using MathNet.Numerics.LinearAlgebra.Factorization;
using GaussianProcess;
using Filter.Model;

namespace GaussianProcess
{
    public class GaussianProcessDynamicFactory
    {
        public static GP2 Build(Type type, double length, double noise)
        {

            return (new GP2((IMatrixkernel)Activator.CreateInstance(type), length, noise));
        }

    }




    public class GP2
    {
        //public int Mte { get; set; }

        //public double[] z { get; }
        //public double[] p { get; }


        //List<double> TrainingPointsX = new List<double>();
        //List<double> TrainingPointsY = new List<double>();

        private IMatrixkernel kernel;
        private double length;
        private double noise;
        //private int results;
        //Matrix<double> Kte;
        //private MathNet.Numerics.LinearAlgebra.Factorization.Svd<double> svd1;
        //private Matrix<double> Kxx_p_noise;
        Matrix<double> Kte;


        static GP2()
        {


        }

        //public void Reset()
        //{
        //    TrainingPointsX.Clear();
        //    TrainingPointsY.Clear();

        //}

        public GP2(IMatrixkernel kernel, double length, double noise)
        {

            this.kernel = kernel;
            this.length = length;
            this.noise = noise;
        }







        public GPOut Predict(double[] trainingPointsX, double[] trainingPointsY, params double[] TestPointsX)
        {

            if (TestPointsX.Count() == 1)
                Kte = kernel.Main(Matrix<double>.Build.Dense(1, 1), length);
            else
                Kte = kernel.Main(DistanceMatrix.Instance.Matrix.SubMatrix(0, TestPointsX.Count(), 0, TestPointsX.Count()), length);
            var svd1 = Task.Run(()=>
            trainingPointsX.Length > 0 ? Update(trainingPointsX, trainingPointsY) : GetDefaultSvd());

            var gpOut = new GPOut();
            Svd<double> svd;

            (gpOut.mu, svd) = Task.Run(() =>
            {
                if (trainingPointsX.Count() > 0)
                {
                    return Evaluate(TestPointsX, trainingPointsX.ToArray(), trainingPointsY.ToArray(), kernel, length, svd1.Result, Kte);
                }
                else
                {
                    return (Vector<double>.Build.DenseOfArray(Enumerable.Range(0, TestPointsX.Count()).Select(_ => 0d).ToArray()), Kte.Svd());
                    //gpOut.sd95 = 1.98 * (Kte.Diagonal()).PointwiseSqrt();
                }
            }).Result;

            gpOut.proj = svd.U * Matrix<double>.Build.DenseOfDiagonalVector(svd.S.PointwiseSqrt());
            gpOut.sd95 = 1.98 * (gpOut.proj * gpOut.proj.Transpose()).Diagonal().PointwiseSqrt();
            gpOut.X = TestPointsX;

            return gpOut;

        }




        private Svd<double> Update(double[] PointsX, double[] PointsY)
        {

            var trarr = PointsX.ToArray();

            Matrix<double> dmTr = MathHelper.ComputeDistanceMatrix(trarr, trarr);

            Vector<double> v = Vector<double>.Build.DenseOfArray(PointsY.ToArray());

            var Kxx_p_noise = kernel.Main(dmTr, length);
            for (int i = 0; i < PointsX.Count(); i++)
                Kxx_p_noise[i, i] += noise;

            var svd = Kxx_p_noise.Svd();

            for (int i = 0; i < PointsX.Count(); i++)
                svd.S[i] = svd.S[i] > Double.Epsilon ? 1.0 / svd.S[i] : 0;

            return svd;

        }




        private static (Vector<double>, Svd<double>) Evaluate(double[] testPointsX, double[] trainingPointsX, double[] trainingPointsY, IMatrixkernel kernel, double length, Svd<double> svd1, Matrix<double> Kte)
        {
            var gpOut = new GPOut();
            Svd<double> svd;

            var dmTeTr = MathHelper.ComputeDistanceMatrix(testPointsX, trainingPointsX.ToArray());
            var tmp = kernel.Main(dmTeTr, length) * svd1.U;
            Vector<double> v = Vector<double>.Build.DenseOfEnumerable(trainingPointsY);

            var mu = tmp * (svd1.S.PointwiseMultiply(svd1.U.Transpose() * v));
            var cov = tmp * Matrix<double>.Build.DenseOfDiagonalVector(svd1.S.PointwiseSqrt());
            cov = cov * cov.Transpose();
            cov = Kte - cov;
            svd = cov.Svd();
            for (int i = 0; i < svd.S.Count; i++)
            {
                svd.S[i] = svd.S[i] < Double.Epsilon ? 0 : svd.S[i];

            }

            return (mu, svd);
        }

        private  Svd<double> GetDefaultSvd()
        {
            Matrix<double> dmTr = MathHelper.ComputeDistanceMatrix(new[] { 0d }, new[] { 0d });

            Vector<double> v = Vector<double>.Build.DenseOfArray(new[] { 0d });

            var Kxx_p_noise = kernel.Main(dmTr, length);
            for (int i = 0; i < 1; i++)
                Kxx_p_noise[i, i] += noise;

            return Kxx_p_noise.Svd();
        }

    }







}





//static class RandomHelper
//{

//    static Random rand = new Random();


//    public static double[] randnArray(int size)
//    {
//        var zs = new double[size];

//        for (int i = 0; i < size; i++)
//        {
//            zs[i] = Normal.Sample(rand, 60, 1);
//        }
//        return zs;
//    }



//}





//public sealed class DistanceMatrix
//{
//    private static readonly Lazy<DistanceMatrix> lazy =
//        new Lazy<DistanceMatrix>(() => new DistanceMatrix());

//    public static DistanceMatrix Instance { get { return lazy.Value; } }

//    public Matrix<double> DistanceMatrix { get; set; }

//    public int MatrixColumnLength { get; set; }

//    private DistanceMatrix()
//    {
//        string fileName = "distance_matrix.json";
//        var path = System.IO.Path.Combine(Filter.Utility.PathHelper.GetParentPathName(), "Resources", fileName);


//        using (StreamReader r = new StreamReader(path))
//        {
//            string json = r.ReadToEnd();
//            var matrix = JsonConvert.DeserializeObject<double[][]>(json);
//            DistanceMatrix = Matrix<double>.Build.DenseOfRowArrays(matrix);
//            MatrixColumnLength = DistanceMatrix.RowCount;
//        }


//    }






//}







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

