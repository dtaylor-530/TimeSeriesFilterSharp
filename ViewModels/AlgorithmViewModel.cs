using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Reactive.Linq;
using GaussianProcess.Wrap;
using Filter.Model;
using UtilityWpf.ViewModel;
using DynamicData.Binding;
using System.Reactive.Subjects;


namespace Filter.ViewModel
{



    public class AlgorithmViewModel :INPCBase
    {
        private ISubject<TwoVariableInput> output;

        public IOutputViewModel<TwoVariableInput> AlgoVM { get; set; }

        public  ISubject<TwoVariableInput> Output        {            get;set;        }

        public CollectionViewModel<ButtonDefinition> SignalButtonsVM { get; set; }


        public AlgorithmViewModel(IObservable<IOutputViewModel<TwoVariableInput>> fwvms)
        {


            Output = new Subject<TwoVariableInput>();

            fwvms.Subscribe(a =>
            {
                this.AlgoVM = (a);

                NotifyChanged(nameof(AlgoVM));

                a.Output.Subscribe(_ => Output.OnNext(_));

            });
        }

    }

}
