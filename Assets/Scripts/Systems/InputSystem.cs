using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Systems.Input;
using UnityEngine;

public class InputSystem : IECSSystem
{ 
    public InputSystem(ECSService service) : base(service) { }

    /// <summary>
    /// ����������� ���� ��� �� ������, �� ����� �������� ���� ������� �� ���
    /// </summary>
    public MouseEvent MouseDownLKM = new MouseEvent();
    
    /// <summary>
    /// ����������� ������ ����� ���� ������ ���
    /// </summary>
    public MouseEvent MouseClickLKM = new MouseEvent();

    /// <summary>
    /// ����������� ������ ����� ���� ������ ���
    /// </summary>
    public MouseEvent MouseClickRKM = new MouseEvent();

    /// <summary>
    /// ����������� ���� ��� �� ������, �� ����� �������� ���� ������� �� ���
    /// </summary>
    public MouseEvent MouseDownRKM = new MouseEvent();

    public MouseEvent MouseUpLKM = new MouseEvent();
    public MouseEvent MouseUpRKM = new MouseEvent();

    private RaycastHit? GetHit()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
            return hit;
        else
            return null;
    }

    public override void Run()
    {
        if(Input.GetMouseButton(0))
        {
            var hit = GetHit();
            if (hit != null)
                MouseClickLKM.Invoke((RaycastHit)hit);
        }
        if(Input.GetMouseButtonDown(0))
        {
            var hit = GetHit();
            if(hit != null)
                MouseDownLKM.Invoke((RaycastHit)hit);
        }
        if (Input.GetMouseButton(1))
        {
            var hit = GetHit();
            if (hit != null)
                MouseClickRKM.Invoke((RaycastHit)hit);
        }
        if (Input.GetMouseButtonDown(1))
        {
            var hit = GetHit();
            if (hit != null)
                MouseDownRKM.Invoke((RaycastHit)hit);
        }
        if(Input.GetMouseButtonUp(0))
        {
            var hit = GetHit();
            if(hit != null)
                MouseUpLKM.Invoke((RaycastHit)hit);
        }
        if (Input.GetMouseButtonUp(1))
        {
            var hit = GetHit();
            if (hit != null)
                MouseUpRKM.Invoke((RaycastHit)hit);
        }
    }
}
