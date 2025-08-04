#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VoxReader;
using VoxReader.Interfaces;

public class VoxParser : MonoBehaviour
{
    [SerializeField] private string pathFile;
    [SerializeField] private TextAsset asset;
    [SerializeField] private string folderPrefix = "panzers";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!AssetDatabase.IsValidFolder($"Assets/SO/{folderPrefix}")) {
            AssetDatabase.CreateFolder($"Assets/SO", folderPrefix);
        }

        IVoxFile voxFile = VoxReader.VoxReader.Read(pathFile);
        

        // Access models of .vox file
        IModel[] models = voxFile.Models;

        // Access voxels of first model in the file
        for (int m = 0; m < models.Length; m++)
        {
            var model = models[m];
            Voxel[] voxels = model.Voxels;

            Debug.Log($"Voxels: name={model.Name} count={voxels.Length}, global size = {model.GlobalSize}, isCopy={model.IsCopy}");

            // // Access properties of a voxel
            // VoxReader.Vector3 position = voxels[0].GlobalPosition;
            // VoxReader.Color color = voxels[0].Color;

            // List<UnityEngine.Vector3> points = new();
            // List<UnityEngine.Color> colors = new();
            Dictionary<UnityEngine.Vector3, UnityEngine.Color> pointsColors = new();

            foreach (Voxel voxel in voxels) // Используем ref для изменения оригинальных значений
            {
                pointsColors.Add(
                    new UnityEngine.Vector3(voxel.GlobalPosition.X, voxel.GlobalPosition.Z, voxel.GlobalPosition.Y),
                    new UnityEngine.Color(voxel.Color.R / 255f, voxel.Color.G / 255f, voxel.Color.B / 255f, voxel.Color.A / 255f)
                );
                // points.Add(new UnityEngine.Vector3(voxel.GlobalPosition.X, voxel.GlobalPosition.Z, voxel.GlobalPosition.Y));
                // // Debug.Log($"voxel.Color=>{voxel.Color}|{voxel.Color.R}, {voxel.Color.G}, {voxel.Color.B}, {voxel.Color.A}");
                // colors.Add(new UnityEngine.Color(voxel.Color.R / 255f, voxel.Color.G / 255f, voxel.Color.B / 255f, voxel.Color.A / 255f));
            }


            var groupSubMeshes = pointsColors.GroupBy(obj => obj.Value)
                .AsParallel()
                // .Select(group => group)
                .ToList();
            Debug.Log($"uniqueColors: {groupSubMeshes.Count()}");
            
            // foreach (var group in groupSubMeshes)
            // {
            //     Debug.Log($"key: {group.Key}");
            //     foreach (var voxel in group)
            //     {
            //         Debug.Log($"  - {voxel.Key} ({voxel.Value})");
            //     }
            // }
            List<SubmeshesData> submeshesDatas = new();
            for (int n = 0; n < groupSubMeshes.Count; n++)
            {
                SubmeshesData submeshesData = new SubmeshesData()
                {
                    color = groupSubMeshes[n].Key,
                    voxels = groupSubMeshes[n].AsParallel().ToDictionary(t => t.Key, t => t.Value).Keys.ToList(),
                };
                submeshesDatas.Add(submeshesData);
            }

            // Create ScriptableObject with data voxels and colors.
            SOVoxelData asset = ScriptableObject.CreateInstance<SOVoxelData>();
            asset.groups = submeshesDatas;
            asset.voxels = pointsColors.Keys.AsParallel().ToList();
            asset.colors = pointsColors.Values.AsParallel().ToList();
            asset.GlobalSize = new UnityEngine.Vector3(model.GlobalSize.X, model.GlobalSize.Y, model.GlobalSize.Z);
            asset.GlobalPosition = new UnityEngine.Vector3(model.GlobalPosition.X, model.GlobalPosition.Y, model.GlobalPosition.Z);
            asset.LocalSize = new UnityEngine.Vector3(model.LocalPosition.X, model.LocalPosition.Y, model.LocalPosition.Z);
            asset.LocalPosition = new UnityEngine.Vector3(model.LocalPosition.X, model.LocalPosition.Y, model.LocalPosition.Z);
            // asset.GlobalRotation = new UnityEngine.Vector3(model.GlobalRotation[0,0],);
            // asset.sizeVoxel = 1f / Mathf.Min(CubeVoxels.size.x, CubeVoxels.size.y, CubeVoxels.size.z);
            string[] namePathArray = pathFile.Split(new char[] { '/' });
            string nameFolderModel = Path.GetFileNameWithoutExtension(namePathArray[namePathArray.Length - 1]);

            string modelName = models[m].Name;
            if (!AssetDatabase.IsValidFolder($"Assets/SO/{folderPrefix}/{nameFolderModel}"))
            {
                AssetDatabase.CreateFolder($"Assets/SO/{folderPrefix}", nameFolderModel);
            }
            string path = AssetDatabase.GenerateUniqueAssetPath($"Assets/SO/{folderPrefix}/{nameFolderModel}/{modelName}_{nameFolderModel}.asset");

            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

        }

    }

}
#endif