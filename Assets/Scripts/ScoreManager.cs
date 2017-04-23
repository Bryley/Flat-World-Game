using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		int[] scores = new int[5];

		for(int i=0; i<5;i++){
			if (PlayerPrefs.HasKey ("score" + (i + 1))) {
				scores [i] = PlayerPrefs.GetInt ("score" + (i + 1));
			}else{
				scores [i] = -5;
			}
		}

		Array.Sort (scores);

		for(int i = 0; i<scores.Length; i++){
			PlayerPrefs.SetInt ("highest" + i, scores [i]);
		}
	}
}
