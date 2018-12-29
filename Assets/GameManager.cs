using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enums;
using System.Linq;

public class GameManager : MonoBehaviour {

	public SoundManager soundManager;

	public GameObject CustomerPrefab;
	public GameObject CustomerParent;

	public GameObject Scoop;

	public GameObject ScoopIcecreamPrefab;
	public GameObject ScoopIcecreamParent;

	public Text StageText;
	public Text TimeText;
	public Text GoldText;

	public GameObject laughBell;

	List<Customer> customers = new List<Customer>();
	List<Icecream> scoopIcecreams = new List<Icecream>();

	float laughBellProb = 15;
	int minScoop = 2;
	int maxScoop = 5;
	int maxServedScoop = 5;
	int icecreamVary = 6;

	int initialTime = 600;
	int currentTime;
	int clockDelay = 4;
	int clockTerm = 10;
	float deltaTime;
	bool lastOrder;

	int currentGold;
	int successGoldPerScoop = 1;
	int failGoldPerScoop = 2;

	void MakeCustomer() {
		// 웃음벨 여부
		var isLaughBell = Random.Range(0, 100) < laughBellProb;
		// 주문 결정
		List<IcecreamTaste> icecream = new List<IcecreamTaste>();
		var scoopNumber = Random.Range(minScoop, maxScoop+1);
		for (int i = 0; i < scoopNumber; i++) {
			IcecreamTaste taste = (IcecreamTaste)Random.Range(0, icecreamVary);
			icecream.Add(taste);
		}

		var customerObject = Instantiate(CustomerPrefab, CustomerParent.transform);
		var customer = customerObject.GetComponent<Customer>();
		customer.Initialize(isLaughBell, icecream);
		customers.Add(customer);
	}

	void AddScoop(string inputString) {
		// 5개 초과로 올리려면 경고음(미구현)만 들림
		if (scoopIcecreams.Count >= maxServedScoop) {
			Debug.LogWarning("Too many scoop");
			soundManager.Play(SE.TooManyScoop);
			// TODO: 경고음
			return;
		}
		
		IcecreamTaste taste;
		int xPos;
		int yPos = 60;

		if (inputString == "a") {
			taste = IcecreamTaste.MintChoco;
			xPos = 80;
		}
		else if (inputString == "s") {
			taste = IcecreamTaste.Choco;
			xPos = 288;
		}
		else if (inputString == "d") {
			taste = IcecreamTaste.BerryBerryStrawBerry;
			xPos = 496;
		}
		else if (inputString == "f") {
			taste = IcecreamTaste.Vanilla;
			xPos = 704;
		}
		else if (inputString == "g") {
			taste = IcecreamTaste.GreenTea;
			xPos = 912;
		}
		else if (inputString == "h") {
			taste = IcecreamTaste.ShootingStar;
			xPos = 1120;
		}
		else return;

		Scoop.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);
		var scoopIcecreamObject = Instantiate(ScoopIcecreamPrefab, ScoopIcecreamParent.transform);
		var scoopIcecream = scoopIcecreamObject.GetComponent<Icecream>();
		scoopIcecream.Initialize(taste);
		scoopIcecreams.Add(scoopIcecream);
		soundManager.Play(SE.AddScoop);
	}

	void RemoveScoop() {
		if (scoopIcecreams.Count < 1) {
			soundManager.Play(SE.TooManyScoop);
			return;
		}

		var scoop = scoopIcecreams[scoopIcecreams.Count - 1];
		scoopIcecreams.Remove(scoop);
		scoop.gameObject.SetActive(false);
		soundManager.Play(SE.AddScoop);
	}

	void ServeIcecream() {
		// 손님이 없을 경우 패스
		if (customers.Count < 1) return;

		// 서빙 판정
		if (IsMatching()) {
			int successGold = scoopIcecreams.Count * successGoldPerScoop;
			currentGold += successGold;
			soundManager.Play(SE.Success);
			Debug.LogWarning("-- Success --");
		}
		else {
			int failGold = scoopIcecreams.Count * failGoldPerScoop;
			currentGold -= failGold;
			soundManager.Play(SE.Fail);
			Debug.LogWarning("-- Fail --");
		}

		// 웃음벨 (미구현)
		if (IsLaughBellActive()) {
			soundManager.Stop();
			if (laughBell.activeInHierarchy) {
				laughBell.SetActive(false);
			}
			laughBell.SetActive(true);
			laughBell.GetComponent<Timer>().ResetTime();
			Debug.LogWarning(">>> LaughBell <<<");
		}

		// 손님 보냄
		var customer = customers[0];
		customers.Remove(customer);
		customer.gameObject.SetActive(false);

		// 아이스크림 리셋
		int currentScoopNumber = scoopIcecreams.Count;
		for (int i = 0; i < currentScoopNumber; i++) {
			var scoop = scoopIcecreams[currentScoopNumber - 1 - i];
			scoopIcecreams.Remove(scoop);
			scoop.gameObject.SetActive(false);
		}
	}

	bool IsMatching() {
		var customer = customers[0];
		var order = customer.Order;

		// 길이가 다를 경우 -> false
		if (order.Count != scoopIcecreams.Count) return false;

		// 맛 판별 (TODO: thinking_face 아무맛)
		for (int i = 0; i < order.Count; i++) {
			if (order[i] != scoopIcecreams[i].GetComponent<Icecream>().Taste)
				return false;
		}

		return true;
	}

	bool IsLaughBellActive() {
		var customer = customers[0];

		// 주는 아이스크림이 비어있을 경우 -> false
		if (scoopIcecreams.Count == 0) return false;

		if (scoopIcecreams.Find(scoop => 
				scoop.GetComponent<Icecream>().Taste 
				== IcecreamTaste.BerryBerryStrawBerry) 
			&& customer.LaughBell)
			return true;
		else return false;
	}

	void UpdateClock() {
		if (currentTime >= 1200) {
			lastOrder = true;
			return;
		}

		deltaTime += Time.deltaTime;
		if (deltaTime > clockDelay) {
			deltaTime = 0;
			currentTime += clockTerm;

			TimeText.text = (currentTime / 60).ToString("D2") + ":" 
						+ (currentTime % 60).ToString("D2");
		}
	}

	void UpdateGold() {
		if (currentGold < 0) currentGold = 0;
		GoldText.text = currentGold.ToString() + "G";
	}

	void Initialize() {
		lastOrder = false;
		TimeText.text = (initialTime / 60).ToString("D2") + ":" 
						+ (initialTime % 60).ToString("D2");
		currentTime = initialTime;
		deltaTime = 0;
	}

	// Use this for initialization
	void Start () {
		Initialize();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateClock();
		UpdateGold();

		if (Input.GetKeyDown(KeyCode.Return)) {
			// if (customers.Count > 2) {
			// 	var customer = customers[0];
			// 	customers.Remove(customer);
			// 	customer.gameObject.SetActive(false);
			// }

			while (customers.Count < 3)
				MakeCustomer();
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			ServeIcecream();
		}

		if (Input.GetKeyDown(KeyCode.Backspace)) {
			RemoveScoop();
		}

		if (Input.anyKeyDown) {
			AddScoop(Input.inputString);
		}

		while (customers.Count < 3)
			MakeCustomer();
	}
}
