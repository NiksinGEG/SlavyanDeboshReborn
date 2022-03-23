using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HexCoords
{
    [SerializeField]
    private int _x, _z;
    public int x
    {
        get { return _x; }
    }
    public int z
    {
        get { return _z; }
    }
    public int y
    {
        get { return -x - z; }
    }

    public HexCoords(int x, int z)
    {
        _x = x;
        _z = z;
    }

    public static HexCoords FromOffset(int x, int z)
    {
        return new HexCoords(x - z / 2, z);
    }

    public static HexCoords FromPosition(Vector3 position)
    {
        float x = position.x / (HexMetrics.innerRadius * 2f);
        float y = -x;

        float offset = position.z / (HexMetrics.outerRadius * 3f);
        x -= offset;
        y -= offset;

        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-x - y);

        if(iX + iY + iZ != 0)
        {
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x - y - iZ);

            if (dX > dY && dX > dZ)
            {
                iX = -iY - iZ;
            }
            else if (dZ > dY)
            {
                iZ = -iX - iY;
            }
        }

        return new HexCoords(iX, iZ);
    }

    public override string ToString()
    {
        return $"({x}, {y}, {z})";
    }

    public bool EqualsTo(HexCoords coords)
    {
        return x == coords.x && y == coords.y && z == coords.z;
    }
}
