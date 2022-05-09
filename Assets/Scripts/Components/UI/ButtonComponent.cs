using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Components.UI
{
    public class ButtonComponent : IECSComponent
    {
        public bool Clicked;

        public string SystemType;

        public string MethodName;

        public void OnClick()
        {
            Clicked = true;
        }
    }
}

