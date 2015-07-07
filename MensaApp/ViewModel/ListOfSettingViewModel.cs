using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensaApp.ViewModel
{
    public class ListOfSettingViewModel
    {
        public ListOfSettingViewModel()
        {
            this.NutritionViewModels = new ObservableCollection<NutritionViewModel>();
            this.AdditiveViewModels = new ObservableCollection<AdditiveViewModel>();
            this.AllergenViewModels = new ObservableCollection<AllergenViewModel>();
            this.InfoSymbolViewModels = new ObservableCollection<InfoSymbolViewModel>();
        }

        private ObservableCollection<InfoSymbolViewModel> _infoSymbolViewModels;
        public ObservableCollection<InfoSymbolViewModel> InfoSymbolViewModels
        {
            get { return _infoSymbolViewModels; }
            set { _infoSymbolViewModels = value; }
        }

        private ObservableCollection<NutritionViewModel> _nutritionViewModels;
        public ObservableCollection<NutritionViewModel> NutritionViewModels
        {
            get { return _nutritionViewModels; }
            set { _nutritionViewModels = value; }
        }

        private ObservableCollection<AdditiveViewModel> _additiveViewModels;
        public ObservableCollection<AdditiveViewModel> AdditiveViewModels
        {
            get { return _additiveViewModels; }
            set { _additiveViewModels = value; }
        }

        private ObservableCollection<AllergenViewModel> _allergenViewModels;
        public ObservableCollection<AllergenViewModel> AllergenViewModels
        {
            get { return _allergenViewModels; }
            set { _allergenViewModels = value; }
        }
    }
}
