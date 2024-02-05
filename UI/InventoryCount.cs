using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryCount : MonoBehaviour
{
    TextMeshProUGUI inventoryCount;
    private void OnEnable()
    {
        inventoryCount = transform.Find("InventoryCount").GetComponent<TextMeshProUGUI>();
        inventoryCount.text = $"{UserItemPanel.instance.baseItemList.Count}/{UserItemPanel.instance.baseItemList.Capacity}";
    }
    void Update()
    {
        inventoryCount.text = $"{UserItemPanel.instance.baseItemList.Count}/{UserItemPanel.instance.baseItemList.Capacity}";
    }
}
