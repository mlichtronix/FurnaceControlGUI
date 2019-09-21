namespace FurnaceControl
{
    using System;
    using System.Text.RegularExpressions;

    public class Message
    {
        /// <summary>
        /// Separator between MessageTypeCode and Message content: "\\:"
        /// </summary>
        public static readonly string DataSeparator = ":";

        /// <summary>
        /// Message TypeCode: "(200|300|400|500|600|650|700|800|900|990|995|999)"
        /// </summary>
        public static readonly string ThreeNumbers = "(200|300|400|500|600|650|700|800|900|990|995|999)";

        /// <summary>
        /// Message content (all until end of line): ".*$"
        /// </summary>
        public static readonly string DataString = ".*$";

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

            Regex messageRgx = new Regex($"{ThreeNumbers}\\{DataSeparator}{DataString}");
            Match msgRegex = messageRgx.Match(data);
            if (msgRegex.Success)
            {
                string rgxData = msgRegex.Value.Trim();
                int typeValue = int.Parse(rgxData.Substring(0, 3));
                type = (Enum.IsDefined(typeof(MessageType), typeValue)) ? (MessageType)typeValue : MessageType.Invalid;
                if (rgxData.Length > 4) { msgData = rgxData.Substring(4).Trim(); }
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
