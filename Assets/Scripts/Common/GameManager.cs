﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;

public class GameManager : MonoBehaviour {
	public enum Side {None, Client, Server};
	public static GameManager instance  {get; private set;}
	public Side side {get; private set;}
	void Awake()
	{
		Debug.developerConsoleVisible = true;
		if (instance == null)
		{
			instance = this;
			side = Side.None;
			DontDestroyOnLoad(gameObject);
		}
	}

	// DATA In Game (Character connecter au jeu)
	public Dictionary<int, Character> characters = new Dictionary<int, Character>();

	// DATA Du JEU
	public List<Sprite[]> bodys = new List<Sprite[]>();
	public List<Sprite[]> eyes = new List<Sprite[]>();
	public List<Sprite[]> frontHairs = new List<Sprite[]>();
	public List<Sprite[]> rearHairs = new List<Sprite[]>();
	public Dictionary<int, Item> itemTemplates = new Dictionary<int, Item>();
	public Dictionary<int, EffectType> effectTypes = new Dictionary<int, EffectType>();

	void Start()
	{
		for (int i = 1; i <= 2; i++)
		{
			Sprite[] list = Resources.LoadAll<Sprite>("Sprites/Body/walk/Body (" + i + ")");
			if (list.Length > 0)
				bodys.Add(list);
		}
		Debug.Log(bodys.Count + " bodys loaded!");

		for (int i = 0; i <= 18; i++)
		{
			Sprite[] list = Resources.LoadAll<Sprite>("Sprites/Eyes/walk/Eyes (" + i + ")");
			if (list.Length > 0)
				eyes.Add(list);
		}
		Debug.Log(eyes.Count + " eyes loaded!");

		for (int i = 0; i <= 15; i++)
		{
			Sprite[] list1 = Resources.LoadAll<Sprite>("Sprites/Front Hair Part1/walk/Front Hair (" + i + ")");
			Sprite[] list2 = Resources.LoadAll<Sprite>("Sprites/Front Hair Part2/walk/Front Hair (" + i + ")");
			if (list1.Length > 0)
				frontHairs.Add(JoinSpriteArray(list1, list2));
		}
		Debug.Log(frontHairs.Count + " front hair loaded!");
		
		for (int i = 0; i <= 20; i++)
		{
			Sprite[] list1 = Resources.LoadAll<Sprite>("Sprites/Rear Hair Part2/walk/Rear Hair (" + i + ")");
			Sprite[] list2 = Resources.LoadAll<Sprite>("Sprites/Rear Hair Part1/walk/Rear Hair (" + i + ")");
			if (list1.Length > 0)
				rearHairs.Add(JoinSpriteArray(list1, list2));
		}
		Debug.Log(rearHairs.Count + " rear hair loaded!");
	}

	Sprite[]	JoinSpriteArray(Sprite[] list1, Sprite[] list2)
	{
		Sprite[] list = new Sprite[list1.Length + list2.Length];
		int j = 0;
		for (int i = 0; i < list1.Length; i++)
		{
			list[j] = list1[i];
			j++;
		}
		for (int i = 0; i < list2.Length; i++)
		{
			list[j] = list2[i];
			j++;
		}
		return list;
	}

	public Item GetItemTemplate(int id)
    {
        Item item;
        itemTemplates.TryGetValue(id, out item);
        return (item);
    }

    public EffectType GetEffect(int id)
    {
        EffectType effectType;
        effectTypes.TryGetValue(id, out effectType);
        return (effectType);
    }

	void OnDestroy()
	{
		DBManager.CloseDataBase();
	}

	private void OnGUI() {
		if (side == Side.None)
		{
			if (GUI.Button(new Rect(10, 10, 200, 30), "Start Server"))
			{
				side = Side.Server;
				ServerSocketScript sss = gameObject.AddComponent<ServerSocketScript>();
				sss.Run();
			}
			if (GUI.Button(new Rect(10, 50, 200, 30), "Start Client"))
			{
				side = Side.Client;
				ClientSocketScript css = gameObject.AddComponent<ClientSocketScript>();
				css.Run();
			}
		}
		if (side == Side.Server)
			OnServerGUI();
	}

	private Vector2 scrollPosition = Vector2.zero;
	private string textCMD = "";

	private void OnServerGUI() {
		int ScreenW = Screen.width;
		int ScreenH = Screen.height;
		ServerSocketScript sss = gameObject.GetComponent<ServerSocketScript>();

		if (GUI.Button(new Rect(ScreenW - 210, 10, 200, 30), "Button"))
		{

		}
		if (GUI.Button(new Rect(ScreenW - 210, 50, 200, 30), "Button"))
		{

		}
		if (GUI.Button(new Rect(ScreenW - 210, 90, 200, 30), "Button"))
		{

		}
		if (GUI.Button(new Rect(ScreenW - 210, 130, 200, 30), "Button"))
		{

		}

		GUIStyle style = GUI.skin.box;
        style.alignment = TextAnchor.UpperLeft;
		style.fixedWidth = ScreenW - 245;
		style.wordWrap = true;

		float h = 0;
		int count = sss.logServer.Count;
		GUIContent[] msgContent = new GUIContent[count];
		for (int i = 0; i < count; i++)
		{
			msgContent[i] = new GUIContent(sss.logServer[i]);
			h += style.CalcHeight(msgContent[i], ScreenW - 245);
		}	
		

		scrollPosition = GUI.BeginScrollView(new Rect(10, 10, ScreenW - 230, ScreenH - 55), scrollPosition, new Rect(0, 0, ScreenW - 245, (int)h), false, true);
		h = 0;
		for (int i = 0; i < count; i++)
		{
			float he = style.CalcHeight(msgContent[i], ScreenW - 245);
			GUI.Label(new Rect(0, h, ScreenW - 245, (int)he), msgContent[i], style);
			h += he;
		}
        GUI.EndScrollView();

		textCMD = GUI.TextField(new Rect(10, ScreenH - 40, ScreenW - 340, 30), textCMD);

		if (GUI.Button(new Rect(ScreenW - 320, ScreenH - 40, 100, 30), "Send") || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			if (textCMD.Length > 0)
			{
				string[] args = textCMD.Split(' ');
				if (args.Length > 0)
				{
					//COMMAND: help
					if (args[0].ToLower() == "help")
					{
						sss.Log("=============== Liste des commandes ===============");
						sss.Log("Exemple: nom <obligatoire> (optionnel)");
						sss.Log("item <templateID> <characterName> (quantity) -> Ajoute un item");
						sss.Log("delitem <ID/templateID/slotID> <characterName> (type) (quantity) -> Retire un item");
						sss.Log("\ttype {0 = ID, 1 = templateID, 2 = slotID}; Default = ID");
					}
					//COMMAND: item <templateID> <characterName> (quantity)
					if (args.Length >= 3 && args[0].ToLower() == "item")
					{
						string characterName = args[2].ToLower();
						int quantity = args.Length >= 4 ? Convert.ToInt32(args[3]) : 1;
						ServerClient sc = sss.GetServerClientByName(characterName);
						if (sc != null)
							sc.CreateAndSendItem(Convert.ToInt32(args[1]), quantity);
						else
							sss.Log("FAIL Add Item : " + characterName + " not found");
					}
					//COMMAND: delitem <ID/templateID/slotID> <characterName> (type) (quantity)
					// type {0 = ID, 1 = templateID, 2 = slotID}; Default = ID
					if (args.Length >= 3 && args[0].ToLower() == "delitem")
					{
						string characterName = args[2].ToLower();
						int type = args.Length >= 4 ? Convert.ToInt32(args[3]) : 0;
						int quantity = args.Length >= 5 ? Convert.ToInt32(args[4]) : 1;
						ServerClient sc = sss.GetServerClientByName(characterName);
						if (sc != null)
							sc.RemoveItem(Convert.ToInt32(args[1]), type, quantity);
						else
							sss.Log("FAIL Delete Item : " + characterName + " not found");
					}
				}
				/* COMMANDE SERVER */
				textCMD = "";
			}
		}
	}

}
