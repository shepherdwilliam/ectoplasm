using System;
using Bunkasai2018;
using DG.Tweening;
using UnityEngine;

namespace Bunkasai2018 {
	public class DoorMechanism : MonoBehaviour {

		[SerializeField] private GameObject[] doors;
		[SerializeField] private Vector3[] doorEndPositions;
		[SerializeField] private Vector3[] doorEndRotations;
		[SerializeField] private float time = 0.2f;

		private AudioSource openingDoorSoundSource;

		void Start () {
			openingDoorSoundSource = GetComponent<AudioSource> ();
		}

		void Update () {

		}

		public void DoorMove () {
			openingDoorSoundSource.Play ();
			var sequence = DOTween.Sequence ();
			for (int i = 0; i < doors.Length; i++) {
				sequence.Join (doors[i].transform.DOLocalMove (doorEndPositions[i], time).SetEase (Ease.InExpo));
				sequence.Join (doors[i].transform.DOLocalRotate (doorEndRotations[i], time).SetEase (Ease.InExpo));
			}
		}
	}
}