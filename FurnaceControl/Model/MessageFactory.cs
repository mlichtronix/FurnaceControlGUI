using System;

namespace FurnaceControl
{
    public static class MessageFactory
    {
        /// <summary>
        /// Get Current Temperature in Furnace
        /// </summary>
        public static Message GetTemperature
        {
            get
            {
                return new Message(MessageType.GetCurTemp);
            }
        }

        /// <summary>
        /// Get Device Unique ID
        /// </summary>
        public static Message HandShake
        {
            get
            {
                return new Message(MessageType.HandShake, Environment.MachineName);
            }
        }

        /// <summary>
        /// Get Program Counter Status
        /// </summary>
        public static Message GetPcStatus
        {
            get
            {
                return new Message(MessageType.GetPcStatus);
            }
        }

        public static Message Halt
        {
            get
            {
                return new Message(MessageType.Halt);
            }
        }

        /// <summary>
        /// Get Current set Program from Furnace
        /// </summary>
        public static Message GetCurProgram
        {
            get
            {
                return new Message(MessageType.GetCurProg);
            }
        }
    }
}
