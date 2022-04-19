
using System.Collections.Generic;
using UnityEngine;
using Assets.Map.WorldMap;

namespace Assets.Scripts
{
    static class Travel
    {
        //private int[,] mapMatrix = new int[,]; 
       

        public static void Moving(Movable c)
        {
            float eps = 0.15f;
            if (Mathf.Abs(c.gameObject.GetComponent<Transform>().position.x - c.WayCells[0].transform.position.x) > eps ||
                        Mathf.Abs(c.gameObject.GetComponent<Transform>().position.z - c.WayCells[0].transform.position.z) > eps)
            {
                c.gameObject.GetComponent<Transform>().position = Vector3.MoveTowards(c.gameObject.transform.position, c.WayCells[0].transform.position, c.MoveSpeed);
            }
            else
                c.WayCells.Remove(c.WayCells[0]);
        }
    }
}
