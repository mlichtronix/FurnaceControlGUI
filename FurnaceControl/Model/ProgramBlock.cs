using System;
using System.ComponentModel;

namespace FurnaceControl
{
    public class ProgramBlock
    {
        /// <summary>
        /// Activation Time (historical record)
        /// </summary>
        [Browsable(false)]
        public DateTime Start { get; set; }

        /// <summary>
        /// Target Temperature in °C
        /// </summary>
        [DisplayName("Target Temperature")]
        public int TargetTemperature { get; set; }

        /// <summary>
        /// Tempering Duration in minutes
        /// </summary>
        [DisplayName("Tempering Duration in minutes")]
        public int TemperingDuration { get; set; }

        /// <summary>
        /// Power Drain Cnfiguration ( 10kW / 30kW )
        /// </summary>
        [DisplayName("Wattage [10kW/30kW]")]
        public Wattage PowerDrain { get; set; }

        /// <summary>
        /// Creates empty instance of <see cref="ProgramBlock"/>
        /// </summary>
        public ProgramBlock() { }

        /// <summary>
        /// Creates instance of <see cref="ProgramBlock"/>
        /// </summary>
        /// <param name="temperature"></param>
        /// <param name="duration"></param>
        /// <param name="drain"></param>
        public ProgramBlock(int temperature, int duration, Wattage drain)
        {
            TargetTemperature = temperature;
            TemperingDuration = duration;
            PowerDrain = drain;
        }

        /// <summary>
        /// Converts <see cref="ProgramBlock"/> to human readable string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[{TargetTemperature}°C, {TemperingDuration}min, {(int)PowerDrain}kW]";
        }

        /// <summary>
        /// Converts <see cref="ProgramBlock"/> to string that Furnace Controler understands
        /// </summary>
        /// <returns></returns>
        public string ToFurnaceString()
        {
            return $"{TargetTemperature}*{TemperingDuration}*{(int)PowerDrain}";
        }
    }
}
