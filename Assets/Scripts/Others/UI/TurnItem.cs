using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnItem : MonoBehaviour
{

    [SerializeField] private Text unitName;
    [SerializeField] private Image indicator;

    public int id;

    public void SetIndicator(bool toggle)
    {
        indicator.gameObject.SetActive(toggle);
    }

    public void SetName(string name)
    {
        unitName.text = name;
    }
}
