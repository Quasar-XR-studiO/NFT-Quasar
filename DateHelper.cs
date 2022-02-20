using System;

namespace ZBoom.Common.SpatialMap
{
    public class DateHelper
    {
        public static string GetCurrentDate()
        {
            return DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        }
    }
}