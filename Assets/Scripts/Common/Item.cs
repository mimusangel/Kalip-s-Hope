﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {

    public int id;
    public int character = 0;
    public int template = 0;
    public string title;
    public string description;
    //public Sprite icon;
    public bool canStack;
    public int number;
    public int slot;

    public List<Effect> stats = new List<Effect>();

    //Template Constructor
    public Item(int id, string title, string description, List<Effect> stats, bool canStack)
    {
        this.id = id;
        this.character = 0;
        this.template = id;
        this.title = title;
        this.description = description;
       // this.icon = Resources.Load<Sprite>("Sprites/Items/" + this.template);
        this.stats = stats;
        this.canStack = canStack;
        this.number = 0;
        this.slot = -1;
    }

    //Item Constructors
    public Item(int id, int character, int template, string title, string description, List<Effect> stats, bool canStack)
    {
        this.id = id;
        this.character = character;
        this.template = template;
        this.title = title;
        this.description = description;
       // this.icon = Resources.Load<Sprite>("Sprites/Items/" + this.template);
        this.stats = stats;
        this.canStack = canStack;
        this.number = 1;
        this.slot = -1;
    }

    public Item(int id, int character, int template, string title, string description, List<Effect> stats, bool canStack, int number, int slot)
    {
        this.id = id;
        this.character = character;
        this.template = template;
        this.title = title;
        this.description = description;
       // this.icon = Resources.Load<Sprite>("Sprites/Items/" + this.template);
        this.stats = stats;
        this.canStack = canStack;
        this.number = number;
        this.slot = slot;
    }

    //Copy Constructor
    public Item(Item item)
    {
        this.id = item.id;
        this.character = item.character;
        this.template = item.template;
        this.title = item.title;
        this.description = item.description;
        //this.icon = Resources.Load<Sprite>("Sprites/Items/" + this.template);
        this.stats = new List<Effect>();
        foreach (Effect stat in item.stats)
            this.stats.Add(new Effect(stat));
        this.canStack = item.canStack;
        this.number = item.number;
        this.slot = item.slot;
    }

    public Item(Packet reader)
    {
        id = reader.ReadInt();
        character = reader.ReadInt();
        template = reader.ReadInt();
        title = reader.ReadString();
        description = reader.ReadString();
        canStack = reader.ReadBool();
        number = reader.ReadInt();
        stats = Converter.ParseEffects(reader.ReadString());
        slot = reader.ReadInt();
    }

    public void Write(Packet writer)
    {
        writer.Add(id);
        writer.Add(character);
        writer.Add(template);
        writer.Add(title);
        writer.Add(description);
        writer.Add(canStack);
        writer.Add(number);
        writer.Add(Converter.FormatEffects(stats));
        writer.Add(slot);
    }
}
