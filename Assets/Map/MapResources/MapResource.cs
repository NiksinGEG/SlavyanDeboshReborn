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

    public void Select()
    {
        GetComponent<Selectable>().IsSelected = !GetComponent<Selectable>().IsSelected;
    }

    public void WhileSelected()
    {
        Material mat = gameObject.GetComponent<Renderer>().material;
        float o_width = mat.GetFloat("_Outline");
        if (o_width < 0.5f)
            o_width += 0.05f;
        mat.SetFloat("_Outline", o_width);
    }
    public void WhileDeselected()
    {
        Material mat = gameObject.GetComponent<Renderer>().material;
        float o_width = mat.GetFloat("_Outline");
        if (o_width > 0f)
            o_width -= 0.05f;
        mat.SetFloat("_Outline", o_width);
    }
}
