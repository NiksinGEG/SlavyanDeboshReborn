using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Systems.Input
{
    public class MouseEvent
    {
        private List<Action<RaycastHit>> funcs = new List<Action<RaycastHit>>();
        public void Invoke(RaycastHit hit)
        {
            foreach (var act in funcs)
                act.Invoke(hit);
        }
        public void AddEvent(Action<RaycastHit> action)
        {
            funcs.Add(action);
        }
        public void RemoveEvent(Action<RaycastHit> action)
        {
            funcs.Remove(action);
        }
        public MouseEvent()
        {
            funcs = new List<Action<RaycastHit>>();
        }

        public static MouseEvent operator +(MouseEvent m, Action<RaycastHit> act)
        {
            m.AddEvent(act);
            return m;
        }
        public static MouseEvent operator -(MouseEvent m, Action<RaycastHit> act)
        {
            m.RemoveEvent(act);
            return m;
        }
    }
}

