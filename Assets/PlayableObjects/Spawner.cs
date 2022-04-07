using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Map.WorldMap;

public class Spawner : MonoBehaviour
{
    PrefabManager manager;
    MeshFilter meshFilter;
    List<MeshFilter> meshFilters;
    private void Awake()
    {
        manager = FindObjectOfType<PrefabManager>();
        meshFilter = GetComponent<MeshFilter>();
        meshFilters = new List<MeshFilter>();

        PlayableObject trop = Instantiate(ChooseUnitPrefab(UnitType.trooper));
        meshFilters.Add(trop.GetComponent<MeshFilter>());

        trop.transform.SetParent(transform);
        
        //Vector3 pos = cell.transform.position;

        trop.SetInnerPosition(0, 0);
    }

    private PlayableObject ChooseUnitPrefab(UnitType unitType)
    {
        if (manager == null)
            Awake();
        return manager.unit_prefabs[(int)unitType];
    }


}
