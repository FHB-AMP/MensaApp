using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace MensaApp.ViewModel
{
    class AllergenViewModel : INotifyPropertyChanged
    {
        private string _id;
        public string Id
        {
            get { return _id; }
            set { this.SetProperty(ref this._id, value); }
        }

        private string _definition;
        public string Definition
        {
            get { return _definition; }
            set { this.SetProperty(ref this._definition, value); }
        }

        private string _containedIn;
        public string ContainedIn
        {
            get { return _containedIn; }
            set { this.SetProperty(ref this._containedIn, value); }
        }

        // property changed logic by jump start
        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;
            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
