namespace FurnaceControl
{
    using System;
    using System.Windows.Forms;
    using System.Collections.Generic;

    public partial class PlanDesigner : Form
    {
        public FiringPlan plan { get; set; }
        private IEnumerable<string> existingNames;

        public PlanDesigner(FiringPlan p, IEnumerable<string> names)
        {
            InitializeComponent();

            plan = p;
            existingNames = names;
            NameBox.Text = plan.Name;
            foreach (var block in plan.Blocks)
            {
                ProgramValues.Rows.Add(block.TargetTemperature, block.TemperingDuration, ((int)block.PowerDrain).ToString());
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
            plan.Name = NameBox.Text.Trim();
            plan.Blocks = blocks.ToArray();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
