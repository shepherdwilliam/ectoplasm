using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Bunkasai2018 {
	public class DishMoveScript : MonoBehaviour {
		[SerializeField] private GameObject[] dishParts;
		private float dishTargetDegree = 0;
		private float dishNowDegree = 0;
		public bool continueMove = true;

		// Use this for initialization
		void Start () {
			UpdateDishTargetDegree (true);
		}

		void UpdateDishTargetDegree (bool sideA) {
			Sequence sequence = DOTween.Sequence ();
			sequence.Append (DOTween.To (
				() => dishNowDegree, // 何を対象にするのか
				num => dishNowDegree = num, // 値の更新
				dishTargetDegree, // 最終的な値
				1).SetEase (Ease.Linear).OnUpdate (() => {
				foreach (GameObject parts in dishParts) {
					parts.transform.rotation = Quaternion.Euler (new Vector3 (-90, dishNowDegree - 90, 0));
				}
			}).OnComplete (() => {
				if (sideA) {
					dishTargetDegree = 45;
				} else {
					dishTargetDegree = -45;
				}
				if (continueMove) {
					UpdateDishTargetDegree (!sideA);
				}
			}));
		}
	}
}