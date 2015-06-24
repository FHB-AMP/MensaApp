using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MensaApp.ViewModel
{
    /// <summary>
    /// ViewModel of the Settings page.
    /// It contains lists of all available allergens and additives.
    /// Is used to manipulate front end.
    /// </summary>
    public class SettingsPageViewModel : INotifyPropertyChanged
    {
        public SettingsPageViewModel()
        {
            this.SelectedNutrition = new NutritionViewModel();
            this.Nutritions = new ObservableCollection<NutritionViewModel>();
            this.Additives = new ObservableCollection<AdditiveViewModel>();
            this.Allergens = new ObservableCollection<AllergenViewModel>();
        }

        public SettingsPageViewModel(ObservableCollection<NutritionViewModel> nutritions, 
            NutritionViewModel selectedNutrition, ObservableCollection<AdditiveViewModel> additives, ObservableCollection<AllergenViewModel> allergens)
        {
            this.SelectedNutrition = selectedNutrition != null ? selectedNutrition : new NutritionViewModel();
            this.Nutritions = nutritions != null ? nutritions : new ObservableCollection<NutritionViewModel>();
            this.Additives = additives != null ? additives : new ObservableCollection<AdditiveViewModel>();
            this.Allergens = allergens != null ? allergens : new ObservableCollection<AllergenViewModel>();
        }

        /// <summary>
        /// List of all kinds of nutritions
        /// </summary>
        private ObservableCollection<NutritionViewModel> _nutritions;
        public ObservableCollection<NutritionViewModel> Nutritions
        {
            get { return _nutritions; }
            set { this.SetProperty(ref this._nutritions, value); }
        }

        /// <summary>
        /// selected kind of nutrition by participant
        /// </summary>
        private NutritionViewModel _selectedNutrition;
        public NutritionViewModel SelectedNutrition
        {
            get { return _selectedNutrition; }
            set { this.SetProperty(ref this._selectedNutrition, value); }
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
