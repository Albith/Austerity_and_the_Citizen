using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

//Class used to display and  update the logic of the water repair icons in the water agency's infrastructure repair event.

public class repairIconScript : MonoBehaviour {

	Button buttonComponent;
	Image imageComponent;

	string unpressedSprite_name="";
	string pressedSprite_name="";

	bool isButtonPressed=false;

	// Use this for initialization
	void Start () {
		
		//Setting the button component.
		buttonComponent=gameObject.GetComponent<Button>();
		imageComponent=gameObject.GetComponent<Image>();

		//Adding an onclick event listener to the button.
		buttonComponent.onClick.AddListener(() => iconPressed());

		unpressedSprite_name="brokenPipeIcon_2";
		pressedSprite_name="brokenPipeIcon_2_selected";

	}

	//Callback function that handles the button's display logic. 
		//It also updates the gameManager with a new player decision.
	public void iconPressed()
	{

		//Switch sprite, IF there are enough personnel to repair.
		
		if(!isButtonPressed && newGameManager.gameManager.areThereEnoughPersonnel_ToRepair())
		{
			isButtonPressed=true;
			
			imageComponent.sprite=Resources.Load<Sprite>(pressedSprite_name);
	
			//Sending the event change to the displayManager.
			newGameManager.gameManager.update_W_InfrastructureRepair(gameObject.name, isButtonPressed);
		}

		//Switch sprite to its unpressed state.
		else if (isButtonPressed)
		{
			isButtonPressed=false;

				imageComponent.sprite=Resources.Load<Sprite>(unpressedSprite_name);

			//Sending the event change to the displayManager.
			newGameManager.gameManager.update_W_InfrastructureRepair(gameObject.name, isButtonPressed);

		}


	}
	
	//Function that resets the current button logic and graphics, called by the gameManager.
	public void resetIconSprite()
	{
		//resets icon button to its unpressed state.
		isButtonPressed=false;
		imageComponent.sprite=Resources.Load<Sprite>(unpressedSprite_name);

	}


}
