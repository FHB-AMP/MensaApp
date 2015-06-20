using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace MensaApp.ViewModel
{
    class AdditiveViewModel : INotifyPropertyChanged
    {
        public AdditiveViewModel() 
        {
            this.ToggleShowMeaningCommand = new ToggleShowMeaningClick();
        }

        public AdditiveViewModel(string id, string definition, string meaning)
        {
            this.Id = id;
            this.Definition = definition;
            this.Meaning = meaning;
            this.ShowMeaning = false;
            this.IsExcluded = false;
            this.ToggleShowMeaningCommand = new ToggleShowMeaningClick();
        }

        public AdditiveViewModel(string id, string definition, string meaning, bool isExcluded) 
        {
            this.Id = id;
            this.Definition = definition;
            this.Meaning = meaning;
            this.ShowMeaning = false;
            this.IsExcluded = isExcluded;
            this.ToggleShowMeaningCommand = new ToggleShowMeaningClick();
        }

        public ICommand ToggleShowMeaningCommand { get; set; }

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

        private string _meaning;
        public string Meaning
        {
            get { return _meaning; }
            set { this.SetProperty(ref this._meaning, value); }
        }

        private bool _showMeaning;
        public bool ShowMeaning
        {
            get { return _showMeaning; }
            set { this.SetProperty(ref this._showMeaning, value); }
        }

        public void ToggleShowMeaning()
        {
            this.ShowMeaning = !this.ShowMeaning;
        }

        /// <summary>
        /// Ist True, wenn ein Zusatzstoff von der Ernährung ausgeschossen ist.
        /// </summary>
        private bool _isExcluded;
        public bool IsExcluded
        {
            get { return _isExcluded; }
            set { _isExcluded = value; }
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

    public class ToggleShowMeaningClick : ICommand
    {
        public bool CanExecute(object parameter)
        {
            AdditiveViewModel additiveVM = (AdditiveViewModel)parameter;
            if (additiveVM != null && additiveVM.Meaning != null && additiveVM.Meaning.Length > 0)
            {
                return true;
            }
            return false;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            AdditiveViewModel additiveVM = (AdditiveViewModel) parameter;
            additiveVM.ToggleShowMeaning();
        }
    }
}
