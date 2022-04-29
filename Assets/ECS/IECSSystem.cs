using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Система, управляющая компонентами.<br/> Важно: не имеет никакой связи с сущностями.
/// </summary>
public class IECSSystem
{
    /// <summary>
    /// Сервис, предоставляющий информацию о компонентах
    /// </summary>
    public ECSService Service;
    public IECSSystem(ECSService service) { Service = service; }
    /// <summary>
    /// Вызывается каждый кадр(фрейм).
    /// </summary>
    public virtual void Run() { }
    /// <summary>
    /// Вызывается до того как вызовется первый Run(). Отлично подходит для установки начальных значений.<br/>
    /// Важно: может выполниться до того как инициализуются все объекты на сцене. В будущем пофикшу.
    /// </summary>
    public virtual void Init() { }
}
