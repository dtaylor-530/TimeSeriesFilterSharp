using Filter.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Filter.Model;
using UtilityWpf.ViewModel;
using Filter.Common;
using UtilityReactive;
using System.Reactive.Subjects;
using System.Reactive.Disposables;

namespace Filter.Controller
{

    public class AlgorithmController
    {

        //public SelectableCollectionViewModel<Type> Selections { get; set; }
        public IObservable<IOutputViewModel<TwoVariableInput>> AlgoViewModels { get; set; }



        public AlgorithmController(IObservable<Type> types/*IObservable<IEnumerable<KeyValuePair<DateTime, double>>> Measurements*/)
        {

            AlgoViewModels = types
                .Select(_ => VMFactory.ViewModelSelector(_))
                .Where(_ => _ != null); 

        }


    }

}
