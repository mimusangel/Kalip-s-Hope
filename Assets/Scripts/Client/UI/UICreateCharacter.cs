using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICreateCharacter : MonoBehaviour {

	public InputField avatarName;
	public Image avatarRearHair;
	public Image avatarBody;
	public Image avatarEye;
	public Image avatarFrontHair; 

	public int rearHair = 0;
	public int body = 0;
	public int eye = 0;
	public int frontHair = 0;
	public Color bodyColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	public Color hairColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	

	public UIColor uIColor;

	public Image bodyColorSelectImg;
	public Image bodyColorImg;
	public Image hairColorSelectImg;
	public Image hairColorImg;

	void Start()
	{
		UpdateFrontHair();
		UpdateBody();
		UpdateEye();
		UpdateRearHair();
	}

	void Update()
	{
		bodyColorImg.color = bodyColor;
		hairColorImg.color = hairColor;
		if (uIColor.gameObject.activeSelf == false)
		{
			hairColorSelectImg.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			bodyColorSelectImg.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
	}

	public void ButtonNextFrontHair()
	{
		frontHair = (frontHair + 1) % GameManager.instance.frontHairs.Count;
		UpdateFrontHair();
	}
	public void ButtonPreviousFrontHair()
	{
		frontHair--;
		if (frontHair < 0)
			frontHair =  GameManager.instance.frontHairs.Count - 1;
		UpdateFrontHair();
	}
	void UpdateFrontHair() {
		avatarFrontHair.sprite = GameManager.instance.frontHairs[frontHair][1];
		avatarFrontHair.color = hairColor;
	}

	public void ButtonNextRearHair()
	{
		rearHair = (rearHair + 1) % GameManager.instance.rearHairs.Count;
		UpdateRearHair();
	}
	public void ButtonPreviousRearHair()
	{
		rearHair--;
		if (rearHair < 0)
			rearHair =  GameManager.instance.rearHairs.Count - 1;
		UpdateRearHair();
	}
	void UpdateRearHair() {
		avatarRearHair.sprite = GameManager.instance.rearHairs[rearHair][1];
		avatarRearHair.color = hairColor;
	}

	public void ButtonNextBody()
	{
		body = (body + 1) % GameManager.instance.bodys.Count;
		UpdateBody();
	}
	public void ButtonPreviousBody()
	{
		body--;
		if (body < 0)
			body =  GameManager.instance.bodys.Count - 1;
		UpdateBody();
	}
	void UpdateBody() {
		avatarBody.sprite = GameManager.instance.bodys[body][1];
		avatarBody.color = bodyColor;
	}

	public void ButtonNextEye() {
		eye = (eye + 1) % GameManager.instance.eyes.Count;
		UpdateEye();
	}
	public void ButtonPreviousEye() {
		eye--;
		if (eye < 0)
			eye =  GameManager.instance.eyes.Count - 1;
		UpdateEye();
	}
	void UpdateEye() {
		avatarEye.sprite = GameManager.instance.eyes[eye][1];
	}

	public void ButtonSetColor(int change)
	{
		uIColor.gameObject.SetActive(true);
		if (change == 1)
		{
			uIColor.changeColor = HairChangeColor;
			hairColorSelectImg.color = new Color(0.5f, 0.0f, 0.0f, 1.0f);
			bodyColorSelectImg.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
		else
		{
			uIColor.changeColor = BodyChangeColor;
			bodyColorSelectImg.color = new Color(0.5f, 0.0f, 0.0f, 1.0f);
			hairColorSelectImg.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
	}

	public void HairChangeColor(Color color)
	{
		hairColor = color;
		UpdateFrontHair();
		UpdateRearHair();
	}

	public void BodyChangeColor(Color color)
	{
		bodyColor = color;
		UpdateBody();
	}	
}
