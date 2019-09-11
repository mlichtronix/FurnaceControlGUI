namespace FurnaceControl
{
    public static class Responses
    {
        /// <summary>
        /// Device ID "CEP-0.5-1150" (according to furnace documentation)
        /// </summary>
        public static readonly string DeviceID = "CEP-0.5-1150";

        /// <summary>
        /// Halted state (Furnace is cooling down)
        /// </summary>
        public static readonly string Halted = "HALTED";

        /// <summary>
        /// Running state (Furnace is performing selected firing program)
        /// </summary>
        public static readonly string Running = "RUNNING";
    }
}
