using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Filter.Utility;
using System.Windows;
using MathNet.Numerics.Distributions;


// Base On
//https://github.com/rlabbe/Kalman-and-Bayesian-Filters-in-Python/blob/master/12-Particle-Filters.ipynb

namespace ParticleFilter.Wrap
{
    public class Wrapper
    {



        public int N { get; set; } = 1000;


        //Point xlim = new Point(0, 20);
        //Point ylim = new Point(0, 20);

        double? initial_x = null;
        //Point[] landmarks = new Point[] { new Point(-1, 2), new Point(5, 10), new Point(12, 14), new Point(18, 21) };

        IReSampler particleSampler = new DefaultReSampler();

        public double EffectiveCountMinRatio { get; set; } = 0.0001;// Double.Epsilon;


        public IEnumerable<KeyValuePair<DateTime, List<Tuple<double, double>>>> BatchRun1D(List<Tuple<DateTime, Point>> meas, int min, int max)
        {

            ParticleFilter1D filter = new ParticleFilter1D();


            filter.Particles = ParticleFactory.BuildSwarm(N, new int[] { min, max }).ToList();
            //filter.Particles.ForEach(_=>_.Orientation=0.5)

            DateTime dt = meas[0].Item1;//.AddTicks(-(long)meas.Select(_ => (double)_.Item1.Ticks).ToArray().SelectDifferences().Average());
                                        // IList<KeyValuePair<DateTime, Tuple<Normal, Normal>>> estimates = new List<KeyValuePair<DateTime, Tuple<Normal, Normal>>>();
            IList<KeyValuePair<DateTime, List<Point>>> predictions = new List<KeyValuePair<DateTime, List<Point>>>();

            //foreach (var _ in meas)
            //{
            return meas.Select(_ =>
            {
                TimeSpan ticks = (_.Item1 - dt);

               // move based on last measurement
               filter.Predict(EffectiveCountMinRatio, ticks);

                var prd = new KeyValuePair<DateTime, List<Tuple<double, double>>>(
                    dt, filter.Particles.Select(__ => Tuple.Create(__.Y, __.Weight)).ToList());


    //            var prd2 = new KeyValuePair<DateTime, List<List<(double, double)>>>(
    //dt, filter.Particles.Select(__ => new List<(double, double)> { (__.Y, __.Weight)/*, (__.YVelocity(), __.Weight)*/ }).ToList());

    //            var xxx = prd2.Value.Select(_d => _d[0].Item2).Sum() - prd.Value.Select(_D => _D.Item2).Sum();


               // incorporate measurements
                filter.Update(_.Item2);

                dt = _.Item1;

                return prd;

            }).ToList();

        }


        
        //     foreach(var _ in meas)
        //    {
        //        TimeSpan ticks = (_.Item1 - dt);

        //// move based on last measurement
        //filter.Predict(EffectiveCountMinRatio, ticks);

            

        //        // incorporate measurements
        //        filter.Update(_.Item2);

        //        dt = _.Item1;

        //        yield return prd;
        //    }






    public IList<KeyValuePair<DateTime, Tuple<Normal, Normal>>> BatchRun(List<Tuple<DateTime, Point>> meas, Point initialPoint)
        {

            ParticleFilter2D filter = new ParticleFilter2D();

            IContinuousDistribution xDistribution = null;
            IContinuousDistribution yDistribution = null;
            IContinuousDistribution oDistribution = null;
            IContinuousDistribution vDistribution = null;


            if (initialPoint == null)
            {
                xDistribution = new ContinuousUniform(-10, 10);
                yDistribution = new ContinuousUniform(-10, 10);
                oDistribution = new ContinuousUniform(0, 2 * Math.PI);
                vDistribution = new ContinuousUniform(0.000001, 0.000001);
            }
            filter.Particles = ParticleFactory.BuildSwarm(N, xDistribution, yDistribution, vDistribution, oDistribution).ToList();



            DateTime dt = meas[0].Item1.AddTicks(-(long)meas.Select(_ => (double)_.Item1.Ticks).ToArray().SelectDifferences().Average());
            IList<KeyValuePair<DateTime, Tuple<Normal, Normal>>> estimates = new List<KeyValuePair<DateTime, Tuple<Normal, Normal>>>();
            IList<KeyValuePair<DateTime, List<Point>>> predictions = new List<KeyValuePair<DateTime, List<Point>>>();


            for (int i = 0; i < meas.Count(); i++)
            {
                long ticks = (meas[i].Item1 - dt).Ticks;

                // move based on last measurement
                try
                {
                    //filter.Predict(ref particles, ticks);
                    filter.Predict(EffectiveCountMinRatio);
                    var est = ParticleFilterHelper.Estimate(filter.Particles);
                    estimates.Add(new KeyValuePair<DateTime, Tuple<Normal, Normal>>(dt, est));
                    // predictions.Add(new KeyValuePair<DateTime, List<Point>>(dt, particles.Select(_=>(Point)(_)).ToList()));
                }
                catch (Exception e) { }

                // incorporate measurements
                filter.Update(meas[i].Item2);



                dt = meas[i].Item1;

            }

            //return predictions;
            return estimates;
        }





        public static Tuple<double,double> ToWeightedEstimate(List<Tuple<double,double>> prediction)
        {

            return Tuple.Create(prediction.Average(__ => __.Item1 * __.Item2), prediction.Average(__ =>  __.Item2));



        }


        public static Tuple<double, double> ToWeightedEstimate(List<(double, double)> prediction)
        {

            return Tuple.Create(prediction.Average(__ => __.Item1 * __.Item2), prediction.Average(__ => __.Item2));



        }




    }

}








//def resample_from_index(particles, weights, indexes):
//    particles[:] = particles[indexes]
//    weights[:] = weights[indexes]
//    weights.fill(1.0 / len(weights))




//def create_uniform_particles(x_range, y_range, hdg_range, N):
//    particles = np.empty((N, 3))
//    particles[:, 0] = uniform(x_range[0], x_range[1], size= N)
//    particles[:, 1] = uniform(y_range[0], y_range[1], size= N)
//    particles[:, 2] = uniform(hdg_range[0], hdg_range[1], size= N)
//    particles[:, 2] %= 2 * np.pi
//    return particles

//def create_gaussian_particles(mean, std, N):
//    particles = np.empty((N, 3))
//    particles[:, 0] = mean[0] + (randn(N) * std[0])
//    particles[:, 1] = mean[1] + (randn(N) * std[1])
//    particles[:, 2] = mean[2] + (randn(N) * std[2])
//    particles[:, 2] %= 2 * np.pi
//    return particles



//def predict(particles, u, std, dt= 1.):
//    """ move according to control input u (heading change, velocity)
//    with noise Q(std heading change, std velocity)`"""

//    N = len(particles)
//    # update heading
//    particles[:, 2] += u[0] + (randn(N) * std[0])
//    particles[:, 2] %= 2 * np.pi

//    # move in the (noisy) commanded direction
//    dist = (u[1] * dt) + (randn(N) * std[1])
//    particles[:, 0] += np.cos(particles[:, 2]) * dist
//    particles[:, 1] += np.sin(particles[:, 2]) * dist


//def update(likelihood, prior):
//    posterior = prior* likelihood
//    return normalize(posterior)


//def update(particles, weights, z, R, landmarks):
//    weights.fill(1.)
//    for i, landmark in enumerate(landmarks):
//        distance = np.linalg.norm(particles[:, 0:2] - landmark, axis=1)
//        weights *= scipy.stats.norm(distance, R).pdf(z[i])

//    weights += 1.e-300      # avoid round-off to zero
//    weights /= sum(weights) # normalize



//def estimate(particles, weights):
//    """returns mean and variance of the weighted particles"""

//    pos = particles[:, 0:2]
//    mean = np.average(pos, weights = weights, axis = 0)
//    var  = np.average((pos - mean)**2, weights=weights, axis=0)
//    return mean, var



//def simple_resample(particles, weights):
//    N = len(particles)
//    cumulative_sum = np.cumsum(weights)
//    cumulative_sum[-1] = 1. # avoid round-off error
//    indexes = np.searchsorted(cumulative_sum, random(N))

//    # resample according to indexes
//particles[:] = particles[indexes]
//    weights.fill(1.0 / N)





//def neff(weights):
//    return 1. / np.sum(np.square(weights))




//    def run_pf1(N, iters= 18, sensor_std_err= .1,
//                do_plot= True, plot_particles= False,
//                xlim= (0, 20), ylim= (0, 20),
//                initial_x= None):
//    landmarks = np.array([[-1, 2], [5, 10], [12, 14], [18, 21]])
//    NL = len(landmarks)


//    plt.figure()

//    # create particles and weights
//    if initial_x is not None:
//        particles = create_gaussian_particles(
//            mean= initial_x, std= (5, 5, np.pi / 4), N= N)
//    else:
//        particles = create_uniform_particles((0,20), (0,20), (0, 6.28), N)
//    weights = np.zeros(N)

//    if plot_particles:
//        alpha = .20
//        if N > 5000:
//            alpha *= np.sqrt(5000)/np.sqrt(N)
//        plt.scatter(particles[:, 0], particles[:, 1],
//                    alpha= alpha, color= 'g')


//    xs = []
//    robot_pos = np.array([0., 0.])
//    for x in range(iters):
//        robot_pos += (1, 1)

//        # distance from robot to each landmark
//        zs = (norm(landmarks - robot_pos, axis=1) + 
//              (randn(NL) * sensor_std_err))

//        # move diagonally forward to (x+1, x+1)
//        predict(particles, u= (0.00, 1.414), std= (.2, .05))



//        # incorporate measurements
//        update(particles, weights, z= zs, R= sensor_std_err,
//               landmarks= landmarks)


//        mu, var = estimate(particles, weights)

//        # resample if too few effective particles
//        if neff(weights) < N/2:
//            indexes = systematic_resample(weights)
//            resample_from_index(particles, weights, indexes)



//        xs.append(mu)

//        if plot_particles:
//            plt.scatter(particles[:, 0], particles[:, 1],
//                        color='k', marker=',', s=1)
//        p1 = plt.scatter(robot_pos[0], robot_pos[1], marker='+',
//                         color='k', s=180, lw=3)
//        p2 = plt.scatter(mu[0], mu[1], marker='s', color='r')

//    xs = np.array(xs)
//# plt.plot(xs[:, 0], xs[:, 1])
//    plt.legend([p1, p2], ['Actual', 'PF'], loc= 4, numpoints= 1)
//    plt.xlim(* xlim)
//    plt.ylim(*ylim)
//    print('final position error, variance:\n\t', mu - np.array([iters, iters]), var)
//    plt.show()

//from numpy.random import seed
//seed(2)
//run_pf1(N= 5000, plot_particles= False)