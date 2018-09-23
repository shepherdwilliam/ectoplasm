using System.Collections;
using System.Collections.Generic;
using Bunkasai2018;
using UnityEngine;

namespace Bunkasai2018 {
	public static class HighScoreManager {

		public static Score[] highScore = new Score[200];
		public static Score thisGameScore;
		public static int i = 0;

		public static void PutThisGameScoreIntoHighScore () {
			highScore[i] = thisGameScore;
			i++;
		}

		public static void SaveScore (Score score) {
			PlayerPrefsUtility.SetObject (score.GetId ().ToString (), score);
			for (int i = 0; i < highScore.Length; i++) {
				if (highScore[i] == null || highScore[i].GetScore () < score.GetScore ()) {
					highScore[i] = score;
					break;
				}
			}
		}
	}
}