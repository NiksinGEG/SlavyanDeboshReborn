using UnityEngine;

namespace Assets.Scripts
{
    public static class Bezier
    {
        public static Vector3 GetPoint(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            float r = 1f - t;
            return Mathf.Pow((1 - t), 2) * a + 2f * t * (1 - t) * b + Mathf.Pow(t, 2) * c;
        }
    }
}
