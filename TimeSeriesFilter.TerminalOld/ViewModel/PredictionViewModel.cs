using FilterSharp.Model;
using FilterSharp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace FilterSharp.ViewModel
{
    public  class PredictionViewModel : ReactiveObject
    {


        [PropertyTools.DataAnnotations.Browsable(false)]
        public ObservableCollection<KeyValuePair<DateTime, double>> Measurements { get; set; } = new ObservableCollection<KeyValuePair<DateTime, double>>();
        [PropertyTools.DataAnnotations.Browsable(false)]
        public ObservableCollection<Estimate> Estimates { get; set; } = new ObservableCollection<Estimate>();

    }
}
