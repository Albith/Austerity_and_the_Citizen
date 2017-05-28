using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

//newGameManager is the class that manages game logic for the game.

public class newGameManager : MonoBehaviour {

//A.We start by defining the main logic variables in the game.

	public static newGameManager gameManager;

	//Setting the number of turns and events per turn.
	public int maxTurns=3;
	public int actorEvents_perTurn=4;

	//This setting is used for debugging the game.
		public bool isTestingEvents=true;
			public char actorToTest='W';
			public string firstEventToTest="";
			bool isFirsTestEvent_Done=false;

	//This variable describes the state of the game.
		//The following values will be used to calculate game state.
		//-1 stands for introduction screen, 0 for tutorial, 1 is gameMode, 2 is for ending.
		int gameState=-1;
	
	//This variable sets the type of water emergency that will take place in the game.  
		//Only one of two emergencies will take place:
			//A drought or a flooding scenario (that can lead to a zika epidemic).
			public bool isDroughtScenario=true;
	
	//Variables used to manage event handling.
		//variable used to toggle between water and health events, following a citizen event.
		public bool didWaterActInTurn=false;
		//variables used to keep track of previous events in the generateNewEvent() function.
			char currentActor=' ';
			char previousActor= ' ';
			char lastCitizenIncident_toAppear= ' ';
			char lastHealthEvent_toAppear= ' ';



	public newDisplayManager displayManager;

	public int currentTurnCounter=0;  //starts at 1.
	public int currentActorCounter=0;  //starts at 0.

	string newEventName="";
		string lastWaterEvent_name="";
		string lastCitizenEvent_name="";
		string lastHealthEvent_name="";


//B.Variables used to keep track of the game events already triggered.
	
	//Some can be visited more than once. Used for random event generation.
	Dictionary<string, int> waterEventsDict;
	Dictionary<string, int> citizenEventsDict;
	Dictionary<string, int> healthEventsDict;


//C. Defining the classes and instances used to describe each of the three actors in the game.
	
	//C-1.The Water Agency.	
		public class WaterAgency
			{

				//Agency funds: Ranges from 0f to 20f(maximum)
				public float funds;

				//Water management variables.
				public float waterLevel;  	
				public float waterUse;		//Refers to waterConsumption by the general population.
				public float rainFall;
					//Constants that defined the different warning levels (that would trigger a drought announcement)
					public float warningWaterLevel=8f;
					public float criticalWaterLevel=4f;
					public float overflowWaterLevel=15f;

				//Emergency variables: Drought-related variables.
				public bool isDroughtAnnounced;
				//The following variable displays the drought state, as determined by the water level.
					//Each value is followed by its meaning:
						//2-above danger levels.
						//1-at or below warning levels. Drought starts.
						//0-at or below critical levels. More intense drought.
						public int waterLevel_dangerState=2;  

				//Variable used to keep track of days with water service (used for rationing during drought)
				public int ration_daysWithWater=7; //no rationing days.

				//Maintenance-related variables.
					
				//The following variables describe possible water incidents,
					//and keep track of incidents taking place.
				public string[] possibleIncidents={"Broken Pipe", "Dirty Water", "Water Cutoff", "None", "None"};
				public string currentWaterIncident="None";
				public bool isIncidentReported=false;			
					public int incidentRisk=0;
					public int numberOfIncidents=0;

				//Member function that generates a Water Incident.
				public void tryToGenerateWaterIncident()
				{


					if(currentWaterIncident == "None")
					{

							if(incidentRisk>0)
							{
								//if incidentRisk=1, chance of water incidents is 25%
								//if incidentRisk=2, chance of water incidents is 75%
								//if incidentRisk=3, chance of water incidents is 100%

								if(incidentRisk==1)
											currentWaterIncident=possibleIncidents[Random.Range(0,possibleIncidents.Length)];

								else if(incidentRisk==2)	
											currentWaterIncident=possibleIncidents[Random.Range(0,possibleIncidents.Length-1)];

								else if(incidentRisk==3)
											currentWaterIncident=possibleIncidents[Random.Range(0,possibleIncidents.Length-2)];

							}

							else 
								currentWaterIncident="None";

						
					}

				}

				//Variables that keep track of the maintenance crews, plus the incidents they have repaired.
				public int numPersonnel;
				public int numTotalPersonnel=6;
				public int numberOfRepairs_Selected=0;
				public bool[] map_areThereIncidents_Array; 

				//boolean that checks whether water monitoring is still in effect.
				public bool isWmonitoringOn=true;					

			};
		
		public WaterAgency wagency;
	

	//C-2. The Health Agency.	
		public class HealthAgency
			{
				//Doctors & Facilities variables.
				//Ranges from 0 to 9000.  Each doctor icon on screen represents 1000 doctors.
				public float numOfDoctors;  
					public float maxNumOfDoctors;
					//other dependent amounts:
						//There are 5 available hospitals, and each can hold 2 doctor units.
						public int maxNumOfHospitals;   
						public int numOfOpenHospitals;

						//Variables used for assigning doctors to hospitals:
						public bool[] isDoctorAssigned_toHospitals_Array;
						public bool[] areHospitalsOpen_Array; //this depends on the upper variable.
						public int numOfAssignedDoctorUnits;
						public string wereDoctorUnitsModified;

					//Variables used to set prices of health procedures (used by the citizen)
					public float medicationsPrice=250f;  
					public float labsPrice=400f;
					public float xRaysPrice=400f;
					public float hospPrice=500f;


				//Funds and money related variables.
				public float funds;	

					//Could be used in the future: referring to the possible incoming funds:
					// public float incomingFunds;
					// public float equipmentPurchases;

				//Emergency-related variables.
				public float floodRisk_facilities;
				//Variable that stores the amount of funding spent on blood banks and blood drives.
				public float bloodSpending;
				//These two variables do not affect the game right now, but could  in the future.
				public float zikaEpidemicRisk=1;  //[0-3]
				public float influenzaEpidemicRisk=1;  //[0-3]


			};
		
		public HealthAgency hagency;

	//C-3. The Retired Citizen Actor.
		 public class Citizen
			{

				//Vital parameters: health & funds.
				public float health;  //Range: from 0 to 20f.

					//Variable that checks how long the citizen has gone without dental checkups,
					//as well as how serious their dental issue is.
					public int dentistEventState=0;

					//Number of medicines the citizen has.  Range: from 0 to 3.
					public int medicationsAmount=2; //decreases over time, recharged if repurchased.
						public int turnsWithoutMedication=0;

						//Variables used to keep track of doctor visits and health plan settings.
							public int turnsWithoutSeeingDoctor=0; //[0-3], after 3 higher chance of ailments happening. 
							public int accessToDoctor=0;  //[0-20] The farther away, the harder it is to see doctor.
							public bool isPayingFullHospitalization=false;
							public bool areLabCostsCovered=true;
							
					//Variables that define and keep track of possible health issues.
					public string[] possibleIncidents={"Flu", "Fall", "Root Canal"};
					public string currentHealthIncident="None";

					//Boolean that checks if an introductory message has been displayed.					
					public bool isDrinkingWaterAccessIssueIntroduced=false;

					//Variables that keep track of the citizen's health incidents.
					public int healthIncidentRisk=0; 
					public int currentHealthIncident_counter=0;

					//Function that triggers a health issue in the citizen.
					public void getNextHealthIncident()
									{

												if(currentHealthIncident_counter< possibleIncidents.Length)
												{
													currentHealthIncident=possibleIncidents[currentHealthIncident_counter];
													currentHealthIncident_counter++;
												}

											if(currentHealthIncident != "None")
												print(">>>>>>CITIZEN: health incident generated: "+currentHealthIncident+".");
											else
												print(">>>>>>CITIZEN: no health incident generated, variable is: "+currentHealthIncident+".");


									}

					
					//Variables that control 'follow-up' medical issues that may arise.
						//Follow up events are: medical requests the citizen's doctor may make.
						public string[] possibleFollowups={"Labs", "ExtraMeds", "Xrays"};
						public string currentFollowup="None";
						public int currentFollowup_counter=0;

						public bool isSkippingFollowup=false;
						public bool didHealthIncidentAppear=false;
						public bool didHealthFollowupAppear=false;
						public int turnsWithoutFollowup=0;
						//Variable that stores the result of any healthcare transaction in the game.
							//0= no transaction has ocurred, default value.
							//1= medicare covers.
							//2= medicare unexpectedly does not cover.
							//3= your insurance dropped this part from the plan entirely.
						public int healthTransactionResult=0;  

					//Function that generates a follow up issue.
					public void generateFollowupEvent()
									{
									
										if(currentFollowup_counter< possibleFollowups.Length)
												{
													currentFollowup=possibleFollowups[currentFollowup_counter];
													currentFollowup_counter++;
												}

										if(currentFollowup != "None")
											print(">>>>>>CITIZEN: follow-up event generated: "+currentFollowup+".");
										else
											print(">>>>>>CITIZEN: no follow-up event generated, variable is: "+currentFollowup+".");

									}

					//Constant values referring to water and health bills the citizen must cover.
					public float waterBill=120f;	
					public float healthcarePrimeBill=227f;
					public float labCosts=50f;

					public int turns_skippedWaterPayments=0; //if it gets to 2 turns, you get a service cutoff.
					public int turns_skippedHealthPayments=0;	//if it gets to 2 turns, you lose your health coverage.
					public bool isHealthCutOff=false;
					public bool isWaterCutOff=false;

					//Just like health incidents, there are utility incidents.
						//These can increase too.

					public int groceriesAmount=3;
						public int groceriesQuality;
						public int turnsWithoutGroceries=0;  //[0-3]

				public float funds;
					public float normalSpendingRate;
					public float ssCheck;

				//Quality variables that describe
				//-things that the actor uses, eats, consumes.

						public int turnsWithoutWater=0;
			
						//Access to Water is affected by:
							//water interruptions.
							//access to clear water.
							public int drinkingWaterAccess=3;  //[0-3], 3 is the best.

						//These values refer to epidemic events that do not occur in the current game.
						public int zikaRisk;	//increases if there's a floodig Incident near the citizen.
							public bool hasZika;
						public int influenzaRisk;
							public bool hasInfluenza;
							public bool hasInfluenzaVaccine;
						public int illnessRisk=0; //[0-3];
			};
		
		public Citizen retiree; //citizen1 is a retiree. I could name this one retiree.
			//Possible endings: abandoned, dead, sick, emigrated, or surviving

//B. Game Functions.
	//B-1. Setup functions performed at the beginning of a game session.
	void Awake()
	{
		gameManager = this;
	}

	// Use this for initialization
	void Start () {
		
		//gameManager starts first.

		//set logic variables.
		gameState=0;   //0 stands for -not playing yet-.
		currentTurnCounter=1;
		currentActorCounter=0;

		//Initialize our actor instances:
		wagency= new WaterAgency();
		retiree= new Citizen();
		hagency= new HealthAgency();

		//Creating the Dictionaries that will record the frequency of certain incidents.
		waterEventsDict = new Dictionary<string,int>();

			waterEventsDict.Add("W_monitoringOff", 0);
			waterEventsDict.Add("W_billIncrease", 0);
			waterEventsDict.Add("W_agencyCuts", 0);

			waterEventsDict.Add("W_noticeDroughtW", 0);
			waterEventsDict.Add("W_noticeDroughtC", 0);

			waterEventsDict.Add("W_heavyRainsMsg", 0);
			waterEventsDict.Add("W_rationDrought", 0);

		citizenEventsDict = new Dictionary<string,int>();
		
			citizenEventsDict.Add("C_changeHPlan", 0);
			citizenEventsDict.Add("C_droughtRisk", 0);
			citizenEventsDict.Add("C_fuMedsNoPay", 0);
			citizenEventsDict.Add("C_fuMedsOP", 0);
			citizenEventsDict.Add("C_hospitalization", 0);
			citizenEventsDict.Add("C_pharmacy", 0);

			citizenEventsDict.Add("C_DVSignup_far", 0);
			citizenEventsDict.Add("C_DVSignup_mid", 0);
			citizenEventsDict.Add("C_DVSignup_close", 0);
			citizenEventsDict.Add("C_DVEmg_far", 0);
			citizenEventsDict.Add("C_DVEmg_mid", 0);
			citizenEventsDict.Add("C_DVEmg_close", 0);

			citizenEventsDict.Add("C_shopping",0);
			citizenEventsDict.Add("C_billAdjustment",0);

		healthEventsDict = new Dictionary<string,int>();
		
			healthEventsDict.Add("H_zikaEmergency", 0);
			healthEventsDict.Add("H_influenzaEpidemic", 0);
			healthEventsDict.Add("H_doctorIncentive", 0);

			healthEventsDict.Add("H_cdtShift", 0);
			healthEventsDict.Add("H_payResidents", 0);
			healthEventsDict.Add("H_doctorPayComplaint", 0);
			healthEventsDict.Add("H_resourceShuffle", 0);
			healthEventsDict.Add("H_bloodDrive", 0);
			healthEventsDict.Add("H_preventiveZika", 0);

		//Declaring the health agency's doctor + facilities adjustment variables.
		hagency.maxNumOfHospitals=5;
		wagency.map_areThereIncidents_Array= new bool[7];
		hagency.isDoctorAssigned_toHospitals_Array=new bool[hagency.maxNumOfHospitals*2];
		hagency.areHospitalsOpen_Array=new bool[hagency.maxNumOfHospitals];


		//Setup the game variables' initial values.
		setupActorVariables();

		if(isTestingEvents)
			beginGamePlay();


	}

	//Function used to print the current game variables to the console.
	void printGameVariables()
	{

		print("=============>TURN#: "+currentTurnCounter+" , EVENT#: "+currentActorCounter);
		print(">>Water Agency variables:	"+"water level, "+wagency.waterLevel+"; funds, "+wagency.funds+"; maintenance crews, "+wagency.numPersonnel+"/"+wagency.numTotalPersonnel+".");
	
		print(">>Citizen variables: health, "+retiree.health+"; medicines, "+retiree.medicationsAmount+"/3; doctor access, "+retiree.accessToDoctor+"/3;");
		print("				   water access, "+retiree.drinkingWaterAccess+"/3; funds, "+retiree.funds+"/1000");

		print(">>Health Agency variables: "+"num of doctors, "+hagency.numOfDoctors+"; funds, "+hagency.funds+"; number of facilities, "+hagency.numOfOpenHospitals+"/5.");

	}

	//General purpose function that shuffles an array.
	void shuffleArray(string[] items)
    {
        // For each spot in the array, pick
        // a random item to swap into that spot.
        for (int i = 0; i < items.Length - 1; i++)
        {
            int j = Random.Range(i, items.Length);
            string temp = items[i];
            items[i] = items[j];
            items[j] = temp;
        }
    }


	//Function that resets all game variables, called at the beginning of a session.
	void setupActorVariables()
	{

		print ("--Setting up actor variables.");

		//0. Game logic variables.
		lastCitizenIncident_toAppear=' ';
		lastHealthEvent_toAppear= ' ';

		lastWaterEvent_name="";
		lastCitizenEvent_name="";
		lastHealthEvent_name="";


		//Setting this variable to make sure Water acts before Health.
		didWaterActInTurn=false;  		
		

		//1.Setting water agency values.
				wagency.funds= Random.Range(8f, 12f);//5f;  //This is a quarter of the max value, 20;

				wagency.waterLevel= Random.Range(14f,17f);//14f;  //This can be a random level between 10 and 20.
			
				wagency.waterUse=1.0f;
			
				if(isDroughtScenario)
					wagency.rainFall=0.3f;	//This can be randomized, will be randomized for heavy rains.
											//Heavy rains happen when triple this maount falls.
				else
					wagency.rainFall=1.3f;

				wagency.numPersonnel= 6;

				wagency.isWmonitoringOn=true;
				wagency.waterLevel_dangerState=2;
					wagency.isDroughtAnnounced=false;

				wagency.currentWaterIncident="None";
				wagency.incidentRisk=3;
				wagency.numberOfIncidents=0;
				wagency.isIncidentReported=false;

				//Initializing adjustment event-arrays.
				for(int i=0; i<wagency.map_areThereIncidents_Array.Length; i++)
					wagency.map_areThereIncidents_Array[i]=false;

		
		//2.Setting citizen values.
				retiree.health= Random.Range(11f, 15f);
				
				retiree.dentistEventState=0;

				retiree.medicationsAmount=Random.Range(1,4);  //3 is the highest level.
					retiree.turnsWithoutMedication=0; //decreases over time, recharged if taking meds.
				
					retiree.turnsWithoutSeeingDoctor=0;
					retiree.accessToDoctor=Random.Range(1,4);

				//Note: funds must be converted to meter values.
				retiree.funds= Random.Range(500,950f);//700f;  //The citizen's funds range from 0 to 1000 dollars.
					retiree.ssCheck=400f; //Monthly social security check received.
					retiree.healthcarePrimeBill=227f;
					retiree.waterBill=100f;

					retiree.areLabCostsCovered=true;
					retiree.isPayingFullHospitalization=false;

					retiree.healthIncidentRisk=3;
							retiree.currentHealthIncident_counter=0;
					
					//Setting health incident variables.
						shuffleArray(retiree.possibleIncidents);
						retiree.currentHealthIncident="None";
						retiree.turns_skippedWaterPayments=0;
						retiree.turns_skippedHealthPayments=0;
						retiree.isHealthCutOff=false;
						retiree.isWaterCutOff=false;

					retiree.currentFollowup="None";
							retiree.currentFollowup_counter=0;

					//Shuffle the health incident followups.
						shuffleArray(retiree.possibleFollowups);
						retiree.isSkippingFollowup=false;
						retiree.turnsWithoutFollowup=0;
						retiree.healthTransactionResult=0;

						retiree.didHealthIncidentAppear=false;
						retiree.didHealthFollowupAppear=false;

				
				
					retiree.turnsWithoutWater=0;		

					retiree.drinkingWaterAccess=3;
					retiree.isDrinkingWaterAccessIssueIntroduced=false;

					retiree.zikaRisk=0;	//[0-3];increases if there's a floodig Incident near the citizen.
						retiree.hasZika=false;
					
					retiree.groceriesAmount=2;

		//3.Setting health agency values.
				//This is not set to actual numbers. Should it?
				hagency.funds=Random.Range(8f,11f);  //say this is half of the maximum value.
				
				hagency.maxNumOfDoctors=9f;
				int newNumberOfDoctors=Random.Range((int)hagency.maxNumOfDoctors-2,(int)hagency.maxNumOfDoctors+1);
				hagency.numOfDoctors= newNumberOfDoctors; 

				hagency.numOfOpenHospitals=0;	

				//This epidemic variables are not used in this version of the simulation.
				hagency.zikaEpidemicRisk=1; //[0-3]
				hagency.influenzaEpidemicRisk=1; //[0-3]
				hagency.bloodSpending= 0.7f;

				//Setting the price of medications to be bought by the user.
				hagency.medicationsPrice=300f;

				//Setting the hospital event's adjustment variables.
				for(int i=0; i<hagency.isDoctorAssigned_toHospitals_Array.Length; i++)
					hagency.isDoctorAssigned_toHospitals_Array[i]=false;
				for(int i=0; i<hagency.areHospitalsOpen_Array.Length; i++)
					hagency.areHospitalsOpen_Array[i]=false;

				//This variable will keep track of changes in the first doctor array above.
				hagency.numOfAssignedDoctorUnits=0;
				hagency.wereDoctorUnitsModified="No";


		//4. Also: resetting our dictionary event values.
			List<string> wKeys = new List<string> (waterEventsDict.Keys);
			List<string> cKeys = new List<string> (citizenEventsDict.Keys);
			List<string> hKeys = new List<string> (healthEventsDict.Keys);


			//Since no event has yet occurred, all frequency variables are set to 0.
			foreach (string key in wKeys)
			{
				waterEventsDict[key] =0;
			}

			foreach (string key in cKeys)
			{
				citizenEventsDict[key] =0;
			}

			foreach (string key in hKeys)
			{
				healthEventsDict[key] =0;
			}

	}

	//Function that begins gameplay.
	public void beginGamePlay()
	{

		print("--Beginning game play.");

		printGameVariables();

		//This means the player has pressed the button to start the game.
		//We change the gameState to reflect this.
		gameState=1;

				

		//We also call the displayManager to update according to the gameState.
		//show the first turn, first event.

	//1.Code used to determine a specific scene, based on user input in the editor.
		//To run this section: uncomment these lines and comment the next block of code.

			//Testing a determined first scene, in this case, a Water Scene.
			// 			didWaterActInTurn=false;
			// 			if(actorToTest=='H')
			// 			{		
			// 				didWaterActInTurn=false;
			// 			}
			// 			else if(actorToTest=='W')
			// 			{
			// 				didWaterActInTurn=true;
			// 				wagency.incidentRisk=3;
			// 				wagency.tryToGenerateWaterIncident();
			// 			}
			// 				determineNextActor("C_");
			// 				newEventName=firstEventToTest;
							
			// 				prepareRandom_W_RepairIncidents();
			// 				H_resourceShuffle_assignAvailableDoctors();

			// 				//test: increase the number of doctors.
			// //				hagency.numOfDoctors=3f;
			// //				H_resourceShuffle_updateDoctors_and_Hospitals();
			// 				displayManager.reset_WRepairSprites();
			// 				displayManager.show_WRepairSprites();

			// 				displayManager.beginGameDisplay(firstEventToTest);



	//2. Starting code that generates a random event
		//Initializing our Health Shuffle and Water Repair-- Map Events:
				H_resourceShuffle_assignAvailableDoctors();

		//If incidentRisk is higher than 0, Let's try to generate an incident.
			if(wagency.incidentRisk>0 && wagency.currentWaterIncident=="None")
					{
							print("WATER AGENCY: Trying to generate Water Incident for citizen.");
							wagency.tryToGenerateWaterIncident();
					}
				if(wagency.currentWaterIncident != "None")
					print(">>>>>>WATER: water event generated: "+wagency.currentWaterIncident+".");
				else
					print(">>>>>>WATER: no water event generated.");

					//wagency.incidentRisk=1;

		//Setting a followup event and health incident to be called in the future:
			retiree.generateFollowupEvent();
			retiree.getNextHealthIncident();

			//Getting the Actor to start with from the public variables.
				if(actorToTest=='H')
				{		
					didWaterActInTurn=true;
					determineNextActor("C_");
					generateNewEvent("C_");
				}
				else if(actorToTest=='W')
				{
					didWaterActInTurn=false;
					wagency.incidentRisk=3;
					wagency.tryToGenerateWaterIncident();
					determineNextActor("C_");
					generateNewEvent("C_");

				}

				//sending in a health event makes generator prepare a citizen Event. Let's test this..
				//This means I'll probably want to randomize starting values.
				else
				{
					determineNextActor("H_");
					generateNewEvent("H_");
				}

				displayManager.beginGameDisplay(newEventName);


	}

//C.Game Logic methods.  These methods update the game's logic.

//This method is called every time the user presses a button (ie makes a choice)
//in the game.  It checks on actor parameters before firing an event.
public void manageGameLogic(string userChoiceSent)
{

		//Manages gameLogic choices that the user makes, and is sent
		//over by the display manager.
		print("		***userChoice is "+userChoiceSent);

		//a. Checking if you're resetting the game.
		if(userChoiceSent=="reset")
		{

			gameState=0;   //0 stands for -not playing yet-.
			currentTurnCounter=1;
			currentActorCounter=0;

			//begin game, just as seen in the Start Screen.
			setupActorVariables();

			//hide all events. 
			displayManager.hideAll_ActorEvents();
			//hide game ending panel.
			displayManager.showPanelsEnding(false);

			//reset maintenance sprites display, if partially hidden.
			displayManager.hide_Maintenance_Crew_afterFundingCuts(false);

			//reset the time-month display.
			displayManager.resetMonthDisplay();

			if(isTestingEvents)
				beginGamePlay();

			else   //3.14 a cheap way to turn the startPanel on. Could do an animation later.
				displayManager.showStartPanel();

		}

		//b.The user has selected a choice that impacts the game variables.
		else{
				
				//Continue statements don't affect variables.
				//if(userChoiceSent.Contains("Continue")) 
				
					//1.Apply changes to actor variables.
					updateGameVariables(userChoiceSent);
				
					//2.Check for ending condition.
					//Check for an ending condition.
						//is citizen compromised?
						//has citizen lost self-sufficiency?
					//If game is over: display gameOver routine.
					checkForEndingCondition();	
				
					//3. If the game is not over:
					if(gameState!=2)
						{
							
							//Check whether we're at the end of a turn, and 
							//end of turn updates must be applied to the variables.
								//To do this, first figure out who the newActor is going to be.
								bool isNextEvent_a_continueEvent=false;
								isNextEvent_a_continueEvent=determineNextActor(userChoiceSent);

							//Increase the game counters.

							//currentActorCounter increments only if the actors have changed.
								//between the previous actor and the current one.
								//the boolean return value from the function also keeps track of this.

								//if(currentActor != previousActor)      
								if(!isNextEvent_a_continueEvent)
									{
										currentActorCounter++;	
									}
				
								//Checking for the end of turn. 
									//Edit the modulus number to change turn length.
								if( currentActorCounter % actorEvents_perTurn == 0 && currentActorCounter>0) // 2 == 0 ) 
								{
									currentActorCounter=0;
									currentTurnCounter++;

									//updating turn here as well.
									displayManager.updateMonthDisplay();											

									//Perform the end-of-the-month update. Maybe this should have habits
									//own event at the start of the turn.
									updatevariables_EndOfMonth();
									
									//Checking ending condition again, in case the end of turn forces a change.
									checkForEndingCondition();

									if(gameState ==2) //if game is ending, go straight to the ending screen.
										{	}
									else 
										{
											//We're going to a new turn.
											generateNewEvent(userChoiceSent);
											//A new turn starts. Call the sequence for this event.
												//For now, the game doesn't offer an explanatory text at 
												//start of turn.
												//hiding the actor events here:
												displayManager.displayTurnAdvance(newEventName);
												
										}
								}

								else
									{
										//There's a new event, but we haven't reached the end of a turn.

										//Generates the new event, but does not display it (happens below).
										if(!isNextEvent_a_continueEvent)
										{
											generateNewEvent(userChoiceSent);	
											//updating the month display here.
											displayManager.updateMonthDisplay();				
										}
										//Go ahead and display the next event from the list.
										displayManager.prepareDisplayNextEvent(newEventName);//"C_changeHPlan");
									}

								//The new event name is used to update the currentActor var, by the way.
								print("Event# "+currentActorCounter+" name: "+newEventName);

						}

					//4.Check for a game-over state.
					if(gameState==2)
						{
							print("---NewGameManager.cs(), game has ended.");
							//disable icons here if needed.
							//Or show the reset option.	
							displayManager.prepareDisplayNextEvent(newEventName);
							displayManager.showPanelsEnding(true);
						}

			}	

		printGameVariables();


} //End of manageGameLogic() function.
	
//Helper methods for checking the game logic:
void checkForEndingCondition()
{
	print("CHECKING ENDING..");

	string retireeEnding="";
	bool isEndingReached=false;

//Retiree endings. 
	
	//A heatstroke ending.
	if(retiree.health<=0.3f && wagency.waterLevel_dangerState<2)
			{
				retireeEnding="C_r_endingSS";	//SS= Sunstroke ending.
				isEndingReached=true;
			}

	//A death ending.
	else if(retiree.health<=0.3f)
			{
				retireeEnding="C_r_endingDH";	//DH= death at home ending.
				isEndingReached=true;
			}
	
	//Low-parameter ending, or broke + unhealthy ending.
	else if ( 
				(	(retiree.health<=5f || retiree.funds<=400f)			&& currentTurnCounter>maxTurns) || 	
		
				(retiree.funds<125f ) //&& retiree.health <=5f )  //an early ending.
			
			)


			{
				retireeEnding="C_r_endingAH";	//AH= abandoned in hospital.
				isEndingReached=true;
			}

	//Mid-parameters ending, sick + dependent ending.
	else if (  
				(	 
					//(retiree.health<11f && retiree.health>5f  ||
					   retiree.funds>400f && retiree.funds<700f 
				
				 	&& currentTurnCounter>maxTurns )  

			)	 					
			{
				retireeEnding="C_r_endingSD";  //SD= reclusive and depressed.
				isEndingReached=true;
			}

	//High-parameters ending.
	else if(retiree.funds>=700f  && currentTurnCounter>maxTurns)	
			{
				retireeEnding="C_r_endingSH";	//SH= surviving with housekeeper
				isEndingReached=true;
			}


			//If any of the previous endings have been reached, change the game's state.
			if(isEndingReached)
				{	
					newEventName=retireeEnding;
					gameState=2;
					print("ENDING REACHED.");
				}
			else
				print("NO ENDING REACHED.");
}


//Main functions that update the game's variables based on user input.
void updateGameVariables(string actionReceived)
{
	
	//Performing each actor's updates in their own separate functions.
	updateGameVariables_WaterAgencyChoices(actionReceived);
	updateGameVariables_CitizenChoices(actionReceived);
	updateGameVariables_HealthAgencyChoices(actionReceived);


	//Before the update concludes, make sure the variable values are within min-max bounds.

	//Water.
		if(wagency.waterLevel<0f)
			wagency.waterLevel=0f;
		if(wagency.waterLevel>20f)
			wagency.waterLevel=20f;

		if(retiree.drinkingWaterAccess>3)
			retiree.drinkingWaterAccess=3;
		if(retiree.drinkingWaterAccess<1)
			retiree.drinkingWaterAccess=1;

		//Water Personnel.
		if(wagency.numPersonnel>wagency.numTotalPersonnel)
			wagency.numPersonnel=wagency.numTotalPersonnel;
		if(wagency.numPersonnel<0)
			wagency.numPersonnel=0;

	//Citizen
		//range check for accessToDoctor.
		if(retiree.accessToDoctor<1)
			retiree.accessToDoctor=1;
		if(retiree.accessToDoctor>3)
			retiree.accessToDoctor=3;

		if(retiree.drinkingWaterAccess<1)
			retiree.drinkingWaterAccess=1;
		else if(retiree.drinkingWaterAccess>3)
			retiree.drinkingWaterAccess=3;


	//Health.
		if(hagency.funds < 0f)
			hagency.funds=0f;
		else if(hagency.funds > 20f)
			hagency.funds=20f;

		if(hagency.numOfDoctors <0f)
			hagency.numOfDoctors=0f;
		else if(hagency.numOfDoctors >9f)
			hagency.numOfDoctors=9f;

}

void updateGameVariables_WaterAgencyChoices(string actionReceived)
{
	//Checking all Water agency choices.
	print("--->Updating Water variables.");

if(actionReceived[0]== 'W') //Skipping input processing if the action is not by the Water actor.
{
		
		//From the event 'Water Monitoring Off'
			if(actionReceived=="W_monitoringOff")
			{
				//Monitoring is turned off.

				//Agencies will not be able to keep track of water quality as well.
				wagency.isWmonitoringOn=false;

				if(wagency.incidentRisk<3)
					wagency.incidentRisk++;

			}

		//Note: Water Bill choices are now handled by the special Slider event.

		//From the event 'Agency cuts'
			//This one rarely appears in-game.
			else if(actionReceived=="W_reduceHours")
			{	
				print("&&&&& REDUCING WATER PERSONNEL.");

				wagency.numTotalPersonnel=3;
				if(wagency.numPersonnel > wagency.numTotalPersonnel)
					wagency.numPersonnel = wagency.numTotalPersonnel;

				//hide the reduced maintenance workers from the scene.
				displayManager.hide_Maintenance_Crew_afterFundingCuts(true);

				wagency.funds += 4f;
			}

			else if(actionReceived=="W_reduceMaintenance")
			{
				//Water incident Risks increase.
				if(wagency.incidentRisk<3)
					wagency.incidentRisk++;
				
				wagency.funds += 3f;
			}


		//After the repair event is finished, reset one of the event's variables.
			if(actionReceived=="W_repair")
			{
				//Do nothing.
				wagency.numberOfRepairs_Selected=0;
			}

		//From the event 'Ration Water-drought'
			//Note: This is handled with the arrow buttons now.
			

		//From the event 'Water brigade'
			//Another event I want to do on the map.
			float extraWaterUse=0f;
			
			if(actionReceived=="W_brigadeU")
			{
				
				extraWaterUse = 2f;
				
				wagency.waterLevel -=extraWaterUse;

				if(wagency.waterLevel<0f)
					wagency.waterLevel=0f;

			}

			else if(actionReceived=="W_brigadeUR")
			{

				extraWaterUse = 1f;
				
				wagency.waterLevel -=extraWaterUse;
				if(wagency.waterLevel<0f)
					wagency.waterLevel=0f;

			}

		

}

//I may do some introductory events, to explain what's happening.

//-----end of Water agency event check-------///

	
		//If incidentRisk is higher than 1, Let's try to generate an incident.
		if(wagency.incidentRisk>0 && wagency.currentWaterIncident=="None")
			wagency.tryToGenerateWaterIncident();

		//If there is a water incident, then deduct water every turn (even if juts a little).
		if(wagency.currentWaterIncident != "None")
			{
				//Prepare the incident arrays, and later update the display up.

				print("WATER INCIDENT:<<<<<<<Water incident detected, leaking water.");

				float waterLost=0f;

				//Broken Pipe, Service Cutoff, Dirty Water.

				if(wagency.currentWaterIncident == "Broken Pipe")
					waterLost=0.5f;

				if(wagency.currentWaterIncident == "Dirty Water")
					waterLost=0.25f;

				int numberOf_Incidents_inIsland=0;
				//For now, we make the water loss from island-wide incidents
					//dependent on whether the retiree suffers an incident.
				for(int i=0; i<wagency.map_areThereIncidents_Array.Length; i++)
				{
					if(wagency.map_areThereIncidents_Array[i])
						numberOf_Incidents_inIsland++;
				}

				//deduct the water available.
				wagency.waterLevel -=waterLost*numberOf_Incidents_inIsland;
				if(wagency.waterLevel<0f)
					wagency.waterLevel=0f;
			}


}

void updateGameVariables_CitizenChoices(string actionReceived)
{
//Updating  the citizen's variables.
		float healthPenalty=0f;

if(actionReceived[0]== 'C') //Skipping input processing if the action is not by the Citizen actor.
{

		//Note: health is on a [0-20] scale,
		//but funds are on a bigger scale (0-1000 dollars)

		//For the event 'sudden doctor visit'
			if(actionReceived=="C_doctorVSignup")
			{

				//Depending on how far you are to the doctor, you will recover less health.
				if(retiree.accessToDoctor==1)  //worst case.
				{
					//See doctor, but lose time and money.  Went to a bad clinic.
					retiree.funds -= 325f;
					retiree.health = 14f;
				}
				else if(retiree.accessToDoctor==2)  //medium case.
				{
					//Spent way too much time. Spent money. Time=money in this case.
					retiree.funds -= 250f;
					retiree.health = 15f;
				}
				else
				{
					//It wasn't that bad.
					retiree.funds -= 125f;
					retiree.health = 15f;
				}

				//updating doctor visit.
				retiree.turnsWithoutSeeingDoctor=0;
				retiree.healthIncidentRisk=2;

				//updating doctor visit.
				retiree.turnsWithoutSeeingDoctor=0;
				//resetting this health check to disable health penalties related to it.
				retiree.didHealthIncidentAppear=false;

				retiree.currentHealthIncident="None";


			}

			//For the event: 'emergency doctor visit'.
			else if(actionReceived=="C_doctorVEmg")
			{

				//Depending on how far you are to the doctor, you will have a worse time.
				if(retiree.accessToDoctor==1)  //worst case.
				{
					//See doctor, but lose time and money.  Went to a bad clinic.
					hagency.funds -= 1.3f;
					retiree.funds -= 75f;
					retiree.health = 14f;

					//Resetting the healthIncidentAppear variable.
				}
				
				else if(retiree.accessToDoctor==2)  //medium case.
				{
					//Spent way too much time. Spent money. Time=money in this case.
					hagency.funds -= 1f;
					retiree.funds -= 35f;
					retiree.health = 15f;
				}
				
				else
				{
					//It wasn't that bad.
					hagency.funds -= 0.7f;
					retiree.health = 16f;
				}


				//updating doctor visit.
				retiree.turnsWithoutSeeingDoctor=0;
				//resetting this health check to disable health penalties related to it.
				retiree.didHealthIncidentAppear=false;

				retiree.currentHealthIncident="None";


			}	

			//For the player choice: 'postpone a doctor visit'
			else if(actionReceived=="C_doctorVPostpone")
			{
				//The chance of a health issue increases.

				if(retiree.healthIncidentRisk<3)
					retiree.healthIncidentRisk++;

			}

		//The following checks are for  the events 'Citizen Bill Adjustment'.
			//Appear when citizen skips a bill payment.
			else if(actionReceived=="C_delayWBill")
			{

				if(retiree.turns_skippedWaterPayments >= 2)
					retiree.turns_skippedWaterPayments++;
			
			}

			else if(actionReceived=="C_delayHBill")
			{
			
				//affect the logic.
					//debt counter adds up. 
					//Will have to pay up in the next month.
						//or, services are cut.

				if(retiree.turns_skippedHealthPayments >= 2)
					retiree.turns_skippedHealthPayments++;

			}

			else if(actionReceived=="C_lessGroceries")
			{
				//affect the logic.
				if(retiree.healthIncidentRisk<3)
					retiree.healthIncidentRisk++;
				
			}

			else if(actionReceived=="C_payOverdueW")
			{
				//affect the logic.
					//debt counter adds up. 
					//Will have to pay up in the next month.
						//or, services are cut.
			
					retiree.funds -= 200f;
					if(retiree.funds < 0f)
						retiree.funds =0f;

					retiree.turns_skippedWaterPayments=0;
			
			}

			else if(actionReceived=="C_payOverdueH")
			{
			
				//affect the logic.
					//debt counter adds up. 
					//Will have to pay up in the next month.
						//otherwise, services are cut.

					retiree.funds -= 200f;
					if(retiree.funds < 0f)
						retiree.funds =0f;

					retiree.turns_skippedHealthPayments=0;
			}


			else if(actionReceived=="C_waterCutoffMsg")
			{
					retiree.isHealthCutOff=true;
			}

			else if(actionReceived=="C_healthCutoffMsg")
			{
					retiree.isWaterCutOff=true;
			}

		//For the 'reporting water incidents' events

			else if(actionReceived=="C_reportIncidentBW")
			{
				//Reporting Broken Water Pipe
				wagency.isIncidentReported=true;
				print("Citizen reports W issue: wagency.isIncidentReported is "+wagency.isIncidentReported+".");

				if(retiree.drinkingWaterAccess>1)
					retiree.drinkingWaterAccess--;

				//Prepare the water incidents map as well.
				prepareRandom_W_RepairIncidents();
			}

			else if(actionReceived=="C_reportIncidentDW")
			{
				//Reporting Dirty Water.
				wagency.isIncidentReported=true;
				print("Citizen reports W issue: wagency.isIncidentReported is "+wagency.isIncidentReported+".");

				if(retiree.drinkingWaterAccess>1)
					retiree.drinkingWaterAccess--;

				//Prepare the water incidents map as well.
				prepareRandom_W_RepairIncidents();

			}	

			else if(actionReceived=="C_reportIncidentWC")
			{
				//Reporting Water Cutoff.
				wagency.isIncidentReported=true;
				print("Citizen reports W issue: wagency.isIncidentReported is "+wagency.isIncidentReported+".");

				if(retiree.drinkingWaterAccess>1)
					retiree.drinkingWaterAccess--;

				//Prepare the water incidents map as well.
				prepareRandom_W_RepairIncidents();

			}

	
		//For the event 'Citizen- Buying Medicines':		
			else if(actionReceived=="C_medsPay")
			{
				retiree.funds -= hagency.medicationsPrice;

				if(retiree.funds <0)
					retiree.funds = 0f;

				//Medications go back to full amount.
				retiree.medicationsAmount=3;

			}

			else if(actionReceived=="C_dontPayMeds")
			{
				//Do nothing for now.
					if(retiree.healthIncidentRisk<3)
					retiree.healthIncidentRisk++;
			}	

		//For the event 'Citizen- Shopping Adjustment':		
			//buy Rice, Vegetables and Fruit.
			else if(actionReceived=="C_shopRVF")
			{
				retiree.funds -= 200f;

				if(retiree.funds <0)
					retiree.funds = 0f;

				if(retiree.groceriesAmount<3)
					retiree.groceriesAmount=3;

				retiree.groceriesQuality= 3;

				retiree.health+=1f;

			}

			//buy Bread, Canned Food and Snacks.
			else if(actionReceived=="C_shopBCS")
			{
				retiree.funds -= 200f;

				if(retiree.funds <0)
					retiree.funds = 0f;

				if(retiree.groceriesAmount<3)
					retiree.groceriesAmount=3;

				retiree.groceriesQuality= 2;

				//no health boost.
					//retiree.health+=1f;

			}

			//buy Sodas, Pastries and Candy.
			else if(actionReceived=="C_shopSPC")
			{
				retiree.funds -= 125f;

				if(retiree.funds <0)
					retiree.funds = 0f;

				if(retiree.groceriesAmount<3)
					retiree.groceriesAmount=3;

				retiree.groceriesQuality= 1;

				retiree.health-=1f;

			}

		//For the event 'Dentist Visit':		
			else if(actionReceived=="C_dentistVisit")
			{
				retiree.funds -= 300f;

				if(retiree.funds <0)
					retiree.funds = 0f;
					
				//resetting this health check to disable health penalties related to it.
				retiree.didHealthIncidentAppear=false;

				retiree.dentistEventState=3; //3 means solved.
			}

			else if(actionReceived=="C_dentistPostpone")
			{
				retiree.health -= 2f;

				if(retiree.health <0)
					retiree.health = 0f;


				if(retiree.healthIncidentRisk<3)
					retiree.healthIncidentRisk++;

			}

		//For the event 'Emergency Dentist Visit':		
			else if(actionReceived=="C_EmgDentistVisit")
			{

				retiree.funds -= 250f;

				if(retiree.funds <0f)
					retiree.funds = 0f;

				retiree.dentistEventState=3; //3 means solved.
	
			}

			else if(actionReceived=="C_EmgDentistHospital")
			{
				//The health agency pays.
				hagency.funds -= 2f;

				if(retiree.funds <0f)
					retiree.funds = 0f;

				retiree.dentistEventState=3; //3 means solved.


			}

			//The shopping or doctor visit event doesn't do anything.
				//No parameters changed.

		//For the event 'C_droughtRisk':		
			else if(actionReceived=="C_droughtDrink")
			{
				retiree.funds -= 125f;

				if(retiree.funds <0f)
					retiree.funds = 0f;

				retiree.health += 2f;

				if(retiree.health >20f)
					retiree.health=20f;
			
				//update Water access variable.
				if(retiree.drinkingWaterAccess<3)
					retiree.drinkingWaterAccess++;

			}

		
		//For the event 'Healthcare plan change'.
			//raise co-pay by 20%.
			else if(actionReceived=="C_copay20")
			{
				retiree.healthcarePrimeBill *=1.20f;
				retiree.funds -= retiree.healthcarePrimeBill;
			}

			//no hospitalization covered.
			else if(actionReceived=="C_noHosp")
			{
				retiree.isPayingFullHospitalization=true;  //this will affect hospital stays.				
			}

			//no radiologoy (Xrays) covered
			else if(actionReceived=="C_noXrays")
			{
				//do something here with lab costs. This will come up if labs are required.
				retiree.areLabCostsCovered=false;
			}

	//For the 'follow-up' medical events.	
			//Follow up event: extra medication.	
				if(actionReceived=="C_fuMedsNoPay")
				{
					retiree.isSkippingFollowup=true;
				
				}

				else if(actionReceived=="C_fuMedsOP")
				{

					//resetting followup variables.
						retiree.currentFollowup = "None";
						retiree.isSkippingFollowup= false;
						//resetting this variable.
							retiree.didHealthFollowupAppear=false;

					retiree.funds -= Random.Range(hagency.medicationsPrice*0.65f, hagency.medicationsPrice*1.2f);
					if(retiree.funds < 0f)
						retiree.funds=0f;

					retiree.health += 1f;
					if (retiree.health > 20f)
						retiree.health= 20f;


				}
				//The other extra med events are continue statements.


			//Follow up event: bloodwork request.	
				if(actionReceived=="C_fuLabsHI")
				{
						//Using health insurance to pay for bloodwork.
						//There's a chance you'll pay full price.
						
						if(retiree.areLabCostsCovered)
						{
							//roll the dice.
							int chance=0; //1-3; 30% chance of paying full price. 

							chance=Random.Range(1,4);

								if(chance<3)
								{
									//covered mostly by medicare (80%)
									retiree.healthTransactionResult=1;
									retiree.funds -= hagency.labsPrice*0.2f;
								}

								else
								{
									//pay full price.
									retiree.healthTransactionResult=2;
									retiree.funds -= hagency.labsPrice;
								}

						}

						else
						{
							//You have to pay full price, same as in the out-of-pocket event.
							retiree.healthTransactionResult=3;
							retiree.funds -= hagency.labsPrice;
						}

							//resetting followup variables.
								retiree.currentFollowup = "None";
								retiree.isSkippingFollowup= false;
								//resetting this variable.
									retiree.didHealthFollowupAppear=false;

							//Checking variables are not out of range.
							if(retiree.funds<0f)
								retiree.funds =0f;

				}

				//Paying lab costs out of pocket.
				else if(actionReceived=="C_fuLabsOP")
				{

					//resetting followup variables.
						retiree.currentFollowup = "None";
						retiree.isSkippingFollowup= false;

						retiree.funds -= hagency.labsPrice;
						//resetting this variable.
							retiree.didHealthFollowupAppear=false;

						//Checking variables are not out of range.
						if(retiree.funds<0f)
							retiree.funds =0f;
				}

				//Not getting lab tests.
				else if(actionReceived=="C_fuLabsNoPay")
				{
					retiree.isSkippingFollowup=true;
				}

			//Follow up events, X-rays and radiology requested.	
				if(actionReceived=="C_fuXraysHI")
				{

					//Using health insurance to pay for Xrays.
					//There's a chance you'll pay full price.
					
					//Throw the dice here.
						//What do I do then, with the result?
						//Where do I change my variables?

						//roll the dice.
						int chance=0; //1-3; 30% chance of paying full price.

						chance=Random.Range(1,4);

							if(chance<3)
							{
								//covered mostly by medicare (80%)
								retiree.healthTransactionResult=1;
								retiree.funds -= hagency.xRaysPrice*0.2f;

							}

							else
							{
								//pay full price.
								retiree.healthTransactionResult=2;
								retiree.funds -= hagency.xRaysPrice;

							}

						//resetting followup variables.
							retiree.currentFollowup = "None";
							retiree.isSkippingFollowup= false;
							//resetting this variable.
								retiree.didHealthFollowupAppear=false;


						//Checking variables are not out of range.
						if(retiree.funds<0f)
							retiree.funds =0f;

				}

				else if(actionReceived=="C_fuXraysOP")
				{
					//resetting followup variables.
						retiree.currentFollowup = "None";
						retiree.isSkippingFollowup= false;

					retiree.funds -= Random.Range(hagency.labsPrice*0.8f, hagency.labsPrice*1.1f);

					//resetting this variable.
					retiree.didHealthFollowupAppear=false;

					//Checking variables are not out of range.
					if(retiree.funds<0f)
							retiree.funds =0f;

				}

				//Not getting Xrays done.
				else if(actionReceived=="C_fuXraysNP")
				{
					//Health risk increases.
					retiree.isSkippingFollowup=true;

				}


		//Handling the hospitalization event here.
				//Citizen agrees to be hospitalized, uses health insurance.
				if(actionReceived=="C_hospHI")
				{
					//Lose money.
					if(retiree.isPayingFullHospitalization)
						retiree.funds -= hagency.hospPrice;
					else
						retiree.funds -= hagency.hospPrice*0.3f;

					//Recover health, to a point.
						if(retiree.accessToDoctor==3)
							retiree.health = 13f;
						if(retiree.accessToDoctor==2)
							retiree.health = 9f;
						if(retiree.accessToDoctor==1)
							retiree.health = 5f;

					//Checking variables are not out of range.
						if(retiree.funds<0f)
							retiree.funds =0f;

					//Reset all health incident variables.  You are healthy for now.
					retiree.currentHealthIncident="None";
					retiree.currentFollowup="None";
					retiree.dentistEventState=0;

				}
				//Citizen refuses to be hospitalized.
				else if(actionReceived=="C_HRefuseOutcome")
				{
					retiree.health -= 5f;
					if (retiree.health < 0f)
						retiree.health= 0f;
				}


		//Citizen funds, variable range check.
		if(retiree.funds >1000f)
			retiree.funds=1000f;
		else if(retiree.funds<0f)
			retiree.funds=0f;

		
}

//----End of events check for Citizen----//

	//If incidentRisk is higher than 1, Let's try to generate an incident.
	if(retiree.currentHealthIncident=="None" && retiree.currentHealthIncident_counter< retiree.possibleIncidents.Length)
			{
				print("CITIZEN: Trying to generate Health Incident.");
				retiree.getNextHealthIncident();
			}

	//End of action received.
	//Illness check. If there is a health incident, then deduct health every turn (even if just a little).
		if(retiree.didHealthIncidentAppear )
			{
				print("HEALTH INCIDENT:<<<<<<<Health incident detected: "+ retiree.currentHealthIncident+".");

				float healthLost=0f;

				//Flu, Fall, Root Canal.

				if(retiree.currentHealthIncident == "Flu")
					healthLost=1f;

				else if(retiree.currentHealthIncident == "Fall")
					healthLost=0.5f;

				else if(retiree.currentHealthIncident == "Root Canal")
					healthLost=0.7f;

				//deduct the water available.
				retiree.health -=healthLost;
				if(retiree.health<0f)
					retiree.health=0f;
			}

}

//The following are variable update methods for the Health Agency.
public void tryEnding_shuffleEvent()
{
	//if there are still doctors that need to be assigned,
		//don't go to the next turn just yet.
	if(hagency.numOfAssignedDoctorUnits<(int)hagency.numOfDoctors) 
	{
		//show a status Message, and don't proceed to next turn.
		displayManager.update_ShuffleEvent_StatusMessage("notEndingYet");
	}

	else		//else, the end of turn is allowed.
		manageGameLogic("H_shuffleUpdate");
}

void updateGameVariables_HealthAgencyChoices(string actionReceived)
{

	//For this actor, funds are abstract (0-20f), 
		//and the number of doctors range from [0-9000]


if(actionReceived[0]== 'H') //Skipping input processing if the action is not by the Health actor.
{

		//From the health resource shuffle event:
			//When the user makes all the doctor assignments,
			//and they press the end-of-turn button, 
				//this choice will be activated.
			if(actionReceived=="H_shuffleUpdate")
			{
				//this settles the issue of doctor assignments, 
					//until the next time the # of doctors changes.
				hagency.wereDoctorUnitsModified = "No";

			}

		//For the event 'Doctor Incentives'.
			else if(actionReceived=="H_lowerTax4")
			{
				hagency.funds -= 4f;
				hagency.numOfDoctors += 1f;  //override the doctor reduction below.
				H_resourceShuffle_updateDoctors_and_Hospitals();
			}

			//leave centers open, with bare minimum of doctors required, only leave emergency rooms open.
			else if(actionReceived=="H_scholarships")
			{
				hagency.funds -= 2f;
				hagency.numOfDoctors += 3.2f;  //override the doctor reduction below.			
				retiree.accessToDoctor ++;
				H_resourceShuffle_updateDoctors_and_Hospitals();
			}

		//For the event group 'Preventive visit communities'.
			//Epidemics may not affect the citizen,
				//but they make the health agency spend more money.
				//citizens will have a harder time finding a doctor.

			//Media campaign for Zika awareness.
				else if(actionReceived=="H_mediaZika")
				{
					hagency.funds -= 2f;
					hagency.zikaEpidemicRisk --;
				}

				//Organize communities to clear up standing water.
				else if(actionReceived=="H_clearStandingW")
				{
					hagency.funds -= 4f;
					hagency.zikaEpidemicRisk--;
				}

				//Don't do anything to prevent zika.
				else if(actionReceived=="H_zikaNoPlan")
				{
					hagency.zikaEpidemicRisk--;

				}

			//Event: preventive Influenza
				else if(actionReceived=="H_influenzaVaccine")
				{
					hagency.influenzaEpidemicRisk--;
				}


		//For the event 'Blood Drives'.
			//The more blood received, the less money spent on buying blood.
			else if(actionReceived=="H_bloodMedia")
			{
				hagency.funds -= 1f;
				hagency.bloodSpending= 0.3f;  
			}

			else if(actionReceived=="H_bloodDrive")
			{
				hagency.funds += 1.5f;
				hagency.bloodSpending= 0.1f;  			
			}			
	
			else if(actionReceived=="H_bloodPostpone")
			{
				//Do nothing.
			}

		//For the events 'Federal Funding Received'.
			else if(actionReceived=="H_fundingH")
			{
				hagency.funds += 7f;
			}

			else if(actionReceived=="H_fundingM")
			{
				hagency.funds += 4f;
				hagency.numOfDoctors -= 2f;  
				H_resourceShuffle_updateDoctors_and_Hospitals();			
			}			
	
			else if(actionReceived=="H_fundingL")
			{
				hagency.funds += 2f;
				hagency.numOfDoctors -= 4f;  
				H_resourceShuffle_updateDoctors_and_Hospitals();			
			}
				
		//These two health emergency events do not surface in the current game.
			//For the event: Zika emergency
				else if(actionReceived=="H_zikaFunds")
				{
					hagency.funds += 3f;
				}

			//For the event: Influenza emergency.
				else if(actionReceived=="H_inflMedFreeze")
				{
					hagency.medicationsPrice=150f;

				}

}//End of health function check.

//---End of input processing. 
	//Checking the range of our two health agency variables displayed.

	if(hagency.numOfDoctors>9f)
		hagency.numOfDoctors=9f;
	else if(hagency.numOfDoctors<0f)
		hagency.numOfDoctors=0f;

	
	if(hagency.funds >20f)
		hagency.funds=20f;
	else if(hagency.funds<0f)
		hagency.funds=0f;

}

//Update function that runs at the end of each month.
	//Water Reservoir levels are updated, Doctor numbers are reduced.
	//The citizen's health drops if suffering a health issue.
	//They also receive their social security check.
void updatevariables_EndOfMonth()
{

		print("END OF MONTH: monthly variable update.");

		//Variables updated monthly are:

	//Water:
			//Recalculate waterUse and Rainfall. Water Levels drop if in a drought scenario.
			if(isDroughtScenario)
				wagency.waterUse= Random.Range(3f,5f);
			else
				wagency.rainFall= Random.Range(5f,8f);		//If greater than 2, show a rainfall event.

			if(isDroughtScenario && currentTurnCounter > (int)(maxTurns/2))
				wagency.rainFall=Random.Range(2f, 3f);

			if(wagency.waterLevel_dangerState<2)
				wagency.waterLevel -= wagency.waterUse*((float)(wagency.ration_daysWithWater)/7f);	
			else
				wagency.waterLevel -= wagency.waterUse; 
	
			wagency.waterLevel += wagency.rainFall;	 //rainFall is 0 if there's a Drought.

			if(wagency.waterLevel < 0f)
				wagency.waterLevel= 0f;

			if(wagency.waterLevel > 20f)
				wagency.waterLevel= 20f;


			//Zika chances rise if rainfall increases.
			if(wagency.waterLevel >= wagency.overflowWaterLevel && wagency.rainFall > 1f)
				{
					if(hagency.zikaEpidemicRisk < 3)
						hagency.zikaEpidemicRisk++;
				}

			//Go to the drought scenario if levels are low enough.
			if(wagency.waterLevel <= wagency.warningWaterLevel &&
				wagency.waterLevel_dangerState > 1)
				{
					print("WATER:Entering Warning Drought Level.");
					wagency.waterLevel_dangerState=1;
					wagency.rainFall=0;
				}

			else if(wagency.waterLevel <= wagency.criticalWaterLevel &&
				wagency.waterLevel_dangerState > 0)
				{
					print("WATER:Entering Critical Drought Level.");
					wagency.waterLevel_dangerState=0;
					wagency.rainFall=0;
				}

			//At the end of the turn, refresh the maintenance crews back to max.
			wagency.numPersonnel=wagency.numTotalPersonnel;
	

		//Health: Only setting the variable ranges.

			if(hagency.funds < 0f)
				hagency.funds=0f;
			else if(hagency.funds > 20f)
				hagency.funds=20f;

			if(hagency.numOfDoctors <0f)
				hagency.numOfDoctors=0f;
			else if(hagency.numOfDoctors >9f)
				hagency.numOfDoctors=9f;


	//Citizen:

		//Generating follow up medical events, to be used later.
		if(retiree.currentFollowup == "None" && retiree.currentFollowup_counter<retiree.possibleFollowups.Length)
			{
				retiree.generateFollowupEvent();
				print(">>>>>>CITIZEN: follow-up event generated: "+retiree.currentFollowup+".");
			}

			
			//Social security check received.
			retiree.funds += retiree.ssCheck;
			if(retiree.funds > 1000f)
				retiree.funds =1000f;
	
		//General variable update. 
				if(retiree.medicationsAmount >0)				
					retiree.medicationsAmount--;
				else if(retiree.turnsWithoutMedication<3)
					retiree.turnsWithoutMedication++;

				if(	retiree.turnsWithoutSeeingDoctor<3)
					retiree.turnsWithoutSeeingDoctor++;

				if(retiree.groceriesAmount >0)	
					retiree.groceriesAmount--;
				else if(retiree.turnsWithoutGroceries<3)
					retiree.turnsWithoutGroceries++;

				//Updating months without paying bills (if bill is skipped)
				if(retiree.turns_skippedHealthPayments >0)
					retiree.turns_skippedHealthPayments++;

				if(retiree.turns_skippedWaterPayments >0)
					retiree.turns_skippedWaterPayments++;

				if(retiree.turns_skippedHealthPayments > 2 && 
					!retiree.isHealthCutOff)
					retiree.isHealthCutOff=true;

				if(retiree.turns_skippedWaterPayments > 2 && 
					!retiree.isWaterCutOff)
					retiree.isWaterCutOff=true;


			//2.Applying health penalties activated above.

				float healthPenalty=0f;

				if(retiree.drinkingWaterAccess == 2)
						healthPenalty += 0.3f;
				if(retiree.drinkingWaterAccess == 1)
						healthPenalty += 0.5f;
				else if(retiree.drinkingWaterAccess == 0)
						healthPenalty += 1f;

				if(retiree.turnsWithoutMedication<=0)
					healthPenalty += 2f;
				if(retiree.turnsWithoutGroceries<=0)
					healthPenalty += 1f;


				if(retiree.dentistEventState==1)
					healthPenalty += 0.5f;
				else if(retiree.dentistEventState==2)
					healthPenalty += 1.3f;

				if(retiree.currentHealthIncident != "None" && retiree.didHealthIncidentAppear)
					healthPenalty += 1.5f;
				
				if(retiree.currentFollowup != "None" && retiree.didHealthFollowupAppear)
					{
						healthPenalty += 1f;
						
					}

				if(wagency.currentWaterIncident=="Broken Pipe")
				{	
					//Zika risk is increased.
					if(retiree.zikaRisk <0)				
					retiree.zikaRisk++;
				}

				//Health emergency variables:
				if(retiree.hasZika)
					healthPenalty += 4f;
				else if(retiree.hasInfluenza)
					healthPenalty += 3f;

					//Applying the health penalty.
					retiree.health -= healthPenalty*0.3f;

			//End of Citizen variable update.

}



//D.Special methods to handle Slider and arrow Button requests, for all actors.

	//--Handling presses to the drought-rationing arrows.
		public void rationingPeriod_Changed(bool areWaterDaysDecreased)
		{

			if(areWaterDaysDecreased)
				wagency.ration_daysWithWater--;

			else 
				wagency.ration_daysWithWater++;

			//Checking the variable's range.
			if(wagency.ration_daysWithWater<0)
				wagency.ration_daysWithWater=0;

			else if(wagency.ration_daysWithWater>7)
				wagency.ration_daysWithWater=7;
			
			//Changing the citizen's drinkingWaterAccess variable.
			if(wagency.ration_daysWithWater<3)
				retiree.drinkingWaterAccess=1;
			else if(wagency.ration_daysWithWater<6)
				retiree.drinkingWaterAccess=2;
			else
				retiree.drinkingWaterAccess=3;

			//Update the Display Manager, but just the second text parameter for now.
			displayManager.update_Drought_Ration_Text();

		}

//Modifying the water Bill amount with a slider.
		void waterBill_Changed(float newValueChange, float newValue )
		{

			//Update the value on the gameManager, and also on the DisplayManager.
				//Water bill, increases funds for wagency.
						//decreases citizen funds.
						//at High levels, decreases water availability for citizen.
						//at High levels, decreases funds for health agency.
					
					float newFunds=newValueChange*0.5f;
					
					//Increases funds for wagency.
					wagency.funds += newFunds;
			
						if(wagency.funds>20f)
							wagency.funds=20f;
						else if(wagency.funds<0f)
							wagency.funds=0f;

					//Decreases citizen funds.
					retiree.funds  -= (newFunds*100f);

					//at high levels, decreases water availability for citizen.
					//also at high levels, decreases funds for health agency.
					if((int)newValue>=8)
					{
						retiree.drinkingWaterAccess -= (int)newValueChange;
						hagency.funds -= newValueChange*0.5f;
					}

					if((int)newValue==7 && (int)newValueChange<0)
					{
						retiree.drinkingWaterAccess -= (int)newValueChange;
						hagency.funds -= newValueChange*0.5f;
					}

					//Finally, call a display update to the modified parameters.
					displayManager.update_Display_from_WaterBillSlider();

		}


		void health_Residencies_Changed(float newValueChange, float newValue)
		{
				
					//Updating the number of health residencies according to a slider input.
			
					float newResidentsIncrease=newValueChange;
					print("health slider value delta is "+newValueChange);

					//Increases doctor numbers in hagency.
						//note: doctors only go up to 9f;
					hagency.numOfDoctors += newResidentsIncrease;
					H_resourceShuffle_updateDoctors_and_Hospitals();
					print("Sliding number of Doctors: "+hagency.numOfDoctors);

					//Increases citizen's doctor access slightly.
					int centeredValue=(int)newValue-2;

					if(centeredValue==-2 || centeredValue==0 && (int)newValueChange==-1)
						retiree.accessToDoctor--;
					
					if(centeredValue==2 || centeredValue==0 && (int)newValueChange==1)
						retiree.accessToDoctor++;

						//range check for accessToDoctor.
						if(retiree.accessToDoctor<1)
							retiree.accessToDoctor=1;
						if(retiree.accessToDoctor>3)
							retiree.accessToDoctor=3;
						

					//Decreases citizen funds (in the opposite direction).
					hagency.funds  -= newValueChange*2.22f;
					print("Sliding number of Health funds: "+hagency.funds);


					//Finally, call a display update to the modified parameters.
					displayManager.update_Display_from_HResidents_Slider();

		}


		//Function that processes Slider requests.
		public void updateGameValuesBySlider(float deltaValue, float newValue, string sliderType)
		{
			if(sliderType=="WBill")
				waterBill_Changed(deltaValue, newValue);
			if(sliderType=="HRes")
				health_Residencies_Changed(deltaValue, newValue);

		}


		//Functions that pre-emptively check whether a parameter can 
			//be updated through a button press.
			public bool check_IconButton_Parameter()
			{
				if(newEventName=="W_infrRepair")
					return areThereEnoughPersonnel_ToRepair();

				else
					return false;		

			}

			public bool areThereEnoughPersonnel_ToRepair()
			{
				bool result=false;

				if(wagency.numPersonnel>0)
					result=true;
				else
					{
						displayManager.flashRepairWarning();
						result= false;
					}

				return result;
			}

			public bool areThereEnoughDoctors_toAdd_toHospital()
			{
				bool result=false;


				if(hagency.numOfAssignedDoctorUnits<(int)hagency.numOfDoctors) //each doctor icon stands for 1000 doctors.
					result=true;
				else
					{
						displayManager.update_ShuffleEvent_StatusMessage("noDoctors");
						result= false;
					}

				print("#######	areThereEnoughDoctors_toAdd_toHospital? numOfDoctors: "+hagency.maxNumOfDoctors+", result: "+result+".");

				return result;
			}

	//Funnctions specifit to the health agency's doctor-hospital management.
		public void update_H_resourceShuffle_fromButton(int doctorIndex, bool isAssigningDoctor)
		{
			if(isAssigningDoctor)
			{
				//Adding a doctor to a hospital.				
					hagency.isDoctorAssigned_toHospitals_Array[doctorIndex]=true;
					hagency.numOfAssignedDoctorUnits++;

					//Fetching the current hospital from the doctor index supplied.
					int hospitalIndex=doctorIndex/2;
					
					//If there are any doctors attached to this hospital, and the hospital is closed,
						//OPEN the hospital.
					if( 
						(hagency.isDoctorAssigned_toHospitals_Array[hospitalIndex*2]
						||
					   hagency.isDoctorAssigned_toHospitals_Array[hospitalIndex*2+1])
					   	&&
					   !hagency.areHospitalsOpen_Array[hospitalIndex])
						{
							hagency.areHospitalsOpen_Array[hospitalIndex]=true;	
						
							hagency.numOfOpenHospitals++;

							//Update the hospital display.
							displayManager.update_HShuffle_hospitalIcon(hospitalIndex, true);
						
							//Health agency loses some money.
								//Also, retiree gains some doctor access, if the hospital is around metro area.
								hagency.funds -= 2f;

						}

						
			
			}

			else
			{
				//Removing a doctor from a hospital.
					hagency.isDoctorAssigned_toHospitals_Array[doctorIndex]=false;
					hagency.numOfAssignedDoctorUnits--;

					//Fetching the current hospital from the doctor index supplied.
					int hospitalIndex=doctorIndex/2;
					
					//If there are NO doctors attached to this hospital, and the hospital is open,
						//CLOSE the hospital.
					if( 
						(!hagency.isDoctorAssigned_toHospitals_Array[hospitalIndex*2]
						&&
					   !hagency.isDoctorAssigned_toHospitals_Array[hospitalIndex*2+1])
					   	&&
					   hagency.areHospitalsOpen_Array[hospitalIndex])
						{
							hagency.areHospitalsOpen_Array[hospitalIndex]=false;

							hagency.numOfOpenHospitals--;

							//Update the hospital displays if needed.
							displayManager.update_HShuffle_hospitalIcon(hospitalIndex, false);
	
							//Health agency earns money.
								//Also, retiree loses some doctor access, if the hospital is around metro area.
								hagency.funds += 2f;

						}

						

			}

			//Checking how the hospital update affects the retiree.
			//If hospitals around the retiree (in metro area) are:
									//3 open --> doctor access=3;
									//2 open --> doctor access=2;
									//1-0 open--> doctor access=1;
			int openHospitals_Around_Citizen=0;

			if(hagency.areHospitalsOpen_Array[0])
				openHospitals_Around_Citizen++;
			// if(hagency.areHospitalsOpen_Array[1])
			// 	openHospitals_Around_Citizen++;
			if(hagency.areHospitalsOpen_Array[4])
				openHospitals_Around_Citizen++;
				
			if(openHospitals_Around_Citizen==0)
				retiree.accessToDoctor=1;
			else if(openHospitals_Around_Citizen<2)
				retiree.accessToDoctor=2;
			else 
				retiree.accessToDoctor=3;


			//Update the status messages for this event.
			displayManager.update_ShuffleEvent_StatusMessage("updateDisplay");
			
			//Update the display elements outside of this particular event.
			displayManager.update_UI_after_HresourceShuffle();

		}

		void H_resourceShuffle_assignAvailableDoctors()
		{
			print("&&&& Assigning available Doctors at the start.");

			//Check available hospitals, according to the number of doctors.
			int currentNumberOfDoctorUnits=(int)hagency.numOfDoctors;

			//Generate temporary variables to better understand
				//how to populate the array.
			int fullHospitals_notAssigned= currentNumberOfDoctorUnits/2;
			int halfFull_Hospitals_notAssigned=currentNumberOfDoctorUnits % 2;

			while(hagency.numOfAssignedDoctorUnits < currentNumberOfDoctorUnits)
			{

				int randomHospitalIndex=Random.Range(0, hagency.maxNumOfHospitals);

				//populating the boolean array with doctors.

				//leaving a hospital half-full.
				if(halfFull_Hospitals_notAssigned>0)
				{
					hagency.isDoctorAssigned_toHospitals_Array[randomHospitalIndex*2]=true;
					hagency.numOfAssignedDoctorUnits++;
					halfFull_Hospitals_notAssigned--;

					hagency.numOfOpenHospitals++;


					//Also, updating the hospital array.
					hagency.areHospitalsOpen_Array[randomHospitalIndex]=true;

				}

				//fully staffing a hospital with two doctors.
				else if(fullHospitals_notAssigned>0 && 
						hagency.areHospitalsOpen_Array[randomHospitalIndex]==false
						)
				{
					hagency.isDoctorAssigned_toHospitals_Array[randomHospitalIndex*2]=true;
					hagency.isDoctorAssigned_toHospitals_Array[randomHospitalIndex*2+1]=true;
					hagency.numOfAssignedDoctorUnits+=2;
					fullHospitals_notAssigned--;

					hagency.numOfOpenHospitals++;

					//Also, updating the hospital array.
					hagency.areHospitalsOpen_Array[randomHospitalIndex]=true;
				}

			}	//End of while loop that randomly populates the hospital arrays.

			//Finally, update the number of assigned doctor units variable.
			hagency.numOfAssignedDoctorUnits= currentNumberOfDoctorUnits;

			print("&&&&&&&&&& Open Hospitals: "+hagency.numOfOpenHospitals+
				  ", assignedDoctorUnits: "+hagency.numOfAssignedDoctorUnits);

			//Also update the displayManager.
			displayManager.update_all_HShuffle_Display();

		}

		//Function called when the number of doctors changes.
		void H_resourceShuffle_updateDoctors_and_Hospitals()
		{

			//Check available hospitals, according to the number of doctors.
			int currentNumberOfDoctorUnits=(int)hagency.numOfDoctors;

			//Check if the number is different from hagency.numOfAssignedDoctorUnits. 
			//If different, we have to update. 
			if(currentNumberOfDoctorUnits == hagency.numOfAssignedDoctorUnits )
			{  
				//Nothing to update, move on.

			}
			
			else if(currentNumberOfDoctorUnits > hagency.numOfAssignedDoctorUnits )
			{
				hagency.wereDoctorUnitsModified="incr";
				//If the number of doctor units have increased, DON'T add a doctor sprite yet.
					//Let the user add the doctors.
			}


			else
			{
				//There are less doctors available, so the open hospitals must be updated. 
				hagency.wereDoctorUnitsModified="decr";	

				//Getting the total number of doctors lost.
				int lostDoctors= hagency.numOfAssignedDoctorUnits-currentNumberOfDoctorUnits;

					if(currentNumberOfDoctorUnits >0)
					{
						//remove doctors from the array randomly.
						while(lostDoctors>0)
						{
							int randomIndex=Random.Range(0,hagency.isDoctorAssigned_toHospitals_Array.Length);

							//Removing doctors from the array.
							if(hagency.isDoctorAssigned_toHospitals_Array[randomIndex]==true)
							{
								hagency.isDoctorAssigned_toHospitals_Array[randomIndex]=false;	
								lostDoctors--;
							
								//Finally, check if the hospital needs to be closed, now that a doctor has been removed.
								int hospitalIndex=randomIndex/2;

								if(!hagency.isDoctorAssigned_toHospitals_Array[hospitalIndex*2]
									&&
								   !hagency.isDoctorAssigned_toHospitals_Array[hospitalIndex*2+1])
									{
										hagency.areHospitalsOpen_Array[hospitalIndex]=false;
										hagency.numOfOpenHospitals--;	
									}
							
							}
	
							//Finally, update the variable assigned doctor units variable (smaller value now).
							hagency.numOfAssignedDoctorUnits=currentNumberOfDoctorUnits;

							
						}

					} //end of the doctor removal and hospital update operation.

					//Checking if all doctors have been lost. In which case, close all the hospitals.
					else
					{
						hagency.numOfAssignedDoctorUnits=0;

						for(int i=0; i<hagency.isDoctorAssigned_toHospitals_Array.Length; i++)
							hagency.isDoctorAssigned_toHospitals_Array[i]=false;
						for(int i=0; i<hagency.areHospitalsOpen_Array.Length; i++)
							hagency.areHospitalsOpen_Array[i]=false;			
					}

			}

			//After updating the map's logic variables:
					//Update the map's sprites display.			
					//Update the event's top sentence, 
						//depending on whether a doctor unit has been added or not.
			if(hagency.wereDoctorUnitsModified != "No")
			{

				//This display update method updates the variables and also the bottom message.
				//if(hagency.wereDoctorUnitsModified != "incr")		
					displayManager.update_all_HShuffle_Display();

				//This function will add different text to the event, depending on the message type.
				displayManager.update_ShuffleEvent_TopMessage(hagency.wereDoctorUnitsModified);
			}

		}

		//Updating the map and logic in the Water-infrastructure repair event.
		public void update_W_InfrastructureRepair(string buttonName, bool isIconActivated)
		{

			//From the event 'Infrastructure Repair'
				if(isIconActivated)	
				{		//Water Incident is repaired.
					if (buttonName.Contains("metro"))
					{	
						//Incident has been repaired.
						wagency.currentWaterIncident="None";
						wagency.isIncidentReported=false;

						//drinking water Access improves.
							retiree.drinkingWaterAccess++;
					}

					//Funds are spent.  These funds could be considered overtime,
						//hence why extra money must be spent.
						wagency.funds -= 1f;

					//Use a maintenance worker.
						wagency.numPersonnel--;

					//update counter checking number of Repairs.
					wagency.numberOfRepairs_Selected++;
				
					//also, update boolean array keeping track of repaired incidents.
						int indexToToggle= int.Parse(buttonName.Substring(0,1));
						wagency.map_areThereIncidents_Array[indexToToggle]=false;

				}

				else 
				{		
					//Water Incident is repaired.
					if (buttonName.Contains("metro"))
					{	
						//Incident returns.
						wagency.currentWaterIncident="Broken_Pipe";
						wagency.isIncidentReported=true;

						//drinking water Access decreases.
							retiree.drinkingWaterAccess--;
					}

					//Funds are regained.  These funds could be considered overtime,
						//hence why extra money must be spent.
						wagency.funds += 1f;

					//Free up a maintenance worker.
						wagency.numPersonnel++;

					//update counter checking number of Repairs.
					wagency.numberOfRepairs_Selected--;
				
					//also, update boolean array keeping track of repaired incidents.
						int indexToToggle= int.Parse(buttonName.Substring(0,1));
						wagency.map_areThereIncidents_Array[indexToToggle]=true;

				
				}

				//Finally, call the display function to update visuals.
				displayManager.update_Display_from_WaterInfrastructureRepair(wagency.numberOfRepairs_Selected);

		}

		//Function called to prepare water repair incidents at the start.
		void prepareRandom_W_RepairIncidents()
		{
			//If there are existing problems that haven't been repaired, show them again.
			
			//sets up what locations will display a Water Repair Event.
			if(wagency.isIncidentReported && !wagency.map_areThereIncidents_Array[0])
				wagency.map_areThereIncidents_Array[0]=true;
			
			int eventsToShow=0;

			if(currentTurnCounter< maxTurns/2)
				eventsToShow=Random.Range(2, 4);

			else 
				eventsToShow=Random.Range(3, wagency.map_areThereIncidents_Array.Length);


			//Check if there are existing issues that haven't been repaired.
			for(int i=0; i<wagency.map_areThereIncidents_Array.Length; i++)
			{
				if(wagency.map_areThereIncidents_Array[i])
					eventsToShow--;
			}

			if(eventsToShow<0)
				eventsToShow=0;

			print("&&&&&& Water incidents to show: "+eventsToShow);

			while(eventsToShow>0)
				{
					int index=Random.Range(1, wagency.map_areThereIncidents_Array.Length);

					if(!wagency.map_areThereIncidents_Array[index])
						{
							wagency.map_areThereIncidents_Array[index]=true;
							eventsToShow--;
						}
				}
			
		}


//>>>>---End of part C, the variable update methods.-------


//D.--Event generation methods.

bool determineNextActor(string actionReceived)
{
  
	bool isEventGenerated=false;
  
	//This function updates the currentActor and previousActor values.
	//If the event leads to a 'continue' statement, the new event is generated here.

	//Otherwise, we generate the event later.

   //1.Check for events that require a specific continuation.
		//The event before the continuation is handled here.
		//Dental Emergencies take you to the hospital.
		
		if(actionReceived=="C_accessIncidentIntro")  
			{
					//updating our event and actor variables.
					
					//Fetching the incident that will be shown.
					if(wagency.currentWaterIncident == "Broken Pipe")
						{	
							newEventName="C_reportIncidentBP";		
						}

					//There's a 'Dirty Water' incident already.
					else if(wagency.currentWaterIncident == "Dirty Water")
						{	
							newEventName="C_reportIncidentDW";		
						}

					else if(wagency.currentWaterIncident == "Water Cutoff")
						{	
							newEventName="C_reportIncidentWC";		
						}
					
					lastCitizenIncident_toAppear='W';					

					previousActor=currentActor;
					currentActor=newEventName[0];

					isEventGenerated=true;
										
			}

		
		else if(actionReceived=="C_EmgDentistHospital")  
			{
					//updating our event and actor variables.
					newEventName="C_doctorVEmg";
					previousActor=currentActor;
					currentActor=newEventName[0];

					isEventGenerated=true;
										
			}


		//Logic for determining what to do after a follow-up medical event..
			//Continue statements that will only be repeated once.

		else if(actionReceived == "C_fuLabsHI")
		{

				if(retiree.healthTransactionResult==1)
					newEventName="C_fuLabsPartial";
				else if(retiree.healthTransactionResult==2)
					newEventName="C_fuLabs_UOP";
				else if(retiree.healthTransactionResult==3)
					newEventName="C_fuLabsOP";

				retiree.healthTransactionResult=0;

				previousActor=currentActor;
				currentActor=newEventName[0];

				isEventGenerated=true;
			
		}

		else if(actionReceived == "C_fuXraysHI")
		{

				if(retiree.healthTransactionResult==1)
					newEventName="C_fuXraysPartial";
				else if(retiree.healthTransactionResult==2)
					newEventName="C_fuXraysOP";

				retiree.healthTransactionResult=0;

				previousActor=currentActor;
				currentActor=newEventName[0];

				isEventGenerated=true;
			
		}


		else if(actionReceived=="C_hospHI")  
			{
					//updating our event and actor variables.
					if(retiree.isPayingFullHospitalization)
						newEventName="C_hospOP";
					else 
						newEventName="C_hospPartial";

					previousActor=currentActor;
					currentActor=newEventName[0];

					isEventGenerated=true;
										
			}


		//Determining next actor after Citizen's Doctor visit events.
		else if(actionReceived=="C_doctorVSignup")
		{
				if(retiree.accessToDoctor==1 &&
				citizenEventsDict["C_DVSignup_far"]<1)  //worst case.
				{
					newEventName="C_DVSignup_far";
					citizenEventsDict[newEventName]++;

					//updating our event and actor variables.
					previousActor=currentActor;
					currentActor=newEventName[0];
					isEventGenerated=true;
				}

				else if(retiree.accessToDoctor==2 &&
				citizenEventsDict["C_DVSignup_mid"]<1)  //medium case.
				{
					newEventName="C_DVSignup_mid";
					citizenEventsDict[newEventName]++;

					//updating our event and actor variables.
					previousActor=currentActor;
					currentActor=newEventName[0];

					isEventGenerated=true;
				}

				else if(citizenEventsDict["C_DVSignup_close"]<1)
				{
					newEventName="C_DVSignup_close";
					citizenEventsDict[newEventName]++;
		
					//updating our event and actor variables.
					previousActor=currentActor;
					currentActor=newEventName[0];

					isEventGenerated=true;
				}
		}

		//Determining next actor after Citizen's Doctor Visit Emergency events.
		else if (actionReceived== "C_doctorVEmg") 
		{

				if(retiree.accessToDoctor==1 &&
				citizenEventsDict["C_DVEmg_far"]<1)  //worst case.
				{
					newEventName="C_DVEmg_far";
					citizenEventsDict[newEventName]++;

					//updating our event and actor variables.
					previousActor=currentActor;
					currentActor=newEventName[0];

					isEventGenerated=true;
				}

				else if(retiree.accessToDoctor==2 &&
				citizenEventsDict["C_DVEmg_mid"]<1)  //medium case.
				{
					newEventName="C_DVEmg_mid";
					citizenEventsDict[newEventName]++;
					
					//updating our event and actor variables.
					previousActor=currentActor;
					currentActor=newEventName[0];

					isEventGenerated=true;
				}

				else if(citizenEventsDict["C_DVEmg_close"]<1)
				{
					newEventName="C_DVEmg_close";
					citizenEventsDict[newEventName]++;
		
					//updating our event and actor variables.
					previousActor=currentActor;
					currentActor=newEventName[0];

					isEventGenerated=true;
				}

		}

		else if (actionReceived== "C_doctorVPostpone")	
			{
					//updating our event and actor variables.
					newEventName=actionReceived;
					previousActor=currentActor;
					currentActor=newEventName[0];

					isEventGenerated=true;
			}

		//Citizen events related to hospitalization.
		else if(actionReceived=="C_hospNoPay")		
			{
					//updating our event and actor variables.
					newEventName=actionReceived;
					previousActor=currentActor;
					currentActor=newEventName[0];

					isEventGenerated=true;
			}	


		//Determining next actor after displaying message-type events.
		else if(actionReceived=="C_runWater" || actionReceived== "C_bottledWater" || 
		   actionReceived=="C_doctorVisit" || actionReceived== "C_shopping" || 
		   actionReceived== "H_payComplaint2")
			{
					//updating our event and actor variables.
					newEventName=actionReceived;
					previousActor=currentActor;
					currentActor=newEventName[0];

					isEventGenerated=true;
			}


		else if(actionReceived=="W_rationDrought") 
			{
					//updating our event and actor variables.
					newEventName=actionReceived;
					previousActor=currentActor;
					currentActor=newEventName[0];
					lastWaterEvent_name=actionReceived;

					isEventGenerated=true;
					//Setting a var here, since this won't happen in update game variables.
					wagency.isDroughtAnnounced=true;
			}


	//The rest of the code just updates the previousActor and currentActor values.
		else{
	
					if(actionReceived[0]=='W' || actionReceived[0]=='H')
					{
						//choice from  a water event.
						//must generate a Citizen event, next.

						previousActor=currentActor;
						currentActor='C';

					}

					//choice from a citizen event.
					//must generate a Health or Water event, next.

					else 
					{

							if(didWaterActInTurn)
							{
								//next event is a Health event.
								previousActor=currentActor;
								currentActor='H';

							}

							else
							{
								//next event is a Water event.
								previousActor=currentActor;
								currentActor='W';

							}
	
					}

			}

	print("determineNextActor(): Next Actor to act is: "+currentActor+".");

	return isEventGenerated;

}

//A long function that generates new events for all 3 actors.
void generateNewEvent(string actionReceived)
{
	//This method checks the game parameters,
		//and sends out an event based on the parameters and the event that has been sent.
		//Continue events are not generated here. 

	//How events are generated.
		//Only for events that do not generate a forced continuation.
		//1.Check the parameters and generate an event that needs parameter changes.
		//2.Lastly, if no event is triggered this way, generate a random event.

	//Drought scenario.

	//0.
	bool isEventGenerated=false;

//----------

	//2.Check the parameters and generate an event that needs parameter changes.
	//Important section here.
	string generatedEventName="";

		if(actionReceived[0]=='W' || actionReceived[0]=='H')
		{

		//A. Generate a citizen event.
				//Testing: call H or W-related events only after H or W acts.

					int triggeredEvents=0;


			//For the citizen:
				//1.Time-sensitive events.
						//Healthcare plan changes. 1st or 2nd turn?

						
					//Healthcare plan changes.
					//if( !didWaterActInTurn )
						if(hagency.funds<7f && currentTurnCounter % 2 == 0 &&
							citizenEventsDict["C_changeHPlan"] < 1 )  //Possibly a later event?
							{	
								//offer incentives to doctors in residence.
								generatedEventName="C_changeHPlan";
								//appears once.
								citizenEventsDict[generatedEventName]++;		
								triggeredEvents++;
							}

				//2.One-time events.
						//Event introductions:
							//Introduction to medical incidents.
							//Introduction to follow-up incidents.
						//Hospitalization.
						//Drought Risk, drink more.
						//Buy medicines(2 times?)
						//Groceries.
						//Skipping payment choice.(has never been accessed before.)
							//Service cutoff.


					//Citizen's follow-up to health-related incidents.	
					//if( !didWaterActInTurn )
						if(actionReceived == "C_fuMedsNoPay" || actionReceived == "C_fuMedsOP" )
						{
							//Doing this to only show these messages once.
							if(citizenEventsDict[actionReceived]<1)
							{
								citizenEventsDict[actionReceived]++;
								generatedEventName=actionReceived;		
								triggeredEvents++;		
							}
						}


						//The hospitalization incident should only happen once.
						//Or, if a serious event occurs --Zika or blood pressure.
						//Hmm..
						//if( !didWaterActInTurn )
							if(triggeredEvents==0) 	
								if(retiree.health < 3f  &&
								citizenEventsDict["C_hospitalization"] < 1 )  
									{	
										//offer incentives to doctors in residence.
										generatedEventName="C_hospitalization";
										//appears once.
										citizenEventsDict[generatedEventName]++;
										lastCitizenIncident_toAppear = 'Z';
			
										triggeredEvents++;
									}

						//Drought risks.
						if( didWaterActInTurn )
							if(
								wagency.isDroughtAnnounced
								&& citizenEventsDict["C_droughtRisk"]<1
							)  //Possibly a later event?
								{	
									//a Warning to drink more fluids.
									generatedEventName="C_droughtRisk";
									//appears once.
									citizenEventsDict[generatedEventName]++;
									lastCitizenIncident_toAppear = 'T';

									triggeredEvents++;
								}

						//Pharmacy event, called when running low on medicines.
						//if( !didWaterActInTurn )
							if(triggeredEvents==0)	
								if(retiree.medicationsAmount <= 1 &&
									citizenEventsDict["C_pharmacy"] < 2 &&
										lastCitizenIncident_toAppear != 'P'
									)  
										{	
											//offer incentives to doctors in residence.
											generatedEventName="C_pharmacy";
											//appears once.
											lastCitizenIncident_toAppear = 'P';
											citizenEventsDict["C_pharmacy"]++;
											triggeredEvents++;
										}	

						//Running low on groceries, shopping event.
							if(triggeredEvents==0)
								if(retiree.groceriesAmount<1
										&&
							   	   citizenEventsDict["C_shopping"] < 2 &&
							   	   lastCitizenIncident_toAppear != 'G'								
								)
									{	
										//offer incentives to doctors in residence.
										generatedEventName="C_shopping";
										citizenEventsDict["C_shopping"]++;
										lastCitizenIncident_toAppear = 'G';
										//appears once.
										triggeredEvents++;
									}

							if(triggeredEvents==0)
							{
								//Paying bill prompt.
								if(retiree.turns_skippedWaterPayments > 1 
									&& !retiree.isWaterCutOff)
									{
										generatedEventName="C_wBillOverdue";
										//appears once.
										triggeredEvents++;
									}

								if(retiree.turns_skippedWaterPayments > 1 
									&& !retiree.isHealthCutOff)
									{
										generatedEventName="C_hBillOverdue";
										//appears once.
										triggeredEvents++;		
									}

								//Health or Water cutoff.
								if(retiree.turns_skippedWaterPayments > 2 
									&& !retiree.isWaterCutOff)
									{
										generatedEventName="C_wPlanCutoff";
										//appears once.
										triggeredEvents++;
									}

								if(retiree.turns_skippedWaterPayments > 2 
									&& !retiree.isHealthCutOff)
									{
										generatedEventName="C_hPlanCutoff";
										//appears once.
										triggeredEvents++;		
									}

							


								//Responses to bill changes.
									// if(retiree.funds< 300f && citizenEventsDict["C_billAdjustment"]<1)
									// 	{	
									// 		//You can put off paying your bills.
									// 		generatedEventName="C_billAdjustment";
									// 		//appears once.
									// 		citizenEventsDict[generatedEventName]++;
									// 		triggeredEvents++;
									// 	}
		
							}
						

						
				//3.Citizen's repeatable events.  We can always count on these events to be generated.		

					//Water-related incidents.
						//Could happen at any moment.
						//Reporting water incidents.
					//if( didWaterActInTurn )
						if(triggeredEvents==0)	
								{
									if(wagency.currentWaterIncident != "None" && !wagency.isIncidentReported)
									{
										//Show an introductory message, if this is the first time viewing.
										if(!retiree.isDrinkingWaterAccessIssueIntroduced)
										{
											retiree.isDrinkingWaterAccessIssueIntroduced=true;
											generatedEventName="C_accessIncidentIntro";	
											triggeredEvents++;
										}
									}

										else if(wagency.currentWaterIncident != "None" && 
												lastCitizenIncident_toAppear !='W')
										{	

											//These 3 incidents can appear more than once.
											if(wagency.currentWaterIncident == "Broken Pipe")
												{	
													generatedEventName="C_reportIncidentBP";		
												}

											//Note: there's a 'Dirty Water' incident already.
											if(wagency.currentWaterIncident == "Dirty Water")
												{	
													generatedEventName="C_reportIncidentDW";		
												}

											if(wagency.currentWaterIncident == "Water Cutoff")
												{	
													generatedEventName="C_reportIncidentWC";		
												}
											
											triggeredEvents++;
											lastCitizenIncident_toAppear='W';

										}

										//Dirty Water incident. 
										/*if(wagency.currentWaterIncident == "Dirty Water")
												{	//Shuffle facilities and doctors.
													generatedEventName="C_DirtyWater";		
													//can appear more than once.
													triggeredEvents++;
												}*/
									
									
								}


					//Citizen's health-related incidents.
					//if( !didWaterActInTurn )
						if(triggeredEvents==0)
							{
									
									//Reporting health incidents.
									if(
											retiree.currentHealthIncident == "Flu" 	&&
											lastCitizenIncident_toAppear != 'H'
									)
									{	

											generatedEventName="C_doctorVisitFlu";		
											//can appear more than once. Well,
											//once for Flu and once for Fall.
											lastCitizenIncident_toAppear='H';
											retiree.didHealthIncidentAppear=true;				
											triggeredEvents++;


											//Take some health away.
											retiree.health -= 2f;
											if(retiree.health<1f)
												retiree.health=1f;
									}

									
									else if(retiree.currentHealthIncident == "Fall"  &&
											lastCitizenIncident_toAppear != 'H' )
									{	

											generatedEventName="C_doctorVisitFall";		
											//can appear more than once. Well,
											//once for Flu and once for Fall.
											lastCitizenIncident_toAppear='H';
											retiree.didHealthIncidentAppear=true;				
											triggeredEvents++;


											//Take some health away.
											retiree.health -= 2f;
											if(retiree.health<1f)
												retiree.health=1f;




									}

									else if(retiree.currentHealthIncident == "Root Canal" && 
											lastCitizenIncident_toAppear != 'H' )
									{
												if(retiree.dentistEventState==0)
												{
									
														generatedEventName="C_dentistVisit";		
														//can appear more than once.
														lastCitizenIncident_toAppear='H';
														retiree.didHealthIncidentAppear=true;				
														triggeredEvents++;	

														//Take some health away.
														retiree.health -= 0.5f;
														if(retiree.health<1f)
															retiree.health=1f;	

														retiree.dentistEventState=1;
												
												}		

											else if(retiree.dentistEventState==1)
												{
									
														generatedEventName="C_dentistVisitEmg";		
														//can appear more than once.
														lastCitizenIncident_toAppear='H';
														retiree.didHealthIncidentAppear=true;				
														triggeredEvents++;	

														//Take some health away.
														retiree.health -= 0.5f;
														if(retiree.health<1f)
															retiree.health=1f;	

														retiree.dentistEventState=2;
												
											}

									}//end of dentist visit group.
							}

						//if( !didWaterActInTurn )
							if(triggeredEvents==0)
							{

									//Reporting follow-up health incidents.
									if(
											retiree.currentFollowup == "Labs" 	&&
											lastCitizenIncident_toAppear != 'F'
									)
									{	
											generatedEventName="C_followupLabs";		
											//can appear more than once. Well,
											//once for Flu and once for Fall.
											triggeredEvents++;
											lastCitizenIncident_toAppear='F';
											retiree.didHealthFollowupAppear=true;
									}

									
									if(retiree.currentFollowup == "ExtraMeds"  &&
											lastCitizenIncident_toAppear != 'F' )
									{	
											generatedEventName="C_followupMeds";		
											//can appear more than once. Well,
											//once for Flu and once for Fall.
											triggeredEvents++;
											lastCitizenIncident_toAppear='F';
											retiree.didHealthFollowupAppear=true;
									}

									else if(retiree.currentFollowup == "Xrays" && 
											lastCitizenIncident_toAppear != 'F')
									{	
											generatedEventName="C_followupXrays";		
											//can appear more than once.
											triggeredEvents++;
											lastCitizenIncident_toAppear='F';
											retiree.didHealthFollowupAppear=true;
									}

					
							}

				
			//Should add: zika sickness message, hospitalization and lab messages.
					

					//Setting game variables, if an event was generated.
					print("TRIGGERED CITIZEN EVENTS: "+triggeredEvents+".  >>>>>>");
					if(generatedEventName != "")
					{
						isEventGenerated=true;

						//updating our actor variables.
						newEventName=generatedEventName;
						previousActor=currentActor;
						currentActor=generatedEventName[0];
						print("		NEW CITIZEN EVENT GENERATED: "+newEventName+".");

						//Storing the event name for reference on the next turn.
						lastCitizenEvent_name=generatedEventName;
					}

					else
						print("		-NO- CITIZEN EVENT GENERATED.");

			}

			//-----else, Generate a Water or Health event.------>
			
			//what's important here?
				//making adjustments.
				//These adjustments are based on agency needs.

	else{		

			if(!didWaterActInTurn)
			{
			//B. Generate a Water Event.		

			//For the water agency:
				//1.Time-sensitive events.
					//None for this actor.
					

				//2.One-time events.
					//Drought notices.
					//Rationing adjustments (2).
					//Agency cuts.
					//Water monitoring off.
					//Bill increase (2)

					int triggeredEvents=0;

					//Drought events.	//turns 4-5.

						//Notifications that the drought has started.
						if(
								wagency.waterLevel_dangerState == 1
								&&
								waterEventsDict["W_noticeDroughtW"]<1
						  )
							{	//notice that a drought is starting-warning levels.
								generatedEventName="W_noticeDroughtW";
								//appears only once.
								waterEventsDict[generatedEventName]++;
								triggeredEvents++;

							}

						else if(
								wagency.waterLevel_dangerState == 0 
								&&
								waterEventsDict["W_noticeDroughtC"]<1
							)
							{	
								//notice that a drought is starting-critical levels.
								generatedEventName="W_noticeDroughtC";
								//appears only once.
								waterEventsDict[generatedEventName]++;
								triggeredEvents++;
							}						

						if(triggeredEvents == 0 )
						{
										
									//Rationing events.
								if(		wagency.waterLevel_dangerState < 2
										&&
										(waterEventsDict["W_noticeDroughtW"]==1 || 
										waterEventsDict["W_noticeDroughtC"]==1)
										&&
										waterEventsDict["W_rationDrought"]<2
										&&
										lastWaterEvent_name != "W_rationDrought"
										)
									{

										//show drought scenario.
												print("&&&&& GOING INTO RATIONING: lastWaterEvent is "+lastWaterEvent_name+".");

												//This is a similar event, with different wording.
												generatedEventName="W_rationDrought";
												//Update the drought event's text.
												displayManager.update_Drought_Event();
												//can appear several times.
												waterEventsDict["W_rationDrought"]++;
												triggeredEvents++;

									}	
								
						}

						
						//Try Funds-related events.
						if(triggeredEvents == 0 )
						{
							
								if(
										wagency.funds <= 10f &&
										waterEventsDict["W_agencyCuts"] < 1
									)	
								{	
									//show agency cuts scenario.
									generatedEventName="W_agencyCuts";
									//appears once.
									waterEventsDict["W_agencyCuts"]++;
									triggeredEvents++;

								}
							
							
							else if(wagency.funds < 12f && 
						   	waterEventsDict["W_monitoringOff"] < 1 )	//turns 1-2
							{	//turn water monitoring off.
								generatedEventName="W_monitoringOff";		
								//appears once.
								waterEventsDict["W_monitoringOff"]++;
								triggeredEvents++;
							}

							else if(
									wagency.funds < 10f && 
								
									waterEventsDict["W_billIncrease"] < 1 &&

									(currentActorCounter != 2  &&
									currentActorCounter <5)
							)  	//turns 3-4
									{	
										//show bill adjustment scenario.
										generatedEventName="W_billIncrease";
										waterEventsDict["W_billIncrease"]++;
										//slider; can appear more than once.
										triggeredEvents++;
									}

						}	
						
						
						//3.Repeatable events.  We can always count on these.
						//Infrastructure repair.
							//Related, water brigade.
							//If in flooding scenario: heavy rain event. 						
						
						//Maintenance and water brigade events.
						if(triggeredEvents == 0 )
						{

						//Maintenance events, placing them first. 
							//Reactions to Citizen action. Run after turn 2.
							if(wagency.isIncidentReported && lastWaterEvent_name != "W_infrRepair")
								{	//go to maintenance event.
									generatedEventName="W_infrRepair";
									
									print("&&&& PREPARING TO DISPLAY - WATER REPAIR ICONS.");

									//Transplanting this code from prepareRandom_Wrepair incidents: 
											//call the display manager to show these incidents.
										//resetting the display values here first.
										displayManager.reset_WRepairSprites();
											displayManager.show_WRepairSprites();

									//appears multiple times.
									triggeredEvents++;
								}

						//Water brigades.
							//Reactions to reports.
							else if(wagency.waterLevel_dangerState < 2 && 
									lastWaterEvent_name != "W_waterBrigade")
								{	
									//show water Brigade scenario.
									generatedEventName="W_waterBrigade";
									//appears multiple times.
									triggeredEvents++;
								}
						}

					


						//Fallback event: infr Repair
						if(triggeredEvents == 0 )
						{

						//Maintenance events, placing them first. 
							//Reactions to Citizen action. Run after turn 2.
							//if(lastWaterEvent_name != "W_infrRepair")
								//{	//go to maintenance event.
									generatedEventName="W_infrRepair";
									
									print("&&&& FALLBACK:  - WATER REPAIR INCIDENT.");
									prepareRandom_W_RepairIncidents();

									//Transplanting this code from prepareRandom_Wrepair incidents: 
											//call the display manager to show these incidents.
										//resetting the display values here first.
										displayManager.reset_WRepairSprites();
											displayManager.show_WRepairSprites();

									//appears multiple times.
									triggeredEvents++;
								//}

						}

					//Setting game variables, if an event was generated.
					print("TRIGGERED WATER EVENTS IS: "+triggeredEvents+".  >>>>>>");
					if(generatedEventName != "")
					{
						isEventGenerated=true;

						//updating our actor variables.
						newEventName=generatedEventName;
						previousActor=currentActor;
						currentActor=generatedEventName[0];
						print("		NEW WATER EVENT GENERATED: "+newEventName+".");

						//Storing the event name for reference on the next turn.
						lastWaterEvent_name=generatedEventName;

					}

					else
						print("		-NO- WATER EVENT GENERATED.");

			}



		//C. Generate a Health event.----------->
			else
			{
				int triggeredEvents=0;


			//For the health agency:
				//1.Time-sensitive events.
					//Federal Funding result (halfway through the game)

					//Funding event. It happens halfway through the game, only once.
						//Explanatory text on the inevitability of this.
						if(currentTurnCounter == (int)maxTurns/2+1)
							{	//Funding event. The odds of the how much
								//funding is received is random.

									int chance= Random.Range(1,11);
									//Chances of low $= 50%.
									//Chances of med $= 40%.								
									//Chances of high $= 10%.

									if(chance <5)
										generatedEventName="H_fundingResultL";

									else if(chance <9)
										generatedEventName="H_fundingResultM";

									else
										generatedEventName="H_fundingResultH";
									
									//appears only once. 
									triggeredEvents++;
							}	


				//3.Repeatable events.  We can always count on these.		
					//Hospital+resources shuffle. (only if doctors amount changes).

						//Doctor # and funds-related events.
						if(triggeredEvents==0)
						{	
							if(hagency.wereDoctorUnitsModified !="No"
								&&
								healthEventsDict["H_resourceShuffle"] < 3
								&&
								lastHealthEvent_name != "H_resourceShuffle"
							)
								{	//Shuffle facilities and doctors.
									generatedEventName="H_resourceShuffle";		

									//can appear more than once.
									healthEventsDict[generatedEventName]++;	
									lastHealthEvent_name = "H_resourceShuffle";

									triggeredEvents++;
								}
						}



				//2.One-time events.
							//Other map events:
								//Blood drive.
							//Zika emergency preventive.
							//Doctor incentives.
							//Residents pay (slider)


					if(triggeredEvents == 0)
						{
							//Epidemic events.
							if(
								hagency.zikaEpidemicRisk==3 &&
							   	healthEventsDict["H_zikaEmergency"] < 1
								)
								{	//show Zika epidemic message.
									generatedEventName="H_zikaEmergency";
									//appears only once.
									healthEventsDict["H_zikaEmergency"]++;
									triggeredEvents++;
								}

							else if(
									hagency.influenzaEpidemicRisk==3 &&
							   		healthEventsDict["H_influenzaEpidemic"] < 1
									)
								{	//show Influenza epidemic message.
									generatedEventName="H_influenzaEpidemic";
									//appears once.
									healthEventsDict[generatedEventName]++;
									triggeredEvents++;
								}
						}

					if(triggeredEvents==0)
					{

							if(
								hagency.numOfDoctors < 6f 
								&&
								healthEventsDict["H_doctorIncentive"] < 1
								)
							{	
								//offer incentives to doctors in residence.
								generatedEventName="H_doctorIncentive";
								//appears once.
								healthEventsDict[generatedEventName]++;
								triggeredEvents++;
							}
					
							//Taking the shifts event out,
								//this is already addressed in the doctor resource shuffle.

						//further adjustments based on # of doctors and facilities.
						//Check these two.
							 else if(hagency.funds < 4f &&
								healthEventsDict["H_payResidents"] < 1
								)  
								{	
									//manage number of residencies.
									generatedEventName="H_payResidents";
									//appears once.
									healthEventsDict[generatedEventName]++;			
									triggeredEvents++;
								}								

						//further adjustments based on # of doctors and facilities.
							else if(hagency.funds < 4f &&
								healthEventsDict["H_doctorPayComplaint"] < 1
								)  
								{	
									//manage number of residencies.
									generatedEventName="H_doctorPayComplaint";
									//appears once.
									healthEventsDict[generatedEventName]++;								
									triggeredEvents++;
								}

					}

					//Epidemic events.
					if(triggeredEvents==0)
					{
					
					//Epidemic preparation measures.
						//Could be a map event.
						if(hagency.funds <= 12f 
							&&
							healthEventsDict["H_bloodDrive"] < 2
							&&
							lastHealthEvent_name != "H_bloodDrive")
							{	//Blood collecting.
								generatedEventName="H_bloodDrive";
								//appears once or twice, and not consecutively.
								healthEventsDict[generatedEventName]++;								
								triggeredEvents++;

							}

						else if(hagency.zikaEpidemicRisk >0
							&&
							healthEventsDict["H_preventiveZika"] < 2
							&&
							lastHealthEvent_name != "H_preventiveZika")
							{	//Vaccine dries.
								generatedEventName="H_preventiveZika";
								//appears once or twice, and not consecutively.
								healthEventsDict[generatedEventName]++;								
								triggeredEvents++;
							}

					}


					//4. Fall-back event.  	
					//Hospital+resources shuffle. (Forcing a doctors amount change).

						//Doctor # and funds-related events.
						if(triggeredEvents==0)
						{	
							if( healthEventsDict["H_resourceShuffle"] < 3
								//&&
								//lastHealthEvent_name != "H_resourceShuffle"
								)
								{	
									hagency.numOfDoctors -=1f;
									H_resourceShuffle_updateDoctors_and_Hospitals();
									displayManager.update_UI_after_HresourceShuffle();

									//Shuffle facilities and doctors.
									generatedEventName="H_resourceShuffle";		

									//can appear more than once.
									healthEventsDict[generatedEventName]++;	
									lastHealthEvent_name = "H_resourceShuffle";

									triggeredEvents++;
								}
						}



					//Setting game variables, if an event was generated.
					print("TRIGGERED HEALTH EVENTS IS: "+triggeredEvents+".  >>>>>>");
					if(generatedEventName != "")
					{
						isEventGenerated=true;

						//updating our actor variables.
						newEventName=generatedEventName;
						previousActor=currentActor;
						currentActor=generatedEventName[0];
						print("		NEW HEALTH EVENT GENERATED: "+newEventName+".");
						//Storing the event name for reference on the next turn.
						lastHealthEvent_name=generatedEventName;

					}

					else
						print("		-NO- HEALTH EVENT GENERATED.");

			} //End of health Event generation.


			//Important: flipping the toggle that determines what agency to go to.
				didWaterActInTurn=!didWaterActInTurn;  
				print("Event #"+currentActorCounter+" Generated: didWaterActInTurn toogle is now "+didWaterActInTurn);

	}  //End of health , water or citizen event generation.


}


} //End of file.

