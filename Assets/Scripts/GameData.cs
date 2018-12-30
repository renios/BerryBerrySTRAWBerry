using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData {
	public static int _stage = 1;
	public static int _totalGold = 0;

	// 각 스테이지 변수/확률
	public static int _openTime = 720;
	public static int _lastOrderTime = 1080;
	public static int _clockDelay = 2;
	public static int _clockTerm = 10;
	public static int[] _icecreamVary = {4, 4, 4, 5, 5, 5, 5, 6, 6, 6};
	public static int[] _minScoop = {1, 1, 1, 1, 1, 2, 2, 2, 2, 2};
	public static int[] _maxScoop = {4, 4, 4, 4, 4, 5, 5, 5, 5, 5};
	public static float[] _laughBellProb = {5, 5, 5, 7, 7, 10, 10, 15, 15, 15};
	public static float[] _thinkingFaceProb = {2, 2, 2, 1.5f, 1.5f, 1.5f, 1.5f, 1, 1, 1};
}
