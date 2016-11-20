using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Moral.Model
{
    class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool SetProperty<T>(ref T field, T value,
                    [CallerMemberName]string propertyName = null)
        {
            //if (Equals(field, value)) { return false; }
            field = value;
            //var handler = this.PropertyChanged;
            //if (handler != null) { handler(this, new PropertyChangedEventArgs(propertyName)); }
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }   
}
