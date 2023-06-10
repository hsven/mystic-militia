using UnityEngine;
using UnityEditor;
using Tessera;

[CanEditMultipleObjects]
[CustomEditor(typeof(ExtendedTesseraTile))]
public class ExtendedTesseraTileEditor : TesseraTileBaseEditor {
    SerializedProperty tile;
    SerializedProperty setOfTiles;
    SerializedProperty blockOffset;
    SerializedProperty possibleUnderTiles;

    void OnEnable()
    {
        tile = serializedObject.FindProperty("tile");
        setOfTiles = serializedObject.FindProperty("setOfTiles");
        blockOffset = serializedObject.FindProperty("blockOffset");
        possibleUnderTiles = serializedObject.FindProperty("possibleUnderTiles");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUILayout.PropertyField(tile);
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.PropertyField(setOfTiles);
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.PropertyField(blockOffset);
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.PropertyField(possibleUnderTiles);
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Separator();

        base.OnInspectorGUI();
        
    }
}