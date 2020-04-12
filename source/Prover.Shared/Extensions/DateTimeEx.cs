using System;

namespace Prover.Shared.Extensions
{
    public static class DateTimeEx
    {
        public static bool BetweenThenAndNow(this DateTime dateTime, DateTime fromDate)
        {
            return dateTime.Between(fromDate, DateTime.Now);
        }

        public static bool IsLessThanTimeAgo(this DateTime dateTime, TimeSpan timeAgo)
        {
            return dateTime.Between(DateTime.Now.Subtract(timeAgo), DateTime.Now);
        }
    }
}