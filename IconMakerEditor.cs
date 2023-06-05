using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;

namespace Tobo.IconMaker
{
    [CustomEditor(typeof(IconMaker))]
    public class IconMakerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            IconMaker iconMaker = (IconMaker)target;
            iconMaker.cam = EditorGUILayout.ObjectField("Camera", iconMaker.cam, typeof(Camera), true) as Camera;
            iconMaker.rt = EditorGUILayout.ObjectField("Render Texture", iconMaker.rt, typeof(RenderTexture), false) as RenderTexture;
            iconMaker.target = EditorGUILayout.ObjectField("Target Object", iconMaker.target, typeof(GameObject), true) as GameObject;
            iconMaker.path = EditorGUILayout.TextField("Folder Path", iconMaker.path);
            iconMaker.fileName = EditorGUILayout.TextField("File Name", iconMaker.fileName);
            bool guiState = GUI.enabled;
            GUI.enabled = false;
            EditorGUILayout.TextField("Full Path", $"Assets/{iconMaker.path}/{iconMaker.fileName}.png");
            GUI.enabled = guiState;

            iconMaker.fullPath = Path.Combine(Application.dataPath, iconMaker.path, iconMaker.fileName + ".png");

            if (GUILayout.Button("Render"))
            {
                iconMaker.Render();
            }
            if (iconMaker.target == null)
                GUI.enabled = false;
            if (GUILayout.Button("Fit to Target"))
            {
                iconMaker.FitToTarget();
            }
            GUI.enabled = guiState;

            GUILayout.Space(15);

            iconMaker.debugGizmos = EditorGUILayout.Toggle("Debug Gizmos", iconMaker.debugGizmos);
        }
    }
}
#endif
