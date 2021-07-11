using System;

namespace Parto.Helper.PersionDateTime
{
    public static class PersianDateTimeExtension
    {
        public static PersianDateTime ToPersianDateTime(this DateTime dateTime) => new(dateTime);
    }
}