using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamScript : MonoBehaviour
{
    [SerializeField]
    public float delta = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = gameObject.transform.position;
        Vector3 newPos = pos;

        if (Input.GetKey(KeyCode.D))
            newPos.x += delta;
        if (Input.GetKey(KeyCode.A))
            newPos.x -= delta;
        if (Input.GetKey(KeyCode.S))
            newPos.z -= delta;
        if (Input.GetKey(KeyCode.W))
            newPos.z += delta;
        if (Input.GetKey(KeyCode.Q))
        {
            Vector3 ax = new Vector3(1, 0, 0);
            gameObject.transform.Rotate(ax, 0.1f);
        }
        if (Input.GetKey(KeyCode.E))
        {
            Vector3 ax = new Vector3(1, 0, 0);
            gameObject.transform.Rotate(ax, -0.1f);
        }
        if (Input.GetKey(KeyCode.Space))
            newPos.y += delta;
        if (Input.GetKey(KeyCode.LeftControl))
            newPos.y -= delta;
        
        gameObject.transform.position = newPos;
    }
}
