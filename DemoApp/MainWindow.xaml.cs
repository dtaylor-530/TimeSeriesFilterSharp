using GeneticSharp.Domain;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        MainWindowViewModel viewmodel;

        public MainWindow()
        {
            InitializeComponent();
            viewmodel = new MainWindowViewModel();
            this.DataContext = viewmodel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel))
                this.DataContext= new MainWindowViewModel();
         
            (this.DataContext as MainWindowViewModel).Run();


        }





        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel))
                this.DataContext = viewmodel;

            var d = viewmodel.n;
            var tl = new List<Tuple<double, double>> { new Tuple<double, double>(1, 3) };


            tl.AddRange(Enumerable.Range(0, d).Select(_ => new Tuple<double, double>(0, 1000)));

            tl.AddRange(Enumerable.Range(0, d).Select(_ => new Tuple<double, double>(0, 1000)));

            var GeneticAlgorithm = new  GeneticAlgorithmWrapper.OuterWrap(tl.ToArray());


            GeneticAlgorithm.Run();


            var result = GeneticAlgorithm.Result;

            //viewmodel.q = result[0];
            viewmodel.r = result[1];

            viewmodel.Run2(result[0] / 100, result[1] / 100);

            //viewmodel.NotifyChanged(nameof(MainWindowViewModel.q));
            viewmodel.NotifyChanged(nameof(MainWindowViewModel.r));

        }




        private void Button2_Click(object sender, RoutedEventArgs e)
        {

            var mvm = new MainWindowViewModel2();
            this.DataContext = mvm;
            mvm.Run();


        }


        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {

            if (!(this.DataContext is MainWindowViewModel))
                viewmodel = new MainWindowViewModel();
            this.DataContext = viewmodel;

            viewmodel.AddEstimate();

          viewmodel.AddMeasurement();
        


        }
    }
}
