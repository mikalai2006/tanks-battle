// using System.Collections.Generic;
// using Unity.VisualScripting;
// using UnityEditor;
// using UnityEditor.IMGUI.Controls;
// using UnityEngine;

// [CustomEditor(typeof(GameMachine))]
// public class GameMachineEditor : Editor
// {
//   private Vector2 scrollPos;

//   // private MyTreeView myTreeView;

//   // public override bool RequiresConstantRepaint()
//   // {
//   //     return true;
//   // }

//   public override void OnInspectorGUI()
//   {
//     GameSetting asset = (GameSetting)Resources.Load("GlobalOption");

//     // Optionally, draw the default inspector content
//     base.OnInspectorGUI();
//     // DrawDefaultInspector();

//     GameMachine machine = (GameMachine)target;

//     // machine.spriteBody = (Sprite)EditorGUILayout.ObjectField(machine.spriteBody, typeof(Sprite), false, GUILayout.Width(110), GUILayout.Height(85));

//     var offset = 50f;
//     var pos = GUILayoutUtility.GetLastRect();
//     Vector2 sizeBody = Vector2.zero;

//     GUI.color = Color.yellow;
//     EditorGUILayout.BeginVertical();

//     // GUILayout.Label($"ВАЖНО: спрайты дула - пивот left-center");
//     // scrollPos = EditorGUILayout.BeginScrollView(pos.position,GUILayout.Height(150));
//     // Draw body.
//     if (machine.body && machine.body.spriteBody)
//     {
//       var textureBody = AssetPreview.GetAssetPreview(machine.body.spriteBody);
//       if (textureBody)
//       {
//         sizeBody = textureBody.Size(); //machine.body.spriteBody.rect.size; //
//       GUILayout.Label($"Size: {sizeBody.x}x{sizeBody.y}/{machine.body.spriteBody.rect.size}");

//         // Draw caterpillars.
//         for (int k = 0; k < machine.catterpillars.Count; k++)
//         {
//           GameCaterpillarOption gameCaterpillar = machine.catterpillars[k];

//           if (gameCaterpillar.Config && gameCaterpillar.Config.sprite)
//           {
//             var textureCat = AssetPreview.GetAssetPreview(gameCaterpillar.Config.sprite);
//             if (textureCat)
//             {
//               // Debug.Log($"textureCat w={gameCaterpillar.spriteCat.rect.width}/h={gameCaterpillar.spriteCat.rect.height}");
//               Rect rectCat = new Rect(
//                 pos.x + (sizeBody.x / 2 - gameCaterpillar.Config.sprite.rect.width / 2) + (gameCaterpillar.offsetCat.x * gameCaterpillar.Config.sprite.pixelsPerUnit),
//                 pos.y + offset + (sizeBody.y / 2 - gameCaterpillar.Config.sprite.rect.height / 2) - (gameCaterpillar.offsetCat.y * gameCaterpillar.Config.sprite.pixelsPerUnit),
//                 gameCaterpillar.Config.sprite.rect.width * (100 / gameCaterpillar.Config.sprite.pixelsPerUnit),
//                 gameCaterpillar.Config.sprite.rect.height * (100 / gameCaterpillar.Config.sprite.pixelsPerUnit)
//               );

//               GUI.color = gameCaterpillar.colorCat;
//               GUI.DrawTexture(rectCat, textureCat, ScaleMode.ScaleToFit, true, 0);

//               Color colr = Color.white;
//               colr.a = 0;

//               EditorGUI.DrawRect(rectCat, colr);
//             }
//           }
//         }

//         Rect rect = new Rect(
//           pos.x,
//           pos.y + offset,
//           sizeBody.x,
//           sizeBody.y
//         );

//         Color col = Color.white;
//         col.a = 0;
//         EditorGUI.DrawRect(rect, col);

//         GUI.color = machine.colorBody;
//         GUI.DrawTexture(rect, textureBody, ScaleMode.ScaleToFit, true, 1.5f);

//       }
//     }

//     // Draw tower.
//     var parentTowers = machine.towers.FindAll(t => !t.isChildren);
//     DrawTower(pos, offset, sizeBody, asset, parentTowers);


//     for (int x = 0; x < 25; x++)
//     {
//       // Loop through as many times as needed
//       EditorGUILayout.Space();
//     }

//     EditorUtility.SetDirty(target);
//     Repaint();


//     EditorGUILayout.EndVertical();
//     // EditorGUILayout.EndScrollView();

//     // // Custom TreeView
//     // serializedObject.Update();

//     // if (myTreeView == null)
//     // {
//     //     myTreeView = new MyTreeView(new TreeViewState(), serializedObject);
//     //     myTreeView.Reload();
//     // }

//     // myTreeView.OnGUI(GUILayoutUtility.GetRect(0, 1000, 200, 500));

//     // serializedObject.ApplyModifiedProperties();
//   }
  
// // class MyTreeView : TreeView
// //     {
// //         private SerializedObject serializedObject;
// //         public MyTreeView(TreeViewState state, SerializedObject serializedObject) : base(state)
// //         {
// //             this.serializedObject = serializedObject;
// //             Reload();
// //         }

// //         protected override TreeViewItem BuildRoot()
// //         {
// //             var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
// //             var allItems = new List<TreeViewItem>();

// //             // Populate allItems with your data (e.g., from serializedObject)
// //             // Example:
// //             // for (int i = 0; i < serializedObject.targetObjects.Length; i++)
// //             // {
// //             //     var item = new TreeViewItem { id = i + 1, depth = 0, displayName = serializedObject.targetObjects[i].name };
// //             //     allItems.Add(item);
// //             // }


// //             SetupDepthsFromParentsAndChildren(root, allItems); // 
// //             return root;
// //         }

// //         protected override void RowGUI(RowGUIArgs args)
// //         {
// //             // Customize row rendering here (e.g., display different controls based on item type)
// //             base.RowGUI(args);
// //         }
// //     }


//   private void DrawTower(Rect pos, float offset, Vector2 sizeBody, GameSetting asset, List<GameTowerOption> parentTowers)
//   {
//     GameMachine machine = (GameMachine)target;

//     for (int i = 0; i < parentTowers.Count; i++)
//     {
//       GameTowerOption gTower = parentTowers[i];
//       Vector2 offsetTower = Vector2.zero;

//       if (gTower.Config && gTower.Config.spriteTower)
//       {
//         var textureTower = AssetPreview.GetAssetPreview(gTower.Config.spriteTower);

//         if (textureTower)
//         {

//           // GUI.Label(rect, "Rectangle Width");
//           // Draw the sprite
//           // Draw the outline (optional)
//           // Debug.Log($"pos.x={pos.x}, sprite width={texture.width}, w={texture2.width / machine.spriteTower.pixelsPerUnit}");
//           offsetTower = new Vector2(gTower.offsetTower.x * gTower.Config.spriteTower.pixelsPerUnit, (gTower.offsetTower.y * gTower.Config.spriteTower.pixelsPerUnit));
//           var xTower = pos.x + ((sizeBody.x - textureTower.width) / 2) + offsetTower.x;
//           var yTower = pos.y + offset + ((sizeBody.y - textureTower.height) / 2) + offsetTower.y;
//           Rect rectTower = new Rect(xTower, yTower, textureTower.width, textureTower.height);
//           GUI.color = gTower.colorTower;
//           GUI.DrawTexture(rectTower, textureTower, ScaleMode.ScaleToFit, true, 1);

//           Color col = Color.white;
//           col.a = 0;

//           EditorGUI.DrawRect(rectTower, col);

//           // Draw muzzles.
//           for (int j = 0; j < gTower.muzzles.Count; j++)
//           {
//             GameMuzzleOption gMuzzleOption = gTower.muzzles[j];
//             if (gMuzzleOption.Config)
//             {
//               Sprite muzzleSprite = gMuzzleOption.Config.spriteMuzzle;
//               var textureMuzzle = AssetPreview.GetAssetPreview(muzzleSprite);

//               if (textureMuzzle)
//               {
//                 var xMuzzle = xTower + textureTower.width / 2 + (gMuzzleOption.offsetMuzzle.x * muzzleSprite.pixelsPerUnit);
//                 var yMuzzle = yTower + (gMuzzleOption.offsetMuzzle.y * muzzleSprite.pixelsPerUnit);
//                 Rect rectMuzzle = new Rect(
//                   // pos.x + (sizeBody.x / 2) + (gMuzzleOption.offsetMuzzle.x * muzzleSprite.pixelsPerUnit) + offsetTower.x,
//                   // pos.y + offset + (sizeBody.y / 2 - textureMuzzle.height / 2) - (gMuzzleOption.offsetMuzzle.y * muzzleSprite.pixelsPerUnit) - offsetTower.y,
//                   xMuzzle,
//                   yMuzzle,
//                   textureMuzzle.width,
//                   textureMuzzle.height
//                 );

//                 GUI.color = gMuzzleOption.Config.color;
//                 GUI.DrawTexture(rectMuzzle, textureMuzzle, ScaleMode.ScaleToFit, true, 1);

//                 col = Color.white;
//                 col.a = 0;

//                 EditorGUI.DrawRect(rectMuzzle, col);

//                 if (asset)
//                 {

//                   GUI.color = Color.yellow;
//                   Rect rectMuzzle2 = new Rect(
//                     // pos.x + (sizeBody.x / 2) + (gMuzzleOption.offsetMuzzle.x * asset.spriteArc5px.pixelsPerUnit) + offsetTower.x,
//                     // pos.y + offset + (sizeBody.y / 2 - asset.spriteArc5px.texture.height / 2) - (gMuzzleOption.offsetMuzzle.y * asset.spriteArc5px.pixelsPerUnit) - offsetTower.y,
//                     xMuzzle,
//                     yMuzzle + textureMuzzle.height / 2 - asset.spriteArc5px.texture.height / 2,
//                     asset.spriteArc5px.texture.width,
//                     asset.spriteArc5px.texture.height
//                   );
//                   GUI.DrawTexture(rectMuzzle2, asset.spriteArc5px.texture, ScaleMode.ScaleToFit, true, 0);

//                   Color col2 = Color.white;
//                   col2.a = 0.5f;

//                   EditorGUI.DrawRect(rectMuzzle2, col2);
//                 }
//               }
//             }
//           }

//           var childrenTowers = machine.towers.FindAll(t => gTower.children.Contains(t.ido));
//           if (childrenTowers.Count > 0)
//           {
//             DrawTower(pos, offset, sizeBody, asset, childrenTowers);
//           }
//         }
//       }

//     }
//   }
// }
