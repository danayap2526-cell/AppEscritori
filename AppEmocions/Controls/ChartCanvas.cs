using System.Drawing.Drawing2D;

namespace AppEmocions.Controls;

public class ChartCanvas : Panel
{
    private readonly EstilGrafico _estil;

    private readonly Color turquesa = Color.FromArgb(54, 184, 177);
    private readonly Color naranja = Color.FromArgb(235, 194, 168);
    private readonly Color gris = Color.FromArgb(135, 145, 165);

    // Línea: (etiqueta, valor 0-100)
    private List<(string Etiqueta, float Valor)> _serie = new();

    // Barras: (etiqueta, % acierto, % error)
    private List<(string Etiqueta, float Aciertos, float Errores)> _barras = new();

    public ChartCanvas(EstilGrafico estil)
    {
        _estil = estil;
        DoubleBuffered = true;
        ResizeRedraw = true;
        BackColor = Color.White;
    }

    public void SetLinea(IEnumerable<(string, float)> datos)
    {
        _serie = datos.ToList();
        Invalidate();
    }

    public void SetBarras(IEnumerable<(string, float, float)> datos)
    {
        _barras = datos.ToList();
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        bool vacio = _estil == EstilGrafico.Linea ? _serie.Count == 0 : _barras.Count == 0;

        if (vacio)
        {
            DibujarVacio(g);
            return;
        }

        if (_estil == EstilGrafico.Linea)
            DibujarLinea(g);
        else
            DibujarBarras(g);
    }

    private void DibujarVacio(Graphics g)
    {
        const string mensaje = "Sense dades per al període";

        using Font fuente = new Font("Segoe UI", 9);
        using Brush pincel = new SolidBrush(gris);

        SizeF medida = g.MeasureString(mensaje, fuente);
        g.DrawString(mensaje, fuente, pincel,
            (Width - medida.Width) / 2, (Height - medida.Height) / 2);
    }

    private void DibujarLinea(Graphics g)
    {
        int ancho = ClientSize.Width;
        int alto = ClientSize.Height;

        if (ancho < 100 || alto < 100)
            return;

        int izquierda = 45;
        int derecha = ancho - 20;
        int arriba = 25;
        int abajo = alto - 45;

        using Pen rejilla = new Pen(Color.FromArgb(225, 232, 239), 1);
        rejilla.DashStyle = DashStyle.Dash;

        using Font fuente = new Font("Segoe UI", 7);
        using Brush pincel = new SolidBrush(Color.FromArgb(145, 155, 175));

        // Eje Y: 100, 75, 50, 25, 0
        for (int i = 0; i < 5; i++)
        {
            int y = arriba + i * (abajo - arriba) / 4;
            g.DrawLine(rejilla, izquierda, y, derecha, y);
            g.DrawString((100 - 25 * i) + "%", fuente, pincel, 0, y - 7);
        }

        int n = _serie.Count;
        PointF[] puntos = new PointF[n];

        for (int i = 0; i < n; i++)
        {
            float x = n == 1
                ? (izquierda + derecha) / 2f
                : izquierda + i * (derecha - izquierda) / (float)(n - 1);

            float valor = Math.Max(0, Math.Min(100, _serie[i].Valor));
            float y = abajo - valor / 100f * (abajo - arriba);

            puntos[i] = new PointF(x, y);
        }

        if (n >= 2)
        {
            using GraphicsPath camino = new GraphicsPath();
            camino.AddCurve(puntos, 0.35f);

            using GraphicsPath area = (GraphicsPath)camino.Clone();

            area.AddLine(puntos[n - 1].X, puntos[n - 1].Y, puntos[n - 1].X, abajo);
            area.AddLine(puntos[n - 1].X, abajo, puntos[0].X, abajo);
            area.CloseFigure();

            using LinearGradientBrush degradado = new LinearGradientBrush(
                new Rectangle(izquierda, arriba, Math.Max(1, derecha - izquierda), abajo - arriba),
                Color.FromArgb(75, 54, 184, 177),
                Color.FromArgb(0, 54, 184, 177),
                LinearGradientMode.Vertical);

            g.FillPath(degradado, area);

            using Pen linea = new Pen(turquesa, 3);
            linea.LineJoin = LineJoin.Round;
            g.DrawPath(linea, camino);
        }

        foreach (PointF punto in puntos)
        {
            g.FillEllipse(Brushes.White, punto.X - 4, punto.Y - 4, 8, 8);

            using Pen borde = new Pen(turquesa, 2.5f);
            g.DrawEllipse(borde, punto.X - 4, punto.Y - 4, 8, 8);
        }

        // Etiquetas del eje X (una de cada dos si hay muchas semanas)
        int paso = n > 10 ? 2 : 1;
        using Font fuenteSemana = new Font("Segoe UI", 7);
        using Brush pincelSemana = new SolidBrush(Color.FromArgb(135, 145, 165));

        for (int i = 0; i < n; i += paso)
        {
            SizeF medida = g.MeasureString(_serie[i].Etiqueta, fuenteSemana);
            g.DrawString(_serie[i].Etiqueta, fuenteSemana, pincelSemana,
                puntos[i].X - medida.Width / 2, abajo + 15);
        }
    }

    private void DibujarBarras(Graphics g)
    {
        int ancho = ClientSize.Width;
        int alto = ClientSize.Height;

        if (ancho < 100 || alto < 100)
            return;

        int izquierda = 10;
        int derecha = ancho - 8;
        int arriba = 25;
        int abajo = alto - 30;

        using Pen rejilla = new Pen(Color.FromArgb(225, 232, 239), 1);
        rejilla.DashStyle = DashStyle.Dash;

        for (int i = 0; i < 4; i++)
        {
            int y = arriba + i * (abajo - arriba) / 3;
            g.DrawLine(rejilla, izquierda, y, derecha, y);
        }

        int n = _barras.Count;
        int espacio = (derecha - izquierda) / n;

        using Font fuente = new Font("Segoe UI", 7);
        using Brush textoBrush = new SolidBrush(gris);

        for (int i = 0; i < n; i++)
        {
            var barra = _barras[i];
            int centro = izquierda + espacio * i + espacio / 2;

            float pctAciertos = Math.Max(0, Math.Min(100, barra.Aciertos));
            float pctErrores = Math.Max(0, Math.Min(100, barra.Errores));

            int alturaAciertos = (int)((abajo - arriba) * pctAciertos / 100);
            int alturaErrores = (int)((abajo - arriba) * pctErrores / 100);

            if (alturaAciertos >= 3)
            {
                Rectangle rectAciertos = new Rectangle(
                    centro - 15, abajo - alturaAciertos, 17, alturaAciertos);

                using SolidBrush pincelAciertos = new SolidBrush(turquesa);
                g.FillRoundedRectangle(pincelAciertos, rectAciertos, 6);
            }

            if (alturaErrores >= 3)
            {
                Rectangle rectErrores = new Rectangle(
                    centro + 7, abajo - alturaErrores, 17, alturaErrores);

                using SolidBrush pincelErrores = new SolidBrush(naranja);
                g.FillRoundedRectangle(pincelErrores, rectErrores, 6);
            }

            SizeF medida = g.MeasureString(barra.Etiqueta, fuente);

            g.DrawString(
                barra.Etiqueta, fuente, textoBrush,
                centro - medida.Width / 2, abajo + 10);
        }
    }
}

// =============================================
// EXTENSIÓN PARA BARRAS REDONDEADAS
// =============================================
