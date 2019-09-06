namespace FurnaceControl
{
    /// <summary>
    /// Power drain configuration
    /// </summary>
    public enum Wattage
    {
        /// <summary>
        /// Open line / Heating is disabled
        /// </summary>
        Power0kW = 0,
        /// <summary>
        /// Δ - Delta Configuration
        /// </summary>
        Power10kW = 10,
        /// <summary>
        /// Y - Star Configuration
        /// </summary>
        Power30kW = 30,
    }
}
