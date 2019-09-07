namespace FurnaceControl
{
    using System;
    using System.Linq;
    using System.Drawing;
    using System.Collections.Generic;

    public class Display
    {
        // Static Constants
        private static readonly float MaxTemp = 1400.0f;        // Maximal Temperature that furnace can handle        
        private static readonly Font fontBig = new Font(FontFamily.GenericMonospace, 25, FontStyle.Bold);
        private static readonly Font fontSmall = new Font(FontFamily.GenericMonospace, 10, FontStyle.Bold);

        // Properties
        public List<Measurement> Measurements { get; set; } = new List<Measurement>();
        public ProgramBlock[] Program { get; set; }
        public List<Heating> Heatings { get; set; } = new List<Heating>();
        public bool SmokeStackClosed { get; set; } = false;
        public int ProgramCounter { get; set; } = -1;
        public DateTime Start { get; set; } = DateTime.Now;

        // Constructor
        public Display() { }

        // Private Methods
        private void DrawTemperatures(Graphics g)
        {
            if (Measurements.Count > 1)
            {
                var size = g.ClipBounds.Size;
                DateTime first = Start;
                DateTime last = DateTime.Now;
                float stepX = (size.Width - 10) / (float)(last - first).TotalMinutes;     // (+1.0f = Offset to get better view and to get better precission)
                float stepY = size.Height / (MaxTemp * 1.0f);

                List<PointF> GraphPoints = new List<PointF>();
                foreach (Measurement m in Measurements)
                {
                    float x = (float)(m.Time - first).TotalMinutes;
                    GraphPoints.Add(new PointF(x * stepX, size.Height - (m.Temperature * stepY)));
                }
                g.DrawLines(new Pen(Brushes.Red, 3), GraphPoints.ToArray());
            }
        }

        private void DrawHeatings(Graphics g)
        {
            if (Heatings.Count > 0)
            {
                var size = g.ClipBounds.Size;
                DateTime first = Start;
                DateTime last = DateTime.Now;
                float stepX = (size.Width - 10) / (float)(last - first).TotalMinutes;     // (+1.0f = Offset to get better view and to get better precission)
                float stepY = size.Height / (MaxTemp * 1.0f);

                for (int h = 0; h < Heatings.Count-1; h++)
                {
                    float x1 = (float)(Heatings[h].StartTime - first).TotalMinutes;
                    float x2 = (float)(Heatings[h+1].StartTime - first).TotalMinutes;
                    var p = new PointF(x1 * stepX, size.Height - ((int)Heatings[h].Power * stepY));
                    var q = new PointF(x2 * stepX, size.Height - ((int)Heatings[h+1].Power * stepY));
                    var r = new RectangleF(p.X, p.Y, q.X, q.Y);
                    g.FillRectangle(Brushes.Lime, r);                    
                }                
            }
        }

        private void DrawGrid(Graphics g)
        {
            float height = g.ClipBounds.Size.Height;
            float stepY = height / MaxTemp;
            float width = g.ClipBounds.Size.Width;
            var pen1 = new Pen(new SolidBrush(Color.FromArgb(128, 128, 128, 128)), 0.5f);
            var pen2 = new Pen(new SolidBrush(Color.FromArgb(64, 128, 128, 128)), 1f);

            // Draw line every 50 °C
            for (int i = 50; i < MaxTemp; i += 50)
            {
                int level = (int)(height - i * stepY);
                if (i % 100 == 0)
                {
                    g.DrawLine(pen1, 0, level, width, level);
                    g.DrawString(i + "°C", fontSmall, Brushes.LightGray, new Point(2, level - fontSmall.Height));
                }
                else
                {
                    g.DrawLine(pen2, 0, level, width, level);
                }
            }

            // Draw line every 30 minutes
            if (Measurements.Count > 1)
            {
                float duration = (float)(Measurements.Last().Time - Measurements.First().Time).TotalMinutes;
                float stepX = width / duration;
                for (double i = 30; i < duration; i += 30)
                {
                    g.DrawLine(pen1, (int)(i * stepX), 0, (int)(i * stepX), height);
                    g.DrawString((i / 60.0).ToString("0.#") + "h", fontSmall, Brushes.LightGray, new Point((int)(2 + i * stepX), (int)(height - fontSmall.Height)));
                }
            }
        }

        // Public Methods
        public void Draw(Graphics g)
        {
            DrawGrid(g);
            if (Measurements.Any())
            {
                Rectangle CurrentTemperatureBox = new Rectangle((int)g.ClipBounds.Width - 204, 2, 202, 42);
                var format = new StringFormat() { Alignment = StringAlignment.Center };
                g.FillRectangle(Brushes.Yellow, CurrentTemperatureBox);
                g.DrawRectangle(new Pen(Brushes.Black, 2), CurrentTemperatureBox);

                g.DrawString(Measurements.Last().Temperature + "°C", fontBig, Brushes.Red, CurrentTemperatureBox, format);
                DrawHeatings(g);
                DrawTemperatures(g);
            }
            g.Flush();
        }
    }
}