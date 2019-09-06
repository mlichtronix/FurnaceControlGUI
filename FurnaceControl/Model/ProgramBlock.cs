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

        public override string ToString()
        {
            return $"[{TargetTemperature}°C,{TemperingDuration}min,{(int)PowerDrain}kW]";
        }
        public string ToFurnaceString()
        {
            return $"{TargetTemperature}*{TemperingDuration}*{(int)PowerDrain}";
        }
    }
}
