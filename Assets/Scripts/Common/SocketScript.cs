using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(Dispatcher))]
abstract public class SocketScript : MonoBehaviour {
	public static SocketScript Instance {get; private set;}
	protected Dictionary<int, Remote> _gameObjects = new Dictionary<int, Remote>();
	public string			address = "127.0.0.1";
	public int				port = 4221;
	protected Dispatcher	_dispatcher;
	protected Socket		_socket = null;
	public ProtocolType		type = ProtocolType.Tcp;

	void Awake()
	{
		Instance = this;
	}
	private void Start() {
		_dispatcher = GetComponent<Dispatcher>();
		InitPacket();
	}
	public IPEndPoint GetAddress()
	{
		IPAddress ipAddress = IPAddress.Parse(address);
		return (new IPEndPoint(ipAddress, port));
	}
	abstract public void Run();
	abstract protected void InitPacket();
	abstract protected void Close();
	public bool IsServer()
	{
		if (GameManager.instance != null)
			return GameManager.instance.side == GameManager.Side.Server;
		return false;
	}

	public bool IsConnected()
	{
		if (_socket != null)
			return _socket.Connected;
		return false;
	}

	virtual public void Log(object message)
	{
		Debug.Log("[Client] " + message);
	}
	
	void OnDestroy()
	{
		Close();
		if (_socket == null)
			return;
		_socket.Close();
	}
	
	public Remote	Instantiate(string prefabName, Vector3 position, Quaternion rotation)
	{
		GameObject prefab = Resources.Load<GameObject>(prefabName);
		if (!prefab)
			return null;
		GameObject go = GameObject.Instantiate(prefab, position, rotation);
		Remote remote = go.AddComponent<Remote>();
		remote.prefabName = prefabName;
		remote.index = go.GetInstanceID();
		remote.ss = this;
		remote.isMine = false;
		remote.socket = null;
		_gameObjects.Add(remote.index, remote);
		return remote;
	}
	public Remote	Instantiate(int index, string prefabName, Vector3 position, Quaternion rotation)
	{
		GameObject prefab = Resources.Load<GameObject>(prefabName);
		if (!prefab)
			return null;
		GameObject go = GameObject.Instantiate(prefab, position, rotation);
		Remote remote = go.AddComponent<Remote>();
		remote.prefabName = prefabName;
		remote.index = index;
		remote.ss = this;
		remote.isMine = false;
		remote.socket = null;
		_gameObjects.Add(index, remote);
		return remote;
	}
	public void		Destroy(Remote remote)
	{
		_gameObjects.Remove(remote.index);
		Destroy(remote.gameObject);
	}
	public Remote		GetGameObject(int id)
	{
		if (!_gameObjects.ContainsKey(id))
			return null;
		return _gameObjects[id];
	}
}