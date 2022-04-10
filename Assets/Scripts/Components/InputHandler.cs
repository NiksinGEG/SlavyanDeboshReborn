using UnityEngine;
using UnityEngine.Events;

public class InputHandler : IECSComponent
{
    ///<summary>
    /// Методы, выполняемые при нажатии ЛКМ на объект, на котором висит этот компонент
    /// </summary>
    [SerializeField] public UnityEvent MouseLKM;
    ///<summary>
    /// Методы, выполняемые при нажатии ПКМ на объект, на котором висит этот компонент
    /// </summary>
    [SerializeField] public UnityEvent MouseRKM;
    ///<summary>
    /// Методы, выполняемые при прокрутке колёсика мыши
    /// </summary>
    [SerializeField] public UnityEvent MouseWheelUp;
    ///<summary>
    /// Методы, выполняемые при прокрутке колёсика мыши
    /// </summary>
    [SerializeField] public UnityEvent MouseWheelDown;
    ///<summary>
    /// Методы, выполняемые при нажатии клавиши клавиатуры
    /// </summary>
    [SerializeField] public UnityEvent KeyPressed;
}
