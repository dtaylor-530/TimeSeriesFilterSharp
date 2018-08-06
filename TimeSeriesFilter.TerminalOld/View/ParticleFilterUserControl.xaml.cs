
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Filter.Common;
using Filter.ViewModel;

namespace TimeSeriesFilter.View
{
    /// <summary>
    /// Interaction logic for ParticleFilterUserControl.xaml
    /// </summary>
    public partial class ParticleFilterUserControl : UserControl
    {
        public ParticleFilterUserControl()
        {
            InitializeComponent();
        }

        private void Button_BatchRun(object sender, RoutedEventArgs e)
        {

            var x = new Filter.ViewModel.ParticleFilterViewModel(100, Tuple.Create(0, 0), Tuple.Create(0, 0));
            var dt = new DateTime(1000000000000000);
            x.BatchRun(SignalGenerator.GetPeriodic(dt,100).Select(_=>new KeyValuePair<DateTime,Point>(_.Key,new Point(0,_.Value))), 100, Tuple.Create(-10, 10));


            this.DataContext = x;
        }




        //private void Button_FromFile(object sender, RoutedEventArgs e)
        //{

        //    var f = new Filter.ViewModel.ParticleFilterViewModel(100, Tuple.Create(0, 0), Tuple.Create(0, 0));

        //    f.BatchRun( DataHelper.GetAlterededAppleData(), 100, Tuple.Create(0, 20));

        //    this.DataContext = f;


        //}


    }
}
