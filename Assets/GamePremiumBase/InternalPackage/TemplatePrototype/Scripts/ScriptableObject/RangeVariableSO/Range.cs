using System;
using UnityEngine;

[Serializable]
public struct Range<T>
{
    #region Constructors
    public Range(T minValue, T maxValue)
    {
        m_MinValue = minValue;
        m_MaxValue = maxValue;
    }
    #endregion

    [SerializeField]
    private T m_MinValue;
    [SerializeField]
    private T m_MaxValue;

    public T minValue
    {
        get => m_MinValue;
        set => m_MinValue = value;
    }
    public T maxValue
    {
        get => m_MaxValue;
        set => m_MaxValue = value;
    }

    public override string ToString()
    {
        return $"[{minValue}, {maxValue}]";
    }
}
public static class RangeExtensions
{
    /// <summary>
    /// Calculate inverse interpolated value range (0 - 1)
    /// </summary>
    /// <param name="value">Current value in range (min-max)</param>
    /// <returns>Return inverse interpolated value range (0 - 1)</returns>
    public static float CalcInverseLerpValue(this Range<float> range, float value)
    {
        return Mathf.InverseLerp(range.minValue, range.maxValue, value);
    }

    /// <summary>
    /// Calculate interpolated value range (min - max) base on weight (0 - 1)
    /// </summary>
    /// <param name="weight">Weight factor range (0 - 1)</param>
    /// <returns>Return interpolated value range (min - max)</returns>
    public static float CalcInterpolatedValue(this Range<float> range, float weight)
    {
        return Mathf.Lerp(range.minValue, range.maxValue, weight);
    }

    /// <summary>
    /// Check whether value is out of range or not
    /// </summary>
    /// <param name="value">Value to check</param>
    /// <returns>Return true if value out of range otherwise false</returns>
    public static bool IsOutOfRange(this Range<float> range, float value)
    {
        return value < range.minValue || value > range.maxValue;
    }

    /// <summary>
    /// Calculate inverse interpolated value range (0 - 1)
    /// </summary>
    /// <param name="value">Current value in range (min-max)</param>
    /// <returns>Return inverse interpolated value range (0 - 1)</returns>
    public static float CalcInverseLerpValue(this Range<int> range, int value)
    {
        return Mathf.InverseLerp(range.minValue, range.maxValue, value);
    }

    /// <summary>
    /// Calculate interpolated value range (min - max) base on weight (0 - 1)
    /// </summary>
    /// <param name="weight">Weight factor range (0 - 1)</param>
    /// <returns>Return interpolated value range (min - max)</returns>
    public static int CalcInterpolatedValue(this Range<int> range, float weight)
    {
        return Mathf.RoundToInt(Mathf.Lerp(range.minValue, range.maxValue, weight));
    }

    /// <summary>
    /// Check whether value is out of range or not
    /// </summary>
    /// <param name="value">Value to check</param>
    /// <returns>Return true if value out of range otherwise false</returns>
    public static bool IsOutOfRange(this Range<int> range, int value)
    {
        return value < range.minValue || value > range.maxValue;
    }

    /// <summary>
    /// Calculate inverse interpolated value range (0 - 1)
    /// </summary>
    /// <param name="value">Current value in range (min-max)</param>
    /// <returns>Return inverse interpolated value range (0 - 1)</returns>
    public static float CalcInverseLerpValue(this Range<Vector3> range, Vector3 value)
    {
        if (range.IsOutOfRange(value))
            return 0f;
        var ba = range.maxValue - range.minValue;
        var ca = value - range.minValue;
        return Mathf.Clamp01(Vector3.Dot(ca, ba) / Vector3.Dot(ba, ba));
    }

    /// <summary>
    /// Calculate interpolated value range (min - max) base on weight (0 - 1)
    /// </summary>
    /// <param name="weight">Weight factor range (0 - 1)</param>
    /// <returns>Return interpolated value range (min - max)</returns>
    public static Vector3 CalcInterpolatedValue(this Range<Vector3> range, float weight)
    {
        return Vector3.Lerp(range.minValue, range.maxValue, weight);
    }

    /// <summary>
    /// Check whether value is out of range or not
    /// </summary>
    /// <param name="value">Value to check</param>
    /// <returns>Return true if value out of range otherwise false</returns>
    public static bool IsOutOfRange(this Range<Vector3> range, Vector3 value)
    {
        // Check if vector value is collinear with vector min and max by calculate the angle of 2 vector
        var ba = range.maxValue - range.minValue;
        var ca = value - range.minValue;
        if (!Mathf.Approximately(Vector3.Angle(ca, ba), 0f))
            return true;
        // Check whether magnitude of vector CA(min-value) is greater than BA(min-max) or not
        if (ca.magnitude > ba.magnitude)
            return true;
        return false;
    }
}