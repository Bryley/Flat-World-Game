using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour {

	public void goToMainMenu(){
		Time.timeScale = 1;
		SceneManager.LoadScene (0);
	}
}
