using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Data;
using UnityEngine;

public class ServerSocketScript : SocketScript {
	public List<string> logServer = new List<string>();
	//List<Socket> _clients;
	Dictionary<Socket, ServerClient> _clientsTable = new Dictionary<Socket, ServerClient>();
	List<Socket> _clientsSocket = new List<Socket>();
	public static bool	readMutex = false;
	override public void Run()
	{
		try {
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, type);
			_socket.Bind(GetAddress());
			_socket.Listen(10);
			Thread connectThread = new Thread(new ThreadStart(ThreadConnect));
			connectThread.Start();
			Thread receiveThread = new Thread(new ThreadStart(ThreadReceive));
			receiveThread.Start();
			Thread isConnectedThread = new Thread(new ThreadStart(ThreadCheckClientIsConnected));
			isConnectedThread.Start();
			InitData();
		} catch(SocketException e) {
			Log("Erreur creation de serveur socket: " + e.Message);
		}
	}
	public void LoadEffectTypes()
    {
        IDataReader buffer = DBManager.Select("*", "EffectType");
        while (buffer.Read())
        {
            int id = buffer.GetInt32(0);
            string name = buffer.GetString(1);
            string color = buffer.GetString(2);
            int category = buffer.GetInt32(3);
            GameManager.instance.effectTypes.Add(id, new EffectType(id, name, color, category));
        }
        Log(GameManager.instance.effectTypes.Count + " EffectType chargés.");
        buffer.Close();
        buffer = null;
    }
	public void LoadItemTemplates()
    {
        IDataReader buffer = DBManager.Select("*", "ItemTemplate");
        while (buffer.Read())
        {
            int id = buffer.GetInt32(0);
            string title = buffer.GetString(1);
            string description = buffer.GetString(2);
            string stats = buffer.IsDBNull(3) ? null : buffer.GetString(3);
            int maxNumber = buffer.GetInt32(4);
			GameManager.instance.itemTemplates.Add(id, new Item(id, title, description, Converter.ParseEffects(stats), maxNumber));
        }
        Log(GameManager.instance.itemTemplates.Count + " Item Template chargés.");
        buffer.Close();
        buffer = null;
    }
	void InitData()
	{
		DBManager.OpenDataBase();
		LoadEffectTypes();
		LoadItemTemplates();
	}
	void ThreadConnect()
	{
		Socket currentClient;
		Log("Le Serveur est a l'ecoute du port: <b>" + port + "</b>");
		while(true)
		{
			currentClient = _socket.Accept();
			Log("Nouveau client: " + currentClient.GetHashCode());
			_clientsSocket.Add(currentClient);
			_clientsTable.Add(currentClient, new ServerClient(this, currentClient));	
		}
	}
	void ThreadReceive()
	{
		List<Socket> readList = new List<Socket>();
		byte[] byteBuffer;
		Dictionary<Socket, List<byte>> arrayByte = new Dictionary<Socket, List<byte>>();
		while(true)
		{
			readList.Clear();
			readList.AddRange(_clientsSocket);
			if (readList.Count > 0)
			{
				Socket.Select(readList, null, null, 1000);
				foreach(Socket client in readList)
				{
					if (!arrayByte.ContainsKey(client))
					{
						arrayByte.Add(client, new List<byte>());
					}
					while(client.Available > 0)
					{
						readMutex = true;
						byteBuffer = new byte[client.Available];
						Log("Packet size received: " + client.Available);
						client.Receive(byteBuffer, 0, client.Available, SocketFlags.None);
						arrayByte[client].AddRange(byteBuffer);
					}
					while (arrayByte[client].Count >= 4)
					{
						int size = BitConverter.ToInt32(arrayByte[client].ToArray(), 0);
						if (arrayByte[client].Count >= size + 4)
						{
							arrayByte[client].RemoveRange(0, 4);
							byte[] packetArray = new byte[size];
							Buffer.BlockCopy(arrayByte[client].ToArray(), 0, packetArray, 0, size);
							arrayByte[client].RemoveRange(0, size);
							Packet readPacket = new Packet(packetArray);
							if (!PacketHandler.Parses(client, readPacket))
							{
								Log("Error Packet!");
							}
						}
						else
							break;
					}
					readMutex = false;
				}
			}
			Thread.Sleep(10);
		}
	}
	void	ClientDisconnect(Socket client)
	{
		foreach(KeyValuePair<int, Remote> entry in _gameObjects)
		{
			if (entry.Value.socket == client)
			{
				_dispatcher.Invoke(() => Destroy(entry.Value));
			}
		}
	}
	void ThreadCheckClientIsConnected()
	{
		while(true)
		{
			for(int i = 0; i < _clientsSocket.Count; i++)
			{
				Socket client = _clientsSocket[i];
				if (client.Poll(10, SelectMode.SelectRead) && client.Available == 0 && !readMutex)
				{
					_clientsSocket.Remove(client);
					_clientsTable[client].Close();
					_clientsTable.Remove(client);
					_dispatcher.Invoke(() => {
							ClientDisconnect(client);
							client.Close();
						}
					);
					i--;
				}
			}
			Thread.Sleep(5);
		}
	}

	public void SendTo(Socket sender, Packet packet)
	{
		if (_clientsTable.ContainsKey(sender))
		{
			_clientsTable[sender].Send(packet);
		}
	}

	public void SendToAll(Packet packet)
	{
		foreach(Socket client in _clientsSocket)
		{
			SendTo(client, packet);
		}
	}

	public void SendToOther(Socket sender, Packet packet)
	{
		foreach(Socket client in _clientsSocket)
		{
			if (client != sender)
				SendTo(client, packet);
		}
	}

	override protected void Close()
	{
		foreach(KeyValuePair<Socket, ServerClient> client in _clientsTable)
		{
			client.Value.Close();
		}
		_clientsTable.Clear();
		foreach(Socket client in _clientsSocket)
		{
			client.Close();
		}
		_clientsSocket.Clear();
	}

	public void checkClearMessage()
	{
		if (logServer.Count > 5000)
			logServer.RemoveAt(0);
	}

	override public void Log(object message)
	{
		logServer.Add(message.ToString());
		Debug.Log("[Server] " + message);
		checkClearMessage();
	}

	public bool ClientIsConnected(string login)
	{
		login = login.ToLower();
		foreach(KeyValuePair<Socket, ServerClient> client in _clientsTable)
		{
			if (client.Value.connected && client.Value.account == login)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsConnected(Socket socket)
	{
		if (_clientsTable.ContainsKey(socket))
			return _clientsTable[socket].connected;
		return false;
	}

	public Socket GetPlayerByName(string name)
	{
		foreach(KeyValuePair<Socket, ServerClient> client in _clientsTable)
		{
			if (client.Value.connected && client.Value.GetName() == name)
			{
				return client.Key;
			}
		}
		return null;
	}
	
	/*
	 * ************************** *
	 * *** INITIALIZE PACKETS *** *
	 * ************************** *
	 */
	override protected void InitPacket()
	{
		// Account Packet
		PacketHandler.packetList.Add(PacketHandler.PacketID_Login, Packet_AccountConnect); // Serv <= Client (string, string)
		PacketHandler.packetList.Add(PacketHandler.PacketID_Register, Packet_AccountRegister); // Serv <= Client (string, string)
		PacketHandler.packetList.Add(PacketHandler.PacketID_OpenMenu, Packet_OpenMenu); // Serv <=> Client (int, ...)
		PacketHandler.packetList.Add(PacketHandler.PacketID_AccountCharacter, Packet_AccountCharacter); // Serv <= Client (int)
		PacketHandler.packetList.Add(PacketHandler.PacketID_AccountCharacterCreate, Packet_AccountCharacterCreate); // Serv <= Client (str, int, str, int, int, int, str)
		// Character Packet
		//PacketHandler.packetList.Add(PacketHandler.PacketID_ListEffectTemplate, Packet_EffectTemplate); // Serv => Client
		//PacketHandler.packetList.Add(PacketHandler.PacketID_ListItemTemplate, Packet_ItemTemplate); // Serv => Client
		// PacketHandler.packetList.Add(PacketHandler.PacketID_Character, Packet_Character); // Serv => Client
		// PacketHandler.packetList.Add(PacketHandler.PacketID_CharacterInventory, PacketID_CharacterInventory); // Serv => Client


		// Chat/Message/Popup
		PacketHandler.packetList.Add(PacketHandler.PacketID_Chat, Packet_Chat); // Serv <=> Client (int, [int], string)
		//PacketHandler.packetList.Add(PacketHandler.PacketID_Popup, Packet_Popup); // Serv => Client (int, string)

		// GameObject
		//PacketHandler.packetList.Add(PacketHandler.PacketID_Instantiate, Packet_Instantiate); // Serv => Client (str, int, bool, Vec3, Quat)
		//PacketHandler.packetList.Add(PacketHandler.PacketID_Destroy, Packet_Destroy); // Serv => Client (int)
		//PacketHandler.packetList.Add(PacketHandler.PacketID_UpdatePosition, Packet_UpdatePosition); // Serv => Client (int, Vec3)
		
	}
	/* *** Account Packet *** */
	void Packet_AccountConnect(Socket sender, Packet packet)
	{
		string login = packet.ReadString();
		string mdp = packet.ReadString();
		if (_clientsTable.ContainsKey(sender))
		{
			if (ClientIsConnected(login))
			{
				_clientsTable[sender].Log("<color=red>Essaye de se connecter au compte: <b>" + login + "</b> qui est déja connecté!</color>");
				 SendTo(sender,
					 PacketHandler.newPacket(
						 PacketHandler.PacketID_Popup,
						 2,
						 "Ce compte est déja connecté au jeu!"
					 )
				 );
			}
			else
				_clientsTable[sender].AccountConnect(login, mdp);
		}
	}
	void Packet_AccountRegister(Socket sender, Packet packet)
	{
		string login = packet.ReadString();
		string mdp = packet.ReadString();
		if (_clientsTable.ContainsKey(sender))
		{
			_clientsTable[sender].AccountRegister(login, mdp);
		}
	}

	void Packet_OpenMenu(Socket sender, Packet packet)
	{
		int menu = packet.ReadInt();
		switch(menu)
		{
			default:
				break;
		}
	}

	void Packet_AccountCharacter(Socket sender, Packet packet)
	{
		if (!IsConnected(sender))
			return ;
		int index = packet.ReadInt();
		_clientsTable[sender].AccountSelectCharacter(index);
	}

	void Packet_AccountCharacterCreate(Socket sender, Packet packet)
	{ // (str, int, str, int, int, int, str)
		if (!IsConnected(sender))
			return ;
		string name = packet.ReadString();
		if (name.Length >= 4)
		{
			int body = packet.ReadInt();
			string bodyColor = packet.ReadString();
			int eye = packet.ReadInt();
			int frontHair = packet.ReadInt();
			int rearHair = packet.ReadInt();
			string hairColor = packet.ReadString();
			_clientsTable[sender].AccountCreateCharacter(name,
				body + ";" + bodyColor + ";" + eye + ";" + frontHair + ";" + rearHair + ";" + hairColor
			);
		}
		else
		{
			SendTo(sender,
				PacketHandler.newPacket(
					PacketHandler.PacketID_Popup,
					1,
					"Le nom de votre personnage est trop court (4 caracteres minimal)."
				)
			);
		}
	}
	/* *** Character Packet *** */

	/* *** Chat/Message/Popup *** */
	void Packet_Chat(Socket sender, Packet packet)
	{
		if (!IsConnected(sender))
			return ;
		int type = packet.ReadInt();
		string msg;
		switch(type)
		{
			case 1: // All
				msg = packet.ReadString();
				if (msg.Length <= 0)
					return ;
				Log("<b>" + _clientsTable[sender].GetName() + ":</b> " + _clientsTable[sender].ParseMsg(msg));
				SendToAll(
					PacketHandler.newPacket(
						PacketHandler.PacketID_Chat,
						1,
						"<b>" + _clientsTable[sender].GetName() + ":</b> " + _clientsTable[sender].ParseMsg(msg)
					)
				);
				break;
			case 2: // To
				string toName = packet.ReadString();
				if (toName.Length <= 0)
					return ;
				msg = packet.ReadString();
				if (msg.Length <= 0)
					return ;
				Socket toSocket = GetPlayerByName(toName);
				if (toSocket != null)
				{
					Log("<b>" + _clientsTable[sender].GetName() + " chuchote a " + toName + ":</b> " + _clientsTable[sender].ParseMsg(msg));
					SendTo(toSocket,
						PacketHandler.newPacket(
							PacketHandler.PacketID_Chat,
							2,
							_clientsTable[sender].GetName(),
							"<b>" + _clientsTable[sender].GetName() + " vous chuchote:</b> " + _clientsTable[sender].ParseMsg(msg)
						)
					);
				}
				else
				{
					Log("<color=orange><b>Le message n'a pas pu etre envoyer:</b> " + toName + " n'existe pas ou n'est pas connecte</color>");
					SendTo(sender,
						PacketHandler.newPacket(
							PacketHandler.PacketID_Chat,
							1,
							"<color=orange><b>Le message n'a pas pu etre envoyer:</b> " + toName + " n'existe pas ou n'est pas connecte</color>"
						)
					);
				}
				break;
			default: // Senser jamais arriver
				msg = packet.ReadString();
				if (msg.Length <= 0)
					return ;
				Log("<color=red><b>Le message n'a pas pu etre envoyer:</b> " + _clientsTable[sender].ParseMsg(msg) + "</color>");
				SendTo(sender,
					PacketHandler.newPacket(
						PacketHandler.PacketID_Chat,
						1,
						"<color=red><b>Le message n'a pas pu etre envoyer:</b> " + _clientsTable[sender].ParseMsg(msg) + "</color>"
					)
				);
				break;
		}
	}
	/* *** GameObject *** */
}
