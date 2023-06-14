using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TextureCreatorWindow : EditorWindow
{
    private string windowTitle = "Texture Creator";
    private Vector2 size = new Vector2(200, 50);
    private int resolution = 16;

    private Color colour = Color.blue;

    private Texture2D texture;
    private Color lastColour;


    [MenuItem("Window/Texture Creator")]
    static void Open()
    {
        TextureCreatorWindow window = GetWindow<TextureCreatorWindow>();
        window.Initialize();
    }

    public void Initialize()
    {
        minSize = size;
        titleContent = new GUIContent(windowTitle);
    }

    private void OnGUI()
    {
        //DrawPreviewTexture();

        GUILayout.Label("Options");
        //GUILayout.BeginVertical();
        colour = EditorGUILayout.ColorField("Colour: ", colour);
        //GUILayout.EndVertical();

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
    }

    /*
    private void DrawPreviewTexture()
    {
        GUILayout.Label("Preview");

        int size = 128;
        GUILayout.Label("", GUILayout.Width(size), GUILayout.Height(size));
        Rect previewRect = GUILayoutUtility.GetLastRect();
        // Dummy element to get a rect

        UpdateTexture();

        EditorGUI.DrawPreviewTexture(previewRect, texture);
    }
    */

    private void UpdateTexture()
    {
        if (texture == null)
        {
            texture = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
            texture.wrapMode = TextureWrapMode.Clamp;
        }

        if (colour != lastColour)
        {
            Color[] colours = new Color[resolution * resolution];
            for (int i = 0; i < colours.Length; i++)
                colours[i] = colour;
            lastColour = colour;
            texture.SetPixels(colours);
            //texture.Apply();
        }
    }
}
