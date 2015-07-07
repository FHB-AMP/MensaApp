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
    public class InfoSymbolViewModel : INotifyPropertyChanged
    {
        public InfoSymbolViewModel()
        {
            this.IsExcluded = false;
        }
        
        /// <summary>
        /// Constructor to create a completly new symbolInfo.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="definition"></param>
        /// <param name="meaning"></param>
        public InfoSymbolViewModel(string id, string definition)
            : this ()
        {
            this.Id = id;
            this.Definition = definition;
        }

        /// <summary>
        /// Constructor fo create a symbolInfo which is restort from file.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="definition"></param>
        /// <param name="meaning"></param>
        /// <param name="isExcluded"></param>
        public InfoSymbolViewModel(string id, string definition, bool isExcluded) 
            : this(id, definition)
        {
            this.IsExcluded = isExcluded;
        }

        /// <summary>
        /// Unique string to identify an information.
        /// e.g. 'mit Geflügelfleisch'
        /// Should be saved in file.
        /// </summary>
        private string _id;
        public string Id
        {
            get { return _id; }
            set { this.SetProperty(ref this._id, value); }
        }

        /// <summary>
        /// String to define a certain information.
        /// Should be saved in file.
        /// </summary>
        private string _definition;
        public string Definition
        {
            get { return _definition; }
            set { this.SetProperty(ref this._definition, value); }
        }

        /// <summary>
        /// Is true, when the participant want to exclude the certain information from its nutrition.
        /// Should be saved in file.
        /// </summary>
        private bool _isExcluded;
        public bool IsExcluded
        {
            get { return _isExcluded; }
            set { this.SetProperty(ref this._isExcluded, value); }
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
