namespace FurnaceControl
{
    public class Responses
    {
        /// <summary>
        /// Device ID "CEP-0.5-1150" (according to furnace documentation)
        /// </summary>
        public static string DeviceID = "CEP-0.5-1150";

        /// <summary>
        /// Halted state (Furnace is cooling down)
        /// </summary>
        public static string Halted = "HALTED";

        /// <summary>
        /// Running state (Furnace is performing selected firing program)
        /// </summary>
        public static string Running = "RUNNING";
    }
}
