using System.Drawing.Drawing2D;

namespace AppEmocions.Controls;

public static class GraphicsExtensions
{
    public static void FillRoundedRectangle(
        this Graphics graphics, Brush brush, Rectangle bounds, int radius)
    {
        using GraphicsPath path = new GraphicsPath();

        int diameter = radius * 2;

        path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);

        path.CloseFigure();

        graphics.FillPath(brush, path);
    }
}
