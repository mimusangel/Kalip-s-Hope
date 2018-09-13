using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour {

	public static UIMainMenu instance {get; private set;}

	public GameObject bgMainMenu;
	public GameObject panelConnection;
	public GameObject panelRegister;
	public GameObject panelCharacters;
	public GameObject panelNewCharacters;
	public GameObject panelGame;

	public Text networkStatusText;

	private GameManager.Side _side = GameManager.Side.None;

	public GameObject allSlot;

	public List<GameObject> allCharacter = new List<GameObject>();

	void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	void Start () {
		bgMainMenu.SetActive(false);
		panelConnection.SetActive(false);
		panelRegister.SetActive(false);
		panelCharacters.SetActive(false);
		panelNewCharacters.SetActive(false);
		panelGame.SetActive(false);
	}
	
	bool IsConnected()
	{
		bool connected = false;
		if (SocketScript.Instance)
			connected = SocketScript.Instance.IsConnected();
		return connected;
	}

	// Update is called once per frame
	void Update () {
		if (GameManager.instance)
		{
			if (GameManager.instance.side != _side)
			{
				_side = GameManager.instance.side;
				if (_side == GameManager.Side.Client)
				{
					ButtonGotoConnexionMenu();
				}
			}
		}
		if (_side == GameManager.Side.Client)
		{
			if (IsConnected())
				networkStatusText.text = "Status: <color=green>Online</color>";
			else
				networkStatusText.text = "Status: <color=red>Offline</color>";
		}
	}
	public void ButtonGotoConnexionMenu()
	{
		bgMainMenu.SetActive(true);
		panelConnection.SetActive(true);
		panelRegister.SetActive(false);
		panelCharacters.SetActive(false);
		panelNewCharacters.SetActive(false);
		panelGame.SetActive(false);
	}
	public void ButtonGotoRegisterMenu()
	{
		bgMainMenu.SetActive(true);
		panelConnection.SetActive(false);
		panelRegister.SetActive(true);
		panelCharacters.SetActive(false);
		panelNewCharacters.SetActive(false);
		panelGame.SetActive(false);
	}
	public void ButtonGotoCharactersMenu()
	{
		foreach(GameObject go in allCharacter)
			Destroy(go);
		allCharacter.Clear();
		GameObject prefab = Resources.Load<GameObject>("Prefabs/Slot");
		foreach(KeyValuePair<int, Character> charac in GameManager.instance.characters)
		{
			GameObject go = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, allSlot.transform);

			allCharacter.Add(go);
		}

		bgMainMenu.SetActive(true);
		panelConnection.SetActive(false);
		panelRegister.SetActive(false);
		panelCharacters.SetActive(true);
		panelNewCharacters.SetActive(false);
		panelGame.SetActive(false);
	}
	public void ButtonGotoNewCharacterMenu()
	{
		bgMainMenu.SetActive(true);
		panelConnection.SetActive(false);
		panelRegister.SetActive(false);
		panelCharacters.SetActive(false);
		panelNewCharacters.SetActive(true);
		panelGame.SetActive(false);
	}
	public void ButtonDisconnect()
	{
		ClientSocketScript css = GameManager.instance.GetComponent<ClientSocketScript>();
		css.Disconnect();
		ButtonGotoConnexionMenu();
	}

	public void ButtonCreateCharacter()
	{
		UICreateCharacter ui = panelNewCharacters.GetComponent<UICreateCharacter>();
		if (ui.avatarName.text.Length >= 4)
		{
			ClientSocketScript css = GameManager.instance.GetComponent<ClientSocketScript>();
			css.Send(
				PacketHandler.newPacket(
					PacketHandler.PacketID_AccountCharacterCreate,
					ui.avatarName.text,
					ui.body,
					ColorUtility.ToHtmlStringRGB(ui.bodyColor),
					ui.eye,
					ui.frontHair,
					ui.rearHair,
					ColorUtility.ToHtmlStringRGB(ui.hairColor)
				)
			);
		}
		else
		{
			UICanvasPopup.instance.AddPopup("Warning", "Le nom de votre personnage est trop court (4 caracteres minimal).");
		}
	}

	public void ButtonCharacterPlay()
	{
		if (GameManager.instance.characters.Count <= 0)
			return;
		Character character = null;
		foreach(KeyValuePair<int, Character> charac in GameManager.instance.characters)
		{
			character = charac.Value;
			break;
		}
		ClientSocketScript css = GameManager.instance.GetComponent<ClientSocketScript>();
		css.Send(
			PacketHandler.newPacket(
				PacketHandler.PacketID_AccountCharacter,
				character.index
			)
		);
	}

	public void ButtonRegister()
	{
		if (!IsConnected())
		{
			if (UICanvasPopup.instance)
			{
				UICanvasPopup.instance.AddPopup("Error", "Vous n'etes pas connecter au serveur.");
			}
			return ;
		}
		InputField login = GameObject.Find("InputFieldRegisterLogin").GetComponent<InputField>();
		InputField pwd = GameObject.Find("InputFieldRegisterPassword").GetComponent<InputField>();
		InputField pwdConfirm = GameObject.Find("InputFieldRegisterPasswordConfirm").GetComponent<InputField>();
		if (login.text.Length >= 4)
		{
			if (pwd.text.Length >= 6)
			{
				if (pwd.text == pwdConfirm.text)
				{
					ClientSocketScript css = GameManager.instance.GetComponent<ClientSocketScript>();
					css.Send(
						PacketHandler.newPacket(PacketHandler.PacketID_Register,
							login.text,
							pwd.text
						)
					);
				}
				else
				{
					if (UICanvasPopup.instance)
					{
						UICanvasPopup.instance.AddPopup("Warning", "La confirmation de mot de passe n'est pas identique au mot de passe.");
					}
				}
			}
			else
			{
				if (UICanvasPopup.instance)
				{
					UICanvasPopup.instance.AddPopup("Warning", "Le mot de passe doit faire 6 caractere au minimum");
				}
			}
		}
		else
		{
			if (UICanvasPopup.instance)
			{
				UICanvasPopup.instance.AddPopup("Warning", "Le login doit faire 4 caractere au minimum");
			}
		}
	}
	public void ButtonConnexion()
	{
		if (!IsConnected())
		{
			if (UICanvasPopup.instance)
			{
				UICanvasPopup.instance.AddPopup("Error", "Vous n'etes pas connecter au serveur.");
			}
			return ;
		}
		InputField login = GameObject.Find("InputFieldConnectLogin").GetComponent<InputField>();
		InputField pwd = GameObject.Find("InputFieldConnectPassword").GetComponent<InputField>();
		if (login.text.Length >= 4)
		{
			if (pwd.text.Length >= 6)
			{
				ClientSocketScript css = GameManager.instance.GetComponent<ClientSocketScript>();
				css.Send(
					PacketHandler.newPacket(PacketHandler.PacketID_Login,
						login.text,
						pwd.text
					)
				);
			}
			else
			{
				if (UICanvasPopup.instance)
				{
					UICanvasPopup.instance.AddPopup("Warning", "Le mot de passe doit faire 6 caractere au minimum");
				}
			}
		}
		else
		{
			if (UICanvasPopup.instance)
			{
				UICanvasPopup.instance.AddPopup("Warning", "Le login doit faire 4 caractere au minimum");
			}
		}
	}
	public void StartGame()
	{
		GameManager.instance.characters.Clear();
		bgMainMenu.SetActive(false);
		panelConnection.SetActive(false);
		panelRegister.SetActive(false);
		panelCharacters.SetActive(false);
		panelNewCharacters.SetActive(false);
		panelGame.SetActive(true);
	}
}
