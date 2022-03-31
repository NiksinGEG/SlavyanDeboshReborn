using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour 
{
    public MapResource rockPrefab;
    public MapResource TreePrefab;
    //public .....

    public MapResource RockPrefab
    {
        get { return rockPrefab; }
        set { rockPrefab = value; }
    }

}
