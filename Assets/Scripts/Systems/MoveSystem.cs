using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Map.WorldMap;
using Assets.Scripts;

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
        if (c.WayCells.Count == 0)
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
        ECSFilter f = new ECSFilter();
        List<Movable> components = f.GetComponents<Movable>();
        foreach (var c in components)
            UpdateComponent(c);
    }

    //TODO: оптимизировать GetWay
    public void SetWay(Movable comp, RaycastHit hit)
    {
        if (hit.transform.gameObject.GetComponentInParent<HexGridChunk>() == null) //if clicked at cell of a map
            return;
        var grid = hit.transform.gameObject.GetComponentInParent<HexGridChunk>().gameObject.GetComponentInParent<HexGrid>();
        var endCell = grid.GetByPosition(hit.point);
        var startPos = comp.gameObject.transform.position;
        var startCell = grid.GetByPosition(startPos);

        comp.WayCells = new List<Vector3>();
        List<HexCell> WayCells = hit.transform.gameObject.GetComponentInParent<HexGridChunk>().gameObject.GetComponentInParent<HexGrid>().GetWay((int)comp.moveType, startCell, endCell);
        comp.WayCells.Add(comp.transform.position);
        foreach (var cell in WayCells)
            comp.WayCells.Add(cell.transform.position);
    }


    //TODO: ловит ексепшн, видимо потому что при создании List<HexCell> вызывается конструктор HexCell(), который на побочном потоке вызывать нельзя
    public async void SetWayAsync(Movable comp, RaycastHit hit)
    {
        if (hit.transform.gameObject.GetComponentInParent<HexGridChunk>() == null) //if clicked at cell of a map
            return;
        var grid = hit.transform.gameObject.GetComponentInParent<HexGridChunk>().gameObject.GetComponentInParent<HexGrid>();
        List<HexCell> WayCells = new List<HexCell>();
        await Task.Run(() =>
        {
            var endCell = grid.GetByPosition(hit.point);
            var startPos = comp.gameObject.transform.position;
            var startCell = grid.GetByPosition(startPos);

            WayCells = grid.GetWay((int)comp.moveType, startCell, endCell);
        });
        comp.WayCells = new List<Vector3>();
        comp.WayCells.Add(comp.transform.position);
        foreach (var cell in WayCells)
            comp.WayCells.Add(cell.transform.position);
    }
}
