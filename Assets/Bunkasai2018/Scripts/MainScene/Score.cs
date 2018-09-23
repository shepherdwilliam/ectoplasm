using System;
using UnityEngine;

namespace Bunkasai2018 {
	public class Score {

		private bool clearBool;
		private int id;
		private int score;
		private float startTime;
		private System.DateTime showStartTime;
		private float endTime;
		private static int totalId = 0;

		public Score () {
			this.clearBool = false;
			this.id = totalId;
			totalId++;
			this.score = 0;
		}

		public bool GetCleared () {
			return clearBool;
		}

		public void Cleared () {
			clearBool = true;
		}

		public int GetId () {
			return id;
		}

		public int GetScore () {
			return score;
		}

		public void AddScore (int increment = 1) {
			score += increment;
		}

		public float GetStartTime () {
			return startTime;
		}

		public System.DateTime GetShowStartTime () {
			return showStartTime;
		}

		public void SetStartTime () {
			startTime = Time.time;
			showStartTime = DateTime.Now;
		}

		public float GetEndTime () {
			return endTime;
		}

		public void SetEndTime () {
			endTime = Time.time;
		}
	}
}