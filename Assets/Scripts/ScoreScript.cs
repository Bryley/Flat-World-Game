using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour {

	public int numberID = 1;

	// Use this for initialization
	void Start () {
		int score = PlayerPrefs.GetInt ("highest" + numberID);

		Text textComp = GetComponent<Text> ();
		if (score < 0) {
			textComp.text = " ";
		} else {
			textComp.text = "SCORE " + numberID + ": " + getTime(score);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public string getTime(int leftOverTime){

		int hours = Mathf.FloorToInt((leftOverTime / 60) / 60);
		leftOverTime -= hours * 60 * 60;

		int mins = Mathf.FloorToInt(leftOverTime / 60);
		leftOverTime -= mins * 60;

		return hours + ":" + mins + ":" + leftOverTime;
	}
}
