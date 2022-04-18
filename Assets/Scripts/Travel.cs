
using System.Collections.Generic;
using UnityEngine;
using Assets.Map.WorldMap;

namespace Assets.Scripts
{
    static class Travel
    {
        //private int[,] mapMatrix = new int[,]; 
        public static List<HexCell> GetWay(Movable c, HexCell startCell, HexCell endCell)
        {
            List<HexCell> res = new List<HexCell>();
            if (c.IsSwimAndMove)
            {
                res.Add(startCell);
                while (startCell != endCell)
                {
                    var neighbours = startCell.neighbours;
                    startCell = neighbours[0];
                    double min = Mathf.Sqrt(Mathf.Pow(endCell.transform.position.x - startCell.transform.position.x, 2) + Mathf.Pow(endCell.transform.position.z - startCell.transform.position.z, 2));
                    foreach (var cell in neighbours)
                    {
                        double local_min = Mathf.Sqrt(Mathf.Pow(endCell.transform.position.x - cell.transform.position.x, 2) + Mathf.Pow(endCell.transform.position.z - cell.transform.position.z, 2));
                        if (local_min < min)
                        {
                            min = local_min;
                            startCell = cell;
                        }
                    }
                    res.Add(startCell);
                }
                return res;
            }
            if(c.IsSwimming)
            {

                return res;
            }
            if(!c.IsSwimming)
            {

                return res;
            }
            return res;
        }

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
