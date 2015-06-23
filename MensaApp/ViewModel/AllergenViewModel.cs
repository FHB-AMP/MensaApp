using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace MensaApp.ViewModel
{
    /// <summary>
    /// ViewModel of an allergen
    /// Is used to manipulate front end.
    /// </summary>
    public class AllergenViewModel : INotifyPropertyChanged
    {
        public AllergenViewModel()
        {
            this.IsExcluded = false;
            this.ShowContainedIn = false;
            this.ToggleShowContainedInCommand = new ToggleShowContainedInClick();
        }

        /// <summary>
        /// Constructor to create a completly new allergen.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="definition"></param>
        /// <param name="containedIn"></param>
        public AllergenViewModel(string id, string definition, string containedIn)
        {
            this.Id = id;
            this.Definition = definition;
            this.ContainedIn = containedIn;
            this.IsExcluded = false;
            this.ShowContainedIn = false;
            this.ToggleShowContainedInCommand = new ToggleShowContainedInClick();
        }

        /// <summary>
        /// Constructor to create an allergen which is restort from file.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="definition"></param>
        /// <param name="containedIn"></param>
        /// <param name="isExcluded"></param>
        public AllergenViewModel(string id, string definition, string containedIn, bool isExcluded)
        {
            this.Id = id;
            this.Definition = definition;
            this.ContainedIn = containedIn;
            this.IsExcluded = isExcluded;
            this.ShowContainedIn = false;
            this.ToggleShowContainedInCommand = new ToggleShowContainedInClick();
        }

        /// <summary>
        /// Command to toggle between show and hide further information of an allergen.
        /// </summary>
        public ICommand ToggleShowContainedInCommand { get; set; }

        /// <summary>
        /// Unique string to identify an allergen.
        /// e.g. '(A)'
        /// Should be saved in file.
        /// </summary>
        private string _id;
        public string Id
        {
            get { return _id; }
            set { this.SetProperty(ref this._id, value); }
        }

        /// <summary>
        /// String to define a certain allergen.
        /// Should be saved in file.
        /// </summary>
        private string _definition;
        public string Definition
        {
            get { return _definition; }
            set { this.SetProperty(ref this._definition, value); }
        }

        /// <summary>
        /// String which discribes food products which contains the given allergen.
        /// Should be saved in file.
        /// </summary>
        private string _containedIn;
        public string ContainedIn
        {
            get { return _containedIn; }
            set { this.SetProperty(ref this._containedIn, value); }
        }

        /// <summary>
        /// Is true, when the participant want to exclude the certain allergen from its nutrition.
        /// Should be saved in file.
        /// </summary>
        private bool _isExcluded;
        public bool IsExcluded
        {
            get { return _isExcluded; }
            set { _isExcluded = value; }
        }

        /// <summary>
        /// Front end attribute to show or hide the 'containedIn' property in the list of allergens.
        /// </summary>
        private bool _showContainedIn;
        public bool ShowContainedIn
        {
            get { return _showContainedIn; }
            set { this.SetProperty(ref this._showContainedIn, value); }
        }

        /// <summary>
        /// Method which toggles the state of the 'ShowContainedIn' property.
        /// </summary>
        public void ToggleShowContainedIn()
        {
            this.ShowContainedIn = !this.ShowContainedIn;
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

        /// <summary>
        /// Command to call the 'ToggleShowContainedIn' method on a button click.
        /// </summary>
        public class ToggleShowContainedInClick : ICommand
        {
            public bool CanExecute(object parameter)
            {
                AllergenViewModel allergenVM = (AllergenViewModel)parameter;
                if (allergenVM != null && allergenVM.ContainedIn != null && allergenVM.ContainedIn.Length > 0)
                {
                    return true;
                }
                return false;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                AllergenViewModel allergenVM = (AllergenViewModel)parameter;
                allergenVM.ToggleShowContainedIn();
            }
        }
    }
}
