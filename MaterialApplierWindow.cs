using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MaterialApplierWindow : EditorWindow
{
    private string windowTitle = "Material Applier";
    private Vector2 size = new Vector2(300, 600);

    private Texture2D sourceTexture;
    private Vector2Int res = new Vector2Int(512, 512);
    private Material mat;
    private Texture2D texture;


    [MenuItem("Window/Material Applier")]
    static void Open()
    {
        MaterialApplierWindow window = GetWindow<MaterialApplierWindow>();
        window.Initialize();
    }

    public void Initialize()
    {
        minSize = size;
        titleContent = new GUIContent(windowTitle);
    }

    private void OnGUI()
    {
        DrawPreviewTexture();

        GUILayout.Label("Options");
        //GUILayout.BeginVertical();
        //colour = EditorGUILayout.ColorField("Colour: ", colour);
        sourceTexture = EditorGUILayout.ObjectField("Source", sourceTexture, typeof(Texture2D), false) as Texture2D;
        res = EditorGUILayout.Vector2IntField("Texture Resolution", res);
        mat = EditorGUILayout.ObjectField("Material", mat, typeof(Material), false) as Material;

        res.x = Mathf.Max(res.x, 8);
        res.y = Mathf.Max(res.y, 8);
        //GUILayout.EndVertical();

        bool enabled = GUI.enabled;

        if (mat == null)
            GUI.enabled = false;

        if (GUILayout.Button("Update"))
        {
            UpdateTexture();
        }

        if (GUILayout.Button("Save"))
        {
            string path = EditorUtility.SaveFilePanel("Save", Application.dataPath, "texture.png", ".png");
            if (path != string.Empty)
            {
                UpdateTexture();

                System.IO.File.WriteAllBytes(path, texture.EncodeToPNG());

                AssetDatabase.Refresh();
            }
        }

        GUI.enabled = enabled;
    }

    private void DrawPreviewTexture()
    {
        GUILayout.Label("Preview");

        int size = 128;
        GUILayout.Label("", GUILayout.Width(size), GUILayout.Height(size));
        Rect previewRect = GUILayoutUtility.GetLastRect();
        // Dummy element to get a rect

        //UpdateTexture();

        if (texture != null)
            EditorGUI.DrawPreviewTexture(previewRect, texture);
    }

    private void UpdateTexture()
    {
        if (mat == null)
        {
            Debug.LogError("Cannot update texture without a material!");
            return;
        }

        //if (sourceTexture != null)
        //    mat.mainTexture = sourceTexture;
        if (sourceTexture != null)
            texture = TextureUtility.GenerateTexture(res.x, res.y, mat, sourceTexture);
        else
            texture = TextureUtility.GenerateTexture(res.x, res.y, mat);
    }
}
