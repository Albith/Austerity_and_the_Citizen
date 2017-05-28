using UnityEngine;
using System.Collections;
//using System.Collections.Generic;
using UnityEngine.UI; // Required when Using UI elements.


public class sliderEffectScript: MonoBehaviour {

	//A general script used to fetch information from an in-game slider to the gameManager.
		//It sends the delta and latest value read from the slider.
		//This generic method is sent over to the gameManager for parsing.

	public string effectType="WBill";
	public int initialValue=0;
	//to use this add first a slider to your canvas in the editor.
	Slider sliderComponent; //attach this to the slider gameobject in  the editor or use Gameobject.Find

	//Important: this is used to send over the delta of the value change.
		//Since we are incrementing values in a game variable, we have to send the delta, not the actual value.
		float oldValue=0f;
		float newValue=0f;
		float deltaValue=0f;

	void Start(){
		//initializing the slider object.
		 sliderComponent = gameObject.GetComponent<Slider>();
		 sliderComponent.value=initialValue;
		 oldValue=initialValue;

		 //Assigning the callback method when the slider is changed.
		  sliderComponent.onValueChanged.AddListener(delegate{sliderValueChanged();});
 	
		}

	public void resetSlider()
	{
		sliderComponent.value=initialValue;
		oldValue=initialValue;
	}

	 public void sliderValueChanged()
		{
		

			//Update the value on the gameManager, and also on the DisplayManager.
				//Water bill, increases funds for wagency.
						//decreases citizen funds.
						//at High levels, decreases water availability for citizen.
						//at High levels, decreases funds for health agency.

			//Once the user clicks the submit button, the final value set gets submitted.
				
				newValue=sliderComponent.value;

				deltaValue=newValue-oldValue;

				//sending the delta Value of slider moves.
				if(deltaValue != 0f)
					newGameManager.gameManager.updateGameValuesBySlider(deltaValue, newValue, effectType);

				oldValue=newValue;

		}		


}
