
using UnityEngine;
    using UnityEngine.Tilemaps;
[CreateAssetMenu(fileName = "RuleTileRotated", menuName = "Tiles3d /Custom Rotated Rule Tile")]
public class RuleTileRotated : RuleTile
{
private Vector3 rotationEuler = new Vector3(0,0, 0);
    // public float yRotationAngle = 0f; // Set this in the Inspector for each rule
    // public float xRotationAngle = 0f; // Set this in the Inspector for each rule

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        Matrix4x4 transformationMatrix = Matrix4x4.TRS(position, tileData.transform.rotation, tileData.transform.lossyScale);
        var m02 = transformationMatrix.m02;
        var m12 = transformationMatrix.m12;
        var m22 = transformationMatrix.m22;

        transformationMatrix.m02 = transformationMatrix.m01;
        transformationMatrix.m12 = transformationMatrix.m21;
        transformationMatrix.m22 = transformationMatrix.m11;

        transformationMatrix.m01 = m02;
        transformationMatrix.m11 = m22;
        transformationMatrix.m21 = m12;

        tileData.transform  = transformationMatrix;
        
        base.GetTileData(position, tilemap, ref tileData);

        // // Apply custom Y-axis rotation
        // Quaternion currentRotation = tileData.transform.rotation;
        // Vector3 rotationEuler = new Vector3(xRotationAngle, currentRotation.y * yRotationAngle, 0);
        // Quaternion Rotation = Quaternion.Euler(rotationEuler);
        // // tileData.transform.rotation = currentRotation * yRotation;


        // localRotation = Quaternion.LookRotation(new Vector3(transform.m02, transform.m12, transform.m22), new Vector3(transform.m01, transform.m11, transform.m21));
        // localRotation = Quaternion.LookRotation(new Vector3(transform.m01, transform.m21, transform.m11), new Vector3(transform.m02, transform.m22, transform.m12));
        
    }

    // public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject instantiatedGameObject)
    // {
    //     if (instantiatedGameObject != null)
    //     {
    //         instantiatedGameObject.transform.Rotate(rotationEuler);
    //         }

    //     return base.StartUp(position, tilemap, instantiatedGameObject);
    // }
}
