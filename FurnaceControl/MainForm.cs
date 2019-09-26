namespace FurnaceControl
{
    using System;
    using System.Linq;
    using System.Drawing;
    using System.IO.Ports;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Drawing.Drawing2D;
    using System.Collections.Generic;

    public partial class MainForm : Form
    {
        private Furnace F;
        private Logger L = new Logger();
        private Display D = new Display();
        private List<FiringPlan> P = new List<FiringPlan>();

        public MainForm()
        {
            InitializeComponent();
            F = new Furnace(L);
            F.PropertyChanged += new PropertyChangedEventHandler(UptateValues);
            L.PropertyChanged += UpdateLogBox;
            L.Add("Logging started");
            RefreshDevices(this, null);
            LoadProgramsFromSettings();
        }

        private void LoadProgramsFromSettings()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.Plans))
            {
                return;
            }
            try
            {
                string[] programs = Properties.Settings.Default.Plans.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries);
                P = programs.Select(x => FiringPlan.FromFurnaceString(x.Trim())).ToList();
                UpdateAvailablePrograms();
            }
            catch
            {
                L.Add("Wrong data in Programs!");
            }
        }

        private void UptateValues(object sender, PropertyChangedEventArgs e)
        {
            InvokeUI(() =>
            {
                switch (e.PropertyName)
                {
                    case "Halted":
                        SetStartHaltButton(F.Halted);
                        break;
                    case "Start":
                        SetStartHaltButton(F.Halted);
                        D.Start = DateTime.Now;
                        break;
                    case "StartTime":
                        L.Add($"Start Time: [{F.StartTime}]");
                        break;
                    case "CloseSmokeAlert":
                        string msg = "Please close smokestack!";
                        L.Add(msg);
                        MessageBox.Show(msg, "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    case "Status":
                        UpdateStatus();
                        break;
                    case "Temperature":
                        D.Temperature = F.Temperature;
                        if (!F.Halted)
                        {
                            D.Measurements.Add(new Measurement(F.Temperature, DateTime.Now));
                        }
                        break;
                    case "ProgramCounter":
                        D.ProgramCounter = F.ProgramCounter;
                        L.Add($"Current Block: {F.ProgramCounter}");
                        break;
                    case "Heating":
                        D.Heatings.Add(new Heating(F.Heating, DateTime.Now));
                        L.Add($"Heating: {F.Heating}");
                        break;
                    case "Program":
                        D.Plan = F.Program;
                        L.Add($"Program set to: [{F.Program.Name}]");
                        break;
                    default:
                        L.Add($"Not Implemented Event: [{e.PropertyName}]");
                        break;
                }
                GraphBox.Invalidate();
            });
        }

        private void InvokeUI(Action a)
        {
            if (IsHandleCreated) { BeginInvoke(new MethodInvoker(a)); }
        }

        private void UpdateLogBox(object sender, PropertyChangedEventArgs e)
        {
            InvokeUI(() =>
            {
                LogBox.Text = L.Log;
                LogBox.SelectionStart = LogBox.Text.Length;
                LogBox.ScrollToCaret();
            });
        }

        private void Exit(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            Application.Exit();
        }

        private void GetDevices()
        {
            DeviceBox.DataSource = null;
            DeviceBox.DataSource = SerialPort.GetPortNames();
        }

        private void SetStartHaltButton(bool halted)
        {
            if (halted)
            {
                StartHaltButton.Text = "START";
                StartHaltButton.BackColor = Color.Lime;
            }
            else
            {
                StartHaltButton.Text = "HALT";
                StartHaltButton.BackColor = Color.Red;
            }
        }

        private void RefreshDevices(object sender, EventArgs e)
        {
            L.Add("Searching for connected devices...");
            GetDevices();
            L.Add("Searching finished.");
        }

        private void ConnectDevice(object sender, EventArgs e)
        {
            switch (F.Status)
            {
                case SerialStatus.Connected:
                    F.DisconnectDevice();
                    break;
                case SerialStatus.Disconnected:
                    F.ConnectDevice((string)DeviceBox.SelectedItem);
                    break;
            }
        }

        public void UpdateStatus()
        {
            InvokeUI(() =>
            {
                switch (F.Status)
                {
                    case SerialStatus.Disconnected:
                        StartHaltButton.Enabled = false;
                        ConnectButton.Text = "Connect";
                        L.Add("Device is disconnected.");
                        GraphRefresh.Stop();
                        DeviceBox.BackColor = Color.White;
                        break;

                    case SerialStatus.Connected:
                        StartHaltButton.Enabled = true;
                        ConnectButton.Text = "Disconnect";
                        L.Add("Device is connected.");
                        D.Start = DateTime.Now;
                        UpdateStatus(this, null);
                        GraphRefresh.Start();
                        DeviceBox.BackColor = Color.Lime;
                        break;

                    case SerialStatus.NotResponding:
                        StartHaltButton.Enabled = false;
                        L.Add("Device is not responding!");
                        DeviceBox.BackColor = Color.Orange;
                        break;

                    case SerialStatus.WrongProtocol:
                        StartHaltButton.Enabled = false;
                        L.Add("Device is not compatible!");
                        F.DisconnectDevice();
                        DeviceBox.BackColor = Color.Red;
                        break;

                    default:
                        L.Add("Invalid state!");
                        break;
                }
            });
        }

        private void UpdateStatus(object sender, EventArgs e)
        {
            F.UpdateStatus();
        }

        private void UpdateImage(object sender, PaintEventArgs e)
        {
            Graphics img = e.Graphics;
            img.SmoothingMode = SmoothingMode.AntiAlias;
            img.CompositingQuality = CompositingQuality.HighQuality;
            img.InterpolationMode = InterpolationMode.HighQualityBicubic;
            D.Draw(img);
        }

        private void UpdateImageNow(object sender, EventArgs e) { GraphBox.Invalidate(); }

        private void StartHaltNow(object sender, EventArgs e)
        {
            if (F.Halted)
            {
                if (PlanSelector.SelectedIndex != -1)
                {
                    F.SetCustomProgram(P[PlanSelector.SelectedIndex]);
                    F.SetTime(DateTime.Now);
                    var v = SchedulePicker.Value;
                    F.Start(SchedulePicker.Value);
                }
                else
                {
                    MessageBox.Show("Select program first!");
                }
            }
            else
            {
                F.Halt();
            }
        }

        private void AddNewProgram(object sender, EventArgs e)
        {
            PlanDesigner designer = new PlanDesigner(new FiringPlan(), GetExistingPlanNames());
            if (designer.ShowDialog() == DialogResult.OK)
            {
                P.Add(designer.plan);
                UpdateAvailablePrograms();
                SaveSettings();
            }
        }

        private void UpdateAvailablePrograms()
        {
            // Update GUI
            int index = PlanSelector.SelectedIndex;
            PlanSelector.Items.Clear();
            PlanSelector.Items.AddRange(P.ToArray());
            PlanSelector.SelectedIndex = index < PlanSelector.Items.Count ? index : -1;
            StartHaltButton.Enabled = PlanSelector.Items != null && F.Status == SerialStatus.Connected;
        }

        private void SaveSettings()
        {
            var plans = string.Join(Environment.NewLine, P.Select(x => x.ToFurnaceString()));
            Properties.Settings.Default.Plans = plans;
            Properties.Settings.Default.Save();
        }

        private void EditSelectedPlan(object sender, EventArgs e)
        {
            var index = PlanSelector.SelectedIndex;
            if (index == -1) { return; }

            var designer = new PlanDesigner(P[index], GetExistingPlanNames());
            if (designer.ShowDialog() == DialogResult.OK)
            {
                P[index] = designer.plan;
                UpdateAvailablePrograms();
                SaveSettings();
            }
        }

        private void RemoveSelectedPlan(object sender, EventArgs e)
        {
            int index = PlanSelector.SelectedIndex;
            if (index == -1) { return; }

            if (MessageBox.Show($"Do you really want to remove [{P[index].Name}] plan?", "Confirm", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                P.RemoveAt(index);
                UpdateAvailablePrograms();
                SaveSettings();
            }
        }

        private string[] GetExistingPlanNames()
        {
            return P?.Select(x => x.Name).ToArray();
        }
    }
}
