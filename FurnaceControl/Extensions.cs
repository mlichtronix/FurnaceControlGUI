namespace FurnaceControl
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;

    public static class Extensions
    {
        private static readonly string DateTimeFormat = "yyyy-MM-dd-HH-mm-ss";
        private static readonly string PlanFormat = "(.*\\|)(\\d+\\*\\d+\\*\\d+;)*(\\d+\\*\\d+\\*\\d+)$";

        /// <summary>
        /// Converts <see cref="DateTime" /> to Furnace DateTime recognizable String [yyyy-MM-dd-HH-mm-ss]
        /// </summary>
        /// <param name="t">DateTime to convert</param>
        /// <returns>Furnace DateTime formated string</returns>
        public static string ToFurnaceString(this DateTime t)
        {
            return t.ToString(DateTimeFormat);
        }

        public static DateTime DateFromFurnaceString(string t)
        {
            return DateTime.ParseExact(t.Trim(), DateTimeFormat, CultureInfo.InvariantCulture);
        }
        
        public static bool IsPlanInCorrectFormat(string data)
        {
            return new Regex(PlanFormat).IsMatch(data);
        }
    }
}
