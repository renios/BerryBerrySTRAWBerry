using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enums;

public class Icecream : MonoBehaviour {

	public Image Image;
	public IcecreamTaste Taste { get; private set; }
	public List<Sprite> IcecreamImages;

	public void Initialize(IcecreamTaste icecream) {
		Taste = icecream;
		Image.sprite = IcecreamImages[(int)icecream];
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
