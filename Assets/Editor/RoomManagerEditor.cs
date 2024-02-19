using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GroundGenerator))]
public class GroundGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var myTarget = (GroundGenerator)target;

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
    }
}