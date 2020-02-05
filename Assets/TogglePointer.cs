using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class TogglePointer : MonoBehaviour {

    public VRTK_Pointer pointer;

	// Use this for initialization
	void Start () {
        GetComponent<VRTK_ControllerEvents>().TouchpadPressed += PressedTouchpad;
        GetComponent<VRTK_ControllerEvents>().TouchpadReleased += ReleasedTouchpad;
    }
	
    private void PressedTouchpad(object sender, ControllerInteractionEventArgs e)
    {
        pointer.Toggle(false);
    }

    private void ReleasedTouchpad(object sender, ControllerInteractionEventArgs e)
    {
        pointer.Toggle(true);
    }

	// Update is called once per frame
	void Update () {
		
	}
}
