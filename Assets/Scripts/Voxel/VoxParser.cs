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
    [SerializeField] private string[] files;
    // [SerializeField] private TextAsset asset;
    [SerializeField] private string folderPrefix = "panzers";
    [SerializeField] private bool isGlobalPosition;
    string OutputPath = "Assets/Prefabs/1Vox";

    void Start()
    {
        // CreateData();
    }

    public void OnSetFiles(string[] paths)
    {
        files = paths;
    }

    public void OnCreateData()
    {
        foreach (string path in files)
        {
            CreateData(path);
        }
    }

    void CreateData(string pathFile)
    {
        if (!AssetDatabase.IsValidFolder($"{OutputPath}/SO/{folderPrefix}"))
        {
            AssetDatabase.CreateFolder($"{OutputPath}/SO", folderPrefix);
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
            Dictionary<UnityEngine.Vector3Int, UnityEngine.Color> pointsColors = new();

            UnityEngine.Vector3 pivot = UnityEngine.Vector3.zero;
            UnityEngine.Vector3Int bounds = new UnityEngine.Vector3Int(model.GlobalSize.X, model.GlobalSize.Z, model.GlobalSize.Y);

            UnityEngine.Vector3Int min = new UnityEngine.Vector3Int(model.GlobalSize.X, model.GlobalSize.Z, model.GlobalSize.Y);
            UnityEngine.Vector3Int max = UnityEngine.Vector3Int.zero;


            // Проходим по всем точкам и находим смещения по всем осям.
            for (int x = 0; x < voxels.Count(); x++) // Используем ref для изменения оригинальных значений
            {
                Voxel voxel = voxels[x];
                if (isGlobalPosition == false)
                {
                    min = new Vector3Int(
                        Mathf.Min(min.x, voxel.LocalPosition.X),
                        Mathf.Min(min.y, voxel.LocalPosition.Z),
                        Mathf.Min(min.z, voxel.LocalPosition.Y)
                    );

                    max.x = Mathf.Max(max.x, voxel.LocalPosition.X);
                    max.y = Mathf.Max(max.y, voxel.LocalPosition.Z);
                    max.z = Mathf.Max(max.z, voxel.LocalPosition.Y);
                }
            }
            Debug.Log($"min:{min}, max:{max}");


            foreach (Voxel voxel in voxels) // Используем ref для изменения оригинальных значений
            {
                pointsColors.Add(
                    isGlobalPosition
                        ? new UnityEngine.Vector3Int(voxel.GlobalPosition.X, voxel.GlobalPosition.Z, voxel.GlobalPosition.Y)
                        : new UnityEngine.Vector3Int(voxel.LocalPosition.X - min.x, voxel.LocalPosition.Z - min.y, voxel.LocalPosition.Y - min.z),
                    new UnityEngine.Color(voxel.Color.R / 255f, voxel.Color.G / 255f, voxel.Color.B / 255f, voxel.Color.A / 255f)
                );

                // points.Add(new UnityEngine.Vector3(voxel.GlobalPosition.X, voxel.GlobalPosition.Z, voxel.GlobalPosition.Y));
                // // Debug.Log($"voxel.Color=>{voxel.Color}|{voxel.Color.R}, {voxel.Color.G}, {voxel.Color.B}, {voxel.Color.A}");
                // colors.Add(new UnityEngine.Color(voxel.Color.R / 255f, voxel.Color.G / 255f, voxel.Color.B / 255f, voxel.Color.A / 255f));
            }

            if (isGlobalPosition == false)
            {
                bounds.x = max.x - min.x + 1;
                bounds.y = max.y - min.y + 1;
                bounds.z = max.z - min.z + 1;

                pivot.x = bounds.x / 2f;
                pivot.y = bounds.y / 2f;
                pivot.z = bounds.z / 2f;
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
                    voxels = groupSubMeshes[n].AsParallel().ToDictionary(t => Vector3Int.FloorToInt(t.Key), t => t.Value).Keys.ToList(),
                };
                submeshesDatas.Add(submeshesData);
            }

            // Create ScriptableObject with data voxels and colors.
            SOVoxelData asset = ScriptableObject.CreateInstance<SOVoxelData>();
            asset.groups = submeshesDatas;
            asset.voxels = pointsColors.Keys.AsParallel().ToList();
            asset.Pivot = pivot;
            asset.Bounds = bounds;
            asset.colors = pointsColors.Values.AsParallel().ToList();
            asset.GlobalSize = new UnityEngine.Vector3Int(model.GlobalSize.X, model.GlobalSize.Y, model.GlobalSize.Z);
            asset.GlobalPosition = new UnityEngine.Vector3(model.GlobalPosition.X, model.GlobalPosition.Y, model.GlobalPosition.Z);

            Matrix4x4 globalRotation = Matrix4x4.identity;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    globalRotation[i, j] = model.GlobalRotation[i, j];
                }
            }
            asset.GlobalRotation = globalRotation.rotation;
            float y = asset.GlobalRotation.y;
            asset.GlobalRotation.y = asset.GlobalRotation.z;
            asset.GlobalRotation.z = y;

            Matrix4x4 localRotation = Matrix4x4.identity;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    localRotation[i, j] = model.LocalRotation[i, j];
                }
            }
            asset.LocalRotation = localRotation.rotation;
            float y2 = asset.LocalRotation.y;
            asset.LocalRotation.y = asset.LocalRotation.z;
            asset.LocalRotation.z = y2;

            asset.LocalSize = new UnityEngine.Vector3Int(model.LocalPosition.X, model.LocalPosition.Y, model.LocalPosition.Z);
            asset.LocalPosition = new UnityEngine.Vector3(model.LocalPosition.X, model.LocalPosition.Y, model.LocalPosition.Z);
            // asset.GlobalRotation = new UnityEngine.Vector3(model.GlobalRotation[0,0],);
            asset.sizeVoxel = 1f; // / Mathf.Min(CubeVoxels.size.x, CubeVoxels.size.y, CubeVoxels.size.z);
            string[] namePathArray = pathFile.Split(new char[] { '/' });
            string nameFolderModel = Path.GetFileNameWithoutExtension(namePathArray[namePathArray.Length - 1]);

            string modelName = models[m].Name;
            if (!AssetDatabase.IsValidFolder($"{OutputPath}/SO/{folderPrefix}/{nameFolderModel}"))
            {
                AssetDatabase.CreateFolder($"{OutputPath}/SO/{folderPrefix}", nameFolderModel);
            }
            string path = AssetDatabase.GenerateUniqueAssetPath($"{OutputPath}/SO/{folderPrefix}/{nameFolderModel}/{modelName}_{nameFolderModel}.asset");

            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

        }

    }

}
#endif