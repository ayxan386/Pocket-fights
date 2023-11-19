using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomManager))]
public class RoomManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var myTarget = (RoomManager)target;

        DrawDefaultInspector();
        if (GUILayout.Button("Generate"))
        {
            myTarget.GenerateFloor();
        }

        if (GUILayout.Button("Generate upper layer"))
        {
            myTarget.GenerateUpperFloor();
        }

        if (GUILayout.Button("Join cells"))
        {
            myTarget.GameOfLife();
        }

        if (GUILayout.Button("Place decors"))
        {
            myTarget.PlaceDecors();
        }
    }
}