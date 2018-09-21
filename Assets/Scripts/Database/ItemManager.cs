using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public static class ItemManager {

    public static Item CreateNewItem(int id, Character character)
    {
        Item template;
        Item item = null;
        int slot; 

        GameManager.instance.itemTemplates.TryGetValue(id, out template);
        if (template != null)
        { 
            slot = character.inventory.FindAvaibleSlot(template);
            if (slot == -1)
                return (null);

            if (character.inventory.slots[slot] == null)
            {
                item = new Item(template);
                item.character = character.index;
                item.template = id;
                item.slot = slot;
                item.number = 1;
                GenerateStats(ref item.stats);
                string formatedStats = Converter.FormatEffects(item.stats);
                string values = item.character + ", " + item.template + ", '" + formatedStats + "', " + item.number + ", " + slot;
                DBManager.Insert("Item", "character, template, stats, number, slot", values);
                item.id = DBManager.LastInsertID("Item");
                character.inventory.slots[slot] = item;
                return (item);
            }
            else
            {
                character.inventory.slots[slot].number++;
                DBManager.Update("Item", "number = " + character.inventory.slots[slot].number, "slot = " + slot);
                return (character.inventory.slots[slot]);
            }
        }
        return (null);
    }

    private static void GenerateStats(ref List<Effect> stats)
    {
        foreach (Effect stat in stats)
        {
            if (stat.maxValue > stat.minValue && stat.type.category != EffectType.DamageCategory)
            {
                float min = stat.minValue, max = stat.maxValue + 1;
                stat.minValue = (int)UnityEngine.Random.Range(min, max);
                stat.maxValue = stat.minValue - 1;
            }
        }
    }


    //Sauvegarde tous les Items depuis la mémoire vers la DB
    // public static void SaveItems()
    // {
    //     int i = 0;

    //     DBManager.Prepare();
    //     foreach (Dictionary<int, Item> items in charactersItems.Values)
    //     {
    //         foreach (Item item in items.Values)
    //         {
    //             string formatedStats = Converter.FormatEffects(item.stats);
    //             string insertValues = item.id + ", " + item.character + ", " + item.template + ", '" + formatedStats + "'";
    //             DBManager.Replace("Item", "id, character, template, stats", insertValues);
    //             i++;
    //         }
    //     }
    //     DBManager.Commit();
    //     Debug.Log(i + " Item sauvegardés.");
    // }

    //Sauvegarde tous les Items d'un personnage vers la DB
    // public static void SaveItems(int character)
    // {
    //     Dictionary<int, Item> items;

    //     charactersItems.TryGetValue(character, out items);
    //     DBManager.Prepare();
    //     foreach (Item item in items.Values)
    //     {
    //         string formatedStats = Converter.FormatEffects(item.stats);
    //         string insertValues = item.id + ", " + item.character + ", " + item.template + ", '" + formatedStats + "'";
    //         DBManager.Replace("Item", "id, character, template, stats", insertValues);
    //     }
    //     DBManager.Commit();
    //     Debug.Log(items.Count + " Item sauvegardés.");
    // }
}
