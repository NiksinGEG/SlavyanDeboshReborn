using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Map.WorldMap;

namespace Assets.Scripts.Path
{
    public class PathNode
    {
        // Исследуемая клетка
        public HexCell cell { get; set; }
        // Длина пути от старта (G).
        public int PathLengthFromStart { get; set; }
        // Точка, из которой пришли в эту точку.
        public PathNode CameFrom { get; set; }
        // Примерное расстояние до цели (H).
        public int HeuristicEstimatePathLength { get; set; }
        // Ожидаемое полное расстояние до цели (F).
        public int EstimateFullPathLength
        {
            get
            {
                return this.PathLengthFromStart + this.HeuristicEstimatePathLength;
            }
        }
    }

    public class A_Star
    {
        private static int GetHeuristicPathLength(Vector3 from, Vector3 to) { return Convert.ToInt32(Math.Abs(from.x - to.x) + Math.Abs(from.z - to.z)); }

        private static int GetDistanceBetweenNeighbours() { return 1; }

        private static List<HexCell> GetPathForNode(PathNode pathNode)
        {
            var result = new List<HexCell>();
            var currentNode = pathNode;
            while (currentNode != null)
            {
                result.Add(currentNode.cell);
                currentNode = currentNode.CameFrom;
            }
            result.Reverse();
            return result;
        }

        private static List<PathNode> CreatePathNodeList(int type, PathNode currentNode, HexCell endCell)
        {
            List<PathNode> pathNode = new List<PathNode>();
            PathNode tmp;
            foreach(var nCell in currentNode.cell.neighbours)
            {
                switch(type)
                {
                    case 0:
                        if(nCell.Type == CellType.water)
                        {
                            tmp = new PathNode()
                            {
                                cell = nCell,
                                CameFrom = currentNode,
                                PathLengthFromStart = currentNode.PathLengthFromStart + GetDistanceBetweenNeighbours(),
                                HeuristicEstimatePathLength = GetHeuristicPathLength(nCell.transform.position, endCell.transform.position)
                            };
                            pathNode.Add(tmp);
                        }
                        break;
                    case 1:
                        if (nCell.Type != CellType.water)
                        {
                            tmp = new PathNode()
                            {
                                cell = nCell,
                                CameFrom = currentNode,
                                PathLengthFromStart = currentNode.PathLengthFromStart + GetDistanceBetweenNeighbours(),
                                HeuristicEstimatePathLength = GetHeuristicPathLength(nCell.transform.position, endCell.transform.position)
                            };
                            pathNode.Add(tmp);
                        }
                        break;
                    default:
                        tmp = new PathNode()
                        {
                            cell = nCell,
                            CameFrom = currentNode,
                            PathLengthFromStart = currentNode.PathLengthFromStart + GetDistanceBetweenNeighbours(),
                            HeuristicEstimatePathLength = GetHeuristicPathLength(nCell.transform.position, endCell.transform.position)
                        };
                        pathNode.Add(tmp);
                        break;
                }

            }
            return pathNode;
        }

        public static List<HexCell> FindPath(int type, HexCell startCell, HexCell endCell)
        {
            var closedSet = new List<PathNode>(); 
            var openSet = new List<PathNode>();

            PathNode startNode = new PathNode()
            {
                cell = startCell,
                CameFrom = null,
                PathLengthFromStart = 0,
                HeuristicEstimatePathLength =  GetHeuristicPathLength(startCell.transform.position, endCell.transform.position)
            };
            openSet.Add(startNode);
            while(openSet.Count > 0)
            {
                var currentNode = openSet.OrderBy(node => node.EstimateFullPathLength).First();
                if (currentNode.cell.transform.position == endCell.transform.position)
                    return GetPathForNode(currentNode);

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                foreach (var neighbourNode in CreatePathNodeList(type, currentNode, endCell))
                {
                    if (closedSet.Count(node => node.cell.transform.position == neighbourNode.cell.transform.position) > 0)
                        continue;
                    var openNode = openSet.FirstOrDefault(node => node.cell.transform.position == neighbourNode.cell.transform.position);
                    if (openNode == null)
                        openSet.Add(neighbourNode);
                    else
                    {
                        if(openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
                        {
                            openNode.CameFrom = currentNode;
                            openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
                        }
                    }

                }
            }
            return null;
        }

    }
}
