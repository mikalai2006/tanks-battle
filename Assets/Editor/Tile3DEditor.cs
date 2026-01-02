using Mikalai2006.Voxel;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Tile3D))]
public class Tile3DEditor : Editor
{
    private Vector2 scrollPosition;
    private int cellSize = 5;
    private int margin = 0;
    private int padding = 0;
    Color colorEmptyVoxel;

// public override bool UseDefaultMargins()
    //         {
    //             return false; // Disables default margins for this Inspector
    //         }

    public override void OnInspectorGUI()
    {
        colorEmptyVoxel = new Color(50f/255f, 50f/255f, 50f/255f, 1);

        DrawDefaultInspector();

        Tile3D target = (Tile3D)base.target;

        if (target == null) {
            return;
        }

        VoxelMeshRender voxelMeshRender = target.transform.GetComponentInChildren<VoxelMeshRender>();

        if (voxelMeshRender == null)
        {
            return;
        }

        target.voxelMeshRender = voxelMeshRender;

        target.TileSideVoxels = target.voxelMeshRender.Config.sOVoxelData.Bounds.x;

        // if (GUILayout.Button("Refresh data"))
        // {
        //     target.OnRefreshData();
        // }

        // if (!Application.isPlaying)
        // {
        //     target.OnStart();
        // }

        if (target.ColorsForward == null || target.ColorsForward.Length == 0)
        {
            return;
        }

        // for (var i = 0; i < target.ColorsForward.Length; i++)
        // {
        //     Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(10));
        //     rect.x += 10;
        //     rect.width -= 20;

        //     EditorGUI.DrawRect(rect, target.ColorsForward[i].color);
        // }

        EditorGUILayout.Space(); // Add some space for separation
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(1100)); // Example: fixed height of 200


        // Start a horizontal group for the grid
        GUILayout.BeginVertical();

        var styleText = new GUIStyle();
        styleText.fontSize = 5;
        
        var styleLabel = new GUIStyle();
        styleLabel.fontSize = 10;
        styleLabel.normal.textColor = Color.green;

        var allSpace = target.TileSideVoxels * cellSize + target.TileSideVoxels * margin + 10;// + target.TileSideVoxels * padding;
        var defaultSpace = target.TileSideVoxels * cellSize + target.TileSideVoxels * margin;

        // Section forward
        for (int row = 0; row < target.TileSideVoxels; row++)
        {
            GUILayout.BeginHorizontal(); // Start a horizontal group for each row
            for (int column = 0; column < target.TileSideVoxels; column++)
            {
                // Calculate the position and size of the current rectangle
                // Rect rect = GUILayoutUtility.GetRect(cellSize, cellSize, GUILayout.Width(cellSize), GUILayout.Height(cellSize));

                Rect rect = new Rect();

                // Adjust for padding and margin
                rect.x += column * (cellSize + padding) + margin;
                rect.y += allSpace - (row * (cellSize + padding) + margin);
                rect.width = cellSize;
                rect.height = cellSize;
                Color color = target.ColorsForward[row * target.TileSideVoxels + column].color; //target.GetVoxelColor(row, column, DirectionSideTile.Forward).color;
                if (color == Color.clear)
                {
                    color = colorEmptyVoxel;
                }
                // Debug.Log($"r={row},c={column}::: rect.x={rect.x}, rect.y={rect.y}, rect.width={rect.width}, rect.height={rect.height}, color={color}");

                // Draw the rectangle
                EditorGUI.DrawRect(rect, color); // Replace GetColorForCell with your logic

                // EditorGUI.LabelField(rect, new GUIContent("1", "This is a helpful tooltip for My Label."));
            }
            GUILayout.EndHorizontal(); // End the horizontal group for the row
        }
        GUILayout.Label($"Forward ({target.ColorsForward.Length} - negZ={target.tileSockets.negZ})", styleLabel);
        GUILayout.Space(allSpace);

        // Section back
        allSpace += defaultSpace + 20;
        for (int row = 0; row < target.TileSideVoxels; row++)
        {
            GUILayout.BeginHorizontal(); // Start a horizontal group for each row
            for (int column = 0; column < target.TileSideVoxels; column++)
            {
                // Calculate the position and size of the current rectangle
                // Rect rect = GUILayoutUtility.GetRect(cellSize, cellSize, GUILayout.Width(cellSize), GUILayout.Height(cellSize));

                Rect rect = new Rect();

                // Adjust for padding and margin
                rect.x += column * (cellSize + padding) + margin;
                rect.y += allSpace - (row * (cellSize + padding) + margin);
                rect.width = cellSize;
                rect.height = cellSize;
                Color color = target.ColorsBack[row * target.TileSideVoxels + column].color; //target.GetVoxelColor(row, column, DirectionSideTile.Forward).color;
                if (color == Color.clear)
                {
                    color = colorEmptyVoxel;
                }
                // Debug.Log($"r={row},c={column}::: rect.x={rect.x}, rect.y={rect.y}, rect.width={rect.width}, rect.height={rect.height}, color={color}");

                // Draw the rectangle
                EditorGUI.DrawRect(rect, color); // Replace GetColorForCell with your logic

                // EditorGUI.LabelField(rect, new GUIContent("1", "This is a helpful tooltip for My Label."));
            }
            GUILayout.EndHorizontal(); // End the horizontal group for the row
        }
        GUILayout.Label($"Back ({target.ColorsBack.Length} - posZ={target.tileSockets.posZ})", styleLabel);
        GUILayout.Space(defaultSpace);

        // Section left
        allSpace += defaultSpace + 12;
        // var opt = new GUIStyle();
        // opt.margin = new RectOffset(0,0,0,allSpace);
        for (int row = 0; row < target.TileSideVoxels; row++)
        {
            GUILayout.BeginHorizontal(); // Start a horizontal group for each row
            for (int column = 0; column < target.TileSideVoxels; column++)
            {
                // Calculate the position and size of the current rectangle
                // Rect rect = GUILayoutUtility.GetRect(cellSize, cellSize, GUILayout.Width(cellSize), GUILayout.Height(cellSize));

                Rect rect = new Rect();

                // Adjust for padding and margin
                rect.x += column * (cellSize + padding) + margin;
                rect.y += allSpace - (row * (cellSize + padding) + margin);
                rect.width = cellSize;
                rect.height = cellSize;
                Color color = target.ColorsLeft[row * target.TileSideVoxels + column].color; //target.GetVoxelColor(row, column, DirectionSideTile.Forward).color;
                if (color == Color.clear)
                {
                    color = colorEmptyVoxel;
                }
                // Debug.Log($"r={row},c={column}::: rect.x={rect.x}, rect.y={rect.y}, rect.width={rect.width}, rect.height={rect.height}, color={color}");

                // Draw the rectangle
                EditorGUI.DrawRect(rect, color); // Replace GetColorForCell with your logic

                // EditorGUI.LabelField(rect, new GUIContent("1", "This is a helpful tooltip for My Label."));
            }
            GUILayout.EndHorizontal(); // End the horizontal group for the row
        }
        GUILayout.Label($"Left ({target.ColorsLeft.Length} - negX={target.tileSockets.negX})", styleLabel);
        GUILayout.Space(defaultSpace);

        // Section right
        allSpace += defaultSpace + 14;
        // var opt = new GUIStyle();
        // opt.margin = new RectOffset(0,0,0,allSpace);
        for (int row = 0; row < target.TileSideVoxels; row++)
        {
            GUILayout.BeginHorizontal(); // Start a horizontal group for each row
            for (int column = 0; column < target.TileSideVoxels; column++)
            {
                // Calculate the position and size of the current rectangle
                // Rect rect = GUILayoutUtility.GetRect(cellSize, cellSize, GUILayout.Width(cellSize), GUILayout.Height(cellSize));

                Rect rect = new Rect();

                // Adjust for padding and margin
                rect.x += column * (cellSize + padding) + margin;
                rect.y += allSpace - (row * (cellSize + padding) + margin);
                rect.width = cellSize;
                rect.height = cellSize;
                Color color = target.ColorsRight[row * target.TileSideVoxels + column].color; //target.GetVoxelColor(row, column, DirectionSideTile.Forward).color;
                if (color == Color.clear)
                {
                    color = colorEmptyVoxel;
                }
                // Debug.Log($"r={row},c={column}::: rect.x={rect.x}, rect.y={rect.y}, rect.width={rect.width}, rect.height={rect.height}, color={color}");

                // Draw the rectangle
                EditorGUI.DrawRect(rect, color); // Replace GetColorForCell with your logic

                // EditorGUI.LabelField(rect, new GUIContent("1", "This is a helpful tooltip for My Label."));
            }
            GUILayout.EndHorizontal(); // End the horizontal group for the row
        }
        GUILayout.Label($"Right ({target.ColorsRight.Length} - posX={target.tileSockets.posX})", styleLabel);
        GUILayout.Space(defaultSpace);

        // Section top
        allSpace += defaultSpace + 14;
        // var opt = new GUIStyle();
        // opt.margin = new RectOffset(0,0,0,allSpace);
        for (int row = 0; row < target.TileSideVoxels; row++)
        {
            GUILayout.BeginHorizontal(); // Start a horizontal group for each row
            for (int column = 0; column < target.TileSideVoxels; column++)
            {
                // Calculate the position and size of the current rectangle
                // Rect rect = GUILayoutUtility.GetRect(cellSize, cellSize, GUILayout.Width(cellSize), GUILayout.Height(cellSize));

                Rect rect = new Rect();

                // Adjust for padding and margin
                rect.x += column * (cellSize + padding) + margin;
                rect.y += allSpace - (row * (cellSize + padding) + margin);
                rect.width = cellSize;
                rect.height = cellSize;
                Color color = (target.ColorsTop != null && target.ColorsTop.Length > 0) ? target.ColorsTop[row * target.TileSideVoxels + column].color: Color.clear; //target.GetVoxelColor(row, column, DirectionSideTile.Forward).color;
                if (color == Color.clear)
                {
                    color = colorEmptyVoxel;
                }
                // Debug.Log($"r={row},c={column}::: rect.x={rect.x}, rect.y={rect.y}, rect.width={rect.width}, rect.height={rect.height}, color={color}");

                // Draw the rectangle
                EditorGUI.DrawRect(rect, color); // Replace GetColorForCell with your logic

                // EditorGUI.LabelField(rect, new GUIContent("1", "This is a helpful tooltip for My Label."));
            }
            GUILayout.EndHorizontal(); // End the horizontal group for the row
        }
        var countTop = target.ColorsTop != null ? target.ColorsTop.Length : 0;
        GUILayout.Label($"Top ({countTop} - posY={target.tileSockets.posY})", styleLabel);
        GUILayout.Space(defaultSpace);

        
        // Section bottom
        allSpace += defaultSpace + 14;
        // var opt = new GUIStyle();
        // opt.margin = new RectOffset(0,0,0,allSpace);
        for (int row = 0; row < target.TileSideVoxels; row++)
        {
            GUILayout.BeginHorizontal(); // Start a horizontal group for each row
            for (int column = 0; column < target.TileSideVoxels; column++)
            {
                // Calculate the position and size of the current rectangle
                // Rect rect = GUILayoutUtility.GetRect(cellSize, cellSize, GUILayout.Width(cellSize), GUILayout.Height(cellSize));

                Rect rect = new Rect();

                // Adjust for padding and margin
                rect.x += column * (cellSize + padding) + margin;
                rect.y += allSpace - (row * (cellSize + padding) + margin);
                rect.width = cellSize;
                rect.height = cellSize;
                Color color = (target.ColorsBottom != null && target.ColorsBottom.Length > 0) ? target.ColorsBottom[row * target.TileSideVoxels + column].color: Color.clear; //target.GetVoxelColor(row, column, DirectionSideTile.Forward).color;
                if (color == Color.clear)
                {
                    color = colorEmptyVoxel;
                }
                // Debug.Log($"r={row},c={column}::: rect.x={rect.x}, rect.y={rect.y}, rect.width={rect.width}, rect.height={rect.height}, color={color}");

                // Draw the rectangle
                EditorGUI.DrawRect(rect, color); // Replace GetColorForCell with your logic

                // EditorGUI.LabelField(rect, new GUIContent("1", "This is a helpful tooltip for My Label."));
            }
            GUILayout.EndHorizontal(); // End the horizontal group for the row
        }
        var countBottom = target.ColorsBottom != null ? target.ColorsBottom.Length : 0;
        GUILayout.Label($"Bottom ({countBottom} - negY={target.tileSockets.negY})", styleLabel);
        GUILayout.Space(defaultSpace);
        
        Handles.color = Color.yellow;
        var centerRects = target.TileSideVoxels/2 * (cellSize + padding + margin);
        Handles.DrawLine(new Vector2(centerRects, 0), new Vector2(centerRects, 1100));

        // End the scroll view
        EditorGUILayout.EndScrollView();

        GUILayout.EndVertical(); // End the vertical group for the grid
    }
}
