using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MensaApp.ViewModel
{
    class MealViewModel : INotifyPropertyChanged
    {
        public MealViewModel() { }

        public MealViewModel(int mealNumber, string name) 
        {
            this.MealNumber = mealNumber;
            this.Name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        private int _mealNumber;
        public int MealNumber
        {
            get { return _mealNumber; }
            set { this.SetProperty(ref this._mealNumber, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool _isSuitableMeal;
        public bool IsSuitableMeal
        {
            get { return _isSuitableMeal; }
            set { this.SetProperty(ref this._isSuitableMeal, value); }
        }
        
        /// <summary>
        /// 
        /// </summary>
        private string _name;
        public string Name
        {
            get { return _name; }
            set { this.SetProperty(ref this._name, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool _isSuitableNutrition;
        public bool IsSuitableNutrition
        {
            get { return _isSuitableNutrition; }
            set { this.SetProperty(ref this._isSuitableNutrition, value); }
        }
        
        /// <summary>
        /// 
        /// </summary>
        private List<string> _symbols;
        public List<string> Symbols
        {
            get { return _symbols; }
            set { this.SetProperty(ref this._symbols, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool _isSuitableAdditives;
        public bool IsSuitableAdditives
        {
            get { return _isSuitableAdditives; }
            set { this.SetProperty(ref this._isSuitableAdditives, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        private List<AdditiveViewModel> _additives;
        public List<AdditiveViewModel> Additives
        {
            get { return _additives; }
            set { this.SetProperty(ref this._additives, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool _isSuitableAllergens;
        public bool IsSuitableAllergens
        {
            get { return _isSuitableAllergens; }
            set { this.SetProperty(ref this._isSuitableAllergens, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        private List<AllergenViewModel> _allergens;
        public List<AllergenViewModel> Allergens
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
