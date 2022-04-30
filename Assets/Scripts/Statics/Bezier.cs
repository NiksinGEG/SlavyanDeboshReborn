using System.Collections.Generic;
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

        public static Vector3 GetPointN(float t, List<Vector3> p)
        {
            if (p.Count < 2)
                return p[0];
            
            List<Vector3> newp = new List<Vector3>();
            for (int i = 0; i < p.Count - 1; i++)
            {
                Debug.DrawLine(p[i], p[i + 1]);
                Vector3 p0p1 = (1 - t) * p[i] + t * p[i + 1];
                newp.Add(p0p1);
            }
            return GetPointN(t, newp);
        }
    }
}
