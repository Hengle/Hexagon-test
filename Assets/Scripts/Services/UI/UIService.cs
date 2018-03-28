using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityWeld.Binding;

namespace Hexagon.Services.UI
{
    [Binding]
    public class UIService : MonoBehaviour, INotifyPropertyChanged 
    {
        private readonly ActionBar _actionBar = new ActionBar();
        private readonly TurnList _turnList = new TurnList();


        [Binding]
        public ActionBar ActionBar
        {
            get { return _actionBar; }
        }

        [Binding]
        public TurnList TurnList {
            get { return _turnList; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

