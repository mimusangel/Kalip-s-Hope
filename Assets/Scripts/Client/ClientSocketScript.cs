using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Data;
using UnityEngine;

public class ClientSocketScript : SocketScript {

	public static bool	readMutex = false;
	override public void Run()
	{
		StartCoroutine("autoConnect");
	}
	void ThreadReceive()
	{
		byte[] byteBuffer;
		List<byte> arrayByte = new List<byte>();
		int indexArray;
		while(true)
		{
			if (_socket == null)
				return;
			while(_socket.Available > 0)
			{
				readMutex = true;
				Log("Received: " + _socket.Available);
				byteBuffer = new byte[_socket.Available];
				_socket.Receive(byteBuffer, 0, _socket.Available, SocketFlags.None);
				arrayByte.AddRange(byteBuffer);
				/*Packet readPacket = new Packet(byteBuffer);
				if (!PacketHandler.Parses(_socket, readPacket))
				{
					Log("Error Packet!");
					// Erreur Packet !
				}*/
			}
			indexArray = 0;
			byte[] tmp = arrayByte.ToArray();
			while (indexArray < arrayByte.Count)
			{
				int size = BitConverter.ToInt32(tmp, indexArray);
				indexArray += 4;
				byte[] packetArray = new byte[size];
				Buffer.BlockCopy(tmp, indexArray, packetArray, 0, size);
				Log(size);
				Packet readPacket = new Packet(packetArray);
				if (!PacketHandler.Parses(_socket, readPacket))
				{
					Log("Error Packet!");
				}
				indexArray += size;
				Log("indexArray: " + indexArray + " < " + arrayByte.Count);
			}
			arrayByte.Clear();
			readMutex = false;
			Thread.Sleep(10);
		}
	}
	IEnumerator autoConnect()
	{
		bool autoconnect = true;
		while (autoconnect)
		{
			try {
				_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, type);
				_socket.Connect(GetAddress());
				if (_socket.Connected)
				{
					Debug.Log("Socket est connecte!");
					Thread receiveThread = new Thread(new ThreadStart(ThreadReceive));
					receiveThread.Start();
					Thread checkIsConnectThread = new Thread(new ThreadStart(ThreadCheckIsConnected));
					checkIsConnectThread.Start();
					autoconnect = false;
				}
			} catch(SocketException e) {
				_socket = null;
				Debug.Log("Erreur connection au serveur: " + e.Message);
			}
			yield return new WaitForSeconds(5.0f);
		}
		
	}
	public void Disconnect()
	{
		_socket.Close();
		_socket = null;
		_dispatcher.Invoke(() => {
			UIMainMenu.instance.ButtonGotoConnexionMenu();
			Run();
		});
	}

	void ThreadCheckIsConnected()
	{
		while(true)
		{
			if (_socket.Poll(10, SelectMode.SelectRead) && _socket.Available == 0 && !readMutex)
			{
				Disconnect();
				return;
			}
			Thread.Sleep(5);
		}
	}
	public void Send(Packet packet)
	{
		if (_socket == null)
			return ;
		//_socket.SendBufferSize = packet.Size();
		_socket.Send(packet.GetBuffer(), packet.Size(), SocketFlags.None);
	}
	override protected void Close()
	{

	}
	/*
	 * ************************** *
	 * *** INITIALIZE PACKETS *** *
	 * ************************** *
	 */
	override protected void InitPacket()
	{
		// Account Packet
		//PacketHandler.packetList.Add(PacketHandler.PacketID_Login, Packet_AccountConnect); // Serv <= Client (string, string)
		//PacketHandler.packetList.Add(PacketHandler.PacketID_Register, Packet_AccountRegister); // Serv <= Client (string, string)
		PacketHandler.packetList.Add(PacketHandler.PacketID_OpenMenu, Packet_OpenMenu); // Serv <=> Client (int, ...)
		//PacketHandler.packetList.Add(PacketHandler.PacketID_AccountCharacter, Packet_AccountCharacter); // Serv <= Client (int)
		

		// Character Packet
		PacketHandler.packetList.Add(PacketHandler.PacketID_ListEffectTemplate, Packet_EffectTemplate); // Serv => Client
		PacketHandler.packetList.Add(PacketHandler.PacketID_ListItemTemplate, Packet_ItemTemplate); // Serv => Client
		PacketHandler.packetList.Add(PacketHandler.PacketID_Character, Packet_Character); // Serv => Client
		PacketHandler.packetList.Add(PacketHandler.PacketID_CharacterInventory, Packet_CharacterInventory); // Serv => Client

		// Chat/Message/Popup
		PacketHandler.packetList.Add(PacketHandler.PacketID_Chat, Packet_Chat); // Serv <=> Client (int, [int], string)
		PacketHandler.packetList.Add(PacketHandler.PacketID_Popup, Packet_Popup); // Serv => Client (int, string)

		// GameObject
		PacketHandler.packetList.Add(PacketHandler.PacketID_Instantiate, Packet_Instantiate); // Serv => Client (str, int, bool, Vec3, Quat)
		PacketHandler.packetList.Add(PacketHandler.PacketID_Destroy, Packet_Destroy); // Serv => Client (int)
		PacketHandler.packetList.Add(PacketHandler.PacketID_UpdatePosition, Packet_UpdatePosition); // Serv => Client (int, Vec3)

	}

	/* *** Account Packet *** */
	void Packet_OpenMenu(Socket sender, Packet packet)
	{
		int menu = packet.ReadInt();
		switch(menu)
		{
			case 1: // Create Player
				_dispatcher.Invoke(() => UIMainMenu.instance.ButtonGotoNewCharacterMenu());
				break;
			case 2: // Select Player
				int count = packet.ReadInt();
				GameManager.instance.characters.Clear();
				for (int i = 0; i < count; i++)
				{
					Character charac = new Character();
					charac.Read(packet);
					GameManager.instance.characters.Add(charac.index, charac);
				}
				_dispatcher.Invoke(() => UIMainMenu.instance.ButtonGotoCharactersMenu());
				break;
			case 42: // Goto Game
				_dispatcher.Invoke(() => UIMainMenu.instance.StartGame());
				break;
			default:
				break;
		}
	}

	/* *** Character Packet *** */
	void Packet_EffectTemplate(Socket sender, Packet packet)
	{
		GameManager.instance.effectTypes.Clear();
		int count = packet.ReadInt();
		Log("C: " + count);
		for (int i = 0; i < count; i++)
		{
			Log("i: " + i);
			EffectType eff = new EffectType(packet);
			GameManager.instance.effectTypes.Add(eff.id, eff);
		}
		Log(count + " Effect Template Received!");
	}
	void Packet_ItemTemplate(Socket sender, Packet packet)
	{
		GameManager.instance.itemTemplates.Clear();
		int count = packet.ReadInt();
		for (int i = 0; i < count; i++)
		{
			Item item = new Item(packet);
			GameManager.instance.itemTemplates.Add(item.id, item);
		}
		Debug.Log(count + " Item Template Received!");
	}
	void Packet_Character(Socket sender, Packet packet)
	{
		Character charac = new Character();
		charac.Read(packet);
		GameManager.instance.characters.Add(charac.index, charac);
		Debug.Log("Character " + charac.name + "#" + charac.index + " received!");
	}

	void Packet_CharacterInventory(Socket sender, Packet packet)
	{
		int charID = packet.ReadInt();
		if (GameManager.instance.characters.ContainsKey(charID))
		{
			GameManager.instance.characters[charID].ReadInventory(packet);
			_dispatcher.Invoke(() => {
				foreach(Item item in GameManager.instance.characters[charID].inventory.Values)
				{
					UIInventory.instance.AddItem(item);
				}
			});
			Debug.Log(GameManager.instance.characters[charID].inventory.Count + " Item in Inventory Received!");
		}
	}	



	/* *** Chat/Message/Popup *** */
	void Packet_Chat(Socket sender, Packet packet)
	{
		int type = packet.ReadInt();
		if (type == 2)
		{
			/*string toName = */packet.ReadString();
			string msg = packet.ReadString();

		}
		else
		{
			string msg = packet.ReadString();

		}
	}
	void Packet_Popup(Socket sender, Packet packet)
	{
		int type = packet.ReadInt();
		string msg = packet.ReadString();
		_dispatcher.Invoke(
			() => {
				if (UICanvasPopup.instance)
				{
					string title = (type == 2) ? "Error" : (type == 1) ? "Warning" : "Information";
					UICanvasPopup.instance.AddPopup(title, msg);
				}
			}
		);
	}
	/* *** GameObject *** */
	void Packet_Instantiate(Socket sender, Packet packet)
	{
		string prefabName = packet.ReadString();
		int index = packet.ReadInt();
		bool isMine = packet.ReadBool();
		Vector3 position = packet.ReadVector3();
		Quaternion rotation = packet.ReadQuaternion();
		_dispatcher.Invoke(
			() => {
				Remote remote = Instantiate(index, prefabName, position, rotation);
				remote.isMine = isMine;
			}
		);
	}
	void Packet_Destroy(Socket sender, Packet packet)
	{
		int index = packet.ReadInt();
		Remote remote = GetGameObject(index);
		if (remote != null)
		{
			_dispatcher.Invoke(
				() => Destroy(remote)
			);
		}
	}
	void Packet_UpdatePosition(Socket sender, Packet packet)
	{
		int index = packet.ReadInt();
		Vector3 position = packet.ReadVector3();
		Remote remote = GetGameObject(index);
		if (remote != null)
		{
			_dispatcher.Invoke(
				() => {
					remote.gameObject.transform.position = position;
				}
			);
		}
	}

}