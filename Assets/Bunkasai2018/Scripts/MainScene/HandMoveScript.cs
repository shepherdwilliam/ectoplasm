using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Bunkasai2018 {
	public class HandMoveScript : MonoBehaviour {
		private float handTargetDegree = 0;
		private float handNowDegree = 0;
		public bool continueMove = true;

		// Use this for initialization
		void Start () {
			UpdateHandTargetDegree ();
		}

		void UpdateHandTargetDegree () {
			Sequence sequence = DOTween.Sequence ();
			sequence.Append (DOTween.To (
				() => handNowDegree, // 何を対象にするのか
				num => handNowDegree = num, // 値の更新
				handTargetDegree, // 最終的な値
				Random.Range (0.2f, 1)).SetEase (Ease.Linear).OnUpdate (() => {
				transform.rotation = Quaternion.Euler (new Vector3 (handNowDegree - 90, -90, 0));
			}).OnComplete (() => {
				handTargetDegree = Random.Range (-120, 120);
				if (continueMove) {
					UpdateHandTargetDegree ();
				}
			}));
		}
	}
}