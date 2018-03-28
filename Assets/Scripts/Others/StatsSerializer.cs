using System.IO;
using UnityEngine;
using Utility;

[ExecuteInEditMode]
public class StatsSerializer : MonoBehaviour {
    private static bool serializedOnce;

    [NamedArrayAttribute(new string[] { "Max HP", "Max Energy", "Attack", "Power", "Speed" })]
    public int[] baseValues = new int[5];

    private void Awake() {
        if (serializedOnce == false) SerializeData();
    }

    public void SerializeData() {
        serializedOnce = true;

        var json = JsonHelper.arrayToJson(baseValues);

        Console.Log(json);

        File.WriteAllText(Application.persistentDataPath + "/statsBaseValue.json", json);
    }

    public class NamedArrayAttribute : PropertyAttribute {
        public readonly string[] names;
        public NamedArrayAttribute(string[] names) { this.names = names; }
    }
}