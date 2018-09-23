using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bunkasai2018 {
	public class TextureScript : MonoBehaviour {
		[SerializeField] private Vector2 scale = Vector2.one;

		// Use this for initialization
		void Start () {
			gameObject.GetComponent<Renderer> ().material.mainTextureScale = new Vector2 (transform.localScale.x * scale.x, transform.localScale.y * scale.y);
		}
	}
}