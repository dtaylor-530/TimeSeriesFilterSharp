using Filter.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot.Wpf;

namespace Filter.ViewModel
{
    public class MeasurementsEstimates : INPCBase
    {

        [PropertyTools.DataAnnotations.Browsable(false)]
        public ObservableCollection<Measurement> Measurements { get; set; }
        [PropertyTools.DataAnnotations.Browsable(false)]
        public ObservableCollection<Measurement> Estimates { get; set; }

        [PropertyTools.DataAnnotations.Browsable(false)]
        public ObservableCollection<Measurement> SmoothedEstimates { get; set; }

        [PropertyTools.DataAnnotations.Browsable(false)]
        public OxyPlot.PlotModel PlotModel { get; set; }

 


        public void n()
        {


            NotifyChanged(nameof(Estimates), nameof(SmoothedEstimates),nameof(PlotModel));

        }
    }
}
