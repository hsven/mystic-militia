using UnityEngine;
using UnityEditor;
using Tessera;

[CanEditMultipleObjects]
[CustomEditor(typeof(ExtendedTesseraTile))]
public class ExtendedTesseraTileEditor : TesseraTileBaseEditor {
    SerializedProperty tile;
    SerializedProperty setOfTiles;
    SerializedProperty isSingleSprite;
    SerializedProperty blockOffset;
    SerializedProperty possibleUnderTiles;
    SerializedProperty possibleSprites;

    void OnEnable()
    {
        tile = serializedObject.FindProperty("tile");
        setOfTiles = serializedObject.FindProperty("setOfTiles");
        isSingleSprite = serializedObject.FindProperty("isSingleSprite");
        blockOffset = serializedObject.FindProperty("blockOffset");
        possibleUnderTiles = serializedObject.FindProperty("possibleUnderTiles");
        possibleSprites = serializedObject.FindProperty("possibleSprites");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUILayout.PropertyField(tile);
        EditorGUILayout.PropertyField(setOfTiles);
        EditorGUILayout.PropertyField(isSingleSprite);
        if(isSingleSprite.boolValue)
        {
            EditorGUILayout.PropertyField(possibleSprites);
        }

        EditorGUILayout.PropertyField(blockOffset);
        EditorGUILayout.PropertyField(possibleUnderTiles);


        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.Separator();

        base.OnInspectorGUI();
        
    }
}