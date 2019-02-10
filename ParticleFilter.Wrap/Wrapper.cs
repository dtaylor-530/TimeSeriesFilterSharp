using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using MathNet.Numerics.Distributions;
using System.Reactive.Linq;
using UtilityHelper;
using FilterSharp.Common;


// Based On
//https://github.com/rlabbe/Kalman-and-Bayesian-FilterSharps-in-Python/blob/master/12-Particle-FilterSharps.ipynb

namespace ParticleFilterSharp.Wrap
{
    public class ParticleFilterSharpWrapper : FilterSharp.Model.IFilterWrapper
    {

        public int N { get; set; } = 1000;

        IReSampler particleSampler = new DefaultReSampler();

        public double EffectiveCountMinRatio { get; set; } = 0.0001;// Double.Epsilon;

        int min = 0;
        int max = 1;

        public IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> BatchRun(IEnumerable<KeyValuePair<DateTime, double>> meas)
        {

            ParticleFilterSharp1D FilterSharp = new ParticleFilterSharp1D();


            FilterSharp.Particles = ParticleFactory.BuildSwarm(N, new int[] { min, max }).ToList();


            DateTime dt = meas.First().Key;

            return meas.Select(_ =>
            {
                TimeSpan ticks = (_.Key - dt);

                // move based on last measurement
                FilterSharp.Predict(EffectiveCountMinRatio, ticks);

                var prd = new KeyValuePair<DateTime, Tuple<double, double>[]>(
                    dt, FilterSharp.Particles.Select(__ => Tuple.Create(__.Y, __.Weight)).ToArray());


                // incorporate measurement
                FilterSharp.Update(new Point(0, _.Value));

                dt = _.Key;

                return prd;

            });

        }




        public IObservable<KeyValuePair<DateTime, Tuple<double, double>[]>> Run(IObservable<KeyValuePair<DateTime, double?>> meas)
        {

            ParticleFilterSharp1D FilterSharp = new ParticleFilterSharp1D();


            FilterSharp.Particles = ParticleFactory.BuildSwarm(N, new int[] { min, max }).ToList();


            return meas.IncrementalTimeOffsets().Select(_ =>
            {

                // move based on last measurement
                FilterSharp.Predict(EffectiveCountMinRatio, _.Key.Item2);

                var prd = new KeyValuePair<DateTime, Tuple<double, double>[]>(
                    _.Key.Item1, FilterSharp.Particles.Select(__ => Tuple.Create(__.Y, __.Weight)).ToArray());


                // incorporate measurement
                if (_.Value != null)
                    FilterSharp.Update(new Point(0, (double)_.Value));


                return prd;

            });

        }





        public IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> BatchRun1D(IEnumerable<KeyValuePair<DateTime, Point>> meas)
        {

            ParticleFilterSharp1D FilterSharp = new ParticleFilterSharp1D();


            FilterSharp.Particles = ParticleFactory.BuildSwarm(N, new int[] { min, max }).ToList();


            DateTime dt = meas.First().Key;

           foreach(var m in meas)
            { 
    
                TimeSpan ticks = (m.Key - dt);

                // move based on last measurement
                FilterSharp.Predict(EffectiveCountMinRatio, ticks);

                var prd = new KeyValuePair<DateTime, Tuple<double, double>[]>(
                    m.Key, FilterSharp.Particles.Select(__ => Tuple.Create(__.Y, __.Weight)).ToArray());


                // incorporate measurement
                if (m.Value != default(Point))
                    FilterSharp.Update(m.Value);

                dt = m.Key;

                yield return prd;

            };

        }







        public IEnumerable<KeyValuePair<DateTime, Tuple<Normal, Normal>>> BatchRun(List<Tuple<DateTime, Point>> meas, Point initialPoint)
        {

            ParticleFilterSharp2D FilterSharp = new ParticleFilterSharp2D();

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
            FilterSharp.Particles = ParticleFactory.BuildSwarm(N, xDistribution, yDistribution, vDistribution, oDistribution).ToList();



            DateTime dt = meas[0].Item1.AddTicks(-(long)meas.Select(_ => (double)_.Item1.Ticks).ToArray().SelectDifferences().Average());



            for (int i = 0; i < meas.Count(); i++)
            {
                long ticks = (meas[i].Item1 - dt).Ticks;

                // move based on last measurement

                FilterSharp.Predict(EffectiveCountMinRatio);
                var est = ParticleHelper.Estimate(FilterSharp.Particles);
                yield return new KeyValuePair<DateTime, Tuple<Normal, Normal>>(dt, est);
                // predictions.Add(new KeyValuePair<DateTime, List<Point>>(dt, particles.Select(_=>(Point)(_)).ToList()));


                // incorporate measurements
                FilterSharp.Update(meas[i].Item2);


                dt = meas[i].Item1;

            }


        }








    }




    public static class ParticleFilterSharpHelper
        {

        public static Tuple<double, double> ToWeightedEstimate(IEnumerable<Tuple<double, double>> prediction)
        {

            return Tuple.Create(prediction.Average(__ => __.Item1 * __.Item2), prediction.Average(__ => __.Item2));



        }


        public static Tuple<double, double> ToWeightedEstimate(IEnumerable<(double, double)> prediction)
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