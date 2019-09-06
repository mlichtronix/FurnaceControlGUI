namespace FurnaceControl
{
    using System;
    using System.Text;
    using System.ComponentModel;

    public class Logger : INotifyPropertyChanged
    {
        private StringBuilder builder = new StringBuilder();
        public event PropertyChangedEventHandler PropertyChanged;
                
        /// <summary>
        /// Add new Log message
        /// </summary>
        /// <param name="line"></param>
        public void Add(string line)
        {
            builder.Append($"{Environment.NewLine}[{DateTime.Now}] {line.Trim()}");
            OnPropertyChanged("Log");
        }

        /// <summary>
        /// Remove all Logging Content
        /// </summary>
        public void Clear()
        {
            builder.Clear();
            OnPropertyChanged("Log");
        }

        /// <summary>
        /// Notify all subscribed clients
        /// </summary>
        /// <param name="info"></param>
        private void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        /// <summary>
        /// Compiled all lines to one long string 
        /// (referenced by Binding source string "Log")
        /// </summary>
        public string Log
        {
            get
            {
                return builder.ToString();
            }
        }
    }
}