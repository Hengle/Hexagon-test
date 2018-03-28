using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld.Binding;

namespace Hexagon.Services.UI
{
    [Binding]
    public class TurnList : INotifyPropertyChanged
    {

        private readonly ObservableList<TurnUnit> _units = new ObservableList<TurnUnit>();
        private bool _active;


        [Binding]
        public bool Active
        {
            get { return _active;}
            set
            {
                _active = value;
                OnPropertyChanged("Active");
            }
        }

        [Binding]
        public ObservableList<TurnUnit> Units
        {
            get { return _units; }
        }

        public void addUnit(int id, string name)
        {
            TurnUnit unit = new TurnUnit(id, name);
            _units.Add(unit);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

}


