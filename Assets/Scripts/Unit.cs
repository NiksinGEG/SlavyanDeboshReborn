using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Assets.Map.WorldMap;

public class Unit : MonoBehaviour
{
    public void Select()
    {
        GetComponent<Selectable>().IsSelected = !GetComponent<Selectable>().IsSelected;
    }

    public void SetCoords()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        { 
            //transform.localPosition = hit.point;
            //transform.localPosition = hit.transform.parent.transform.localPosition
            var choosenChunk = hit.transform.gameObject.GetComponentInParent<HexGridChunk>();
            HexCoords tmp = HexCoords.FromPosition(hit.point);
            Vector3 coords;
            coords.x = tmp.x; coords.y = tmp.y; coords.z = tmp.z;
            transform.position = HexCoords.FromHitToCoords(hit);
        }
            
    }
}
