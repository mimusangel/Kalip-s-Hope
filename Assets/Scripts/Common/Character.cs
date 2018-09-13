using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class Character
{
	public int index {get; private set;}
	public string name {get; private set;}
	public int level {get; private set;}
	public int exp {get; private set;}
	public string skillTree {get; private set;}
	public int life {get; private set;}

	public Dictionary<int, Item> inventory = new Dictionary<int, Item>();

	public Character()
	{}

	public void Read(IDataReader reader)
	{
		index = reader.GetInt32(0);
		name = reader.GetString(1);
		level = reader.GetInt32(2);
		exp = reader.GetInt32(3);
		skillTree = reader.IsDBNull(4) ? "" : reader.GetString(4);
		life = reader.GetInt32(5);
	}

	public void Read(Packet reader)
	{
		index = reader.ReadInt();
		name = reader.ReadString();
		level = reader.ReadInt();
		exp = reader.ReadInt();
		skillTree = reader.ReadString();
		life = reader.ReadInt();
	}

	public void Write(Packet writer)
	{
		writer.Add(index);
		writer.Add(name);
		writer.Add(level);
		writer.Add(exp);
		writer.Add(skillTree);
		writer.Add(life);
	}

	public void LoadInventory()
	{
		inventory.Clear();
		IDataReader buffer = DBManager.Select("*", "Item", "character=" + index);
        while (buffer.Read())
        {
            int id = buffer.GetInt32(0);
            Item template = GameManager.instance.GetItemTemplate(buffer.GetInt32(2));
            string title = template.title;
            string description = template.description;
            string stats = buffer.IsDBNull(3) ? null : buffer.GetString(3);
            int maxnumber = template.maxNumber;
            int number = buffer.GetInt32(4);
            inventory.Add(id, new Item(id, index, template.id, title, description, Converter.ParseEffects(stats), maxnumber, number));
        }
	}

	public void WriteInventory(Packet writer)
	{
		writer.Add(inventory.Count);
		foreach(Item item in inventory.Values)
		{
			item.Write(writer);
		}
	}

	public void ReadInventory(Packet reader)
	{
		inventory.Clear();
		int count = reader.ReadInt();
		for (int i = 0; i < count; i++)
		{
			Item item = new Item(reader);
			inventory.Add(item.id, item);
		}
	}
}