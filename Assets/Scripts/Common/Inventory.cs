using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class Inventory {

    public Character character {get; private set;}
    public Dictionary<int, Item> slots = new Dictionary<int, Item>();//Key = item.slot
    public int size = 24;
    public int itemNb = 0;

    /*
	 * ************************** *
	 * ***     BOTH SIDES     *** *
	 * ************************** *
	*/

    public Inventory(Character character)
    {
        this.character = character;
    }

    private void InitializeSlots()
    {
        itemNb = 0;
        slots.Clear();
        for (int i = 0; i < size; i++)
            slots.Add(i, null);
    }

    public int FindAvaibleSlot(Item item)
	{
        int freeSlot = -1;
        Item slot;

        for (int i = 0; i < size; i++)
        {
            slots.TryGetValue(i, out slot);
            if (slot == null)
            {
                if (!item.canStack)//Si l'item ne se stack pas, on retourne l'index du slot libre
                    return (i);
                if (freeSlot == -1)//Sinon on l'enregistre
                    freeSlot = i;
            }
            else if (item.canStack && slot.template == item.template)//Si l'item peut se stack et que le slot correspond, on retourne l'index du slot
                return (i);
        }
        return (freeSlot);//Aucun slot libre: freeSlot = -1; Si l'item se stack et qu'on a pas trouvé d'item correspondant: freeSlot = premier slot libre
	}

    public Item FindItemById(int id)
    {
        foreach(Item item in slots.Values)
		{
            if (item != null && item.id == id)
			    return (item);
		}
        return (null);
    }

    public Item FindItemByTemplate(int templateID)
    {
        foreach(Item item in slots.Values)
		{
            if (item != null && item.template == templateID)
			    return (item);
		}
        return (null);
    }

    public List<Item> FindItemsByTemplate(int templateID)
    {
        List<Item> items = new List<Item>();

        foreach(Item item in slots.Values)
		{
            if (item != null && item.template == templateID)
			    items.Add(item);
		}
        return (items);
    }

    public Item FindItemBySlot(int slotID)
    {
        if (slotID >= 0 && slotID < size)
            return (slots[slotID]);
        return (null);
    }

    public Item FindItem(int id, int type = 0)
    {
        switch (type)
        {
            case 1:
                return (FindItemByTemplate(id));

            case 2:
                return (FindItemBySlot(id));

            default:
                return (FindItemById(id));
        }
    }

    public bool IsValidSlot(int slot)
    {
        return (slot >= 0 && slot < size);
    }

    public void SwapItem(int slot1, int slot2)
    {
        if (IsValidSlot(slot1) && IsValidSlot(slot2))
        {
            Item tmp = slots[slot1];
            if (slots[slot1] != null)
                slots[slot1].slot = slot2;
            if (slots[slot2] != null)
                slots[slot2].slot = slot1;
            slots[slot1] = slots[slot2];
            slots[slot2] = tmp;
        }
    }

    /*
	 * ************************** *
	 * ***    SERVER SIDE     *** *
	 * ************************** *
	*/

    public int Load()
	{
		InitializeSlots();
		IDataReader buffer = DBManager.Select("*", "Item", "character=" + character.index);
        while (buffer.Read())
        {
            int id = buffer.GetInt32(0);
            Item template = GameManager.instance.GetItemTemplate(buffer.GetInt32(2));
            string title = template.title;
            string description = template.description;
            string stats = buffer.IsDBNull(3) ? null : buffer.GetString(3);
            bool canStack = template.canStack;
            int number = buffer.GetInt32(4);
			int slot = buffer.GetInt32(5);
            slots[slot] = new Item(id, character.index, template.id, title, description, Converter.ParseEffects(stats), canStack, number, slot);
            itemNb++;
        }
        return (itemNb);
	}

    /*
	 * ************************** *
	 * ***    CLIENT SIDE     *** *
	 * ************************** *
	*/

    public Item AddItem(Packet reader)
    {
        Item item = new Item(reader);
        if (slots[item.slot] == null)
            itemNb++;
        slots[item.slot] = item;
        return (item);
    }

    public bool RemoveItem(int slot, int quantity)
    {
        Item item;

        if (slot >= 0 && slot < size && slots[slot] != null)
        {
            item = slots[slot];
            if (item.number - quantity <= 0)
            {
                slots[slot] = null;
                item = null;
                itemNb--;
            }
            else
            {
                item.number -= quantity;
            }
            return (true);
        }
        return (false);
    }

    /*
	 * ************************** *
	 * ***      PACKETS       *** *
	 * ************************** *
	*/

    public void Write(Packet writer)
	{
		writer.Add(itemNb);
		foreach(Item item in slots.Values)
		{
            if (item != null)
			    item.Write(writer);
		}
	}

	public int Read(Packet reader)
	{
		InitializeSlots();
		int count = reader.ReadInt();
		for (int i = 0; i < count; i++)
		{
			Item item = new Item(reader);
            slots[item.slot] = item;
            itemNb++;
		}
        return (itemNb);
	}
}
