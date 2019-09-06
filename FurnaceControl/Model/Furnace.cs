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
        private FiringProgram program = null;
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

        public FiringProgram Program
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
                    comport.WriteLine(msg.ToString());
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
                    L.Add(ex.Message);
                }                
            }
            Status = SerialStatus.Disconnected;
        }

        private void ProcessReceivedMessage(Message msg)
        {            
            switch (msg.Type)
            {
                case MessageType.CloseSmokeAlert:
                    OnPropertyChanged(msg.Type.ToString());
                    break;
                case MessageType.Error:
                    break;
                case MessageType.Heating:
                    Heating = (Wattage)int.Parse(msg.Data);
                    break;
                case MessageType.GetCurProg:
                    ParseProgram(msg.Data);
                    break;
                case MessageType.GetCurTemp:
                    Temperature = int.Parse(msg.Data);
                    break;
                case MessageType.GetPcStatus:
                    ProgramCounter = int.Parse(msg.Data);                    
                    break;
                case MessageType.Halt:
                    Halted = msg.Data == Responses.Halted;
                    break;
                case MessageType.HandShake:
                    Status = SerialStatus.Connected;
                    break;
                case MessageType.SetCustomProgram:
                    Program = FiringProgram.FromString(msg.Data);
                    break;
                case MessageType.SetPredProgrm:
                    break;
                default:
                    L.Add($"Invalid message received: [{msg.Data}]");
                    break;
            }
        }

        private void ParseProgram(string data)
        {
            try
            {
                Program = FiringProgram.FromString(data);
            }
            catch (Exception ex)
            {
                L.Add(ex.Message);
            }
        }

        private void HandleReceivedMessage(object sender, SerialDataReceivedEventArgs e)
        {
            var time = DateTime.Now;
            string content = ((SerialPort)sender).ReadLine();
            ProcessReceivedMessage(Message.FromStringAndDate(content, time));
        }
               
        // Public Methods
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ConnectDevice(string portName)
        {
            comport = new SerialPort(portName, 9600);
            comport.Encoding = System.Text.Encoding.ASCII;
            comport.ReadTimeout = 3000;
            comport.DataReceived += new SerialDataReceivedEventHandler(HandleReceivedMessage);

            try
            {
                if (!comport.IsOpen)
                {
                    comport.Open();
                    comport.ReadExisting(); // Clear previous communication
                }
                SendMessage(MessageFactory.HandShake);
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
            SendMessage(MessageFactory.PcStatus);
        }

        public void DisconnectDevice()
        {
            if (comport.IsOpen) { comport.Close(); }
            Status = SerialStatus.Disconnected;
        }

        public void Halt()
        {
            if (!Halted)
            {
                SendMessage(MessageFactory.Halt);
            }
        }

        public void Start()
        {
            if (Halted)
            {
                SendMessage(new Message(MessageType.Start, StartTime.ToFurnaceString()));
            }
        }

        public void SetProgrm(FiringProgram program)
        {
            SendMessage(new Message(MessageType.SetCustomProgram, program.ToString()));
        }
    }
}