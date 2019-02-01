using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoardCreator))]
public class BoardCreatorInspector : Editor
{
    public int height;

    public BoardCreator current
    {
        get
        {
            return (BoardCreator)target;
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Clear"))
            current.Clear();
        if (GUILayout.Button("Grow"))
            current.Grow();
        if (GUILayout.Button("Shrink"))
            current.Shrink();
        if (GUILayout.Button("Grow Area"))
            current.GrowArea();
        if (GUILayout.Button("Shrink Area"))
            current.ShrinkArea();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Set height"))
            current.SetHeight(height);
        height = EditorGUILayout.IntField(height);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Save"))
            current.Save();
        if (GUILayout.Button("Load"))
            current.Load();
        if (GUI.changed)
            current.UpdateMarker();
    }
}