using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Map.WorldMap;
using Assets.Scripts;
using Assets.Scripts.Statics;

public class MoveSystem : IECSSystem
{
    public MoveSystem(ECSService s) : base(s) { }

    public override void Init()
    {

    }

    private float UpdateT(Movable c)
    {
        var ms = c.MoveSpeed * Time.deltaTime;
        return c.t + ms;
    }

    private Vector3 GetNewPoint(Movable comp, float t)
    {
        return Bezier.GetPointN(t, comp.WayCells);
    }

    private void OnFinishPosition(Movable c)
    {
        c.t = 0;
        c.WayCells.Clear();
    }

    private bool UpdateRotation(Movable c, Vector3 newPoint)
    {
        Quaternion toRotation = Quaternion.LookRotation(newPoint - c.transform.position);
        bool res = Quaternion.Angle(c.transform.rotation, toRotation) < 20f;
        Quaternion newRotation = Quaternion.RotateTowards(c.transform.rotation, toRotation, c.RotationSpeed * Time.deltaTime);
        c.transform.rotation = newRotation;
        return res;
    }

    private void UpdateComponent(Movable c)
    {
        if (c.WayCells.Count == 0 || c.WayCells == null)
            return;
        var newT = UpdateT(c);
        if (newT > 1f)
            OnFinishPosition(c);
        else
        {
            var newPoint = GetNewPoint(c, newT);
            bool canMove = UpdateRotation(c, newPoint);
            if (canMove)
            {
                c.transform.position = newPoint;
                c.t = newT;
            }
        }
    }

    public override void Run()
    {
        Map.getInstance();

        ECSFilter f = new ECSFilter();
        List<Movable> components = f.GetComponents<Movable>();
        foreach (var c in components)
            UpdateComponent(c);
    }

    //TODO: оптимизировать GetWay
    public void SetWay(Movable comp, RaycastHit hit)
    {
        if (hit.transform.gameObject.GetComponentInParent<HexGridChunk>() == null && hit.transform.gameObject.GetComponent<MapResource>() == null) //if clicked at cell of a map
                return;

        var endCell = Map.GetByPosition(hit.point);
        var startCell = Map.GetByPosition(comp.gameObject.transform.position);
        if (startCell == endCell)
            return;

        comp.WayCells = new List<Vector3>();
        List<HexCell> WayCells = Map.GetWay((int)comp.moveType, startCell, endCell);
        comp.WayCells.Add(comp.transform.position);
        foreach (var cell in WayCells)
            comp.WayCells.Add(cell.transform.position);


        var net_c = comp.gameObject.GetComponent<NetComponent>();
        if (net_c != null)
        {
            net_c.ComponentToSend = comp;
            net_c.NeedSend = true;
        }
    }


    //TODO: ловит ексепшн, видимо потому что при создании List<HexCell> вызывается конструктор HexCell(), который на побочном потоке вызывать нельзя
    public async void SetWayAsync(Movable comp, RaycastHit hit)
    {
        if (hit.transform.gameObject.GetComponentInParent<HexGridChunk>() == null) //if clicked at cell of a map
            return;
        List<HexCell> WayCells = new List<HexCell>();
        await Task.Run(() =>
        {
            var endCell = Map.GetByPosition(hit.point);
            var startCell = Map.GetByPosition(comp.gameObject.transform.position);

            WayCells = Map.GetWay((int)comp.moveType, startCell, endCell);
        });
        comp.WayCells = new List<Vector3>();
        comp.WayCells.Add(comp.transform.position);
        foreach (var cell in WayCells)
            comp.WayCells.Add(cell.transform.position);
    }
}
