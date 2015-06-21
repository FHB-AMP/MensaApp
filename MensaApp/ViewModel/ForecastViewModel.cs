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
    /// <summary>
    /// ViewModel of the meals page.
    /// It contains lists of the current day and the forecast days.
    /// Is used to manipulate front end.
    /// </summary>
    class ForecastViewModel : INotifyPropertyChanged
    {
        public ForecastViewModel()
        {
            this.Today = new ObservableCollection<DayViewModel>();
            this.ForecastDays = new ObservableCollection<DayViewModel>();
        }

        public ForecastViewModel(DayViewModel today, ObservableCollection<DayViewModel> forecastDays)
        {
            this.Today = new ObservableCollection<DayViewModel>();
            this.Today.Add(today);
            this.ForecastDays = forecastDays;
        }

        /// <summary>
        /// Current day.
        /// Should be contain a single list item only.
        /// </summary>
        private ObservableCollection<DayViewModel> _today;
        public ObservableCollection<DayViewModel> Today
        {
            get { return _today; }
            set { this.SetProperty(ref this._today, value); }
        }

        /// <summary>
        /// Forecast days.
        /// Should be contain three list items.
        /// </summary>
        private ObservableCollection<DayViewModel> _forecastDays;
        public ObservableCollection<DayViewModel> ForecastDays
        {
            get { return _forecastDays; }
            set { this.SetProperty(ref this._forecastDays, value); }
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
