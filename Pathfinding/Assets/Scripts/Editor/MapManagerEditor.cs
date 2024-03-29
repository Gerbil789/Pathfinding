using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapManager))] 
public class MapManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapManager yourComponent = (MapManager)target;

        if (GUILayout.Button("Generate Map"))
        {
            yourComponent.GenerateMap();
        }

        if (GUILayout.Button("Clear Map"))
        {
            yourComponent.ClearMap();
        }
    }
}
