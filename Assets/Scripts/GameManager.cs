using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enums;
using System.Linq;
using UnityEngine.SceneManagement;

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

	public GameObject LaughBell;

	public GameObject SuccessEffect;
	public GameObject FailEffect;

	public GameObject StartPanel;
	public Text StartText;

	public GameObject ResultPanel;
	public Text ResultStageText;
	public Text ResultText;

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
	int totalGold;

	int laughBellCount;

	bool playable;
	bool printResult;

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

	void PlayEffect(Effect effect) {
		SuccessEffect.SetActive(false);
		FailEffect.SetActive(false);

		if (effect == Effect.Success) {
			SuccessEffect.SetActive(true);
		}
		if (effect == Effect.Fail) {
			FailEffect.SetActive(true);
		}
	}

	void ServeIcecream() {
		// 손님이 없을 경우 패스
		if (customers.Count < 1) return;

		// 서빙 판정
		if (IsMatching()) {
			int successGold = scoopIcecreams.Count * successGoldPerScoop;
			currentGold += successGold;
			PlayEffect(Effect.Success);
			soundManager.Play(SE.Success);
			Debug.LogWarning("-- Success --");
		}
		else {
			int failGold = scoopIcecreams.Count * failGoldPerScoop;
			currentGold -= failGold;
			PlayEffect(Effect.Fail);
			soundManager.Play(SE.Fail);
			Debug.LogWarning("-- Fail --");
		}

		// 웃음벨
		if (IsLaughBellActive()) {
			laughBellCount += 1;
			soundManager.Stop();
			if (LaughBell.activeInHierarchy) {
				LaughBell.SetActive(false);
			}
			LaughBell.SetActive(true);
			LaughBell.GetComponent<Timer>().ResetTime();
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

		// 마지막 손님이면 게임 종료
		UpdateGold();
		if (lastOrder) {
			playable = false;
			printResult = true;

			PrintResult();
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
		if (lastOrder) return;

		deltaTime += Time.deltaTime;
		if (deltaTime > clockDelay) {
			deltaTime = 0;
			currentTime += clockTerm;

			TimeText.text = (currentTime / 60).ToString("D2") + ":" 
						+ (currentTime % 60).ToString("D2");
		}

		if (currentTime >= 1200) {
			lastOrder = true;
			TimeText.color = Color.red;
		}
	}

	void UpdateGold() {
		if (currentGold < 0) currentGold = 0;
		GoldText.text = currentGold.ToString() + "G";
	}

	void Initialize() {
		StartPanel.SetActive(true);
		StartText.text = "Stage " + GameData._stage;

		playable = false;
		printResult = false;

		lastOrder = false;

		laughBellCount = 0;

		// 상단 UI 3종
		StageText.text = "Stage " + GameData._stage;

		TimeText.color = Color.black;
		TimeText.text = (initialTime / 60).ToString("D2") + ":" 
						+ (initialTime % 60).ToString("D2");
		currentTime = initialTime;
		deltaTime = 0;

		currentGold = GameData._totalGold;
		GoldText.text = currentGold.ToString() + "G";
	}

	void PrintResult() {
		// 활성화된 이펙트 끄기
		LaughBell.SetActive(false);
		SuccessEffect.SetActive(false);
		FailEffect.SetActive(false);

		ResultPanel.SetActive(true);

		totalGold = currentGold - 20 - 50 - (laughBellCount * 10);
		ResultStageText.text = "Stage " + GameData._stage;
		ResultText.text = currentGold + "G" + '\n' +
						  "-20G" + '\n' +
						  "-50G" + '\n' +
						  "-" + (laughBellCount * 10) + "G" + '\n' + '\n' +
						  totalGold;
	}

	// Use this for initialization
	void Start () {
		Initialize();
	}
	
	// Update is called once per frame
	void Update () {
		if (!playable) {
			if (Input.GetKeyDown(KeyCode.Space)) {
				// 스테이지 시작
				if (!printResult) {
					StartPanel.SetActive(false);
					playable = true;
				}
				// 스테이지 종료 (게임오버 / 엔딩 / 다음스테이지)
				else {
					if (totalGold < 0) {
						SceneManager.LoadScene("GameOver");
						return;
					}
					
					if (GameData._stage > 9) {
						SceneManager.LoadScene("Ending");
						return;
					}

					GameData._stage += 1;
					GameData._totalGold = totalGold;
					SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
				}
			}

			return;
		}

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

		while (!lastOrder && customers.Count < 3)
			MakeCustomer();

		if (Input.GetKeyDown(KeyCode.Escape)) {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		// 테스트용. 시계 강제 업데이트
		if (Input.GetKeyDown(KeyCode.Alpha0)) {
			currentTime = 1180;
			deltaTime = 5;
			UpdateClock();
		}
	}
}
