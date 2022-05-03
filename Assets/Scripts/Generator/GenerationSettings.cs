using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System;

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GenerationSettings
{
    public int Seed;

    public int tropicTreeProcent;
    public int standartTreeProcent;
    public int taigaTreeProcent;
    public int winterTreeProcent;

    public int terrainChunkCountX;
    public int terrainChunkCountY;

    public int rockProcent;

    public int mainlandsCount;

    public int mixingBiomesCount;

    /*public byte[] GetBytes()
    {
        byte[] bytes = new byte[Marshal.SizeOf(typeof(GenerationSettings))];
        GCHandle pinStructure = GCHandle.Alloc(this, GCHandleType.Pinned);
        try
        {
            Marshal.Copy(pinStructure.AddrOfPinnedObject(), bytes, 0, bytes.Length);
            return bytes;
        }
        finally
        {
            pinStructure.Free();
        }
    }

    public static int GetSize()
    {
        return 10 * sizeof(int);
    }

    public void Set(byte[] byteArr)
    {

    }*/
}
