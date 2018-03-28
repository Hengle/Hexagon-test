using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityWeld.Binding;

namespace Hexagon.Services.UI
{
    [Binding]
    public class ActionBar : INotifyPropertyChanged
    {
        private int _movePoints;
        private int _currentHealth;
        private int _maxHealth;

        public ActionBar()
        {
            _abilitySlots = new ObservableList<AbilitySlot> {
                new AbilitySlot(0),
                new AbilitySlot(1)
            };
        }

        private readonly ObservableList<AbilitySlot> _abilitySlots;

        [Binding]
        public ObservableList<AbilitySlot> AbilitySlots
        {
            get { return _abilitySlots; }
        }

        [Binding]
        public int movePoints
        {
            get { return _movePoints; }
            set
            {
                _movePoints = value;
                OnPropertyChanged("movePoints");
            }
        }

        public string HpText
        {
            get { return String.Format("HP " + currentHealth + "/" + maxHealth);}
        }

        [Binding]
        public float HealthPercentage
        {
            get { return (float) currentHealth / maxHealth; }
        }

        
        public int currentHealth
        {
            get { return _currentHealth; }
            set
            {
                _currentHealth = value; 
                OnPropertyChanged("HpText");
                OnPropertyChanged("HealthPercentage");
            }
        }

        [Binding]
        public int maxHealth
        {
            get { return _maxHealth; }
            set
            {
                _maxHealth = value; 
                OnPropertyChanged("HpText");
                OnPropertyChanged("HealthPercentage");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}


