using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Используется для получения системой компонентов
/// </summary>
/// <typeparam name="T">Тип компонентов, с которыми будет работать система</typeparam>
public class ECSFilter<T> : IEnumerable<T>, IEnumerator<T> where T: IECSComponent
{
    private int _pos;
    private Func<T, bool> _pred;

    public ECSFilter()
    {
        _pos = -1;
        _pred = null;
    }

    public ECSFilter(Func<T, bool> predicate)
    {
        _pos = -1;
        _pred = predicate;
    }

    public bool MoveNext()
    {
        var cnt = ECSInstance.Instance().Components.Count;
        while (_pos < cnt 
                && ECSInstance.Instance().Components[_pos].GetType() != typeof(T)
                && (_pred == null || !_pred.Invoke((T)ECSInstance.Instance().Components[_pos])))
            _pos++;
        return _pos < cnt;
    }

    public void Reset()
    {
        _pos = -1;
    }
    public void Dispose() { }

    public T Current
    {
        get
        {
            if (_pos == -1 || _pos >= ECSInstance.Instance().Components.Count)
                throw new ArgumentException();
            return (T)ECSInstance.Instance().Components[_pos];
        }
    }
    object IEnumerator.Current => throw new NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < ECSInstance.Instance().Components.Count; i++)
            if (ECSInstance.Instance().Components[i].GetType() == typeof(T)
                && (_pred == null || _pred.Invoke((T)ECSInstance.Instance().Components[i])))
                yield return (T)ECSInstance.Instance().Components[i];
    }

    public int Count
    {
        get
        {
            int res = 0;
            foreach (var c in this)
                res++;
            return res;
        }
    }

    public T First()
    {
        foreach (var c in this)
            return c;
        return null;
    }
}
