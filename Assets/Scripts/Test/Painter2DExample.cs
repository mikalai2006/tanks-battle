using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class Painter2DExample : MonoBehaviour
{
    public void OnEnable()
    {
        var doc = GetComponent<UIDocument>();
        doc.rootVisualElement.generateVisualContent += Draw;
    }

    void Draw(MeshGenerationContext ctx)
    {
        var painter = ctx.painter2D;
        painter.lineWidth = 10.0f;
        painter.lineCap = LineCap.Round;
        painter.strokeColor = Color.red;

        painter.BeginPath();
        painter.MoveTo(new Vector2(10, 10));
        painter.BezierCurveTo(new Vector2(100, 100), new Vector2(200, 0), new Vector2(300, 100));
        painter.Stroke();
    }
}