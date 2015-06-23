﻿using System;
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
    /// ViewModel of a meal.
    /// Is used to manipulate front end.
    /// </summary>
    public class MealViewModel : INotifyPropertyChanged
    {
        public MealViewModel()
        {
            this.MealNumber = 1;
            this.Name = "";
            this.IsSuitableMeal = true;
            this.IsSuitableNutrition = true;
            this.IsSuitableAdditives = true;
            this.IsSuitableAllergens = true;
            this.Symbols = new ObservableCollection<string>();
            this.Additives = new ObservableCollection<AdditiveViewModel>();
            this.Allergens = new ObservableCollection<AllergenViewModel>();
        }

        public MealViewModel(int mealNumber, string name)
            : this()
        {
            this.MealNumber = mealNumber;
            this.Name = name;
        }

        public MealViewModel(int mealNumber, string name, bool suitableMeal, bool suitableNutrition, bool suitableAdditives, bool suitableAllergens)
            : this(mealNumber, name)
        {
            this.IsSuitableMeal = suitableMeal;
            this.IsSuitableNutrition = suitableNutrition;
            this.IsSuitableAdditives = suitableAdditives;
            this.IsSuitableAllergens = suitableAllergens;
        }

        public MealViewModel(int mealNumber, string name, ObservableCollection<string> symbols, ObservableCollection<AdditiveViewModel> additives, ObservableCollection<AllergenViewModel> allergens)
            : this(mealNumber, name)
        {
            this.Symbols = symbols;
            this.Additives = additives;
            this.Allergens = allergens;
        }

        public MealViewModel(int mealNumber, string name, ObservableCollection<string> symbols, ObservableCollection<AdditiveViewModel> additives, 
            ObservableCollection<AllergenViewModel> allergens, bool suitableMeal, bool suitableNutrition, bool suitableAdditives, bool suitableAllergens)
            : this(mealNumber, name, symbols, additives, allergens)
        {
            this.IsSuitableMeal = suitableMeal;
            this.IsSuitableNutrition = suitableNutrition;
            this.IsSuitableAdditives = suitableAdditives;
            this.IsSuitableAllergens = suitableAllergens;
        }

        /// <summary>
        /// Describes the position of meal in the list of meals.
        /// </summary>
        private int _mealNumber;
        public int MealNumber
        {
            get { return _mealNumber; }
            set { this.SetProperty(ref this._mealNumber, value); }
        }

        /// <summary>
        /// It True, when the nutirtion, additives and allergens of the meal matches the settings of the participant.
        /// </summary>
        private bool _isSuitableMeal;
        public bool IsSuitableMeal
        {
            get { return _isSuitableMeal; }
            set { this.SetProperty(ref this._isSuitableMeal, value); }
        }

        /// <summary>
        /// The name of the meal.
        /// </summary>
        private string _name;
        public string Name
        {
            get { return _name; }
            set { this.SetProperty(ref this._name, value); }
        }

        /// <summary>
        /// Is True, when nutrition of the meal matches the nutrition settings of the participant.
        /// </summary>
        private bool _isSuitableNutrition;
        public bool IsSuitableNutrition
        {
            get { return _isSuitableNutrition; }
            set { this.SetProperty(ref this._isSuitableNutrition, value); }
        }

        /// <summary>
        /// Symbols of the meal. Given by API.
        /// </summary>
        private ObservableCollection<string> _symbols;
        public ObservableCollection<string> Symbols
        {
            get { return _symbols; }
            set { this.SetProperty(ref this._symbols, value); }
        }

        /// <summary>
        /// Is True, when additives of the meal matches the additive settings of the participant.
        /// </summary>
        private bool _isSuitableAdditives;
        public bool IsSuitableAdditives
        {
            get { return _isSuitableAdditives; }
            set { this.SetProperty(ref this._isSuitableAdditives, value); }
        }

        /// <summary>
        /// Additives of the meal. Given by API.
        /// </summary>
        private ObservableCollection<AdditiveViewModel> _additives;
        public ObservableCollection<AdditiveViewModel> Additives
        {
            get { return _additives; }
            set { this.SetProperty(ref this._additives, value); }
        }

        /// <summary>
        /// Is True, when allergens of the meal matches the allergen settings of the participant.
        /// </summary>
        private bool _isSuitableAllergens;
        public bool IsSuitableAllergens
        {
            get { return _isSuitableAllergens; }
            set { this.SetProperty(ref this._isSuitableAllergens, value); }
        }

        /// <summary>
        /// Allergens of the meal.
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
