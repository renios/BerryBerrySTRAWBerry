using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class SoundManager : MonoBehaviour {

	public AudioClip Success;
	public AudioClip Fail;
	public AudioClip AddScoop;
	public AudioClip TooManyScoop;

	public AudioSource audioSource;

	public void Play(SE se) {
		if (se == SE.Success)
			audioSource.PlayOneShot(Success);
		else if (se == SE.Fail)
			audioSource.PlayOneShot(Fail);
		else if (se == SE.AddScoop)
			audioSource.PlayOneShot(AddScoop);
		else if (se == SE.TooManyScoop)
			audioSource.PlayOneShot(TooManyScoop);
	}

	public void Stop() {
		audioSource.Stop();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
