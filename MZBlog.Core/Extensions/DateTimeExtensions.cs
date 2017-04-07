using System;

namespace MZBlog.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime CloneToUtc(this DateTime dt)//修复ModelBind时将期望的UTC时间绑定为Local
        {
            if (dt.Kind == DateTimeKind.Utc)
                return dt;
            var utc = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, DateTimeKind.Utc);
            return utc;
        }

        public static DateTime ToChineseTime(this DateTime dt)
        {
            //var cnOffset = TimeZoneInfo.FindSystemTimeZoneById("Asia/Shanghai").BaseUtcOffset;
            //var cnUTC = dt.Add(cnOffset);
            var cnUtc = dt.AddHours(8);
            var cnDt = new DateTime(cnUtc.Year, cnUtc.Month, cnUtc.Day, cnUtc.Hour, cnUtc.Minute, cnUtc.Second, cnUtc.Millisecond, DateTimeKind.Unspecified);
            return cnDt;
        }
    }
}