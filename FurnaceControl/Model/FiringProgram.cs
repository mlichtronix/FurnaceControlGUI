using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace FurnaceControl
{
    [Serializable]
    public class FiringProgram
    {
        /// <summary>
        /// Program Name
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; } = "Custom Program";

        /// <summary>
        /// Schedule for start at certain date/time
        /// </summary>
        [XmlIgnore]
        public DateTime Schedule { get; set; }

        /// <summary>
        /// Temperatures and tempering times (Burning technologic protocol)
        /// </summary>
        [XmlArray]
        public ProgramBlock[] Blocks { get; set; } = new ProgramBlock[] { };

        /// <summary>
        /// Creates instance of <see cref="FiringProgram"/>
        /// </summary>
        public FiringProgram() { }


        internal static FiringProgram FromString(string data)
        {
            var program = new FiringProgram();
            string[] pairs = data.Split('|');
            List<ProgramBlock> blocks = new List<ProgramBlock>();
            foreach (var pair in pairs)
            {
                string[] tdp = pair.Split('*');
                if (tdp.Length == 3)
                {
                    var block = new ProgramBlock()
                    {
                        TargetTemperature = int.Parse(tdp[0]),
                        TemperingDuration = int.Parse(tdp[1]),
                        PowerDrain = (Wattage)int.Parse(tdp[2])
                    };
                    blocks.Add(block);
                }
                else
                {
                    throw new Exception($"Program is in wrong format! [{data}]");
                }
            }
            program.Blocks = blocks.ToArray();
            return program;
        }

        public override string ToString()
        {
            return Name;
        }

        public string ToFurnaceString()
        {
            //Example: "Custom program|2019-12-31-23-59-59|400*30*10;960*30*30;1200*60*30"
            return $"{Name}|{Schedule.ToFurnaceString()}|{string.Join(";", Blocks.Select(x => x.ToFurnaceString()))}";
        }

        public static FiringProgram FromFurnaceString(string data)
        {
            var parts = data.Split('|');
            string name = parts[0];
            DateTime date = DateTime.ParseExact(parts[1], Extensions.DateTimeFormat, CultureInfo.InvariantCulture);
            string[] blocksStr = parts[2].Split(';');
            List<ProgramBlock> blocks = new List<ProgramBlock>();
            foreach (var block in blocksStr)
            {
                string[] values = block.Split('*');
                int c = int.Parse(values[0]);
                int d = int.Parse(values[1]);
                int w = int.Parse(values[2]);
                blocks.Add(new ProgramBlock() { TargetTemperature = c, TemperingDuration = d, PowerDrain = (Wattage)w });
            }
            return new FiringProgram() { Name = name, Schedule = date, Blocks = blocks.ToArray() };
        }
    }
}
