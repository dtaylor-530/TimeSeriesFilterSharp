

using Filter.Common;
using Filter.ViewModel;
using GaussianProcess;
using GaussianProcess.Wrap;
using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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

using System.Windows.Threading;


namespace TimeSeriesFilter.View
{
    /// <summary>
    /// Interaction logic for GaussianProcessUserControl.xaml
    /// </summary>
    public partial class GaussianProcessUserControl : UserControl
    {
        private MasterGPViewModel mvm;

        public GaussianProcessUserControl()
        {
            InitializeComponent();
            mvm = new MasterGPViewModel();
            //this.DataContextChanged += GaussianProcessUserControl_DataContextChanged;
            this.DataContext = mvm;
          
        }

  
        public class MasterGPViewModel:INotifyPropertyChanged
        {

            public GPViewModelStatic gpvm1 { get; set; } = new GPViewModelStatic();
            public GPViewModel2 gpvm2 { get; set; } = new GPViewModel2();
        

            #region INotifyPropertyChanged Implementation
            /// <summary>
            /// Occurs when any properties are changed on this object.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;


            /// <summary>
            /// A helper method that raises the PropertyChanged event for a property.
            /// </summary>
            /// <param name="propertyNames">The names of the properties that changed.</param>
            public virtual void NotifyChanged(params string[] propertyNames)
            {
                foreach (string name in propertyNames)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                }
            }


            protected void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string caller = "")
            {

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));

            }

 
            #endregion




        }





        //******* Gaussian Process ********



        private async void ButtonSample_Click(object sender, RoutedEventArgs e)
        {


            for (int i = 0; i < 10; i++)
            {
                mvm.gpvm2.Sample();
                await Task.Run(() => System.Threading.Thread.Sleep(1000));
            }


        }





        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {

            mvm.gpvm1.Add();


        }



        private void ButtonRun_Click(object sender, RoutedEventArgs e)
        {


            mvm.gpvm1.Add(20);



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




        //private GaussianProcessWrapper Make()
        //{


        //}





    }


   
}
