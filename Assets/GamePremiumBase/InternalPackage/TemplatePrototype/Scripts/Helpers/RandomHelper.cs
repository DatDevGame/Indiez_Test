using System;
using UnityEngine;
using UnityEngine.Animations;
using static UnityEngine.ParticleSystem;

namespace HCore.Helpers
{
    public static class RandomHelper
    {
        /// <summary>
        /// Prevents infinite loop and crash UnityEditor in while loop
        /// </summary>
        private const int MaxAttempt = 100;

        public static float Random01(Predicate<float> predicate = null)
        {
            return RandomRange(0, 1, predicate);
        }

        public static float RandomOpposite(this float range, Predicate<float> predicate = null)
        {
            return RandomRange(-range, range, predicate);
        }

        public static int RandomOpposite(this int range, Predicate<int> predicate = null)
        {
            return RandomRange(-range, range, predicate);
        }

        public static float RandomRange(this Range<float> range, Predicate<float> predicate = null)
        {
            return RandomRange(range.minValue, range.maxValue, predicate);
        }

        public static float RandomRange(this RangeFloatVariable floatRangeVariable, Predicate<float> predicate = null)
        {
            return RandomRange(floatRangeVariable.rangeValue, predicate);
        }

        public static int RandomRange(this Range<int> range, Predicate<int> predicate = null)
        {
            return RandomRange(range.minValue, range.maxValue, predicate);
        }

        public static int RandomRange(this RangeIntVariable intRangeVariable, Predicate<int> predicate = null)
        {
            return RandomRange(intRangeVariable.rangeValue, predicate);
        }

        public static Vector3 RandomRange(this Range<Vector3> range, Predicate<Vector3> predicate = null)
        {
            return RandomRange(range.minValue, range.maxValue, predicate);
        }

        public static Vector3 RandomRange(this RangeVector3Variable vector3RangeVariable, Predicate<Vector3> predicate = null)
        {
            return RandomRange(vector3RangeVariable.rangeValue, predicate);
        }

        public static float RandomRange(float min, float max, Predicate<float> predicate = null)
        {
            int attempt = 0;
            while (true)
            {
                var randomValue = UnityEngine.Random.Range(min, max);
                if ((predicate?.Invoke(randomValue) ?? true) || attempt > MaxAttempt)
                    return randomValue;
                attempt++;
            }
        }

        public static int RandomRange(int min, int max, Predicate<int> predicate = null)
        {
            int attempt = 0;
            while (true)
            {
                var randomValue = UnityEngine.Random.Range(min, max);
                if ((predicate?.Invoke(randomValue) ?? true) || attempt > MaxAttempt)
                    return randomValue;
                attempt++;
            }
        }

        public static Vector3 RandomRange(Vector3 min, Vector3 max, Predicate<Vector3> predicate = null)
        {
            int attempt = 0;
            while (true)
            {
                var randomValue = new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
                if ((predicate?.Invoke(randomValue) ?? true) || attempt > MaxAttempt)
                    return randomValue;
                attempt++;
            }
        }

        public static float RandomRange(this MinMaxCurve minMaxCurve)
        {
            return RandomRange(minMaxCurve.constantMin, minMaxCurve.constantMax);
        }

        public static Vector3 RandomDirection(this Axis axisFlag)
        {
            var randomDirection = Vector3.zero;
            if ((axisFlag & Axis.X) != 0)
                randomDirection += Vector3.right * RandomOpposite(1f);
            if ((axisFlag & Axis.Y) != 0)
                randomDirection += Vector3.up * RandomOpposite(1f);
            if ((axisFlag & Axis.Z) != 0)
                randomDirection += Vector3.forward * RandomOpposite(1f);
            return randomDirection;
        }

        public static Vector3 RandomDirection(Predicate<Vector3> predicate, Axis axisFlag = Axis.X | Axis.Y | Axis.Z)
        {
            int attempt = 0;
            while (true)
            {
                var randomDirection = RandomDirection(axisFlag);
                if ((predicate?.Invoke(randomDirection) ?? true) || attempt > MaxAttempt)
                    return randomDirection;
                attempt++;
            }
        }

        public static Vector3 RandomPositionByBounds(BoundingSphere boundingSphere, Predicate<Vector3> predicate = null)
        {
            int attempt = 0;
            while (true)
            {
                var randomNormalizedDirection = RandomDirection(Axis.X | Axis.Y | Axis.Z).normalized;
                var randomPoint = boundingSphere.position + randomNormalizedDirection * boundingSphere.radius;
                if ((predicate?.Invoke(randomPoint) ?? true) || attempt > MaxAttempt)
                    return randomPoint;
                attempt++;
            }
        }

        public static Vector3 RandonPositionByBounds(Bounds boundingBox, Predicate<Vector3> predicate = null)
        {
            int attempt = 0;
            while (true)
            {
                var randomNormalizedDirection = new Vector3(
                    RandomOpposite(boundingBox.extents.x),
                    RandomOpposite(boundingBox.extents.y),
                    RandomOpposite(boundingBox.extents.z));

                var randomPoint = boundingBox.center + randomNormalizedDirection;
                if ((predicate?.Invoke(randomPoint) ?? true) || attempt > MaxAttempt)
                    return randomPoint;
                attempt++;
            }
        }
    }
}
