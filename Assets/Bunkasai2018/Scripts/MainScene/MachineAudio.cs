using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bunkasai2018 {
	public class MachineAudio : MonoBehaviour {

		[SerializeField] private AudioClip[] soundtrack;
		[SerializeField] private GameObject gameManagerObject;
		private AudioSource audioSource;

		// Use this for initialization
		void Start () {
			audioSource = GetComponent<AudioSource> ();
			PlayNextSong ();
		}

		// Update is called once per frame
		void Update () {

		}

		void PlayNextSong () {
			if (gameManagerObject.GetComponent<GameManager> ().IsCurrentState (GameState.Game)) {
				audioSource.clip = soundtrack[Random.Range (0, soundtrack.Length)];
				audioSource.Play ();
				Invoke ("PlayNextSong", audioSource.clip.length);
			}
		}
	}
}