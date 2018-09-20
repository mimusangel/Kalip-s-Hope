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

	public Inventory inventory;

	public Character()
	{
		inventory = new Inventory(this);
	}

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
}