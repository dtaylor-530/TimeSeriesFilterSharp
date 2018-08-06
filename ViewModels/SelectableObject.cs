
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
//using static BettingSystem.Model.ProfitSeries;

namespace Filter.ViewModel

{
    //public class SelectableObject<T> : INotifyPropertyChanged, ISelectable//where T : struct, IConvertible,ISelectable
    //{
    //    public T Object { get; set; }

    //    //public string Name { get { return Enum.ToString(); } }

    //    public SelectableObject(T obj)
    //    {

    //        Object = obj;

    //    }


    //    private bool isSelected;


    //    public bool IsSelected
    //    {
    //        get { return isSelected; }
    //        set
    //        {
    //            isSelected = value;
    //            NotifyChanged(nameof(IsSelected));
    //        }
    //    }

    //    public override string ToString()
    //    {
    //        return Object.ToString();
    //    }

    //    #region INotifyPropertyChanged Implementation
    //    /// <summary>
    //    /// Occurs when any properties are changed on this object.
    //    /// </summary>
    //    public event PropertyChangedEventHandler PropertyChanged;


    //    /// <summary>
    //    /// A helper method that raises the PropertyChanged event for a property.
    //    /// </summary>
    //    /// <param name="propertyNames">The names of the properties that changed.</param>
    //    public virtual void NotifyChanged(params string[] propertyNames)
    //    {
    //        foreach (string name in propertyNames)
    //        {
    //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    //        }
    //    }


    //    protected void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string caller = "")
    //    {

    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));

    //    }
    //    #endregion
    //}



    //public static class SelectableObjectHelper
    //{
    //    public static void Subscribe<T>(SelectableObject<T> s, Action a)
    //    {
    //        s.WhenValueChanged(t => t.IsSelected)
    //                        .Throttle(TimeSpan.FromMilliseconds(250))
    //                       .Subscribe(b => a());

    //    }
    //}



    public interface ISelectable
    {

        bool IsSelected { get; set; }



    }


    public class SelectableObject<T>
    {
        public T Object { get; set; }


        public SelectableObject(T obj)
        {

            Object = obj;

        }

        public ReactiveProperty<bool> IsSelected { get; set; } = new ReactiveProperty<bool>(false);


        public override string ToString()
        {
            return Object.ToString();
        }


    }



}

