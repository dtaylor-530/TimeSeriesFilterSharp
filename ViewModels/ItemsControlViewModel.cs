using DynamicData;
using DynamicData.Binding;
using Filter.Model;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter.ViewModel
{
    public class GenericViewModel<T> : AbstractNotifyPropertyChanged
    {


        private SelectableObject<T> _selectedItem;


        public SelectableObject<T> SelectedItem
        {
            get => _selectedItem;
            set => SetAndRaise(ref _selectedItem, value);
        }


        public string Title { get; private set; }


        private readonly ObservableCollection<SelectableObject<T>> _items= new ObservableCollection<SelectableObject<T>>();


        public ObservableCollection<SelectableObject<T>> Items => _items;


        public GenericViewModel(IObservable<T> observable,
            IObservable<Func<T, bool>> filter,
            IScheduler scheduler, string title = null)
        {

            observable.Select
                //.Subscribe(_=>
                //Console.WriteLine("sdffsdffffffff!"));
                //.Filter(filter)
                (s =>
                {
                    var so = new SelectableObject<T>(s);

                    return so;
                })
                .Do(so => so.IsSelected
                        .Throttle(TimeSpan.FromMilliseconds(250))
                       .Subscribe(b =>
                       {
                           this.SelectedItem = this.Items.FirstOrDefault(sof => sof.IsSelected.Value== true);
                       }))

                  .ObserveOn(scheduler)

                .Subscribe(
                _ => Items.Add(_),
                ex => Console.WriteLine("Error in generic view model"));

            Title = title;

        }


    }
    public static class helper
    {
        public static IObservable<T> GetSelectedObjectSteam<T>(this GenericViewModel<T> si)
        {
            //si.WhenValueChanged(t => t.SelectedItem).Subscribe(_ =>
            //Console.WriteLine());

            //si.WhenValueChanged(t => t.SelectedItem)
            //    .Throttle(TimeSpan.FromMilliseconds(250))
            //    .Where(_ => _ != null)
            //     .Select(_ => _.Object).Subscribe(_ =>
            //Console.WriteLine());


            return si.WhenValueChanged(t => t.SelectedItem)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Where(_ => _ != null)
                 .Select(_ => _.Object);



        }
    }
}