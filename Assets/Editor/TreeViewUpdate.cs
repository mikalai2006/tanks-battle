// using UnityEditor;
//      using UnityEngine.UIElements;
//      using UnityEditor.UIElements;
//      using UnityEngine;
// using System.Collections.Generic;

// public class TreeViewUpdate : EditorWindow
//      {
//          [MenuItem("Planets/Standard Tree")]
//          public static void ShowWindow()
//          {
//              GetWindow<TreeViewUpdate>("Planets Tree");
//          }

//          public VisualTreeAsset uxml;

//          public void CreateGUI()
//          {
//              // Загрузка UXML файла
//              uxml.CloneTree(rootVisualElement);

//              // Получение TreeView из UXML
//              var treeView = rootVisualElement.Q<TreeView>("tree-view");

//              // Пример данных (можно заменить на свои)
//                 var data = new List<TreeViewItemData<string>>();
//                 data.Add(new TreeViewItemData<string>(1, "Planets", null));

//                 var data1 = new List<TreeViewItemData<string>>();
//                 data1.Add(new TreeViewItemData<string>(2, "  - Mercury"));

//                 data.Add(new TreeViewItemData<string>(3, "   - Childrens", data1));
                
//             //  data.Add(new TreeViewItemData<string>(3, "  - Venus", 1));
//         //  data.Add(new TreeViewItemData<string>(4, "  - Earth", 1));
//         //  data.Add(new TreeViewItemData<string>(5, "  - Mars", 1));
//         //  data.Add(new TreeViewItemData<string>(6, "Moons", null));
//         //  data.Add(new TreeViewItemData<string>(7, "  - Earth's Moon", 6));

//         // Заполнение TreeView
//         treeView.SetRootItems(data);
//              treeView.ExpandAll();

//              // Обработчик выбора элементов (опционально)
//              treeView.onItemsChosen += items =>
//              {
//                  foreach (var item in items)
//                  {
//                      Debug.Log($"Выбран элемент: {item}");
//                  }
//              };
//          }
//      }