using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Map.WorldMap;

public struct HexCoords
{
    [SerializeField]
    private int _x, _z;
    public int x
    {
        get { return _x; }
        set { _x = value; }
    }
    public int z
    {
        get { return _z; }
        set { _z = value; }
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

    public int MakeIndex(int mapWidth)
    {
        return x + z * mapWidth + z / 2;
    }

    public HexCoords GetNeighbourCoords(int direction)
    {
        HexCoords nei_coords = new HexCoords();
        nei_coords.x = x;
        nei_coords.z = z;
        switch (direction)
        {
            case 0:
                nei_coords.z += 1;
                break;
            case 1:
                nei_coords.x += 1;
                break;
            case 2:
                nei_coords.x += 1;
                nei_coords.z -= 1;
                break;
            case 3:
                nei_coords.z -= 1;
                break;
            case 4:
                nei_coords.x -= 1;
                break;
            case 5:
                nei_coords.x -= 1;
                nei_coords.z += 1;
                break;
        }
        return nei_coords;
    }
}
