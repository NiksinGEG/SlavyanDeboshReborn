using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamScript : MonoBehaviour
{
    [SerializeField]
    public float delta = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = gameObject.transform.position;
        Quaternion q = gameObject.transform.rotation;
        //float delta = 1f;
        if (Input.GetKey(KeyCode.D))
        {
            Vector3 newPos = new Vector3(pos.x + delta, pos.y, pos.z);
            gameObject.transform.position = newPos;
        }
        if(Input.GetKey(KeyCode.A))
        {
            Vector3 newPos = new Vector3(pos.x - delta, pos.y, pos.z);
            gameObject.transform.position = newPos;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 newPos = new Vector3(pos.x, pos.y, pos.z - delta);
            gameObject.transform.position = newPos;
        }
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 newPos = new Vector3(pos.x, pos.y, pos.z + delta);
            gameObject.transform.position = newPos;
        }
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
        {
            Vector3 newPos = new Vector3(pos.x, pos.y + delta, pos.z);
            gameObject.transform.position = newPos;
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Vector3 newPos = new Vector3(pos.x, pos.y - delta, pos.z);
            gameObject.transform.position = newPos;
        }
    }
}
