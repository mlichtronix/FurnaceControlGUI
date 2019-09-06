namespace FurnaceControl
{
    using System;
    using System.Windows.Forms;
    using System.Collections.Generic;

    public partial class ProgramDesigner : Form
    {
        bool AllValid = true;
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
            program.Name = NameBox.Text.Trim();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
