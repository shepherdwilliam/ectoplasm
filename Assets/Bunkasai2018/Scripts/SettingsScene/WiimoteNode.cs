using System.Collections;
using System.Collections.Generic;
using Bunkasai2018;
using UnityEngine;
using WiimoteApi;

namespace Bunkasai2018 {
	public class WiimoteNode : MonoBehaviour {
		[SerializeField] private UnityEngine.UI.Text nameLabel;
		[SerializeField] private UnityEngine.UI.Toggle toggle1;
		[SerializeField] private UnityEngine.UI.Toggle toggle2;
		[SerializeField] private UnityEngine.UI.Toggle toggle3;
		[SerializeField] private UnityEngine.UI.Toggle toggle4;
		private byte pastWiimoteByte = 0;

		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {

		}

		public void OnNumValueChanged (int result) {
			byte usedWiimoteByte = transform.root.GetComponent<SettingsManager> ().usedWiimoteByte;
			usedWiimoteByte = (byte) (usedWiimoteByte ^ pastWiimoteByte);
			switch (result) {
				case 0:
					transform.root.GetComponent<SettingsManager> ().usedWiimoteByte = (byte) (usedWiimoteByte ^ 0);
					pastWiimoteByte = 0;
					break;
				case 1:
					transform.root.GetComponent<SettingsManager> ().usedWiimoteByte = (byte) (usedWiimoteByte ^ 1);
					pastWiimoteByte = 1;
					break;
				case 2:
					transform.root.GetComponent<SettingsManager> ().usedWiimoteByte = (byte) (usedWiimoteByte ^ 2);
					pastWiimoteByte = 2;
					break;
				case 3:
					transform.root.GetComponent<SettingsManager> ().usedWiimoteByte = (byte) (usedWiimoteByte ^ 4);
					pastWiimoteByte = 4;
					break;
				case 4:
					transform.root.GetComponent<SettingsManager> ().usedWiimoteByte = (byte) (usedWiimoteByte ^ 8);
					pastWiimoteByte = 8;
					break;
				case 5:
					transform.root.GetComponent<SettingsManager> ().usedWiimoteByte = (byte) (usedWiimoteByte ^ 16);
					pastWiimoteByte = 16;
					break;
				case 6:
					transform.root.GetComponent<SettingsManager> ().usedWiimoteByte = (byte) (usedWiimoteByte ^ 32);
					pastWiimoteByte = 32;
					break;
				default:
					break;
			}
			if (result > 0) {
				SettingsManager.wiimotePathTable[nameLabel.text] = result;
			} else {
				SettingsManager.wiimotePathTable.Remove (nameLabel.text);
			}
			toggle1.GetComponent<UnityEngine.UI.Toggle> ().isOn = SettingsManager.sideABool;
			toggle2.GetComponent<UnityEngine.UI.Toggle> ().isOn = result == 4 || result == 5 || result == 6;
			toggle3.GetComponent<UnityEngine.UI.Toggle> ().isOn = result == 2 || result == 3 || result == 6;
			toggle4.GetComponent<UnityEngine.UI.Toggle> ().isOn = result == 1 || result == 3 || result == 5;
			WiimoteManager.Wiimotes[transform.GetSiblingIndex ()].SendPlayerLED (SettingsManager.sideABool, result == 4 || result == 5 || result == 6, result == 2 || result == 3 || result == 6, result == 1 || result == 3 || result == 5);
		}
		public void SetName (string name) {
			nameLabel.text = name;
		}
	}
}