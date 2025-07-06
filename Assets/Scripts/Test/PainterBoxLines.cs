using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class PainterBoxLines : MonoBehaviour
{
    private UIDocument doc;
    public void Start()
    {
        doc = GetComponent<UIDocument>();
        // VisualElement box1 = doc.rootVisualElement.Q<VisualElement>("Box1");
        // box1.generateVisualContent += Draw;
        doc.rootVisualElement.RegisterCallback<GeometryChangedEvent>(GeometryChangedCallback);
    }

    private void GeometryChangedCallback(GeometryChangedEvent evt)
    {
        doc.rootVisualElement.UnregisterCallback<GeometryChangedEvent>(GeometryChangedCallback);

        var allBoxs = doc.rootVisualElement.Query<VisualElement>(className: "tree_col").ToList();

        for (int i = 0; i < allBoxs.Count; i++)
        {
            var treeItems = allBoxs[i].Query<VisualElement>(className: "tree_item").ToList();
            for (int j = 0; j < treeItems.Count; j++)
            {
                treeItems[j].generateVisualContent += Draw;
                Debug.Log($"item0: {treeItems[j].parent.worldBound.position}");
            }
        }
    }

    void Draw(MeshGenerationContext ctx)
    {
        
        Debug.Log($"item: {ctx.visualElement.worldBound.position}");
        var painter = ctx.painter2D;
        painter.lineWidth = 2.0f;
        painter.lineCap = LineCap.Round;
        painter.strokeColor = Color.red;

        painter.BeginPath();
        painter.MoveTo(new Vector2(10, 10));
        painter.BezierCurveTo(new Vector2(100, 100), new Vector2(200, 0), new Vector2(300, 100));
        painter.Stroke();
    }
}