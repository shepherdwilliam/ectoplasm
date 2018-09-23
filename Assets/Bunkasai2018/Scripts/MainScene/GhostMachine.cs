using Bunkasai2018;
using DG.Tweening;
using UnityEngine;

namespace Bunkasai2018 {
	public enum GhostMachineState {
		Idle,
		Attack,
		Death
	}

	public class GhostMachine : StatefulObjectBase<GhostMachine, GhostMachineState> {

		[SerializeField] private GameObject ghostPrefab;
		[SerializeField] private GameObject particles;
		[SerializeField] public GameObject popUpGameObject;
		[SerializeField] private GameObject[] handObjects;
		[SerializeField] private GameObject[] dishObjects;
		[SerializeField] private float maxLastBossHP = 50;
		[SerializeField] private float infinitePopUpTime = 2;
		[SerializeField] private float infinitePopUpCycleTime = 5;
		[SerializeField] private float infiniteTimeRandomNess = 1;
		private Vector3 playerPosition;
		private float lastBossHP;
		private float infiniteNextPopUpTime;

		// Use this for initialization
		void Start () {
			if (GameManager.debugMode) {
				lastBossHP = 5;
			} else {
				lastBossHP = maxLastBossHP;
			}
			stateList.Add (new StateIdle (this));
			stateList.Add (new StateAttack (this));
			stateList.Add (new StateDeath (this));
			stateMachine = new StateMachine<GhostMachine> ();
			ChangeState (GhostMachineState.Idle);
		}

		public void StartAttack (Vector3 _playerPosition) {
			playerPosition = _playerPosition;
			playerPosition.y += 0.8f;
			infiniteNextPopUpTime = Time.time + infinitePopUpTime;
			ChangeState (GhostMachineState.Attack);
		}

		public void Attacked (float atk = 1) {
			lastBossHP = lastBossHP - atk;
			if (lastBossHP <= 0) {
				ChangeState (GhostMachineState.Death);
			}
		}

		void OnTriggerEnter (Collider other) {
			if (other.gameObject.tag == "Player Bullet") {
				if (IsCurrentState (GhostMachineState.Idle) || IsCurrentState (GhostMachineState.Attack)) {
					Attacked (other.GetComponent<Bullet> ().strength);
				}
			}
		}

		private class StateIdle : State<GhostMachine> {
			public StateIdle (GhostMachine owner) : base (owner) { }

			public override void Enter () { }

			public override void Execute () { }

			public override void Exit () { }
		}

		private class StateAttack : State<GhostMachine> {
			public StateAttack (GhostMachine owner) : base (owner) { }

			public override void Enter () { }

			public override void Execute () {
				if (owner.lastBossHP > 0) {
					if (Time.time >= owner.infiniteNextPopUpTime) {
						owner.infiniteNextPopUpTime += owner.infinitePopUpCycleTime + Random.Range (-owner.infiniteTimeRandomNess, owner.infiniteTimeRandomNess);
						GameObject ghost = Instantiate (owner.ghostPrefab, owner.popUpGameObject.transform);
						ghost.GetComponent<Ghost> ().PopThisUp (owner.playerPosition, 1, false);
					}
				}
			}

			public override void Exit () { }
		}

		private class StateDeath : State<GhostMachine> {
			public StateDeath (GhostMachine owner) : base (owner) { }

			public override void Enter () {
				owner.particles.GetComponent<ParticleSystem> ().Play ();
				owner.GetComponent<AudioSource> ().Play ();
				owner.popUpGameObject.GetComponent<GhostMachinePopUp> ().KillAllGhosts ();
				foreach (GameObject theobject in owner.handObjects) {
					theobject.GetComponent<HandMoveScript> ().continueMove = false;
				}
				foreach (GameObject theobject in owner.dishObjects) {
					theobject.GetComponent<DishMoveScript> ().continueMove = false;
				}
				int x = 0;
				DOTween.To (() => x, num => x = num, 1, 1.5f).OnComplete (() => {
					foreach (Transform child in owner.transform) {
						var renderer = child.GetComponent<MeshRenderer> ();
						if (renderer != null) {
							renderer.enabled = false;
						}
					}
					owner.transform.parent.GetComponent<ActionPoint> ().OnExit ();
				});
			}

			public override void Execute () { }

			public override void Exit () { }
		}
	}
}