 using UnityEditor.AssetImporters;
     using UnityEngine;
     using System.IO;
     
     [ScriptedImporter(1, "objx")]
public class ObjImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        string fileContent = File.ReadAllText(ctx.assetPath);
        TextAsset textAsset = new TextAsset(fileContent);
        ctx.AddObjectToAsset("main obj", textAsset);
        ctx.SetMainObject(textAsset);
    }
}
