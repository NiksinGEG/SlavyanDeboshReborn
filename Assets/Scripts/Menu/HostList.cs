using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostList : MonoBehaviour
{
    [SerializeField] public HostItem itemPrefab;
    public List<HostItem> items = new List<HostItem>();

    public void Add(string address, string name)
    {
        var item = Instantiate(itemPrefab);
        item.transform.SetParent(transform);
        item.HostAddressText.text = address;
        item.HostNameText.text = name;
        items.Add(item);
    }

    public bool HasHost(string addressOrName)
    {
        foreach (var item in items)
            if (item.HostAddressText.text == addressOrName || item.HostNameText.text == addressOrName)
                return true;
        return false;
    }
}
