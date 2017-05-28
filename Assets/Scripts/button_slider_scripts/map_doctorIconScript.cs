using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

//Class used to display and  update the logic of the doctor icons in health agency's facilities shuffle event.

public class map_doctorIconScript : MonoBehaviour {

	Button buttonComponent;
	Image imageComponent;

	public Color availableColor;
	public Color unavailableColor;

	bool isDoctorAvailable;

	//variable computed from the object's name.
	public int doctorIndex;

	// Use this for initialization.
	void Awake () {
		
		//Setting the button component.
		buttonComponent=gameObject.GetComponent<Button>();
		imageComponent=gameObject.GetComponent<Image>();

		//isDoctorAvailable begins set to false, so set the unavailable color.
		isDoctorAvailable=false;
		imageComponent.color=unavailableColor;


		//Adding an onclick event listener to the button.
		buttonComponent.onClick.AddListener(() => iconPressed());

		//Calculating the object's index within the displayManager's doctorButton array.
			//the number index is the last character of the object name.
		doctorIndex= int.Parse(gameObject.name.Substring(gameObject.name.Length-1));

	}

	//Callback function that handles the button's display logic. 
		//It also updates the gameManager with a new player decision.
	public void iconPressed()
	{

		//Switch color, IF there are enough doctors to deploy to that particular hospital.
		
		if(!isDoctorAvailable && newGameManager.gameManager.areThereEnoughDoctors_toAdd_toHospital())
		{
			isDoctorAvailable=true;

			imageComponent.color=availableColor;

			//Sending the event change to the displayManager.
			newGameManager.gameManager.update_H_resourceShuffle_fromButton(doctorIndex, isDoctorAvailable);
		
		}

		//Switch color of the sprite to its unavailable state.
		else if (isDoctorAvailable)
		{
			isDoctorAvailable=false;

			imageComponent.color=unavailableColor;

			//Sending the event change to the displayManager.
			newGameManager.gameManager.update_H_resourceShuffle_fromButton(doctorIndex, isDoctorAvailable);
			//print("*****	After: Doctor Icon #"+doctorIndex+": button pressed, availability now "+isDoctorAvailable+".");			

		}


	}

	//Function that resets the current button logic and graphics, called by the gameManager.
	public void setIconValue(bool doctorAvailability)
	{

		//This function is called by the displayManager,
			//not called when user presses a button.

		//if(isDoctorAvailable==doctorAvailability)
		//{} //do nothing if the state hasn't changed.
			
			isDoctorAvailable=doctorAvailability;

			if(doctorAvailability)
				imageComponent.color=availableColor;
			else
				imageComponent.color=unavailableColor;
	
	}


}
