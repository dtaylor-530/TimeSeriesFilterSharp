using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Filter.ViewModel
{


    public class MainViewModel
    {

        public StaticViewModel DynamicVM { get; set; }

        public StaticViewModel StaticVM { get; set; }


        public MainViewModel(Dispatcher dispatcher)
        {
            var newThread = NewThreadScheduler.Default;
            var ui = (new DispatcherScheduler(dispatcher));


            StaticVM = VMFactory.BuildStatic(ui);

            //DynamicVM = VMFactory.BuildDynamic(newThread); //

        }

    }





    //public class DynamicViewModel : INPCBase
    //{

    //    public CollectionViewModel<ButtonDefinition> ButtonListVM { get; set; }

    //    public PredictionsViewModel PredictionsVM { get; set; }

    //    public TimeValueSeriesViewModel SeriesVM { get; set; }

    //    public DynamicViewModel(IScheduler scheduler)
    //    {

    //        var defs = ButtonDefinitionFactory.Build<PredictionsViewModel>((a) =>
    //        {
    //            this.PredictionsVM = a;
    //            NotifyChanged(nameof(PredictionsVM));
    //        }, typeof(Filter.ViewModel.VMDynamicFactory), scheduler);

    //        ButtonListVM = new CollectionViewModel<ButtonDefinition>(defs);

    //    }
    //}


    public class StaticViewModel : INPCBase
    {

        public CollectionViewModel<ButtonDefinition> SignalButtonsVM { get; set; }

        //public CollectionViewModel<ButtonDefinition> AlgoButtonsVM { get; set; }

        public GenericViewModel<Type> TypesVM { get; set; }

        public IFilterWrapperViewModel AlgoVM { get; set; }


        public PredictionsViewModel PredictionsVM { get; set; }

        public TimeValueSeriesViewModel SeriesVM { get; set; }


        public StaticViewModel(IEnumerable<ButtonDefinition> defs, GenericViewModel<Type> types, IObservable<TimeValueSeriesViewModel> timeValueSeriesViewModelService, IObservable<PredictionsViewModel> predictionsViewModelService,
            IObservable<IFilterWrapperViewModel> fwvms)
            
        {
            SignalButtonsVM = new CollectionViewModel<ButtonDefinition>(defs);


            //odefs.Subscribe(a =>
            //{
            //    this.AlgoButtonsVM = new CollectionViewModel<ButtonDefinition>(a);
            //    NotifyChanged(nameof(AlgoButtonsVM));
            //});
            TypesVM = types;


            fwvms.Subscribe(a =>
            {
                this.AlgoVM=(a);
                NotifyChanged(nameof(AlgoVM));
            });
            predictionsViewModelService.Subscribe(a =>
            {
                this.PredictionsVM = a;
                NotifyChanged(nameof(PredictionsVM));
            });

            timeValueSeriesViewModelService.Subscribe(a =>
            {
                this.SeriesVM = a;
                NotifyChanged(nameof(SeriesVM));
            });

            //var defs = ButtonDefinitionFactory.Build<PredictionsViewModel>((a) =>
            //{
            //    this.PredictionsVM = a;
            //    NotifyChanged(nameof(PredictionsVM));
            //}, typeof(Filter.ViewModel.VMStaticFactory));



        }
    }
}
