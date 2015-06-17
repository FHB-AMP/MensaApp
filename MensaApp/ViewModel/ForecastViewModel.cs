using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MensaApp.ViewModel
{
    class ForecastViewModel : INotifyPropertyChanged
    {
        public ForecastViewModel()
        {
            this.Days = new ObservableCollection<DayViewModel>();
        }

        public ForecastViewModel(ObservableCollection<DayViewModel> days)
        {
            this.Days = days;
        }

        /// <summary>
        /// 
        /// </summary>
        private ObservableCollection<DayViewModel> _days;
        public ObservableCollection<DayViewModel> Days
        {
            get { return _days; }
            set { this.SetProperty(ref this._days, value); }
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
