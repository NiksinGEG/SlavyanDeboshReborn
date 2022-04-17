
using System.Collections.Generic;
using UnityEngine;
using Assets.Map.WorldMap;

namespace Assets.Scripts
{
    static class Travel
    {
        public static List<HexCell> GetWay(HexCell startCell, HexCell endCell)
        {
            List<HexCell> res = new List<HexCell>();

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
    }
}
