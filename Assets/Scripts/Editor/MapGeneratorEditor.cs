using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    MapGenerator generator;

    void OnEnable()
    {
        generator = (MapGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        if (Application.isPlaying)
        {
            if (GUILayout.Button("Generate Map"))
            {
                generator.GenerateMap();
            }
        }
        DrawDefaultInspector();

    }
}