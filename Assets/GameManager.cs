using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enums;

public class GameManager : MonoBehaviour {

	public GameObject CustomerPrefab;
	public GameObject CustomerParent;

	List<Customer> customers = new List<Customer>();

	float laughBellProb = 15;
	int minScoop = 2;
	int maxScoop = 5;
	int icecreamVary = 6;

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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Return)) {
			if (customers.Count > 2) {
				var destroyCustomer = customers[0];
				customers.Remove(destroyCustomer);
				destroyCustomer.gameObject.SetActive(false);
			}
			MakeCustomer();
		}
	}
}
