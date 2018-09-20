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

        //TODO GET SLOT
        GameManager.instance.itemTemplates.TryGetValue(id, out template);
        if (template != null)
        {
            item = new Item(template);
            item.character = character.index;
            item.template = id;
            item.slot = 0; // TODO GET SLOT
            GenerateStats(ref item.stats);
            string formatedStats = Converter.FormatEffects(item.stats);
            string values = item.character + ", " + item.template + ", '" + formatedStats + "'";
            DBManager.Insert("Item", "character, template, stats", values);
            item.id = DBManager.LastInsertID("Item");

            // Dictionary<int, Item> characterItems;
            // charactersItems.TryGetValue(character, out characterItems);
            // characterItems.Add(item.id, item);
        }
        return (item);
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
