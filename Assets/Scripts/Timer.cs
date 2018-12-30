using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {

	public Animator PairedAnim;

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
			if (PairedAnim != null) 
				PairedAnim.enabled = false;
			gameObject.SetActive(false);
		}
	}
}
