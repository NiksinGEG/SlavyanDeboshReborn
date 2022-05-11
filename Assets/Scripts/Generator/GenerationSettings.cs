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

    public float waterHeihgt;
    public float sandHeight;
    public float terrainHeight;
    public float rockHeight;

    public int mixingBiomesCount;

    public float[,] heightMatrix; 

    public float minHeight;
    public float maxHeight;
}
