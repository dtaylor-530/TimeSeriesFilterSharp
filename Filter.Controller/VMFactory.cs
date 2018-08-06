using GaussianProcess.Wrap;
using KalmanFilter.Wrap;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Filter.Model;
using UtilityWpf.ViewModel;
using System.Reactive.Subjects;
using Filter.ViewModel;


namespace Filter.Controller
{

    public class VMFactory
    {

    




        public static IEnumerable<ButtonDefinition> BuildButtons(ISubject<IEnumerable<KeyValuePair<DateTime, double>>> x)
        {
            Action<IEnumerable<KeyValuePair<DateTime, double>>> av = (a) => x.OnNext(a);

            return ButtonDefinitionFactory.Build(av, typeof(TimeSeries.Service.SignalDefaultGenerator));
        }





        //public static IObservable<CollectionViewModel<KeyValuePair<DateTime, double>>> BuildTimeValueSeriesViewModel(IObservable<IEnumerable<KeyValuePair<DateTime, double>>> s)
        //{
        //    return s.Select(_ =>            new CollectionViewModel<KeyValuePair<DateTime, double>>(_));

        //}


        public static SelectableCollectionViewModel<Type> SelectionViewModelService(IScheduler s)
        {
            var types = Filter.Common.TypeHelper.GetInheritingTypes(typeof(Filter.Model.IFilterWrapper));

            return new SelectableCollectionViewModel<Type>(types.ToObservable(), s);

        }



        //public static IObservable<IOutputViewModel<IFilterWrapper>> BuildProcessViewModel(IObservable<Type> s)
        //{


        //    return s.Select(_ => ViewModelSelector(_))
        //         .Where(_ => _ != null);

        //}


        public static IOutputViewModel<TwoVariableInput> ViewModelSelector(Type type)
        {
            IEnumerable<Type> kernels = null;

            switch (type.Name)
            {
                default:
                case (nameof(GaussianProcessWrapper)):
                    {
                        kernels = kernels?? GaussianProcess.KernelHelper.LoadKernels();
                        return new GaussianProcessViewModel(kernels);
                    }

                case (nameof(DiscreteOuterWrapper)):
                    {

                        return new KalmanFilterViewModel();
                    }

            }


        }





    }

}
