using MensaApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensaApp.DataModel
{
    /// <summary>
    /// Model to exchange data between MealsPage to DetailPage
    /// </summary>
    public class DetailPageParameter
    {
        public DetailPageParameter() { }

        public DetailPageParameter(DateTime date, MealViewModel meal) 
        {
            this.date = date;
            this.meal = meal;
        }
        /// <summary>
        /// Date of the selected meal
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// Selected meal which should be shown in the detail page
        /// </summary>
        public MealViewModel meal { get; set; }
    }
}
