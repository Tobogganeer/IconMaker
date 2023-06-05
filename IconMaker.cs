using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

namespace Tobo.IconMaker
{
    public class IconMaker : MonoBehaviour
    {
        public Camera cam;
        public RenderTexture rt;
        public GameObject target;
        public string path = "folder";
        public string fileName = "image";
        public string fullPath;
        public bool debugGizmos;

        public void Render()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(fullPath))
            {
                Debug.LogWarning("Export failed: fullPath was null or empty!");
                return;
            }
            if (rt.height != rt.width)
            {
                Debug.LogWarning("Export Failed: RenderTexture is not a square.");
                return;
            }
            if (!rt.isPowerOfTwo)
            {
                Debug.LogWarning("Export Failed: RenderTexture resolution not a power of two.");
                return;
            }

            cam.targetTexture = rt;
            RenderTexture.active = rt;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0, 0, 0, 0);
            cam.Render();


            Texture2D texture = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false, false);
            texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            //texture.Apply();

            RenderTexture.active = null;

            byte[] bytes = texture.EncodeToPNG();

            DestroyImmediate(texture);

            //byte[] bytes = ImageConversion.
            if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            }
            File.WriteAllBytes(fullPath, bytes);
            Debug.Log("Exported texture to " + fullPath);
            AssetDatabase.Refresh();
#else
            Debug.Log("This is an editor script?");
#endif
        }

        const float FitMargin = 1.1f;

        public void FitToTarget()
        {
            // https://forum.unity.com/threads/fit-object-exactly-into-perspective-cameras-field-of-view-focus-the-object.496472/

            if (target == null)
            {
                Debug.LogWarning("Cannot fit to empty target!");
                return;
            }

            Bounds b = GetBounds(target);
            //float objScaleRadius = b.extents.magnitude;
            float objScaleRadius = Mathf.Max(b.extents.x, b.extents.y, b.extents.z);
            // Looks better from testing in gizmos

            float minDistance = objScaleRadius * FitMargin / Mathf.Sin(Mathf.Deg2Rad * cam.fieldOfView / 2f);
            cam.transform.position = Vector3.back * minDistance;
        }

        Bounds GetBounds(GameObject obj)
        {
            Bounds b = new Bounds();
            foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>())
            {
                b.Encapsulate(renderer.localBounds);
            }

            return b;
        }

        private void OnDrawGizmos()
        {
            if (target != null && debugGizmos)
            {
                Bounds b = GetBounds(target);
                float objScaleRadius = b.extents.magnitude;
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(Vector3.zero, objScaleRadius * FitMargin);
                Gizmos.DrawWireCube(b.center, b.size);

                Gizmos.color = Color.yellow;
                float avgRadius = (b.extents.x + b.extents.y + b.extents.z) / 3f;
                Gizmos.DrawWireSphere(Vector3.zero, avgRadius * FitMargin);

                Gizmos.color = Color.green;
                float largestRadius = Mathf.Max(b.extents.x, b.extents.y, b.extents.z);
                Gizmos.DrawWireSphere(Vector3.zero, largestRadius * FitMargin);
            }
        }
    }
}
