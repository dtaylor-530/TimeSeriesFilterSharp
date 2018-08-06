using DynamicData.Binding;
using Filter.ViewModel;
using Filter.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using UtilityWpf.ViewModel;
using Filter.Service;
using UtilityReactive;
using Filter.Model;
using System.Windows.Media.Media3D;
using UtilityEnum;
using System.Windows;

namespace TimeSeriesFilter.Terminal
{
    public class MainViewModel: INPCBase
    {
        public LiveTableViewModel<Point3D> LiveSortableVM { get; private set; }

        //public OutputViewModel DynamicVM { get; set; }

        public PredictionViewModel OutputVM { get; set; }

        public AlgorithmViewModel InputVM { get; set; }
        public CollectionViewModel<ButtonDefinition> SignalButtonsVM { get; set; }

        public OutputViewModel<Flux> FluxInputVM { get; set; }

        public OutputViewModel<bool> OptimisationInputVM { get; set; }
        public SelectableCollectionViewModel<Type> TypesVM { get; }



        public MainViewModel( DispatcherService ds)
        {

            var tss = new TimeSeriesService();

            TimeSeriesController tsc = new TimeSeriesController(tss.Measurements);

            TypesVM = VMFactory.SelectionViewModelService(ds.UI);

            SignalButtonsVM = new CollectionViewModel<ButtonDefinition>(tsc.Buttons,Application.Current.Dispatcher);

            var t = TypesVM.Output.BufferUntilInactive().Where(a => a != null);

            AlgorithmController c = new AlgorithmController(t);

            InputVM = new AlgorithmViewModel( c.AlgoViewModels);

            FluxInputVM = new ToggleViewModel<Flux>();
        
            OptimisationInputVM = new ToggleViewModel("Optimisation");



            FluxInputVM.Output.DistinctUntilChanged().Subscribe(_ =>
            {
                IPredictionService prrds = null;
                PredictionsController prds = null;
                if (_ == Flux.Static)
                {
                     prrds = new PredictionsService(InputVM.Output.BufferUntilInactive(), tss.Measurements, OptimisationInputVM.Output, t, ds.Background);
                     prds = new PredictionsController((prrds as PredictionsService).Predictions, ds.UI);
                }

                else if (_ == Flux.Dynamic)
                { 
                     prrds = new DynamicPredictionService(InputVM.Output.BufferUntilInactive(), tss.Measurements, OptimisationInputVM.Output, t, ds.Background);
                     prds = new PredictionsController((prrds as DynamicPredictionService).Predictions, ds.UI);
                }

           
                LiveSortableVM = new LiveTableViewModel<Point3D>(prrds.Solutions, nameof(Point3D.Z));
                OutputVM = new PredictionViewModel(tsc.TimeValueSeries, prds.PositionPredictionsVM, prds.VelocityPredictionsVM /*InputVM.Output, tsc.Measurements*/);
                NotifyChanged(nameof(LiveSortableVM));
                NotifyChanged(nameof(OutputVM));
            });
        }

    }
}
