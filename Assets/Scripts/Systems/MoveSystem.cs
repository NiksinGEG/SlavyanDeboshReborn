using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Map.WorldMap;

public class MoveSystem : IECSSystem
{
    public MoveSystem(ECSService s) : base(s) { }

    private List<HexCell> Travel(Movable c)
    {
        List<HexCell> way = new List<HexCell>();
        way.Add(HexCoords.FromPositionToCell(c.gameObject.GetComponent<Transform>().position));
        HexCell current = HexCoords.FromPositionToCell(c.gameObject.GetComponent<Transform>().position);
        HexCell endCell = HexCoords.FromPositionToCell(c.position);
        while (current != endCell)
        {
            var neighbours = current.neighbours;
            current = neighbours[0];
            double min = Math.Sqrt(Math.Pow(endCell.transform.position.x - current.transform.position.x, 2) + Math.Pow(endCell.transform.position.z - current.transform.position.z, 2));
            foreach(HexCell n in neighbours)
            {
                double local_min = Math.Sqrt(Math.Pow(endCell.transform.position.x - n.transform.position.x, 2) + Math.Pow(endCell.transform.position.z - n.transform.position.z, 2));
                if(local_min < min)
                {
                    min = local_min;
                    current = n;
                }
            }
            way.Add(current);
        }
        return way;
    }

    public override void Init()
    {
        ECSFilter f = new ECSFilter(Service);
        List<IECSComponent> components = f.GetComponents<Movable>();
        foreach (var _c in components)
        {
            Movable c = (Movable)_c;
            c.position = c.gameObject.GetComponent<Transform>().position;
        }
    }

    public override void Run()
    {
        ECSFilter f = new ECSFilter(Service);
        List<IECSComponent> components = f.GetComponents<Movable>();
        foreach(var _c in components)
        {
            Movable c = (Movable)_c;
            if (c.isSelected)
            {
                List<HexCell> travel = Travel(c);
                float eps = 0.1f;
                //Vector3 pos = c.gameObject.GetComponent<Transform>().position;
                foreach (var cell in travel)
                {
                    /*
                   if(c.gameObject.GetComponent<Transform>().position != cell.transform.position)                       
                   while (Mathf.Abs(c.gameObject.GetComponent<Transform>().position.x - cell.transform.position.x) > eps || Mathf.Abs(c.gameObject.GetComponent<Transform>().position.z - cell.transform.position.z) > eps)
                        c.gameObject.GetComponent<Transform>().position = Vector3.Lerp(c.gameObject.GetComponent<Transform>().position, c.position, 0.15f);*/
                }
                c.isSelected = !c.isSelected;
            }
        }
    }
}
