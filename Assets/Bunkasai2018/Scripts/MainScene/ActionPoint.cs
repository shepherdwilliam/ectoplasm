using System.Collections;
using System.Collections.Generic;
using Bunkasai2018;
using DG.Tweening;
using DigitalRuby.RainMaker;
using UnityEngine;
using UnityEngine.Events;

namespace Bunkasai2018 {
	public class ActionPoint : MonoBehaviour {

		[SerializeField] private GameObject player;
		[SerializeField] private GameObject gameManagerObject;
		[SerializeField] private bool infiniteType = false;
		//false:なしまたは一括 true:無限
		[SerializeField] private bool gameEndBool = false;
		[SerializeField] public bool slowCamera = false;
		[SerializeField] public bool linearMove = false;
		[SerializeField] public bool lightFlicker = false;
		[SerializeField] private bool rainStart = false;
		[SerializeField] private bool textShow = false;
		[SerializeField] private string[] texts;
		[SerializeField] private AudioClip[] textAudios;
		[SerializeField] private float[] textTime;
		[SerializeField] private UnityEngine.Events.UnityEvent onMoveFinishedEvents = new UnityEvent ();
		private int ghostAliveNumNow;

		void Start () {
			if (!infiniteType) {
				ghostAliveNumNow = transform.childCount;
			}
			gameObject.GetComponent<MeshRenderer> ().enabled = false;
		}

		public void OnMoveAlmostFinished () {
			if (!infiniteType) {
				var playerController = player.GetComponent<PlayerController> ();
				foreach (Transform child in transform) {
					child.GetComponent<Ghost> ().PopThisUp (playerController.actionPointPositions[playerController.actionPointIdNow], 0, false);
				}
			}
		}

		public void OnMoveFinished () {
			if (textShow) {
				//博士がいない!?とりあえず機械を壊さないと!
				var sequence = DOTween.Sequence ();
				var gameManager = gameManagerObject.GetComponent<GameManager> ();
				sequence.Append (gameManager.dialogImage.DOFade (1, 0.2f).OnStart (() => {
					gameManager.openCloseAudioSource.PlayOneShot (gameManager.dialogOpenClip);
				}));
				int i = 0;
				int j = 0;
				foreach (string subtext in texts) {
					sequence.Append (gameManager.tutorialLabel.DOFade (1, 0.0f).OnStart (() => {
						gameManager.tutorialLabel.text = subtext;
						gameManager.communicationsAudioSource.PlayOneShot (textAudios[j]);
						j++;
					}));
					sequence.Append (gameManager.tutorialLabel.DOFade (1, textTime[i]));
					i++;
				}
				sequence.Append (gameManager.tutorialLabel.DOFade (0, 0.0f));
				if (infiniteType) {
					sequence.Append (gameManager.dialogImage.DOFade (0, 0.2f).OnStart (() => {
						gameManager.openCloseAudioSource.PlayOneShot (gameManager.dialogCloseClip);
					}).OnComplete (() => {
						transform.GetChild (0).GetComponent<GhostMachine> ().StartAttack (player.transform.position);
					}));
				} else {
					sequence.Append (gameManager.dialogImage.DOFade (0, 0.2f).OnStart (() => {
						gameManager.openCloseAudioSource.PlayOneShot (gameManager.dialogCloseClip);
					}).OnComplete (() => {
						CheckExit ();
					}));
				}
			}

			if (lightFlicker) {
				Sequence sequence = DOTween.Sequence ();
				sequence.Append (DOTween.To (() => 0, // Getter
					num => player.GetComponent<PlayerController> ().SetSpotlightIntensity (num), // Setter
					1, // 最終的な値
					1).SetEase (Ease.Linear));
			}

			if (rainStart) {
				DOTween.To (() => 0, // Getter
					num => gameManagerObject.GetComponent<GameManager> ().rainObject.GetComponent<RainScript> ().RainIntensity = num, // Setter
					1, // 最終的な値
					10).SetEase (player.GetComponent<PlayerController> ().lightCurve);
			}

			onMoveFinishedEvents.Invoke ();

			if (!textShow) {
				CheckExit ();
			}
		}

		public void OnExit () {
			if (gameEndBool) {
				//TODO: ラスボス撃破後のイベント
				gameManagerObject.GetComponent<GameManager> ().ChangeState (GameState.Clear);
			} else {
				player.GetComponent<PlayerController> ().MoveToNextActionPoint ();
			}
		}

		public void CheckExit () {
			if (!infiniteType && ghostAliveNumNow == 0) {
				OnExit ();
			}
		}

		public void OneGhostKilled () {
			ghostAliveNumNow--;
			CheckExit ();
		}

		public void KillAllGhosts () {
			if (infiniteType) {
				transform.GetChild (0).GetComponent<GhostMachine> ().popUpGameObject.GetComponent<GhostMachinePopUp> ().KillAllGhosts ();
			} else {
				foreach (Transform child in transform) {
					child.GetComponent<Ghost> ().Kill ();
				}
			}
		}

		public void AttackAllGhosts (float atk = 1) {
			if (infiniteType) {
				transform.GetChild (0).GetComponent<GhostMachine> ().Attacked (atk);
				transform.GetChild (0).GetComponent<GhostMachine> ().popUpGameObject.GetComponent<GhostMachinePopUp> ().AttackAllGhosts (atk);
			} else {
				foreach (Transform child in transform) {
					child.GetComponent<Ghost> ().Attacked (atk);
				}
			}
		}
	}
}