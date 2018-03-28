using System.IO;
using UnityEngine;
using Utility;

[ExecuteInEditMode]
public class PrefabSerializer : MonoBehaviour
{
    private static bool serializedOnce;
    public GameObject[] prefabs;

    private void Awake()
    {
        if (serializedOnce == false) SerializeData();
    }

    public void SerializeData()
    {
        serializedOnce = true;

        var json = JsonHelper.arrayToJson(prefabs);

        Console.Log(json);

        File.WriteAllText(Application.persistentDataPath + "/prefabs.json", json);
    }
}