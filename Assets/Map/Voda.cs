using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voda : MonoBehaviour
{
    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        Vector3 new_pos = transform.position;
        new_pos.y = HexMetrics.elevationStep * -0.5f;
        transform.position = new_pos;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
