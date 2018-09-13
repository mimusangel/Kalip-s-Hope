using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {

    public int id;
    public int character = 0;
    public int template = 0;
    public string title;
    public string description;
    //public Sprite icon;
    public int maxNumber;
    public int number;

    public List<Effect> stats = new List<Effect>();

    //Template Constructor
    public Item(int id, string title, string description, List<Effect> stats, int maxNumber)
    {
        this.id = id;
        this.character = 0;
        this.template = id;
        this.title = title;
        this.description = description;
       // this.icon = Resources.Load<Sprite>("Sprites/Items/" + this.template);
        this.stats = stats;
        this.maxNumber = maxNumber;
        this.number = 0;
    }

    //Item Constructors
    public Item(int id, int character, int template, string title, string description, List<Effect> stats, int maxNumber)
    {
        this.id = id;
        this.character = character;
        this.template = template;
        this.title = title;
        this.description = description;
       // this.icon = Resources.Load<Sprite>("Sprites/Items/" + this.template);
        this.stats = stats;
        this.maxNumber = maxNumber;
        this.number = 1;
    }

    public Item(int id, int character, int template, string title, string description, List<Effect> stats, int maxNumber, int number)
    {
        this.id = id;
        this.character = character;
        this.template = template;
        this.title = title;
        this.description = description;
       // this.icon = Resources.Load<Sprite>("Sprites/Items/" + this.template);
        this.stats = stats;
        this.maxNumber = maxNumber;
        this.number = number;
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
        this.maxNumber = item.maxNumber;
        this.number = item.number;
    }

    public Item(Packet reader)
    {
        id = reader.ReadInt();
        character = reader.ReadInt();
        template = reader.ReadInt();
        title = reader.ReadString();
        description = reader.ReadString();
        maxNumber = reader.ReadInt();
        number = reader.ReadInt();
        stats = Converter.ParseEffects(reader.ReadString());
    }

    public void Write(Packet writer)
    {
        writer.Add(id);
        writer.Add(character);
        writer.Add(template);
        writer.Add(title);
        writer.Add(description);
        writer.Add(maxNumber);
        writer.Add(number);
        writer.Add(Converter.FormatEffects(stats));
    }
}
