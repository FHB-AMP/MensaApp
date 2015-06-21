﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MensaApp.ViewModel
{
    /// <summary>
    /// ViewModel of the Settings page.
    /// It contains lists of all available allergens and additives.
    /// Is used to manipulate front end.
    /// </summary>
    class SettingViewModel : INotifyPropertyChanged
    {
        public SettingViewModel()
        {
            this.Additives = new ObservableCollection<AdditiveViewModel>();
            this.Allergens = new ObservableCollection<AllergenViewModel>();
        }

        /// <summary>
        /// The list of all additives which should be shown at the settings page.
        /// </summary>
        private ObservableCollection<AdditiveViewModel> _additives;
        public ObservableCollection<AdditiveViewModel> Additives
        {
            get { return _additives; }
            set { this.SetProperty(ref this._additives, value); }
        }

        /// <summary>
        /// The list of all allergens which should be shown at the settings page.
        /// </summary>
        private ObservableCollection<AllergenViewModel> _allergens;
        public ObservableCollection<AllergenViewModel> Allergens
        {
            get { return _allergens; }
            set { this.SetProperty(ref this._allergens, value); }
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
