using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown) {
			if (Input.inputString == "1") {
				GameData._stage = 1;
				GameData._totalGold = 0;
			}
			if (Input.inputString == "2") GameData._stage = 2;
			if (Input.inputString == "3") GameData._stage = 3;
			if (Input.inputString == "4") GameData._stage = 4;
			if (Input.inputString == "5") GameData._stage = 5;
			if (Input.inputString == "6") GameData._stage = 6;
			if (Input.inputString == "7") GameData._stage = 7;
			if (Input.inputString == "8") GameData._stage = 8;
			if (Input.inputString == "9") GameData._stage = 9;
		}
	}
}
