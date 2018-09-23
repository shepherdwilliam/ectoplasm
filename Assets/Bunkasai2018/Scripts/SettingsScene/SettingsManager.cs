using System.Collections;
using System.Collections.Generic;
using Bunkasai2018;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using WiimoteApi;

namespace Bunkasai2018 {
	public class SettingsManager : MonoBehaviour {
		[SerializeField] private GameObject contentObject;
		[SerializeField] private GameObject wiimoteNodePrefab;
		public static bool sideABool = true;
		public byte usedWiimoteByte = 0;
		public static Dictionary<string, int> wiimotePathTable;
		//key:hidapi_path, value:player id(1-4) value

		private Rect error_hidapi_windowRect = new Rect (20, 20, 420, 170);
		private Rect error_wiimoteNotConnected_windowRect = new Rect (20, 200, 420, 170);
		public static bool error_hidapi = false;
		public static bool error_wiimoteNotConnected = false;

		[RuntimeInitializeOnLoadMethod]
		static void OnRuntimeMethodLoad () {
			Screen.SetResolution (1200, 900, true);
		}

		private void Awake () {
			//DontDestroyOnLoad (gameObject);
			int maxDisplayCount = 2;
			for (int i = 0; i < maxDisplayCount && i < Display.displays.Length; i++) {
				Display.displays[i].Activate ();
			}
		}

		// Use this for initialization
		void Start () {
			//Wiiリモコン接続表
			HIDapi.hid_init ();
			wiimotePathTable = new Dictionary<string, int> ();
			foreach (Wiimote remote in WiimoteManager.Wiimotes) {
				WiimoteManager.Cleanup (remote);
			}
		}

		// Update is called once per frame
		void Update () {
			//Debug.Log (usedWiimoteByte);
		}

		void OnGUI () {
			if (error_hidapi) {
				error_hidapi_windowRect = GUI.Window (0, error_hidapi_windowRect, ErrorHidapiWindow, "Error code: 1");
			}
			if (error_wiimoteNotConnected) {
				error_wiimoteNotConnected_windowRect = GUI.Window (0, error_wiimoteNotConnected_windowRect, ErrorWiimoteNotConnectedWindow, "Error code: 2");
			}
		}

		private void ErrorHidapiWindow (int windowID) {
			GUI.Label (new Rect (10, 20, 400, 100), "Error code:1\nHIDAPI(パソコンにwiiリモコン繋げるやつ)に問題が発生しました。アプリが1個しか動いていないことを確認してください。それでもダメならアプリを再起動してください。それでもダメならパソコンを再起動してください。それでもダメなら太一に連絡してください。");
			if (GUI.Button (new Rect (10, 130, 400, 30), "OK")) {
				error_hidapi = false;
			}
		}

		private void ErrorWiimoteNotConnectedWindow (int windowID) {
			GUI.Label (new Rect (10, 20, 400, 100), "Error code:2\nwiiリモコンが4つ繋がっていません。もしくは、1〜4番までのIDが選択されてません。");
			if (GUI.Button (new Rect (10, 130, 400, 30), "OK")) {
				error_wiimoteNotConnected = false;
			}
		}

		public void OnSideValueChanged (int result) {
			if (result == 0) {
				sideABool = true;
			} else {
				sideABool = false;
			}
		}

		public void OnLookForWiimotesButtonPressed () {
			WiimoteManager.FindWiimotes ();
		}

		public void OnListUpdateButtonPressed () {
			foreach (Transform child in contentObject.transform) {
				GameObject.Destroy (child.gameObject, 0f);
			}
			foreach (Wiimote mote in WiimoteManager.Wiimotes) {
				GameObject wiimote = Instantiate (wiimoteNodePrefab, contentObject.transform);
				wiimote.GetComponent<WiimoteNode> ().SetName (mote.hidapi_path);
			}
		}

		public void OnRunGameButtonPressed () {
			if (usedWiimoteByte == 15) {
				foreach (Wiimote mote in WiimoteManager.Wiimotes) {
					mote.SetupIRCamera (IRDataType.EXTENDED);
				}
				SceneManager.LoadSceneAsync ("main");
			} else {
				error_wiimoteNotConnected = true;
			}
		}

		public bool IsItemEnabled (int id) {
			if ((usedWiimoteByte >> (id - 2)) % 2 == 0) {
				return true;
			} else {
				return false;
			}
		}
	}
}