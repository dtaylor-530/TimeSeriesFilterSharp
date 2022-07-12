using FilterSharp.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace TimeSeriesFilterSharp.View
{
    /// <summary>
    /// Interaction logic for GaussianProcessUserControl.xaml
    /// </summary>
    public partial class GaussianProcessUserControl : UserControl
    {
        private GPViewModel2 mvm;

        public GaussianProcessUserControl()
        {
            InitializeComponent();
            mvm = new GPViewModel2();
            Secondary.DataContext = mvm;
            //this.DataContextChanged += GaussianProcessUserControl_DataContextChanged;
            //this.DataContext = mvm;
        }

        //******* Gaussian Process ********

        private async void ButtonSample_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                mvm.Sample();
                //await Task.Run(() => System.Threading.Thread.Sleep(1000));
            }
        }

        private void ButtonRunDynamic_Click(object sender, RoutedEventArgs e)
        {
            //public void Initialise(double a, double b)
            //{
            //    GaussianProcess = new GP2(new ExponentiatedQuadratic(), a, b);

            //}

            //return new AccordGenetic.Wrapper.TimeSeries2DOptimisation<IEnumerable<KeyValuePair<DateTime, double>>, IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>>>
            //     ((ss) => (a, b) => TypeHelper.GetInstance<IFilterWrapper>(t, a, b).BatchRun(ss), e, ErrorHelper.GetErrorSum)
            //     .GetSubjectAsObservable();
        }
    }
}