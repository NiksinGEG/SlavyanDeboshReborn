using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Map.WorldMap;
using Assets.Scripts.Path;

namespace Assets.Scripts.Statics
{
    class Map
    {
        private static Map instance;
        public static HexGrid mapReference;

        Map()
        {
            mapReference = MonoBehaviour.FindObjectOfType<HexGrid>();
        }

		public static List<HexCell> GetWay(int type, HexCell startCell, HexCell endCell)
		{
			switch (type)
			{
				case 0:
					if (endCell.Type != CellType.water)
					{
						return null;
					}
					break;
				case 1:
					if (endCell.Type == CellType.water)
					{
						return null;
					}
					break;
			}
			List<HexCell> path = A_Star.FindPath(type, startCell, endCell);
			return path;
		}

		public static HexCell GetByPosition(Vector3 pos)
        {
            int ind = HexCoords.FromPosition(pos).MakeIndex(mapReference.cellCountX);
            return mapReference.cellList[ind];
        }

        public static Map getInstance()
        {
            if (instance == null)
                instance = new Map();
            return instance;
        }
    }
}
