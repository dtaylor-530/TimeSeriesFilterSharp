//using Accord.Extensions.Statistics.Filters;
//using DotImaging.Primitives2D;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace KalmanFilter.Wrap
//{
//    class AccordExKalmanWrap
//    {


//            ConstantVelocityProcess process;
//            KalmanFilter<ModelState, PointF> kalman;

//            public Kalman2dDemo()


//                process = new ConstantVelocityProcess();
//                initializeKalman();

//                timer.Enabled = true;
//            }

//            private void initializeKalman()
//            {
//                float accelNoise = (float)numProcessNoise.Value;
//                float measurementNoise = (float)numMeasurementNoise.Value;

//                var measurementDimension = 2; //just coordinates

//                var initialState = process.GetNoisyState(accelNoise); //assuming we measured process params (noise)
//                var initialStateError = ModelState.GetProcessNoise(accelNoise);

//                kalman = new DiscreteKalmanFilter<ModelState, PointF>(initialState, initialStateError,
//                                                                      measurementDimension /*(position)*/, 0 /*no control*/,
//                                                                      x => ModelState.ToArray(x), x => ModelState.FromArray(x), x => new double[] { x.X, x.Y });

//                kalman.ProcessNoise = ModelState.GetProcessNoise(accelNoise);
//                kalman.MeasurementNoise = Matrix.Diagonal<double>(kalman.MeasurementVectorDimension, measurementNoise).ElementwisePower(2); //assuming we measured process params (noise) - ^2 => variance

//                kalman.MeasurementMatrix = new double[,] //just pick point coordinates for an observation [2 x 4] (look at ConstantVelocity2DModel)
//                    { 
//                   //X,  vX, Y,  vY (look at ConstantVelocity2DModel)
//                    {1,  0,  0,  0}, //picks X
//                    {0,  0,  1,  0}  //picks Y
//                    };

//                kalman.TransitionMatrix = ModelState.GetTransitionMatrix(ConstantVelocityProcess.TimeInterval);
//            }



        
//    }



//    public class ConstantVelocityProcess
//    {
//        public static Size WorkingArea = new Size(100, 100);
//        public static float TimeInterval = 1;

//        NormalDistribution normalDistribution = new NormalDistribution(0, 0.2);
//        Random rand = new Random();

//        ConstantVelocity2DModel initialState;
//        ConstantVelocity2DModel currentState;

//        public ConstantVelocityProcess()
//        {
//            currentState = new ConstantVelocity2DModel
//            {
//                Position = new PointF(50, 1),
//                Velocity = new PointF(0.3f, 0.3f)
//            };

//            initialState = currentState;
//        }

//        public void GoToNextState(out bool doneFullCycle)
//        {
//            Func<PointF, bool> isBorder = (point) =>
//            {
//                return point.X <= 0 || point.X >= WorkingArea.Width ||
//                       point.Y <= 0 || point.Y >= WorkingArea.Height;
//            };

//            doneFullCycle = false;
//            var prevPos = currentState.Position;
//            var speed = currentState.Velocity;

//            if (isBorder(currentState.Position))
//            {
//                var temp = speed.X;
//                speed.X = -speed.Y;
//                speed.Y = temp;

//                if (speed.Equals(initialState.Velocity)) doneFullCycle = true;
//            }

//            var nextState = new ConstantVelocity2DModel
//            {
//                Position = new PointF
//                {
//                    X = prevPos.X + speed.X * TimeInterval,
//                    Y = prevPos.Y + speed.Y * TimeInterval
//                },

//                Velocity = speed
//            };

//            currentState = nextState;
//        }

//        public ConstantVelocity2DModel GetNoisyState(double accelerationNoise)
//        {
//            var processNoiseMat = ConstantVelocity2DModel.GetProcessNoise(accelerationNoise);
//            var noise = normalDistribution.Generate(ConstantVelocity2DModel.Dimension).Multiply(processNoiseMat);

//            return new ConstantVelocity2DModel
//            {
//                Position = new PointF
//                {
//                    X = currentState.Position.X + (float)noise[0],
//                    Y = currentState.Position.Y + (float)noise[2]
//                },

//                Velocity = new PointF
//                {
//                    X = currentState.Velocity.X + (float)noise[1],
//                    Y = currentState.Velocity.Y + (float)noise[3]
//                }
//            };
//        }

//        public DotImaging.Primitives2D.PointF TryGetNoisyMeasurement(double measurementNoise, out bool isSuccess, double missingMeasurementProbability = 0.2)
//        {
//            isSuccess = rand.NextDouble() > missingMeasurementProbability;
//            if (!isSuccess)
//                return new PointF();

//            return new PointF
//            {
//                X = currentState.Position.X + (float)normalDistribution.Generate() * (float)measurementNoise,
//                Y = currentState.Position.Y + (float)normalDistribution.Generate() * (float)measurementNoise
//            };
//        }


//    }
//}
