using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������, ����������� ������������.<br/> �����: �� ����� ������� ����� � ����������.
/// </summary>
public class IECSSystem
{
    /// <summary>
    /// ������, ��������������� ���������� � �����������
    /// </summary>
    public ECSService Service;
    public IECSSystem(ECSService service) { Service = service; }
    /// <summary>
    /// ���������� ������ ����(�����).
    /// </summary>
    public virtual void Run() { }
    /// <summary>
    /// ���������� �� ���� ��� ��������� ������ Run(). ������� �������� ��� ��������� ��������� ��������.<br/>
    /// �����: ����� ����������� �� ���� ��� �������������� ��� ������� �� �����. � ������� �������.
    /// </summary>
    public virtual void Init() { }
}
