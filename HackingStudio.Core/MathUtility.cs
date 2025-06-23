using System.Numerics;
using System.Runtime.CompilerServices;

namespace Maths;

/// <summary>
/// Provides mathematical utility functions.
/// </summary>
public static class MathUtility
{
    /// <summary>
    /// Raises a number to a specified power.
    /// </summary>
    /// <typeparam name="TNum">The numeric type.</typeparam>
    /// <param name="value">The base value.</param>
    /// <param name="power">The exponent.</param>
    /// <returns>The result of raising the base value to the specified power.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNum Pow<TNum>(TNum value, int power) where TNum : INumber<TNum>
    {
        TNum result = value;

        for (int i = 1; i < power; i++) {
            result *= value;
        }

        return result;
    }

    /// <summary>
    /// Sums a list of values selected from a collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <typeparam name="TNum">The numeric type of the sum.</typeparam>
    /// <param name="items">The collection of items.</param>
    /// <param name="selector">A function to select the values to sum.</param>
    /// <returns>The sum of the selected values.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNum Sum<T, TNum>(IList<T> items, Func<T, TNum> selector) where TNum : INumber<TNum>, new()
    {
        TNum result = new();

        for (int i = 0; i < items.Count; i++) {
            var num = selector(items[i]);
            result += num;
        }

        return result;
    }
    
    /// <summary>
    /// Sums a list of values selected from a collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <typeparam name="TNum">The numeric type of the sum.</typeparam>
    /// <param name="items">The collection of items.</param>
    /// <param name="selector">A function to select the values to sum.</param>
    /// <returns>The sum of the selected values.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNum Sum<T, TNum>(IEnumerable<T> items, Func<T, TNum> selector) where TNum : INumber<TNum>, new()
    {
        TNum result = new();

        foreach (var x in items) {
            var num = selector(x);
            result += num;
        }

        return result;
    }

    /// <summary>
    /// Calculates the percentage difference between two numeric values.
    /// </summary>
    /// <typeparam name="TNum">The numeric type.</typeparam>
    /// <param name="start">The starting value.</param>
    /// <param name="end">The ending value.</param>
    /// <returns>The percentage difference between the start and end values.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNum CalculatePercentageDifference<TNum>(TNum start, TNum end) where TNum : INumber<TNum>
    {
        if (start == TNum.Zero) {
            throw new ArgumentException("Start value cannot be zero, because division by zero is a no-go!");
        }

        TNum difference = end - start;
        TNum percentageDifference = difference / start * TNum.CreateChecked(100);

        return percentageDifference;
    }

    /// <summary>
    /// Calculates the linear parameter that produces the interpolant value within the range [a, b].
    /// </summary>
    /// <typeparam name="TNum">The numeric type.</typeparam>
    /// <param name="a">The start value.</param>
    /// <param name="b">The end value.</param>
    /// <param name="value">The interpolant value.</param>
    /// <returns>The linear parameter that produces the interpolant value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNum ReverseLerp<TNum>(TNum a, TNum b, TNum value) where TNum : INumber<TNum>, new()
    {
        if (a == b) {
            throw new ArgumentException("a and b cannot be the same value.");
        }

        return (value - a) / (b - a);
    }

    /// <summary>
    /// Linearly interpolates between two values based on a parameter.
    /// </summary>
    /// <param name="a">The start value.</param>
    /// <param name="b">The end value.</param>
    /// <param name="t">The interpolation parameter, clamped between 0 and 1.</param>
    /// <returns>The interpolated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Lerp(float a, float b, float t)
	{
		t = Math.Clamp(t, 0.0f, 1.0f);

		return a + (b - a) * t;
	}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSexyPrime(int i)
    {
        // First, if i isn’t prime, it can’t be sexy!
        if (!IsPrime(i))
            return false;

        // Check if either i + 6 or i - 6 (if i is large enough) is prime.
        return IsPrime(i + 6) || (i > 6 && IsPrime(i - 6));
    }

    // A simple helper method to check for primality.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsPrime(int n)
    {
        if (n < 2)
            return false;
        if (n == 2)
            return true;
        if (n % 2 == 0)
            return false;
        int boundary = (int)Math.Sqrt(n);
        for (int j = 3; j <= boundary; j += 2) {
            if (n % j == 0)
                return false;
        }
        return true;
    }
}