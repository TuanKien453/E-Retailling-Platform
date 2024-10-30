namespace E_Retalling_Portal.Util
{
    public class TimeUtil
    {
        public static bool IsCurrentTimeBetween(string startTime, string endTime)
        {
            DateTime startDateTime = DateTime.ParseExact(startTime, "yyyyMMddHHmmss", null);
            DateTime endDateTime = DateTime.ParseExact(endTime, "yyyyMMddHHmmss", null);

            DateTime currentTime = DateTime.Now;

            return currentTime >= startDateTime && currentTime <= endDateTime;
        }
    }
}
