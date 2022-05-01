using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public static class GlobalVariables
{
    public static int Seed;
    public static int Port = 1849;
    public static string HostAddress;

    public static Convertor convertor;

    public static NetworkStream NetStream;
}
