namespace System;

public static class DateTimeExtensions
{
    public static int DifferenceInDaysFor(this DateTime firstDate, DateTime secondDate) => Math.Abs((firstDate - secondDate).Days);
}