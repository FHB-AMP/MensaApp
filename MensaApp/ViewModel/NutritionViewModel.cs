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
    /// defines a kind of nutrition and its properties.
    /// </summary>
    public class NutritionViewModel : INotifyPropertyChanged
    {
        public NutritionViewModel()
        {
            this.ExcludedSymbols = new ObservableCollection<string>();
            this.ExcludedAdditives = new ObservableCollection<AdditiveViewModel>();
            this.ExcludedAllergens = new ObservableCollection<AllergenViewModel>();
            this.IsSelectedNutrition = false;
        }

        public NutritionViewModel(string id, string name, ObservableCollection<string> excludedSymbols,
            ObservableCollection<AdditiveViewModel> excludedAdditives, ObservableCollection<AllergenViewModel> excludedAllergens)
        {
            this.Id = id;
            this.Name = name;
            this.ExcludedSymbols = excludedSymbols != null ? excludedSymbols : new ObservableCollection<string>();
            this.ExcludedAdditives = excludedAdditives != null ? excludedAdditives : new ObservableCollection<AdditiveViewModel>();
            this.ExcludedAllergens = excludedAllergens != null ? excludedAllergens : new ObservableCollection<AllergenViewModel>();
            this.IsSelectedNutrition = false;
        }

        public NutritionViewModel(string id, string name, ObservableCollection<string> excludedSymbols, ObservableCollection<AdditiveViewModel> excludedAdditives,
            ObservableCollection<AllergenViewModel> excludedAllergens, bool isSelectedNutrition)
            : this(id, name, excludedSymbols, excludedAdditives, excludedAllergens)
        {
            this.IsSelectedNutrition = isSelectedNutrition;
        }

        /// <summary>
        /// id of nutrition
        /// </summary>
        private string _id;
        public string Id
        {
            get { return _id; }
            set { this.SetProperty(ref this._id, value); }
        }

        /// <summary>
        /// Define which nutrition is choosen by the participant
        /// </summary>
        private String _name;
        public String Name
        {
            get { return _name; }
            set { this.SetProperty(ref this._name, value); }
        }

        /// <summary>
        /// List of symbols which are excluded by the certain nutrition
        /// </summary>
        private ObservableCollection<String> _excludedSymbols;
        public ObservableCollection<String> ExcludedSymbols
        {
            get { return _excludedSymbols; }
            set { this.SetProperty(ref this._excludedSymbols, value); }
        }

        /// <summary>
        /// List of additives which are excluded by the certain nutrition
        /// </summary>
        private ObservableCollection<AdditiveViewModel> _excludedAdditives;
        public ObservableCollection<AdditiveViewModel> ExcludedAdditives
        {
            get { return _excludedAdditives; }
            set { this.SetProperty(ref this._excludedAdditives, value); }
        }

        /// <summary>
        /// List of allergens which are excluded by the certain nutrition
        /// </summary>
        private ObservableCollection<AllergenViewModel> _excludedAllergens;
        public ObservableCollection<AllergenViewModel> ExcludedAllergens
        {
            get { return _excludedAllergens; }
            set { this.SetProperty(ref this._excludedAllergens, value); }
        }

        /// <summary>
        /// Is True, when the participant has choosen this nutrition.
        /// </summary>
        private bool _isSelectedNutrition;
        public bool IsSelectedNutrition
        {
            get { return _isSelectedNutrition; }
            set { this.SetProperty(ref this._isSelectedNutrition, value); }
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
