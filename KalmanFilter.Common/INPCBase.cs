﻿//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Filter.ViewModel
//{

//        public abstract class INPCBase : INotifyPropertyChanged
//        {
//            #region INotifyPropertyChanged Implementation
//            /// <summary>
//            /// Occurs when any properties are changed on this object.
//            /// </summary>
//            public event PropertyChangedEventHandler PropertyChanged;


//            /// <summary>
//            /// A helper method that raises the PropertyChanged event for a property.
//            /// </summary>
//            /// <param name="propertyNames">The names of the properties that changed.</param>
//            public virtual void NotifyChanged(params string[] propertyNames)
//            {
//                foreach (string name in propertyNames)
//                {
//                    OnPropertyChanged(new PropertyChangedEventArgs(name));
//                }
//            }

//            /// <summary>
//            /// Raises the PropertyChanged event.
//            /// </summary>
//            /// <param name="e">Event arguments.</param>
//            protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
//            {
//            this.PropertyChanged?.Invoke(this, e);
//        }
//            #endregion
//        }

    
//}
