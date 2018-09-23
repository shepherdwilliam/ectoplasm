using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bunkasai2018 {
	public class GhostMachinePopUp : MonoBehaviour {

		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {

		}

		public void KillAllGhosts () {
			foreach (Transform child in transform) {
				child.GetComponent<Ghost> ().Kill ();
			}
		}

		public void AttackAllGhosts (float atk = 1) {
			foreach (Transform child in transform) {
				child.GetComponent<Ghost> ().Attacked (atk);
			}
		}
	}
}