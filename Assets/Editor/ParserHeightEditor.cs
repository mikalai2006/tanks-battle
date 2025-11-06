using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ParserHeight))]
public class ParserHeightEditor : Editor
{
    private Vector2 scrollPosition;
    private int cellSize = 5;
    private int margin = 0;
    private int padding = 0;
    Color colorEmptyVoxel;

    public override void OnInspectorGUI()
    {
        colorEmptyVoxel = new Color(50f/255f, 50f/255f, 50f/255f, 1);

        DrawDefaultInspector();

        ParserHeight target = (ParserHeight)base.target;

        if (target == null) {
            return;
        }

        if (GUILayout.Button("Create data"))
        {
            target.Init();
            target.GenerateHeightMap();
        }


            EditorGUILayout.Space(); // Add some space for separation
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(800)); // Example: fixed height of 200

            // Start a horizontal group for the grid
            GUILayout.BeginVertical();

            var styleText = new GUIStyle();
            styleText.fontSize = 5;

            var styleLabel = new GUIStyle();
            styleLabel.fontSize = 10;
            styleLabel.normal.textColor = Color.green;

            var allSpace = target.gridSize.x * cellSize + target.gridSize.y * margin + 10;// + target.TileSideVoxels * padding;

        if (target.data != null && target.data.Count > 0)
        {
            // Section draw.
            for (int y = 0; y < target.gridSize.y; y++)
            {
                GUILayout.BeginHorizontal(); // Start a horizontal group for each row
                for (int x = 0; x < target.gridSize.x; x++)
                {
                    // Calculate the position and size of the current rectangle
                    // Rect rect = GUILayoutUtility.GetRect(cellSize, cellSize, GUILayout.Width(cellSize), GUILayout.Height(cellSize));

                    Rect rect = new Rect();

                    // Adjust for padding and margin
                    rect.x += x * (cellSize + padding) + margin;
                    rect.y += allSpace - (y * (cellSize + padding) + margin);
                    rect.width = cellSize;
                    rect.height = cellSize;
                    var valueHeight = target.data[new Vector2Int(x, y)];
                    Color color = new Color(valueHeight / target._settings.heightSize, valueHeight / target._settings.heightSize, valueHeight / target._settings.heightSize, 1);
                    // Debug.Log($"r={row},c={column}::: rect.x={rect.x}, rect.y={rect.y}, rect.width={rect.width}, rect.height={rect.height}, color={color}");

                    // Draw the rectangle
                    EditorGUI.DrawRect(rect, color); // Replace GetColorForCell with your logic

                    // EditorGUI.LabelField(rect, new GUIContent("1", "This is a helpful tooltip for My Label."));
                }
                GUILayout.EndHorizontal(); // End the horizontal group for the row
            }
        }
        GUILayout.Label($"Grid", styleLabel);
        GUILayout.Space(allSpace);

        
        Handles.color = Color.yellow;
        var centerRects = target.gridSize.x/2 * (cellSize + padding + margin);
        Handles.DrawLine(new Vector2(centerRects, 0), new Vector2(centerRects, 1000));

        // End the scroll view
        EditorGUILayout.EndScrollView();

        GUILayout.EndVertical(); // End the vertical group for the grid
    }
}
