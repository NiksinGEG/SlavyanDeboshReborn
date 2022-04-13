using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public void Select()
    {
        GetComponent<Selectable>().IsSelected = !GetComponent<Selectable>().IsSelected;
    }


}
