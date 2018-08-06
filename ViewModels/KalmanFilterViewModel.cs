using Filter.Model;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityWpf.ViewModel;

namespace Filter.ViewModel
{
    public class KalmanFilterViewModel : OutputViewModel<TwoVariableInput> //      OutputViewModel<IFilterWrapper>
    {

        public ReactiveProperty<double> Q0 { get; } = new ReactiveProperty<double>(1d);
        public ReactiveProperty<double> Q1 { get; } = new ReactiveProperty<double>(1d);
        public ReactiveProperty<double> R { get; } = new ReactiveProperty<double>(1d);



        public KalmanFilterViewModel()
        {

            Output = Q0
               .CombineLatest(Q1, (a, b) => new double[] { a, b })
               .CombineLatest(R, (c, d) =>  new TwoVariableInput(a:c[0],b:d,f:typeof(KalmanFilter.Wrap.DiscreteOuterWrapper)))
               .ToReactiveProperty();

        }

    }





}
