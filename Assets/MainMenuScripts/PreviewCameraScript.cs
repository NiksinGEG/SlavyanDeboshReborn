using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Map;

public class PreviewCameraScript : MonoBehaviour
{
    public MainMap map;
    public Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var centerPos = map.grid.cellList[(map.grid.cellList.Length) / 2].transform.position;
        camera.transform.position = centerPos;
        Vector3 tmp = camera.transform.localPosition;
        tmp.z = -1158f;
        camera.transform.localPosition = tmp;
        camera.fieldOfView = map.grid.chunkCountZ * 2.5f;
        if (map.grid.chunkCountX == map.grid.chunkCountZ)
        {
            Rect temp = camera.rect;
            temp.x = 0.3f;
            camera.rect = temp;
        }

        Debug.Log($"Map pos - {map.transform.position} \n" +
                  $"HexGrid pos & local pos - {map.grid.transform.position}; {map.grid.transform.localPosition} \n" +
                  $"Camera pos & local pos - {camera.transform.position}; {camera.transform.localPosition}");
        //transform.localPosition = map.grid.transform.localPosition;
    }
}
