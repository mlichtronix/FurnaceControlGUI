﻿namespace FurnaceControl
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
        private List<FiringProgram> P = new List<FiringProgram> ();
        
        public MainForm()
        {
            InitializeComponent();
            D.Start = DateTime.Now.AddMinutes(-11);
            D.Heatings.AddRange(new Heating[]
            {
                new Heating(Wattage.Power0kW, DateTime.Now.AddMinutes(-9)),
                new Heating(Wattage.Power10kW, DateTime.Now.AddMinutes(-8)),
                new Heating(Wattage.Power10kW, DateTime.Now.AddMinutes(-7)),
                new Heating(Wattage.Power30kW, DateTime.Now.AddMinutes(-6)),
                new Heating(Wattage.Power30kW, DateTime.Now.AddMinutes(-5)),
                new Heating(Wattage.Power0kW, DateTime.Now.AddMinutes(-4)),
                new Heating(Wattage.Power10kW, DateTime.Now.AddMinutes(-3)),
                new Heating(Wattage.Power10kW, DateTime.Now.AddMinutes(-2)),
                new Heating(Wattage.Power30kW, DateTime.Now.AddMinutes(-1)),                
            });
            D.Measurements.AddRange(new Measurement[] 
            {
                new Measurement( 25, DateTime.Now.AddMinutes(-9)),
                new Measurement( 50, DateTime.Now.AddMinutes(-8)),
                new Measurement(150, DateTime.Now.AddMinutes(-7)),
                new Measurement(300, DateTime.Now.AddMinutes(-6)),
                new Measurement(600, DateTime.Now.AddMinutes(-5)),
                new Measurement(900, DateTime.Now.AddMinutes(-4)),
                new Measurement(960, DateTime.Now.AddMinutes(-3)),
                new Measurement(800, DateTime.Now.AddMinutes(-2)),
                new Measurement(600, DateTime.Now.AddMinutes(-1)),
                new Measurement(300, DateTime.Now.AddMinutes( 0)),
            });

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
                    case "Program":
                        D.Program = F.Program.Blocks;
                        L.Add($"Program set to: [{F.Program.Name}]");
                        break;
                    default:
                        L.Add($"Not Implemented Event: [{e.PropertyName}]");
                        break;
                }
            });
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

        private void SetStartHaltButton(bool halted)
        {
            if (halted)
            {
                StartHaltButton.Text = "START";
            }
            else
            {
                StartHaltButton.Text = "HALT";
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
                if (ProgramSelector.SelectedIndex != -1)
                {
                    F.SetCustomProgram(P[ProgramSelector.SelectedIndex]);
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
            Properties.Settings.Default.Save();
        }
    }
}
