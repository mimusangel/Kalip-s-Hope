using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFollowMouse : MonoBehaviour {
	
	void LateUpdate ()
    {
        transform.position = Input.mousePosition;
	}
}
