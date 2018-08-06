using Filter.Model;
using GaussianProcess;
using GaussianProcess.Wrap;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityWpf.ViewModel;

namespace Filter.ViewModel
{
    public class GaussianProcessViewModel : OutputViewModel<TwoVariableInput> //OutputViewModel<IFilterWrapper>
    {
        public ReactiveProperty<Type> Kernel { get; }
        public ReactiveProperty<double> Length { get; } = new ReactiveProperty<double>(1d);
        public ReactiveProperty<double> Noise { get; } = new ReactiveProperty<double>(1d);

        public IEnumerable<Type> Kernels { get; set; }


        public GaussianProcessViewModel(IEnumerable< Type> kernels)
        {
            Kernel = new ReactiveProperty<Type>(kernels.First());

            Kernels = kernels;

            Output = Kernel.Where(_ => _ != null)

                  .CombineLatest(Length, (a, b) => new { Kernel = a, Length = b })
                  .CombineLatest(Noise, (c, d) =>
                 new TwoVariableInput(a: c.Length, b:c.Length, f: typeof(GaussianProcessWrapper)))
               .ToReactiveProperty();


        }



   
    }






}
