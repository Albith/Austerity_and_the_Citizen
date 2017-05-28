using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


//This class handles the display logic for the game.  
	//It updates the display with the game variables' latest values.
	//It also handles all the animations, as well as
	//managing the display logic (it hides and shows events, as requested by the game manager.)

public class newDisplayManager : MonoBehaviour {

	//The following letters will be used to describe display components:
		//W= water agency.
		//H= health agency.
		//C= citizen.

//Enumeration element to manage objects in the user interface.
		public enum DisplayFields
		{
			WATER_LEVEL=0,
			WATER_FUNDS,
			WATER_MAINTENANCE,
			CITIZEN_HEALTH,
			CITIZEN_MEDS,
			CITIZEN_D_ACCESS,
			CITIZEN_W_ACCESS,
			CITIZEN_FUNDS,
			HEALTH_DOCTORS,
			HEALTH_FUNDS,
			HEALTH_FACILITIES
		}


//A.--Variables used to access and set the display.

	//STATIC UI elements.
	public bool isAnimatingImageHeight=true;

	//These are all interface components, no game logic here.
		//0.The Starting Window and grey panels.
			public GameObject startPanel;
			public GameObject nextTurnPanel;
			public GameObject[] greyPanels;
			
			//Introduction screens and 
				//panels used to darken the display.
			public CanvasGroup[] darkPanel_screens;
			public CanvasGroup[] introductory_screens;
			public int current_introScreen=0;

			//Keeping track of the previous event called, just to coordinate events being displayed.
					string previousEvent="";
					string lastWaterEvent="";
					string lastCitizenEvent="";
					string lastHealthEvent="";

		//1.UI display parameters to change.
			
			//Water agency parameters.
					public GameObject WaterSprite;	//controls the water height.
					public Image[] Water_cashSprites;		//controls the display of the sprites used for funds.
					public Image[] Water_maintenanceSprites; //in case I add the maintenance water workers.

						//Colors needed for:
							//waterLevel changes.
							public	Color water_normalColor;
							public	Color water_warningColor;
							public	Color water_criticalColor;

							//Cash sprites: no color info needed, 
								//sprites are simply switched.

							//Maintenance sprites: on and off.
							public	Color water_crewAvailable_color;
							public	Color water_crewAtWork_color;	
								
						//Also: previous amount vars.
							int previousWaterStatus=3;
							int previous_WcashSprites_number=0;
							int previous_WmaintenanceSprites_number=-1;

					//Repair event Sprites.
					public Image[] W_RepairSprites;
					//public Image[] W_waterScarcity_Sprites;

			//Health agency parameters.
				//Including newer UI parameters.
					public Image[] Health_doctorSprites;		//controls the display of the sprites used for funds.
					public Image[] Health_cashSpritesH;		//controls the display of the sprites used for funds.

						//Colors needed for:
							//doctor States: on and off.
							public	Color health_doctorAvailable_color;
							public	Color health_doctorMissing_color;	

							//Cash sprites: with cash (green), without cash(dark+transparent)
								//Note: these variables are already set, above.

						//Also: previous amount vars.
							int previous_HdoctorSprites_number=0;
							int previous_HcashSprites_number=0;
			
					
					//Health resource Shuffle-event Sprites.
					public Image[] Hshuffle_hospitalSprites;
					public map_doctorIconScript[] Hshuffle_hospitalObjects;
					public Text Hshuffle_messageText;
						public Color hospitalClosed_Color;
					//public Image[] Hshuffle_arrowSprites;  //Maybe arrowSprites don't have to be added?

			//Citizen parameters.
				//Including newer UI parameters.			
					public GameObject Citizen_HealthSprite;
					public GameObject Citizen_HealthOutline_sprite;
						public Color C_outline_normalColor;
						public Color C_outline_alertColor;
					public CanvasGroup[] Citizen_medSprites;
					public Image[] Citizen_doctorAccessSprites;
					public Image[] Citizen_waterAccesSprites;
					public Image[] Citizen_pennySprites;

						//Colors needed for:
							//health Sprite: color coding for state of the character.
								public Color citizen_maxHealth_color;
								public Color citizen_minHealth_color;								
								public int citizenHealth_displayType=3;

							//Med sprites: color coding for pill outline, pill background (on and off state)
								//no colors needed, we're turning sprites on and off.										

							//Water access sprites: color coding for on and off state.
								public Color citizen_waterAccess_color;
								public Color citizen_noWaterAccess_color;							

							//Doctor access sprites: color coding for on and off state.
								public Color citizen_doctorAccess_color;
								public Color citizen_noDoctorAccess_color;							

							//Cash sprites: with cash (green), without cash(dark+transparent)
								//no color needed, turing coin sprites on and off.						

						//Also: previous amount vars.
							float previous_CitizenHealth=-1f;
							int previous_Cmeds_amount=-1;
							int previous_CwaterAccess_number=-1;
							int previous_CdoctorAccess_number=-1;
							int previous_Cpennies_amount=-1;

						//booleans to check for critical condition (low health or low funds.)
							bool isCHealthLow=false;
							bool areCFundsLow=false;
							bool isCLowHealth_anim_running=false;
							bool areCLowFunds_warning_running=false;


			//Variables used to check whether to perform highlight animations.
				//The first array always is set to all values false at the start of the turn.
				//As values are updated, this booleans may be toggled on.
				bool[] isPerforming_HighlightAnimations_Array;
				float highlightAnimDuration=1f;
					//Used to loop color changes in Elements.
					bool isWaterSpriteLooping=false;
					//bool isCitizenSpriteLooping=false;
					//This array contains less elements (2 less) than the boolean array.
					//No water or retiree sprites. Those are accessed by their sprite variables.
					public CanvasGroup[] screenHighlightableAreas;
					//used for toggling off highlight animations.
					bool isGameStarting;


			//Timestep (month) display vars.
				public GameObject currentMonth_ticker;
				public Color default_tickerColor;
				public Color higlight_tickerColor;
				float timeDisplay_ticker_startPosX=-243f;
				float ticker_moveSpeed=0.4f;
				float ticker_moveOffset=30.75f;

				float timeDisplay_colorAnim_speed=0.8f;
				public Color timeDisplay_highlightedColor;
				public Color timeDisplay_brightHiglightColor;
				public Color month_defaultColor;
				public Color week_defaultColor;

				int currentWeek=0;
				public Image[] timeMarkers;

//B.--->--General methods common to the displays are shown here.

//This method is used to correct a display bug in Unity, 
	//and also for initializing some important variables.
void Awake()
{
	//for toggling off the highlight animation.
	isGameStarting=true;

	//currentMonth_ticker.GetComponent<Image>().color=higlight_tickerColor;

	print("newDisplayManager: Start() function.");
	//0. Defining and setting my animation check array.
		 isPerforming_HighlightAnimations_Array= new bool[11];
		 //First 3 [0-2] are for Water vars. (even if less are used)
		 //Next 5 [3-7] are for Citizen vars. (even if less are used)
		 //Last 3 [8-10] are for Health vars. (even if less are used)
		 for (int i = 0; i < isPerforming_HighlightAnimations_Array.Length; i++)
		 	isPerforming_HighlightAnimations_Array[i]=false;

	//Running a function for all buttons.
		//A hack' so previously clicked choices do not reappear as higlighted.

	 Button[] gameButtons = (Button[]) GameObject.FindObjectsOfType (typeof(Button));

	 foreach (Button currentButton in gameButtons)
 	{
 		currentButton.onClick.AddListener(() => { unhoverButton(currentButton); }); 
 	}


}

public void unhoverButton(Button thisButton)
{
			  

			  if(thisButton.tag == "specialButton")
			  {

				print(">>>>>>>>>> CALLING DISABLE COLOR SET----.");
				//reset the button text's color to Normal.
				thisButton.GetComponent<blankButtonScript>().changeColor(3);
			  }

			  else 
			  {
				  thisButton.enabled=false;
			  	  thisButton.enabled=true;
			  }

}

//0.---2 functions for updating the Timestep display.

public void resetMonthDisplay()
{

	//reset the counter.
	currentWeek=0;
	//color the first turn with highlight.
	timeMarkers[currentWeek].color=timeDisplay_highlightedColor;

	RectTransform monthTicker_transform=currentMonth_ticker.GetComponent<RectTransform>();
	//Move the ticker back to the default position.
	monthTicker_transform.localPosition= 
				new Vector3(
					timeDisplay_ticker_startPosX,
					monthTicker_transform.localPosition.y,
					monthTicker_transform.localPosition.z
							);

	//Reset color highlights.
		//Ticker back to default.
		currentMonth_ticker.GetComponent<Image>().color= default_tickerColor;

		//All markers go back to default colors.
		for(int i=0; i<timeMarkers.Length; i++)
		{
			if(i % 4==0 && i>0)
				timeMarkers[i].color= month_defaultColor;
			else
				timeMarkers[i].color= week_defaultColor;
		}

}

public void updateMonthDisplay()
{
	//Advance the ticker to the next position.
		//Play animation: ticker moves.
		//Play animation: month color changes.

	StartCoroutine(updateMonthDisplay_sequence(ticker_moveSpeed));

}


//1.---Parameter display methods go here.

	//---Water Agency display methods.
		void update_WaterLevel_Display()
		{

			//Get the latest value, compare it to the new value.
			float latestWaterLevel=newGameManager.gameManager.wagency.waterLevel;

			//1. Update the Water Level Display.
					//Modifying the waterSprite.
							//Max height value (100%)= 63.2
							//Min height value (0%)= 3.5
							float waterPercentage= latestWaterLevel/20f;	
							//set water height in the sprite.
							RectTransform WaterSprite_transform= WaterSprite.GetComponent<RectTransform>();
							if(isAnimatingImageHeight)
							{
								float endHeight=Mathf.Lerp(3.5f, 63.2f, waterPercentage);
								float startHeight= WaterSprite_transform.rect.height;
								StartCoroutine(animate_Water_heightChange(startHeight, endHeight, 0.8f));

							}

							else
							{
								//Setting new image height directly, no animation needed.
								WaterSprite_transform.sizeDelta= 
									new Vector2(WaterSprite_transform.rect.width, 
												Mathf.Lerp(3.5f, 63.2f, waterPercentage) );
							}
			//2. Check whether to show a color animation, 
				//to show a status change.
				int latestWaterStatus=0;

					if(latestWaterLevel<=newGameManager.gameManager.wagency.criticalWaterLevel)
						latestWaterStatus=1;
					else if(latestWaterLevel<=newGameManager.gameManager.wagency.warningWaterLevel)
						latestWaterStatus=2;
					else
						latestWaterStatus=3;

				if(latestWaterStatus != previousWaterStatus && latestWaterStatus !=3 )
					{	//only highlighting when dropping to a warning or critical water level.
						if(!isGameStarting)
						isPerforming_HighlightAnimations_Array[0]=true; //0 refers to the waterLevel display onscreen.
					}

			//3. Finally, update the previousWaterStatus variable.
				previousWaterStatus= latestWaterStatus;
		}


		void update_WaterFunds_Display( )
		{
			//Get the funds amount.
			float latestWater_funds=newGameManager.gameManager.wagency.funds;

			//Compute the icons to show, by dividing the funds into 12 parts.
			int latest_WcashSprites_number=-1;
			float temp=latestWater_funds/2f;

			//Approximating 9.8<-->9.99 to 10f
			if(temp>=9.8f && temp <10f)
				temp=10f;

			latest_WcashSprites_number= (int)(temp);

			//Check if the sprites visible have changed.
			if(latest_WcashSprites_number != previous_WcashSprites_number)
			{
				//*The icons that are not shown are replaced by a cube icon.

				//if the amount is odd (1,3,5,7) , show the bottom money icon 
					//and vanish the top cashSprite.

				//if the amount is even (2,4,6,8) (and less than the total), replace the bottom money icon with a cube 
					//and vanish the top cashSprite.

				//render the dollar image in these.
				int numberActualCash=latest_WcashSprites_number;

					for(int i=0; i<numberActualCash; i++)
					{
						if(!Water_cashSprites[i].enabled)
							Water_cashSprites[i].enabled=true;

						Water_cashSprites[i].GetComponent<Image>().sprite= Resources.Load<Sprite>("moneyStack3");
						
					}
				
				int numberMissingCash=10-latest_WcashSprites_number;

				int numberVanishedSprites=0;

				if(numberActualCash < 10 && numberActualCash % 2 !=0)
					numberVanishedSprites=1;

				//If there's an odd number of cash sprites, hide the sprite directly after.
				if(numberVanishedSprites>0)	
				{
					if(Water_cashSprites[numberActualCash].isActiveAndEnabled)
						Water_cashSprites[numberActualCash].enabled=false;

				}

				//Lastly, if there's an even amount of missing cash, show the block sprite.
				if(numberMissingCash>1)
				{

					for(int i=numberActualCash+numberVanishedSprites; i<10; i++)
					{
						if(i%2 ==0)  //show the block sprite at the even-numbered block.
						{
							if(Water_cashSprites[i].isActiveAndEnabled)
								Water_cashSprites[i].enabled=false;

						}

						else  //hide the odd-numbered block.
						{
							if(Water_cashSprites[i].isActiveAndEnabled)
								Water_cashSprites[i].enabled=false;

						}

					}

				}//end of block sprite loop.

			//Also, updating the animation check array.
				//1 refers to the water agency's funds.
				if(!isGameStarting)
					isPerforming_HighlightAnimations_Array[1]=true; 

			//Finally, update the previous-status variable.
				previous_WcashSprites_number= latest_WcashSprites_number;

			}// end of image-updating code.

		}

		//Display method for the repair event.
		void update_Water_MaintenanceCrew_Display()
		{
					//Get the maintenance crew parameters amount.
					int latest_WmaintenanceSprites_number=newGameManager.gameManager.wagency.numPersonnel;
					int total_number_WmaintenanceSprites=newGameManager.gameManager.wagency.numTotalPersonnel;

					//there are 6 maintenance crews, no need to approximate this value.

					//Checking the range for this value.
					if(latest_WmaintenanceSprites_number<0)
						latest_WmaintenanceSprites_number=0;
					if(latest_WmaintenanceSprites_number>total_number_WmaintenanceSprites)
						latest_WmaintenanceSprites_number=total_number_WmaintenanceSprites;


					if(latest_WmaintenanceSprites_number != previous_WmaintenanceSprites_number)
					{

						//Updating the values.
						//First loop puts in the 'maintenance crew are available' icon color.
						for(int i=0; i<latest_WmaintenanceSprites_number; i++)
						{
							if(Water_maintenanceSprites[i].color.r != water_crewAvailable_color.r )
								Water_maintenanceSprites[i].color=water_crewAvailable_color;

						}
				

						//Second, if there are maintenance crew not available, display them. 
						if(latest_WmaintenanceSprites_number<total_number_WmaintenanceSprites)
						{

							for(int i=latest_WmaintenanceSprites_number; i<total_number_WmaintenanceSprites; i++)
							{
								
							if(Water_maintenanceSprites[i].color.r != water_crewAtWork_color.r )
								Water_maintenanceSprites[i].color=water_crewAtWork_color;
							
							}	//end of maintenance crew sprite hiding loop.

						}			

						//Also, updating the animation check array.
							//2 refers to the maintenance crew.
							if(!isGameStarting)
								isPerforming_HighlightAnimations_Array[2]=true; 

						//Finally, update the previous status variable for this display.	
						previous_WmaintenanceSprites_number= latest_WmaintenanceSprites_number;
					
					}//end of if statement that checks for a change in sprite numbers.
	
		}

	//---Citizen actor methods.
		//Called every time game logic changes actor values.
			//Can also update the citizen's two access displays.
	
		void update_Citizen_Health()
		{
			//Try changing the height, and the color.

			//Get the latest value, compare it to the new value.
			float latest_CitizenHealth=newGameManager.gameManager.retiree.health;

			if(latest_CitizenHealth != previous_CitizenHealth)
			{

				//A. Changing the height.
					//Modifying the amount of health.
							//Max height value (100%)= 100
							//Min height value (0%)= 0
							float healthPercentage= latest_CitizenHealth*5f;  //set to a 100%.	
							//set health height in the sprite.
							if(citizenHealth_displayType==1 || citizenHealth_displayType==3)
							{
								RectTransform ChealthSprite_transform= Citizen_HealthSprite.GetComponent<RectTransform>();
							
									if(isAnimatingImageHeight)
									{
											float endHeight=healthPercentage;
											float startHeight= ChealthSprite_transform.rect.height;
											StartCoroutine(animate_Citizen_heightChange(startHeight, endHeight, 0.6f));

									}

									else
									{
										//Setting new image height directly, no animation needed.
										ChealthSprite_transform.sizeDelta= 
										new Vector2(ChealthSprite_transform.rect.width, 
													healthPercentage); 	//if a different range: Mathf.Lerp(3.5f, 63.2f, healthPercentage) );
									}
							
							}

				//B. Changing the color.
							if(citizenHealth_displayType==2 || citizenHealth_displayType==3)
							{	
									//animating the color change.
									if(isAnimatingImageHeight)
									{
										Color endColor= Color.Lerp(citizen_minHealth_color, 
																citizen_maxHealth_color, 
																healthPercentage*0.01f);
										StartCoroutine(	
														fade_Image_ToColor(Citizen_HealthSprite.GetComponent<Image>(), 
														endColor, 0.6f) 
													  );

									}
									else
									{
											Citizen_HealthSprite.GetComponent<Image>().color=
											Color.Lerp(citizen_minHealth_color, 
													citizen_maxHealth_color, 
													healthPercentage*0.01f);
									}
							}
				
				//Extra: Check whether to show a color animation, 
						//to show a status change.
						//in the case of the health parameter, 
						//perform a flashing animation when retiree has low health.

						print("$$$$$$$$$$	HEALTH PERCENTAGE is: "+healthPercentage+".");

						if(healthPercentage <= 20f && !isCHealthLow)
							{
								isCHealthLow=true;
								isPerforming_HighlightAnimations_Array[3]=true;
							}
						else if(healthPercentage >20f && isCHealthLow)
							{
								//toggle vars off.
								//Stop flashing Coroutine.

								isCHealthLow=false;
								if(isCLowHealth_anim_running)
									isCLowHealth_anim_running=false;
									//This will end the flashing animation.	
							}
						print("$$$$$$$$$$	HEALTH CHECK, isCHealthLow: "+isCHealthLow+".");


					//C. Finally, update the previous health status variable.
						previous_CitizenHealth= latest_CitizenHealth;

				}


		}

		void update_Citizen_Medicine_Display()
		{

			int latest_numberOfMeds= newGameManager.gameManager.retiree.medicationsAmount;

			if(latest_numberOfMeds != previous_Cmeds_amount)
				{
		
					if(latest_numberOfMeds==3)
						{
							Citizen_medSprites[1].alpha=1f;
							Citizen_medSprites[2].alpha=1f;

						}

					if(latest_numberOfMeds==2)
						{
							Citizen_medSprites[1].alpha=1f;
							Citizen_medSprites[2].alpha=0f;

						}

					if(latest_numberOfMeds==1)
						{
							Citizen_medSprites[1].alpha=0f;
							Citizen_medSprites[2].alpha=0f;
						}

				//Also, updating the animation check array.
					//4 refers to the citizen's water access..
					if(!isGameStarting)
						isPerforming_HighlightAnimations_Array[4]=true; 		

				//Finally, update the previous state variable.
					previous_Cmeds_amount= latest_numberOfMeds;			

				}

		
		}

		void update_Citizen_Access_Displays(bool isWaterAccess)
		{
			
			//Get the access amount.
			int latest_Access_number;
			int previous_Access_number;

			if(isWaterAccess)	//Water Access
				{
					latest_Access_number= newGameManager.gameManager.retiree.drinkingWaterAccess;
					previous_Access_number= previous_CwaterAccess_number;

				}
			else				//Doctor Access
				{
					latest_Access_number= newGameManager.gameManager.retiree.accessToDoctor;
					previous_Access_number= previous_CdoctorAccess_number;
				}
					//quick range check.
					if(latest_Access_number>3)
						latest_Access_number=3;
					if(previous_Access_number<1)
						previous_Access_number=1;

			if(latest_Access_number != previous_Access_number)
			{

				//Updating the values.

					if(!isWaterAccess)		//Doctor access variables.
					{	

						if(latest_Access_number==3)
							{
								//all doctor images are set to the black.
								Citizen_doctorAccessSprites[1].color=citizen_doctorAccess_color;
								Citizen_doctorAccessSprites[2].color=citizen_doctorAccess_color;
							}

						if(latest_Access_number==2)
							{
								//set 3rd image to transparent, 2nd set to black.
								Citizen_doctorAccessSprites[1].color=citizen_doctorAccess_color;
								Citizen_doctorAccessSprites[2].color=citizen_noDoctorAccess_color;

							}

						if(latest_Access_number==1)
							{
								//set both images to have a transparent color.
								Citizen_doctorAccessSprites[1].color=citizen_noDoctorAccess_color;
								Citizen_doctorAccessSprites[2].color=citizen_noDoctorAccess_color;
							}
						
						//Also, updating the animation check array.
							//5 refers to the citizen's doctor access..
							if(!isGameStarting)
								isPerforming_HighlightAnimations_Array[5]=true; 
			
						}//end of doctorAccess update.

					else		//Water access variables.
					{	
						if(latest_Access_number==3)
							{
								//all water images are set to the blue drop.
								Citizen_waterAccesSprites[1].color=citizen_waterAccess_color;
								Citizen_waterAccesSprites[2].color=citizen_waterAccess_color;
							}

						if(latest_Access_number==2)
							{
								//set 3rd image to opaque, 2nd set to blue.
								Citizen_waterAccesSprites[1].color=citizen_waterAccess_color;
								Citizen_waterAccesSprites[2].color=citizen_noWaterAccess_color;

							}

						if(latest_Access_number==1)
							{
								//set both images to have an opaque color.
								Citizen_waterAccesSprites[1].color=citizen_noWaterAccess_color;
								Citizen_waterAccesSprites[2].color=citizen_noWaterAccess_color;
							}
			
						//Also, updating the animation check array.
							//6 refers to the citizen's water access..
							if(!isGameStarting)
								isPerforming_HighlightAnimations_Array[6]=true; 			
			
					}//end of doctorAccess update.

				//Finally, update the tracking variables.		
					if(isWaterAccess)	//Water Access
						previous_CwaterAccess_number= latest_Access_number;
					else				//Doctor Access
						previous_CdoctorAccess_number= latest_Access_number;
			}

		}

		void update_Citizen_Coins_Display()
		{
			//Get the funds amount.
			float latest_citizenFunds=newGameManager.gameManager.retiree.funds;

			//Compute the icons to show, by dividing the funds into 8 parts.
			int latest_citizen_coinsAmount=-1;
			float temp=latest_citizenFunds/125f;

			//Approximating 7.8<-->7.99 to 8f
			if(temp>=7.8f && temp <8f)
				temp=8f;

			latest_citizen_coinsAmount= (int)(temp);

			//Check if the sprites visible have changed.
			if(latest_citizen_coinsAmount != previous_Cpennies_amount)
			{
				//*Simplified code.

				//render the dollar image in these.
				int numberActualCoins=latest_citizen_coinsAmount;

					for(int i=0; i<numberActualCoins; i++)
					{
						if(!Citizen_pennySprites[i].enabled)
							Citizen_pennySprites[i].enabled=true;
						
					}
				

				//Lastly, if there's any amount missing coins, go ahead and hide them. 
					if(numberActualCoins<8)
					{

						for(int i=numberActualCoins; i<8; i++)
						{
							
								if(Citizen_pennySprites[i].isActiveAndEnabled)
									Citizen_pennySprites[i].enabled=false;
						
						}	//end of sprite hiding loop.

					}
				

			//Also, updating the animation check array.
				//7 refers to the citizen's funds..
				if(!isGameStarting)
					{

						//Are you dangerously low on money? If so, show a warning sign.
						if(latest_citizen_coinsAmount<3 && !areCFundsLow)
							areCFundsLow=true;

						//if not, turn off the display and revert to the yellow color.
						else if(latest_citizen_coinsAmount>=3 && areCLowFunds_warning_running)
							{
								areCFundsLow=false;
								areCLowFunds_warning_running=false;

								//also, revert the coin display back to normal.	
								Image coinDisplayHighlight=
									screenHighlightableAreas[5].gameObject.GetComponent<Image>();
								coinDisplayHighlight.color= new Color32(255,203,2,255);

								screenHighlightableAreas[5].alpha=0f;

							}

						isPerforming_HighlightAnimations_Array[7]=true; 
					}
						

			//Finally, update the previous-status variable.
				previous_Cpennies_amount= latest_citizen_coinsAmount;

			}// end of image-updating code.

		}


	//Health Agency display methods.
		void update_HealthDoctors_Display()
		{

					//Get the health parameters amount.
					float latestHealth_doctorNumber=newGameManager.gameManager.hagency.numOfDoctors;

					//Compute the icons to show, by dividing the funds into 9 parts.
					int latest_HdoctorsSprites_number=-1;

					//Approximating 8.8<-->8.99 to 9f
					if(latestHealth_doctorNumber>=8.9f && latestHealth_doctorNumber <9f || latestHealth_doctorNumber>9f)
						latestHealth_doctorNumber=9f;

					latest_HdoctorsSprites_number= (int)(latestHealth_doctorNumber);

					if(latest_HdoctorsSprites_number != previous_HdoctorSprites_number)
					{

						//Updating the values.
						//First loop puts in the 'doctors are available' icon color.
						for(int i=0; i<latest_HdoctorsSprites_number; i++)
						{
							if(Health_doctorSprites[i].color.r != health_doctorAvailable_color.r )
								Health_doctorSprites[i].color=health_doctorAvailable_color;

						}
				

						//Second, if there are doctors not available, display them. 
						if(latest_HdoctorsSprites_number<9)
						{

							for(int i=latest_HdoctorsSprites_number; i<9; i++)
							{
								
							if(Health_doctorSprites[i].color.r != health_doctorMissing_color.r )
								Health_doctorSprites[i].color=health_doctorMissing_color;
							
							}	//end of doctor sprite hiding loop.

						}			

						//Also, updating the animation check array.
							//8 refers to the health agency's number of doctors..
							if(!isGameStarting)			
								isPerforming_HighlightAnimations_Array[8]=true; 		

						//Finally, update the previous status variable for this display.	
						previous_HdoctorSprites_number= latest_HdoctorsSprites_number;
					
					}//end of if statement that checks for a change in sprite numbers.
	
		}

		void update_HealthFunds_Display( )
		{
			//Get the funds amount.
			float latestHealth_funds=newGameManager.gameManager.hagency.funds;

			//Compute the icons to show, by dividing the funds into 12 parts.
			int latest_HcashSprites_number=-1;
			float temp=latestHealth_funds/2f;

			//Approximating 9.8<-->9.99 to 10f
			if(temp>=9.8f && temp <10f)
				temp=10f;

			latest_HcashSprites_number= (int)(temp);

			//Check if the sprites visible have changed.
			if(latest_HcashSprites_number != previous_HcashSprites_number)
			{
				//*The icons that are not shown are replaced by a cube icon.

				//if the amount is odd (1,3,5,7) , show the bottom money icon 
					//and vanish the top cashSprite.

				//if the amount is even (2,4,6,8) (and less than the total), replace the bottom money icon with a cube 
					//and vanish the top cashSprite.

				//render the dollar image in these.
				int numberActualCash=latest_HcashSprites_number;

					for(int i=0; i<numberActualCash; i++)
					{
						if(!Health_cashSpritesH[i].enabled)
							Health_cashSpritesH[i].enabled=true;

						Health_cashSpritesH[i].GetComponent<Image>().sprite= Resources.Load<Sprite>("moneyStack3");
						
					}
				
				int numberMissingCash=10-latest_HcashSprites_number;

				int numberVanishedSprites=0;

				if(numberActualCash < 10 && numberActualCash % 2 !=0)
					numberVanishedSprites=1;

				//If there's an odd number of cash sprites, hide the sprite directly after.
				if(numberVanishedSprites>0)	
				{
					if(Health_cashSpritesH[numberActualCash].isActiveAndEnabled)
						Health_cashSpritesH[numberActualCash].enabled=false;

				}

				//Lastly, if there's an even amount of missing cash, show the block sprite.
				if(numberMissingCash>1)
				{

					for(int i=numberActualCash+numberVanishedSprites; i<10; i++)
					{
						if(i%2 ==0)  //show the block sprite at the even-numbered block.
						{
							if(Health_cashSpritesH[i].isActiveAndEnabled)
								Health_cashSpritesH[i].enabled=false;

							//Health_cashSpritesH[i].GetComponent<Image>().sprite= Resources.Load<Sprite>("moneyStack3_bare");

						}

						else  //hide the odd-numbered block.
						{
							if(Health_cashSpritesH[i].isActiveAndEnabled)
								Health_cashSpritesH[i].enabled=false;

						}

					}

				}//end of block sprite loop.

			//Also, updating the animation check array.
				//9 refers to the health agency's funds..
				if(!isGameStarting)	
					isPerforming_HighlightAnimations_Array[9]=true; 		

			//3. Finally, update the previous-status variable.
				previous_HcashSprites_number= latest_HcashSprites_number;

			}// end of image-updating code.

		}

//Finally, a big update function that calls the previous methods.
	public void updateParameterDisplays()
	{

	//--->Icon-based parameter updates.
			//Water.
					//Update the Water Level.
					update_WaterLevel_Display();
					//Update the Cash sprites.
					update_WaterFunds_Display();
					//Update the Maintenance sprites.
					update_Water_MaintenanceCrew_Display();


			//Citizen.
				//update Health.
					update_Citizen_Health();
				//Updating the medicine sprites.
					update_Citizen_Medicine_Display();	
				//false= updating the Doctor Access sprites.
					update_Citizen_Access_Displays(false);
				//true= updating the Water Access sprites.
					update_Citizen_Access_Displays(true);
				//Updating the pennies sprites.
					update_Citizen_Coins_Display();


			//Health.
					//Update the amount of doctors display.
					update_HealthDoctors_Display();
					//Update the Cash sprites.
					update_HealthFunds_Display();
					//If you add the facilities section, it would be updated here.


			//Check whether to perform animations.
					//The rule is, if there was a value change reflected in the display,
					//trigger a highlight animation for that parameter.
					checkWhetherToPerformAnimations();		

		}

//1b.--Highlight animation functions.
		void checkWhetherToPerformAnimations()
		{

			//Loop through the highlight animation check array.
				//If the animation performance variable is set to true,
				//Call the highlight animation for that paramater.

			for (int i=0; i<isPerforming_HighlightAnimations_Array.Length; i++)
			{
				if(isPerforming_HighlightAnimations_Array[i])
					{
						StartCoroutine(performHighlightAnimation(i));
					}
			}

			//There may be other kinds of animations here,
				//so we shall check.

			//Finally,
				//reset all animation check booleans to false before starting the next turn.
		 		for (int i = 0; i < isPerforming_HighlightAnimations_Array.Length; i++)
		 			{
						 isPerforming_HighlightAnimations_Array[i]=false;
					}
		}

		IEnumerator performHighlightAnimation(int whichElement)
		{

			if(whichElement==0)
			{
				print("&&&	WATER LEVEL: COLOR ANIMATION START: "+previousWaterStatus+".");

				Color highlightColor;
				
				//Animate the Water Level sprite.
				//checking the water status variable.
				if(previousWaterStatus==2)
					highlightColor= water_warningColor;
				if(previousWaterStatus==2)
					highlightColor= water_criticalColor;
				else
					highlightColor=water_normalColor;
				
				//Perform the color highlight.

				Image waterSpriteImage=WaterSprite.GetComponent<Image>();
			
				StartCoroutine(fade_Image_ToColor(waterSpriteImage, highlightColor , 0.3f ));//0.6f);
				yield return new WaitForSeconds(0.5f);
				StartCoroutine(fade_Image_ToColor(waterSpriteImage, water_normalColor , 0.3f));//0.6f);					

			}

			else if(whichElement==3)
			{
				//Animate the Retiree sprite.
				print("$$$$$$$$	Low Health, flash the retiree sprite.");


				if(isCHealthLow && !isCLowHealth_anim_running)
				{
					print("$$$$$$$$	Low Health, starting the coroutine.");
					isCLowHealth_anim_running=true;
					StartCoroutine(fade_CitizenImage_ToColor_Loop(C_outline_alertColor, 0.8f, 0.8f));

				}

			}

			else{

					//Animating another parameter
						//in the canvas group array.

					int elementToAnimate=whichElement;

					if(whichElement<3)
						elementToAnimate--;

					else if(whichElement>3)
						elementToAnimate -=2;

					//Checking if funds display is being called, and coins are low.
					if(whichElement == 7 && areCFundsLow && !areCLowFunds_warning_running) 
						{
							areCLowFunds_warning_running=true;

							//fetch the image component of the highlightable area.
							Image coinDisplayHighlight=
								screenHighlightableAreas[elementToAnimate].gameObject.GetComponent<Image>();
							coinDisplayHighlight.color= new Color32(255,62,62,255);

							//turn on the highlightable canvas.
							screenHighlightableAreas[elementToAnimate].alpha=1f;

						}

					else
						{
							fadeIn_CanvasGroup(screenHighlightableAreas[elementToAnimate], 0.1f );//0.6f);
							yield return new WaitForSeconds(0.7f);
							fadeOut_CanvasGroup(screenHighlightableAreas[elementToAnimate], 0.3f);//0.6f);					
						}
				}

		} //end of PerformHighlight coroutine.

//Every button choice highlights something.

//Put a red highlight when funds or health are low.



//End of 1b.--Animation functions.

		//a helper function that formats game parameters in 2 ways:
			//Computes a percentage value based on 20 as 100%.
			//Computes a floating point value to an integer estimate.
			//Returns a string with the final value and a percentage sign.
		string getPercentageString(float value)
		{

			//1. Computing a percentage based on 20 as the max value.
			float percentage= value/20.0f*100;

			//2. Computes an integer estimate of the percentage.
			int estimate= Mathf.RoundToInt(percentage);

			//Returning the estimate as a string, with a percentage sign. 
			return estimate+"%";

		}



//C. Special event display functions go here.  These are unique because they relate to display information that 
	//fades in and out.

//----Begin Game Display() function pair.
		public void beginGameDisplay(string firstEvent)
		{
			
			//toggling off the highlight animation display, at the start.
				isGameStarting=true;

				//Toggling off other animation markers.
					isCHealthLow=false;
					areCFundsLow=false;
					isCLowHealth_anim_running=false;

					if(!areCLowFunds_warning_running)  //turn off the red display.
					{
						Image coinDisplayHighlight=
								screenHighlightableAreas[5].gameObject.GetComponent<Image>();
						coinDisplayHighlight.color= new Color32(255,203,2,255);
					}


					areCLowFunds_warning_running=false;

			//resetting an animation display array.
			for (int i = 0; i < isPerforming_HighlightAnimations_Array.Length; i++)
				isPerforming_HighlightAnimations_Array[i]=false;

			//update the slider values, back to their default values.
			reset_gameSliders();

			//Updating the current Turn display, before the rest of the parameters are updated.
			updateTurnDisplay();

			StartCoroutine(beginGameDisplay_coroutine(firstEvent));

		}

		//Method that resets the gameSliders in the game.
		void reset_gameSliders()
		{

				GameObject[] gameSliders= GameObject.FindGameObjectsWithTag("gameSlider");

				for(int i=0; i<gameSliders.Length; i++)
				{
					gameSliders[i].GetComponent<sliderEffectScript>().resetSlider();

				}


		}
		
		//3.13 Using a coroutine instead.
		IEnumerator beginGameDisplay_coroutine(string firstEvent) {

			if(!newGameManager.gameManager.isTestingEvents)
				{
					fadeOut_CanvasGroup(introductory_screens[current_introScreen], 0.3f);					
				}

            yield return new WaitForSeconds(0f);//0.1f);

			//Fade the 'next turn' panel in and out.  
			fadeIn_Event("nextTurn", 0.1f );//0.6f);
			updateParameterDisplays();		
            yield return new WaitForSeconds(1.5f);
			fadeOut_Event("nextTurn", 0.1f);//0.6f);					

			//Finally, display the first event.
			displayEvent(firstEvent);

			//One more thing: toggling on the highlight animation display, for the rest of the game session.
				isGameStarting=false;

        }

	void updateTurnDisplay()
	{
		//3.13 Update the currentTurn displayed in the Next Turn panel.
			if(newGameManager.gameManager.currentTurnCounter >= newGameManager.gameManager.maxTurns)
				nextTurnPanel.transform.GetChild(0).GetComponent<Text>().text=
				"now starting...\nthe Last Month";

			else 
			nextTurnPanel.transform.GetChild(0).GetComponent<Text>().text=
				"now starting...\nMonth "+newGameManager.gameManager.currentTurnCounter;
	}


	//A similar function for resetting the display. This merely brings up the startPanel.
	public void showStartPanel()
	{

			//resetting intro screen counter variable.
			current_introScreen=0;

			fadeIn_Event("startWindow", 0.1f ); //0.6f);	

			//Also turning on the dark panels, if they're off.
				darkPanel_screens[0].alpha=1f;
				darkPanel_screens[1].alpha=1f;
				darkPanel_screens[2].alpha=1f;


	}

//Functions that display information only when advancing a turn.
	//(A turn advances when W-C, H-C have acted, for a total of 4 events)

		//Note: These functions are the same as the beginGameDisplay() ones above.
			//Consider merging, unless you will display an introductory event 
			//below, at the start of each new turn.

		//As done with the beginGameDisplay() function, I'm creating a function pair here.
		public void displayTurnAdvance(string newTurnFirstEvent_Name)
		{

			//disable the visible events' buttons.
			enableVisibleEvents(false);

			//Updating the current Turn display, before the rest of the parameters are updated.
			updateTurnDisplay();

			//call a Sequence.
			StartCoroutine(displayTurnAdvance_Sequence(newTurnFirstEvent_Name));

		}

		IEnumerator displayTurnAdvance_Sequence(string newTurnFirstEvent_Name) 
		{

			//Fade the Next Turn Message in and out.

			//3.14, To do later: Go to an explanatory panel, going over what has possibly changed.

			//Go to the next event.

            yield return new WaitForSeconds(0f);//0.1f);

			//Fade the 'next turn' panel in and out.  
			fadeIn_Event("nextTurn", 0.1f );//0.6f);
            yield return new WaitForSeconds(1.5f);
			fadeOut_Event("nextTurn", 0.1f);//0.6f);					

            yield return new WaitForSeconds(0.1f);
			updateParameterDisplays();		


			//Finally, display the first event.
			displayEvent(newTurnFirstEvent_Name);

			


        }


		void enableVisibleEvents(bool isEnabling)
		{

					//Fetching the last events for all actors.			
							//1.Last Water Event.
							CanvasGroup currentWaterEvent= 
										GameObject.FindWithTag(lastWaterEvent).GetComponent<CanvasGroup>();

							//2.Last Citizen Event.
							CanvasGroup currentCitizenEvent= 
										GameObject.FindWithTag(lastWaterEvent).GetComponent<CanvasGroup>();

							//3.Last Health Event.
								CanvasGroup currentHealthEvent= 
											GameObject.FindWithTag(lastWaterEvent).GetComponent<CanvasGroup>();

					if(isEnabling)
					{
							//enabling all actors.				
									currentWaterEvent.interactable=true;
									currentWaterEvent.blocksRaycasts=true;
									currentWaterEvent.ignoreParentGroups=true;
							
									currentCitizenEvent.interactable=true;
									currentCitizenEvent.blocksRaycasts=true;
									currentCitizenEvent.ignoreParentGroups=true;
						
									currentHealthEvent.interactable=true;
									currentHealthEvent.blocksRaycasts=true;
									currentHealthEvent.ignoreParentGroups=true;

					}

					else
					{
							//enabling all actors.				
									currentWaterEvent.interactable=false;
									currentWaterEvent.blocksRaycasts=false;
									currentWaterEvent.ignoreParentGroups=false;
							
									currentCitizenEvent.interactable=false;
									currentCitizenEvent.blocksRaycasts=false;
									currentCitizenEvent.ignoreParentGroups=false;
						
									currentHealthEvent.interactable=false;
									currentHealthEvent.blocksRaycasts=false;
									currentHealthEvent.ignoreParentGroups=false;

					}

		}		//end of enableVisibleEvents();


//Function called at end-of-game.
		//This function is usually called at the end of the game, 
			//it hides all the previously active events so that the game can reset.
		public void hideAll_ActorEvents()
		{
			//Getting all the most recent event canvases, according to our variables.
			
			//Fetching the last events for all actors.			
					//1.Last Water Event.
					CanvasGroup waterEvent_toHide= 
								GameObject.FindWithTag(lastWaterEvent).GetComponent<CanvasGroup>();
			
						
						waterEvent_toHide.alpha=0f;

						waterEvent_toHide.interactable=false;
						waterEvent_toHide.blocksRaycasts=false;
						waterEvent_toHide.ignoreParentGroups=false;

					//2.Last Citizen Event.
					CanvasGroup citizenEvent_toHide= 
								GameObject.FindWithTag(lastCitizenEvent).GetComponent<CanvasGroup>();
			
						citizenEvent_toHide.alpha=0f;

						citizenEvent_toHide.interactable=false;
						citizenEvent_toHide.blocksRaycasts=false;
						citizenEvent_toHide.ignoreParentGroups=false;

					//3.Last Health Event.
					CanvasGroup healthEvent_toHide= 
								GameObject.FindWithTag(lastHealthEvent).GetComponent<CanvasGroup>();

						healthEvent_toHide.alpha=0f;

						healthEvent_toHide.interactable=false;
						healthEvent_toHide.blocksRaycasts=false;
						healthEvent_toHide.ignoreParentGroups=false;

		}


//D. Standard functions, the most common display functions called during gameplay.
		
    	public void prepareDisplayNextEvent(string nextTurn)
		{

			//updating the parameter levels to be shown.
			updateParameterDisplays();

			//Call the first turn to display, which means that one of the utilities' events is shown.
			displayEvent(nextTurn);

		}

		void reset_SpecialButtons_ifAny(string currentEvent)
			{

				print(">>>>>>>>>		Check for Special Buttons ");

				//if the children gameobjects have a tag 'specialButton', reset them to their normal color.
				Transform currentEvent_transform= GameObject.FindWithTag(currentEvent).transform;

				int numberOfChildren=currentEvent_transform.childCount;

				for(int i=0; i<numberOfChildren; i++)
				{

					if(currentEvent_transform.GetChild(i).tag =="specialButton")
						{
							print(">>>>>>>>			Found a Special Button.");
							currentEvent_transform.GetChild(i).GetComponent<blankButtonScript>().changeColor(1);
						}

				}


			}

		//Manages which events to hide, show, darken.  
			//Also activates and deactivates events' ability to read user input.
		void displayEvent(string currentEvent)
		{

			//Checking whether to hide the previous event object, if the previous event happenend on the same actor window
				//as the new one.
			if(previousEvent!="")
				{
					
					//Parsing the currentEvent string to determine what actor's event to show or hide.
								//If both previous and current event have the same starting letter,
									//the previous event should be hidden.

					//Fetching the previous event.
					CanvasGroup previousEvent_CanvasGroup= 
								GameObject.FindWithTag(previousEvent).GetComponent<CanvasGroup>();

					//If the current event overlaps the previous event, 
					//you have to hide the previous one.
					if(currentEvent[0]==previousEvent[0])
					{
						//hiding the previous event.
						previousEvent_CanvasGroup.alpha=0f;
					
					}

					//The current event is in a different window from the previous event.
					else
					{
						//a.Turn on the grey panel for the previous event.
								int panelToHide=-1;

								if(previousEvent[0]=='C')
									panelToHide=1;
														
								else if(previousEvent[0]=='W')
									panelToHide=0;

								else if(previousEvent[0]=='H')
									panelToHide=2;

								if(panelToHide==-1)
									print("newDisplayManager.cs, DisplayEvent()-->line 287 Error: event tag information is incomplete, no actor specified.");

								else
								{
								
									Color panelColor = greyPanels[panelToHide].GetComponent<Image>().color;
									panelColor.a = 0.364f;  //equivalent to 93/255
									greyPanels[panelToHide].GetComponent<Image>().color = panelColor;		
								}	

						//b.Turn off the grey panel for the current event.
								panelToHide=-1;

								if(currentEvent[0]=='C')
									panelToHide=1;
														
								else if(currentEvent[0]=='W')
									panelToHide=0;

								else if(currentEvent[0]=='H')
									panelToHide=2;

								if(panelToHide==-1)
									print("newDisplayManager.cs, DisplayEvent()-->line 310 Error: event tag information is incomplete, no actor specified.");

								else
								{
								
									Color panelColor = greyPanels[panelToHide].GetComponent<Image>().color;
									panelColor.a = 0f;
									greyPanels[panelToHide].GetComponent<Image>().color = panelColor;			
								}


						//c.If the current event overlaps with the last event for any panel, hide it. 
								if(lastCitizenEvent!="" && previousEvent != lastCitizenEvent)
									if(currentEvent[0]==lastCitizenEvent[0])
											GameObject.FindWithTag(lastCitizenEvent).GetComponent<CanvasGroup>().alpha=0f;

								if(lastWaterEvent!="" && previousEvent != lastWaterEvent)
									if(currentEvent[0]==lastWaterEvent[0])
											GameObject.FindWithTag(lastWaterEvent).GetComponent<CanvasGroup>().alpha=0f;

								if(lastHealthEvent!="" && previousEvent != lastHealthEvent)
									if(currentEvent[0]==lastHealthEvent[0])
											GameObject.FindWithTag(lastHealthEvent).GetComponent<CanvasGroup>().alpha=0f;

					}

					//Making the previous event non-interactable, and also -not- blocking raycasts.
					previousEvent_CanvasGroup.interactable=false;
					previousEvent_CanvasGroup.blocksRaycasts=false;
					previousEvent_CanvasGroup.ignoreParentGroups=false;


				}


				else{

					//Perform this code from the above sample. Hide the panel for the first event.

					//b.Turn off the grey panel for the current event.
								int panelToHide=-1;

								if(currentEvent[0]=='C')
									panelToHide=1;
														
								else if(currentEvent[0]=='W')
									panelToHide=0;

								else if(currentEvent[0]=='H')
									panelToHide=2;

								if(panelToHide==-1)
									print("newDisplayManager.cs, DisplayEvent()-->line 310 Error: event tag information is incomplete, no actor specified.");

								else
								{
								
									Color panelColor = greyPanels[panelToHide].GetComponent<Image>().color;
									panelColor.a = 0f;
									greyPanels[panelToHide].GetComponent<Image>().color = panelColor;			
								}

					}

					//4.17: Checking for any special buttons, if any.
					reset_SpecialButtons_ifAny(currentEvent);


					//showing the current event.
					print("=======displayManager: the event I want to load is: "+currentEvent);

					//Displaying new event, Non fade-in version. Commented out.
						 //CanvasGroup currentEvent_CanvasGroup= 
						 //			GameObject.FindWithTag(currentEvent).GetComponent<CanvasGroup>();
						// 	currentEvent_CanvasGroup.alpha=1f;
						 	//currentEvent_CanvasGroup.interactable=true;
							// currentEvent_CanvasGroup.blocksRaycasts=true;
						 	// currentEvent_CanvasGroup.ignoreParentGroups=true;

					//Trying a fade-in with my events.
						fadeIn_Event(currentEvent, 0.1f);

					//Checking the kind of event being displayed.
						//If it's a slider event, you may want to change the color of the bars.
							//To do later...

					//Storing the currentEvent in the previousEvent variables.
					previousEvent=currentEvent;
			
					//Saving the event according to the actor.
					if(currentEvent[0]=='C')
						lastCitizenEvent=currentEvent;
					else if(currentEvent[0]=='W')
						lastWaterEvent=currentEvent;
					else if(currentEvent[0]=='H')
						lastHealthEvent=currentEvent;
					else	
						print("newDisplayManager.cs, DisplayEvent()-->line 311 Error: event tag information is incomplete, no actor specified.");

		}

	public void showPanelsEnding(bool shouldShow)
	{
		//Hiding both agency panels to emphasize the ending scene.

		if(shouldShow)
			{
				Color panelColor = greyPanels[0].GetComponent<Image>().color;
				panelColor.a = 0.664f;  //equivalent to 93/255
				greyPanels[0].GetComponent<Image>().color = panelColor;

				panelColor = greyPanels[2].GetComponent<Image>().color;
				panelColor.a = 0.664f;  //0.664f, equivalent to 93/255
				greyPanels[2].GetComponent<Image>().color = panelColor;

				//Turn on all dark screen panels (C, W & H).
					darkPanel_screens[0].alpha=1f;
					darkPanel_screens[1].alpha=1f;
					darkPanel_screens[2].alpha=1f;
			}

		else
			{
				Color panelColor = greyPanels[0].GetComponent<Image>().color;
				panelColor.a = 0f;  //equivalent to 93/255
				greyPanels[0].GetComponent<Image>().color = panelColor;

				panelColor = greyPanels[2].GetComponent<Image>().color;
				panelColor.a = 0f;  //equivalent to 93/255
				greyPanels[2].GetComponent<Image>().color = panelColor;

				//Turn off 3 dark screen panels (C,W & H).
					darkPanel_screens[0].alpha=0f;
					darkPanel_screens[1].alpha=0f;
					darkPanel_screens[2].alpha=0f;

			}


	}


//E. Special Event Panel Methods. Managing the display for special events with sliders or maps.

	
//1. Drought Rationing methods.
	public void update_Drought_Event()
	{

		//0. fetch the drought event.
		GameObject droughtEvent= GameObject.FindWithTag("W_rationDrought");

		//1.Update the first text according to the drought crisis level.
			//Fetching the first text.
			GameObject temp= droughtEvent.transform.GetChild(0).gameObject;
			Text droughtText1= temp.GetComponent<Text>();
			
			if(newGameManager.gameManager.wagency.waterLevel_dangerState == 1)
			droughtText1.text= "Drought conditions continue for most  of the island.  Reservoirs are at <b>warning</b> levels.\nChoose the number of days you want to ration water. Limit water use to:";

			else if(newGameManager.gameManager.wagency.waterLevel_dangerState == 0)
			droughtText1.text= "Drought conditions continue for most  of the island.  Reservoirs are at <b>critical</b> levels.\nChoose the number of days you want to ration water. Limit water use to:";


		//2.Update the second text, show the current rationing period.
		update_Drought_Ration_Text();

	}

	public void update_Drought_Ration_Text()
	{

		//0. fetch the drought event's ration Text, and with it its text component.
			GameObject rationText_object=GameObject.FindWithTag("W_rationText");
			Text droughtText2= rationText_object.GetComponent<Text>();
			
		//1. Update the second text, show the current rationing period.
			if(newGameManager.gameManager.wagency.ration_daysWithWater == 1)
			droughtText2.text= "<b>1 day a week</b>";

			else if(newGameManager.gameManager.wagency.ration_daysWithWater == 7)
			droughtText2.text= "<b>7 days a week</b>\n(no rationing)";

			else if(newGameManager.gameManager.wagency.ration_daysWithWater == 0)
			droughtText2.text= "<b>0 days a week</b>\n(no potable water)";
			
			else 
			{
				droughtText2.text= "<b>"+
								newGameManager.gameManager.wagency.ration_daysWithWater+
								" days a week</b>";
			}

		//Update the drinkingWaterAccess Display, why not.
			isPerforming_HighlightAnimations_Array[6]=false;
			update_Citizen_Access_Displays(true);
			//Extra:checking for higlights.
			if(isPerforming_HighlightAnimations_Array[6])
						{
							StartCoroutine(performHighlightAnimation(6));
						}

	}

//2. Water Repair event's methods.
	public void show_WRepairSprites()
	{

		for (int i=0; i<W_RepairSprites.Length; i++)
		{
			if(newGameManager.gameManager.wagency.map_areThereIncidents_Array[i])
				W_RepairSprites[i].enabled=true;

			//else 	
				//W_RepairSprites[i].enabled=false;

		}
	}

	public void reset_WRepairSprites()
	{
		//Resets all unused repair icons to their unpressed state.
		for (int i=0; i<W_RepairSprites.Length; i++)
		{
			W_RepairSprites[i].GetComponent<repairIconScript>().resetIconSprite();
			W_RepairSprites[i].enabled=false;
		}

		//also resetting the button text.
		update_Display_from_WaterInfrastructureRepair(0);

	}

	//Hides part of the maintenance crew after a specific event choice.
	public void hide_Maintenance_Crew_afterFundingCuts(bool isHiding)
	{

		//Cutting crew by half.
		//Hide the last 3 sprites in the array.
		if(isHiding)
			for(int i=newGameManager.gameManager.wagency.numTotalPersonnel; i<6; i++)
				Water_maintenanceSprites[i].enabled=false;

		//When resetting, show the total amount of sprites.
		else		
			for(int i=0; i<Water_maintenanceSprites.Length; i++)
				Water_maintenanceSprites[i].enabled=true;

	}

	public void flashRepairWarning()
	{

		StartCoroutine(flashRepairWarning_coRoutine());

	}

	IEnumerator flashRepairWarning_coRoutine()
	{
		GameObject warningText_object= GameObject.FindWithTag("repairWarningText");
		Text warningText= warningText_object.GetComponent<Text>();

		fadeIn_Text(warningText, 0.3f);//0.6f);
		yield return new WaitForSeconds(1f);
		fadeOut_Text(warningText, 0.3f);//0.6f);					
	}

	public void update_Display_from_WaterInfrastructureRepair(int numberOfRepairs)
	{
		//Using the helper display function.
		updateDisplaysForVariable((int)DisplayFields.WATER_FUNDS);
		updateDisplaysForVariable((int)DisplayFields.CITIZEN_W_ACCESS);
		updateDisplaysForVariable((int)DisplayFields.WATER_MAINTENANCE);

		if(numberOfRepairs==0)
		{

			GameObject eventButtonObjectText= GameObject.FindWithTag("infrButtonText");
			eventButtonObjectText.GetComponent<Text>().text="Don't dispatch\nmaintenace crews";

		}

		else
		{
			
			string repairMessage="";

			if (numberOfRepairs==1)
				repairMessage="Dispatch 1\nmaintenace crew";

			else
				repairMessage="Dispatch "+numberOfRepairs+"\nmaintenace crews";

			GameObject eventButtonObjectText= GameObject.FindWithTag("infrButtonText");
			eventButtonObjectText.GetComponent<Text>().text=repairMessage;

		}

	}

//3.Water Bill slider's events.
	public void update_Display_from_WaterBillSlider()
	{
		//Using the helper display function.
		updateDisplaysForVariable((int)DisplayFields.WATER_FUNDS);
		updateDisplaysForVariable((int)DisplayFields.CITIZEN_FUNDS);
		updateDisplaysForVariable((int)DisplayFields.CITIZEN_W_ACCESS);
		updateDisplaysForVariable((int)DisplayFields.HEALTH_FUNDS);
	}


//4.Health events' display methods.
	//
	public void update_Display_from_HResidents_Slider()
	{
		//Using the helper display function.
		updateDisplaysForVariable((int)DisplayFields.HEALTH_DOCTORS);
		updateDisplaysForVariable((int)DisplayFields.HEALTH_FUNDS);
		updateDisplaysForVariable((int)DisplayFields.CITIZEN_D_ACCESS);
	}

	//--Health: doctor+resource shuffle functions.
	public void update_UI_after_HresourceShuffle()
	{
		//Using the helper display function.
		updateDisplaysForVariable((int)DisplayFields.HEALTH_FUNDS);
		updateDisplaysForVariable((int)DisplayFields.CITIZEN_D_ACCESS);
		
	}

	public void update_all_HShuffle_Display()
	{
		//Takes the boolean variable for the doctor icons and
			//updates the map display with that information.

			//Update the doctor icons.
			for(int i=0; i<Hshuffle_hospitalObjects.Length; i++)
			{
				bool currentDoctorIconState= 
					newGameManager.gameManager.hagency.isDoctorAssigned_toHospitals_Array[i];
				
				print("Doctor assigned to hospital array#"+i+", : "+currentDoctorIconState+".");

				//Calling the object's script that will set the button state and color.
				Hshuffle_hospitalObjects[i].setIconValue(currentDoctorIconState);
			}

			//Update the hospital icons.
			for(int i=0; i<Hshuffle_hospitalSprites.Length; i++)
			{
				update_HShuffle_hospitalIcon(i,
											newGameManager.gameManager.hagency.areHospitalsOpen_Array[i]);
			}

			//Update the text also.
			update_ShuffleEvent_StatusMessage("updateDisplay");

			//You may have to do some other color coding here.

	}

	public void update_HShuffle_hospitalIcon(int hospitalIndex, bool isHospitalOpen)
	{

		//Assigning the hospital icon color here.
		if(isHospitalOpen)
			Hshuffle_hospitalSprites[hospitalIndex].color=Color.white;
		else 
			Hshuffle_hospitalSprites[hospitalIndex].color=hospitalClosed_Color;

	}

	public void update_ShuffleEvent_StatusMessage(string messageType)
	{
		if(messageType=="noDoctors")
			{
				
				Hshuffle_messageText.text="All available doctors\n\t\t\t\t\thave been assigned to a hospital. ";
				StartCoroutine(flash_hShuffle_Message_coRoutine());
			}
		else if(messageType=="closeHosp")
			{
				Hshuffle_messageText.text="This hospital will be closed. ";
				StartCoroutine(flash_hShuffle_Message_coRoutine());
			}
		else if(messageType=="notEndingYet")
			{
				Hshuffle_messageText.text="There are still doctors\n\t\t\t\tavailable to staff hospitals.";
				StartCoroutine(flash_hShuffle_Message_coRoutine());
			}

		else 
			{

				 int numberOf_unassignedDoctors=
				 	-1*newGameManager.gameManager.hagency.numOfAssignedDoctorUnits
					 +(int)newGameManager.gameManager.hagency.numOfDoctors;

				Hshuffle_messageText.text=
				"<i>Number of unassigned doctors= "+ numberOf_unassignedDoctors
				+"\n\t\t\t\t\t\t\t\t\t\tOpen hospitals="+newGameManager.gameManager.hagency.numOfOpenHospitals+"</i>";
				
			}
	}

	//Updating the text prompt in the Health agency's hospital-doctor staffing event.
	public void update_ShuffleEvent_TopMessage(string updateType)
	{
		//Create the message to display.
		string newMessage="";

		if(updateType=="decr")  
		{
			//Showing a 'doctors decreased' message.
			newMessage="<b>Fewer doctors are available to staff our hospitals.</b>";

		}

		else
		{
			//Showing a 'doctors increased' message.
			newMessage="<b>More doctors are available to staff our hospitals.</b>";
		}

		//Add the message to the event's text.
			GameObject shuffleEvent_textObject=GameObject.FindWithTag("HresShuffle_Text");
			Text eventText=shuffleEvent_textObject.GetComponent<Text>();

				//Fetching the old text.
					//getting the index of the end of the first sentence.			
				string oldMessage=eventText.text;
				int startIndex=oldMessage.IndexOf("Click");

				//The new message will replace the first sentence with our generated sentence.
				string completeMessage=newMessage+oldMessage.Substring(startIndex-1);
				eventText.text=completeMessage;
	}

	//Displaying a warning message in the health agency's shuffle event.
	IEnumerator flash_hShuffle_Message_coRoutine()
	{
		//This routine will not fade out the message text.
			//The text stays onscreen for explanation.

		//Hiding the text, first.
		Hshuffle_messageText.color=	new Color(Hshuffle_messageText.color.r,
											  Hshuffle_messageText.color.g, 
											  Hshuffle_messageText.color.b, 0f);

		fadeIn_Text(Hshuffle_messageText, 0.3f);//0.6f);
		yield return new WaitForSeconds(2f);

		//Revert back to the standard information message.
			//same as the standard info message.
			int numberOf_unassignedDoctors=
				 	-1*newGameManager.gameManager.hagency.numOfAssignedDoctorUnits
					 +(int)newGameManager.gameManager.hagency.numOfDoctors;

				Hshuffle_messageText.text=
				"<i>Number of unassigned doctors= "+ numberOf_unassignedDoctors
				+"\n\t\t\t\t\t\t\t\t\t\tOpen hospitals="+newGameManager.gameManager.hagency.numOfOpenHospitals+"</i>";
	}

//5.Function that performs all checks and display operations for a given interface element.
	void updateDisplaysForVariable(int i)
	{	
			
			isPerforming_HighlightAnimations_Array[i]=false;

			//Performing Variable Update functions, depending on the index entered.
			if(i==(int)DisplayFields.WATER_LEVEL)
				update_WaterLevel_Display();			
			else if(i==(int)DisplayFields.WATER_FUNDS)
				update_WaterFunds_Display();
			else if(i==(int)DisplayFields.WATER_MAINTENANCE)
				update_Water_MaintenanceCrew_Display();
			else if(i==(int)DisplayFields.CITIZEN_HEALTH)
				update_Citizen_Health();
			else if(i==(int)DisplayFields.CITIZEN_MEDS)
				update_Citizen_Medicine_Display();	
			else if(i==(int)DisplayFields.CITIZEN_D_ACCESS)
				update_Citizen_Access_Displays(false);
			else if(i==(int)DisplayFields.CITIZEN_W_ACCESS)
				update_Citizen_Access_Displays(true);
			else if(i==(int)DisplayFields.CITIZEN_FUNDS)
				update_Citizen_Coins_Display();
			else if(i==(int)DisplayFields.HEALTH_DOCTORS)
				update_HealthDoctors_Display();
			else if(i==(int)DisplayFields.HEALTH_FUNDS)
				update_HealthFunds_Display();
			/*if(i==(int)DisplayFields.HEALTH_FACILITIES)
				update_HealthFunds_Display();*/	
	
			//Extra:checking for higlights.
			if(isPerforming_HighlightAnimations_Array[i])
						{
							StartCoroutine(performHighlightAnimation(i));
						}

	}


//F.Animation methods used for fading events and images in+out, as well as fading between colors.
	//For fading out events by using their tag.
	void fadeOut_Event(string eventName, float duration)
	{
		StartCoroutine(Fade_Event_To(eventName, 0.0f, duration));
	}

	void fadeIn_Event(string eventName, float duration)
	{
		StartCoroutine(Fade_Event_To(eventName, 1.0f, duration));
	}

	IEnumerator Fade_Event_To(string eventName, float aValue, float aTime)
	{
		CanvasGroup eventCanvas= GameObject.FindWithTag(eventName).GetComponent<CanvasGroup>();
		
		//1.First of all, depending on action, turn the Canvas Group variables on or off.
	
			if(aValue>0.0f)
				{
					//fading In.
					//Turning on the Canvas Group variables.

					eventCanvas.interactable=true;
					eventCanvas.blocksRaycasts=true;
					eventCanvas.ignoreParentGroups=true;

				}

			else 
				{	
					//fading Out.
					//Turning off the Canvas Group variables.

					eventCanvas.interactable=false;
					eventCanvas.blocksRaycasts=false;
					eventCanvas.ignoreParentGroups=false;

				}

		//2. Doing the actual fade.
		float step= Time.deltaTime / aTime;

		//We're using the same function to fade in or out, 
		//we just change the arithmetic sign.
		int sign=0;
		if(aValue>0.0f)
			sign=1;
		else 
			sign=-1;

		for (float t = 0.0f; t < 1.0f; t += step)
		{
            eventCanvas.alpha += sign*step;
			yield return null;
		}


	}


//For fading out images directly, by providing a canvas group.
	void fadeOut_CanvasGroup(CanvasGroup eventCanvas, float duration)
	{
		StartCoroutine(Fade_CanvasGroup_To(eventCanvas, 0.0f, duration));
	}

	void fadeIn_CanvasGroup(CanvasGroup eventCanvas, float duration)
	{

		StartCoroutine(Fade_CanvasGroup_To(eventCanvas, 1.0f, duration));

	}

	IEnumerator Fade_CanvasGroup_To(CanvasGroup eventCanvas, float aValue, float aTime)
	{
		float step= Time.deltaTime / aTime;

		//We're using the same function to fade in or out, 
		//we just change the arithmetic sign.
		int sign=0;
		if(aValue>0.0f)
			sign=1;
		else 
			sign=-1;

		for (float t = 0.0f; t < 1.0f; t += step)
		{
            eventCanvas.alpha += sign*step;
			yield return null;
		}

		//Changing canvas properties (interaction, etc.) if this is a tutorial screen.
		if(eventCanvas.gameObject.tag == "tutorialScreen" || 
		   eventCanvas.gameObject.tag == "startWindow")
		{
				if(aValue>0.0f)
					{
						//fading In.
						//Turning on the Canvas Group variables.

						eventCanvas.interactable=true;
						eventCanvas.blocksRaycasts=true;
						eventCanvas.ignoreParentGroups=true;

					}

				else 
					{	
						//fading Out.
						//Turning off the Canvas Group variables.

						eventCanvas.interactable=false;
						eventCanvas.blocksRaycasts=false;
						eventCanvas.ignoreParentGroups=false;

					}
		}

	}

//Same methods, but for Image components.
	void fadeOut_Text(Text textTofade, float duration)
	{
		StartCoroutine(Fade_TextAlpha_To(textTofade, 0.0f, duration));
	}

	void fadeIn_Text(Text textTofade, float duration)
	{

		StartCoroutine(Fade_TextAlpha_To(textTofade, 1.0f, duration));

	}
	
	IEnumerator Fade_TextAlpha_To(Text textTofade, float aValue, float aTime)
	{
		float step= Time.deltaTime / aTime;
		Color currentColor=textTofade.color;

		//We're using the same function to fade in or out, 
		//we just change the arithmetic sign.
		int sign=0;
		if(aValue>0.0f)
			sign=1;
		else 
			sign=-1;

		for (float t = 0.0f; t < 1.0f; t += step)
		{
            float newAlpha= textTofade.color.a + sign*step;
			textTofade.color=new Color(currentColor.r, 
										currentColor.g,
										currentColor.b,
										newAlpha);
			yield return null;
		}

	}

	IEnumerator fade_Image_ToColor(Image imageToChange, Color endColor, float aTime)
	{
		float step= Time.deltaTime / aTime;

		//We're using the same function to fade in or out, 
		//we just change the arithmetic sign.

		Color startColor=imageToChange.color;

		for (float t = 0.0f; t < 1.0f; t += step)
		{
			Color newColor= Color.Lerp(startColor, endColor, t);
            imageToChange.color =newColor;
			yield return null;
		}

	}

	IEnumerator updateMonthDisplay_sequence(float tickerSpeed)
	{
		//update the counter.
		currentWeek++;

		//1. Animate ticker moving.

			RectTransform monthTicker_transform=currentMonth_ticker.GetComponent<RectTransform>();


			Vector3 startPosition= monthTicker_transform.localPosition;
			Vector3 endPosition=
					new Vector3(
							timeDisplay_ticker_startPosX+currentWeek*ticker_moveOffset,
							monthTicker_transform.localPosition.y,
							monthTicker_transform.localPosition.z
							);
			print("@@@@@@@@		startPosition for ticker is "+startPosition.x);
			print("@@@@@@@@		endPosition for ticker is "+endPosition.x);

			float step= Time.deltaTime / tickerSpeed;

			for (float t = 0.0f; t < 1.0f; t += step)
			{			
	            monthTicker_transform.localPosition = Vector3.Lerp(startPosition, endPosition, t);
				yield return null;
			}

			monthTicker_transform.localPosition=endPosition;

		//2. Animate the colorChange.
			//Change color to brightest highlight.
			StartCoroutine(fade_Image_ToColor(timeMarkers[currentWeek], 
						timeDisplay_brightHiglightColor, 
						timeDisplay_colorAnim_speed ));

			yield return new WaitForSeconds(0.5f);//0.02f);

			//Change color to normal highlight.
			StartCoroutine(fade_Image_ToColor(timeMarkers[currentWeek], 
						timeDisplay_highlightedColor, 
						timeDisplay_colorAnim_speed ));

		//2b. Animate a color change for the ticker as well.
			//Change color to brightest highlight.			
			//Image ticker_ImageComponent=currentMonth_ticker.GetComponent<Image>();
			
			StartCoroutine(fade_Image_ToColor(currentMonth_ticker.GetComponent<Image>(), 
						higlight_tickerColor, 
						timeDisplay_colorAnim_speed ));

			yield return new WaitForSeconds(1f);

			//Change color to normal highlight.
			StartCoroutine(fade_Image_ToColor(currentMonth_ticker.GetComponent<Image>(), 
						default_tickerColor, 
						timeDisplay_colorAnim_speed ));

	}


	IEnumerator fade_CitizenImage_ToColor_Loop(Color endColor, float fadeTime, float waitTime)
	{
	

		while(isCLowHealth_anim_running)
		{
			print("$$$$$$$$		FLASHING HEALTH WARNING.");
			StartCoroutine(fade_Image_ToColor(Citizen_HealthOutline_sprite.GetComponent<Image>(),endColor , fadeTime ));//0.6f);
			yield return new WaitForSeconds(waitTime);
			StartCoroutine(fade_Image_ToColor(Citizen_HealthOutline_sprite.GetComponent<Image>(),C_outline_normalColor , fadeTime));//0.6f);					
			yield return new WaitForSeconds(waitTime);

		}
	
	}

	IEnumerator fade_WaterImage_ToColor_Loop(Color endColor, float fadeTime, float waitTime)
	{
	
		while(isWaterSpriteLooping)
		{
			fade_Image_ToColor(WaterSprite.GetComponent<Image>(),endColor , fadeTime );//0.6f);
			yield return new WaitForSeconds(waitTime);
			fade_Image_ToColor(WaterSprite.GetComponent<Image>(),water_normalColor , fadeTime);//0.6f);					
			yield return new WaitForSeconds(waitTime);

		}
	
	}

	IEnumerator animate_Water_heightChange(float startHeight, float endHeight, float duration)
	{
	
		float step= Time.deltaTime / duration;

		//set water height in the sprite.
		RectTransform WaterSprite_transform= WaterSprite.GetComponent<RectTransform>();
		

		float currentHeight= startHeight;

		for (float t = 0.0f; t < 1.0f; t += step)
		{

			WaterSprite_transform.sizeDelta= 
			new Vector2(	WaterSprite_transform.rect.width, 
							Mathf.Lerp(startHeight, endHeight, t) );

			yield return null;
		}

	}

	IEnumerator animate_Citizen_heightChange(float startHeight, float endHeight, float duration)
	{
	
		float step= Time.deltaTime / duration;

		//set water height in the sprite.
		RectTransform CitizenSprite_transform= Citizen_HealthSprite.GetComponent<RectTransform>();
		

		float currentHeight= startHeight;

		for (float t = 0.0f; t < 1.0f; t += step)
		{

			CitizenSprite_transform.sizeDelta= 
			new Vector2(	CitizenSprite_transform.rect.width, 
							Mathf.Lerp(startHeight, endHeight, t) );

			yield return null;
		}

	}


//G. Functions used to handle the introductory sequences.---
public void startButtonPressed()
{  
		newGameManager.gameManager.beginGamePlay();

		//if previously showing the tutorial screens,
			//hide the introductory actor events.
			if(current_introScreen>0)
				hideAll_ActorEvents();	

		//Turn off the dark panels.
			darkPanel_screens[0].alpha=0f;
			darkPanel_screens[1].alpha=0f;
			darkPanel_screens[2].alpha=0f;

}

public void advance_IntroScreen(int which_actorIntro_toShow)
{
	
	print("^^^^^^ CurrentIntroScreen is "+current_introScreen+
		  ", Showing actor's intro in index# "+which_actorIntro_toShow);

	//In between tutorial screens, the game will show information
		//on each actor's panel.
	if(current_introScreen==1)
	{
		//Option 0: show the Water Agency's description.
			//Turn off the previous introductory screen.
		if(which_actorIntro_toShow==0)
		{
			

			//hide the previous tutorial screen.
			fadeOut_CanvasGroup(introductory_screens[1], 0.1f);					

			//show the water panel's introductory text.
			displayEvent("W_intro");

			//hide the dark panel for this actor.
				darkPanel_screens[0].alpha=0f;

			//possibly do a pulsing animation.

		}

		//Switching this order up: W, H & C.
			//Option 2: show the Health Agency's description.
		if(which_actorIntro_toShow==2)
		{

			//show the water panel's introductory text.
			displayEvent("H_intro");

			//hide the dark panel for this actor,turn on the previous.
				darkPanel_screens[0].alpha=1f;
				darkPanel_screens[2].alpha=0f;

				//possibly do a pulsing animation.
		}


		//Option 2: show the Citizen's description.
		if(which_actorIntro_toShow==1)
		{

			//show the water panel's introductory text.
			displayEvent("C_intro");

			//hide the dark panel for this actor, turn on the previous.
				darkPanel_screens[2].alpha=1f;
				darkPanel_screens[1].alpha=0f;

				//possibly do a pulsing animation.
		}

		//Option 3: done with the panel actors, go to the next tutorial window
	
	}

	if(which_actorIntro_toShow <0)	//this code manages the tutorial windows.
	{
		//Turn on the health actor's dark panel.
		if(current_introScreen ==1)
			darkPanel_screens[1].alpha=1f;

		//hide the previous tutorial screen.
		if(current_introScreen != 1)
		fadeOut_CanvasGroup(introductory_screens[current_introScreen], 0.1f);					
		
		current_introScreen++;	

		//show the next tutorial screen.
		fadeIn_CanvasGroup(introductory_screens[current_introScreen], 0.1f);					

	}
}


}//End of DisplayManager.
