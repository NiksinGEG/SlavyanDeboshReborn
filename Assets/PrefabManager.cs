using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour 
{
    [SerializeField] public MapResource[] tree_prefabs;
    [SerializeField] public MapResource[] rock_prefabs;
    [SerializeField] public MapResource[] grass_prefabs;
    [SerializeField] public MapResource[] forest_prefabs;
    [SerializeField] public MapResource[] desert_prefabs;
    [SerializeField] public MapResource[] tropic_tree_prefabs;
    [SerializeField] public MapResource[] tropic_grass_prefab;
    [SerializeField] public MapResource[] winter_tree_prefabs;
    [SerializeField] public MapResource[] winter_grass_prefabs;
    //[SerializeField] public PlayableObject[] building_prefabs;
}
