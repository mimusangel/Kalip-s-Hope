using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Security.Cryptography;
using System;
using System.Data;
using System.Text;
using UnityEngine;

public class ServerClient {
	private ServerSocketScript	_ss;
	private Socket	_socket;

	// Utilitaire

	// Donnee compte et joueur
	public string account {get; private set; }
	private int _accountID = -1;
	public bool connected {get; private set;}
	Dictionary<int, Character> _characters = new Dictionary<int, Character>();
	public int characterSelected {get; private set;}


	public ServerClient(ServerSocketScript ss, Socket socket)
	{
		_ss = ss;
		_socket = socket;
		characterSelected = -1;
	}

	public void Send(Packet packet)
	{
		if (_socket == null)
			return ;
		if (!_socket.Connected)
			return ;
		_socket.Send(packet.GetBuffer(), packet.Size(), SocketFlags.None);
	}

	public void Close()
	{
		_characters.Clear();
		connected = false;
		Log("déconnecté!");
	}

	public Character GetConnectedCharacter()
	{
		if (characterSelected >= 0)
			return _characters[characterSelected];
		return null;
	}

	public string GetName()
	{
		if (characterSelected >= 0)
			return _characters[characterSelected].name;
		return account;
	}

	public void Log(object message)
	{
		string info = "";
		if (connected)
		{
			info += account;
			if (_accountID > 0)
				info += "#" + _accountID;
			if (characterSelected >= 0)
				info += " / " + _characters[characterSelected].name;
		}
		else
			info += _socket.GetHashCode();
		
		Debug.Log("[Server: " + info + "] " + message);
		_ss.logServer.Add("[" + info + "] " + message.ToString());
		_ss.checkClearMessage();
	}

	private string HashPassword(string mdp)
	{
		mdp = "qhonore" + mdp + "mgallo";
		SHA256Managed crypt = new SHA256Managed();
		string hash = string.Empty;
		byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(mdp));
		foreach (byte theByte in crypto)
		{
			hash += theByte.ToString("x2");
		}
		return hash;
	}

	private void InitAccount()
	{
		_accountID = -1;
		characterSelected = -1;
		_characters.Clear();
		IDataReader reader = DBManager.Select("id", "Account", "login='"+account+"'");
		if (reader.Read())
		{
			_accountID = reader.GetInt32(0);
		}
		reader.Close();
		reader = null;
		Log("Chargement des donnees du compte effectuer.");
	}

	private void InitCharactersList()
	{
		_characters.Clear();
		IDataReader reader = DBManager.Select("id, name, level, exp, skilltree, life", "Character", "account="+_accountID);
		while (reader.Read())
		{
			Character charac = new Character();
			charac.Read(reader);
			_characters.Add(charac.index, charac);
		}
		reader.Close();
		reader = null;
	}

	public void AccountConnect(string login, string mdp)
	{
		login = login.ToLower();
		mdp = HashPassword(mdp); // ICI On Cript le MDP avec notre Sel
		if (DBHandler.AccountConnection(login, mdp))
		{
			Log("C'est connecter a sont compte: " + login);
			connected = true;
			account = login;
			InitAccount();
			InitCharactersList();
			Log(_characters.Count + " character a ete charger!");
			if (_characters.Count > 0)
			{ // Envoie de la liste de ses personnage
				Packet packet = PacketHandler.newPacket(
					PacketHandler.PacketID_OpenMenu,
					2,
					_characters.Count
				);
				foreach(KeyValuePair<int, Character> charac in _characters)
				{
					charac.Value.Write(packet);
				}
				_ss.SendTo(_socket, packet);
			}
			else
			{ // Envoie de la creation de personnage
				_ss.SendTo(_socket,
					PacketHandler.newPacket(
						PacketHandler.PacketID_OpenMenu,
						1
					)
				);
			}
		}
		else
		{
			Log(DBHandler.Error);
			_ss.SendTo(_socket,
				PacketHandler.newPacket(
					PacketHandler.PacketID_Popup,
					1,
					DBHandler.Error
				)
			);
		}
	}

	public void AccountRegister(string login, string mdp)
	{
		login = login.ToLower();
		mdp = HashPassword(mdp); // ICI On Cript le MDP avec notre Sel
		//Log("mdp crypt Length: " + mdp.Length);
		if (DBHandler.AccountRegister(login, mdp))
		{
			
			Log("Viens de creer sont compte: " + login.ToLower());
			connected = true;
			account = login;
			InitAccount();
			// Envoie de la creation de personnage
			_ss.SendTo(_socket,
				PacketHandler.newPacket(
					PacketHandler.PacketID_OpenMenu,
					1
				)
			);
		}
		else
		{
			Log(DBHandler.Error);
			_ss.SendTo(_socket,
				PacketHandler.newPacket(
					PacketHandler.PacketID_Popup,
					1,
					DBHandler.Error
				)
			);
		}
	}

	public void LoadAndSendDataPlayer()
	{
		
		Packet packet;
		// Envoie Effet Template
		packet = PacketHandler.newPacket(
			PacketHandler.PacketID_ListEffectTemplate,
			GameManager.instance.effectTypes.Count
		);
		Log(GameManager.instance.effectTypes.Count + " Effect Type Count");
		foreach(EffectType eff in GameManager.instance.effectTypes.Values)
		 	eff.Write(packet);
		Log("Effect Type Packet Size: " + packet.Size());
		_ss.SendTo(_socket, packet);
		
		// Envoie Item Template
		packet = PacketHandler.newPacket(
			PacketHandler.PacketID_ListItemTemplate,
			GameManager.instance.itemTemplates.Count
		);
		Log(GameManager.instance.itemTemplates.Count + " Item Template Count");
		foreach(Item item in GameManager.instance.itemTemplates.Values)
		 	item.Write(packet);
		Log("Item Template Packet Size: " + packet.Size());
		_ss.SendTo(_socket, packet);

		// Chargement Inventaire
		int nb = _characters[characterSelected].inventory.Load();
		Log(nb + " items loaded!");

		// Envoie du Personnage
		packet = PacketHandler.newPacket(
			PacketHandler.PacketID_Character
		);
		_characters[characterSelected].Write(packet);
		Log("Character Packet Size: " + packet.Size());
		_ss.SendTo(_socket, packet);

		// Envoie de l'inventaire
		packet = PacketHandler.newPacket(
			PacketHandler.PacketID_CharacterInventory,
			characterSelected
		);
		_characters[characterSelected].inventory.Write(packet);
		Log("Inventory Packet Size: " + packet.Size());
		_ss.SendTo(_socket, packet);

	}

	public void AccountSelectCharacter(int index)
	{
		if (_characters.ContainsKey(index))
		{
			characterSelected = index;
			Log("Viens de se connecter au personnage nommer: " + _characters[index].name);
			Packet packet = PacketHandler.newPacket(
				PacketHandler.PacketID_OpenMenu,
				42
			);
			_ss.SendTo(_socket, packet);
			Log("Go to game Packet Size: " + packet.Size());
			LoadAndSendDataPlayer();
		}
		else
		{
			Log("Le Personnage n'a pas ete trouve! (acc: " + _accountID + " / i: " + index + ")");
			_ss.SendTo(_socket,
				PacketHandler.newPacket(
					PacketHandler.PacketID_Popup,
					1,
					"Le Personnage n'a pas ete trouve!"
				)
			);
		}
	}

	public void AccountCreateCharacter(string name, string sprite)
	{
		if (DBHandler.CharacterExist(name) == false)
		{
			characterSelected = DBHandler.CharacterAdd(_accountID, name, sprite);
			Debug.Log("ID: " + characterSelected);
			InitCharactersList();
			Log("Personnage a ete cree: " + _characters[characterSelected].name);
			_ss.SendTo(_socket,
				PacketHandler.newPacket(
					PacketHandler.PacketID_Popup,
					0,
					"Bienvenue " + _characters[characterSelected].name + " dans le monde de Kalip's!"
				)
			);
			_ss.SendTo(_socket, 
				PacketHandler.newPacket(
					PacketHandler.PacketID_OpenMenu,
					42
				)
			);
			LoadAndSendDataPlayer();
		}
		else
		{
			_ss.SendTo(_socket,
				PacketHandler.newPacket(
					PacketHandler.PacketID_Popup,
					1,
					"Un personnage portant se nom existe deja."
				)
			);
		}
	}

	public string ParseMsg(string msg) // Permet de faire une sorte de BBCode
	{
		msg = msg.Replace("%account%", account);
		msg = msg.Replace("%level%", "42");
		msg = msg.Replace("%name%", "CharacterName");
		return msg;
	} 
}