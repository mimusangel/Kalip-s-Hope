using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour {

    public static UIInventory instance {get; private set;}
    public Dictionary<int, UIItem> uiItems = new  Dictionary<int, UIItem>();
    public GameObject slotPrefab;
    public Transform slotPanel;
    private Inventory _inventory = null;

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
            uiItems.Add(i, slotInstance.GetComponentInChildren<UIItem>());
        }
    }

    public void Initialize(Inventory inventory)
	{
        _inventory = inventory;
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

    public void AddItem(Item item)
    {
        if (item.slot == -1)
            UpdateSlot(_inventory.FindAvaibleSlot(item), item);
        else
            UpdateSlot(item.slot, item);

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
