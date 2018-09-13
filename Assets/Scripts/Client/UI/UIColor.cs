using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIColor : MonoBehaviour {

	public delegate void ChangeColor(Color color);

	public Image result;
	public Slider red;
	public Slider green;
	public Slider blue;
	
	public ChangeColor changeColor = null;

	void Start () {
		Debug.Log(ColorUtility.ToHtmlStringRGB(result.color));
	}

	public void SliderChangeValue()
	{
		result.color = new Color(red.value, green.value, blue.value, 1.0f);
		if (changeColor != null)
		{
			changeColor(result.color);
		}
	}
	
	public void ButtonRandom()
	{
		red.value = Random.Range(0.0f, 1.0f);
		green.value = Random.Range(0.0f, 1.0f);
		blue.value = Random.Range(0.0f, 1.0f);
	}

	public void ButtonOK()
	{
		gameObject.SetActive(false);
	}
}
