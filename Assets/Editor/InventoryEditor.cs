using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InventoryController))]
public class InventoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var myTarget = (InventoryController)target;

        DrawDefaultInspector();
        if (GUILayout.Button("Random selected item"))
        {
            myTarget.AddRandomItem();
        }
    }
}