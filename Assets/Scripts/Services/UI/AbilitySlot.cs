using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;
using UnityWeld.Binding;

namespace Hexagon.Services.UI
{
    [Binding]
    public class AbilitySlot : INotifyPropertyChanged
    {
        private Sprite _icon;
        private KeyCode _key;
        private int _slotIndex;
        private bool _activated;

        [Binding]
        public Sprite Icon
        {
            get { return _icon; }
            set
            {
                _icon = value; 
                OnPropertyChanged("Icon");
            }
        }

        [Binding]
        public KeyCode Key {
            get { return _key; }
            set {
                _key = value;
                OnPropertyChanged("Key");
            }
        }

        [Binding]
        public bool Activated
        {
            get { return _activated; }
            set
            {
                _activated = value;
                OnPropertyChanged("Activated");
            }
        }

        [Binding]
        public void clicked()
        {
            if (Activated && SlotClickedEvent != null) SlotClickedEvent(abilityID);
        }

        public static event Action<int> SlotClickedEvent; 

        public int abilityID { get; set; }

        [Binding] public string SlotNumber { get {return (SlotIndex +1).ToString(); }}

        public int SlotIndex
        {
            get { return _slotIndex; }
            set
            {
                _slotIndex = value; 
                OnPropertyChanged("SlotNumber");
            }
        }

        public AbilitySlot(int i)
        {
            SlotIndex = i;
            Key = (KeyCode)System.Enum.Parse(typeof(KeyCode), String.Format("Alpha" + SlotNumber));
        }

        private void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

    }
}

