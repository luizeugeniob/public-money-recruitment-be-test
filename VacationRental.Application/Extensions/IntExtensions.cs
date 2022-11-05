namespace System;

public static class IntExtensions
{
    public static int RandomIntExcept(this int maxNumber, IEnumerable<int> except)
    {
        var exclude = new HashSet<int>(except);
        var range = Enumerable.Range(1, maxNumber).Where(i => !exclude.Contains(i));

        var rand = new Random();
        int index = rand.Next(0, maxNumber - exclude.Count);
        return range.ElementAt(index);
    }
}
