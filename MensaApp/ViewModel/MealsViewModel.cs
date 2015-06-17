﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MensaApp.ViewModel
{
    class MealsViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// 
        /// </summary>
        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set { this.SetProperty(ref this._date, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        private List<MealViewModel> _meals;
        public List<MealViewModel> Meals
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
