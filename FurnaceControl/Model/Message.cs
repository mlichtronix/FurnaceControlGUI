namespace FurnaceControl
{
    using System;
    using System.Text.RegularExpressions;

    public class Message
    {
        public static readonly string DataSeparator = ":";          // Separator between MessageTypeCode and Message content
        public static readonly string ThreeNumbers = "\\d{3}";      // Message TypeCode
        public static readonly string DataString = ".*$";           // Message content (all until end of line)

        /// <summary>
        /// Message Type
        /// </summary>
        public MessageType Type { get; private set; } = MessageType.NoOp;

        /// <summary>
        /// Message Data / Content
        /// </summary>
        public string Data { get; set; } = "";

        /// <summary>
        /// Time when message arrived
        /// </summary>
        public DateTime Received { get; set; }

        public Message(MessageType type, string data = "")
        {
            Type = type;
            Data = data;
        }

        public override string ToString()
        {
            return $"{Type}{DataSeparator}{Data}";
        }

        public string ToFurnaceString()
        {
            return $"{(int)Type}{DataSeparator}{Data}";
        }

        /// <summary>
        /// Creates instance of <see cref="Message"/> from string and date
        /// </summary>
        /// <param name="data">Received stream from serial port</param>
        public static Message FromStringAndDate(string data, DateTime received)
        {
            MessageType type = MessageType.NoOp;
            string msgData = string.Empty;

            Regex messageRgx = new Regex($"{ThreeNumbers}{DataSeparator}{DataString}");
            if (messageRgx.Match(data).Success)
            {                
                int typeValue = int.Parse(data.Substring(0, 3).Trim());
                type = (Enum.IsDefined(typeof(MessageType), typeValue)) ? (MessageType)typeValue : MessageType.Invalid;
                if (data.Length > 4) { msgData = data.Substring(4); }
            }
            else
            {
                type = MessageType.Error;
                msgData = data;
            }

            return new Message(type, msgData)
            {
                Received = received,
            };
        }
    }
}
