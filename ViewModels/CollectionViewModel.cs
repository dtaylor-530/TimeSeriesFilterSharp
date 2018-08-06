using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;



namespace Filter.ViewModel
{
    public class CollectionViewModel<T>
    {

        public ObservableCollection<T> Items { get; set; } = new ObservableCollection<T>();

        public CollectionViewModel(IEnumerable<T> buttondefinitions)
        {
            foreach (var bd in buttondefinitions)
            {
                Items.Add(bd);
            }

        }


    }
}
