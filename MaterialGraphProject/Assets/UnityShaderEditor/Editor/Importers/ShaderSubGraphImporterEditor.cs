﻿using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEditor.MaterialGraph.Drawing;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

[CustomEditor(typeof(ShaderSubGraphImporter))]
public class ShaderSubGraphImporterEditor : ScriptedImporterEditor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Shader Editor"))
        {
            AssetImporter importer = target as AssetImporter;
            Debug.Assert(importer != null, "importer != null");
            ShowGraphEditWindow(importer.assetPath);
        }

    }
    private static void ShowGraphEditWindow(string path)
    {
        var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
        var windows = Resources.FindObjectsOfTypeAll<SubGraphEditWindow>();
        bool foundWindow = false;
        foreach (var w in windows)
        {
            if (w.selected == asset)
            {
                foundWindow = true;
                w.Focus();
            }
        }

        if (!foundWindow)
        {
            var window = CreateInstance<SubGraphEditWindow>();
            window.Show();
            window.ChangeSelection(asset);
        }
    }
}

