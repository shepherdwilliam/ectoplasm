using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Video;

namespace Bunkasai2018 {
	public class CameraScript : MonoBehaviour {

		[SerializeField] Camera mainCamera;
		[SerializeField] GameObject ghostObject;
		[SerializeField] GameObject door;
		[SerializeField] GameObject plane;

		// Use this for initialization
		void Start () {
			var sequence = DOTween.Sequence ();
			int x = 0;
			sequence.Append (DOTween.To (() => x, num => x = num, 1, 1.0f).OnComplete (() => {
				door.GetComponent<AudioSource> ().Play ();
			}));
			sequence.Append (DOTween.To (() => x, num => x = num, 1, 4.0f).OnComplete (() => {
				ghostObject.transform.DOMove (new Vector3 (0, 3.8f, 0), 0.1f).SetRelative ().SetEase (Ease.OutExpo);
				ghostObject.transform.DOScale (new Vector3 (60, 60, 75), 0.1f).SetEase (Ease.Linear);
			}));
			sequence.Append (DOTween.To (() => x, num => x = num, 1, 0.1f).OnComplete (() => {
				ghostObject.transform.DOJump (new Vector3 (0, 0.1f, 0), 0.5f, 4, 0.6f).SetRelative ().SetEase (Ease.Linear);
				ghostObject.GetComponent<AudioSource> ().Play ();
			}));
			sequence.Append (DOTween.To (() => x, num => x = num, 1, 0.95f).OnComplete (() => {
				plane.GetComponent<MeshRenderer> ().enabled = true;
				plane.GetComponent<AudioSource> ().volume = 0.6f;
			}));

		}

		// Update is called once per frame
		void Update () {

		}
	}
}