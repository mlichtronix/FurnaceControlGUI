namespace FurnaceControl
{
    using System;

    public static class Extensions
    {
        public static readonly string DateTimeFormat = "yyyy-MM-dd-HH-mm-ss";

        /// <summary>
        /// Converts <see cref="DateTime" /> to Furnace DateTime recognizable String [yyyy-MM-dd-HH-mm-ss]
        /// </summary>
        /// <param name="t">DateTime to convert</param>
        /// <returns>Furnace DateTime formated string</returns>
        public static string ToFurnaceString(this DateTime t)
        {
            return t.ToString(DateTimeFormat);
        }
    }
}
