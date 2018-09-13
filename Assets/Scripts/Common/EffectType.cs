using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class EffectType
{
    //CONSTANTES
    public const int DefaultCategory = 0;
    public const int DamageCategory = 1;
    public const int HealCategory = 2;

    public int id;
    public string name;
    public string color;
    public int category;

    public EffectType()
    {
        this.id = 0;
        this.name = "";
        this.color = "";
        this.category = 0;
    }

    public EffectType(int id, string name, string color, int category)
    {
        this.id = id;
        this.name = name;
        this.color = color;
        this.category = category;
    }

    public EffectType(Packet reader)
    {
        this.id = reader.ReadInt();
        Debug.Log("id: " + id);
        this.name = reader.ReadString();
        Debug.Log("name: " + name);
        this.color = reader.ReadString();
        Debug.Log("color: " + color);
        this.category = reader.ReadInt();
        Debug.Log("category: " + category);
    }

    public void Write(Packet writer)
    {
        writer.Add(id);
        writer.Add(name);
        writer.Add(color);
        writer.Add(category);
    }
}