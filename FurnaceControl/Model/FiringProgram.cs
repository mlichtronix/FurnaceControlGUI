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

        public string ToFurnaceString()
        {
            //Example: "Custom program|400*30*10;960*30*30;1200*60*30"
            return $"{Name}|{string.Join(";", Blocks.Select(x => x.ToFurnaceString()))}";
        }

        internal static FiringProgram FromFurnaceString(string data)
        {
            var program = new FiringProgram();
            var nameBlocks = data.Split('|');            
            var triplets = nameBlocks[1].Split(';');
            List<ProgramBlock> blocks = new List<ProgramBlock>();
            foreach (var triplet in triplets)
            {
                string[] tdp = triplet.Split('*');
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
            program.Name = nameBlocks[0];
            program.Blocks = blocks.ToArray();
            return program;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}