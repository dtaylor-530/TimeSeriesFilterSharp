
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
using Filter.Common;

namespace TimeSeriesFilter.View
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






        public void Button_Discrete(object sender, RoutedEventArgs e)
        {

            this.DataContext = new KalmanFilterViewModel();


            (this.DataContext as KalmanFilterViewModel).Run(2,52);


        }



        private void Button_DiscreteOptimised(object sender, RoutedEventArgs e)
        {

            this.DataContext = new KalmanFilterViewModel();
            (this.DataContext as KalmanFilterViewModel).RunOptimised();
        }



        private void Button_DiscreteDelayed(object sender, RoutedEventArgs e)
        {
            this.DataContext = new KalmanFilterViewModel();
            (this.DataContext as KalmanFilterViewModel).RunDelayed(2,52);
        }



        private void Button_DiscreteSmooth(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is KalmanFilterViewModel))
                return;

            (this.DataContext as KalmanFilterViewModel).Smooth();
        }







    }
}
