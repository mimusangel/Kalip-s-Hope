using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UICharacterSlot : MonoBehaviour, IPointerClickHandler {

	public static UICharacterSlot selected = null;

	public int index;
	public Text CharacterName;
	public Text CharacterClasse;
	public Text CharacterLevel;

	
	Image _image;

	void Start () {
		if (selected == null)
		{
			selected = this;
		}
		_image = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
		if (selected == this)
		{
			_image.color = Color.red;
		}
		else
		{
			_image.color = Color.white;
		}
	}

	public void OnPointerClick(PointerEventData eventData)
    {
        selected = this;
    }
}
