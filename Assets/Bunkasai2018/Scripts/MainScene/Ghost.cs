using Bunkasai2018;
using DG.Tweening;
using UnityEngine;

namespace Bunkasai2018 {
	public enum GhostState {
		Idle,
		PopUp,
		Attack,
		Move,
		Death
	}

	//https://easings.net/ja#

	public class Ghost : StatefulObjectBase<Ghost, GhostState> {

		public Vector3 playerPosition;
		[SerializeField] private GameObject gameManagerObject;
		[SerializeField] public GameObject enemyBulletPrefab;
		[SerializeField] private Material[] materials; //0: normal; 1: death
		[SerializeField] private float maxHP = 1;
		[SerializeField] private float popUpTime = 0;
		[SerializeField] private Vector3 popUpVector = new Vector3 (3, 0, 0);
		private float HP;
		private Vector3 originalPosition;
		[SerializeField] private float speed = 1.5f;
		private float popUpSpeed = 10f;
		[SerializeField] private Vector3 finalScale = new Vector3 (32, 32, 40);
		[SerializeField] private float maxX = 0.8f;
		[SerializeField] private float minX = -0.8f;
		[SerializeField] private float maxY = 3;
		[SerializeField] private float minY = -3;
		[SerializeField] private float maxZ = 0.8f;
		[SerializeField] private float minZ = -0.2f;
		[SerializeField] private bool rotateOnMove = true;
		[SerializeField] private bool lightBool = true;
		private int type = 0; //0: non-infinite normal 1:infinite normal 2: tutorial
		private AudioSource laughAudioSource;
		private AudioSource deathAudioSource;

		// Use this for initialization
		void Start () { }

		public void PopThisUp (Vector3 _playerPosition, int _type, bool tutorial) {
			if (GameManager.debugMode) {
				HP = 1;
			} else {
				HP = maxHP;
			}
			stateList.Add (new StateIdle (this));
			stateList.Add (new StatePopUp (this));
			stateList.Add (new StateAttack (this));
			stateList.Add (new StateMove (this));
			stateList.Add (new StateDeath (this));
			stateMachine = new StateMachine<Ghost> ();
			originalPosition = transform.position + popUpVector;
			laughAudioSource = GetComponents<AudioSource> () [0];
			deathAudioSource = GetComponents<AudioSource> () [1];
			GetComponent<Renderer> ().material = materials[0];
			GetComponent<MeshRenderer> ().enabled = true;
			GetComponent<Light> ().enabled = lightBool;
			GetComponent<CapsuleCollider> ().enabled = true;
			type = _type;
			if (!tutorial) {
				playerPosition = _playerPosition;
				playerPosition.y += 0.8f;
				ChangeState (GhostState.PopUp);
			}
		}

		public void Attacked (float atk = 1) {
			//Debug.Log ("Ghost Attacked");
			if (IsCurrentState (GhostState.PopUp) || IsCurrentState (GhostState.Attack) || IsCurrentState (GhostState.Move) || type == 2) {
				HP = HP - atk;
				if (HP <= 0) {
					ChangeState (GhostState.Death);
				}
			}
		}

		public void Kill () {
			HP = 0;
			ChangeState (GhostState.Death);
		}

		void OnTriggerEnter (Collider other) {
			if (other.gameObject.tag == "Player Bullet") {
				Attacked (other.GetComponent<Bullet> ().strength);
			}
		}

		private class StateIdle : State<Ghost> {

			public StateIdle (Ghost owner) : base (owner) { }

			public override void Enter () { }

			public override void Execute () { }

			public override void Exit () { }
		}

		private class StatePopUp : State<Ghost> {
			private float initTime;
			private Vector3 targetPosition;
			private bool trigger = false;

			public StatePopUp (Ghost owner) : base (owner) { }

			public override void Enter () {
				initTime = Time.time;
				targetPosition = owner.transform.position + owner.popUpVector;
				/*Vector3 rotationVector = Quaternion.LookRotation (owner.playerPosition - owner.transform.position).eulerAngles;
				rotationVector.x = 0;
				rotationVector.z = 0;
				owner.transform.rotation = Quaternion.Euler (rotationVector);*/
			}

			public override void Execute () {
				if (Time.time > initTime + owner.popUpTime && trigger == false) {
					Sequence sequence = DOTween.Sequence ();
					float duration = owner.popUpVector.magnitude / owner.popUpSpeed;
					sequence.Append (owner.transform.DOMove (targetPosition, duration).SetEase (Ease.OutCubic).OnComplete (() => {
						if (owner.HP > 0) {
							owner.ChangeState (GhostState.Attack);
						}
					}));
					sequence.Join (owner.transform.DOScale (owner.finalScale, duration).SetEase (Ease.InQuad));
					owner.laughAudioSource.Play ();
					trigger = true;
				}
			}

			public override void Exit () { }
		}

		private class StateAttack : State<Ghost> {

			public StateAttack (Ghost owner) : base (owner) { }

			public override void Enter () {
				Sequence attackSequence = DOTween.Sequence ();
				Vector3 lookAtVector = owner.playerPosition;
				lookAtVector.y += 8;
				attackSequence.Append (owner.transform.DOLookAt (lookAtVector, 0.6f).SetEase (Ease.OutCubic));
				attackSequence.Append (owner.transform.DOLookAt (owner.playerPosition, 0.3f).SetEase (Ease.OutBack).OnStart (() => {
					if (owner.HP > 0) {
						GameObject enemyBullet = Instantiate (owner.enemyBulletPrefab, owner.transform.position, Quaternion.identity);
						Bullet b = enemyBullet.GetComponent<Bullet> ();
						b.targetDirection = (owner.playerPosition - owner.transform.position).normalized;
						Destroy (enemyBullet, 3f);
					}
				}).OnComplete (() => {
					if (owner.HP > 0) {
						owner.ChangeState (GhostState.Move);
					}
				}));
			}

			public override void Execute () { }

			public override void Exit () { }
		}

		private class StateMove : State<Ghost> {
			public StateMove (Ghost owner) : base (owner) { }

			public override void Enter () {
				Vector3 targetPosition = new Vector3 (Random.Range (owner.minX, owner.maxX), Random.Range (owner.minY, owner.maxY), Random.Range (owner.minZ, owner.maxZ)) + owner.originalPosition;
				Vector3 lookAtVector = targetPosition;
				lookAtVector.y = owner.transform.position.y;
				Sequence moveSequence = DOTween.Sequence ();
				if (owner.rotateOnMove) {
					moveSequence.Append (owner.transform.DOLookAt (lookAtVector, 0.3f).SetEase (Ease.InOutQuad));
					moveSequence.Append (owner.transform.DOMove (targetPosition, (targetPosition - owner.transform.position).magnitude / owner.speed).SetEase (Ease.InOutQuad));
					moveSequence.Append (owner.transform.DOLookAt (owner.playerPosition, 0.3f).SetEase (Ease.InOutQuad).OnComplete (() => {
						if (owner.HP > 0) {
							owner.ChangeState (GhostState.Attack);
						}
					}));
				} else {
					moveSequence.Append (owner.transform.DOMove (targetPosition, (targetPosition - owner.transform.position).magnitude / owner.speed).SetEase (Ease.InOutQuad).OnComplete (() => {
						if (owner.HP > 0) {
							owner.ChangeState (GhostState.Attack);
						}
					}));
					moveSequence.Join (owner.transform.DOLookAt (owner.playerPosition + owner.transform.position - targetPosition, (targetPosition - owner.transform.position).magnitude / owner.speed).SetEase (Ease.Linear));
				}

				/*Vector3 differenceVector = owner.transform.position - owner.player.transform.position;
				radius = differenceVector.magnitude;
				differenceVector.y = 0;
				targetY = Random.Range (owner.originalY - owner.maxDownDistance, owner.originalY + owner.maxUpDistance);
				float angleDir = owner.player.transform.eulerAngles.y * (Mathf.PI / 180.0f);
				Debug.Log ("owner.player.transform.eulerAngles.y" + owner.player.transform.eulerAngles.y);
				nowDegree = Quaternion.FromToRotation (new Vector3 (Mathf.Cos (angleDir), Mathf.Sin (angleDir), 0.0f), differenceVector).eulerAngles.y + owner.maxRightDegree;
				Debug.Log ("nowDegree" + nowDegree);
				//Vector3 touchWorldPosition = owner.gameCamera.ScreenToWorldPoint (new Vector3 (Screen.width / 2, Screen.height / 2, 11.0f));
				//targetObject.transform.position = touchWorldPosition;
				targetDegree = Random.Range (0, owner.maxRightDegree + owner.maxLeftDegree);
				Debug.Log ("targetDegree" + targetDegree);
				float degreeDifference = Mathf.Abs (targetDegree - nowDegree);
				moveSequence.Append (DOTween.To (
					() => nowDegree,
					num => nowDegree = num,
					targetDegree,
					degreeDifference * 0.02f
				).SetEase (Ease.Linear));
				moveSequence.Join (owner.transform.DOMoveY (targetY, degreeDifference * 0.02f).SetEase (Ease.InOutQuad).OnComplete (() => {
					if (owner.HP > 0) {
						owner.ChangeState (GhostState.Attack);
					}
				}));
				moveSequence.Join (owner.transform.DOLookAt (owner.player.transform.position, degreeDifference * 0.02f));*/
			}

			public override void Execute () {
				/* 
				Vector3 deltaTargetPosition = new Vector3 ();
				deltaTargetPosition.x = radius * Mathf.Cos ((owner.player.transform.eulerAngles.z - nowDegree) * Mathf.PI / 180) + owner.player.transform.position.x;
				deltaTargetPosition.z = radius * Mathf.Sin ((owner.player.transform.eulerAngles.z - nowDegree) * Mathf.PI / 180) + owner.player.transform.position.z;
				owner.transform.position = Vector3.MoveTowards (owner.transform.position, deltaTargetPosition, 10 * Time.deltaTime);*/
				//Debug.Log (nowDegree);
			}

			public override void Exit () { }
		}

		private class StateDeath : State<Ghost> {
			public StateDeath (Ghost owner) : base (owner) { }

			public override void Enter () {
				owner.transform.DOKill ();
				if (owner.type == 0) {
					HighScoreManager.thisGameScore.AddScore (1);
					int x = 0;
					DOTween.To (() => x, num => x = num, 1, 2).OnComplete (() => {
						owner.transform.parent.GetComponent<ActionPoint> ().OneGhostKilled ();
					});
				} else if (owner.type == 2) {
					owner.gameManagerObject.GetComponent<GameManager> ().TutorialGhostKilled ();
				}
				foreach (Transform child in owner.transform) {
					child.GetComponent<ParticleSystem> ().Play ();
				}
				owner.deathAudioSource.Play ();
				owner.GetComponent<MeshRenderer> ().enabled = false;
				owner.GetComponent<Light> ().intensity = 0;
				Destroy (owner.gameObject, 3.0f);
			}

			public override void Execute () { }

			public override void Exit () { }
		}
	}
}