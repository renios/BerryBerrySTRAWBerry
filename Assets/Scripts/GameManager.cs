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

	public Image FifthLid;
	public Image SixthLid;

	public GameObject Scoop;
	public Animator ScoopAnim;

	public GameObject ScoopIcecreamPrefab;
	public GameObject ScoopIcecreamParent;

	public Text StageText;
	public Text TimeText;
	public Text GoldText;

	public GameObject LaughBell;
	public Animator ShakeAnim;

	public GameObject SuccessEffect;
	public GameObject FailEffect;

	public AudioSource TickTockSource;

	public GameObject StartPanel;
	public Text StartText;

	public GameObject ResultPanel;
	public Text ResultStageText;
	public Text ResultText;

	List<Customer> customers = new List<Customer>();
	List<Icecream> scoopIcecreams = new List<Icecream>();

	float thinkingFaceProb = 1.5f;
	float laughBellProb = 15;
	int minScoop = 2;
	int maxScoop = 5;
	int maxServedScoop = 5;
	int icecreamVary = 6;

	int initialTime = 720;
	int currentTime;
	int clockDelay = 2;
	int clockTerm = 10;
	float deltaTime;
	bool lastOrder;
	int lastOrderTime = 1080;

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
		List<IcecreamTaste> tasteList = new List<IcecreamTaste>();
		List<bool> thinkingFaceList = new List<bool>();
		var scoopNumber = Random.Range(minScoop, maxScoop+1);
		for (int i = 0; i < scoopNumber; i++) {
			IcecreamTaste taste = (IcecreamTaste)Random.Range(0, icecreamVary);
			tasteList.Add(taste);
			var isThinkingFace = Random.Range(0, 100f) < thinkingFaceProb;
			thinkingFaceList.Add(isThinkingFace);
		}

		var customerObject = Instantiate(CustomerPrefab, CustomerParent.transform);
		var customer = customerObject.GetComponent<Customer>();
		customer.Initialize(isLaughBell, tasteList, thinkingFaceList);
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
		else if (icecreamVary > 4 && inputString == "g") {
			taste = IcecreamTaste.GreenTea;
			xPos = 912;
		}
		else if (icecreamVary > 5 && inputString == "h") {
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
		ScoopAnim.SetTrigger("Move");
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
			ShakeAnim.enabled = true;
			ShakeAnim.Play("Truck");
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
		var thinkingFaceList = customer.ThinkingFaceList;

		// 길이가 다를 경우 -> false
		if (order.Count != scoopIcecreams.Count) return false;

		// 맛 판별
		for (int i = 0; i < order.Count; i++) {
			// ThinkingFace면 무조건 통과
			if (thinkingFaceList[i] == true)
				continue;

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

		if (currentTime >= lastOrderTime - 60 && !TickTockSource.enabled) {
			TickTockSource.enabled = true;
		}

		if (currentTime >= lastOrderTime) {
			lastOrder = true;
			TimeText.color = Color.red;
		}
	}

	void UpdateGold() {
		if (currentGold < 0) currentGold = 0;
		GoldText.text = currentGold.ToString() + "G";
	}

	void Initialize() {
		var stage = GameData._stage;
		StartPanel.SetActive(true);
		StartText.text = "Stage " + stage;
		soundManager.Play(SE.Shutter);

		playable = false;
		printResult = false;

		lastOrder = false;

		laughBellCount = 0;

		// 각종 변수
		currentTime = GameData._openTime;
		lastOrderTime = GameData._lastOrderTime;
		clockDelay = GameData._clockDelay;
		clockTerm = GameData._clockTerm;
		icecreamVary = GameData._icecreamVary[stage-1]; 
		minScoop = GameData._minScoop[stage-1];
		maxScoop = GameData._maxScoop[stage-1];
		laughBellProb = GameData._laughBellProb[stage-1];
		thinkingFaceProb = GameData._thinkingFaceProb[stage-1];

		// 안쓰는 아이스크림 비활성화
		if (icecreamVary < 5) FifthLid.enabled = true;
		if (icecreamVary < 6) SixthLid.enabled = true;

		// 상단 UI 3종
		StageText.text = "Stage " + stage;

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
		ShakeAnim.enabled = false;
		SuccessEffect.SetActive(false);
		FailEffect.SetActive(false);
		TickTockSource.enabled = false;

		soundManager.Play(SE.Shutter);
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
					soundManager.Play(SE.Bell);
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
			currentTime = lastOrderTime - 30;
			deltaTime = 5;
			UpdateClock();
		}
	}
}
