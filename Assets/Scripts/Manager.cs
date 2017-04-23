using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

	public GameObject mainMenu;
	public GameObject highscoresMenu;
	public GameObject helpMenu;

	// Use this for initialization
	void Start () {
		if(!PlayerPrefs.HasKey("number")){
			PlayerPrefs.SetInt ("number", 0);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void openHighscores(){
		highscoresMenu.SetActive (true);
		mainMenu.SetActive (false);
	}
	public void openMainMenu(){
		mainMenu.SetActive (true);
		highscoresMenu.SetActive (false);
		helpMenu.SetActive (false);
	}

	public void openHelpMenu(){
		mainMenu.SetActive (false);
		helpMenu.SetActive (true);
	}

	public void quit(){
		Application.Quit ();
	}
}
