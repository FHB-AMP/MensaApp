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
    /// <summary>
    /// ViewModel of an additive
    /// Is used to manipulate front end.
    /// </summary>
    public class AdditiveViewModel : INotifyPropertyChanged
    {
        public AdditiveViewModel()
        {
            this.ShowMeaning = false;
            this.IsExcluded = false;
            this.ToggleShowMeaningCommand = new ToggleShowMeaningClick();
        }

        /// <summary>
        /// Constructor to create a completly new additive.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="definition"></param>
        /// <param name="meaning"></param>
        public AdditiveViewModel(string id, string definition, string meaning)
        {
            this.Id = id;
            this.Definition = definition;
            this.Meaning = meaning;
            this.ShowMeaning = false;
            this.IsExcluded = false;
            this.ToggleShowMeaningCommand = new ToggleShowMeaningClick();
        }

        /// <summary>
        /// Constructor fo create am additive which is restort from file.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="definition"></param>
        /// <param name="meaning"></param>
        /// <param name="isExcluded"></param>
        public AdditiveViewModel(string id, string definition, string meaning, bool isExcluded) 
        {
            this.Id = id;
            this.Definition = definition;
            this.Meaning = meaning;
            this.ShowMeaning = false;
            this.IsExcluded = isExcluded;
            this.ToggleShowMeaningCommand = new ToggleShowMeaningClick();
        }

        /// <summary>
        /// Command to toggle between show and hide further information of an additive.
        /// </summary>
        public ICommand ToggleShowMeaningCommand { get; set; }

        /// <summary>
        /// Unique string to identify an allergen.
        /// e.g. '(1)'
        /// Should be saved in file.
        /// </summary>
        private string _id;
        public string Id
        {
            get { return _id; }
            set { this.SetProperty(ref this._id, value); }
        }

        /// <summary>
        /// String to define a certain additive.
        /// Should be saved in file.
        /// </summary>
        private string _definition;
        public string Definition
        {
            get { return _definition; }
            set { this.SetProperty(ref this._definition, value); }
        }

        /// <summary>
        /// String which discribes meaning of a certain additive.
        /// Should be saved in file.
        /// </summary>
        private string _meaning;
        public string Meaning
        {
            get { return _meaning; }
            set { this.SetProperty(ref this._meaning, value); }
        }

        /// <summary>
        /// Is true, when the participant want to exclude the certain additive from its nutrition.
        /// Should be saved in file.
        /// </summary>
        private bool _isExcluded;
        public bool IsExcluded
        {
            get { return _isExcluded; }
            set { _isExcluded = value; }
        }

        /// <summary>
        /// Front end attribute to show or hide the 'meaning' property in the list of additives.
        /// </summary>
        private bool _showMeaning;
        public bool ShowMeaning
        {
            get { return _showMeaning; }
            set { this.SetProperty(ref this._showMeaning, value); }
        }


        /// <summary>
        /// Method which toggles the state of the 'ShowMeaning' property.
        /// </summary>
        public void ToggleShowMeaning()
        {
            this.ShowMeaning = !this.ShowMeaning;
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

    /// <summary>
    /// Command to call the 'ToggleShowMeaning' method on a button click.
    /// </summary>
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
