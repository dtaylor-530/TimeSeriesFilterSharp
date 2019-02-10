#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using System;
using System.Linq;

using Accord.Extensions.Statistics.Filters;
using Accord.Math;
using ModelState = KalmanFilter.Wrap.ConstantVelocity1DModel;
using Accord.Statistics.Distributions.Univariate;
using Accord.Extensions.Math;
using System.Collections.Generic;

namespace KalmanFilter.Wrap
{
    public class AccordEx1DKWrapper
    {

        private double accelNoise;
        private double measurementNoise;
        private ModelState State;
        private DiscreteKalmanFilter<ModelState, double> kalman;

        private readonly int measurementDimension = 1;

        public AccordEx1DKWrapper(double processNoise, double measurementNoise, DateTime dt, double initialPosition)
        {

            initialize(processNoise, measurementNoise, dt, initialPosition);


        }

        private void initialize(double processNoise, double measNoise, DateTime dt, double initialPosition)
        {
            accelNoise = processNoise;
            measurementNoise = measNoise;


            State = new ConstantVelocity1DModel(dt.Ticks, initialPosition);/*process.GetNoisyState(accelNoise); //assuming we measured process params (noise)*/
            var initialStateError = State.GetProcessNoise(accelNoise);


            kalman = new DiscreteKalmanFilter<ModelState, double>(State, initialStateError,
                                                                  measurementDimension /*(position)*/, 0 /*no control*/,
                                                                  x => x.ToArray(), x => ModelState.FromArray(x), x => new double[] { x });

            kalman.ProcessNoise = State.GetProcessNoise(accelNoise);
            kalman.MeasurementNoise = Matrix.Diagonal<double>(kalman.MeasurementVectorDimension, measurementNoise).Power(2); //assuming we measured process params (noise) - ^2 => variance

            kalman.MeasurementMatrix = new double[,] //just pick point coordinates for an observation [2 x 4] (look at ConstantVelocity2DModel)
                {
                    {1,  0}

                };


        }





        public IEnumerable<double[]> BatchRun(IEnumerable<Tuple<DateTime,double>> measurements)
        {

            foreach(var meas in measurements)
            {
                yield return Run(meas.Item1, meas.Item2);

            }

        }



        private double[] Run(DateTime dt, double position)
        {

            State.UpdateTime(dt);

            kalman.TransitionMatrix = State.GetTransitionMatrix();

            kalman.Predict();

            var s = kalman.State;

            kalman.Correct(position);

            var correctedPosition = kalman.State.Position;

            return s.ToArray();
        }



    }




    //public static class StateFactory
    //{


    //    public ConstantVelocity1DModel GetNoisyState(double accelerationNoise)
    //    {
    //        var processNoiseMat = ConstantVelocity1DModel.GetProcessNoise(accelerationNoise);
    //        var noise = NormalDistribution.Generate(ConstantVelocity1DModel.Dimension).Multiply(processNoiseMat);

    //        return new ConstantVelocity1DModel
    //        {
    //            Position = 
    //              currentState.Position+ (float)noise[0]
    //            ,

    //            Velocity =

    //               currentState.Velocity + (float)noise[3]

    //        };
    //    }

    //}



    /// <summary>
    /// Linear acceleration model for 2D case.
    /// <para>Vector is composed as: [X, vX, Y, vY]</para>
    /// Model is constructed as following:
    /// p(i) = p(i-1) + (&#x0394;t) * v(i-1);
    /// v(i) = v(i-1);
    /// </summary>
    public class ConstantVelocity1DModel : ICloneable
    {
        /// <summary>
        /// Gets the dimension of the model.
        /// </summary>
        public const int Dimension = 4;

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public double Position;
        /// <summary>
        /// Gets or sets the velocity.
        /// </summary>
        public double Velocity;

        private double TimeDifference;

        public double Time { get; set; }

        /// <summary>
        /// Constructs an empty model.
        /// </summary>
        public ConstantVelocity1DModel(double time, double position, double velocity = 0)
        {
            Position = position;
            Time = time;
            Velocity = velocity;

        }

        /// <summary>
        /// Evaluates the model by using the provided transition matrix.
        /// </summary>
        /// <param name="transitionMat">Transition matrix.</param>
        /// <returns>New model state.</returns>
        public ConstantVelocity1DModel Evaluate(double[,] transitionMat)
        {
            var stateVector = transitionMat.Dot(this.ToArray());
            return ConstantVelocity1DModel.FromArray(stateVector);
        }



        public void UpdateTime(DateTime dt)
        {

            TimeDifference = (dt.Ticks - Time);
            Time = dt.Ticks;

        }



        /// <summary>
        /// Gets the state transition matrix [4 x 4].
        /// </summary>
        /// <param name="timeInterval">Time interval.</param>
        /// <returns>State transition matrix.</returns>
        public double[,] GetTransitionMatrix()
        {

            return new double[,]
                {
                    {1, TimeDifference,  },
                    {0, 1, },

                };
        }

        /// <summary>
        /// Gets the position measurement matrix [2 x 4] used in Kalman FilterSharping.
        /// </summary>
        /// <returns>Position measurement matrix.</returns>
        public static double[,] GetPositionMeasurementMatrix()
        {
            return new double[,] //just pick point coordinates for an observation [2 x 6] (look at used state model)
                { 
                   //X,  vX, Y,  vY   (look at ConstantAcceleration2DModel)
                    {1,  0, }, //picks X

                };
        }

        /// <summary>
        /// Gets process-noise matrix [4 x 2] where the location is affected by (dt * dt) / 2 and velocity with the factor of dt - integrals of dt. 
        /// Factor 'dt' represents time interval.
        /// </summary>
        /// <param name="accelerationNoise">Acceleration noise.</param>
        /// <param name="timeInterval">Time interval.</param>
        /// <returns>Process noise matrix.</returns>
        public double[,] GetProcessNoise(double accelerationNoise)
        {

            var G = new double[,]
            {
                {(TimeDifference*TimeDifference) / 2, 0},
                {TimeDifference, 0},
                //{0, (dt*dt) / 2},
                //{0, dt}
            };

            var Q = Matrix.Diagonal<double>(G.ColumnCount(), accelerationNoise); //TODO - check: noise * noise ?
            var processNoise = G.Multiply(Q).Multiply(G.Transpose());
            return processNoise;
        }

        #region Array conversion

        /// <summary>
        /// Converts the array to the model.
        /// </summary>
        /// <param name="arr">Array to convert from.</param>
        /// <returns>Model.</returns>
        public static ConstantVelocity1DModel FromArray(double[] arr)
        {

            return new ConstantVelocity1DModel
     (
         position: arr[0],
           velocity: arr[1],
         time: arr[2]
     );
        }


        /// <summary>
        /// Converts the model to the array.
        /// </summary>
        /// <param name="modelState">Model to convert.</param>
        /// <returns>Array.</returns>
        public double[] ToArray()
        {
            return new double[]
                {
                   this.Position,
                                this.Velocity,
                                this.Time
                };
        }

        #endregion

        /// <summary>
        /// Clones the model.
        /// </summary>
        /// <returns>The copy of the model.</returns>
        public object Clone()
        {
            return new ConstantVelocity1DModel
            (
                position: this.Position,
                  velocity: this.Velocity,
                time: this.Time
            );
        }
    }
}




//    public class ConstantVelocityProcess
//{
//    public static Size WorkingArea = new Size(100, 100);
//    public static float TimeInterval = 1;

//    NormalDistribution normalDistribution = new NormalDistribution(0, 0.2);
//    Random rand = new Random();

//    ConstantVelocity2DModel initialState;
//    ConstantVelocity2DModel currentState;

//    public ConstantVelocityProcess()
//    {
//        currentState = new ConstantVelocity2DModel
//        {
//            Position = new PointF(50, 1),
//            Velocity = new PointF(0.3f, 0.3f)
//        };

//        initialState = currentState;
//    }

//    public bool GoToNextState()
//    {
//        Func<PointF, bool> isBorder = (point) =>
//        {
//            return point.X <= 0 || point.X >= WorkingArea.Width ||
//                   point.Y <= 0 || point.Y >= WorkingArea.Height;
//        };

//        doneFullCycle = false;
//        var prevPos = currentState.Position;
//        var speed = currentState.Velocity;

//        if (isBorder(currentState.Position))
//        {
//            var temp = speed.X;
//            speed.X = -speed.Y;
//            speed.Y = temp;

//            if (speed.Equals(initialState.Velocity)) doneFullCycle = true;
//        }

//        var nextState = new ConstantVelocity2DModel
//        {
//            Position = new PointF
//            {
//                X = prevPos.X + speed.X * TimeInterval,
//                Y = prevPos.Y + speed.Y * TimeInterval
//            },

//            Velocity = speed
//        };

//        currentState = nextState;
//    }

//    public ConstantVelocity2DModel GetNoisyState(double accelerationNoise)
//    {
//        var processNoiseMat = ConstantVelocity2DModel.GetProcessNoise(accelerationNoise);
//        var noise = normalDistribution.Generate(ConstantVelocity2DModel.Dimension).Multiply(processNoiseMat);

//        return new ConstantVelocity2DModel
//        {
//            Position = new PointF
//            {
//                X = currentState.Position.X + (float)noise[0],
//                Y = currentState.Position.Y + (float)noise[2]
//            },

//            Velocity = new PointF
//            {
//                X = currentState.Velocity.X + (float)noise[1],
//                Y = currentState.Velocity.Y + (float)noise[3]
//            }
//        };
//    }

//    public PointF TryGetNoisyMeasurement(double measurementNoise, out bool isSuccess, double missingMeasurementProbability = 0.2)
//    {
//        isSuccess = rand.NextDouble() > missingMeasurementProbability;
//        if (!isSuccess)
//            return new PointF();

//        return new PointF
//        {
//            X = currentState.Position.X + (float)normalDistribution.Generate() * (float)measurementNoise,
//            Y = currentState.Position.Y + (float)normalDistribution.Generate() * (float)measurementNoise
//        };
//    }



