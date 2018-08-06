using Filter.Model;
using Filter.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter.ViewModel
{
    public  class PredictionViewModel : INPCBase
    {


        [PropertyTools.DataAnnotations.Browsable(false)]
        public ObservableCollection<KeyValuePair<DateTime, double>> Measurements { get; set; } = new ObservableCollection<KeyValuePair<DateTime, double>>();
        [PropertyTools.DataAnnotations.Browsable(false)]
        public ObservableCollection<Estimate> Estimates { get; set; } = new ObservableCollection<Estimate>();

    }
}
