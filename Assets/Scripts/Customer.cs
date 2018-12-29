using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enums;

public class Customer : MonoBehaviour {

	public Image Image;
	public GameObject IcecreamParent;
	public GameObject IcecreamPrefab;
	public List<IcecreamTaste> Order { get; private set;}
	public List<bool> ThinkingFaceList {get; private set; }
	public bool LaughBell { get; private set; }

	public void Initialize(bool isLaughBell, List<IcecreamTaste> order, List<bool> thinkingFaceList) {
		// 손님의 외모 결정
		Object[] spritePool;

		if (isLaughBell) {
			spritePool = Resources.LoadAll("Character/LaughBell", typeof(Sprite));
		} else {
			spritePool = Resources.LoadAll("Character/NotLaughBell", typeof(Sprite));
		}

		LaughBell = isLaughBell;
		var sprite = spritePool[Random.Range(0, spritePool.Length)] as Sprite;
		Image.sprite = sprite;

		// 아이스크림 배치
		Order = order;
		ThinkingFaceList = thinkingFaceList;
		for (int i = 0; i < order.Count; i++) {
			var icecream = Instantiate(IcecreamPrefab, IcecreamParent.transform);
			icecream.GetComponent<Icecream>().Initialize(order[i], thinkingFaceList[i]);
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
