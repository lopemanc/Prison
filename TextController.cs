using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TextController : MonoBehaviour {

	public Text text;
	private PlayerState myState;
	private List<PlayerState> map = new List<PlayerState>();

	// Use this for initialization
	void Start ()
	{
		// Below we define the state machine using a 3D array.  
		//	Each top level element defines a PlayerState
		//		First element for each entry is the PlayerState data (StateName fallowed by State Description.  Third poition is ignored.  
		//		Second through Fourth element each define a Menu entry.).
		//			First element is Menu Entry Key, followed by Menu Entry Label, New State for menu Pick
		
		string[,,] mapData = new string[,,]
		{
			{
				{ "InCell", "You are in a Prison cell wearing an orange jumpsuit.  You don't remeber how you got here, but staying doesn't seems like a good idea. Theres not much here...some bedding, a mirror and a barred door.", ""},
				{"D", "Look at the door", "LookingAtLock" },
				{"B", "Look at the bedding.", "LookingAtSheets" },
				{"M", "Look at the Mirror", "LookingAtMirror" }
			},
			{
				{ "LookingAtMirror", "The Mirror is broken.  But man, you don't look so good.  I doubt you ever looked really good but your pretty banged up.", "" },
				{"T", "Take a sliver of the mirror.", "InCellHoldingMirror" },
				{"R", "Return to pacing around he cell.", "InCell"},
				{"", "", ""}
			},
			{
				{ "LookingAtSheets", "These things are disgusting.  You have really got to do a better job of keeping the place up if you are going to be staying a while.", ""},
				{"R", "Return to pacing around he cell.", "InCell"},
				{"", "", ""},
				{"", "", ""}
			},
			{
				{"LookingAtLock", "The lock looks solid but simple with a large old fashioned keyhole.", ""},
				{"R", "Return to pacing around he cell.", "InCell"},
				{"", "", ""},
				{"", "", ""}
			},
			{
				{"InCellHoldingMirror", "You now have a sliver of mirror, but you are still trapped in the cell.", ""},
				{ "S" , "Slit your wrist with the piece of mirror.", "Suicide" },
				{ "L" , "Use the mirror to pick the lock", "InHall" },
				{ "W" , "Wait for a guard and stab him with the piece of mirror", "DeadFromStabbingGuard" }
			},
			{
				{"Suicide", "The cowards way out.  Well that's one way to achieve freedom...but you loose the game.", ""},
				{ "P" , "Play Again.", "InCell"},
				{"", "", ""},
				{"", "", ""}
			},
			{
				{"DeadFromStabbingGuard", "As the guard enters your cell you stab in the neck.  You struggle with him as he fights desparatly for his life.  But there is no way the guard is coming out on top of this fight.  You hold him down until the life leave his eyes.  You relax your grip turn and look up just in time to see a socond guards batton crush your skull.  That's one way to achieve freedom...but you loose the game.", ""},
				{ "P" , "Play Again.", "InCell"},
				{"", "", ""},
				{"", "", ""}
			},
			{
				{"InHall", "The mirror cracked off in the lock but at least it opened. You are now in the hallway not far from the cell.  There is a stairwell that leads up and a closet open janatorial closet.  The closet contains janitor overalls, and a broom.", ""},
				{"B" , "Take the broom.", "InHallWithBroom"},
				{"O" , "Put on the overalls", "InHallWearingOveralls"},
				{"S" , "Go upstairs.", "FollowStairsUnpreparied"}
			},
			{
				{"InHallWithBroom", "You are now in the hallway not far from the cell.  There is a stairwell that leads up and a closet open janatorial closet.  The closet contains janitor overalls.", ""},
				{"S" , "Sweep the hallway.", "InHallWithBroom"},
				{"O" , "Put on the overalls", "InHallFullyLoaded"},
				{"G" , "Go upstairs.", "FollowStairsUnprepared"}
			},
			{
				{"InHallFullyLoaded", "You are now in the hallway not far from the cell.  There is a stairwell that leads up and a closet open janatorial closet.  The closet is empty.", ""},
				{"S" , "Sweep the hallway.", "InHallWithBroom"},
				{"O" , "Take off the overalls", "InHallFullyLoaded"},
				{"G" , "Go upstairs.", "FollowStairsPrepared"}
			},
			{
				{"InHallWearingOveralls", "You are now in the hallway not far from the cell.  There is a stairwell that leads up and a closet open janatorial closet.  The closet contains a broom.", ""},
				{"B" , "Take the broom.", "InHallFullyLoaded" },
				{"O" , "Take off overalls", "InHall" },
				{"S" , "Go upstairs.", "FollowStairsPartlyPrepared" }
			},
			{
				{"FollowStairsPrepared", "As you are about half way of the up the stairs a door opens above and footsteps are approaching fast.  Theres is no time to go anywhere.", ""},
				{ "A" , "Attack the guard with the broom.", "DeadOnStairs" },
				{ "W" , "Put your head down and keep walking.", "UpstairsHall"},
				{"", "", ""}
			},
						{
				{"FollowStairsUnprepared", "As you are about half way of the up the stairs a door opens above and footsteps are approaching fast.  Theres is no time to go anywhere.", ""},
				{ "A" , "Attack the guard.", "DeadOnStairs"},
				{ "W" , "Put your head down and keep walking.", "DeadOnStairs"},
				{"", "", ""}
			},
			{
				{"DeadOnStairs", "The guard is coming down the stairs fast.  You make eye contact and he immediately sees your intent.  There's no place to go, he tackles you and you both tumble down the stairs.  The guard lives but you end up with a broken neck.  You're dead.", ""},
				{"P" , "You're dead.  Game Over. Play Again.", "InCell"},
				{"", "", ""},
				{"", "", ""}
			},
			{
				{"DeadHeadOn", "The guards come threw the door and look at you suspiciously.  Something must have tipped them off because they immediatly draw their pistols and shoot you dead.",""},
				{"P" , "Game Over. Play Again.", "InCell"},
				{"", "", ""},
				{"", "", ""}
			},
			{
				{"UpstairsHall", "You have made it to ground level.  Freedom appears to be close.  Down the hallway you can see a door that leads outside and another next to it marked Administration.  Two guards are coming in from outside.",""},
				{"W" , "Walk towards the Exit.", "DeadHeadOn" },
				{"S" , "Stay where you are.", "DeadHeadOn" },
				{"B" , "Use the broom to sweep the hall.", "SweepUpstairs" }
			},
			{
				{"SweepUpstairs", "The guards enter and glance your way.  Then they turn and go through the door marked Administration.  You are still a fair distance from the exit.  You realize the exit door is closing quickly and will likely lock and require and code to be entered on the keypad you see next to the door.",""},
				{"R" , "Run to the Exit.", "DeadFromRunning"},
				{"B" , "Slide the broom down the hall to stop the door from closing.", "Freedom"},
				{"", "", ""}
			},
			{
				{"SweepUpstairs", "The guards enter and glance your way.  Then they turn and go through the door marked Administration.  You are still a fair distance from the exit.  You realize the exit door is closing quickly and will likely lock and require and code to be entered on the keypad you see next to the door.",""},
				{"R" , "Run to the Exit.", "DeadFromRunning"},
				{"B" , "Slide the broom down the hall to stop the door from closing.", "Freedom"},
				{"", "", ""}
			},
			{
				{"DeadFromRunning", "You are running for the door, your footsteps echo loudly through the halls.  Loudly enough to make the guards turn back.  Maybe they could have stopped you without lethal force.  But you didn't want to go back to that cell ayway.",""},
				{"P" , "Game Over. Play Again.", "InCell"},
				{"", "", ""},
				{"", "", ""}
			},
			{
				{"Freedom", "The broom slides easily across the floor and stops the door from closing and latching.  It made very little noise sliding and doesn't seem to have attracted any attention.  You calmly walk to the door, open it and walk out.",""},
				{"P" , "Game Over. Play Again.", "InCell"},
				{"", "", ""},
				{"", "", ""}
			}
		};

		// Build the statemachine map from the data provided
		for (int i = 0; i < mapData.GetLength(0); i++)
		{
			List<PlayerStateOption> options = new List<PlayerStateOption>();
			for (int j = 1; j < mapData.GetLength(1); j++)
			{
				if(mapData[i, j, 0] != "")		// Only add non blank menu choices
				{
					options.Add(new PlayerStateOption(mapData[i, j, 0], mapData[i, j, 1], mapData[i, j, 2]));
				};
			};
			// Build state with story and menu choices
			map.Add(new PlayerState(mapData[i, 0, 0], mapData[i, 0, 1], options));	
		};
		myState = map[0];	// Start in Cell
	}
	
	// Update is called once per frame
	void Update ()
	{
		DisplayState();									// Describe current situation to play and give choices
		string choice = Input.inputString;				// Get user key
		if (choice.Length != 0)
		{	// translate key to menu choice
			choice = choice.Substring(0, 1).ToUpper();
			var path = myState.OptionForKey(choice);
			if (path != null)
			{	// process menu choice to go to next state
				myState = StateForName(path.newStateName);
			}
		};
	}

	private void DisplayState()
	{	// Describe current situation to play and give choices
		string[] entries = myState.options.Select(opt => opt.ToString()).ToArray();
		text.text = myState.description + "\n\n" + string.Join( "\n", entries);
	}

	private PlayerState StateForName(string aStateName)
	{   // Find the state for aStateName
		return map.Find(st => st.name == aStateName);
	}

}

public class PlayerState
{
	public string name;
	public string description;
	public List<PlayerStateOption> options;

	public PlayerState(string aName, string aText, List<PlayerStateOption> optionsList)
	{	
		name = aName;
		description = aText;
		options = optionsList;
	}

	public PlayerStateOption OptionForKey(string key)
	{	// Find menu option for key press
		return options.Find(option => option.key == key);
	}
}

public class PlayerStateOption
{
	public string key;
	public string menuEntryText;
	public string newStateName;

public PlayerStateOption(string aKey, string aText, string aNewStateName)
	{	
		key = aKey;
		menuEntryText = aText;
		newStateName = aNewStateName;
	}

	public override string ToString()
	{	// Convert menu option for display to user
		return ("<" + key + "> " + menuEntryText);
	}
}
