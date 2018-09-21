using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class Inventory {

    public Character character {get; private set;}
    public Dictionary<int, Item> slots = new Dictionary<int, Item>();//Key = item.slot
    public int size = 24;

    public Inventory(Character character)
    {
        this.character = character;
    }

    private void InitializeSlots()
    {
        slots.Clear();
        for (int i = 0; i < size; i++)
            slots.Add(i, null);
    }

    /*public void AddItem(Item item)
    {
        if (items.Count < this.size)
        {
            items.Add(item.id, item);
            UIInventory.instance.AddItem(item);
        }
    }*/

    public int Load()
	{
		InitializeSlots();
		IDataReader buffer = DBManager.Select("*", "Item", "character=" + character.index);
        int i = 0;
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
            i++;
        }
        return (i);
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

    /*public void AddItem(int id)
    {
        if (slots.Count >= this.slotsNb)
            return;
        Item item = ItemManager.CreateNewItem(id, );
        if (item != null)
        {
            slots.Add(item.id, item);
            UIInventory.instance.AddItem(item);
            Debug.Log("Objet ajouté : " + item.title);
        }
    }

    public Item GetItem(int id)
    {
        Item item;
        slots.TryGetValue(id, out item);
        return (item);
    }

    public void RemoveItem(int id)
    {
        Item item = GetItem(id);
        if (item != null)
        {
            slots.Remove(item.id);
            UIInventory.instance.RemoveItem(item);
            Debug.Log("Objet retiré : " + item.title);
        }
    }*/

    public void Write(Packet writer)
	{
		writer.Add(slots.Count);
		foreach(Item item in slots.Values)
		{
			item.Write(writer);
		}
	}

	public int Read(Packet reader)
	{
        int nb = 0;
		InitializeSlots();
		int count = reader.ReadInt();
		for (int i = 0; i < count; i++)
		{
			Item item = new Item(reader);
            slots[item.slot] = item;
            nb++;
		}
        return (nb);
	}
}
