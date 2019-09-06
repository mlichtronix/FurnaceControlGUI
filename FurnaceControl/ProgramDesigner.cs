namespace FurnaceControl
{
    using System;
    using System.Windows.Forms;
    using System.Collections.Generic;

    public partial class ProgramDesigner : Form
    {
        public FiringProgram program { get; set; }
        private IEnumerable<string> existingNames;

        public ProgramDesigner(FiringProgram p, IEnumerable<string> names)
        {
            InitializeComponent();

            program = p;
            existingNames = names;
            NameBox.Text = program.Name;
            foreach (var block in program.Blocks)
            {
                ProgramValues.Rows.Add(block.TargetTemperature, block.TemperingDuration, block.PowerDrain);
            }
        }

        private void ApplyValues(object sender, EventArgs e)
        {            
            var blocks = new List<ProgramBlock>();
            for (int r = 0; r < ProgramValues.RowCount - 1; r++)
            {
                int c = int.Parse(ProgramValues[0, r].Value.ToString());
                int d = int.Parse(ProgramValues[1, r].Value.ToString());
                int w = int.Parse(ProgramValues[2, r].Value.ToString());
                blocks.Add(new ProgramBlock(c, d, (Wattage)w));
            }
            program.Name = NameBox.Text.Trim();
            program.Blocks = blocks.ToArray();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
