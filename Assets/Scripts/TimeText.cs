using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeText : MonoBehaviour {

	Text textComp;

	public string prefix = "YOUR TIME WAS: ";

	// Use this for initialization
	void Start () {
		textComp = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		textComp.text = prefix + GameManager.instance.getTime ();
	}
}
