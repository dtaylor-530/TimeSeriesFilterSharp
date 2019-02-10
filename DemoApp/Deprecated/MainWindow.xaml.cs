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
using OxyPlot;
using OxyPlot.Wpf;
using System.Timers;

using Filter.ViewModel;
using Filter.Service;
using UtilityWpf.ViewModel;

namespace TimeSeriesFilter.Terminal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        MainViewModel viewmodel;

        public MainWindow()
        {
            InitializeComponent();
            var dis = Application.Current.Dispatcher;
            var ds = new DispatcherService(dis);

            viewmodel = new MainViewModel( ds,dis);
            this.DataContext = viewmodel;
        }

   







    }
}
