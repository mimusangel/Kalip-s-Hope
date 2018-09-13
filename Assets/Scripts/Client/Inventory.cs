using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

  /*  public Dictionary<int, Item> characterItems = new Dictionary<int, Item>();
    public ItemDatabase itemDatabase;
    public UIInventory inventoryUI;
    public int slotsNb = 24;

    //DEBUG
    public static Inventory instance = null;

    //DEBUG
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            inventoryUI.gameObject.SetActive(!inventoryUI.gameObject.activeSelf);
        if (Input.GetKeyDown(KeyCode.Keypad0))
            AddItem(0);
        if (Input.GetKeyDown(KeyCode.Keypad1))
            AddItem(1);
        if (Input.GetKeyDown(KeyCode.Keypad2))
            AddItem(2);
        if (Input.GetKeyDown(KeyCode.Keypad3))
            AddItem(3);
        if (Input.GetKeyDown(KeyCode.Keypad4))
            AddItem(4);
        if (Input.GetKeyDown(KeyCode.Keypad5))
            AddItem(5);
    }

    public void AddItem(Item item)
    {
        if (characterItems.Count < this.slotsNb)
        {
            characterItems.Add(item.id, item);
            inventoryUI.AddItem(item);
        }
    }

    public void AddItem(int id)
    {
        if (characterItems.Count >= this.slotsNb)
            return;
        Item item = ItemDatabase.instance.CreateNewItem(id, 0);
        if (item != null)
        {
            characterItems.Add(item.id, item);
            inventoryUI.AddItem(item);
            Debug.Log("Objet ajouté : " + item.title);
        }
    }

    public Item GetItem(int id)
    {
        Item item;
        characterItems.TryGetValue(id, out item);
        return (item);
    }

    public void RemoveItem(int id)
    {
        Item item = GetItem(id);
        if (item != null)
        {
            characterItems.Remove(item.id);
            inventoryUI.RemoveItem(item);
            Debug.Log("Objet retiré : " + item.title);
        }
    }*/
}
