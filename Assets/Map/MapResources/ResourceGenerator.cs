using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using Assets.Map.WorldMap;

namespace Assets.Map.MapResources
{
    public class ResourceGenerator : MonoBehaviour
    {
        [SerializeField] Transform rockPrefab_1;
        [SerializeField] Transform rockPrefab_2;
        [SerializeField] Transform rockPrefab_3;
        [SerializeField] Transform rockPrefab_4;
        [SerializeField] Transform rockPrefab_5;

        Transform choosenPrefab;

        public Transform ChoosenPrefab
        {
            get { return choosenPrefab; }
            set { choosenPrefab = value; }
        }

        public Transform ChooseRockPrefab(System.Random rndSeed)
        {
            int num = rndSeed.Next(1, 5);
            return num switch
            {
                1 => rockPrefab_1,
                2 => rockPrefab_2,
                3 => rockPrefab_3,
                4 => rockPrefab_4,
                5 => rockPrefab_5,
                _ => rockPrefab_1,
            };
        }

       

        public void GenerateResource(HexGrid grid,MapResource resource, System.Random rndSeed)
        {
            
        }
    }
}
