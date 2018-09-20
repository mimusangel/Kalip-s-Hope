using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasPositioningExtensions : MonoBehaviour {

	static Canvas canvas;

	void Start()
	{
		canvas = GetComponent<Canvas>();
	}

	public static Vector3 WorldToCanvasPosition(Vector3 worldPosition, Camera camera = null)
	{
		if (camera == null)
		{
			camera = Camera.main;
		}
		Vector3 viewportPosition = camera.WorldToViewportPoint(worldPosition);
		return ViewportToCanvasPosition(viewportPosition);
	}

	public static Vector3 ScreenToCanvasPosition(Vector3 screenPosition)
	{
		Vector3 viewportPosition = new Vector3(screenPosition.x / Screen.width,
			screenPosition.y / Screen.height,
			0);
		return ViewportToCanvasPosition(viewportPosition);
	}

	public static Vector3 ViewportToCanvasPosition(Vector3 viewportPosition)
	{
		Vector3 centerBasedViewPortPosition = viewportPosition - new Vector3(0.5f, 0.5f, 0);
		RectTransform canvasRect = canvas.GetComponent<RectTransform>();
		Vector2 scale = canvasRect.sizeDelta;
		return Vector3.Scale(centerBasedViewPortPosition, scale);
	}
}
