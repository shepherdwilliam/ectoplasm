using System.Collections;
using System.Collections.Generic;
using Bunkasai2018;
using UnityEngine;

namespace Bunkasai2018 {
	public class WiimoteNumItem : MonoBehaviour {

		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {
			GetComponent<UnityEngine.UI.Toggle> ().interactable = transform.root.GetComponent<SettingsManager> ().IsItemEnabled (transform.GetSiblingIndex ());
		}
	}
}