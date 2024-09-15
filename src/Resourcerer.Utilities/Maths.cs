namespace Resourcerer.Utilities;

public class Maths
{
    public static decimal Discount(decimal fullPrice, int totalDiscountPercent)
    {
        if (totalDiscountPercent > 0 && totalDiscountPercent <= 100)
        {
            return fullPrice - fullPrice * totalDiscountPercent / 100;
        }
        else return fullPrice;
    }

    public static double SafeAverage<T>(IEnumerable<T> items, Func<T, double> selector)
    {
        if(items.Any())
        {
            return items.Average(selector);
        }
        else
        {
            return 0;
        }
    }
}
