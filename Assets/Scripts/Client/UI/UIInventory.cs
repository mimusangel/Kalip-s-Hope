using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour {

    public static UIInventory instance {get; private set;}
    public List<UIItem> uiItems = new List<UIItem>();
    public GameObject slotPrefab;
    public Transform slotPanel;
    public int slotNb = 24;

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < slotNb; i++)
        {
            GameObject slotInstance = Instantiate(slotPrefab, Vector3.zero, Quaternion.identity, slotPanel);
           // slotInstance.transform.SetParent(slotPanel);
            uiItems.Add(slotInstance.GetComponentInChildren<UIItem>());
        }
    }

    public void UpdateSlot(int slot, Item item)
    {
        uiItems[slot].UpdateItem(item);
    }

    public void AddItem(Item item)
    {
        if (item.slot == -1)
            UpdateSlot(uiItems.FindIndex(i => i.item == null), item);
        else
            UpdateSlot(item.slot, item);

    }

    public int FindSlot(Item item)
    {
        return (uiItems.FindIndex(i => i.item == null));
    }

    public void RemoveItem(Item item)
    {
        UpdateSlot(uiItems.FindIndex(i => i.item == item), null);
    }
}
