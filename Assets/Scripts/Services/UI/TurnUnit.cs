using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld.Binding;

namespace Hexagon.Services.UI
{
    [Binding]
    public class TurnUnit : INotifyPropertyChanged {
        private string _name;
        private bool _selected;

        public int id { get; set; }

        [Binding]
        public string name {
            get {
                return _name;
            }
            set {
                _name = value;
                OnPropertyChanged("name");
            }
        }

        [Binding]
        public bool selected
        {
            get { return _selected; }
            set
            {
                _selected = value; 
                OnPropertyChanged("selected");

            }
        }

        public TurnUnit(int unitId, string unitName) {
            id = unitId;
            name = unitName;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}


