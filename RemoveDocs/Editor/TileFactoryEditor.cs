using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

[CustomEditor(typeof(TileFactory))]
public class TileFactoryEditor : Editor
{

    public override void OnInspectorGUI()
    {
        GameSetting asset = (GameSetting)Resources.Load("GlobalOption");

        base.OnInspectorGUI();

        EditorGUILayout.Space(30);
        
        TileFactory tileF = (TileFactory)target;

        var pos = GUILayoutUtility.GetLastRect();

        EditorGUILayout.BeginVertical();

        var buttonCreate = GUI.Button(pos, "Clear tiles");
        if (buttonCreate)
        {
            // tileF.land.ClearAllTiles();
            // EditorUtility.SetDirty(target);
            BoundsInt bounds = tileF.land.cellBounds;
            TileBase[] allTiles = tileF.land.GetTilesBlock(bounds);
            
             for (int x = 0; x < bounds.size.x; x++) {
                for (int y = 0; y < bounds.size.y; y++) {
                    TileBase tile = allTiles[x + y * bounds.size.x];
                    if (tile != null) {
                        Debug.Log("x:" + x + " y:" + y + " tile:" + tile.name);
                    } else {
                        Debug.Log("x:" + x + " y:" + y + " tile: (null)");
                    }
                }
            }    
        }



        EditorGUILayout.EndVertical();

    }
}
