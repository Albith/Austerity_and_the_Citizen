using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//Class used to display and update buttons that only show their text (no graphics around the text).

public class blankButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{

	Color32 normal_fontColor;
	Color32 highlighted_fontColor;
	Color32 pressed_fontColor;
	Color32 disabled_fontColor;

	//Parameter that keeps track of the attached button's actorType(1 of 3).
	public char actorType='W';

	//constant vars.
	int NORMAL=1;
	int HIGHLIGHTED=2;
	int PRESSED=3;
	int DISABLED=4;

	int currentButtonState;

	Text textObject;

	// Use this for initialization
	void Start () {

		//Setting the text object.
			textObject= gameObject.transform.GetChild(0).GetComponent<Text>();

		//Setting color values unique to each actor.
			if(actorType=='W')
			{
				normal_fontColor= new Color32(28,61,128,255);
				highlighted_fontColor= new Color32(225,225,180,255);
				pressed_fontColor= new Color32(56,101,80,255);
				disabled_fontColor= new Color32(98,92,92,255);
			}

			else if(actorType=='H')
			{
				normal_fontColor= new Color32(98,30,15,255);
				highlighted_fontColor= new Color32(225,225,180,255);
				pressed_fontColor= new Color32(141,0,109,255);
				disabled_fontColor= new Color32(98,92,92,255);
			}

			else if(actorType=='C')
			{
				normal_fontColor= new Color32(63,73,25,255);
				highlighted_fontColor= new Color32(225,225,180,255);
				pressed_fontColor= new Color32(141,97,22,255);
				disabled_fontColor= new Color32(98,92,92,255);
			}

		//Setting font color to a default: the normal color.
			currentButtonState=NORMAL;
			changeColor(NORMAL);

		Button buttonComponent=gameObject.GetComponent<Button>();
			//Adding an onclick event listener to the button.
			buttonComponent.onClick.AddListener(() => changeColor(PRESSED));

	}
	
	//Added hover events.
		public void OnPointerEnter(PointerEventData eventData)
		{
			changeColor(HIGHLIGHTED);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			//print("%%%%%%%%%%		at POINTER EXIT: CURRENT BUTTON STATE: "+currentButtonState);	
			if(currentButtonState != PRESSED)
				changeColor(NORMAL);
				
		}

		//Added disable event.
		void OnDisable() {
			changeColor(DISABLED);

		}


	//Changes the button text's color and its state.
	public void changeColor(int buttonState)
	{
		currentButtonState=buttonState;

		if(buttonState==NORMAL)
			textObject.color= normal_fontColor;

		else if(buttonState==HIGHLIGHTED)
			textObject.color= highlighted_fontColor;

		else if(buttonState==PRESSED)
			textObject.color= pressed_fontColor;

		else if(buttonState==DISABLED)
			{
				textObject.color= disabled_fontColor;
			}
	}

}
