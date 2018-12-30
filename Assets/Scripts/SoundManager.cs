using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class SoundManager : MonoBehaviour {

	public AudioClip Success;
	public AudioClip Fail;
	public AudioClip AddScoop;
	public AudioClip TooManyScoop;
	public AudioClip Click;
	public AudioClip Shutter;
	public AudioClip Bell;
	public AudioClip TickTock;

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
		else if (se == SE.Click)
			audioSource.PlayOneShot(Click);
		else if (se == SE.Shutter)
			audioSource.PlayOneShot(Shutter);
		else if (se == SE.Bell)
			audioSource.PlayOneShot(Bell);
		else if (se == SE.TickTock)
			audioSource.PlayOneShot(TickTock);
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
