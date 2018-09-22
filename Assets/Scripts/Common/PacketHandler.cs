using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using UnityEngine;

class PacketHandler {
	public const int PacketID_Login = 0;
	public const int PacketID_Register = 1;
	public const int PacketID_OpenMenu = 2; // Ouvre un menu
	public const int PacketID_AccountCharacter = 3;
	public const int PacketID_AccountCharacterCreate = 4;


	public const int PacketID_ListEffectTemplate = 5;
	public const int PacketID_ListItemTemplate = 6;

	public const int PacketID_Character = 10; // Envoie le personnage du joueur connecter pour informer les autre joueurs
	public const int PacketID_CharacterInventory = 11; // Envoie l'inventaire du joueur
	public const int PacketID_CharacterSkillTree = 12; // Envoie l'arbre des competences/spell
	public const int PacketID_CharacterSkillBook = 13; // Envoie la list des spells
	public const int PacketID_AddItem = 20; // Ajouter un item
	public const int PacketID_RemoveItem = 21; // Retirer un item

	public const int PacketID_Chat = 50;
	public const int PacketID_Popup = 51;

	public const int PacketID_Kick = 60;
	public const int PacketID_Ban = 61;

	public const int PacketID_Instantiate = 100;
	public const int PacketID_Destroy = 101;
	public const int PacketID_UpdatePosition = 150;


	public static Packet	newPacket(int packet_id, params object[] list)
	{
		Packet packet = new Packet();
		packet.Add(packet_id);
		for (int i = 0; i < list.Length; i++)
			packet.Add(list[i]);
		return packet;
	}

	
	public delegate void PacketReceive(Socket sender, Packet packet);
	public static Dictionary<int, PacketReceive> packetList = new Dictionary<int, PacketReceive>();

	public static bool Parses(Socket sender, Packet packet)
	{
		int packetID = packet.ReadInt();
		Debug.Log("PacketHandler Parsing: " + packetID);
		if (packetList.ContainsKey(packetID))
		{
			packetList[packetID](sender, packet);
			return true;
		}
		return false;
	}
}