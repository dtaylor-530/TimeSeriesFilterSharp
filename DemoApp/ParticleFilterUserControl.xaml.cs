using Filter.Utility;
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

namespace DemoApp
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

            x.BatchRun(null, 100, Tuple.Create(-10, 10));


            this.DataContext = x;
        }




        private void Button_FromFile(object sender, RoutedEventArgs e)
        {
            var x = Filter.Utility.CSVParser.Parse();
            var d = x.Select(_ => _.Item1).Min();
            var y = x.Select(_=>new Tuple<DateTime,double>(_.Item1,_.Item2-60)).ToPoints();


            var f = new Filter.ViewModel.ParticleFilterViewModel(100, Tuple.Create(0, 0), Tuple.Create(0, 0));

            f.BatchRun(y, 100, Tuple.Create(0, 20));


            this.DataContext = f;


        }


    }
}
