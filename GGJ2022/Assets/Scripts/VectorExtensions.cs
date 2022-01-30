using UnityEngine;

namespace Assets.Scripts
{
    public static class VectorExtensions
    {
        public static Vector3 DivideBy(this Vector3 numerator, Vector3 denominator)
        {
            return new Vector3(
                numerator.x / denominator.x,
                numerator.y / denominator.y,
                numerator.z / denominator.z
            );
        }

        public static Vector3 ScaleBy(this Vector3 multiplier1, Vector3 multiplier2)
        {
            return new Vector3(
                multiplier1.x * multiplier2.x,
                multiplier1.y * multiplier2.y,
                multiplier1.z * multiplier2.z
            );
        }

        public static Bounds ScaleBy(this Bounds bounds, Vector3 scale)
        {
            return new Bounds(bounds.center, bounds.size.ScaleBy(scale));
        }

        public static Vector3 GetAveragedByYScale(float scaleX, float scaleZ)
        {
            return new Vector3(scaleX, (scaleX + scaleZ) / 2, scaleZ);
        }
    }
}
