using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace NmodSlave
{
    public abstract class ObservableModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            RaisePropertyChanged((propertyExpression.Body as MemberExpression).Member.Name);
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetValue<T>(ref T field, T val, [CallerMemberName] string propertyName = null, Action onValueChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, val))
                return;

            field = val;
            RaisePropertyChanged(propertyName);
            if (onValueChanged != null)
                onValueChanged();
        }
    }
}