using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StatsSerializer))]
public class StatsSerializerInspector : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        StatsSerializer myScript = (StatsSerializer)target;
        if (GUILayout.Button("Export Json file")) {
            myScript.SerializeData();
        }
    }
}

[CustomPropertyDrawer(typeof(StatsSerializer.NamedArrayAttribute))]
public class NamedArrayDrawer : PropertyDrawer {
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
        try {
            int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
            EditorGUI.PropertyField(rect, property, new GUIContent(((StatsSerializer.NamedArrayAttribute)attribute).names[pos]));
        }
        catch {
            EditorGUI.PropertyField(rect, property, label);
        }
    }
}


