using UnityEngine;
using UnityEditor;

//Version 0.2 | s.b.Newsom Edition

//Source from http://answers.unity3d.com/questions/179775/game-window-size-from-editor-window-in-editor-mode.html
//Modified by seieibob for use at the Virtual Environment and Multimodal Interaction Lab at the University of Maine.
//Use however you'd like!

//Modified by sbNewsom. Like it is said above, use as you like! If you're interested in my work, check out:
//http://www.sbnewsom.com

/// <summary>
/// Displays a popup window that undocks, repositions and resizes the game window according to
/// what is specified by the user in the popup. Offsets are applied to ensure screen borders are not shown.
/// </summary>
public class GameWindowMover: EditorWindow {
	
 	//The size of the toolbar above the game view, excluding the OS border.
	private int tabHeight = 22;
	
	private bool toggle = true;
	
	//Get the size of the window borders. Changes depending on the OS.
	#if UNITY_STANDALONE_WIN
	//Windows settings
	private int osBorderWidth = 5;
	#elif UNITY_STANDALONE_OSX
	//Mac settings (untested)
	private int osBorderWidth = 0; //OSX windows are borderless.
	#else
	//Linux / other platform; sizes change depending on the variant you're running
	private int osBorderWidth = 5;
	#endif
	
	//Desired window resolution
	private Vector2 gameSize = new Vector2(1280, 800);
	//Desired window position
	private Vector2 gamePosition = new Vector2(0, 0);
	
	//Tells the script to use the default resolution specified in the player settings.
	private bool usePlayerSettingsResolution = false;
	
	//For those that duplicate screen
	private bool useDesktopResolution = true;
 
	//Shows the popup
    [MenuItem ("Window/Rift VR Game Mode")]
    static void OpenPopup() {
		GameWindowMover window = (GameWindowMover)(EditorWindow.GetWindow(typeof(GameWindowMover)));
		//Set popup window properties
		Vector2 popupSize = new Vector2(300, 140);
		//When minSize and maxSize are the same, no OS border is applied to the window.
		window.minSize = popupSize;
		window.maxSize = popupSize;
		window.title = "RiftMode";
		window.ShowPopup();
    }
 
	//Returns the current game view as an EditorWindow object.
    public static EditorWindow GetMainGameView(){
		//Creates a game window. Only works if there isn't one already.
		EditorApplication.ExecuteMenuItem("Window/Game");
		
		System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
		System.Reflection.MethodInfo GetMainGameView = T.GetMethod("GetMainGameView",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
		System.Object Res = GetMainGameView.Invoke(null,null);
		return (EditorWindow)Res;
    }
 
    void OnGUI(){
		
		EditorGUILayout.Space();
		
if(useDesktopResolution){
			gameSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
		}

	GUILayout.Label("Rift Game Mode is now activated. ");
		
		GUILayout.Label("Don't close this panel to keep script in effect.");
	}
	void Update() {
		if(Application.isPlaying){
			MoveGameWindow();
			toggle = true;
		} else {
			if(toggle){
				CloseGameWindow();
				toggle = false;
			}
		}
		

		
    }
	
	void MoveGameWindow(){
		EditorWindow gameView = GetMainGameView();
		gameView.title = "Game (Oculus Rift)";
		//When minSize and maxSize are the same, no OS border is applied to the window.
		gameView.minSize = new Vector2(gameSize.x, gameSize.y + tabHeight - osBorderWidth);
		gameView.maxSize = gameView.minSize;
		Rect newPos = new Rect(gamePosition.x, gamePosition.y - tabHeight, gameSize.x, gameSize.y + tabHeight - osBorderWidth);
		gameView.position = newPos;	
		gameView.ShowPopup();	
	}
	
	void CloseGameWindow(){
		EditorWindow gameView = GetMainGameView();
		gameView.Close();	
	}
 
}