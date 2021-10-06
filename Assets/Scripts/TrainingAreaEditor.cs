using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TrainingArea))]
public class TrainingAreaEditor : Editor
{


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TrainingArea myScript = (TrainingArea)target;
        if (GUILayout.Button("Load Map"))
        {
            myScript.LoadMap();
        }
    }


}
