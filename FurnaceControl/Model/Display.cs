namespace FurnaceControl
{
    using System;
    using System.Linq;
    using System.Drawing;
    using System.Collections.Generic;

    public class Display
    {
        // Static Constants
        private static readonly float MaxTemp = 1400.0f;    // Maximal Temperature that furnace can handle
        private static readonly Font fontBig = new Font(FontFamily.GenericMonospace, 25, FontStyle.Bold);
        private static readonly Font fontSmall = new Font(FontFamily.GenericMonospace, 10, FontStyle.Bold);
        private static readonly StringFormat center = new StringFormat() { Alignment = StringAlignment.Center };

        // Properties
        public int Temperature = 0;
        public List<Measurement> Measurements { get; set; } = new List<Measurement>();
        public FiringPlan Plan { get; set; } = new FiringPlan();
        public List<Heating> Heatings { get; set; } = new List<Heating>();
        public bool SmokeStackClosed { get; set; } = false;
        public int ProgramCounter { get; set; } = -1;
        public DateTime Start { get; set; }

        // Constructor
        public Display() { }

        // Private Methods
        private void DrawTemperatures(Graphics g)
        {
            if (Measurements.Count > 1)
            {
                var size = g.ClipBounds.Size;
                DateTime first = Start;
                DateTime last = Measurements.Last().Time;
                float duration = (float)(last - first).TotalSeconds;
                float stepX = size.Width / duration;
                float stepY = size.Height / (MaxTemp * 1.0f);

                List<PointF> GraphPoints = new List<PointF>();
                foreach (Measurement m in Measurements)
                {
                    float x = (float)(m.Time - first).TotalSeconds;
                    GraphPoints.Add(new PointF(x * stepX, size.Height - (m.Temperature * stepY)));
                }
                var LinePen = new Pen(Brushes.Red, 3);
                g.DrawLines(LinePen, GraphPoints.ToArray());
                var lp = GraphPoints.Last();
                g.FillEllipse(Brushes.Red, new RectangleF(lp.X - 5, lp.Y - 5, 10, 10));
            }
        }

        private void DrawProgram(Graphics g)
        {
            if (Plan == null) { return; }
            int marginLeft = 70;
            g.DrawString($"{Plan.Name}", fontBig, Brushes.Black, new Point(marginLeft, 10));
            for (int i = 0; Plan.Blocks != null && i < Plan.Blocks.Count(); i++)
            {
                var block = Plan.Blocks[i];
                string currentBlock = $"{block.TargetTemperature.ToString().PadLeft(4, ' ')}°C {block.TemperingDuration.ToString().PadLeft(3, ' ')}min {(int)block.PowerDrain}kW";
                if (ProgramCounter == i)
                {
                    currentBlock += " <<<";
                }
                g.DrawString(currentBlock, fontSmall, Brushes.Black, new PointF(marginLeft, 13 + fontBig.Height + (i * (fontSmall.Height + 3))));
            }
        }

        private void DrawHeatings(Graphics g)
        {
            if (Heatings.Count > 0)
            {
                var size = g.ClipBounds.Size;
                DateTime last = Measurements.Last().Time;
                float stepX = size.Width / (float)(last - Start).TotalSeconds;
                float stepY = (size.Height / MaxTemp) * 10;

                for (int h = 0; h < Heatings.Count; h++)
                {
                    float x1 = (float)(Heatings[h].StartTime - Start).TotalSeconds;
                    float x2 = (float)(DateTime.Now - Start).TotalSeconds;
                    if (h < Heatings.Count - 1)
                    {
                        x2 = (float)(Heatings[h + 1].StartTime - Start).TotalSeconds;
                    }

                    float height = size.Height - ((int)Heatings[h].Power * stepY);
                    float from = x1 * stepX;
                    float lenght = (x2 * stepX) - from;
                    var r = new RectangleF(from, height, lenght, size.Height);
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
                DrawHeatings(g);
                DrawTemperatures(g);

                Rectangle CurrentTemperatureBox = new Rectangle((int)g.ClipBounds.Width - 204, 2, 202, 42);

                g.FillRectangle(Brushes.Yellow, CurrentTemperatureBox);
                g.DrawRectangle(new Pen(Brushes.Black, 2), CurrentTemperatureBox);
                g.DrawString(Temperature + "°C", fontBig, Brushes.Red, CurrentTemperatureBox, center);
            }
            DrawProgram(g);
            g.Flush();
        }
    }
}