using System.Collections;
using System.Collections.Generic;
using Bunkasai2018;
using DG.Tweening;
using UnityEngine;

public class ButtonManager : MonoBehaviour {

	[SerializeField] private GameObject gameManagerObject;
	[SerializeField] private GameObject display2ManagerObject;
	[SerializeField] private GameObject player;
	[SerializeField] public ArduinoSerialHandler serialHandler;
	[SerializeField] private ParticleSystem bombParticle;
	[SerializeField] private float bombStrength = 20;
	private int bombRemaining = 3;
	private float lastBombTime = 0;
	private float bombCoolTime = 5;

	// Use this for initialization
	void Start () {
		//serialHandler.OnDataReceived += OnDataReceived;
	}

	// Update is called once per frame
	void Update () {
		if (gameManagerObject.GetComponent<GameManager> ().IsCurrentState (GameState.Game)) {
			if (Input.GetKeyDown (KeyCode.Keypad0) || Input.GetKeyDown (KeyCode.Keypad1) || Input.GetKeyDown (KeyCode.Keypad2) || Input.GetKeyDown (KeyCode.Keypad3) || Input.GetKeyDown (KeyCode.Keypad4) || Input.GetKeyDown (KeyCode.Keypad5) || Input.GetKeyDown (KeyCode.Keypad6) || Input.GetKeyDown (KeyCode.Keypad7) || Input.GetKeyDown (KeyCode.Keypad8) || Input.GetKeyDown (KeyCode.Keypad9)) {
				if (Time.time > lastBombTime + bombCoolTime) {
					UseBomb ();
				}
			}
		}
	}

	void OnDataReceived (string message) {
		Debug.Log (message);
		var data = message.Split (new string[] { "\t" }, System.StringSplitOptions.None);
		if (data.Length < 2) return;

		try {

		} catch (System.Exception e) {
			Debug.LogWarning (e.Message);
		}
	}

	private void UseBomb () {
		if (bombRemaining > 0) {
			bombRemaining--;
			gameManagerObject.GetComponent<GameManager> ().bombImages[bombRemaining].GetComponent<UnityEngine.UI.Image> ().DOFade (0, 0f);
			var playerController = player.GetComponent<PlayerController> ();
			playerController.actionPointGroup.transform.GetChild (playerController.actionPointIdNow).GetComponent<ActionPoint> ().AttackAllGhosts (bombStrength);
			lastBombTime = Time.time;
			bombParticle.Play ();
			GetComponent<AudioSource> ().Play ();
			display2ManagerObject.GetComponent<Display2Manager> ().BombUpdate (bombRemaining);
		}
	}
}