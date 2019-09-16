namespace FurnaceControl
{
    using System;
    using System.IO.Ports;
    using System.ComponentModel;

    public partial class Furnace : INotifyPropertyChanged
    {
        private readonly Logger L;
        private SerialPort comport;

        // Internal fields
        private bool halted = true;
        private int temperature = -1;
        private int programCounter = -1;        
        private FiringPlan program = null;
        private Wattage heating = Wattage.Power0kW;
        private SerialStatus status = SerialStatus.Disconnected;
        private DateTime startTime = DateTime.Now;

        // Constructors
        public Furnace(Logger logger) { L = logger; }

        // Public Porperties
        public event PropertyChangedEventHandler PropertyChanged;

        public Wattage Heating
        {
            get { return heating; }
            set
            {
                if (heating == value) { return; }
                heating = value;
                OnPropertyChanged("Heating");
            }
        }

        public DateTime StartTime
        {
            get { return startTime; }
            set
            {
                if (startTime == value) { return; }
                startTime = value;
                OnPropertyChanged("StartTime");
            }
        }

        public bool Halted
        {
            get { return halted; }
            set
            {
                if (halted == value) { return; }
                halted = value;
                OnPropertyChanged("Halted");
            }
        }

        public SerialStatus Status
        {
            get { return status; }
            private set
            {
                if (status == value) { return; }
                status = value;                
                OnPropertyChanged("Status");
            }
        }

        private void SyncCurrentStatus()
        {
            SendMessage(MessageFactory.HandShake);
            SendMessage(MessageFactory.GetTemperature);
            SendMessage(MessageFactory.GetCurProgram);
            SendMessage(MessageFactory.GetPcStatus);
        }

        public FiringPlan Program
        {
            get { return program; }
            set
            {
                program = value;
                OnPropertyChanged("Program");
            }
        }

        public int ProgramCounter
        {
            get { return programCounter; }
            set
            {
                if (programCounter == value) { return; }
                programCounter = value;
                
                OnPropertyChanged("ProgramCounter");
            }
        }

        public int Temperature
        {
            get { return temperature; }
            set
            {
                temperature = value;
                OnPropertyChanged("Temperature");
            }
        }

        // Private Methods
        private void SendMessage(Message msg)
        {
            if (comport != null && comport.IsOpen)
            {
                try
                {
                    comport.WriteLine(msg.ToFurnaceString());
                    Status = SerialStatus.Connected;
                    return;
                }
                catch (TimeoutException)
                {
                    Status = SerialStatus.NotResponding;                    
                    return;
                }
                catch (Exception ex)
                {
                    L.Add($"SendMessage: [{ex.Message}]");
                }                
            }
            Status = SerialStatus.Disconnected;
        }

        private void ProcessReceivedMessage(Message msg)
        {            
            switch (msg.Type)
            {
                case MessageType.HandShake:         // 200
                    if (msg.Data == Responses.DeviceID)
                    {
                        Status = SerialStatus.Connected;
                    }
                    break;
                case MessageType.SetTime:           // 300
                    L.Add($"Furnace Time: [{Extensions.DateFromFurnaceString(msg.Data)}]");
                    break;
                case MessageType.GetCurTemp:        // 400
                    Temperature = int.Parse(msg.Data);
                    break;
                case MessageType.GetPcStatus:       // 500
                    ProgramCounter = int.Parse(msg.Data);
                    break;
                case MessageType.GetCurProg:        // 600
                    ParseProgram(msg.Data);
                    break;
                case MessageType.SetProgram:        // 650
                    SendMessage(MessageFactory.GetCurProgram);
                    break;
                case MessageType.Start:             // 700
                    Halted = false;
                    StartTime = Extensions.DateFromFurnaceString(msg.Data);
                    break;
                case MessageType.CloseSmokeAlert:   // 800
                    OnPropertyChanged(msg.Type.ToString());
                    break;
                case MessageType.Error:             // 990
                    break;
                case MessageType.Heating:           // 950
                    Heating = (Wattage)int.Parse(msg.Data);
                    break;
                case MessageType.Halt:              // 999
                    Halted = true;
                    L.Add("Furnace is Halted");
                    break;                
                default:
                    L.Add($"Invalid Message Type: [{msg.Data}]");
                    break;
            }
        }

        private void ParseProgram(string data)
        {
            try
            {
                Program = FiringPlan.FromFurnaceString(data);
            }
            catch (Exception ex)
            {
                L.Add(ex.Message);
            }
        }

        private void HandleReceivedMessage(object sender, SerialDataReceivedEventArgs e)
        {
            var time = DateTime.Now;
            try
            {
                string content = ((SerialPort)sender).ReadLine();
                ProcessReceivedMessage(Message.FromStringAndDate(content, time));
            }
            catch (TimeoutException ex)
            {
                L.Add(ex.Message);
            }
        }

        // Public Methods
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ConnectDevice(string portName)
        {
            if (string.IsNullOrEmpty(portName)) { return; }

            comport = new SerialPort(portName, 9600)
            {
                Encoding = System.Text.Encoding.ASCII,
                ReadTimeout = 3000
            };
            comport.DataReceived += new SerialDataReceivedEventHandler(HandleReceivedMessage);

            try
            {
                if (!comport.IsOpen)
                {
                    comport.Open();
                    comport.ReadExisting(); // Clear previous communication
                }                
                SyncCurrentStatus();
            }
            catch
            {
                if (comport.IsOpen) { comport.Close(); }
                Status = SerialStatus.Disconnected;
            }
        }

        public void UpdateStatus()
        {
            SendMessage(MessageFactory.GetTemperature);
            SendMessage(MessageFactory.GetPcStatus);
        }

        public void DisconnectDevice()
        {
            if (comport.IsOpen) { comport.Close(); }
            Status = SerialStatus.Disconnected;
        }

        public void Halt()
        {
            SendMessage(MessageFactory.Halt);            
        }

        internal void SetTime(DateTime t)
        {
            SendMessage(new Message(MessageType.SetTime, t.ToFurnaceString()));
        }

        public void SetCustomProgram(FiringPlan p)
        {
            SendMessage(new Message(MessageType.SetProgram, p.ToFurnaceString()));
        }

        public void Start(DateTime t)
        {        
            SendMessage(new Message(MessageType.Start, t.ToFurnaceString()));
        }
    }
}