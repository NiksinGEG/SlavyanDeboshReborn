using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapResource : MonoBehaviour
{
    Vector3 centerPos;

    /// <summary>
    /// Устанавливает позицию ресурса внутри гекса. x и z - вещественные числа от -1 до 1
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void SetInnerPosition(float x, float z)
    {
        Vector3 pos = transform.position;
        pos.x += x * HexMetrics.innerRadius * HexMetrics.solidFactor;
        pos.z += z * HexMetrics.innerRadius * HexMetrics.solidFactor;
        transform.position = pos;
    }
    private void Awake()
    {
        centerPos = transform.position;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
