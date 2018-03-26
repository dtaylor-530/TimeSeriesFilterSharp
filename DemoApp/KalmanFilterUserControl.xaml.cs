using Filter.Utility;
using Filter.ViewModel;
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
    /// Interaction logic for KalmanFilterUserControl.xaml
    /// </summary>
    public partial class KalmanFilterUserControl : UserControl
    {
        public KalmanFilterUserControl()
        {
            InitializeComponent();
        }




        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Unbind();


            //if (!(viewmodel is MainWindowViewModel))
            //    viewmodel = new MainWindowViewModel();
            //this.DataContext = viewmodel;
            //(this.DataContext as MainWindowViewModel).Run();


        }





        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            // Unbind();
            //if (!(this.DataContext is MainWindowViewModel))
            //    this.DataContext = viewmodel;

            //var d = viewmodel.n;
            //var tl = new List<Tuple<double, double>> { new Tuple<double, double>(1, 3) };


            //tl.AddRange(Enumerable.Range(0, d).Select(_ => new Tuple<double, double>(5, 10)));

            //tl.AddRange(Enumerable.Range(0, d).Select(_ => new Tuple<double, double>(5, 10)));

            //var GeneticAlgorithm = new GeneticAlgorithmWrapper.OuterWrap(tl.ToArray());


            //GeneticAlgorithm.Run();


            //var result = GeneticAlgorithm.Result;

            ////viewmodel.q = result[0];
            //viewmodel.r = result[1];

            ////viewmodel.Run2(result[1], result[3]);

            ////viewmodel.NotifyChanged(nameof(MainWindowViewModel.q));
            //viewmodel.NotifyChanged(nameof(MainWindowViewModel.r));

        }









        //***************************************










        private void Button3_Click(object sender, RoutedEventArgs e)
        {


            this.DataContext = new AccordKalmanFilterViewModel();


            (this.DataContext as AccordKalmanFilterViewModel).Run();


        }






        //public void Unbind()
        //{

        //    mseries.ItemsSource = null;
        //    leseries.ItemsSource = null;
        //    aeseries.ItemsSource = null;
        //    //BindingOperations.ClearBinding(mseries, ScatterSeries.ItemsSourceProperty);
        //    //BindingOperations.ClearBinding(leseries, LineSeries.ItemsSourceProperty);
        //    //BindingOperations.ClearBinding(aeseries, AreaSeries.ItemsSourceProperty);



        //}


        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            ////Unbind();
            //if (!(this.DataContext is MainWindowViewModel))
            //    viewmodel = new MainWindowViewModel();
            //this.DataContext = viewmodel;

            //viewmodel.AddEstimate();

            //viewmodel.AddMeasurement();



        }


        private void ButtonSmooth_Click(object sender, RoutedEventArgs e)
        {
            ////Unbind();
            //if (!(this.DataContext is MainWindowViewModel))
            //    viewmodel = new MainWindowViewModel();
            //this.DataContext = viewmodel;


            //viewmodel.Smooth();





        }

        private void Button_DiscreteRecursion(object sender, RoutedEventArgs e)
        {
            ////Unbind();
            this.DataContext = new KalmanFilterViewModel();

            (this.DataContext as KalmanFilterViewModel).q = new double[] { 5, 5 };
            (this.DataContext as KalmanFilterViewModel).r = new double[] { 100 };
            (this.DataContext as KalmanFilterViewModel).RunRecursive();
        }





        public void Button_Discrete(object sender, RoutedEventArgs e)
        {

            this.DataContext = new KalmanFilterViewModel();

            (this.DataContext as KalmanFilterViewModel).q = new double[] { 5, 5 };
            (this.DataContext as KalmanFilterViewModel).r = new double[] { 100 };
            (this.DataContext as KalmanFilterViewModel).Run();


        }


        public void Button_DiscreteEnsemble(object sender, RoutedEventArgs e)
        {

            this.DataContext = new KalmanFilterViewModel();

            (this.DataContext as KalmanFilterViewModel).RunEnsemble();


        }








        private void Button_DiscreteSmooth(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is KalmanFilterViewModel))
                return;

            (this.DataContext as KalmanFilterViewModel).Smooth();
        }




        private void Button_DiscreteDelayed(object sender, RoutedEventArgs e)
        {
            this.DataContext = new KalmanFilterViewModel();

            (this.DataContext as KalmanFilterViewModel).q = new double[] { 5, 5 };
            (this.DataContext as KalmanFilterViewModel).r = new double[] { 100 };

            (this.DataContext as KalmanFilterViewModel).RunDelayed(null,true);
        }




        private void Button_DiscreteFile(object sender, RoutedEventArgs e)
        {
            var x =Filter. Utility.CSVParser.Parse();
            var d = x.Select(_ => _.Item1).Min();
            var y = x.ToArrays();

            this.DataContext = new KalmanFilterViewModel();

            (this.DataContext as KalmanFilterViewModel).q = new double[] { 5, 5 };
            (this.DataContext as KalmanFilterViewModel).r = new double[] { 100 };

            (this.DataContext as KalmanFilterViewModel).RunDelayed(y);


        }






        //public void Button_DiscreteRecursion(object sender, RoutedEventArgs e)
        //{
        //    if (!(viewmodel is MainWindowViewModel))
        //        viewmodel = new MainWindowViewModel();
        //    this.DataContext = viewmodel;
        //    (this.DataContext as MainWindowViewModel).RunRecursion2();

        //}




    }
}
