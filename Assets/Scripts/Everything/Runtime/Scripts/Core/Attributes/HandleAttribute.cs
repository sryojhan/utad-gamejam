using UnityEngine;

public class HandleAttribute : PropertyAttribute
{
    public bool useWorldSpace;

    public bool useLocalSpace => !useWorldSpace;

    public bool drawLabel;
    public bool drawConnectionLine;
    public Color Color { get; private set; }

    public HandleAttribute()
    {
        drawLabel = true;
        drawConnectionLine = true;
        Color = Color.cyan;
    }

    public HandleAttribute(float r, float g, float b)
    {
        Color = new Color(r, g, b, 1f);
    }
}