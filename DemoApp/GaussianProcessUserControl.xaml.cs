
using Filter.ViewModel;
using MathNet.Numerics.Distributions;
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
    /// Interaction logic for GaussianProcessUserControl.xaml
    /// </summary>
    public partial class GaussianProcessUserControl : UserControl
    {
        public GaussianProcessUserControl()
        {
            InitializeComponent();
        }



        //******* Gaussian Process ********



        private async void Button2_Click(object sender, RoutedEventArgs e)
        {
            //Unbind();
            var mvm = new GPViewModel();
            this.DataContext = mvm;


            for (int i = 0; i < 10; i++)
            {

                await Task.Run(() => System.Threading.Thread.Sleep(1000));
                mvm.Sample();
            }


        }


        private void ButtonRF_Click(object sender, RoutedEventArgs e)
        {
            //Unbind();


            var x = Filter.Utility.CSVParser.Parse();
            var d = x.Select(_ => _.Item1).Min();
            var avg= x.Take(30).Average(_ => _.Item2);

            var xy = x
                .Select(_ => new Tuple<double, double>((double)_.Item2-avg, (double)(_.Item1 - d).TotalDays/10)).Take(30).ToList();




            var mvm =  new GPViewModel();
            this.DataContext = mvm;
            (mvm as GPViewModel).Run(xy.Select(_=>_.Item2).ToArray(),xy.Select(_=>_.Item1).ToArray());


        }



        private void ButtonAdd2_Click(object sender, RoutedEventArgs e)
        {
            // Unbind();
            var mvm = (this.DataContext is GPViewModel) ? this.DataContext : new GPViewModel();
            this.DataContext = mvm;
            (mvm as GPViewModel).AddRandomPoint();


        }



        private void ButtonRun_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();


            var axpts = new double[10];
            Normal.Samples(rand, axpts, 0, 2);
            var aypts = new double[10];
            Normal.Samples(rand, aypts, 0, 2);

            var mvm = (this.DataContext is GPViewModel) ? this.DataContext : new GPViewModel();
            this.DataContext = mvm;
            (mvm as GPViewModel).Run(axpts,aypts);


        }


    }
}
