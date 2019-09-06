using System;

namespace FurnaceControl
{
    public class Measurement
    {
        /// <summary>
        /// Time of measurement
        /// </summary>
        public DateTime Time { get; private set; }

        /// <summary>
        /// Measured Temperature in furnace
        /// </summary>
        public int Temperature { get; set; }

        /// <summary>
        /// Creates instance of <see cref="Measurement"/>
        /// </summary>
        /// <param name="temperature">Measured temperature in range [0-1300]°C</param>
        /// <param name="time">Time when measurement was made</param>
        public Measurement(int temperature, DateTime time)
        {
            Time = time;
            Temperature = temperature;
        }
    }
}
