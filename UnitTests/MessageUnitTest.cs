namespace UnitTests
{
    using FurnaceControl;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class MessageTest
    {
        [TestMethod]
        public void MessageConstructorTest()
        {
            string data = "Test";
            Message message = Message.FromStringAndDate($"001{Message.DataSeparator}{data}", DateTime.Now);
            Assert.AreEqual(MessageType.NoOp, message.Type);
            Assert.AreEqual(data, message.Data);
        }

        [TestMethod]
        public void EnumIsDefinedTest()
        {
            int validEnumValue = 200;
            bool result1 = Enum.IsDefined(typeof(MessageType), validEnumValue);
            Assert.AreEqual(true, result1);

            int invalidEnumValue = 201;
            bool result2 = Enum.IsDefined(typeof(MessageType), invalidEnumValue);
            Assert.AreEqual(false, result2);
        }
    }
}
