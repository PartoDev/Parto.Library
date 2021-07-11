using System;
using System.Globalization;

namespace Parto.Helper.PersionDateTime
{
    public readonly struct PersianDateTime : IPersianDateTime
    {
        public PersianDateTime(int year,
            int month,
            int day,
            int hour = 0,
            int minute = 0,
            int second = 0,
            int millisecond = 0)
        {
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
            Second = second;
            Millisecond = millisecond;
        }

        public PersianDateTime(DateTime dateTime)
        {
            PersianCalendar persianCalendar = new();
            if (dateTime > persianCalendar.MinSupportedDateTime)
            {
                Year = persianCalendar.GetYear(dateTime);
                Month = persianCalendar.GetMonth(dateTime);
                Day = persianCalendar.GetDayOfMonth(dateTime);
                Hour = persianCalendar.GetHour(dateTime);
                Minute = persianCalendar.GetMinute(dateTime);
                Second = persianCalendar.GetSecond(dateTime);
                Millisecond = persianCalendar.GetMilliseconds(dateTime);
            }
            else
            {
                Year = 0;
                Month = 0;
                Day = 0;
                Hour = 0;
                Minute = 0;
                Second = 0;
                Millisecond = 0;
            }
        }

        public PersianDateTime(string? dateTimeString)
        {
            Year = 0;
            Month = 0;
            Day = 0;
            Hour = 0;
            Minute = 0;
            Second = 0;
            Millisecond = 0;
            if (dateTimeString is null)
                return;

            var persianDateTimeSplit = dateTimeString.Split(' ');
            var persianDate = persianDateTimeSplit[0];
            var persianDateSplit = persianDate.Split('/');
            if (persianDateSplit.Length >= 1 &&
                int.TryParse(persianDateSplit[0], out var year))
                Year = year;

            if (persianDateSplit.Length >= 2 &&
                int.TryParse(persianDateSplit[1], out var month))
                Month = month;

            if (persianDateSplit.Length >= 3 &&
                int.TryParse(persianDateSplit[2], out var day))
                Day = day;

            if (persianDateTimeSplit.Length != 2)
                return;

            var time = persianDateTimeSplit[1];
            var timeSplit = time.Split(':');
            if (timeSplit.Length >= 1 &&
                int.TryParse(timeSplit[0], out var hour))
                Hour = hour;

            if (timeSplit.Length >= 2 &&
                int.TryParse(timeSplit[1], out var minute))
                Minute = minute;

            if (timeSplit.Length >= 3 &&
                int.TryParse(timeSplit[2], out var second))
                Second = second;

            if (timeSplit.Length >= 4 &&
                double.TryParse(timeSplit[2], out var millisecond))
                Millisecond = millisecond;
        }

        public int Year { get; }

        public int Month { get; }

        public int Day { get; }

        public int Hour { get; }

        public int Minute { get; }

        public int Second { get; }

        public double Millisecond { get; }

        public override string ToString() => $"{Year}/{Month}/{Day} {Hour}:{Minute}:{Second}";

        public string ToDateString() => $"{Year}/{Month}/{Day}";

        public DateTime ToDateTime() =>
            new PersianCalendar().ToDateTime(Year, Month, Day, Hour, Minute, Second, (int) Millisecond);

        public static PersianDateTime FromDateTime(DateTime dateTime) => new(dateTime);

        public override bool Equals(object? obj) => GetHashCode() == obj?.GetHashCode();

        public override int GetHashCode() => HashCode.Combine(Year, Month, Day, Hour, Minute, Second, Millisecond);

        public static bool operator ==(PersianDateTime left, PersianDateTime right) => left.Equals(right);

        public static bool operator !=(PersianDateTime left, PersianDateTime right) => !(left == right);

        public static implicit operator DateTime(PersianDateTime persianDateTime) => persianDateTime.ToDateTime();

        public static implicit operator PersianDateTime(DateTime dateTime) => new(dateTime);

        public bool Equals(PersianDateTime other) => GetHashCode() == other.GetHashCode();
    }
}