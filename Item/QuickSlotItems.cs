using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotItems : MonoBehaviour
{
    static public QuickSlotItems instance;
    QuickSlotBaseItem baseItem;
    public QuickSlotBaseItem[] baseItems = new QuickSlotBaseItem[5];
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        baseItem = transform.Find("ItemBase").GetComponent<QuickSlotBaseItem>();

        for (int i = 0; i < baseItems.Length; i++)
        {
            var newItem = Instantiate(baseItem, baseItem.transform.parent);
            newItem.Init();
            newItem.quickSlotItemNumber.text = (i+1).ToString();
            baseItems[i] = newItem;
        }

        baseItem.gameObject.SetActive(false);
    }
}
