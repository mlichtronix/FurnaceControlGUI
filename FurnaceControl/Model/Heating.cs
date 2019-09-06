using System;

namespace FurnaceControl
{
    public class Heating
    {
        /// <summary>
        /// Heating Start Time
        /// </summary>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// Heating power configuration
        /// </summary>
        public Wattage Power { get; private set; } = Wattage.Power0kW;

        /// <summary>
        /// Creates instance of <see cref="Heating"/>
        /// </summary>
        /// <param name="p">Heating Power Wattage</param>
        public Heating(Wattage p)
        {
            StartTime = DateTime.Now;
            Power = p;
        }

        /// <summary>
        /// Creates instance of <see cref="Heating"/>
        /// </summary>
        /// <param name="p">Heating Power Wattage</param>
        /// <param name="t">Start time</param>
        public Heating(Wattage p, DateTime t)
        {
            StartTime = t;
            Power = p;
        }
    }
}
