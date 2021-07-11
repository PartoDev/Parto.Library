using System;

namespace Parto.Helper.PersionDateTime
{
    public interface IPersianDateTime : IEquatable<PersianDateTime>
    {
        int Year { get; }
        int Month { get; }
        int Day { get; }
        int Hour { get; }
        int Minute { get; }
        int Second { get; }
        double Millisecond { get; }
        string ToString();
        string ToDateString();
        DateTime ToDateTime();
        bool Equals(object? obj);
        new bool Equals(PersianDateTime other);
        int GetHashCode();
    }
}