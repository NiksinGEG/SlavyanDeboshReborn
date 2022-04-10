using UnityEngine;
using UnityEngine.Events;

public class InputHandler : IECSComponent
{
    ///<summary>
    /// ������, ����������� ��� ������� ��� �� ������, �� ������� ����� ���� ���������
    /// </summary>
    [SerializeField] public UnityEvent MouseLKM;
    ///<summary>
    /// ������, ����������� ��� ������� ��� �� ������, �� ������� ����� ���� ���������
    /// </summary>
    [SerializeField] public UnityEvent MouseRKM;
    ///<summary>
    /// ������, ����������� ��� ��������� ������� ����
    /// </summary>
    [SerializeField] public UnityEvent MouseWheelUp;
    ///<summary>
    /// ������, ����������� ��� ��������� ������� ����
    /// </summary>
    [SerializeField] public UnityEvent MouseWheelDown;
    ///<summary>
    /// ������, ����������� ��� ������� ������� ����������
    /// </summary>
    [SerializeField] public UnityEvent KeyPressed;
}
