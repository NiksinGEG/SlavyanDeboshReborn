using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour 
{
    [SerializeField] 
    public MapResource[] tree_prefabs;
    [SerializeField]
    public MapResource[] rock_prefabs;
    [SerializeField]
    public MapResource[] grass_prefabs;
    [SerializeField]
    public MapResource[] forest_prefabs;
}
