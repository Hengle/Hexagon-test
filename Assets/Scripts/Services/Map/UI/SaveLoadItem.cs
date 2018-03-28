using UnityEngine;
using UnityEngine.UI;

namespace Hexagon.Services.Map.UI
{
    public class SaveLoadItem : MonoBehaviour
    {
        private string mapName;

        public SaveLoadMenu menu;

        public string MapName
        {
            get { return mapName; }
            set
            {
                mapName = value;
                transform.GetChild(0).GetComponent<Text>().text = value;
            }
        }

        public void Select()
        {
            menu.SelectItem(mapName);
        }
    }
}