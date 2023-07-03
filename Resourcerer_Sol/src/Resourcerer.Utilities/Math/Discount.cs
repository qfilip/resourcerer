namespace Resourcerer.Utilities.Math;

public class Discount
{
    public static double Compute(double fullPrice, int totalDiscountPercent)
    {
        if (totalDiscountPercent > 0 && totalDiscountPercent <= 100)
        {
            return fullPrice - (fullPrice * totalDiscountPercent / 100);
        }
        else return fullPrice;
    }
}
