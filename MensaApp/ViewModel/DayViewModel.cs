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
    /// ViewModel of a day. 
    /// It contains the date of day and the meals which are available at that certain day
    /// Is used to manipulate front end.
    /// </summary>
    class DayViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Constructor to create a day with the current date and an empty list of meals.
        /// </summary>
        public DayViewModel()
        {
            this.Date = DateTime.Now;
            this.Meals = new ObservableCollection<MealViewModel>();
        }

        /// <summary>
        /// Constructor to create a day with a date and an empty list of meals.
        /// </summary>
        /// <param name="date"></param>
        public DayViewModel(DateTime date)
        {
            this.Date = date;
            this.Meals = new ObservableCollection<MealViewModel>();
        }

        /// <summary>
        /// Constructor create a day with date and a list of meals.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="meals"></param>
        public DayViewModel(DateTime date, ObservableCollection<MealViewModel> meals)
        {
            this.Date = date;
            this.Meals = meals;
        }

        /// <summary>
        /// Date of a certain day.
        /// </summary>
        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set { this.SetProperty(ref this._date, value); }
        }

        /// <summary>
        /// List of meals which are available at that certain day.
        /// </summary>
        private ObservableCollection<MealViewModel> _meals;
        public ObservableCollection<MealViewModel> Meals
        {
            get { return _meals; }
            set { this.SetProperty(ref this._meals, value); }
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
