using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Bathymetry))]
public class BathymetryEditor : Editor {
    public override void OnInspectorGUI() {
        Bathymetry bathymetry = (Bathymetry)target;
        
        if (DrawDefaultInspector()) {
            if (bathymetry.autoUpdate) {
                bathymetry.DrawBathymetry();
            }
        }

        if (GUILayout.Button("Update")) {
            bathymetry.DrawBathymetry();
        }
    }
}
