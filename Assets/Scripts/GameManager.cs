using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public GameObject woodPlank;
	public GameObject woodPlankHologramInstance;
	public GameObject player;
	public GameObject deathScreen;

	public GameObject pauseMenu;
	public GameObject signMenu;
	public GameObject winMenu;

	GameObject hollogram;
	Player playerScript;
	bool paused;

	bool playing;

	int woodLeftOver = 10;
	public int maxWoodCount = 100;

	public int numberOfIslands = 5;

	int time;
	float secondsCounter;

	List<string> islandsFound = new List<string> ();

	void Awake(){
		if(instance == null){
			instance = this;
		}
	}

	// Use this for initialization
	void Start () {
		playerScript = (Player) player.GetComponent(typeof(Player));

		time = 0;
		secondsCounter = 0f;

		playing = true;
	}
	
	// Update is called once per frame
	void Update () {
		displayHologram ();


		//INPUT
		if (Input.GetKeyDown (KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) {
			placePlank ();
		}

		if(Input.GetKeyDown(KeyCode.Space)){
			Player script = (Player)player.GetComponent (typeof(Player));
			script.attack ();
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (paused == true) {
				playGame ();
			} else {
				pauseGame ();
			}
		}

		Text woodTextComp = GameObject.FindGameObjectWithTag ("WoodAmount").GetComponent<Text> ();

		//Display Left Over wood to Text UI.
		woodTextComp.text = woodLeftOver.ToString ();
		if (woodLeftOver >= maxWoodCount || woodLeftOver <= 0) {
			woodTextComp.color = Color.red;
		} else {
			woodTextComp.color = Color.white;
		}

		Text signAmount = GameObject.FindGameObjectWithTag ("SignAmount").GetComponent<Text> ();

		signAmount.text = "ISLANDS FOUND: " + islandsFound.Count.ToString () + "/" + numberOfIslands.ToString ();
		if (islandsFound.Count >= numberOfIslands) {
			displayWinMenu ();
		}

		if (playing && !paused) {
			secondsCounter += Time.deltaTime;

			if (secondsCounter >= 1f) {
				secondsCounter = 0;
				time++;
			}
		}
	}

	void displayHologram(){
		if (woodLeftOver > 0) {
			Vector2 holoPos = playerScript.getHoloPos ();


			if (holoPos != new Vector2(5000f, 5000f)) {
				Destroy (hollogram);

				hollogram = Instantiate (woodPlankHologramInstance, holoPos, Quaternion.identity);
			}
		}
	}

	void placePlank(){
		if (woodLeftOver > 0) {
			Instantiate (woodPlank, playerScript.getHoloPos (), Quaternion.identity);
			woodLeftOver--;
		}
	}
	public void pauseGame(){
		Time.timeScale = 0;
		pauseMenu.SetActive (true);
		paused = true;

	}
	public void playGame(){
		pauseMenu.SetActive (false);
		Time.timeScale = 1;

		paused = false;
	}

	public void addWood(int amount){
		if (woodLeftOver + amount <= maxWoodCount) {
			woodLeftOver += amount;
		}
		else{
			woodLeftOver = maxWoodCount;
		}
	}

	public void foundIsland(string island){
		if(!islandsFound.Contains(island)){
			islandsFound.Add (island);
		}
	}

	public void openSignMenu(string islandName){

		signMenu.SetActive (true);

		GameObject islandNameObject = GameObject.FindGameObjectWithTag ("IslandName");
		Text islandNameText = islandNameObject.GetComponent<Text> ();
		islandNameText.text = islandName;
	}
	public void closeSignMenu(){

		signMenu.SetActive (false);
	}

	public void displayWinMenu(){

		Time.timeScale = 0f;
		playing = false;
		winMenu.SetActive (true);
		PlayerPrefs.SetInt ("score" + PlayerPrefs.GetInt("number"), time);
		PlayerPrefs.SetInt ("number", PlayerPrefs.GetInt ("number") + 1);
	}

	public string getTime(){

		int leftOverTime = time;

		int hours = Mathf.FloorToInt((leftOverTime / 60) / 60);
		leftOverTime -= hours * 60 * 60;

		int mins = Mathf.FloorToInt(leftOverTime / 60);
		leftOverTime -= mins * 60;

		return hours + ":" + mins + ":" + leftOverTime;
	}

	public IEnumerator playerDeath(){
		deathScreen.GetComponent<Animator> ().Play ("FadeOut");
		yield return new WaitForSeconds(1);
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);

	}

	public int getHealth(){
		Player script = (Player)player.GetComponent (typeof(Player));
		return script.health;
	}
}
