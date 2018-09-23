using System.Collections;
using System.Collections.Generic;
using Bunkasai2018;
using UnityEngine;

namespace Bunkasai2018 {
	public class Bullet : MonoBehaviour {
		[SerializeField] private float bulletspeed = 13.0f;
		[SerializeField] public int type = 0; //0:player 1: enemy 2:invalid
		public Vector3 targetDirection;
		public float strength = 1;
		public int owner = 0;
		public Display2Manager display2Manager;
		private bool hit = false;

		// Use this for initialization
		void Start () {
			GetComponent<Rigidbody> ().AddForce (targetDirection * bulletspeed);
			Invoke ("FinalDestroy", 2.5f);
		}

		// Update is called once per frame
		void Update () { }

		void OnTriggerEnter (Collider other) {
			if (type == 0) {
				if (other.gameObject.tag == "Ghost") {
					Debug.Log ("ghost");
					//other.GetComponent<Rigidbody> ().AddForce (targetDirection);
					OnBulletDestroy ();
				} else if (other.gameObject.tag == "Enemy Bullet") {
					Destroy (other, 0);
					OnBulletDestroy ();
				} else {
					FinalDestroy ();
					Destroy (gameObject, 0f);
				}
			}
		}

		void OnBulletDestroy () {
			GetComponent<Rigidbody> ().isKinematic = false;
			hit = true;
			type = 2;
			tag = "Invalid Player Bullet";
			GetComponent<MeshRenderer> ().enabled = false;
			transform.GetChild (1).GetComponent<ParticleSystem> ().Play ();
			Destroy (gameObject, 0.2f);
			display2Manager.GunUpdate (owner, true);
		}

		private void FinalDestroy () {
			if (type == 0) {
				if (!hit) {
					display2Manager.GunUpdate (owner, false);
				}
			}
		}
	}
}