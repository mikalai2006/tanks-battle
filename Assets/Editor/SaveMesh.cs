using UnityEngine;
    using UnityEditor;
    using System.IO;

    public static class MeshFilterContext
    {
        [MenuItem("CONTEXT/MeshFilter/Save Mesh Asset")]
        public static void SaveMeshAsset(MenuCommand menuCommand)
        {
            MeshFilter filter = menuCommand.context as MeshFilter;
            if (filter == null || filter.sharedMesh == null)
            {
                Debug.LogWarning("No MeshFilter or Mesh found to save.");
                return;
            }

            string path = EditorUtility.SaveFilePanelInProject(
                "Save Mesh Asset",
                filter.sharedMesh.name + ".asset",
                "asset",
                "Enter a file name for the Mesh Asset."
            );

            if (!string.IsNullOrEmpty(path))
            {
                Mesh meshToSave = Object.Instantiate(filter.sharedMesh); // Create a copy to avoid modifying the original shared mesh
                AssetDatabase.CreateAsset(meshToSave, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("Mesh saved to: " + path);
            }
        }
    }