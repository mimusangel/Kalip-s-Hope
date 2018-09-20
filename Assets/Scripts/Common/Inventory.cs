using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class Inventory {

    public Character character {get; private set;}
    public Dictionary<int, Item> items = new Dictionary<int, Item>();
    public int slotsNb = 24;

    public Inventory(Character character)
    {
        this.character = character;
    }

    public void AddItem(Item item)
    {
        if (items.Count < this.slotsNb)
        {
            items.Add(item.id, item);
            UIInventory.instance.AddItem(item);
        }
    }

    public void Load()
	{
		items.Clear();
		IDataReader buffer = DBManager.Select("*", "Item", "character=" + character.index);
        while (buffer.Read())
        {
            int id = buffer.GetInt32(0);
            Item template = GameManager.instance.GetItemTemplate(buffer.GetInt32(2));
            string title = template.title;
            string description = template.description;
            string stats = buffer.IsDBNull(3) ? null : buffer.GetString(3);
            int maxnumber = template.maxNumber;
            int number = buffer.GetInt32(4);
			int slot = buffer.GetInt32(5);
            items.Add(id, new Item(id, character.index, template.id, title, description, Converter.ParseEffects(stats), maxnumber, number, slot));
        }
	}

    public int FindAvaibleSlot(Item item)
	{
		if (items.Count < this.slotsNb)
        {
            // TODO
        }
            return (UIInventory.instance.FindSlot(item));
        return (-1);
	}

    /*public void AddItem(int id)
    {
        if (items.Count >= this.slotsNb)
            return;
        Item item = ItemManager.CreateNewItem(id, );
        if (item != null)
        {
            items.Add(item.id, item);
            UIInventory.instance.AddItem(item);
            Debug.Log("Objet ajouté : " + item.title);
        }
    }

    public Item GetItem(int id)
    {
        Item item;
        items.TryGetValue(id, out item);
        return (item);
    }

    public void RemoveItem(int id)
    {
        Item item = GetItem(id);
        if (item != null)
        {
            items.Remove(item.id);
            UIInventory.instance.RemoveItem(item);
            Debug.Log("Objet retiré : " + item.title);
        }
    }*/

    public void Write(Packet writer)
	{
		writer.Add(items.Count);
		foreach(Item item in items.Values)
		{
			item.Write(writer);
		}
	}

	public void Read(Packet reader)
	{
		items.Clear();
		int count = reader.ReadInt();
		for (int i = 0; i < count; i++)
		{
			Item item = new Item(reader);
			items.Add(item.id, item);
		}
	}
}
