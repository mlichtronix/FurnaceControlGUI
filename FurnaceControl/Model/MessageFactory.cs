using System;

namespace FurnaceControl
{
    public static class MessageFactory
    {
        public static Message NoOp
        {
            get
            {
                return new Message(MessageType.NoOp);
            }
        }

        public static Message GetTemperature
        {
            get
            {
                return new Message(MessageType.GetCurTemp);
            }
        }

        public static Message HandShake
        {
            get
            {
                return new Message(MessageType.HandShake, Environment.MachineName);
            }
        }

        public static Message PcStatus
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
    }
}
