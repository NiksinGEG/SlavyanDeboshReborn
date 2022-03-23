using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Assets.Map.WorldMap
{
    public class HexCell : MonoBehaviour
    {
        [SerializeField] public Vector3 coords;
        public EventHandler MouseLeftClick;
    }
}
