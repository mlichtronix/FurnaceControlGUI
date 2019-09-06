namespace FurnaceControl
{
    using System;
    using System.Drawing;
    using System.IO.Ports;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Drawing.Drawing2D;
    using System.Collections.Generic;
    using System.Linq;

    public partial class MainForm : Form
    {
        private Furnace F;
        private Logger L = new Logger();
        private Display D = new Display();
        private List<FiringProgram> P = new List<FiringProgram>();
        
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
            if(string.IsNullOrEmpty(Properties.Settings.Default.Programs)) { return; }
            try
            {
                string[] programs = Properties.Settings.Default.Programs.Split(Environment.NewLine.ToArray());
                P = programs.Select(x => FiringProgram.FromFurnaceString(x.Trim())).ToList();
            }
            catch
            {
                L.Add("Wrong data in Settings.Default.Programs!");
            }
        }

        private void UptateValues(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CloseSmokeAlert":
                    L.Add("Please close smokestack!");
                    break;
                case "Status":
                    UpdateStatus();
                    break;
                case "Temperature":
                    var m = new Measurement(F.Temperature, DateTime.Now);
                    D.Measurements.Add(m);
                    L.Add($"Temperature update: {m.Temperature} °C");
                    break;
                case "ProgramCounter":
                    D.ProgramCounter = F.ProgramCounter;
                    L.Add($"Current Block: {F.ProgramCounter}");
                    break;
                case "Heating":
                    D.Heatings.Add(new Heating(F.Heating, DateTime.Now));
                    L.Add($"Heating: {F.Heating}");
                    break;
                default:
                    L.Add($"NotImplemented Event: [{e.PropertyName}]");
                    break;
            }
        }

        private void InvokeUI(Action a)
        {
            if(IsHandleCreated) { BeginInvoke(new MethodInvoker(a)); }
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
                        FurnaceGroup.Enabled = false;
                        ConnectButton.Text = "Connect";
                        L.Add("Device is disconnected.");
                        GraphRefresh.Stop();
                        DeviceBox.BackColor = Color.White;
                        break;

                    case SerialStatus.Connected:
                        FurnaceGroup.Enabled = true;                        
                        ConnectButton.Text = "Disconnect";
                        L.Add("Device is connected.");
                        D.Start = DateTime.Now;
                        RefreshGraph(this, null);
                        GraphRefresh.Start();
                        DeviceBox.BackColor = Color.Lime;
                        break;

                    case SerialStatus.NotResponding:
                        FurnaceGroup.Enabled = false;
                        L.Add("Device is not responding!");
                        DeviceBox.BackColor = Color.Orange;
                        break;

                    case SerialStatus.WrongProtocol:
                        FurnaceGroup.Enabled = false;
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

        private void RefreshGraph(object sender, EventArgs e)
        {
            F.UpdateStatus();
            GraphBox.Invalidate();
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
                F.SetProgrm(P[ProgramSelector.SelectedIndex]);
                F.Start();
            }
            else
            {
                F.Halt();
            }
        }

        private void AddNewProgram(object sender, EventArgs e)
        {
            var ExistingNames = P?.Select(x => x.Name);
            ProgramDesigner designer = new ProgramDesigner(new FiringProgram(), ExistingNames);
            
            if (designer.ShowDialog() == DialogResult.OK)
            {
                P.Add(designer.program);
            }
            UpdateAvailablePrograms();
        }

        private void UpdateAvailablePrograms()
        {
            ProgramSelector.Items.Clear();
            ProgramSelector.Items.AddRange(P.ToArray());
            StartHaltButton.Enabled = ProgramSelector.Items != null;
        }

        private void SaveBeforeClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Programs = string.Join(Environment.NewLine, P.Select(x => x.ToFurnaceString()));
        }
    }
}
