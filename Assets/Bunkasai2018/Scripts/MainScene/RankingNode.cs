using System;
using UnityEngine;

namespace Bunkasai2018 {
	public class RankingNode : MonoBehaviour {

		[SerializeField] private UnityEngine.UI.Text clearTimeLabel;
		[SerializeField] private UnityEngine.UI.Text startTimeLabel;
		private float clearTime;

		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {

		}

		public void SetTime (float _clearTime, System.DateTime _startTime) {
			clearTime = _clearTime;
			clearTimeLabel.text = "Clear Time:\n" + Mathf.FloorToInt (clearTime / 60).ToString () + ":" + (clearTime % 60).ToString ();
			startTimeLabel.text = "Start Time:\n" + _startTime.ToString ();
		}

		public float GetClearTime () {
			return clearTime;
		}
	}
}