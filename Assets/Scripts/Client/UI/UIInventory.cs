using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour {

    public static UIInventory instance {get; private set;}
    public Dictionary<int, UIItem> uiItems = new  Dictionary<int, UIItem>();
    public GameObject slotPrefab;
    public Transform slotPanel;
    public Inventory inventory = null;
    public int pointedSlot = -1;

    private void Awake()
    {
        instance = this;
    }

    private void InitializeSlots(int size)
    {
        uiItems.Clear();
        for (int i = 0; i < size; i++)
        {
            GameObject slotInstance = Instantiate(slotPrefab, Vector3.zero, Quaternion.identity, slotPanel);
            UIItem uiitem = slotInstance.GetComponentInChildren<UIItem>();
            uiitem.slot = i;
            uiItems.Add(i, uiitem);
        }
    }

    public void Initialize(Inventory inv)
	{
        inventory = inv;
		InitializeSlots(inventory.size);
        foreach(Item item in inventory.slots.Values)
        {
            if (item != null)
                UIInventory.instance.AddItem(item);
        }
	}

    public void UpdateSlot(int slot, Item item)
    {
        uiItems[slot].UpdateItem(item);
    }

    public void UpdateSlot(int slot)
    {
        uiItems[slot].UpdateItem(inventory.slots[slot]);
    }

    public void AddItem(Item item)
    {
        if (item.slot == -1)
            UpdateSlot(inventory.FindAvaibleSlot(item), item);
        else
            UpdateSlot(item.slot, item);

    }

    public UIItem PointedSlot()
    {
        if (pointedSlot != -1)
            return (uiItems[pointedSlot]);
        return (null);
    }

    // public int FindSlot(Item item)
    // {
    //     return (uiItems.FindIndex(i => i.item == null));
    // }

    // public void RemoveItem(Item item)
    // {
    //     UpdateSlot(uiItems.FindIndex(i => i.item == item), null);
    // }
}
