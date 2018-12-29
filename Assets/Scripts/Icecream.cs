using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enums;

public class Icecream : MonoBehaviour {

	public Image Image;
	public Image ThinkingFaceImage;
	public IcecreamTaste Taste { get; private set; }
	public bool ThinkingFace { get; private set; }
	public List<Sprite> IcecreamImages;

	public void Initialize(IcecreamTaste icecream, bool isThinkingFace = false) {
		Taste = icecream;
		ThinkingFace = isThinkingFace;
		Image.sprite = IcecreamImages[(int)icecream];
		if (ThinkingFace)
			ThinkingFaceImage.enabled = true;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
