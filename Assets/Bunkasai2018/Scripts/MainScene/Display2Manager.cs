using System;
using UnityEngine;
using WiimoteApi;

namespace Bunkasai2018 {
	public class Display2Manager : MonoBehaviour {

		[SerializeField] private GameObject gameManagerObject;
		[SerializeField] private UnityEngine.UI.Text stateLabel;
		[SerializeField] private UnityEngine.UI.Text dateTimeLabel;
		[SerializeField] private UnityEngine.UI.Text playTimeLabel;
		[SerializeField] private UnityEngine.UI.Text teamIdLabel;
		[SerializeField] private UnityEngine.UI.Text actionPointNowLabel;
		[SerializeField] private UnityEngine.UI.Text[] playerGunNumLabels;
		[SerializeField] private UnityEngine.UI.Text totalGunNumLabel;
		[SerializeField] private UnityEngine.UI.Text averageGunNumLabel;
		[SerializeField] private UnityEngine.UI.Text[] playerGunRateLabels;
		[SerializeField] private UnityEngine.UI.Text averageGunRateLabel;
		[SerializeField] private UnityEngine.UI.Text bombUsageLabel;
		[SerializeField] private GameObject contentObject;
		[SerializeField] private GameObject rankingNodePrefab;
		private static bool gameState = false; //false: title true: game
		public static int teamNumber = 0;
		private static int[] gunNums = new int[4];
		private static int[] hitGunNums = new int[4];

		void Awake () {
			//DontDestroyOnLoad (gameObject);
		}

		void Start () {
			stateLabel.text = "入れ替え中";
			dateTimeLabel.text = DateTime.Now.ToString ();

			if (SettingsManager.sideABool) {
				teamIdLabel.text = "A" + teamNumber;
			} else {
				teamIdLabel.text = "B" + teamNumber;
			}

			for (int i = 0; i < 4; i++) {
				gunNums[i] = 0;
				hitGunNums[i] = 0;
			}
		}

		// Update is called once per frame
		void Update () {
			dateTimeLabel.text = DateTime.Now.ToString ();
			if (HighScoreManager.thisGameScore != null && gameState && gameManagerObject.GetComponent<GameManager> ().IsCurrentState (GameState.Game)) {
				playTimeLabel.text = "Play Time: " + (Mathf.FloorToInt (Time.time - HighScoreManager.thisGameScore.GetStartTime ()) / 60).ToString () + ":" + ((Time.time - HighScoreManager.thisGameScore.GetStartTime ()) % 60).ToString ();
			} else if (HighScoreManager.thisGameScore != null) {
				playTimeLabel.text = "Clear Time: " + (Mathf.FloorToInt ((HighScoreManager.thisGameScore.GetEndTime () - HighScoreManager.thisGameScore.GetStartTime ()) / 60)).ToString () + ":" + ((HighScoreManager.thisGameScore.GetEndTime () - HighScoreManager.thisGameScore.GetStartTime ()) % 60).ToString ();
			}
		}

		public void GameStart () {
			gameState = true;
			stateLabel.text = "ゲーム中";

			teamNumber++;
			if (SettingsManager.sideABool) {
				teamIdLabel.text = "A" + teamNumber;
			} else {
				teamIdLabel.text = "B" + teamNumber;
			}

			for (int i = 0; i < 4; i++) {
				gunNums[i] = 0;
				hitGunNums[i] = 0;
			}
		}

		public void GameEnd () {
			gameState = false;
			stateLabel.text = "入れ替え中";
		}

		public void ActionPointUpdate (int number) {
			actionPointNowLabel.text = "ActionPoint Now:\n" + number;
		}

		public void GunUpdate (int number, bool hit) {
			gunNums[number]++;
			if (hit) {
				hitGunNums[number]++;
			}
			playerGunNumLabels[number].text = gunNums[number].ToString ();
			if (gunNums[number] == 0) {
				playerGunRateLabels[number].text = "0";
			} else {
				playerGunRateLabels[number].text = Math.Round ((float) hitGunNums[number] / gunNums[number], 4).ToString ();
			}
			int totalGunNum = 0;
			int totalHitNum = 0;
			for (int i = 0; i < 4; i++) {
				totalGunNum += gunNums[i];
				totalHitNum += hitGunNums[i];
			}
			totalGunNumLabel.text = totalGunNum.ToString ();
			averageGunNumLabel.text = (totalGunNum / 4).ToString ();
			if (totalGunNum == 0) {
				averageGunRateLabel.text = "0";
			} else {
				averageGunRateLabel.text = Math.Round ((float) totalHitNum / totalGunNum, 4).ToString ();
			}
		}

		public void BombUpdate (int bombLeft) {
			bombUsageLabel.text = "ボム残り回数: " + bombLeft + "/3";
		}

		public void RankingUpdate () {
			foreach (Transform child in contentObject.transform) {
				GameObject.Destroy (child.gameObject, 0f);
			}
			for (int j = 0; j <= HighScoreManager.i; j++) {
				if (HighScoreManager.highScore[j] != null) {
					GameObject rankingNode = Instantiate (rankingNodePrefab, contentObject.transform);
					rankingNode.GetComponent<RankingNode> ().SetTime (HighScoreManager.highScore[j].GetEndTime () - HighScoreManager.highScore[j].GetStartTime (), HighScoreManager.highScore[j].GetShowStartTime ());
				}
			}

			RankingSort ();
		}

		public void RankingSort () {
			for (int i = 1; i < contentObject.transform.childCount - 1; i++) {
				for (int j = 2; j < contentObject.transform.childCount - i + 1; j++) {
					if (contentObject.transform.GetChild (j - 1).GetComponent<RankingNode> ().GetClearTime () < contentObject.transform.GetChild (j - 2).GetComponent<RankingNode> ().GetClearTime ()) {
						contentObject.transform.GetChild (j - 1).SetSiblingIndex (j - 2);
					}
				}
			}
		}

		public void OnLookForWiimotesButtonPressed () {
			WiimoteManager.FindWiimotes ();
		}

		public void OnEnableWiimotesButtonPressed () {
			foreach (Wiimote mote in WiimoteManager.Wiimotes) {
				mote.SetupIRCamera (IRDataType.EXTENDED);
				mote.RequestIdentifyWiiMotionPlus ();
				mote.Accel.CalibrateAccel (AccelCalibrationStep.A_BUTTON_UP);
			}
		}
	}
}