using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public static class GlobalVariables
{
    public static int Port = 1849;
    public static string HostAddress;

    public static GenerationSettings generationSettings;

    public static NetworkStream NetStream;

    public static string SelfName = "";
    public static string EnemyName = "";

    public static RaycastHit lastHit;
}
