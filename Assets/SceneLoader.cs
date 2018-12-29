using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	public string NextSceneName;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			// 타이틀에서 정보 초기화
			if (SceneManager.GetActiveScene().name == "Title") {
				GameData._stage = 1;
				GameData._totalGold = 0;
			}

			SceneManager.LoadScene(NextSceneName);
		}
	}
}
