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
using Filter.ViewModel;
using System.Reactive.Concurrency;

namespace TimeSeriesFilter.View
{
    /// <summary>
    /// Interaction logic for OptimisationUserControl.xaml
    /// </summary>
    public partial class OptimisationUserControl : UserControl
    {

        OptimisationViewModel vm;
        public OptimisationUserControl()
        {
            var signal = SignalGenerator.GetPeriodic(new DateTime(10000000), 30);

            InitializeComponent();
            vm= new OptimisationViewModel(signal.ToList());
     
            this.DataContext = vm;

        }


        private void ButtonRun_Click(object sender, RoutedEventArgs e)
        {
            //vm.Run2(DispatcherScheduler.Current);
            vm.Run(DispatcherScheduler.Current);

        }

        private void ButtonRunTest_Click(object sender, RoutedEventArgs e)
        {
            vm.RunTest(DispatcherScheduler.Current);
        }

        private void ButtonRunKalmanFilterTest_Click(object sender, RoutedEventArgs e)
        {
            vm.RunTest2(DispatcherScheduler.Current);
        }
    }
}
