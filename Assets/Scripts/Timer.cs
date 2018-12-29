using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {

	float remainTime;
	int lifeTime = 10;

	public void ResetTime() {
		remainTime = 0;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		remainTime += Time.deltaTime;

		if (remainTime > lifeTime) {
			gameObject.SetActive(false);
		}
	}
}
