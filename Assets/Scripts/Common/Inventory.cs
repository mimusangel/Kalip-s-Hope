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
